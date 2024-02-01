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
            LeagueDataCrawler dataCrawler = new("RGAPI-503f08cb-9cc4-47a9-8dd6-75ec5ffde053");

            List<string> players = await dataCrawler.CrawlPlayersAsync(Tier.PLATINUM, PlatformRoute.NA1);

            using var context = new BanMeContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeContext>>());

            foreach (string puuid in players)
            {
                context.Add(new Player { PUUID = puuid });
            }

            context.SaveChanges();
        }


        public static async Task InitChampGameStatsDb(IServiceProvider serviceProvider)
        {
            LeagueDataCrawler dataCrawler = new("RGAPI-503f08cb-9cc4-47a9-8dd6-75ec5ffde053");

            using var context = new BanMeContext(serviceProvider.GetRequiredService<DbContextOptions<BanMeContext>>());

            HashSet<Match> matches = new();

            /*foreach (Player player in context.PlatPuuids)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(player.PUUID, RegionalRoute.AMERICAS);
                matches.UnionWith(playerMatches);
            }*/

            for (int i = 0; i < 50; i++)
            {
                var playerMatches = await dataCrawler.CrawlMatchesAsync(context.PlatPuuids.ElementAt(i).PUUID, RegionalRoute.AMERICAS);
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
            entry.TopWinRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.TOP].Wins, stats.RoleStats[LeagueConsts.Roles.TOP].Picks);
            entry.TopPickRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.TOP].Picks, totalGames);
            entry.MidWinRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.MIDDLE].Wins, stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks);
            entry.MidPickRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.MIDDLE].Picks, totalGames);
            entry.JungleWinRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.JUNGLE].Wins, stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks);
            entry.JunglePickRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.JUNGLE].Picks, totalGames);
            entry.BotWinRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.BOTTOM].Wins, stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks);
            entry.BotPickRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.BOTTOM].Picks, totalGames);
            entry.SuppWinRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.SUPPORT].Wins, stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks);
            entry.SuppPickRate = CalcPercentageOf(stats.RoleStats[LeagueConsts.Roles.SUPPORT].Picks, totalGames);
            entry.BanRate = CalcPercentageOf(stats.Bans, totalGames);
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
