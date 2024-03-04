using BanMe.Domain.Entities;
using BanMe.Domain.Interfaces;
using BanMe.Infrastructure.Data;
using Camille.Enums;
using Quartz;

namespace BanMe.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal class UpdatePlayersBackgroundJob : IJob
{
	private readonly IPlayerPuuidRepository _playerPuuidRepository;

	private readonly IRiotDataCrawler _riotDataCrawler;

	public UpdatePlayersBackgroundJob(IPlayerPuuidRepository playerPuuidRepository, IRiotDataCrawler riotDataCrawler)
	{
		_playerPuuidRepository = playerPuuidRepository;
		_riotDataCrawler = riotDataCrawler;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		Tier[] selectedTiers = { Tier.EMERALD, Tier.DIAMOND, Tier.MASTER, Tier.GRANDMASTER, Tier.CHALLENGER };

		await _playerPuuidRepository.ClearAsync();

		foreach (Tier tier in selectedTiers)
		{
			var playerPuuids = await _riotDataCrawler.CrawlPlayersAsync(tier, PlatformRoute.NA1);

			foreach (string puuid in playerPuuids)
			{
				_playerPuuidRepository.Add(new Player { PUUID = puuid });
			}

			await _playerPuuidRepository.SaveAsync();
		}
	}
}
