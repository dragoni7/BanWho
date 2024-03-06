using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanWho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppInfo",
                columns: table => new
                {
                    AppVersion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatchUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedGames = table.Column<int>(type: "int", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInfo", x => x.AppVersion);
                });

            migrationBuilder.CreateTable(
                name: "ChampGameStats",
                columns: table => new
                {
                    ChampionName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TopWins = table.Column<int>(type: "int", nullable: false),
                    TopWinRate = table.Column<float>(type: "real", nullable: false),
                    TopPicks = table.Column<int>(type: "int", nullable: false),
                    TopPickRate = table.Column<float>(type: "real", nullable: false),
                    MidWins = table.Column<int>(type: "int", nullable: false),
                    MidWinRate = table.Column<float>(type: "real", nullable: false),
                    MidPicks = table.Column<int>(type: "int", nullable: false),
                    MidPickRate = table.Column<float>(type: "real", nullable: false),
                    JungleWins = table.Column<int>(type: "int", nullable: false),
                    JungleWinRate = table.Column<float>(type: "real", nullable: false),
                    JunglePicks = table.Column<int>(type: "int", nullable: false),
                    JunglePickRate = table.Column<float>(type: "real", nullable: false),
                    BotWins = table.Column<int>(type: "int", nullable: false),
                    BotWinRate = table.Column<float>(type: "real", nullable: false),
                    BotPicks = table.Column<int>(type: "int", nullable: false),
                    BotPickRate = table.Column<float>(type: "real", nullable: false),
                    SuppWins = table.Column<int>(type: "int", nullable: false),
                    SuppWinRate = table.Column<float>(type: "real", nullable: false),
                    SuppPicks = table.Column<int>(type: "int", nullable: false),
                    SuppPickRate = table.Column<float>(type: "real", nullable: false),
                    Bans = table.Column<int>(type: "int", nullable: false),
                    BanRate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampGameStats", x => x.ChampionName);
                });

            migrationBuilder.CreateTable(
                name: "ChampMatchupStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnemyChampion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    WinRate = table.Column<float>(type: "real", nullable: false),
                    Picks = table.Column<int>(type: "int", nullable: false),
                    ChampionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampMatchupStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPuuids",
                columns: table => new
                {
                    PUUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegionalRoute = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPuuids", x => x.PUUID);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedMatches",
                columns: table => new
                {
                    MatchID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMatches", x => x.MatchID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInfo");

            migrationBuilder.DropTable(
                name: "ChampGameStats");

            migrationBuilder.DropTable(
                name: "ChampMatchupStats");

            migrationBuilder.DropTable(
                name: "PlayerPuuids");

            migrationBuilder.DropTable(
                name: "ProcessedMatches");
        }
    }
}
