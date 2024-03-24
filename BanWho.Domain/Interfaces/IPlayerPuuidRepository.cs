using BanWho.Domain.Entities;

namespace BanWho.Domain.Interfaces;

public interface IPlayerPuuidRepository
{
    Task<Player[]> GetAllAsync();

    Task<bool> ContainsPlayerAsync(string puuid);

    Task AddAsync(Player player);

    Task ClearAsync();

    Task SaveAsync();

    int Count();
}
