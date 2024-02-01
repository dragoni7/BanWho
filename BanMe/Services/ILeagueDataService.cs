namespace BanMe.Services
{
	public interface ILeagueDataService
	{
		Task<string> GetLatestPatch();
	}
}
