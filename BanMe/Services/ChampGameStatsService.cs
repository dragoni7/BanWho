using BanMe.Data;
using BanMe.Entities;
using BanMe.Util;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Services
{
    public class ChampGameStatsService : IChampGameStatsService
    {
        private readonly BanMeDbContext _context;

        public ChampGameStatsService(BanMeDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChampGameStats>> GetRecommendedBans(string role, int amount)
        {
			List<ChampGameStats> champGameStats = null;

			switch (role)
            {
                case LeagueConsts.Roles.TOP:
                    {
						var topWins = await _context.ChampGameStats
							.OrderByDescending(entry => entry.TopPickRate)
							.Take(amount)
							.ToListAsync();

						champGameStats = [.. topWins.OrderByDescending(entry => entry.TopWinRate)];

						break;
                    }
				case LeagueConsts.Roles.MIDDLE:
					{
						var topWins = await _context.ChampGameStats
							.OrderByDescending(entry => entry.MidPickRate)
							.Take(amount)
							.ToListAsync();

						champGameStats = [.. topWins.OrderByDescending(entry => entry.MidWinRate)];

						break;
					}
				case LeagueConsts.Roles.JUNGLE:
					{
						var topWins = await _context.ChampGameStats
							.OrderByDescending(entry => entry.JunglePickRate)
							.Take(amount)
							.ToListAsync();

						champGameStats = [.. topWins.OrderByDescending(entry => entry.JungleWinRate)];
						break;
					}
				case LeagueConsts.Roles.BOTTOM:
					{
						var topWins = await _context.ChampGameStats
							.OrderByDescending(entry => entry.BotPickRate)
							.Take(amount)
							.ToListAsync();

						champGameStats = [.. topWins.OrderByDescending(entry => entry.BotWinRate)];
						break;
					}
				case LeagueConsts.Roles.SUPPORT:
					{
						var topWins = await _context.ChampGameStats
							.OrderByDescending(entry => entry.SuppPickRate)
							.Take(amount)
							.ToListAsync();

						champGameStats = [.. topWins.OrderByDescending(entry => entry.SuppWinRate)];
						break;
					}
			}

			return champGameStats;
		}
	}
}
