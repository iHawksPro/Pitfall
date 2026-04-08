using System;

[Serializable]
public class TimeCycleDay
{
	public const int HOURS_IN_DAY = 8;

	public TimeCycleHour[] m_Hours = new TimeCycleHour[8];

	public string m_Name;

	public TimeCycleDay(string name)
	{
		m_Name = name;
	}
}
