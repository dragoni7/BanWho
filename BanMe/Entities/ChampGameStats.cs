using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
    public class ChampGameStats
    {
        [Key]
        public required string ChampionName { get; set; }

        public float TopWinRate { get; set; }

        public float TopPickRate { get; set; }

        public float MidWinRate { get; set; }

        public float MidPickRate { get; set; }

        public float JungleWinRate { get; set; }

        public float JunglePickRate { get; set; }

        public float BotWinRate { get; set; }

        public float BotPickRate { get; set; }

        public float SuppWinRate { get; set; }

        public float SuppPickRate { get; set; }

        public float BanRate { get; set; }
    }
}
