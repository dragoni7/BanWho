﻿using BanWho.Domain.Interfaces;
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
		services.AddDbContext<BanWhoDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
			throw new InvalidOperationException("connection string DefaultConnection not found")));

		services.AddScoped<IChampGameStatsRepository, ChampGameStatsRepository>();
		services.AddScoped<IChampMatchupStatsRepository, ChampMatchupStatsRepository>();
		services.AddScoped<IBanWhoInfoRepository, BanWhoInfoRepository>();
		services.AddScoped<IPlayerPuuidRepository, PlayerPuuidRepository>();
		services.AddScoped<IProcessedMatchesRepository, ProcessedMatchesRepository>();
		services.AddScoped<IRiotDataCrawler, RiotDataCrawler>();

		services.AddQuartz(options =>
		{
#if DEBUG
			var jobKey = JobKey.Create(nameof(UpdateChampGameStatsBackgroundJob));

			options
				.AddJob<UpdateChampGameStatsBackgroundJob>(jobKey)
				.AddTrigger(trigger =>
					trigger
						.ForJob(jobKey)
						.WithSimpleSchedule(schedule =>
						schedule.WithIntervalInSeconds(5)
						.WithRepeatCount(0))
						);
#else
			var updatePlayersJobKey = JobKey.Create(nameof(UpdatePlayersBackgroundJob));

			options
				.AddJob<UpdatePlayersBackgroundJob>(updatePlayersJobKey)
				.AddTrigger(trigger => trigger.StartNow())
				.AddTrigger(trigger =>
					trigger
						.ForJob(updatePlayersJobKey)
						.WithSimpleSchedule(schedule =>
						schedule.WithIntervalInHours(168) // every week
						.RepeatForever())
						);

			var updateChampStatsJobKey = JobKey.Create(nameof(UpdateChampGameStatsBackgroundJob));

			options
				.AddJob<UpdateChampGameStatsBackgroundJob>(updateChampStatsJobKey)
				.AddTrigger(trigger =>
					trigger
						.ForJob(updateChampStatsJobKey)
						.WithSimpleSchedule(schedule =>
						schedule.WithIntervalInHours(72) // every 3 days
						.RepeatForever())
						);
#endif
		});

		services.AddQuartzHostedService();

		return services;
	}
}