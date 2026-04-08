using UnityEngine;

public class VungleAndroid
{
	private static AndroidJavaObject _plugin;

	static VungleAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.VunglePlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void init(string appId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", appId);
		}
	}

	public static void onPause()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onPause");
		}
	}

	public static void onResume()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onResume");
		}
	}

	public static bool isVideoAvailable()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("isVideoAvailable", new object[0]);
	}

	public static void setSoundEnabled(bool isEnabled)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setSoundEnabled", isEnabled);
		}
	}

	public static void setAdOrientation(VungleAdOrientation orientation)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setAdOrientation", (int)orientation);
		}
	}

	public static bool isSoundEnabled()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		return _plugin.Call<bool>("isSoundEnabled", new object[0]);
	}

	public static void playAd(bool showCloseButton, bool incentivized = false, string user = "")
	{
		if (Application.platform == RuntimePlatform.Android)
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
