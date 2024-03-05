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
		System.Diagnostics.Debug.WriteLine("Starting Update Players Background Job at " + DateTime.UtcNow);

#if DEBUG
		Tier[] selectedTiers = [Tier.EMERALD];
#else
		Tier[] selectedTiers = [Tier.EMERALD, Tier.DIAMOND, Tier.MASTER, Tier.GRANDMASTER, Tier.CHALLENGER];
#endif

		PlatformRoute[] selectedRoutes =
		{
			PlatformRoute.EUW1, PlatformRoute.EUN1, PlatformRoute.TR1, PlatformRoute.RU, // Europe
			PlatformRoute.JP1, PlatformRoute.KR, // Asia
			PlatformRoute.NA1, PlatformRoute.BR1, PlatformRoute.LA1, PlatformRoute.LA2, // Americas
			PlatformRoute.OC1, PlatformRoute.PH2, PlatformRoute.SG2, PlatformRoute.TH2, PlatformRoute.TW2, PlatformRoute.VN2 // Sea
		};

		await _playerPuuidRepository.ClearAsync();

		foreach (PlatformRoute route in selectedRoutes)
		{
			foreach (Tier tier in selectedTiers)
			{
				var playerPuuids = await _riotDataCrawler.CrawlPlayersAsync(tier, route);

				foreach (string puuid in playerPuuids)
				{
					int regionalRoute = (int)GetRegionalRouteFromPlatform(route);

                    if ( regionalRoute == 0)
						continue;

					_playerPuuidRepository.Add(new Player { PUUID = puuid, RegionalRoute = regionalRoute });
				}

				await _playerPuuidRepository.SaveAsync();
			}
		}

		System.Diagnostics.Debug.WriteLine("Finished Update Players Background Job at " + DateTime.UtcNow);
	}

	private RegionalRoute GetRegionalRouteFromPlatform(PlatformRoute route)
	{
		switch (route)
		{
			case PlatformRoute.EUW1:
			case PlatformRoute.EUN1:
			case PlatformRoute.TR1:
			case PlatformRoute.RU:
				{
					return RegionalRoute.EUROPE;
				}
			case PlatformRoute.JP1:
			case PlatformRoute.KR:
				{
					return RegionalRoute.ASIA;
				}
			case PlatformRoute.NA1:
			case PlatformRoute.BR1:
			case PlatformRoute.LA1:
			case PlatformRoute.LA2:
				{
					return RegionalRoute.AMERICAS;
				}
			case PlatformRoute.OC1:
			case PlatformRoute.PH2:
			case PlatformRoute.SG2:
			case PlatformRoute.TH2:
			case PlatformRoute.TW2:
			case PlatformRoute.VN2:
				{
					return RegionalRoute.SEA;
				}
			default:
				{
					return 0;
				}
		}
	}
}
