using BanMe.Application.Abstractions;
using BanMe.Domain.Interfaces;

namespace BanMe.Application.Entities.Queries.GetRecordedGames;

internal sealed class GetRecordedGamesHandler : IQueryHandler<GetRecordedGames, RecordedGamesResponse>
{
	private readonly IBanMeInfoRepository _banMeInfoRepository;

	public GetRecordedGamesHandler(IBanMeInfoRepository banMeInfoRepository)
	{
		_banMeInfoRepository = banMeInfoRepository;
	}

	public async Task<RecordedGamesResponse> Handle(GetRecordedGames request, CancellationToken cancellationToken)
	{
		int recordedGames = await _banMeInfoRepository.GetRecordedGamesAsync();
		var response = new RecordedGamesResponse(recordedGames);

		return response;
	}
}
