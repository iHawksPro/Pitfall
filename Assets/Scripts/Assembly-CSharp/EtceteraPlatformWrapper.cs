using UnityEngine;

public class EtceteraPlatformWrapper
{
	private static string LocalizeString(string inStr)
	{
		if (inStr.StartsWith("S_"))
		{
			inStr = Language.Get(inStr);
		}
		return inStr;
	}

	public static void ShowWaitingWithLabel(string strLabel)
	{
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			Debug.LogWarning("Etcetera waiting dialog unavailable: " + strLabel);
			return;
		}
		EtceteraAndroid.showProgressDialog(string.Empty, strLabel);
	}

	public static void ShowBlankWaiting()
	{
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			Debug.LogWarning("Etcetera waiting dialog unavailable.");
			return;
		}
		EtceteraAndroid.showProgressDialog(string.Empty, string.Empty);
	}

	public static void HideWaitingDialog()
	{
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			return;
		}
		EtceteraAndroid.hideProgressDialog();
	}

	public static void ShowAlert(string strTitle, string strBody, string strOK)
	{
		strTitle = LocalizeString(strTitle);
		strBody = LocalizeString(strBody);
		strOK = LocalizeString(strOK);
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			Debug.LogWarning("Etcetera alert unavailable: " + strTitle + " / " + strBody);
			return;
		}
		EtceteraAndroid.showAlert(strTitle, strBody, strOK);
	}

	public static void ShowWebPage(string strURI)
	{
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			Application.OpenURL(strURI);
			return;
		}
		EtceteraAndroid.showWebView(strURI);
	}

	public static void SetBadgeCount(int iBadge)
	{
	}

	public static void ShowAlert(string title, string message, string positiveButton, string negativeButton)
	{
		title = LocalizeString(title);
		message = LocalizeString(message);
		positiveButton = LocalizeString(positiveButton);
		negativeButton = LocalizeString(negativeButton);
		if (Application.platform == RuntimePlatform.Android && !EtceteraAndroid.isPluginAvailable())
		{
			Debug.LogWarning("Etcetera alert unavailable: " + title + " / " + message);
			return;
		}
		EtceteraAndroid.showAlert(title, message, positiveButton, negativeButton);
	}
}
