using UnityEngine;

public static class RecoveredCompatibility
{
	public static bool IsDeviceRuntime
	{
		get
		{
			return !Application.isEditor;
		}
	}

	public static bool IsAndroidRuntime
	{
		get
		{
			return Application.platform == RuntimePlatform.Android && IsDeviceRuntime;
		}
	}

	public static bool SkipLegacySplash
	{
		get
		{
			return IsAndroidRuntime;
		}
	}

	public static bool DisableLegacyStartupPromos
	{
		get
		{
			return IsAndroidRuntime;
		}
	}

	public static bool LimitTitleStatePreload
	{
		get
		{
			return IsAndroidRuntime;
		}
	}
}
