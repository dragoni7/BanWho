using BanMe.Domain.Interfaces;

namespace BanMe.Infrastructure.Data.Repositories
{
	internal class BanMeInfoRepository : IBanMeInfoRepository
	{
		private readonly BanMeDbContext _context;

		public BanMeInfoRepository(BanMeDbContext context)
		{
			_context = context;
		}

		public async Task<int> GetRecordedGamesAsync()
		{
			var appInfo = await _context.GetBanMeInfoAsync();
			return appInfo.RecordedGames;
		}

		public async Task UpdateRecordedGamesAsync(int numToAdd)
		{
			var appInfo = await _context.GetBanMeInfoAsync();
			appInfo.RecordedGames += numToAdd;
		}
	}
}
