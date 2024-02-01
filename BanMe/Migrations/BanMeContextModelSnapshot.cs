﻿// <auto-generated />
using BanMe.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BanMe.Migrations
{
    [DbContext(typeof(BanMeContext))]
    partial class BanMeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BanMe.Entities.ChampGameStats", b =>
                {
                    b.Property<string>("ChampionName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("BanRate")
                        .HasColumnType("real");

                    b.Property<float>("BotPickRate")
                        .HasColumnType("real");

                    b.Property<float>("BotWinRate")
                        .HasColumnType("real");

                    b.Property<float>("JunglePickRate")
                        .HasColumnType("real");

                    b.Property<float>("JungleWinRate")
                        .HasColumnType("real");

                    b.Property<float>("MidPickRate")
                        .HasColumnType("real");

                    b.Property<float>("MidWinRate")
                        .HasColumnType("real");

                    b.Property<float>("SuppPickRate")
                        .HasColumnType("real");

                    b.Property<float>("SuppWinRate")
                        .HasColumnType("real");

                    b.Property<float>("TopPickRate")
                        .HasColumnType("real");

                    b.Property<float>("TopWinRate")
                        .HasColumnType("real");

                    b.HasKey("ChampionName");

                    b.ToTable("ChampGameStats");
                });

            modelBuilder.Entity("BanMe.Entities.Player", b =>
                {
                    b.Property<string>("PUUID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PUUID");

                    b.ToTable("PlatPuuids");
                });
#pragma warning restore 612, 618
        }
    }
}
