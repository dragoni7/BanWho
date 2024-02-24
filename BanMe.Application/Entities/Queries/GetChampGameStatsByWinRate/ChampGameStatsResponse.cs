using BanMe.Domain.Entities;

namespace BanMe.Application.Entities.Queries.GetChampGameStatsByWinRate;

public sealed record ChampGameStatsResponse(List<ChampGameStats> ChampGameStats);