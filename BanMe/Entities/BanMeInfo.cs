using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
	public class BanMeInfo
	{
		[Key]
		public required string AppVersion { get; set; }

		public required string PatchUsed { get; set; }

		public int RecordedGames { get; set; }

		public string? ApiKey { get; set; }
	}
}
