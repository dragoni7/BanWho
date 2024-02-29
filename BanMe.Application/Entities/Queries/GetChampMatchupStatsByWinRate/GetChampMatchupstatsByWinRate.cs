using BanMe.Application.Abstractions;

namespace BanMe.Application.Entities.Queries.GetChampMatchupStatsByWinRate;

public sealed record GetChampMatchupStatsByWinRate(string ChampName, int Amount) : IQuery<ChampMatchupStatsResponse>
{

}
