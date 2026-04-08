using System;
using System.Collections;
using UnityEngine;

public class VungleWrapper : MonoBehaviour
{
	public static VungleWrapper Instance;

	public string iOSAppID;

	public string AndroidAppID;

	public Action ShowingVungleAd;

	public Action ClosingVungleAd;

	private bool mCachedGameIsMutedState;

	private NetworkReachability mCachedConnectionState;

	public bool AdIsAvailable
	{
		get
		{
			bool flag = false;
			flag = VungleAndroid.isVideoAvailable();
			Debug.Log("Vungle: Is ad available? " + ((!flag) ? "False" : "True"));
			return flag;
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
		Debug.Log("Vungle: Registering for event callbacks");
		VungleAndroidManager.onAdStartEvent += OnAdStartEvent;
		VungleAndroidManager.onAdEndEvent += OnAdEndEvent;
		VungleAndroidManager.onCachedAdAvailableEvent += OnCachedAdAvailableEvent;
		VungleAndroidManager.onVideoViewEvent += OnVideoViewEvent;
	}

	private void Start()
	{
		Init();
		mCachedConnectionState = Application.internetReachability;
		StartCoroutine(ConnectionStateCheck());
	}

	private void Init()
	{
		VungleAndroid.init(AndroidAppID);
	}

	private void OnDestroy()
	{
		Debug.Log("Vungle: Unregistering from event callbacks");
		VungleAndroidManager.onAdStartEvent -= OnAdStartEvent;
		VungleAndroidManager.onAdEndEvent -= OnAdEndEvent;
		VungleAndroidManager.onCachedAdAvailableEvent -= OnCachedAdAvailableEvent;
		VungleAndroidManager.onVideoViewEvent -= OnVideoViewEvent;
	}

	private IEnumerator ConnectionStateCheck()
	{
		WaitForSeconds wfs = new WaitForSeconds(1f);
		while (true)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable && mCachedConnectionState == NetworkReachability.NotReachable)
			{
				Debug.Log("Vungle: Reinitialising now that an internet connection has been established");
				Init();
			}
			mCachedConnectionState = Application.internetReachability;
			yield return wfs;
		}
	}

	private void OnAdStartEvent()
	{
		Debug.Log("Vungle: Ad started");
		mCachedGameIsMutedState = MusicManager.Instance.IsMuted;
		MusicManager.Instance.IsMuted = true;
	}

	private void OnAdEndEvent()
	{
		Debug.Log("Vungle: Ad ended");
		MusicManager.Instance.IsMuted = mCachedGameIsMutedState;
		if (ClosingVungleAd != null)
		{
			ClosingVungleAd();
		}
	}

	private void OnCachedAdAvailableEvent()
	{
		Debug.Log("Vungle: Cached ad is available");
	}

	private void OnVideoViewEvent(double watched, double length)
	{
		Debug.Log(string.Format("Vungle: Video viewed. Length = {0}. Watched = {1}.", watched, length));
	}

	public bool ShowAd()
	{
		Debug.Log("Vungle: Attempting to show ad");
		if (AdIsAvailable)
		{
			if (ShowingVungleAd != null)
			{
				ShowingVungleAd();
			}
			VungleAndroid.playModalAd(true);
			return true;
		}
		return false;
	}
}
