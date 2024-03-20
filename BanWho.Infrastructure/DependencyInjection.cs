using BanWho.Domain.Interfaces;
using BanWho.Infrastructure.Data;
using BanWho.Infrastructure.Data.Repositories;
using BanWho.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BanWho.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
#if DEBUG
		services.AddDbContext<BanWhoDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DevConnection") ??
			throw new InvalidOperationException("connection string DevConnection not found")));
#else
		services.AddDbContext<BanWhoDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") ??
			throw new InvalidOperationException("connection string AZURE_SQL_CONNECTIONSTRING not found")));
#endif
		services.AddScoped<IChampGameStatsRepository, ChampGameStatsRepository>();
		services.AddScoped<IChampMatchupStatsRepository, ChampMatchupStatsRepository>();
		services.AddScoped<IBanWhoInfoRepository, BanWhoInfoRepository>();
		services.AddScoped<IPlayerPuuidRepository, PlayerPuuidRepository>();
		services.AddScoped<IProcessedMatchesRepository, ProcessedMatchesRepository>();
		services.AddScoped<IRiotDataCrawler, RiotDataCrawler>();

		services.AddQuartz(options =>
		{
#if DEBUG
			var updateChampStatsJobKey = new JobKey(nameof(UpdateChampGameStatsBackgroundJob));			
			options
				.AddJob<UpdateChampGameStatsBackgroundJob>(j => j
				.WithIdentity(updateChampStatsJobKey)
				.WithDescription("Updates champ matchup and gamestats db from latest api data."));
			options.AddTrigger(t => t
				.WithIdentity("UpdateChampGameStatsBackgroundJob-Trigger")
				.ForJob(updateChampStatsJobKey)
				.StartAt(DateTime.Now.AddDays(2))
				.WithSimpleSchedule(schedule =>
				schedule.WithIntervalInHours(48)
				.RepeatForever()));

			var updatePlayersJobKey = new JobKey(nameof(UpdatePlayersBackgroundJob));
			options
				.AddJob<UpdatePlayersBackgroundJob>(j => j
				.WithIdentity(updatePlayersJobKey)
				.WithDescription("Updates global player puuids from latest api data."));
			options
				.AddTrigger(t => t
				.WithIdentity("UpdatePlayersBackgroundJob-Trigger")
				.ForJob(updatePlayersJobKey)
				.StartNow()
				.WithSimpleSchedule(schedule =>
				schedule.WithIntervalInHours(168)
				.RepeatForever()));
#else
			var updateChampStatsJobKey = new JobKey(nameof(UpdateChampGameStatsBackgroundJob));			
			options
				.AddJob<UpdateChampGameStatsBackgroundJob>(j => j
				.WithIdentity(updateChampStatsJobKey)
				.WithDescription("Updates champ matchup and gamestats db from latest api data."));
			options.AddTrigger(t => t
				.WithIdentity("UpdateChampGameStatsBackgroundJob-Trigger")
				.ForJob(updateChampStatsJobKey)
				.StartAt(DateTime.Now.AddDays(2))
				.WithSimpleSchedule(schedule =>
				schedule.WithIntervalInHours(48)
				.RepeatForever()));

			var updatePlayersJobKey = new JobKey(nameof(UpdatePlayersBackgroundJob));
			options
				.AddJob<UpdatePlayersBackgroundJob>(j => j
				.WithIdentity(updatePlayersJobKey)
				.WithDescription("Updates global player puuids from latest api data."));
			options
				.AddTrigger(t => t
				.WithIdentity("UpdatePlayersBackgroundJob-Trigger")
				.ForJob(updatePlayersJobKey)
				.StartNow()
				.WithSimpleSchedule(schedule =>
				schedule.WithIntervalInHours(168)
				.RepeatForever()));
#endif
		});

		services.AddQuartzHostedService();

		return services;
	}
}
