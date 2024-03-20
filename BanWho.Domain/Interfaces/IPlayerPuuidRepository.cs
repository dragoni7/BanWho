using BanWho.Domain.Entities;

namespace BanWho.Domain.Interfaces;

public interface IPlayerPuuidRepository
{
    Task<List<Player>> GetAllAsync();

    Task<Player> GetPlayerAsync(string puuid);

    Task AddAsync(Player player);

    Task ClearAsync();

    Task SaveAsync();

    int Count();
}
