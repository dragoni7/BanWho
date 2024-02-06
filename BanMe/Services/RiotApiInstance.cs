using BanMe.Data;
using Camille.RiotGames;

namespace BanMe.Services
{
	public class RiotApiInstance : IRiotApiInstance
	{
		private readonly RiotGamesApi _riotApiInstance;

		public RiotApiInstance(IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			_riotApiInstance = RiotGamesApi.NewInstance(scope.ServiceProvider.GetRequiredService<BanMeDbContext>().AppInfo.First().ApiKey);
		}

		public RiotGamesApi GetApiInstance()
		{
			return _riotApiInstance;
		}
	}
}
