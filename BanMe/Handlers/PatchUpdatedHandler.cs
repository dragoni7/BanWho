using BanMe.Data;
using BanMe.Services;
using BanMeInfrastructure.Jobs;
using BanMeInfrastructure.Messages;
using MediatR;

namespace BanMe.Handlers
{
	public class PatchUpdatedHandler : IRequestHandler<PatchUpdateMessage>
	{
		public async Task Handle(PatchUpdateMessage request, CancellationToken cancellationToken)
		{
			using IServiceScope scope = request.ServiceProvider.CreateScope();

			var dbContext = scope.ServiceProvider.GetRequiredService<BanMeDbContext>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<PatchUpdateBackgroundJob>>();
			var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();

			string fetchedPatch = "";

			using (HttpClient client = new())
			{
				var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
				fetchedPatch = json.First();
			}

			var appInfo = await dbContext.GetBanMeInfoAsync();

			if (appInfo.PatchUsed != fetchedPatch)
			{
				logger.LogInformation($"Updating db from old patch {appInfo.PatchUsed} to {fetchedPatch}");

				appInfo.PatchUsed = fetchedPatch;

				await dbContext.DumpPatchDataAsync();

				await seeder.SeedPlayerPuuidsAsync();

				logger.LogInformation("Update complete");
			}


			logger.LogInformation($"{{UtcNow}} Current Patch: {fetchedPatch}", DateTime.UtcNow);
		}
	}
}
