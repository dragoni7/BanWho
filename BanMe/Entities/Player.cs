using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
    public class Player
    {
        [Key]
        public required string PUUID { get; set; }
    }
}
