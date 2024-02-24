using BanMe.Domain.Entities;

namespace BanMe.Domain.Interfaces;

public interface IPlayerPuuidRepository
{
    Task<List<Player>> GetAllAsync();

    void Add(Player player);

    Task ClearAsync();

    Task SaveAsync();

    int Count();
}
