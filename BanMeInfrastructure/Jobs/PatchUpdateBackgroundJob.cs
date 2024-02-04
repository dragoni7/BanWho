using Microsoft.Extensions.DependencyInjection;
using Quartz;
using MediatR;
using BanMeInfrastructure.Messages;

namespace BanMeInfrastructure.Jobs
{
	[DisallowConcurrentExecution]
	public class PatchUpdateBackgroundJob : IJob
	{
		private readonly IServiceProvider _serviceProvider;

		public PatchUpdateBackgroundJob(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			using var scope = _serviceProvider.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

			await mediator.Send(new PatchUpdateMessage(_serviceProvider));
		}
	}
}
