using Microsoft.EntityFrameworkCore;
using Camille.Enums;
using Camille.RiotGames.MatchV5;
using BanMe.Entities;

namespace BanMe.Data
{
    public static class SeedData
    {

        public static async Task InitPlayerDb(IServiceProvider serviceProvider)
        {
            LeagueDataCrawler dataCrawler = new("RGAPI-0c12efe7-317f-4240-b2f2-87a9cb89736c");

			using var context = new BanMeContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeContext>>());

			Tier[] selectedTiers = { Tier.EMERALD, Tier.DIAMOND };

			foreach (Tier tier in selectedTiers)
            {
				var playerPuuids = await dataCrawler.CrawlPlayersAsync(tier, PlatformRoute.NA1);

				foreach (string puuid in playerPuuids)
				{
					context.Add(new Player { PUUID = puuid });
				}
			}

            context.SaveChanges();
        }

        public static async Task InitChampGameStatsDb(IServiceProvider serviceProvider)
        {
            LeagueDataCrawler dataCrawler = new("RGAPI-0c12efe7-317f-4240-b2f2-87a9cb89736c");

			using var context = new BanMeContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeContext>>());
            
            HashSet<Match> matches = new();

			/*foreach (Player player in context.PlayerPuuids)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(player.PUUID, RegionalRoute.AMERICAS);
                matches.UnionWith(playerMatches);
            }*/

			for (int i = 0; i < 5; i++)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(context.PlayerPuuids.ElementAt(i).PUUID, RegionalRoute.AMERICAS);
                matches.UnionWith(playerMatches);
            }

            int totalGames = matches.Count;

            Dictionary<Champion, FlatChampStats> champData = dataCrawler.GatherChampData(dataCrawler.GatherMatchData(matches));

            foreach (var data in champData)
            {
                try
                {
                    var entry = context.ChampGameStats.FirstOrDefault(e => e.ChampionName == data.Key.ToString());

                    if (entry == null)
                    {
                        ChampGameStats newEntry = new()
                        {
                            ChampionName = data.Key.ToString()
                        };

						UpdateChampGameStats(newEntry, data.Value, totalGames);

						context.ChampGameStats.Add(newEntry);
					}
                    else
                    {
						UpdateChampGameStats(entry, data.Value, totalGames);
					}
                }
                catch (Exception ex)
                {

                }
                context.SaveChanges();
            }
        }

        private static void UpdateChampGameStats(ChampGameStats entry, FlatChampStats stats, int totalGames)
        {
            entry.TopWins += stats.RoleStats[LeagueConsts.Roles.TOP].Wins;
            entry.TopPicks += stats.RoleStats[LeagueConsts.Roles.TOP].Picks;
            entry.MidWins += stats.RoleStats[LeagueConsts.Roles.MIDDLE].Wins;
            entry.MidPicks += stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks;
            entry.JungleWins += stats.RoleStats[LeagueConsts.Roles.JUNGLE].Wins;
            entry.JunglePicks += stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks;
            entry.BotWins += stats.RoleStats[LeagueConsts.Roles.BOTTOM].Wins;
            entry.BotPicks += stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks;
            entry.SuppWins += stats.RoleStats[LeagueConsts.Roles.SUPPORT].Wins;
            entry.SuppPicks += stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks;
            entry.Bans += stats.Bans;

            foreach (var matchup in stats.MatchUps)
            {
                var m = entry.MatchupStats.FirstOrDefault(m => m.EnemyChampion == matchup.Key.ToString());

				if (m != null)
                {
					int index = entry.MatchupStats.IndexOf(m);
                    entry.MatchupStats.ElementAt(index).Wins += matchup.Value.Wins;
					entry.MatchupStats.ElementAt(index).Picks += matchup.Value.Picks;
				}
                else
                {
					entry.MatchupStats.Add(new ChampMatchupStats() { EnemyChampion = matchup.Key.ToString(), Wins = matchup.Value.Wins, Picks = matchup.Value.Picks });
				}
            }

            /*entry.TopWins = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.TOP].Wins, stats.RoleStats[LeagueConsts.Roles.TOP].Picks);
            entry.TopPicks = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.TOP].Picks, totalGames);
            entry.MidWins = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.MIDDLE].Wins, stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks);
            entry.MidPicks = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks, totalGames);
            entry.JungleWins = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.JUNGLE].Wins, stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks);
            entry.JunglePicks = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks, totalGames);
            entry.BotWins = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.BOTTOM].Wins, stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks);
            entry.BotPicks = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks, totalGames);
            entry.SuppWins = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.SUPPORT].Wins, stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks);
            entry.SuppPicks = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks, totalGames);
            entry.Bans = CalcPercentageOf(stats.Bans, totalGames);*/
        }

        private static float CalcPercentageOf(float n1, float n2)
        {
            if (n2 == 0)
            {
                return 0f;
            }

            return n1 * 100 / n2;
        }
    }
}
