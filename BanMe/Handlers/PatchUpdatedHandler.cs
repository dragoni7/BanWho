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

			var banMecontext = scope.ServiceProvider.GetRequiredService<BanMeDbContext>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<PatchUpdateBackgroundJob>>();

			string fetchedPatch = "";

			using (HttpClient client = new())
			{
				var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
				fetchedPatch = json.First();
			}

			var appInfo = await banMecontext.GetBanMeInfoAsync();

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
