using System.ComponentModel.DataAnnotations;

namespace BanMe.Domain.Entities;

public class Player
{
    [Key]
    public required string PUUID { get; set; }
}
