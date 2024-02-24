namespace BanMe.Domain.Interfaces;

public interface IBanMeInfoRepository
{
	Task UpdateRecordedGamesAsync(int numToAdd);

	Task<int> GetRecordedGamesAsync();
}
