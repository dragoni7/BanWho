using BanMe.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Data
{
    public class BanMeContext : DbContext
    {
        public BanMeContext(DbContextOptions<BanMeContext> options) : base(options)
        {
        }

        public DbSet<ChampGameStats> ChampGameStats { get; set; } = default!;

        public DbSet<Player> PlatPuuids { get; set; } = default!;
    }
}
