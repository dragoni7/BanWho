using BanWho.Domain.Entities;

namespace BanWho.Domain.Interfaces;

public interface IProcessedMatchesRepository
{
	void Add(ProcessedMatch match);

	public Task TrimUnusedMatches();

	bool ContainsMatchId(string id);

}
