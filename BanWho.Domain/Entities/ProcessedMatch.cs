using System.ComponentModel.DataAnnotations;

namespace BanWho.Domain.Entities;

public class ProcessedMatch
{
	[Key]
	public required string MatchID { get; set; }
}
