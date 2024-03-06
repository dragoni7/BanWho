using System.ComponentModel.DataAnnotations;

namespace BanWho.Domain.Entities;

public class Player
{
    [Key]
    public required string PUUID { get; set; }

    public required int RegionalRoute { get; set; }
}
