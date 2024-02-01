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

        public async Task<List<ChampGameStats>> GetRecommendedBans(string role, int amount)
        {
			List<ChampGameStats> champGameStats = null;

			switch (role)
            {
                case LeagueConsts.Roles.TOP:
                    {
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.TopPickRate).Take(amount).OrderByDescending(entry => entry.TopWinRate).ToListAsync();
						break;
                    }
				case LeagueConsts.Roles.MIDDLE:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.MidPickRate).Take(amount).OrderByDescending(entry => entry.MidWinRate).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.JUNGLE:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.JunglePickRate).Take(amount).OrderByDescending(entry => entry.JungleWinRate).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.BOTTOM:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.BotPickRate).Take(amount).OrderByDescending(entry => entry.BotWinRate).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.SUPPORT:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.SuppPickRate).Take(amount).OrderByDescending(entry => entry.SuppWinRate).ToListAsync();
						break;
					}
			}

			return champGameStats;
		}
	}
}
