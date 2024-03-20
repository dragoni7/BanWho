using BanWho.Domain.Interfaces;

namespace BanWho.Infrastructure.Data.Repositories;

internal class BanWhoInfoRepository : IBanWhoInfoRepository
{
	private readonly BanWhoDbContext _context;

	public BanWhoInfoRepository(BanWhoDbContext context)
	{
		_context = context;
	}

	public async Task<int> GetRecordedGamesAsync()
	{
		var appInfo = await _context.GetBanWhoInfoAsync();
		return appInfo.RecordedGames;
	}

	public async Task UpdateRecordedGamesAsync(int numToAdd)
	{
		var appInfo = await _context.GetBanWhoInfoAsync();
		appInfo.RecordedGames += numToAdd;
	}

	public async Task<string> GetAppVersionAsync()
	{
		var appInfo = await _context.GetBanWhoInfoAsync();
		return appInfo.AppVersion;
	}

	public async Task<string> GetPatchUsedAsync()
	{
		var appInfo = await _context.GetBanWhoInfoAsync();
		return appInfo.PatchUsed;
	}

	public async Task UpdatePatchUsedAsync(string newPatch)
	{
		var appInfo = await _context.GetBanWhoInfoAsync();
		appInfo.PatchUsed = newPatch;
		appInfo.RecordedGames = 0;

		await _context.DumpPatchDataAsync();
	}
}
