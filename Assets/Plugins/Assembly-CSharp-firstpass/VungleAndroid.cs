using UnityEngine;

public class VungleAndroid
{
	private static AndroidJavaObject _plugin;

	private static bool _pluginUnavailable;

	private static bool IsAvailable()
	{
		return Application.platform == RuntimePlatform.Android && !_pluginUnavailable && _plugin != null;
	}

	static VungleAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.VunglePlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
		catch (System.Exception ex)
		{
			_pluginUnavailable = true;
			Debug.LogWarning("Vungle Android plugin unavailable: " + ex.Message);
		}
	}

	public static void init(string appId)
	{
		if (IsAvailable())
		{
			_plugin.Call("init", appId);
		}
	}

	public static void onPause()
	{
		if (IsAvailable())
		{
			_plugin.Call("onPause");
		}
	}

	public static void onResume()
	{
		if (IsAvailable())
		{
			_plugin.Call("onResume");
		}
	}

	public static bool isVideoAvailable()
	{
		if (!IsAvailable())
		{
			return false;
		}
		return _plugin.Call<bool>("isVideoAvailable", new object[0]);
	}

	public static void setSoundEnabled(bool isEnabled)
	{
		if (IsAvailable())
		{
			_plugin.Call("setSoundEnabled", isEnabled);
		}
	}

	public static void setAdOrientation(VungleAdOrientation orientation)
	{
		if (IsAvailable())
		{
			_plugin.Call("setAdOrientation", (int)orientation);
		}
	}

	public static bool isSoundEnabled()
	{
		if (!IsAvailable())
		{
			return true;
		}
		return _plugin.Call<bool>("isSoundEnabled", new object[0]);
	}

	public static void playAd(bool showCloseButton, bool incentivized = false, string user = "")
	{
		if (IsAvailable())
		{
			if (user == null)
			{
				user = string.Empty;
			}
			_plugin.Call("playAd", showCloseButton, incentivized, user);
		}
	}

	public static void playModalAd(bool showCloseButton)
	{
		playAd(showCloseButton, false, string.Empty);
	}

	public static void playIncentivizedAd(string user, bool showCloseButton)
	{
		playAd(showCloseButton, true, user);
	}
}
