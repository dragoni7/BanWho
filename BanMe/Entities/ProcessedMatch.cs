using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
	public class ProcessedMatch
	{
		[Key]
		public required string MatchID { get; set; }
	}
}
