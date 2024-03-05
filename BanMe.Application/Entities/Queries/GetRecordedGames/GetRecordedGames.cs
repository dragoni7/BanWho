using BanMe.Application.Abstractions;

namespace BanMe.Application.Entities.Queries.GetRecordedGames;

public sealed record GetRecordedGames() : IQuery<RecordedGamesResponse>;
