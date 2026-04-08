using System;
using System.Collections.Generic;
using UnityEngine;

public class FacebookAndroid
{
	private static AndroidJavaObject _facebookPlugin;

	private static bool _pluginUnavailable;

	private static bool IsAvailable()
	{
		return Application.platform == RuntimePlatform.Android && !_pluginUnavailable && _facebookPlugin != null;
	}

	static FacebookAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.FacebookPlugin"))
				{
					_facebookPlugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
				}
			}
			catch (Exception ex)
			{
				_pluginUnavailable = true;
				Debug.LogWarning("Facebook Android plugin unavailable: " + ex.Message);
			}
			FacebookManager.preLoginSucceededEvent += delegate
			{
				Facebook.instance.accessToken = getAccessToken();
			};
		}
	}

	public static void init(string appId)
	{
		if (IsAvailable())
		{
			_facebookPlugin.Call("init", appId);
			Facebook.instance.accessToken = getAccessToken();
		}
	}

	public static bool isSessionValid()
	{
		if (!IsAvailable())
		{
			return false;
		}
		return _facebookPlugin.Call<bool>("isSessionValid", new object[0]);
	}

	public static string getAccessToken()
	{
		if (!IsAvailable())
		{
			return string.Empty;
		}
		return _facebookPlugin.Call<string>("getSessionToken", new object[0]);
	}

	public static void extendAccessToken()
	{
		if (IsAvailable())
		{
			_facebookPlugin.Call("extendAccessToken");
		}
	}

	public static void login()
	{
		loginWithRequestedPermissions(new string[0]);
	}

	public static void loginWithRequestedPermissions(string[] permissions, string urlSchemeSuffix)
	{
		loginWithRequestedPermissions(permissions);
	}

	public static void loginWithRequestedPermissions(string[] permissions)
	{
		if (IsAvailable())
		{
			IntPtr methodID = AndroidJNI.GetMethodID(_facebookPlugin.GetRawClass(), "showLoginDialog", "([Ljava/lang/String;)V");
			AndroidJNI.CallVoidMethod(_facebookPlugin.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(new object[1] { permissions }));
		}
	}

	public static void logout()
	{
		if (IsAvailable())
		{
			_facebookPlugin.Call("logout");
			Facebook.instance.accessToken = string.Empty;
		}
	}

	public static void showPostMessageDialog()
	{
		showDialog("stream.publish", null);
	}

	public static void showPostMessageDialogWithOptions(string link, string linkName, string linkToImage, string caption)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("link", link);
		dictionary.Add("name", linkName);
		dictionary.Add("picture", linkToImage);
		dictionary.Add("caption", caption);
		Dictionary<string, string> parameters = dictionary;
		showDialog("stream.publish", parameters);
	}

	public static void showDialog(string dialogType, Dictionary<string, string> parameters)
	{
		if (!IsAvailable())
		{
			return;
		}
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.Bundle"))
		{
			IntPtr methodID = AndroidJNI.GetMethodID(androidJavaObject.GetRawClass(), "putString", "(Ljava/lang/String;Ljava/lang/String;)V");
			object[] array = new object[2];
			if (parameters != null)
			{
				foreach (KeyValuePair<string, string> parameter in parameters)
				{
					array[0] = new AndroidJavaObject("java.lang.String", parameter.Key);
					array[1] = new AndroidJavaObject("java.lang.String", parameter.Value);
					AndroidJNI.CallVoidMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
				}
			}
			_facebookPlugin.Call("showDialog", dialogType, androidJavaObject);
		}
	}

	public static void restRequest(string restMethod, string httpMethod, Dictionary<string, string> parameters)
	{
		if (!IsAvailable())
		{
			return;
		}
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		parameters.Add("method", restMethod);
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.Bundle"))
		{
			IntPtr methodID = AndroidJNI.GetMethodID(androidJavaObject.GetRawClass(), "putString", "(Ljava/lang/String;Ljava/lang/String;)V");
			object[] array = new object[2];
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				array[0] = new AndroidJavaObject("java.lang.String", parameter.Key);
				array[1] = new AndroidJavaObject("java.lang.String", parameter.Value);
				AndroidJNI.CallVoidMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
			}
			_facebookPlugin.Call("restRequest", httpMethod, androidJavaObject);
		}
	}

	public static void graphRequest(string graphPath, string httpMethod, Dictionary<string, string> parameters)
	{
		if (!IsAvailable())
		{
			return;
		}
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.Bundle"))
		{
			IntPtr methodID = AndroidJNI.GetMethodID(androidJavaObject.GetRawClass(), "putString", "(Ljava/lang/String;Ljava/lang/String;)V");
			object[] array = new object[2];
			if (parameters != null)
			{
				foreach (KeyValuePair<string, string> parameter in parameters)
				{
					array[0] = new AndroidJavaObject("java.lang.String", parameter.Key);
					array[1] = new AndroidJavaObject("java.lang.String", parameter.Value);
					AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
				}
			}
			_facebookPlugin.Call("graphRequest", graphPath, httpMethod, androidJavaObject);
		}
	}
}
