using BanWho.Application.Abstractions;

namespace BanWho.Application.Entities.Queries.GetChampGameStatsByWinRate;

public sealed record GetChampGameStatsByWinRate(string Role, int Amount) : IQuery<ChampGameStatsResponse>
{

}
