using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
    public class ChampGameStats
    {
        [Key]
        public required string ChampionName { get; set; }

        public int TopWins { get; set; } = 0;

        public int TopPicks { get; set; } = 0;

		public int MidWins { get; set; } = 0;

		public int MidPicks { get; set; } = 0;

		public int JungleWins { get; set; } = 0;

		public int JunglePicks { get; set; } = 0;

		public int BotWins { get; set; } = 0;

		public int BotPicks { get; set; } = 0;

		public int SuppWins { get; set; } = 0;

		public int SuppPicks { get; set; } = 0;

		public int Bans { get; set; } = 0;

		public IList<ChampMatchupStats> MatchupStats { get; set; } = new List<ChampMatchupStats>();
    }
}
