using BanWho.Application.Abstractions;
using BanWho.Domain.Interfaces;

namespace BanWho.Application.Entities.Queries.GetChampGameStatsByWinRate;

internal sealed class GetChampGameStatsByWinRateHandler : IQueryHandler<GetChampGameStatsByWinRate, ChampGameStatsResponse>
{
	private readonly IChampGameStatsRepository _champGameStatsRepository;

	public GetChampGameStatsByWinRateHandler(IChampGameStatsRepository champGameStatsRepository)
	{
		_champGameStatsRepository = champGameStatsRepository;
	}

	public async Task<ChampGameStatsResponse> Handle(GetChampGameStatsByWinRate request, CancellationToken cancellationToken)
	{
		var champGameStats = await _champGameStatsRepository.GetByWinRatesAsync(request.Role, request.Amount);

		if (champGameStats == null)
		{
			// error
			System.Diagnostics.Debug.WriteLine($"Could not get champ game stats for {request.Role}");
		}

		var response = new ChampGameStatsResponse(champGameStats);

		return response;
	}
}
