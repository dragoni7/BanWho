using BanWho.Domain.Consts;
using BanWho.Domain.Entities;
using BanWho.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BanWho.Infrastructure.Data.Repositories
{
	internal class ProcessedMatchesRepository : IProcessedMatchesRepository
	{
		private readonly BanWhoDbContext _context;

		public ProcessedMatchesRepository(BanWhoDbContext context)
		{
			_context = context;
		}

		public void Add(ProcessedMatch match)
		{
			_context.ProcessedMatches.Add(match);
		}

		public async void TrimUnusedMatches()
		{
			var playerPuuids = _context.PlayerPuuids;
			var processedMatches = _context.ProcessedMatches;

			if (processedMatches.Count() > playerPuuids.Count() * BanWhoConsts.DataThresholds.MatchesTrackedPerPlayer)
			{
				int toRemove = playerPuuids.Count() - playerPuuids.Count() * BanWhoConsts.DataThresholds.MatchesTrackedPerPlayer;
				processedMatches.RemoveRange(processedMatches.Skip(playerPuuids.Count() - toRemove));
			}
		}

		public bool ContainsMatchId(string id)
		{
			return _context.ProcessedMatches.Any(i => i.MatchID == id);
		}
	}
}
