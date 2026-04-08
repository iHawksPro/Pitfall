using UnityEngine;

public class ScreenTimeout
{
	private static ScreenTimeout m_instance;

	public static ScreenTimeout Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new ScreenTimeout();
			}
			return m_instance;
		}
	}

	public bool AllowTimeout
	{
		get
		{
			return Screen.sleepTimeout != -1;
		}
		set
		{
			Screen.sleepTimeout = ((!value) ? (-1) : (-2));
		}
	}

	private ScreenTimeout()
	{
	}
}
