using BanMe.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace BanMe.Jobs
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
			var banMecontext = scope.ServiceProvider.GetRequiredService<BanMeContext>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<PatchUpdateBackgroundJob>>();

			string fetchedPatch = "";

			using (HttpClient client = new())
			{
				var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
				fetchedPatch = json.First();
			}

			var appInfo = await banMecontext.AppInfo.FirstAsync();

			if (appInfo.PatchUsed != fetchedPatch)
			{
				logger.LogInformation($"Updated db from old patch {appInfo.PatchUsed} to {fetchedPatch}");
				appInfo.PatchUsed = fetchedPatch;

				// clear dbs for new patch

				banMecontext.SaveChanges();
			}


			logger.LogInformation($"{{UtcNow}} Current Patch: {fetchedPatch}", DateTime.UtcNow);
		}
	}
}
