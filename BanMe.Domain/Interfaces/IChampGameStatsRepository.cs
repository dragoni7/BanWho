using BanMe.Domain.Entities;

namespace BanMe.Domain.Interfaces;

public interface IChampGameStatsRepository
{
    void Add(ChampGameStats entity);

	public Task<ChampGameStats> GetByChampNameAsync(string champName);

	public Task<List<ChampGameStats>> GetByWinRatesAsync(string role, int amount);

    public Task SaveAsync();
}
