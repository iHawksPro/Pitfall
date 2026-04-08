using System;
using System.Globalization;

public static class DateTimeExtensions
{
	public static int WeekNumber(this DateTime value)
	{
		return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}
}
