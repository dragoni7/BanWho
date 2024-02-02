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
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.TopPicks).Take(amount).OrderByDescending(entry => entry.TopWins).ToListAsync();
						break;
                    }
				case LeagueConsts.Roles.MIDDLE:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.MidPicks).Take(amount).OrderByDescending(entry => entry.MidWins).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.JUNGLE:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.JunglePicks).Take(amount).OrderByDescending(entry => entry.JungleWins).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.BOTTOM:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.BotPicks).Take(amount).OrderByDescending(entry => entry.BotWins).ToListAsync();
						break;
					}
				case LeagueConsts.Roles.SUPPORT:
					{
						champGameStats = await _context.ChampGameStats.OrderByDescending(entry => entry.SuppPicks).Take(amount).OrderByDescending(entry => entry.SuppWins).ToListAsync();
						break;
					}
			}

			return champGameStats;
		}
	}
}
