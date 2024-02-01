using BanMe.Data;
using BanMe.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Services
{
    public class ChampGameStatsService : IChampGameStatsService
    {
        private readonly BanMeContext _context;

        public ChampGameStatsService(BanMeContext context)
        {
            _context = context;
        }

        public async Task<List<ChampGameStats>> GetAllChampGameStats()
        {
            var champGameStats = await _context.ChampGameStats.ToListAsync();
            return champGameStats;
        }
    }
}
