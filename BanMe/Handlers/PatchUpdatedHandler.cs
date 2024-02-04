using BanMe.Data;
using BanMeInfrastructure.Jobs;
using BanMeInfrastructure.Messages;
using MediatR;

namespace BanMe.Handlers
{
	public class PatchUpdatedHandler : IRequestHandler<PatchUpdateMessage>
	{
		public async Task Handle(PatchUpdateMessage request, CancellationToken cancellationToken)
		{
			using var scope = request.ServiceProvider.CreateScope();

			var dbContext = scope.ServiceProvider.GetRequiredService<BanMeDbContext>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<PatchUpdateBackgroundJob>>();

			string fetchedPatch = "";

			using (HttpClient client = new())
			{
				var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
				fetchedPatch = json.First();
			}

			var appInfo = await dbContext.GetBanMeInfoAsync();

			if (appInfo.PatchUsed != fetchedPatch)
			{
				logger.LogInformation($"Updated db from old patch {appInfo.PatchUsed} to {fetchedPatch}");
				appInfo.PatchUsed = fetchedPatch;

				await dbContext.DumpPatchDataAsync();

				// re-seed db

				dbContext.SaveChanges();
			}


			logger.LogInformation($"{{UtcNow}} Current Patch: {fetchedPatch}", DateTime.UtcNow);
		}
	}
}
