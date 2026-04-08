using System;
using UnityEngine;

public class EtceteraAndroid
{
	public enum ScalingMode
	{
		None = 0,
		AspectFit = 1,
		Fill = 2
	}

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

	static EtceteraAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.EtceteraPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
		catch (Exception ex)
		{
			_pluginUnavailable = true;
			Debug.LogWarning("Etcetera Android plugin unavailable: " + ex.Message);
		}
	}

	private static void DisablePlugin(string action, Exception ex)
	{
		_pluginUnavailable = true;
		_plugin = null;
		Debug.LogWarning("Etcetera Android plugin call failed during " + action + ": " + ex.Message);
	}

	private static void Call(string methodName, params object[] args)
	{
		if (!IsAvailable())
		{
			return;
		}
		try
		{
			_plugin.Call(methodName, args);
		}
		catch (Exception ex)
		{
			DisablePlugin(methodName, ex);
		}
	}

	private static bool CallBool(string methodName, params object[] args)
	{
		if (!IsAvailable())
		{
			return false;
		}
		try
		{
			return _plugin.Call<bool>(methodName, args);
		}
		catch (Exception ex)
		{
			DisablePlugin(methodName, ex);
			return false;
		}
	}

	private static void SetValue(string fieldName, object value)
	{
		if (!IsAvailable())
		{
			return;
		}
		try
		{
			_plugin.Set(fieldName, value);
		}
		catch (Exception ex)
		{
			DisablePlugin(fieldName, ex);
		}
	}

	public static void setSystemUiVisibilityToLowProfile(bool useLowProfile)
	{
		Call("setSystemUiVisibilityToLowProfile", useLowProfile);
	}

	public static void playMovie(string pathOrUrl, uint bgColor, bool showControls, ScalingMode scalingMode, bool closeOnTouch)
	{
		Call("playMovie", pathOrUrl, (int)bgColor, showControls, (int)scalingMode, closeOnTouch);
	}

	public static void showToast(string text, bool useShortDuration)
	{
		Call("showToast", text, useShortDuration);
	}

	public static void showAlert(string title, string message, string positiveButton)
	{
		showAlert(title, message, positiveButton, string.Empty);
	}

	public static void showAlert(string title, string message, string positiveButton, string negativeButton)
	{
		Call("showAlert", title, message, positiveButton, negativeButton);
	}

	public static void showAlertPrompt(string title, string message, string promptHint, string promptText, string positiveButton, string negativeButton)
	{
		Call("showAlertPrompt", title, message, promptHint, promptText, positiveButton, negativeButton);
	}

	public static void showAlertPromptWithTwoFields(string title, string message, string promptHintOne, string promptTextOne, string promptHintTwo, string promptTextTwo, string positiveButton, string negativeButton)
	{
		Call("showAlertPromptWithTwoFields", title, message, promptHintOne, promptTextOne, promptHintTwo, promptTextTwo, positiveButton, negativeButton);
	}

	public static void showProgressDialog(string title, string message)
	{
		Call("showProgressDialog", title, message);
	}

	public static void hideProgressDialog()
	{
		Call("hideProgressDialog");
	}

	public static void showWebView(string url)
	{
		Call("showWebView", url);
	}

	public static void showCustomWebView(string url, bool disableTitle, bool disableBackButton)
	{
		Call("showCustomWebView", url, disableTitle, disableBackButton);
	}

	public static void showEmailComposer(string toAddress, string subject, string text, bool isHTML)
	{
		showEmailComposer(toAddress, subject, text, isHTML, string.Empty);
	}

	public static void showEmailComposer(string toAddress, string subject, string text, bool isHTML, string attachmentFilePath)
	{
		Call("showEmailComposer", toAddress, subject, text, isHTML, attachmentFilePath);
	}

	public static bool isSMSComposerAvailable()
	{
		return CallBool("isSMSComposerAvailable", new object[0]);
	}

	public static void showSMSComposer(string text)
	{
		Call("showSMSComposer", text);
	}

	public static void promptToTakePhoto(int desiredWidth, int desiredHeight, string name)
	{
		Call("promptToTakePhoto", desiredWidth, desiredHeight, name);
	}

	public static void promptForPictureFromAlbum(int desiredWidth, int desiredHeight, string name)
	{
		Call("promptForPictureFromAlbum", desiredWidth, desiredHeight, name);
	}

	public static void promptToTakeVideo(string name)
	{
		Call("promptToTakeVideo", name);
	}

	public static bool saveImageToGallery(string pathToPhoto, string title)
	{
		return CallBool("saveImageToGallery", pathToPhoto, title);
	}

	public static void scaleImageAtPath(string pathToImage, float scale)
	{
		Call("scaleImageAtPath", pathToImage, scale);
	}

	public static void initTTS()
	{
		Call("initTTS");
	}

	public static void teardownTTS()
	{
		Call("teardownTTS");
	}

	public static void speak(string text)
	{
		speak(text, TTSQueueMode.Add);
	}

	public static void speak(string text, TTSQueueMode queueMode)
	{
		Call("speak", text, (int)queueMode);
	}

	public static void stop()
	{
		Call("stop");
	}

	public static void playSilence(long durationInMs, TTSQueueMode queueMode)
	{
		Call("playSilence", durationInMs, (int)queueMode);
	}

	public static void setPitch(float pitch)
	{
		Call("setPitch", pitch);
	}

	public static void setSpeechRate(float rate)
	{
		Call("setSpeechRate", rate);
	}

	public static void askForReview(int launchesUntilPrompt, int hoursUntilFirstPrompt, int hoursBetweenPrompts, string title, string message, bool isAmazonAppStore)
	{
		if (isAmazonAppStore)
		{
			SetValue("isAmazonAppStore", true);
		}
		Call("askForReview", launchesUntilPrompt, hoursUntilFirstPrompt, hoursBetweenPrompts, title, message);
	}

	public static void askForReviewNow(string title, string message, bool isAmazonAppStore)
	{
		if (isAmazonAppStore)
		{
			SetValue("isAmazonAppStore", true);
		}
		Call("askForReviewNow", title, message);
	}

	public static void resetAskForReview()
	{
		Call("resetAskForReview");
	}

	public static void inlineWebViewShow(string url, int x, int y, int width, int height)
	{
		Call("inlineWebViewShow", url, x, y, width, height);
	}

	public static void inlineWebViewClose()
	{
		Call("inlineWebViewClose");
	}

	public static void inlineWebViewSetUrl(string url)
	{
		Call("inlineWebViewSetUrl", url);
	}

	public static void inlineWebViewSetFrame(int x, int y, int width, int height)
	{
		Call("inlineWebViewSetFrame", x, y, width, height);
	}
}
