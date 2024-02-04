using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
    public class ChampGameStats
    {
        [Key]
        public required string ChampionName { get; set; }

        public int TopWins { get; set; } = 0;

		public float TopWinRate { get; set; } = 0f;

        public int TopPicks { get; set; } = 0;

		public float TopPickRate { get; set; } = 0f;

		public int MidWins { get; set; } = 0;

		public float MidWinRate { get; set; } = 0f;

		public int MidPicks { get; set; } = 0;

		public float MidPickRate { get; set; } = 0f;

		public int JungleWins { get; set; } = 0;

		public float JungleWinRate { get; set; } = 0f;

		public int JunglePicks { get; set; } = 0;

		public float JunglePickRate { get; set; } = 0f;

		public int BotWins { get; set; } = 0;

		public float BotWinRate { get; set; } = 0f;

		public int BotPicks { get; set; } = 0;

		public float BotPickRate { get; set; } = 0f;

		public int SuppWins { get; set; } = 0;

		public float SuppWinRate { get; set; } = 0f;

		public int SuppPicks { get; set; } = 0;

		public float SuppPickRate { get; set; } = 0f;

		public int Bans { get; set; } = 0;

		public float BanRate { get; set; } = 0f;

		public IList<ChampMatchupStats> MatchupStats { get; set; } = new List<ChampMatchupStats>();
    }
}
