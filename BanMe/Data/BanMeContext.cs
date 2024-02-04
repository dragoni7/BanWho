﻿using BanMe.Entities;
using BanMe.Services;
using Microsoft.EntityFrameworkCore;

namespace BanMe.Data
{
    public class BanMeContext : DbContext
    {
        public BanMeContext(DbContextOptions<BanMeContext> options) : base(options)
        {
        }

        public DbSet<ChampGameStats> ChampGameStats { get; set; } = default!;

        public DbSet<Player> PlayerPuuids { get; set; } = default!;

        public DbSet<ProcessedMatch> ProcessedMatches { get; set; } = default!;

        public DbSet<BanMeInfo> AppInfo { get; set; } = default!;
	}
}
