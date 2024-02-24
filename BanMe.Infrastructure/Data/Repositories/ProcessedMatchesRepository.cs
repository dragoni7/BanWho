using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;

namespace BanMe.Infrastructure.Data.Repositories
{
	internal class ProcessedMatchesRepository : IProcessedMatchesRepository
	{
		private readonly BanMeDbContext _context;

		public ProcessedMatchesRepository(BanMeDbContext context)
		{
			_context = context;
		}

		public void Add(ProcessedMatch match)
		{
			_context.ProcessedMatches.Add(match);
		}

		public void TrimUnusedMatches()
		{
			var playerPuuids = _context.PlayerPuuids;
			var processedMatches = _context.ProcessedMatches;

			if (processedMatches.Count() > playerPuuids.Count() * 20)
			{
				int toRemove = playerPuuids.Count() - playerPuuids.Count() * 20;
				processedMatches.RemoveRange(processedMatches.TakeLast(toRemove));
			}
		}

		public bool ContainsMatchId(string id)
		{
			return _context.ProcessedMatches.Any(i => i.MatchID == id);
		}
	}
}
