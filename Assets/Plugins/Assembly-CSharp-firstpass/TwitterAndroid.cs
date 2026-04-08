using System.Collections.Generic;
using UnityEngine;

public class TwitterAndroid
{
	private static AndroidJavaObject _plugin;

	private static bool _pluginUnavailable;

	private static bool IsAvailable()
	{
		return Application.platform == RuntimePlatform.Android && !_pluginUnavailable && _plugin != null;
	}

	public static bool isPluginAvailable()
	{
		return IsAvailable();
	}

	static TwitterAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.TwitterPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
		catch (System.Exception ex)
		{
			_pluginUnavailable = true;
			Debug.LogWarning("Twitter Android plugin unavailable: " + ex.Message);
		}
	}

	public static void init(string consumerKey, string consumerSecret)
	{
		if (IsAvailable())
		{
			_plugin.Call("init", consumerKey, consumerSecret);
		}
	}

	public static bool isLoggedIn()
	{
		if (!IsAvailable())
		{
			return false;
		}
		return _plugin.Call<bool>("isLoggedIn", new object[0]);
	}

	public static void showLoginDialog()
	{
		if (IsAvailable())
		{
			_plugin.Call("showLoginDialog");
		}
	}

	public static void logout()
	{
		if (IsAvailable())
		{
			_plugin.Call("logout");
		}
	}

	public static void postUpdate(string update)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("status", update);
		Dictionary<string, string> parameters = dictionary;
		performRequest("post", "/1/statuses/update.json", parameters);
	}

	public static void postUpdateWithImage(string update, byte[] image)
	{
		if (IsAvailable())
		{
			_plugin.Call("postUpdateWithImage", update, image);
		}
	}

	public static void getHomeTimeline()
	{
		performRequest("get", "/1/statuses/home_timeline.json", null);
	}

	public static void getFollowers()
	{
		performRequest("get", "/1/statuses/followers.json", null);
	}

	public static void performRequest(string methodType, string path, Dictionary<string, string> parameters)
	{
		if (IsAvailable())
		{
			string text = ((parameters == null) ? string.Empty : parameters.toJson());
			_plugin.Call("performRequest", methodType, path, text);
		}
	}
}
