using Azure.Core;
using BanMe.Data;
using BanMe.Entities;
using BanMe.Util;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Services
{
	public class DbSeeder : IDbSeeder
	{

		private readonly IServiceProvider _serviceProvider;

		public DbSeeder(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task SeedPlayerPuuidsAsync()
		{
			using IServiceScope scope = _serviceProvider.CreateScope();

			var dbContext = scope.ServiceProvider.GetRequiredService<BanMeDbContext>();

			var dataCrawler = _serviceProvider.GetRequiredService<ILeagueDataCrawler>();

			Tier[] selectedTiers = { Tier.EMERALD, Tier.DIAMOND };

			foreach (Tier tier in selectedTiers)
			{
				var playerPuuids = await dataCrawler.CrawlPlayersAsync(tier, PlatformRoute.NA1);

				foreach (string puuid in playerPuuids)
				{
					dbContext.Add(new Player { PUUID = puuid });
				}

				await dbContext.SaveChangesAsync();
			}
		}

		public async Task SeedChampGameStatsAsync()
		{
			using IServiceScope scope = _serviceProvider.CreateScope();

			var dbContext = scope.ServiceProvider.GetRequiredService<BanMeDbContext>();

			var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>();

			if (! await dbContext.PlayerPuuids.AnyAsync())
			{
				return;
			}

			var appInfo = await dbContext.GetBanMeInfoAsync();

			var dataCrawler = _serviceProvider.GetRequiredService<ILeagueDataCrawler>();

			/*foreach (Player player in context.PlayerPuuids)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(player.PUUID, RegionalRoute.AMERICAS);
                matches.UnionWith(playerMatches);
            }*/


			// gather match ids from players

			HashSet<string> matchIDsToProcess = new();

			for (int i = 0; i < 100; i++)
			{
				var playerMatchIDs = await dataCrawler.GatherMatchIDsAsync(dbContext.PlayerPuuids.ElementAt(i).PUUID, RegionalRoute.AMERICAS);
				matchIDsToProcess.UnionWith(playerMatchIDs);
			}

			// remove matches already processed
			matchIDsToProcess.RemoveWhere(id => dbContext.ProcessedMatches.Any(i => i.MatchID == id));
			System.Diagnostics.Debug.WriteLine("Found " + matchIDsToProcess.Count() + " unprocessed matches");

			// return if no new matches to process
			if (matchIDsToProcess.Count() == 0)
			{
				return;
			}

			// get new matches
			HashSet<Match> matches = await dataCrawler.CrawlMatchesAsync(matchIDsToProcess, RegionalRoute.AMERICAS);

			// add new matches to processed matches
			foreach (string matchId in matchIDsToProcess)
			{
				dbContext.ProcessedMatches.Add(new ProcessedMatch() { MatchID = matchId });
			}

			// update recorded games
			System.Diagnostics.Debug.WriteLine("Found " + matches.Count + " new matches");
			//System.Diagnostics.Debug.WriteLine("Recorded games prior to adding = " + appInfo.RecordedGames);
			appInfo.RecordedGames += matches.Count;
			System.Diagnostics.Debug.WriteLine("Recorded games after to adding = " + appInfo.RecordedGames);

			// remove oldest processed match entries
			if (dbContext.ProcessedMatches.Count() > dbContext.PlayerPuuids.Count() * 20)
			{
				int toRemove = dbContext.ProcessedMatches.Count() - dbContext.PlayerPuuids.Count() * 20;
				dbContext.ProcessedMatches.RemoveRange(dbContext.ProcessedMatches.TakeLast(toRemove));
			}

			// get champ stats from matches
			Dictionary<Champion, FlatChampStats> champData = dataCrawler.GatherChampData(dataCrawler.GatherMatchData(matches));

			// update or add champ data entries
			foreach (var data in champData)
			{
				try
				{
					var entry = dbContext.ChampGameStats.FirstOrDefault(e => e.ChampionName == data.Key.ToString());

					if (entry == null)
					{
						ChampGameStats newEntry = new()
						{
							ChampionName = data.Key.ToString()
						};

						UpdateChampGameStats(newEntry, data.Value, appInfo.RecordedGames);

						dbContext.ChampGameStats.Add(newEntry);
					}
					else
					{
						UpdateChampGameStats(entry, data.Value, appInfo.RecordedGames);
					}
				}
				catch (Exception ex)
				{

				}
				dbContext.SaveChanges();
			}
		}

		private void UpdateChampGameStats(ChampGameStats entry, FlatChampStats stats, int recordedGames)
		{
			entry.TopWins += stats.RoleStats[LeagueConsts.Roles.TOP].Wins;
			entry.TopPicks += stats.RoleStats[LeagueConsts.Roles.TOP].Picks;
			entry.MidWins += stats.RoleStats[LeagueConsts.Roles.MIDDLE].Wins;
			entry.MidPicks += stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks;
			entry.JungleWins += stats.RoleStats[LeagueConsts.Roles.JUNGLE].Wins;
			entry.JunglePicks += stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks;
			entry.BotWins += stats.RoleStats[LeagueConsts.Roles.BOTTOM].Wins;
			entry.BotPicks += stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks;
			entry.SuppWins += stats.RoleStats[LeagueConsts.Roles.SUPPORT].Wins;
			entry.SuppPicks += stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks;
			entry.Bans += stats.Bans;

			entry.TopWinRate = MathUtil.AsPercentageOf(entry.TopWins, entry.TopPicks);
			entry.TopPickRate = MathUtil.AsPercentageOf(entry.TopPicks, recordedGames);

			entry.MidWinRate = MathUtil.AsPercentageOf(entry.MidWins, entry.MidPicks);
			entry.MidPickRate = MathUtil.AsPercentageOf(entry.MidPicks, recordedGames);

			entry.JungleWinRate = MathUtil.AsPercentageOf(entry.JungleWins, entry.JunglePicks);
			entry.JunglePickRate = MathUtil.AsPercentageOf(entry.JunglePicks, recordedGames);

			entry.BotWinRate = MathUtil.AsPercentageOf(entry.BotWins, entry.BotPicks);
			entry.BotPickRate = MathUtil.AsPercentageOf(entry.BotPicks, recordedGames);

			entry.SuppWinRate = MathUtil.AsPercentageOf(entry.SuppWins, entry.SuppPicks);
			entry.SuppPickRate = MathUtil.AsPercentageOf(entry.SuppPicks, recordedGames);

			entry.BanRate = MathUtil.AsPercentageOf(entry.Bans, recordedGames);

			foreach (var matchup in stats.MatchUps)
			{
				var m = entry.MatchupStats.FirstOrDefault(m => m.EnemyChampion == matchup.Key.ToString());

				if (m != null)
				{
					int index = entry.MatchupStats.IndexOf(m);
					entry.MatchupStats.ElementAt(index).Wins += matchup.Value.Wins;
					entry.MatchupStats.ElementAt(index).Picks += matchup.Value.Picks;
				}
				else
				{
					entry.MatchupStats.Add(new ChampMatchupStats()
					{
						EnemyChampion = matchup.Key.ToString(),
						Wins = matchup.Value.Wins,
						Picks = matchup.Value.Picks,
						WinRate = MathUtil.AsPercentageOf(matchup.Value.Wins, matchup.Value.Picks)
					});
				}
			}
		}
	}
}
