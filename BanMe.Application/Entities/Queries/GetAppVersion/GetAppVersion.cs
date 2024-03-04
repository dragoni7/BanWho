using BanMe.Application.Abstractions;

namespace BanMe.Application.Entities.Queries.GetAppVersion;

public sealed record GetAppVersion() : IQuery<AppVersionResponse>;
