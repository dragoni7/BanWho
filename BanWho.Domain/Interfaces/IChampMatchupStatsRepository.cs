using BanWho.Domain.Entities;

namespace BanWho.Domain.Interfaces;
public interface IChampMatchupStatsRepository
	{
		public void Add(ChampMatchupStats matchup);

		public Task<List<ChampMatchupStats>> GetAllMatchupsByChampAsync(string champ);

		public Task<List<ChampMatchupStats>> GetHardestMatchupsByChampAsync(string champ, int amount);

		public Task SaveAsync();
	}

