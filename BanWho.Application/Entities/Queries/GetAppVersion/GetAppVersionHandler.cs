using BanWho.Application.Abstractions;
using BanWho.Domain.Interfaces;

namespace BanWho.Application.Entities.Queries.GetAppVersion;

internal sealed class GetAppVersionHandler : IQueryHandler<GetAppVersion, AppVersionResponse>
{
	private readonly IBanWhoInfoRepository _banMeInfoRepository;

	public GetAppVersionHandler(IBanWhoInfoRepository banMeInfoRepository)
	{
		_banMeInfoRepository = banMeInfoRepository;
	}

	public async Task<AppVersionResponse> Handle(GetAppVersion request, CancellationToken cancellationToken)
	{
		string appVersion = await _banMeInfoRepository.GetAppVersionAsync();
		var response = new AppVersionResponse(appVersion);

		return response;
	}
}
