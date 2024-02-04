using BanMe.Data;

namespace BanMe.Services
{
	public class BanMeInfoService : IBanMeInfoService
	{
		private readonly BanMeDbContext _context;

		public BanMeInfoService(BanMeDbContext context)
		{
			_context = context;
		}

		public async Task<string> GetAppVersion()
		{
			var info = await _context.GetBanMeInfoAsync();
			return info.AppVersion;
		}

		public async Task<string> GetPatch()
		{
			var info = await _context.GetBanMeInfoAsync();
			return info.PatchUsed;
		}

		public async Task<int> GetRecordedGamesCount()
		{
			var info = await _context.GetBanMeInfoAsync();
			return info.RecordedGames;
		}
	}
}
