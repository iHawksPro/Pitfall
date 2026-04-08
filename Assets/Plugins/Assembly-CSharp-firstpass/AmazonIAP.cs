using System;
using UnityEngine;

public class AmazonIAP
{
	private static AndroidJavaObject _plugin;

	private static bool _pluginUnavailable;

	private static bool IsAvailable()
	{
		return Application.platform == RuntimePlatform.Android && !_pluginUnavailable && _plugin != null;
	}

	static AmazonIAP()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.amazon.AmazonIAPPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
		catch (Exception ex)
		{
			_pluginUnavailable = true;
			Debug.LogWarning("Amazon IAP Android plugin unavailable: " + ex.Message);
		}
	}

	public static void initiateItemDataRequest(string[] items)
	{
		if (IsAvailable())
		{
			IntPtr methodID = AndroidJNI.GetMethodID(_plugin.GetRawClass(), "initiateItemDataRequest", "([Ljava/lang/String;)V");
			AndroidJNI.CallVoidMethod(_plugin.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(new object[1] { items }));
		}
	}

	public static void initiatePurchaseRequest(string sku)
	{
		if (IsAvailable())
		{
			_plugin.Call("initiatePurchaseRequest", sku);
		}
	}

	public static void initiateGetUserIdRequest()
	{
		if (IsAvailable())
		{
			_plugin.Call("initiateGetUserIdRequest");
		}
	}
}
