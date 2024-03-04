using BanMe.Application.Abstractions;
using BanMe.Domain.Interfaces;

namespace BanMe.Application.Entities.Queries.GetAppVersion;

internal sealed class GetAppVersionHandler : IQueryHandler<GetAppVersion, AppVersionResponse>
{
	private readonly IBanMeInfoRepository _banMeInfoRepository;

	public GetAppVersionHandler(IBanMeInfoRepository banMeInfoRepository)
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
