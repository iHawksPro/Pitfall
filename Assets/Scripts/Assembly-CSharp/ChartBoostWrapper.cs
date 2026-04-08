using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartBoostWrapper : MonoBehaviour
{
	public const string kLocationMainMenu = "Main Menu";

	public const string kLocationLevelSelectScreen = "Level Select Screen";

	public const string kLocationIAPStore = "IAP Store";

	public static ChartBoostWrapper Instance;

	public string AndroidAppID;

	public string AndroidAppSignature;

	private bool mHasCheckedIfIsNewPlayer;

	private List<string> mAlreadyCachingInterstitialForLocations = new List<string>();

	private string mLastRequestedLocation;

	private NetworkReachability mCachedConnectionState;

	private bool mIsInitialised;

	private float mRequiresReinitAfter;

	public bool IsNewPlayer { get; private set; }

	public bool IsPlayingAd { get; private set; }

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
		mCachedConnectionState = Application.internetReachability;
	}

	private void OnEnable()
	{
		ChartBoostAndroidManager.didFailToLoadInterstitialEvent += OnDidFailToLoadInterstitialEvent;
		ChartBoostAndroidManager.didFinishInterstitialEvent += OnDidFinishInterstitialEvent;
		ChartBoostAndroidManager.didCacheInterstitialEvent += OnDidCacheInterstitialEvent;
		ChartBoostAndroidManager.didShowInterstitialEvent += OnDidShowInterstitialEvent;
		Init();
		StartCoroutine(ConnectionStateCheck());
	}

	private void OnDisable()
	{
		ChartBoostAndroidManager.didFailToLoadInterstitialEvent -= OnDidFailToLoadInterstitialEvent;
		ChartBoostAndroidManager.didFinishInterstitialEvent -= OnDidFinishInterstitialEvent;
		ChartBoostAndroidManager.didCacheInterstitialEvent -= OnDidCacheInterstitialEvent;
		ChartBoostAndroidManager.didShowInterstitialEvent -= OnDidShowInterstitialEvent;
	}

	private void Init()
	{
		if (mIsInitialised)
		{
			Debug.Log("Chartboost: Ignoring Init() as we are already initialised");
			return;
		}
		Debug.Log("Chartboost: Initialising...");
		ChartBoostAndroid.init(AndroidAppID, AndroidAppSignature);
		mAlreadyCachingInterstitialForLocations.Clear();
		CacheInterstitial("Main Menu");
		CacheInterstitial("Level Select Screen");
		CacheInterstitial("IAP Store");
		mIsInitialised = true;
	}

	private IEnumerator ConnectionStateCheck()
	{
		WaitForSeconds wfs = new WaitForSeconds(1f);
		while (true)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable && mCachedConnectionState == NetworkReachability.NotReachable)
			{
				Debug.Log("Chartboost: Reinitialising now that an internet connection has been established");
				Init();
			}
			if (Application.internetReachability == NetworkReachability.NotReachable && mCachedConnectionState != NetworkReachability.NotReachable)
			{
				Debug.Log("Chartboost: Deinitialising as we lost the internet connection");
				mIsInitialised = false;
			}
			mCachedConnectionState = Application.internetReachability;
			yield return wfs;
		}
	}

	private void Update()
	{
		if (!mIsInitialised && Time.time > mRequiresReinitAfter)
		{
			Init();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			Debug.Log("Chartboost: Reinitialising as the application has regained focus");
			mRequiresReinitAfter = Time.time + 5f;
		}
		else
		{
			Debug.Log("Chartboost: Deinitialising as the application has lost focus");
			mIsInitialised = false;
		}
	}

	public void ShowAd(string location)
	{
		if (Application.isEditor)
		{
			Debug.Log("Chartboost: Skipping ad request in editor for " + location);
			return;
		}
		if (!mHasCheckedIfIsNewPlayer)
		{
			Debug.Log("Is Bedrock active? " + ((!Bedrock.isBedrockActive()) ? "false" : "true"));
			mHasCheckedIfIsNewPlayer = true;
			IsNewPlayer = Bedrock.GetUserVariableAsBool("NewPlayer", true);
			Bedrock.SetUserVariableAsBool("NewPlayer", false);
		}
		if (IsNewPlayer)
		{
			Debug.Log("Chartboost: This is a new player and no ads will be shown on their first boot.");
		}
		else if (ChartBoostAndroid.hasCachedInterstitial(location))
		{
			Debug.Log("Chartboost: Attempting To show an interstitial for " + location);
			ChartBoostAndroid.showInterstitial(location);
		}
		else
		{
			Debug.Log("Chartboost: No Interstitial cached, attempting to cache one now for " + location);
			CacheInterstitial(location);
		}
	}

	private void OnDidFailToLoadInterstitialEvent(string location)
	{
		Debug.Log("Chartboost: Failed to load an interstitial for " + location);
		IsPlayingAd = false;
	}

	private void OnDidFinishInterstitialEvent(string reason)
	{
		Debug.Log("Chartboost: Finished an interstitial, reason: " + reason);
		switch (reason)
		{
		case "close":
			IsPlayingAd = false;
			UnlockGame(true);
			break;
		case "click":
			IsPlayingAd = false;
			UnlockGame(true);
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				EtceteraPlatformWrapper.ShowAlert("S_IAP_NO_NET_TITLE", "S_IAP_NO_NET_BODY", "S_IAP_OK");
			}
			break;
		}
	}

	private void OnDidCacheInterstitialEvent(string location)
	{
		Debug.Log("Chartboost: Cached an interstitial for " + location);
		mAlreadyCachingInterstitialForLocations.Remove(location);
	}

	private void OnDidShowInterstitialEvent(string location)
	{
		Debug.Log("Chartboost: Showing an interstitial for " + location);
		mLastRequestedLocation = location;
		IsPlayingAd = true;
		LockGame();
	}

	private void LockGame()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if ((bool)gameController && !gameController.IsPaused)
		{
			UIButton.sInputLocked = true;
			gameController.PauseGame();
		}
	}

	private void UnlockGame(bool shouldCacheInterstitial)
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if ((bool)gameController && gameController.IsPaused)
		{
			UIButton.sInputLocked = false;
			gameController.UnpauseThings();
		}
		if (shouldCacheInterstitial)
		{
			CacheInterstitial(mLastRequestedLocation);
		}
	}

	private void CacheInterstitial(string location)
	{
		if (ChartBoostAndroid.hasCachedInterstitial(location))
		{
			Debug.Log("Chartboost: Already have a cached interstitial for " + location);
			return;
		}
		if (mAlreadyCachingInterstitialForLocations.Contains(location))
		{
			Debug.Log("Chartboost: Already attempting to cache an interstitial for " + location);
		}
		mAlreadyCachingInterstitialForLocations.Add(location);
		ChartBoostAndroid.cacheInterstitial(location);
	}
}
