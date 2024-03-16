using BanWho.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanWho.Infrastructure.Data;

public class BanWhoDbContext : DbContext
{
    public BanWhoDbContext(DbContextOptions<BanWhoDbContext> options) : base(options)
    {
    }

    public DbSet<ChampGameStats> ChampGameStats { get; set; } = default!;

    public DbSet<ChampMatchupStats> ChampMatchupStats { get; set; } = default!;

    public DbSet<Player> PlayerPuuids { get; set; } = default!;

    public DbSet<ProcessedMatch> ProcessedMatches { get; set; } = default!;

    public DbSet<BanWhoInfo> AppInfo { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

	}

	public async Task<BanWhoInfo> GetBanWhoInfoAsync()
    {
		return await AppInfo.FirstAsync();
    }

    public async Task DumpPatchDataAsync()
    {
		await Database.ExecuteSqlRawAsync("DELETE FROM [ChampMatchupStats]");
        await ChampGameStats.ExecuteDeleteAsync();

        var appInfo = await GetBanWhoInfoAsync();
        appInfo.RecordedGames = 0;

        await SaveChangesAsync();
	}
}
