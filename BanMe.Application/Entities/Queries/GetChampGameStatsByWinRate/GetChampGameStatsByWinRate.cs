using BanMe.Application.Abstractions;

namespace BanMe.Application.Entities.Queries.GetChampGameStatsByWinRate;

public sealed record GetChampGameStatsByWinRate(string Role, int Amount) : IQuery<ChampGameStatsResponse>
{

}
