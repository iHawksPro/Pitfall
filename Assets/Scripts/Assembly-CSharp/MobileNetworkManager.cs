using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileNetworkManager : MonoBehaviour
{
	private enum FacebookOp
	{
		FB_NONE = 0,
		FB_LOGIN = 1,
		FB_POST_MESSAGE = 2
	}

	private enum TwitterOp
	{
		TW_NONE = 0,
		TW_LOGIN = 1,
		TW_POST_MESSAGE = 2
	}

	public static MobileNetworkManager m_instance;

	public string m_facebookAppID;

	public string m_twitterConsumerKey;

	public string m_twitterConsumerSecret;

	private IMobileNetworkImpl m_impl;

	private bool m_isLoggedIn;

	private bool m_loginInProgress;

	private bool m_waitingForLogin;

	private bool m_firstLogin;

	private float loginStartTime;

	private float lastErrorTime;

	private bool m_loginErrorShown;

	private FacebookOp m_currentFbOp;

	private FacebookOp m_pendingFbOp;

	private string m_pendingFbCaption;

	private string m_pendingFbLink;

	private string m_pendingFbLinkName;

	private string m_pendingFbDesc;

	private string m_pendingFbImage;

	private TwitterOp m_currentTwOp;

	private TwitterOp m_pendingTwOp;

	private string m_pendingTwitterCaption;

	public static MobileNetworkManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsLoggedIn
	{
		get
		{
			return m_isLoggedIn;
		}
	}

	public bool IsLoginInProgress
	{
		get
		{
			return m_loginInProgress;
		}
	}

	public bool SupportsAchievements
	{
		get
		{
			bool result = false;
			if (m_impl != null)
			{
				result = m_impl.supportsAchievements();
			}
			return result;
		}
	}

	public string PlayerAlias
	{
		get
		{
			if (m_impl != null)
			{
				return m_impl.PlayerAlias();
			}
			return string.Empty;
		}
	}

	public static event Action playerAuthenticated;

	public static event Action<string> playerFailedToAuthenticate;

	public static event Action playerLoggedOut;

	public static event Action<string> reportScoreFailed;

	public static event Action<string> reportScoreFinished;

	public static event Action<string> retrieveScoresFailed;

	public static event Action<List<MobileNetworkLeaderboardScore>> scoresLoaded;

	public static event Action<string> loadAchievementsFailed;

	public static event Action<List<MobileNetworkAchievement>> achievementsLoaded;

	private void Awake()
	{
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		m_isLoggedIn = false;
		CreateNetworkImpl();
		FacebookAndroid.init(m_facebookAppID);
		TwitterAndroid.init(m_twitterConsumerKey, m_twitterConsumerSecret);
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void CreateNetworkImpl()
	{
		m_impl = base.gameObject.AddComponent<MobileNetworkBedrock>();
		m_impl.Init();
	}

	public void reportScore(long score, string leaderboardID)
	{
		if (m_impl != null)
		{
			m_impl.reportScore(score, leaderboardID);
		}
	}

	public void retrieveScores(bool friendsOnly, string leaderboardID, int startRank, int endRank)
	{
		if (m_impl != null)
		{
			m_impl.retrieveScores(friendsOnly, leaderboardID, startRank, endRank);
		}
	}

	public void showLeaderboards()
	{
		if (!m_isLoggedIn)
		{
			LoginWaitAndPerformAction(showLeaderboards);
		}
		else
		{
			m_impl.showLeaderboards();
		}
	}

	public void getAchievements()
	{
		if (m_impl != null)
		{
			m_impl.getAchievements();
		}
	}

	public void reportAchievement(string AchievementID, float PercentComplete)
	{
		reportAchievement(AchievementID, PercentComplete, true);
	}

	public void reportAchievement(string AchievementID, float PercentComplete, bool ShowBanner)
	{
		if (m_impl != null)
		{
			m_impl.reportAchievement(AchievementID, PercentComplete, ShowBanner);
			if (PercentComplete >= 100f)
			{
				SwrveEventsProgression.AchievementCompleted(AchievementID);
			}
		}
	}

	public void resetAchievements()
	{
		if (m_impl != null)
		{
			m_impl.resetAchievements();
		}
	}

	public void showAchievements()
	{
		if (!m_isLoggedIn)
		{
			LoginWaitAndPerformAction(showAchievements);
		}
		else
		{
			m_impl.showAchievements();
		}
	}

	public void Login()
	{
		m_firstLogin = true;
		StartLogin();
	}

	private bool StartLogin()
	{
		bool result = false;
		if (!m_loginInProgress)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable || !m_impl.IsMobileNetworkAvailable())
			{
				OnNoNetwork();
			}
			else if (!m_impl.IsPlayerAuthenticated())
			{
				m_impl.AuthenticateLocalPlayer();
				m_loginInProgress = true;
				result = true;
				loginStartTime = Time.realtimeSinceStartup;
			}
		}
		return result;
	}

	private void LoginWaitAndPerformAction(Action onComplete)
	{
		if (m_waitingForLogin)
		{
			return;
		}
		bool flag = false;
		if (m_loginInProgress)
		{
			flag = true;
		}
		else
		{
			m_loginErrorShown = false;
			if (StartLogin())
			{
				flag = true;
			}
		}
		if (flag)
		{
			StartCoroutine(WaitForLogin(onComplete));
		}
		m_waitingForLogin = flag;
	}

	private IEnumerator WaitForLogin(Action onComplete)
	{
		EtceteraPlatformWrapper.ShowWaitingWithLabel(Language.Get("S_NET_CONNECTING_DROID"));
		if (UIManager.instance != null)
		{
			UIManager.instance.blockInput = true;
		}
		while (m_loginInProgress)
		{
			yield return null;
		}
		EtceteraPlatformWrapper.HideWaitingDialog();
		if (UIManager.instance != null)
		{
			UIManager.instance.blockInput = false;
		}
		if (m_isLoggedIn)
		{
			onComplete();
		}
		else if (!m_loginErrorShown && !m_firstLogin)
		{
			OnLoginFailed();
		}
		m_firstLogin = false;
		m_loginInProgress = false;
		m_waitingForLogin = false;
	}

	private void ShowSysDialog(string TitleKey, string BodyKey)
	{
		EtceteraPlatformWrapper.ShowAlert(Language.Get(TitleKey), Language.Get(BodyKey), Language.Get("S_OK"));
	}

	private void OnNoNetwork()
	{
		ShowSysDialog("S_NET_NONETWORK_TITLE", "S_NET_NONETWORK_BODY_DROID");
	}

	private void OnLoginFailed()
	{
		ShowSysDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID");
	}

	private void OnLoginCancelled()
	{
		ShowSysDialog("S_NET_DISABLED_TITLE_DROID", "S_NET_DISABLED_BODY_DROID");
	}

	public void _OnPlayerAuthenticated()
	{
		TBFUtils.DebugLog(m_impl.PlayerAlias() + " logged into mobile network");
		m_isLoggedIn = true;
		m_loginInProgress = false;
		m_firstLogin = false;
		if (MobileNetworkManager.playerAuthenticated != null)
		{
			MobileNetworkManager.playerAuthenticated();
		}
	}

	public void _OnPlayerNotAuthenticated(string Error)
	{
		TBFUtils.DebugLog("Failed to log into mobile network: " + Error);
		m_isLoggedIn = false;
		m_loginInProgress = false;
		float num = Time.realtimeSinceStartup - loginStartTime;
		if (Error.Contains("cancelled"))
		{
			if (num < 0.5f)
			{
				m_loginErrorShown = true;
				OnLoginCancelled();
			}
		}
		else if (Time.realtimeSinceStartup - lastErrorTime > 1f)
		{
			lastErrorTime = Time.realtimeSinceStartup;
			m_loginErrorShown = true;
			OnLoginFailed();
		}
		if (MobileNetworkManager.playerFailedToAuthenticate != null)
		{
			MobileNetworkManager.playerFailedToAuthenticate(Error);
		}
	}

	public void _OnPlayerLoggedOut()
	{
		TBFUtils.DebugLog("Player logged out");
		m_isLoggedIn = false;
		m_loginInProgress = false;
		if (MobileNetworkManager.playerLoggedOut != null)
		{
			MobileNetworkManager.playerLoggedOut();
		}
	}

	public void _OnReportScoreFailed(string Error)
	{
		TBFUtils.DebugLog("Reporting score failed: " + Error);
		if (MobileNetworkManager.reportScoreFailed != null)
		{
			MobileNetworkManager.reportScoreFailed(Error);
		}
	}

	public void _OnReportScoreFinished(string Msg)
	{
		TBFUtils.DebugLog("Reporting score finished: " + Msg);
		if (MobileNetworkManager.reportScoreFinished != null)
		{
			MobileNetworkManager.reportScoreFinished(Msg);
		}
	}

	public void _OnRetrieveScoresFailed(string Error)
	{
		TBFUtils.DebugLog("Retrieve scores failed: " + Error);
		if (MobileNetworkManager.retrieveScoresFailed != null)
		{
			MobileNetworkManager.retrieveScoresFailed(Error);
		}
	}

	public void _OnScoresLoaded(List<MobileNetworkLeaderboardScore> Scores)
	{
		TBFUtils.DebugLog("Scores loaded");
		if (MobileNetworkManager.scoresLoaded != null)
		{
			MobileNetworkManager.scoresLoaded(Scores);
		}
	}

	public void _OnAchievementsLoaded(List<MobileNetworkAchievement> Achievements)
	{
		TBFUtils.DebugLog("Achievements Loaded");
		if (MobileNetworkManager.achievementsLoaded != null)
		{
			MobileNetworkManager.achievementsLoaded(Achievements);
		}
	}

	public void _OnAchievementLoadFailed(string error)
	{
		TBFUtils.DebugLog("Achievement Load Failed");
		if (MobileNetworkManager.loadAchievementsFailed != null)
		{
			MobileNetworkManager.loadAchievementsFailed(error);
		}
	}

	public bool PostToFacebook(string Caption, string Link, string LinkName, string Description, string ImageLink)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ShowAlertDialog("S_NET_NONETWORK_TITLE", "S_FACEBOOK_NONETWORK_BODY", "S_OK");
			return false;
		}
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		if (Application.platform == RuntimePlatform.Android && !FacebookAndroid.isPluginAvailable())
		{
			ShowAlertDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID", "S_OK");
			return false;
		}
		Resources.UnloadUnusedAssets();
		m_pendingFbCaption = Caption;
		m_pendingFbLink = Link;
		m_pendingFbLinkName = LinkName;
		m_pendingFbDesc = Description;
		m_pendingFbImage = ImageLink;
		if (FacebookAndroid.isSessionValid())
		{
			m_pendingFbOp = FacebookOp.FB_NONE;
			m_currentFbOp = FacebookOp.FB_NONE;
			DoFbPost();
		}
		else
		{
			if (m_currentFbOp != FacebookOp.FB_NONE)
			{
				TBFUtils.DebugLog(m_currentFbOp.ToString() + " still running. Terminating and logging out");
				StopCoroutine("PerformFacebookOps");
				FacebookManager.loginSucceededEvent -= OnFacebookLogin;
				FacebookManager.loginFailedEvent -= OnFacebookLoginFailed;
				FacebookAndroid.logout();
				m_currentFbOp = FacebookOp.FB_NONE;
			}
			m_pendingFbOp = FacebookOp.FB_POST_MESSAGE;
			StartFacebookLogin();
		}
		return true;
	}

	private void StartFacebookLogin()
	{
		if (Application.platform == RuntimePlatform.Android && !FacebookAndroid.isPluginAvailable())
		{
			ShowAlertDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID", "S_OK");
			m_currentFbOp = FacebookOp.FB_NONE;
			m_pendingFbOp = FacebookOp.FB_NONE;
			return;
		}
		m_currentFbOp = FacebookOp.FB_LOGIN;
		StartCoroutine("PerformFacebookOps");
	}

	private void NextFbOp()
	{
		m_currentFbOp = m_pendingFbOp;
		m_pendingFbOp = FacebookOp.FB_NONE;
	}

	private void OnFacebookLogin()
	{
		TBFUtils.DebugLog("Logged into Facebook");
		NextFbOp();
	}

	private void OnFacebookLoginFailed(string Error)
	{
		TBFUtils.DebugLog(string.Format("Facebook login failed: {0}", Error));
		NextFbOp();
	}

	private IEnumerator PerformFacebookOps()
	{
		while (m_currentFbOp != FacebookOp.FB_NONE)
		{
			switch (m_currentFbOp)
			{
			case FacebookOp.FB_LOGIN:
			{
				TBFUtils.DebugLog("Requesting Facebook login");
				FacebookManager.loginSucceededEvent += OnFacebookLogin;
				FacebookManager.loginFailedEvent += OnFacebookLoginFailed;
				string[] permissions = new string[2] { "publish_actions", "user_games_activity" };
				FacebookAndroid.loginWithRequestedPermissions(permissions);
				yield return null;
				float startWait = Time.realtimeSinceStartup;
				float loginTimeOut = 120f;
				bool abort = false;
				while (m_currentFbOp == FacebookOp.FB_LOGIN && !(Time.realtimeSinceStartup > startWait + loginTimeOut))
				{
					yield return null;
				}
				FacebookManager.loginSucceededEvent -= OnFacebookLogin;
				FacebookManager.loginFailedEvent -= OnFacebookLoginFailed;
				if (!FacebookAndroid.isSessionValid() || abort)
				{
					TBFUtils.DebugLog("Facebook login failed");
					FacebookAndroid.logout();
					m_currentFbOp = FacebookOp.FB_NONE;
				}
				break;
			}
			case FacebookOp.FB_POST_MESSAGE:
				DoFbPost();
				NextFbOp();
				break;
			}
			yield return null;
		}
	}

	private void DoFbPost()
	{
		TBFUtils.DebugLog("Posting to facebook");
		if (m_pendingFbCaption != string.Empty)
		{
			m_pendingFbOp = FacebookOp.FB_NONE;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("link", m_pendingFbLink);
			dictionary.Add("picture", m_pendingFbImage);
			dictionary.Add("name", m_pendingFbLinkName);
			dictionary.Add("caption", m_pendingFbCaption);
			dictionary.Add("description", m_pendingFbDesc);
			TBFUtils.DebugLog("Before FacebookBinding.showDialog");
			FacebookAndroid.showDialog("stream.publish", dictionary);
			TBFUtils.DebugLog("After FacebookBinding.showDialog");
			SwrveEventsSocial.FacebookBoast();
		}
		else
		{
			m_pendingFbOp = FacebookOp.FB_NONE;
		}
	}

	private void ShowAlertDialog(string TitleKey, string BodyKey, string ButtonKey)
	{
		string strTitle = Language.Get(TitleKey);
		string strBody = Language.Get(BodyKey);
		string strOK = Language.Get(ButtonKey);
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}

	public bool PostToTwitter(string Caption)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ShowAlertDialog("S_NET_NONETWORK_TITLE", "S_TWITTER_NONETWORK_BODY", "S_OK");
			return false;
		}
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		if (Application.platform == RuntimePlatform.Android && !TwitterAndroid.isPluginAvailable())
		{
			ShowAlertDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID", "S_OK");
			return false;
		}
		Resources.UnloadUnusedAssets();
		m_pendingTwitterCaption = Caption;
		if (TwitterAndroid.isLoggedIn())
		{
			m_currentTwOp = TwitterOp.TW_NONE;
			m_pendingTwOp = TwitterOp.TW_NONE;
			DoTwitterPost();
		}
		else
		{
			m_pendingTwOp = TwitterOp.TW_POST_MESSAGE;
			StartTwitterLogin();
		}
		return true;
	}

	private void StartTwitterLogin()
	{
		if (Application.platform == RuntimePlatform.Android && !TwitterAndroid.isPluginAvailable())
		{
			ShowAlertDialog("S_NET_UNAVAILABLE_TITLE_DROID", "S_NET_UNAVAILABLE_BODY_DROID", "S_OK");
			m_currentTwOp = TwitterOp.TW_NONE;
			m_pendingTwOp = TwitterOp.TW_NONE;
			return;
		}
		TBFUtils.DebugLog("Start Twitter login");
		m_currentTwOp = TwitterOp.TW_LOGIN;
		StartCoroutine("PerformTwitterOps");
	}

	private void NextTwitterOp()
	{
		m_currentTwOp = m_pendingTwOp;
		m_pendingTwOp = TwitterOp.TW_NONE;
	}

	private void OnTwitterLogin()
	{
		TBFUtils.DebugLog("Logged into Twitter");
		NextTwitterOp();
	}

	private void OnTwitterLoginFailed(string Error)
	{
		TBFUtils.DebugLog(string.Format("Twitter login failed: {0}", Error));
		NextTwitterOp();
	}

	private IEnumerator PerformTwitterOps()
	{
		while (m_currentTwOp != TwitterOp.TW_NONE)
		{
			switch (m_currentTwOp)
			{
			case TwitterOp.TW_LOGIN:
			{
				TwitterAndroid.showLoginDialog();
				yield return null;
				float startWait = Time.realtimeSinceStartup;
				float loginTimeOut = 120f;
				bool abort = false;
				while (m_currentTwOp == TwitterOp.TW_LOGIN && !(Time.realtimeSinceStartup > startWait + loginTimeOut))
				{
					yield return null;
				}
				if (!TwitterAndroid.isLoggedIn() || abort)
				{
					TBFUtils.DebugLog("Twitter login failed");
					TwitterAndroid.logout();
					TwitterAndroid.init(m_twitterConsumerKey, m_twitterConsumerSecret);
					m_currentTwOp = TwitterOp.TW_NONE;
				}
				break;
			}
			case TwitterOp.TW_POST_MESSAGE:
				DoTwitterPost();
				NextTwitterOp();
				break;
			}
		}
	}

	private void DoTwitterPost()
	{
		TBFUtils.DebugLog("Posting to Twitter");
		if (m_pendingTwitterCaption != string.Empty)
		{
			TwitterAndroid.postUpdate(m_pendingTwitterCaption);
			SwrveEventsSocial.TwitterBoast();
		}
	}
}
