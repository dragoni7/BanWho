using BanMe.Application.Abstractions;
using BanMe.Application.Entities.Queries.GetAppVersion;
using BanMe.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
