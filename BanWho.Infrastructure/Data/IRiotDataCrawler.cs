﻿using BanWho.Domain.Entities;
using Camille.Enums;
using Camille.RiotGames.MatchV5;

namespace BanWho.Infrastructure.Data;

internal interface IRiotDataCrawler
{
    public Task<HashSet<string>> CrawlPlayersAsync(Tier tier, PlatformRoute region);

    public Task<string[]> GatherMatchIDsAsync(string playerPuuid, RegionalRoute region);

    public Task<HashSet<Match>> CrawlMatchesAsync(HashSet<string> matchIDs, RegionalRoute region);

    public List<MatchData> GatherMatchData(HashSet<Match> matches);

    public Dictionary<Champion, FlatChampStats> GatherChampData(List<MatchData> matchesData);
}
