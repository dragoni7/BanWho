using BanMe.Application.Abstractions;
using BanMe.Domain.Interfaces;

namespace BanMe.Application.Entities.Queries.GetChampMatchupStatsByWinRate;

internal sealed class GetChampMatchupStatsByWinRateHandler : IQueryHandler<GetChampMatchupStatsByWinRate, ChampMatchupStatsResponse>
{
	private readonly IChampMatchupStatsRepository _champMatchupStatsRepository;

	public GetChampMatchupStatsByWinRateHandler(IChampMatchupStatsRepository champMatchupStatsRepository)
	{
		_champMatchupStatsRepository = champMatchupStatsRepository;
	}

	public async Task<ChampMatchupStatsResponse> Handle(GetChampMatchupStatsByWinRate request, CancellationToken cancellationToken)
	{
		var champMatchupStats = await _champMatchupStatsRepository.GetHardestMatchupsByChampAsync(request.ChampName, request.Amount);

		if (champMatchupStats == null)
		{
			// error
			System.Diagnostics.Debug.WriteLine($"Could not get champ matchup stats for {request.ChampName}");
		}

		var response = new ChampMatchupStatsResponse(champMatchupStats);

		return response;
	}
}
