using BanMe.Domain.Entities;

namespace BanMe.Domain.Interfaces;

public interface IChampGameStatsRepository
{
    void Add(ChampGameStats entity);

    public ChampGameStats? GetByChampName(string champName);

	Task<List<ChampGameStats>> GetByWinRatesAsync(string role, int amount);

    public Task SaveAsync();
}
