using Camille.Enums;
using Camille.RiotGames;
using Camille.RiotGames.LeagueV4;
using Camille.RiotGames.MatchV5;

namespace BanMe.Data
{
	public class LeagueDataCrawler
    {
        private readonly RiotGamesApi riotApiInstance;

        public LeagueDataCrawler(string apiKey)
        {
            riotApiInstance = RiotGamesApi.NewInstance(apiKey);
		}

        public async Task<List<string>> CrawlPlayersAsync(Tier tier, PlatformRoute region)
        {
            HashSet<LeagueEntry> tierEntries = new();

            for (int i = 1; i < 5; i++)
            {
                var entries = await riotApiInstance.LeagueV4().GetLeagueEntriesAsync(region, QueueType.RANKED_SOLO_5x5, tier, (Division)i);
                tierEntries.UnionWith(entries.Where(e => e.Inactive == false));
			}

            List<string> puuids = new();

            foreach (LeagueEntry entry in tierEntries)
            {
                var summoner = await riotApiInstance.SummonerV4().GetBySummonerIdAsync(PlatformRoute.NA1, entry.SummonerId);
                puuids.Add(summoner.Puuid);
            }

            return puuids;
        }

        public async Task<HashSet<Match>> CrawlMatchesAsync(string puuid, RegionalRoute region)
        {
            HashSet<Match> matchSet = new();

            string[] matches = await riotApiInstance.MatchV5().GetMatchIdsByPUUIDAsync(region, puuid);

            foreach (string id in matches)
            {
                Match match = await riotApiInstance.MatchV5().GetMatchAsync(region, id);
                matchSet.Add(match);
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
}
