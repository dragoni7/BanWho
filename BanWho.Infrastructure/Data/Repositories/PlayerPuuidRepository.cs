using BanWho.Domain.Entities;
using BanWho.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanWho.Infrastructure.Data.Repositories;

internal class PlayerPuuidRepository : IPlayerPuuidRepository
{
	private readonly BanWhoDbContext _context;

	public PlayerPuuidRepository(BanWhoDbContext context)
	{
		_context = context;
	}

	public async Task AddAsync(Player player)
	{
		await _context.PlayerPuuids.AddAsync(player);
	}

	public async Task ClearAsync()
	{
		await _context.PlayerPuuids.ExecuteDeleteAsync();
	}

	public int Count()
	{
		return _context.PlayerPuuids.Count();
	}

	public async Task<List<Player>> GetAllAsync()
	{
		return await _context.PlayerPuuids.ToListAsync();
	}

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();
	}
}
