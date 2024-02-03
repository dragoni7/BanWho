using BanMe.Data;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Services
{
	public class BanMeInfoService : IBanMeInfoService
	{
		private readonly BanMeContext _context;

		public BanMeInfoService(BanMeContext context)
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
