using BanWho.Application.Abstractions;
using BanWho.Domain.Interfaces;

namespace BanWho.Application.Entities.Queries.GetPatchUsed;

internal sealed class GetPatchUsedHandler : IQueryHandler<GetPatchUsed, PatchUsedResponse>
{
	private readonly IBanWhoInfoRepository _banMeInfoRepository;

	public GetPatchUsedHandler(IBanWhoInfoRepository banMeInfoRepository)
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
