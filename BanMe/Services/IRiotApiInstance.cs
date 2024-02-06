using Camille.RiotGames;

namespace BanMe.Services
{
	public interface IRiotApiInstance
	{
		public RiotGamesApi GetApiInstance();
	}
}
