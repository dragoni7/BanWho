using BanWho.Application.Abstractions;

namespace BanWho.Application.Entities.Queries.GetAppVersion;

public sealed record GetAppVersion() : IQuery<AppVersionResponse>;
