using BanWho.Application.Abstractions;

namespace BanWho.Application.Entities.Queries.GetChampMatchupStatsByWinRate;

public sealed record GetChampMatchupStatsByWinRate(string ChampName, int Amount) : IQuery<ChampMatchupStatsResponse>
{

}
