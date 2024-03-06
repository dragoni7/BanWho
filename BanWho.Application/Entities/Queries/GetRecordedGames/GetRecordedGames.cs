using BanWho.Application.Abstractions;

namespace BanWho.Application.Entities.Queries.GetRecordedGames;

public sealed record GetRecordedGames() : IQuery<RecordedGamesResponse>;
