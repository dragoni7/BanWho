namespace BanMe.Services
{
	public interface IBanMeInfoService
	{
		Task<string> GetAppVersion();

		Task<string> GetPatch();

		Task<int> GetRecordedGamesCount();
	}
}
