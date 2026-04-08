using UnityEngine;

public class TBFUtils
{
	private static AndroidJavaObject TBFUtilsObject;

	private static bool TBFUtilsUnavailable;

	private static float UIScaling;

	private static float InverseUIScaling;

	private static float UITextScaling;

	private static float InverseUITextScaling;

	private static float ScreenAspectRatio;

	public static string BuildTime
	{
		get
		{
			return "--:--:--";
		}
	}

	public static string BuildDate
	{
		get
		{
			return "<not build>";
		}
	}

	public static string BuildVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = CallTBFJar<string>("getBuildVersion");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				return "1.0.0";
			}
			return BundleVersion;
		}
	}

	public static string BundleVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = CallTBFJar<string>("getBundleVersion");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				return "1.0.0";
			}
			return "1.0";
		}
	}

	public static bool IsMusicPlaying
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return false;
			}
			return false;
		}
	}

	public static float VolumeLevel
	{
		get
		{
			return 1f;
		}
	}

	public static bool IsCracked
	{
		get
		{
			return false;
		}
	}

	public static string iPhoneGen
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return SystemInfo.deviceModel;
			}
			return "Unknown";
		}
	}

	public static string DeviceManfacturer
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = CallTBFJar<string>("getDeviceManufacturer");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return "Unknown";
		}
	}

	public static string DeviceModel
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = CallTBFJar<string>("getDeviceModel");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return SystemInfo.deviceModel;
		}
	}

	public static string OSVersion
	{
		get
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				string text = CallTBFJar<string>("getOSVersion");
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return SystemInfo.operatingSystem;
		}
	}

	public static string ODIN
	{
		get
		{
			return "123456789";
		}
	}

	public static string DeviceLocaleLanguage
	{
		get
		{
			return "en-US";
		}
	}

	private static AndroidJavaObject GetTBFUtilsObject()
	{
		if (TBFUtilsUnavailable)
		{
			return null;
		}
		if (TBFUtilsObject == null)
		{
			try
			{
				TBFUtilsObject = new AndroidJavaObject("com.activision.TBFUtils");
				if (TBFUtilsObject == null)
				{
					DebugLog("Could not create java object for TBFUtils");
				}
				else
				{
					CallTBFJar("Initialise");
				}
			}
			catch (System.Exception ex)
			{
				TBFUtilsUnavailable = true;
				Debug.LogWarning("TBFUtils Android bridge unavailable: " + ex.Message);
				TBFUtilsObject = null;
			}
		}
		return TBFUtilsObject;
	}

	public static void CallTBFJar(string function)
	{
		AndroidJavaObject tBFUtilsObject = GetTBFUtilsObject();
		if (tBFUtilsObject != null)
		{
			tBFUtilsObject.Call(function);
			DebugLog("function: " + function);
		}
		else
		{
			DebugLog("TBFUtilsObject was null " + function);
		}
	}

	public static T CallTBFJar<T>(string function)
	{
		AndroidJavaObject tBFUtilsObject = GetTBFUtilsObject();
		if (tBFUtilsObject != null)
		{
			try
			{
				T result = tBFUtilsObject.Call<T>(function, new object[0]);
				if (result != null)
				{
					DebugLog("function: " + function + " " + result.ToString());
				}
				return result;
			}
			catch (System.Exception ex)
			{
				TBFUtilsUnavailable = true;
				Debug.LogWarning("TBFUtils call failed for '" + function + "': " + ex.Message);
			}
		}
		DebugLog("TBFUtilsObject was null " + function);
		return default(T);
	}

	public static void LaunchURL(string URL)
	{
	}

	public static void LaunchStoreForRating()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CallTBFJar("launchMarket");
		}
	}

	public static void CancelLocalNotificationWithKeyVal(string Key, string Value)
	{
	}

	public static void ShowInterstitialAd(string Url, string Referrer)
	{
	}

	public static bool Is256mbDevice()
	{
		if (SystemInfo.systemMemorySize < 512)
		{
			return true;
		}
		return false;
	}

	public static bool Is512mbDevice()
	{
		return Is256mbDevice();
	}

	public static bool IsSmallScreenDevice()
	{
		if (Screen.height <= 640)
		{
			return true;
		}
		return false;
	}

	public static bool IsRetinaHdDevice()
	{
		return false;
	}

	public static bool IsDeviceAniPad()
	{
		return false;
	}

	public static void DebugLog(string strMsg)
	{
	}

	public static void ForceRecalculateOnUIScales()
	{
		CalculateUIScaling();
	}

	public static float GetAspectRatio()
	{
		if (ScreenAspectRatio == 0f)
		{
			CalculateUIScaling();
		}
		return ScreenAspectRatio;
	}

	public static float GetUIScaling()
	{
		if (UIScaling == 0f)
		{
			CalculateUIScaling();
		}
		return UIScaling;
	}

	public static float GetInverseUIScaling()
	{
		if (InverseUIScaling == 0f)
		{
			CalculateUIScaling();
		}
		return InverseUIScaling;
	}

	public static float GetUITextScaling()
	{
		if (UITextScaling == 0f)
		{
			CalculateUIScaling();
		}
		return UITextScaling;
	}

	public static float GetInverseUITextScaling()
	{
		if (InverseUITextScaling == 0f)
		{
			CalculateUIScaling();
		}
		return InverseUITextScaling;
	}

	private static void CalculateUIScaling()
	{
		UIScaling = 2f;
		UITextScaling = 1f;
		int num = Screen.height;
		int num2 = Screen.width;
		if (Screen.height > Screen.width)
		{
			num = Screen.width;
			num2 = Screen.height;
		}
		ScreenAspectRatio = (float)num2 / (float)num;
		DebugLog("Screen Aspect Ratio " + ScreenAspectRatio + "(" + num2 + "/" + num + ")");
		DebugLog("System Memory Size " + SystemInfo.systemMemorySize);
		if (num == 320)
		{
			UITextScaling = 2f;
		}
		UITextScaling = 768f / (float)num;
		InverseUIScaling = 1f / UIScaling;
		InverseUITextScaling = 1f / UITextScaling;
	}
}
