using BanWho.Domain.Entities;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames;
using System.Net.Http.Json;
using Camille.RiotGames.LeagueV4;
using BanWho.Domain.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BanWho.Infrastructure.Data.Repositories;

namespace BanWho.Infrastructure.Data;

internal class RiotDataCrawler : IRiotDataCrawler
{
    private readonly BanWhoInfoRepository _banWhoInfoRepository;

	private readonly ILogger<RiotDataCrawler> _logger;

    private readonly IConfiguration _config;

    private const string apiKeySection = "RIOTAPI_KEY";

	public RiotDataCrawler(BanWhoDbContext context, BanWhoInfoRepository banWhoInfoRepository, ILogger<RiotDataCrawler> logger, IConfiguration config)
    {
        _banWhoInfoRepository = banWhoInfoRepository;
		_logger = logger;
		_config = config;
    }

    public async Task<HashSet<string>> CrawlPlayersAsync(Tier tier, PlatformRoute region)
    {
		RiotGamesApi riotApi = RiotGamesApi.NewInstance(_config.GetRequiredSection(apiKeySection).Value);
		
        HashSet<string> puuids = new();

		switch (tier)
        {
            case Tier.MASTER:
                {
					var entries = await riotApi.LeagueV4().GetMasterLeagueAsync(region, QueueType.RANKED_SOLO_5x5);

                    puuids = await GetPuuidsFromLeagueListAsync(riotApi, entries, region);

					break;
				}
            case Tier.GRANDMASTER:
                {
					var entries = await riotApi.LeagueV4().GetGrandmasterLeagueAsync(region, QueueType.RANKED_SOLO_5x5);

					puuids = await GetPuuidsFromLeagueListAsync(riotApi, entries, region);

					break;
				}
			case Tier.CHALLENGER:
				{
					var entries = await riotApi.LeagueV4().GetChallengerLeagueAsync(region, QueueType.RANKED_SOLO_5x5);

					puuids = await GetPuuidsFromLeagueListAsync(riotApi, entries, region);

					break;
				}
			default:
                {
					HashSet<LeagueEntry> tierEntries = new();

					int lowDivision = tier == Tier.EMERALD ? 4 : 5;

					for (int i = 1; i < lowDivision; i++)
					{
						var entries = await riotApi.LeagueV4().GetLeagueEntriesAsync(region, QueueType.RANKED_SOLO_5x5, tier, (Division)i);
						tierEntries.UnionWith(entries.Where(e => e.Inactive == false));
					}

					foreach (LeagueEntry entry in tierEntries)
					{
						var summoner = await riotApi.SummonerV4().GetBySummonerIdAsync(region, entry.SummonerId);
						puuids.Add(summoner.Puuid);
					}

					break;
				}
        }

        return puuids;
    }

    public async Task<string[]> GatherMatchIDsAsync(string playerPuuid, RegionalRoute region)
    {
		RiotGamesApi riotApi = RiotGamesApi.NewInstance(_config.GetRequiredSection(apiKeySection).Value);

        return await riotApi.MatchV5().GetMatchIdsByPUUIDAsync(region, playerPuuid);
    }

    public async Task<HashSet<Match>> CrawlMatchesAsync(HashSet<string> matchIDs, RegionalRoute region)
    {
        HashSet<Match> matchSet = new();

		RiotGamesApi riotApi = RiotGamesApi.NewInstance(_config.GetRequiredSection(apiKeySection).Value);

        foreach (string id in matchIDs)
        {
            try
            {
				Match match = await riotApi.MatchV5().GetMatchAsync(region, id);
                string patchUsed = await _banWhoInfoRepository.GetPatchUsedAsync();

				if (match != null)
                {
					string[] matchPatchStr = match.Info.GameVersion.Split(".");
                    int matchPatchNumMajor = Convert.ToInt32(matchPatchStr[0]);
                    int matchPatchNumMinor = Convert.ToInt32(matchPatchStr[1]);

                    string[] currentPatchStr = patchUsed.Split(".");
                    int currentPatchMajor = Convert.ToInt32(currentPatchStr[0]);
                    int currentPatchMinor = Convert.ToInt32(currentPatchStr[1]);

                    bool isPatchNew = matchPatchNumMajor > currentPatchMajor || (matchPatchNumMajor == currentPatchMajor && matchPatchNumMinor > currentPatchMinor);
                    bool isPatchSame = matchPatchNumMajor == currentPatchMajor && matchPatchNumMinor == currentPatchMinor;


					if (isPatchNew)
                    {
                        matchSet = new();

                        string fetchedPatch = "";

                        using (HttpClient client = new())
                        {
                            var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
                            fetchedPatch = json.First();
                        }

						string[] latestPatchStr = fetchedPatch.Split(".");
						int latestPatchMajor = Convert.ToInt32(latestPatchStr[0]);
						int latestPatchMinor = Convert.ToInt32(latestPatchStr[1]);

                        bool isPatchAhead = matchPatchNumMajor > latestPatchMajor || (matchPatchNumMajor == latestPatchMajor && matchPatchNumMinor > latestPatchMinor);

                        if (!isPatchAhead)
                        {
							_logger.LogInformation($"Dumping database from old patch {patchUsed} for new patch {fetchedPatch}");

                            await _banWhoInfoRepository.UpdatePatchUsedAsync(fetchedPatch);

							matchSet.Add(match);
						}
                    }
                    else if (isPatchSame)
                    {
                        matchSet.Add(match);
                    }
                }
			}
            catch
            {
                // error with new gamemode? wait for Camille update.
                continue;
            }
        }

        return matchSet;
    }

    public List<MatchData> GatherMatchData(HashSet<Match> matches)
    {
        List<MatchData> matchData = new();

        foreach (Match match in matches)
        {
            MatchData mData = new(match.Info.Teams[0].Win);

            bool badEntry = false;

            foreach (Participant participant in match.Info.Participants)
            {
                if (participant.TeamPosition == string.Empty)
                {
                    badEntry = true;
                    break;
                }

                if (participant.TeamId == match.Info.Teams[0].TeamId)
                {
                    mData.SetTeamAChampion(participant);
                }
                else
                {
                    mData.SetTeamBChampion(participant);
                }
            }

            if (badEntry)
            {
                continue;
            }

            foreach (Team team in match.Info.Teams)
            {
                foreach (Ban ban in team.Bans)
                {
                    mData.Bans.Add(ban.ChampionId);
                }
            }

            matchData.Add(mData);
        }

        return matchData;
    }

    public Dictionary<Champion, FlatChampStats> GatherChampData(List<MatchData> matchesData)
    {
        Dictionary<Champion, FlatChampStats> champStats = new();

        foreach (MatchData matchData in matchesData)
        {
            // process bans
            foreach (Champion banned in matchData.Bans)
            {
                if (champStats.TryGetValue(banned, out FlatChampStats stats))
                {
                    stats.Bans++;
                    champStats[banned] = stats;
                }
                else
                {
                    FlatChampStats newStats = new();
                    newStats.Bans++;
                    champStats.Add(banned, newStats);
                }
            }

            // process teams
            GetDataFromTeams(matchData.TeamA, matchData.TeamB, champStats);
        }

        return champStats;
    }

    private async Task<HashSet<string>> GetPuuidsFromLeagueListAsync(RiotGamesApi riotApi, LeagueList list, PlatformRoute platformRoute)
    {
		HashSet<string> puuids = new();

		foreach (LeagueItem entry in list.Entries.Where(e => e.Inactive == false))
		{
			var summoner = await riotApi.SummonerV4().GetBySummonerIdAsync(platformRoute, entry.SummonerId);
			puuids.Add(summoner.Puuid);
		}

        return puuids;
	}

    private void GetDataFromTeams(TeamData teamAData, TeamData teamBData, Dictionary<Champion, FlatChampStats> champStats)
    {
        foreach (string role in LeagueConsts.Roles.ALL)
        {
            teamAData.ChampRole.TryGetValue(role, out Champion champA);
            teamBData.ChampRole.TryGetValue(role, out Champion champB);

            UpdateChampRoleStats(champStats, role, champA, champB, teamAData.Win);
            UpdateChampRoleStats(champStats, role, champB, champA, teamBData.Win);
        }
    }

    private void UpdateChampRoleStats(Dictionary<Champion, FlatChampStats> champStats, string role, Champion champ, Champion enemyChamp, bool won)
    {
        if (champStats.TryGetValue(champ, out FlatChampStats stats))
        {
            stats.RoleStats.TryGetValue(role, out WinStats roleStats);

            roleStats.Wins += won ? 1 : 0;
            roleStats.Picks++;

            stats.RoleStats[role] = roleStats;

            UpdateChampMatchupStats(stats, enemyChamp, won);

            champStats[champ] = stats;
        }
        else
        {
            FlatChampStats newStats = new();

            newStats.RoleStats.TryGetValue(role, out WinStats roleStats);

            roleStats.Wins += won ? 1 : 0;
            roleStats.Picks++;

            newStats.RoleStats[role] = roleStats;

            UpdateChampMatchupStats(newStats, enemyChamp, won);

            champStats.Add(champ, newStats);
        }
    }

    private void UpdateChampMatchupStats(FlatChampStats stats, Champion enemyChamp, bool won)
    {
        if (stats.MatchUps.TryGetValue(enemyChamp, out WinStats matchupStats))
        {
            matchupStats.Wins += won ? 1 : 0;
            matchupStats.Picks++;

            stats.MatchUps[enemyChamp] = matchupStats;
        }
        else
        {
            WinStats newWinStats = new() { Wins = 0, Picks = 0 };
            newWinStats.Wins += won ? 1 : 0;
            newWinStats.Picks++;

            stats.MatchUps.Add(enemyChamp, newWinStats);
        }
    }
}
