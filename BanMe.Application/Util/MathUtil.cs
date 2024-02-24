namespace BanMe.Application.Util;

public static class MathUtil
{
	public static float AsPercentageOf(float n1, float n2)
	{
		if (n2 == 0)
		{
			return 0f;
		}

		return n1 * 100 / n2;
	}
}
