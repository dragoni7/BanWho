using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanMe.Migrations
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
                    TopPicks = table.Column<int>(type: "int", nullable: false),
                    MidWins = table.Column<int>(type: "int", nullable: false),
                    MidPicks = table.Column<int>(type: "int", nullable: false),
                    JungleWins = table.Column<int>(type: "int", nullable: false),
                    JunglePicks = table.Column<int>(type: "int", nullable: false),
                    BotWins = table.Column<int>(type: "int", nullable: false),
                    BotPicks = table.Column<int>(type: "int", nullable: false),
                    SuppWins = table.Column<int>(type: "int", nullable: false),
                    SuppPicks = table.Column<int>(type: "int", nullable: false),
                    Bans = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampGameStats", x => x.ChampionName);
                });
            
            migrationBuilder.CreateTable(
                name: "PlayerPuuids",
                columns: table => new
                {
                    PUUID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPuuids", x => x.PUUID);
                });

            migrationBuilder.CreateTable(
                name: "ChampMatchupStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnemyChampion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wins = table.Column<int>(type: "int", nullable: false),
                    Picks = table.Column<int>(type: "int", nullable: false),
                    ChampGameStatsChampionName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampMatchupStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChampMatchupStats_ChampGameStats_ChampGameStatsChampionName",
                        column: x => x.ChampGameStatsChampionName,
                        principalTable: "ChampGameStats",
                        principalColumn: "ChampionName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChampMatchupStats_ChampGameStatsChampionName",
                table: "ChampMatchupStats",
                column: "ChampGameStatsChampionName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInfo");

            migrationBuilder.DropTable(
                name: "ChampMatchupStats");

            migrationBuilder.DropTable(
                name: "PlayerPuuids");

            migrationBuilder.DropTable(
                name: "ChampGameStats");
        }
    }
}
