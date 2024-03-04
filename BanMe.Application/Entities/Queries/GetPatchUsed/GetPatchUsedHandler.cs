using BanMe.Application.Abstractions;
using BanMe.Domain.Interfaces;

namespace BanMe.Application.Entities.Queries.GetPatchUsed;

internal sealed class GetPatchUsedHandler : IQueryHandler<GetPatchUsed, PatchUsedResponse>
{
	private readonly IBanMeInfoRepository _banMeInfoRepository;

	public GetPatchUsedHandler(IBanMeInfoRepository banMeInfoRepository)
	{
		_banMeInfoRepository = banMeInfoRepository;
	}

	public async Task<PatchUsedResponse> Handle(GetPatchUsed request, CancellationToken cancellationToken)
	{
		string patchUsed = await _banMeInfoRepository.GetPatchUsedAsync();
		var response = new PatchUsedResponse(patchUsed);

		return response;
	}
}
