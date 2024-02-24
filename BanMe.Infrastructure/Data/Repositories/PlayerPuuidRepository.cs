using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Infrastructure.Data.Repositories
{
	internal class PlayerPuuidRepository : IPlayerPuuidRepository
	{
		private readonly BanMeDbContext _context;

		public PlayerPuuidRepository(BanMeDbContext context)
		{
			_context = context;
		}

		public void Add(Player player)
		{
			_context.PlayerPuuids.Add(player);
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
}
