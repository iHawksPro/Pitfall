using System;
using Prime31;

public class VungleAndroidManager : AbstractManager
{
	public static event Action onAdStartEvent;

	public static event Action onAdEndEvent;

	public static event Action onCachedAdAvailableEvent;

	public static event Action<double, double> onVideoViewEvent;

	static VungleAndroidManager()
	{
		AbstractManager.initialize(typeof(VungleAndroidManager));
	}

	private void onAdStart(string empty)
	{
		if (VungleAndroidManager.onAdStartEvent != null)
		{
			VungleAndroidManager.onAdStartEvent();
		}
	}

	private void onAdEnd(string empty)
	{
		if (VungleAndroidManager.onAdEndEvent != null)
		{
			VungleAndroidManager.onAdEndEvent();
		}
	}

	private void onCachedAdAvailable(string empty)
	{
		if (VungleAndroidManager.onCachedAdAvailableEvent != null)
		{
			VungleAndroidManager.onCachedAdAvailableEvent();
		}
	}

	private void onVideoView(string str)
	{
		if (VungleAndroidManager.onVideoViewEvent != null)
		{
			string[] array = str.Split('-');
			if (array.Length == 2)
			{
				VungleAndroidManager.onVideoViewEvent(double.Parse(array[0]), double.Parse(array[1]));
			}
		}
	}
}
