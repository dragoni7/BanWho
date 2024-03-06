using BanWho.Application.Abstractions;

namespace BanWho.Application.Entities.Queries.GetPatchUsed;

public sealed record GetPatchUsed() : IQuery<PatchUsedResponse>;
