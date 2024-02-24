using BanMe.Domain.Consts;
using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Infrastructure.Data.Repositories
{
	internal class ChampGameStatsRepository : IChampGameStatsRepository
    {
        private readonly BanMeDbContext _context;

        public ChampGameStatsRepository(BanMeDbContext context)
        {
            _context = context;
        }

        public void Add(ChampGameStats entity)
        {
            _context.ChampGameStats.Add(entity);
        }

		public ChampGameStats? GetByChampName(string champName)
		{
			return _context.ChampGameStats.FirstOrDefault(e => e.ChampionName == champName);
		}

		public async Task<List<ChampGameStats>> GetByWinRatesAsync(string role, int amount)
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

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
