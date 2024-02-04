using BanMeInfrastructure.Jobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BanMeInfrastructure
{
	public static class DependencyInjection
	{
		public static void AddInfrastructure(this IServiceCollection services)
		{
			services.AddMediatR(cfg => {
				cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
			});

			services.AddQuartz(options =>
			{
				var jobKey = JobKey.Create(nameof(PatchUpdateBackgroundJob));

				options
					.AddJob<PatchUpdateBackgroundJob>(jobKey)
					.AddTrigger(trigger =>
						trigger
							.ForJob(jobKey)
							.WithSimpleSchedule(schedule =>
							schedule.WithIntervalInSeconds(10)
							.RepeatForever())
							);
			});

			services.AddQuartzHostedService();
		}

		public static void AddMediatRType<T>(this IServiceCollection services)
		{
			services.AddMediatR(cfg => {
				cfg.RegisterServicesFromAssembly(typeof(T).Assembly);
			});
		}
	}
}
