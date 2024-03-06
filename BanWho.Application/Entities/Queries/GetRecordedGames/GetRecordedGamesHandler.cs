using BanWho.Application.Abstractions;
using BanWho.Domain.Interfaces;

namespace BanWho.Application.Entities.Queries.GetRecordedGames;

internal sealed class GetRecordedGamesHandler : IQueryHandler<GetRecordedGames, RecordedGamesResponse>
{
	private readonly IBanWhoInfoRepository _banMeInfoRepository;

	public GetRecordedGamesHandler(IBanWhoInfoRepository banMeInfoRepository)
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
