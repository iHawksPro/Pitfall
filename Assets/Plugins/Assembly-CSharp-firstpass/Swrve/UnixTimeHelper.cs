using System;

namespace Swrve
{
	public class UnixTimeHelper
	{
		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long GetSeconds()
		{
			return (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
		}

		public static long GetMilliseconds()
		{
			return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
		}
	}
}
