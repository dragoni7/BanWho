using BanMe.Domain.Entities;

namespace BanMe.Application.Entities.Queries.GetChampMatchupStatsByWinRate;

public sealed record ChampMatchupStatsResponse(List<ChampMatchupStats> ChampMatchupStats);
