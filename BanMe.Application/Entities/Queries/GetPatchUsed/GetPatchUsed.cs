using BanMe.Application.Abstractions;

namespace BanMe.Application.Entities.Queries.GetPatchUsed;

public sealed record GetPatchUsed() : IQuery<PatchUsedResponse>;
