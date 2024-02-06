namespace BanMe.Services
{
	public interface IDbSeeder
	{
		public Task SeedPlayerPuuidsAsync();

		public Task SeedChampGameStatsAsync();
	}
}
