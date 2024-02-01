
namespace BanMe.Services
{
	public class LeagueDataService : ILeagueDataService
	{
		public async Task<string> GetLatestPatch()
		{
			string patch = "";

			using (HttpClient client = new HttpClient())
			{
				var json = await client.GetFromJsonAsync<List<string>>("http://ddragon.leagueoflegends.com/api/versions.json");
				patch = json.First();
			}

			return patch;
		}
	}
}
