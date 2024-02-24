using BanMe.Application.Util;
using BanMe.Domain.Consts;
using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using BanMe.Infrastructure.Data;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Quartz;

namespace BanMe.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal class UpdateChampGameStatsBackgroundJob : IJob
{
	private readonly IRiotDataCrawler _riotDataCrawler;

	private readonly IPlayerPuuidRepository _playerPuuidRepository;

	private readonly IProcessedMatchesRepository _processedMatchesRepository;

	private readonly IBanMeInfoRepository _banMeInfoRepository;

	private readonly IChampGameStatsRepository _champGameStatsRepository;

	public UpdateChampGameStatsBackgroundJob
		(IRiotDataCrawler riotDataCrawler,
		IPlayerPuuidRepository playerPuuidRepository,
		IProcessedMatchesRepository processedMatchesRepository,
		IBanMeInfoRepository banMeInfoRepository,
		IChampGameStatsRepository champGameStatsRepository)
	{
		_riotDataCrawler = riotDataCrawler;
		_playerPuuidRepository = playerPuuidRepository;
		_processedMatchesRepository = processedMatchesRepository;
		_banMeInfoRepository = banMeInfoRepository;
		_champGameStatsRepository = champGameStatsRepository;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		await SeedChampGameStatsAsync();
	}

	public async Task SeedChampGameStatsAsync()
	{
		// gather match ids from players

		HashSet<string> matchIDsToProcess = new();

		List<Player> playerPuuids = await _playerPuuidRepository.GetAllAsync();

		for (int i = 0; i < 200; i++)
		{
			var playerMatchIDs = await _riotDataCrawler.GatherMatchIDsAsync(playerPuuids.ElementAt(i).PUUID, RegionalRoute.AMERICAS);
			matchIDsToProcess.UnionWith(playerMatchIDs);
		}

		// remove matches already processed
		matchIDsToProcess.RemoveWhere(id => _processedMatchesRepository.ContainsMatchId(id));
		System.Diagnostics.Debug.WriteLine("Found " + matchIDsToProcess.Count() + " unprocessed matches");

		// return if no new matches to process
		if (matchIDsToProcess.Count() == 0)
		{
			return;
		}

		// get new matches
		HashSet<Match> matches = await _riotDataCrawler.CrawlMatchesAsync(matchIDsToProcess, RegionalRoute.AMERICAS);

		// add new matches to processed matches
		foreach (string matchId in matchIDsToProcess)
		{
			_processedMatchesRepository.Add(new ProcessedMatch() { MatchID = matchId });
		}

		// update recorded games
		System.Diagnostics.Debug.WriteLine("Found " + matches.Count + " new matches");

		await _banMeInfoRepository.UpdateRecordedGamesAsync(matches.Count);
		int recordedGames = await _banMeInfoRepository.GetRecordedGamesAsync();

		System.Diagnostics.Debug.WriteLine("Recorded games after to adding = " + recordedGames);

		// remove oldest processed match entries
		_processedMatchesRepository.TrimUnusedMatches();

		// get champ stats from matches
		Dictionary<Champion, FlatChampStats> champData = _riotDataCrawler.GatherChampData(_riotDataCrawler.GatherMatchData(matches));

		// update or add champ data entries
		foreach (var data in champData)
		{
			try
			{
				var entry = _champGameStatsRepository.GetByChampName(data.Key.ToString());

				if (entry == null)
				{
					ChampGameStats newEntry = new()
					{
						ChampionName = data.Key.ToString()
					};

					UpdateChampGameStats(newEntry, data.Value, recordedGames);

					_champGameStatsRepository.Add(newEntry);
				}
				else
				{
					UpdateChampGameStats(entry, data.Value, recordedGames);
				}
			}
			catch (Exception ex)
			{

			}

			await _champGameStatsRepository.SaveAsync();
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
