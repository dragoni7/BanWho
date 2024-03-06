using System.ComponentModel.DataAnnotations;

namespace BanWho.Domain.Entities;

public class BanWhoInfo
{
	[Key]
	public required string AppVersion { get; set; }

	public required string PatchUsed { get; set; }

	public int RecordedGames { get; set; } = 0;

	public string? ApiKey { get; set; }
}
