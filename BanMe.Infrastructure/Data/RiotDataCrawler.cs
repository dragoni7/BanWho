using BanMe.Domain.Entities;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using Camille.RiotGames;
using System.Net.Http.Json;
using Camille.RiotGames.LeagueV4;
using BanMe.Domain.Consts;

namespace BanMe.Infrastructure.Data;

internal class RiotDataCrawler : IRiotDataCrawler
{
    private readonly BanMeDbContext _context;

    public RiotDataCrawler(BanMeDbContext context)
    {
        _context = context;
    }

    public async Task<List<string>> CrawlPlayersAsync(Tier tier, PlatformRoute region)
    {
        HashSet<LeagueEntry> tierEntries = new();

        RiotGamesApi riotApi = RiotGamesApi.NewInstance(_context.AppInfo.First().ApiKey);

        int lowDivision = tier == Tier.EMERALD ? 4 : 5;

        for (int i = 1; i < lowDivision; i++)
        {
            var entries = await riotApi.LeagueV4().GetLeagueEntriesAsync(region, QueueType.RANKED_SOLO_5x5, tier, (Division)i);
            tierEntries.UnionWith(entries.Where(e => e.Inactive == false));
        }

        List<string> puuids = new();

        foreach (LeagueEntry entry in tierEntries)
        {
            var summoner = await riotApi.SummonerV4().GetBySummonerIdAsync(PlatformRoute.NA1, entry.SummonerId);
            System.Diagnostics.Debug.Print("summoner puuid: " + summoner);
            puuids.Add(summoner.Puuid);
        }

        return puuids;
    }

    public async Task<string[]> GatherMatchIDsAsync(string playerPuuid, RegionalRoute region)
    {
        RiotGamesApi riotApi = RiotGamesApi.NewInstance(_context.AppInfo.First().ApiKey);

        return await riotApi.MatchV5().GetMatchIdsByPUUIDAsync(region, playerPuuid);
    }

    public async Task<HashSet<Match>> CrawlMatchesAsync(HashSet<string> matchIDs, RegionalRoute region)
    {
        HashSet<Match> matchSet = new();

        RiotGamesApi riotApi = RiotGamesApi.NewInstance(_context.AppInfo.First().ApiKey);

        foreach (string id in matchIDs)
        {
            Match match = await riotApi.MatchV5().GetMatchAsync(region, id);
            string[] matchPatchStr = match.Info.GameVersion.Split(".");
            int matchPatchNumMajor = Convert.ToInt32(matchPatchStr[0]);
            int matchPatchNumMinor = Convert.ToInt32(matchPatchStr[1]);

            var appInfo = await _context.GetBanMeInfoAsync();
            string[] currentPatchStr = appInfo.PatchUsed.Split(".");
            int currentPatchMajor = Convert.ToInt32(currentPatchStr[0]);
            int currentPatchMinor = Convert.ToInt32(currentPatchStr[1]);

            if (matchPatchNumMajor > currentPatchMajor || matchPatchNumMajor == currentPatchMajor && matchPatchNumMinor > currentPatchMinor)
            {
                // new patch
                matchSet = new();

                string fetchedPatch = "";

                using (HttpClient client = new())
                {
                    var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
                    fetchedPatch = json.First();
                }

                appInfo.PatchUsed = fetchedPatch;

                await _context.DumpPatchDataAsync();

                matchSet.Add(match);
            }
            else if (matchPatchNumMajor == currentPatchMajor && matchPatchNumMinor == currentPatchMinor)
            {
                matchSet.Add(match);
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
