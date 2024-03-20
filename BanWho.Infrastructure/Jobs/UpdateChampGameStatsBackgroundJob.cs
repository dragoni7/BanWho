using BanWho.Application.Util;
using BanWho.Domain.Consts;
using BanWho.Domain.Entities;
using BanWho.Domain.Interfaces;
using BanWho.Infrastructure.Data;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BanWho.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal class UpdateChampGameStatsBackgroundJob : IJob
{
	private readonly ILogger<UpdateChampGameStatsBackgroundJob> _logger;

	private readonly IRiotDataCrawler _riotDataCrawler;

	private readonly IPlayerPuuidRepository _playerPuuidRepository;

	private readonly IProcessedMatchesRepository _processedMatchesRepository;

	private readonly IBanWhoInfoRepository _banMeInfoRepository;

	private readonly IChampGameStatsRepository _champGameStatsRepository;

	private readonly IChampMatchupStatsRepository _champMatchupStatsRepository;

	public UpdateChampGameStatsBackgroundJob
		(ILogger<UpdateChampGameStatsBackgroundJob> logger,
		IRiotDataCrawler riotDataCrawler,
		IPlayerPuuidRepository playerPuuidRepository,
		IProcessedMatchesRepository processedMatchesRepository,
		IBanWhoInfoRepository banMeInfoRepository,
		IChampGameStatsRepository champGameStatsRepository,
		IChampMatchupStatsRepository champMatchupStatsRepository)
	{
		_logger = logger;
		_riotDataCrawler = riotDataCrawler;
		_playerPuuidRepository = playerPuuidRepository;
		_processedMatchesRepository = processedMatchesRepository;
		_banMeInfoRepository = banMeInfoRepository;
		_champGameStatsRepository = champGameStatsRepository;
		_champMatchupStatsRepository = champMatchupStatsRepository;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		_logger.LogInformation("----- Starting Update Champ Game Stats Background Job at " + DateTime.UtcNow + " -----\n");
		await SeedChampGameStatsAsync();
		_logger.LogInformation("----- Finished Update Champ Game Stats Background Job at " + DateTime.UtcNow + " -----\n");
	}

	public async Task SeedChampGameStatsAsync()
	{
		List<Player> playerPuuids = await _playerPuuidRepository.GetAllAsync();
		Dictionary<RegionalRoute, HashSet<string>> matchIDsToProcess = new();

		foreach (Player player in playerPuuids)
		{

			var playerMatchIDs = await _riotDataCrawler.GatherMatchIDsAsync(player.PUUID, (RegionalRoute)player.RegionalRoute);

			if (!matchIDsToProcess.ContainsKey((RegionalRoute)player.RegionalRoute))
			{
				matchIDsToProcess.Add((RegionalRoute)player.RegionalRoute, new());
			}

			var newMatchIDs = playerMatchIDs.Where(id => !_processedMatchesRepository.ContainsMatchId(id));

			if (newMatchIDs.Any())
			{
				matchIDsToProcess[(RegionalRoute)player.RegionalRoute].UnionWith(newMatchIDs);
			}
			else
			{
				continue;
			}
		}

		foreach (var pair in matchIDsToProcess)
		{
			_logger.LogInformation($"Found {pair.Value.Count} unprocessed matches for {pair.Key}\n");
		}

		if (matchIDsToProcess.Values.Count == 0)
			return;

		HashSet<Match> matches = new();

		_logger.LogInformation($"Crawling Matches...\n");

		foreach (var matchIDSet in matchIDsToProcess)
		{
			var crawledMatches = await _riotDataCrawler.CrawlMatchesAsync(matchIDSet.Value, matchIDSet.Key);

			matches.UnionWith(crawledMatches);

			_logger.LogInformation($"Marking {matchIDSet.Value.Count} matches as processed.\n");

			foreach (string matchId in matchIDSet.Value)
			{
				_processedMatchesRepository.Add(new ProcessedMatch() { MatchID = matchId });
			}
		}

		_logger.LogInformation($"Found {matches.Count} new matches\n");

		await _banMeInfoRepository.UpdateRecordedGamesAsync(matches.Count);
		int recordedGames = await _banMeInfoRepository.GetRecordedGamesAsync();

		_logger.LogInformation($"Updated recorded games to {recordedGames}\n");

		_processedMatchesRepository.TrimUnusedMatches();

		// get champ stats from matches
		Dictionary<Champion, FlatChampStats> champData = _riotDataCrawler.GatherChampData(_riotDataCrawler.GatherMatchData(matches));

		// update or add champ data entries
		foreach (var data in champData)
		{
			try
			{
				var entry = await _champGameStatsRepository.GetByChampNameAsync(data.Key.ToString());

				if (entry == null)
				{
					ChampGameStats newEntry = new()
					{
						ChampionName = data.Key.ToString()
					};

					UpdateChampGameStats(newEntry, data.Value, recordedGames);
					await UpdateChampMatchupStatsAsync(newEntry, data.Value.MatchUps);

					_champGameStatsRepository.Add(newEntry);

					await _champGameStatsRepository.SaveAsync();
				}
				else
				{
					UpdateChampGameStats(entry, data.Value, recordedGames);
					await UpdateChampMatchupStatsAsync(entry, data.Value.MatchUps);
					await _champGameStatsRepository.SaveAsync();
				}
			}
			catch (Exception)
			{

			}
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
	}

	private async Task UpdateChampMatchupStatsAsync(ChampGameStats entry, Dictionary<Champion, WinStats> matchupStats)
	{
		var existingMatchups = await _champMatchupStatsRepository.GetAllMatchupsByChampAsync(entry.ChampionName);

		foreach (var matchup in matchupStats)
		{
			var m = existingMatchups.FirstOrDefault(e => e.EnemyChampion == matchup.Key.ToString());

			if (m == null)
			{
				_champMatchupStatsRepository.Add(new()
				{ 
					EnemyChampion = matchup.Key.ToString(),
					Wins = matchup.Value.Wins,
					Picks = matchup.Value.Picks,
					WinRate = MathUtil.AsPercentageOf(matchup.Value.Wins, matchup.Value.Picks),
					ChampionName = entry.ChampionName
				});
			}
            else
            {
				m.Wins += matchup.Value.Wins;
				m.Picks += matchup.Value.Picks;
				m.WinRate = MathUtil.AsPercentageOf(m.Wins, m.Picks);
            }

			await _champMatchupStatsRepository.SaveAsync();
		}
	}
}
