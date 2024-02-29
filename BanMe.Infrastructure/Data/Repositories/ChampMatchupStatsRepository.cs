using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Infrastructure.Data.Repositories;

internal class ChampMatchupStatsRepository : IChampMatchupStatsRepository
{
	private readonly BanMeDbContext _context;

	public ChampMatchupStatsRepository(BanMeDbContext context)
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

		return champMatchupStats.OrderByDescending(entry => entry.Picks).ThenByDescending(entry => entry.WinRate).Take(amount).ToList();
	}

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();
	}
}
