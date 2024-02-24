using BanMe.Domain.Entities;

namespace BanMe.Domain.Interfaces;

public interface IProcessedMatchesRepository
{
	void Add(ProcessedMatch match);

	public void TrimUnusedMatches();

	bool ContainsMatchId(string id);

}
