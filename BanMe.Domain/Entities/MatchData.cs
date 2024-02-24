using BanMe.Domain.Consts;
using Camille.Enums;
using Camille.RiotGames.MatchV5;

namespace BanMe.Domain.Entities;

public class MatchData
{
    public TeamData TeamA { get; private set; }
    public TeamData TeamB { get; private set; }

    public List<Champion> Bans;

    public MatchData(bool aWin)
    {
        Bans = new();

        if (aWin)
        {
            TeamA = new(true);
            TeamB = new(false);
        }
        else
        {
            TeamA = new(false);
            TeamB = new(true);
        }
    }

    public void SetTeamAChampion(Participant p)
    {
        SetChampion(TeamA, p);
    }

    public void SetTeamBChampion(Participant p)
    {
        SetChampion(TeamB, p);
    }

    private void SetChampion(TeamData team, Participant p)
    {
        switch (p.TeamPosition)
        {
            case LeagueConsts.Roles.TOP:
                {
                    team.ChampRole.Add(LeagueConsts.Roles.TOP, p.ChampionId);
                    break;
                }
            case LeagueConsts.Roles.MIDDLE:
                {
                    team.ChampRole.Add(LeagueConsts.Roles.MIDDLE, p.ChampionId);
                    break;
                }
            case LeagueConsts.Roles.JUNGLE:
                {
                    team.ChampRole.Add(LeagueConsts.Roles.JUNGLE, p.ChampionId);
                    break;
                }
            case LeagueConsts.Roles.BOTTOM:
                {
                    team.ChampRole.Add(LeagueConsts.Roles.BOTTOM, p.ChampionId);
                    break;
                }
            case LeagueConsts.Roles.SUPPORT:
                {
                    team.ChampRole.Add(LeagueConsts.Roles.SUPPORT, p.ChampionId);
                    break;
                }
        }
    }
}

public struct TeamData
{
    public bool Win { get; set; }

    public Dictionary<string, Champion> ChampRole { get; set; }

    public TeamData(bool win)
    {
        ChampRole = new();
        Win = win;
    }
}

public struct FlatChampStats
{
    public Dictionary<string, WinStats> RoleStats { get; set; }

    public Dictionary<Champion, WinStats> MatchUps { get; set; }

    public int Bans { get; set; }

    public FlatChampStats()
    {
        RoleStats = new() 
        {
            { LeagueConsts.Roles.TOP, new WinStats() { Wins = 0, Picks = 0 } },
            { LeagueConsts.Roles.MIDDLE, new WinStats() { Wins = 0, Picks = 0 } },
            { LeagueConsts.Roles.JUNGLE, new WinStats() { Wins = 0, Picks = 0 } },
            { LeagueConsts.Roles.BOTTOM, new WinStats() { Wins = 0, Picks = 0 } },
            { LeagueConsts.Roles.SUPPORT, new WinStats() { Wins = 0, Picks = 0 } }
        };

        MatchUps = new();
        Bans = 0;
    }
}

public struct WinStats
{
    public int Wins;
    public int Picks;
}
