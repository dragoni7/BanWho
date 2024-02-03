using BanMe.Data;
using BanMe.Entities;
using BanMe.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
						champGameStats = await _context.ChampGameStats
							.OrderByDescending(entry => entry.TopWins)
							.ThenByDescending(entry => entry.TopPicks)
							.Take(amount).ToListAsync();
						break;
                    }
				case LeagueConsts.Roles.MIDDLE:
					{
						champGameStats = await _context.ChampGameStats
							.OrderByDescending(entry => entry.MidWins)
							.ThenByDescending(entry => entry.MidPicks)
							.Take(amount).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.JUNGLE:
					{
						champGameStats = await _context.ChampGameStats
							.OrderByDescending(entry => entry.JungleWins)
							.ThenByDescending(entry => entry.JunglePicks)
							.Take(amount).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.BOTTOM:
					{
						champGameStats = await _context.ChampGameStats
							.OrderByDescending(entry => entry.BotWins)
							.ThenByDescending(entry => entry.BotPicks)
							.Take(amount).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.SUPPORT:
					{
						champGameStats = await _context.ChampGameStats
							.OrderByDescending(entry => entry.SuppWins)
							.ThenByDescending(entry => entry.SuppPicks)
							.Take(amount).ToListAsync();
						break;
					}
			}

			return champGameStats;
		}
	}
}
