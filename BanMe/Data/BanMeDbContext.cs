using BanMe.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Data
{
	public class BanMeDbContext : DbContext
    {
        public BanMeDbContext(DbContextOptions<BanMeDbContext> options) : base(options)
        {
        }

        public DbSet<ChampGameStats> ChampGameStats { get; set; } = default!;

        public DbSet<Player> PlayerPuuids { get; set; } = default!;

        public DbSet<ProcessedMatch> ProcessedMatches { get; set; } = default!;

        public DbSet<BanMeInfo> AppInfo { get; set; } = default!;

        public async Task<BanMeInfo> GetBanMeInfoAsync()
        {
			return await AppInfo.FirstAsync();
        }

        public async Task DumpPatchDataAsync()
        {
            await Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE [ChampMatchupStats]");
			await Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE [ChampGameStats]");
			await Database.ExecuteSqlRawAsync("TRUNCATE TABLE [PlayerPuuids]");
		}
	}
}
