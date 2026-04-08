using UnityEngine;

public class ChartBoostAndroid
{
	private static AndroidJavaObject _plugin;

	private static bool _pluginUnavailable;

	private static bool IsAvailable()
	{
		return Application.platform == RuntimePlatform.Android && !_pluginUnavailable && _plugin != null;
	}

	static ChartBoostAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.chartboost.ChartBoostPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
		catch (System.Exception ex)
		{
			_pluginUnavailable = true;
			Debug.LogWarning("Chartboost Android plugin unavailable: " + ex.Message);
		}
	}

	public static void onStart()
	{
		if (IsAvailable())
		{
			_plugin.Call("onStart");
		}
	}

	public static void onDestroy()
	{
		if (IsAvailable())
		{
			_plugin.Call("onDestroy");
		}
	}

	public static void onStop()
	{
		if (IsAvailable())
		{
			_plugin.Call("onStop");
		}
	}

	public static void onBackPressed()
	{
		if (IsAvailable())
		{
			_plugin.Call("onBackPressed");
		}
	}

	public static void init(string appId, string appSignature)
	{
		if (IsAvailable())
		{
			_plugin.Call("init", appId, appSignature);
		}
	}

	public static void cacheInterstitial(string location)
	{
		if (IsAvailable())
		{
			if (location == null)
			{
				location = string.Empty;
			}
			_plugin.Call("cacheInterstitial", location);
		}
	}

	public static bool hasCachedInterstitial(string location)
	{
		if (!IsAvailable())
		{
			return false;
		}
		if (location == null)
		{
			location = string.Empty;
		}
		return _plugin.Call<bool>("hasCachedInterstitial", new object[1] { location });
	}

	public static void showInterstitial(string location)
	{
		if (IsAvailable())
		{
			if (location == null)
			{
				location = string.Empty;
			}
			_plugin.Call("showInterstitial", location);
		}
	}

	public static void cacheMoreApps()
	{
		if (IsAvailable())
		{
			_plugin.Call("cacheMoreApps");
		}
	}

	public static bool hasCachedMoreApps()
	{
		if (!IsAvailable())
		{
			return false;
		}
		return _plugin.Call<bool>("hasCachedMoreApps", new object[0]);
	}

	public static void showMoreApps()
	{
		if (IsAvailable())
		{
			_plugin.Call("showMoreApps");
		}
	}
}
