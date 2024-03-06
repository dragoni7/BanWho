namespace BanWho.Domain.Interfaces;

public interface IBanWhoInfoRepository
{
	Task UpdateRecordedGamesAsync(int numToAdd);

	Task<int> GetRecordedGamesAsync();

	Task<string> GetAppVersionAsync();

	Task<string> GetPatchUsedAsync();
}
