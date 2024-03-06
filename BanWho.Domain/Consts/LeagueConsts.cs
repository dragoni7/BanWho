namespace BanWho.Domain.Consts;

public class LeagueConsts
{
    public static class Roles
    {
        public const string ANY = "ANY";

        public const string TOP = "TOP";

        public const string MIDDLE = "MIDDLE";

        public const string JUNGLE = "JUNGLE";

        public const string BOTTOM = "BOTTOM";

        public const string SUPPORT = "UTILITY";

        public static readonly string[] ALL = [TOP, MIDDLE, JUNGLE, BOTTOM, SUPPORT];
    }
}
