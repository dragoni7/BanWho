using System.ComponentModel.DataAnnotations;

namespace BanMe.Domain.Entities;

public class ProcessedMatch
{
	[Key]
	public required string MatchID { get; set; }
}
