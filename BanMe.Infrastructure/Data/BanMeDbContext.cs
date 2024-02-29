using BanMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Infrastructure.Data;

public class BanMeDbContext : DbContext
{
    public BanMeDbContext(DbContextOptions<BanMeDbContext> options) : base(options)
    {
    }

    public DbSet<ChampGameStats> ChampGameStats { get; set; } = default!;

    public DbSet<ChampMatchupStats> ChampMatchupStats { get; set; } = default!;

    public DbSet<Player> PlayerPuuids { get; set; } = default!;

    public DbSet<ProcessedMatch> ProcessedMatches { get; set; } = default!;

    public DbSet<BanMeInfo> AppInfo { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

	}

	public async Task<BanMeInfo> GetBanMeInfoAsync()
    {
		return await AppInfo.FirstAsync();
    }

    public async Task DumpPatchDataAsync()
    {
        System.Diagnostics.Debug.WriteLine("Dumping db");

		await Database.ExecuteSqlRawAsync("DELETE FROM [ChampMatchupStats]");
        await ChampGameStats.ExecuteDeleteAsync();

        var appInfo = await GetBanMeInfoAsync();
        appInfo.RecordedGames = 0;

        SaveChanges();
	}
}
