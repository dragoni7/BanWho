using BanWho.Domain.Consts;
using BanWho.Domain.Entities;
using BanWho.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanWho.Infrastructure.Data.Repositories;

internal class ChampMatchupStatsRepository : IChampMatchupStatsRepository
{
	private readonly BanWhoDbContext _context;

	public ChampMatchupStatsRepository(BanWhoDbContext context)
	{
		_context = context;
	}

	public void Add(ChampMatchupStats matchup)
	{
		_context.ChampMatchupStats.Add(matchup);
	}

	public async Task<List<ChampMatchupStats>> GetAllMatchupsByChampAsync(string champ)
	{
		return await _context.ChampMatchupStats.Where(e => e.ChampionName == champ).ToListAsync();
	}

	public async Task<List<ChampMatchupStats>> GetHardestMatchupsByChampAsync(string champ, int amount)
	{
		var champMatchupStats = await _context.ChampMatchupStats.Where(e => e.ChampionName == champ).ToListAsync();

		return champMatchupStats.OrderByDescending(entry => (entry.WinRate > BanWhoConsts.StatThresholds.MinWinRate && entry.WinRate < BanWhoConsts.StatThresholds.MaxWinRate)).ThenByDescending(entry => entry.Picks > BanWhoConsts.StatThresholds.MinPicks).Take(amount).ToList();
	}

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();
	}
}
