namespace BanMe.Domain.Consts;

public class BanMeConsts
{
	public static class StatThresholds
	{
		public const float MinWinRate = 49f;

		public const float MaxWinRate = 100f;

		public const float MinPicks = 10f;
	}

	public static class DataThresholds
	{
		public const int DataAmountToDisplay = 10;

		public const int MatchesTrackedPerPlayer = 20;
	}
}
