using System;
using System.Text;
using UnityEngine;

public static class TimeUtils
{
	public static readonly long UnixEpochTicks = new DateTime(1970, 1, 1).Ticks;

	public static readonly DateTime UnixEpoch = new DateTime(UnixEpochTicks, DateTimeKind.Utc);

	public static readonly uint SecondsPerHour = 3600u;

	public static readonly uint SecondsPerDay = 86400u;

	public static DateTime GetDateTimeFromUnixUtcTime(uint secondsSinceUnixEpoch)
	{
		long ticks = UnixEpochTicks + (long)secondsSinceUnixEpoch * 10000000L;
		return new DateTime(ticks, DateTimeKind.Utc);
	}

	public static uint GetSecondsSinceUnixEpoch(DateTime dateTime)
	{
		DateTime dateTime2 = dateTime.ToUniversalTime();
		double num = Math.Round((dateTime2 - UnixEpoch).TotalSeconds);
		if (num < 0.0)
		{
			Debug.LogError(string.Concat("Cannot convert time '", dateTime, "' that was before unix epoch!"));
			return 0u;
		}
		return Convert.ToUInt32(num);
	}

	public static string GetFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			string arg = Language.Get((timeSpan.Days != 1) ? "S_TIME_DAYS" : "S_TIME_DAY");
			return string.Format("{0} {1}", timeSpan.Days, arg);
		}
		if (timeSpan.TotalHours > 1.0)
		{
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0} {1}", num, Language.Get("S_TIME_HOURS"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0} {1}", num2, Language.Get("S_TIME_MINUTES"));
		}
		if (timeSpan.Seconds > 1)
		{
			return string.Format("{0} {1}", timeSpan.Seconds, Language.Get("S_TIME_SECONDS"));
		}
		return Language.Get("S_TIME_NOW");
	}

	public static string GetShortFuzzyTimeStringFromSeconds(uint seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return string.Format("{0}{1}", timeSpan.Days, Language.Get("S_TIME_DAY_ABBREV"));
		}
		if (timeSpan.TotalHours > 1.0)
		{
			double num = Math.Ceiling(timeSpan.TotalHours);
			return string.Format("{0}{1}", num, Language.Get("S_TIME_HOUR_ABBREV"));
		}
		if (timeSpan.TotalMinutes > 1.0)
		{
			double num2 = Math.Ceiling(timeSpan.TotalMinutes);
			return string.Format("{0}{1}", num2, Language.Get("S_TIME_MINUTE_ABBREV"));
		}
		return string.Format("{0}{1}", timeSpan.Seconds, Language.Get("S_TIME_SECOND_ABBREV"));
	}

	public static string GetLongTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_DAY", "S_TIME_DAYS", timeSpan.Days);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_HOUR", "S_TIME_HOURS", timeSpan.Hours);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_MINUTE", "S_TIME_MINUTES", timeSpan.Minutes);
		AppendLocalizedTimeText(stringBuilder, "S_TIME_SECOND", "S_TIME_SECONDS", timeSpan.Seconds);
		return stringBuilder.ToString();
	}

	private static void AppendLocalizedTimeText(StringBuilder buffer, string singularIdentifier, string pluralIdentifier, int wholeAmount)
	{
		if (wholeAmount > 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			buffer.AppendFormat("{0} {1}", wholeAmount, Language.Get(pluralIdentifier));
		}
		else if (wholeAmount == 1)
		{
			if (buffer.Length > 0)
			{
				buffer.Append(" ");
			}
			buffer.AppendFormat("{0} {1}", wholeAmount, Language.Get(singularIdentifier));
		}
	}

	public static string GetShortTimeStringFromSeconds(long seconds)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.TotalDays >= 1.0)
		{
			return stringBuilder.Append("> ").Append(timeSpan.Days).Append(Language.Get("S_TIME_DAY_ABBREV"))
				.ToString();
		}
		if (timeSpan.TotalHours >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Hours).Append(Language.Get("S_TIME_HOUR_ABBREV"));
		}
		if (timeSpan.TotalMinutes >= 1.0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Minutes).Append(Language.Get("S_TIME_MINUTE_ABBREV"));
		}
		if (timeSpan.Seconds > 0)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(timeSpan.Seconds).Append(Language.Get("S_TIME_SECOND_ABBREV"));
		}
		return stringBuilder.ToString();
	}

	public static uint GetSecondsSince(uint startTime, uint currentTime)
	{
		return (currentTime > startTime) ? (currentTime - startTime) : 0u;
	}

	public static string FloatToMMSShhString(float fTimerValue)
	{
		long num = (long)fTimerValue;
		long num2 = num % 60;
		long num3 = num / 60;
		long num4 = (long)((fTimerValue - (float)num) * 100f);
		return num3.ToString("00") + ":" + num2.ToString("00") + "." + num4.ToString("00");
	}

	public static string FloatToMMSShString(float fTimerValue)
	{
		long num = (long)fTimerValue;
		long num2 = num % 60;
		long num3 = num / 60;
		long num4 = (long)((fTimerValue - (float)num) * 100f) / 10;
		return num3.ToString("00") + ":" + num2.ToString("00") + "." + num4.ToString("0");
	}

	public static string FloatToMMSSString(float fTimerValue)
	{
		long num = (long)fTimerValue;
		long num2 = num % 60;
		return (num / 60).ToString("00") + ":" + num2.ToString("00");
	}
}
