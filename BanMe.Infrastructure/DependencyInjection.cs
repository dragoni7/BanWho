using BanMe.Domain.Interfaces;
using BanMe.Infrastructure.Data;
using BanMe.Infrastructure.Data.Repositories;
using BanMe.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BanMe.Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<BanMeDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
			throw new InvalidOperationException("connection string DefaultConnection not found")));

		services.AddScoped<IChampGameStatsRepository, ChampGameStatsRepository>();
		services.AddScoped<IBanMeInfoRepository, BanMeInfoRepository>();
		services.AddScoped<IPlayerPuuidRepository, PlayerPuuidRepository>();
		services.AddScoped<IProcessedMatchesRepository, ProcessedMatchesRepository>();
		services.AddScoped<IRiotDataCrawler, RiotDataCrawler>();

		/*services.AddQuartz(options =>
		{
			var jobKey = JobKey.Create(nameof(UpdateChampGameStatsBackgroundJob));

			options
				.AddJob<UpdateChampGameStatsBackgroundJob>(jobKey)
				.AddTrigger(trigger =>
					trigger
						.ForJob(jobKey)
						.WithSimpleSchedule(schedule =>
						schedule.WithIntervalInSeconds(10)
						.WithRepeatCount(0))
						);
		});

		services.AddQuartzHostedService();*/

		return services;
	}
}
