using BanWho.Domain.Entities;

namespace BanWho.Domain.Interfaces;

public interface IProcessedMatchesRepository
{
	void Add(ProcessedMatch match);

	public void TrimUnusedMatches();

	bool ContainsMatchId(string id);

}
