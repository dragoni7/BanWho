using BanWho.Domain.Entities;

namespace BanWho.Application.Entities.Queries.GetChampGameStatsByWinRate;

public sealed record ChampGameStatsResponse(List<ChampGameStats> ChampGameStats);