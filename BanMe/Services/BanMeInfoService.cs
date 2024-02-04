using BanMe.Data;
using Microsoft.EntityFrameworkCore;

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
			var info = await _context.AppInfo.FirstAsync();
			return info.AppVersion;
		}

		public async Task<string> GetPatch()
		{
			var info = await _context.AppInfo.FirstAsync();
			return info.PatchUsed;
		}

		public async Task<int> GetRecordedGamesCount()
		{
			var info = await _context.AppInfo.FirstAsync();
			return info.RecordedGames;
		}
	}
}
