using BanMe.Data;
using Camille.Enums;
using Camille.RiotGames.MatchV5;

namespace BanMe.Services
{
	public interface ILeagueDataCrawler
	{
		public Task<List<string>> CrawlPlayersAsync(Tier tier, PlatformRoute region);

		public Task<string[]> GatherMatchIDsAsync(string playerPuuid, RegionalRoute region);

		public Task<HashSet<Match>> CrawlMatchesAsync(HashSet<string> matchIDs, RegionalRoute region);

		public List<MatchData> GatherMatchData(HashSet<Match> matches);

		public Dictionary<Champion, FlatChampStats> GatherChampData(List<MatchData> matchesData);
	}
}
