using Microsoft.EntityFrameworkCore;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using BanMe.Entities;
using BanMe.Util;
using BanMe.Services;

namespace BanMe.Data
{
    public static class SeedData
    {

        public static async Task InitPlayerDb(IServiceProvider serviceProvider)
        {
			using var dbContext = new BanMeDbContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeDbContext>>());

            var appInfo = await dbContext.GetBanMeInfoAsync();

            var dataCrawler = serviceProvider.GetRequiredService<ILeagueDataCrawler>();

			Tier[] selectedTiers = { Tier.EMERALD, Tier.DIAMOND };

			foreach (Tier tier in selectedTiers)
            {
				var playerPuuids = await dataCrawler.CrawlPlayersAsync(tier, PlatformRoute.NA1);

				foreach (string puuid in playerPuuids)
				{
					dbContext.Add(new Player { PUUID = puuid });
				}
			}

            dbContext.SaveChanges();
        }

        public static async Task InitChampGameStatsDb(IServiceProvider serviceProvider)
        {
			using var dbContext = new BanMeDbContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeDbContext>>());

			var appInfo = await dbContext.GetBanMeInfoAsync();

			var dataCrawler = serviceProvider.GetRequiredService<ILeagueDataCrawler>();

			/*foreach (Player player in context.PlayerPuuids)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(player.PUUID, RegionalRoute.AMERICAS);
                matches.UnionWith(playerMatches);
            }*/


			// gather match ids from players

			HashSet<string> matchIDsToProcess = new();

			for (int i = 0; i < 40; i++)
            {
                var playerMatchIDs = await dataCrawler.GatherMatchIDsAsync(dbContext.PlayerPuuids.ElementAt(i).PUUID, RegionalRoute.AMERICAS);
				matchIDsToProcess.UnionWith(playerMatchIDs);
            }

			// remove matches already processed
			System.Diagnostics.Debug.WriteLine("Removed " + matchIDsToProcess.RemoveWhere(id => dbContext.ProcessedMatches.Any(i => i.MatchID == id)) + " match ids from query.");
            System.Diagnostics.Debug.WriteLine("Found " + matchIDsToProcess.Count() + " new matches");

            // return if no new matches to process
            if (matchIDsToProcess.Count() == 0)
            {
                return;
            }

			// add new matches to processed matches
			foreach (string matchID in matchIDsToProcess)
            {
                dbContext.ProcessedMatches.Add(new ProcessedMatch() { MatchID = matchID });
            }

			// remove oldest processed match entries
			if (dbContext.ProcessedMatches.Count() > dbContext.PlayerPuuids.Count() * 20)
            {
                int toRemove = dbContext.ProcessedMatches.Count() - dbContext.PlayerPuuids.Count() * 20;
				dbContext.ProcessedMatches.RemoveRange(dbContext.ProcessedMatches.TakeLast(toRemove));
			}

            // update recorded games
            appInfo.RecordedGames += matchIDsToProcess.Count();

            // get new matches
			HashSet<Match> matches = await dataCrawler.CrawlMatchesAsync(matchIDsToProcess, RegionalRoute.AMERICAS);

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

        private static void UpdateChampGameStats(ChampGameStats entry, FlatChampStats stats, int recordedGames)
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
