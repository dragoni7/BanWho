using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanMe.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChampGameStats",
                columns: table => new
                {
                    ChampionName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TopWinRate = table.Column<float>(type: "real", nullable: false),
                    TopPickRate = table.Column<float>(type: "real", nullable: false),
                    MidWinRate = table.Column<float>(type: "real", nullable: false),
                    MidPickRate = table.Column<float>(type: "real", nullable: false),
                    JungleWinRate = table.Column<float>(type: "real", nullable: false),
                    JunglePickRate = table.Column<float>(type: "real", nullable: false),
                    BotWinRate = table.Column<float>(type: "real", nullable: false),
                    BotPickRate = table.Column<float>(type: "real", nullable: false),
                    SuppWinRate = table.Column<float>(type: "real", nullable: false),
                    SuppPickRate = table.Column<float>(type: "real", nullable: false),
                    BanRate = table.Column<float>(type: "real", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampGameStats");

            migrationBuilder.DropTable(
                name: "PlayerPuuids");
        }
    }
}
