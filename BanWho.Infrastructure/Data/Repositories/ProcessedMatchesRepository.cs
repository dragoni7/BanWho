using BanWho.Domain.Consts;
using BanWho.Domain.Entities;
using BanWho.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BanWho.Infrastructure.Data.Repositories
{
	internal class ProcessedMatchesRepository : IProcessedMatchesRepository
	{
		private readonly BanWhoDbContext _context;

		private readonly ILogger<IProcessedMatchesRepository> _logger;

		public ProcessedMatchesRepository(BanWhoDbContext context, ILogger<IProcessedMatchesRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public void Add(ProcessedMatch match)
		{
			_context.ProcessedMatches.Add(match);
		}

		public async Task TrimUnusedMatches()
		{
			var playerPuuids = _context.PlayerPuuids;
			var processedMatches = _context.ProcessedMatches;
			int processedMatchCount = await processedMatches.CountAsync();
			int playersCount = await playerPuuids.CountAsync();
			bool shouldTrim = processedMatchCount > playersCount * BanWhoConsts.DataThresholds.MatchesTrackedPerPlayer;

			_logger.LogInformation($"Found {processedMatchCount} total processed matches and {playersCount} player puuids.");

			if (shouldTrim)
			{
				int toRemove = playersCount - playersCount * BanWhoConsts.DataThresholds.MatchesTrackedPerPlayer;
				_logger.LogInformation($"Trimming {toRemove} oldest processed matches");
				processedMatches.RemoveRange(processedMatches.Skip(processedMatchCount - toRemove));
			}
		}

		public bool ContainsMatchId(string id)
		{
			return _context.ProcessedMatches.Any(i => i.MatchID == id);
		}
	}
}
