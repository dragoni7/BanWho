using BanMe.Entities;

namespace BanMe.Services
{
    public interface IChampGameStatsService
    {
        Task<List<ChampGameStats>> GetAllChampGameStats();
    }
}
