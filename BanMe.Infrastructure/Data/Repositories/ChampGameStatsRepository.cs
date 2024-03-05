using BanMe.Domain.Consts;
using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Infrastructure.Data.Repositories;
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

	public async Task<ChampGameStats> GetByChampNameAsync(string champName)
	{
		return await _context.ChampGameStats.FirstOrDefaultAsync(e => e.ChampionName == champName);
	}

	public async Task<List<ChampGameStats>> GetByWinRatesAsync(string role, int amount)
	{
		List<ChampGameStats> champGameStats = null;

		switch (role)
		{
			case LeagueConsts.Roles.TOP:
				{
					champGameStats = await _context.ChampGameStats
						.OrderByDescending(entry => entry.TopWinRate > BanMeConsts.StatThresholds.MinWinRate)
						.ThenByDescending(entry => entry.TopPickRate)
						.Take(amount)
						.ToListAsync();

					break;
				}
			case LeagueConsts.Roles.MIDDLE:
				{
					champGameStats = await _context.ChampGameStats
						.OrderByDescending(entry => entry.MidWinRate > BanMeConsts.StatThresholds.MinWinRate)
						.ThenByDescending(entry => entry.MidPickRate)
						.Take(amount)
						.ToListAsync();

					break;
				}
			case LeagueConsts.Roles.JUNGLE:
				{
					champGameStats = await _context.ChampGameStats
						.OrderByDescending(entry => entry.JungleWinRate > BanMeConsts.StatThresholds.MinWinRate)
						.ThenByDescending(entry => entry.JunglePickRate)
						.Take(amount)
						.ToListAsync();
					break;
				}
			case LeagueConsts.Roles.BOTTOM:
				{
					champGameStats = await _context.ChampGameStats
						.OrderByDescending(entry => entry.BotWinRate > BanMeConsts.StatThresholds.MinWinRate)
						.ThenByDescending(entry => entry.BotPickRate)
						.Take(amount)
						.ToListAsync();
					break;
				}
			case LeagueConsts.Roles.SUPPORT:
				{
					champGameStats = await _context.ChampGameStats
						.OrderByDescending(entry => entry.SuppWinRate > BanMeConsts.StatThresholds.MinWinRate)
						.ThenByDescending(entry => entry.SuppPickRate)
						.Take(amount)
						.ToListAsync();
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
