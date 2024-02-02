namespace BanMe.Services
{
	public interface IBanMeInfoService
	{
		Task<string> GetLatestPatch();
	}
}
