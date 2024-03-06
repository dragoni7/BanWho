using BanWho.Domain.Entities;

namespace BanWho.Application.Entities.Queries.GetChampMatchupStatsByWinRate;

public sealed record ChampMatchupStatsResponse(List<ChampMatchupStats> ChampMatchupStats);
