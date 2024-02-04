using System.ComponentModel.DataAnnotations;

namespace BanMe.Entities
{
	public class ChampMatchupStats
	{
		[Key]
		public int Id { get; set; }

		public required string EnemyChampion { get; set; }

		public int Wins { get; set; } = 0;

		public float WinRate { get; set; } = 0f;

		public int Picks { get; set; } = 0;
	}
}
