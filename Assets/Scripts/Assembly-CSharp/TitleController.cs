using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class TitleController : StateController
{
	public UIButton m_startButton;

	public UIButton m_storeButton;

	public GameObject m_leaderboardButton;

	public UIButton m_resetButton;

	public GameObject m_facebookButton;

	public GameObject m_twitterButton;

	public GameObject m_challengesButton;

	public GameObject m_GiftButton;

	public GameObject m_MoreAppsButton;

	public GameObject m_HomeButton;

	public OnSaleDialog m_saleDialog;

	[HideInInspector]
	public BuildInfoDisplay m_buildInfo;

	private bool m_initialLaunch;

	private bool m_SelectionMade;

	[HideInInspector]
	public static float m_pendingCheckpointRestart;

	[HideInInspector]
	public static bool m_pendingTutorialLaunch;

	private bool CanPressButton
	{
		get
		{
			return !m_SelectionMade && !DialogManager.DialogActive;
		}
	}

	public override void Awake()
	{
		base.Awake();
		m_initialLaunch = true;
		m_SelectionMade = false;
		if (m_buildInfo != null)
		{
			UnityEngine.Object.Destroy(m_buildInfo.gameObject);
			m_buildInfo = null;
		}
		if (m_resetButton != null)
		{
			UnityEngine.Object.Destroy(m_resetButton.gameObject);
			m_resetButton = null;
		}
		GameObject gameObject = new GameObject("AndroidETCManager");
		gameObject.AddComponent<EtceteraAndroidManager>();
		if (m_HomeButton != null)
		{
			UnityEngine.Object.Destroy(m_HomeButton);
			m_HomeButton = null;
		}
		if (m_twitterButton != null)
		{
			UnityEngine.Object.Destroy(m_twitterButton);
			m_twitterButton = null;
		}
		if (m_facebookButton != null)
		{
			UnityEngine.Object.Destroy(m_facebookButton);
			m_facebookButton = null;
		}
		if (TBFUtils.Is256mbDevice())
		{
			UnityEngine.Object.Destroy(m_leaderboardButton);
			m_leaderboardButton = null;
		}
		if (m_leaderboardButton != null)
		{
			UnityEngine.Object.Destroy(m_leaderboardButton);
			m_leaderboardButton = null;
		}
		if (m_GiftButton != null)
		{
			UnityEngine.Object.Destroy(m_GiftButton);
			m_GiftButton = null;
		}
	}

	private IEnumerator Start()
	{
		UIMenuBackground.Instance.Hide();
		while (GameController.Instance == null)
		{
			yield return null;
		}
		while (GameController.Instance.IsLoading)
		{
			yield return null;
		}
	}

	protected override void OnStateActivate(string OldStateName)
	{
		m_SelectionMade = false;
		base.OnStateActivate(OldStateName);
		if (m_MoreAppsButton != null && (string.IsNullOrEmpty(SwrveServerVariables.Instance.MoreAppsURI) || SwrveServerVariables.Instance.MoreAppsURI == "null"))
		{
			UnityEngine.Object.Destroy(m_MoreAppsButton);
			m_MoreAppsButton = null;
		}
	}

	protected override IEnumerator AnimateStateIn()
	{
		DialogManager.ResetCounter();
		if (TBFUtils.Is256mbDevice())
		{
			GC.Collect();
		}
		HideState();
		base.gameObject.BroadcastMessage("SetOffScreen", SendMessageOptions.DontRequireReceiver);
		while (GameController.Instance == null)
		{
			yield return null;
		}
		GameController.Instance.HideHUD();
		bool initialLaunch = m_initialLaunch;
		if (m_initialLaunch)
		{
			LoadingScreen.Instance.Hide();
			if (!UIMenuBackground.Instance.IsPostSplashPlaying)
			{
				UIMenuBackground.Instance.SwitchCameraFocus("Title");
				UIMenuBackground.Instance.Show();
				UIMenuBackground.Instance.PlayPostSplashAnim();
			}
			float waitTime = UIMenuBackground.Instance.PostSplashAnimLength();
			yield return new WaitForSeconds(waitTime);
			m_initialLaunch = false;
			if (!MobileNetworkManager.Instance.IsLoggedIn)
			{
				MobileNetworkManager.Instance.Login();
			}
			m_SelectionMade = false;
		}
		base.gameObject.BroadcastMessage("BringOnScreen", SendMessageOptions.DontRequireReceiver);
		ShowState();
		MusicManager.Instance.PlayTitleMusic();
		if (!RecoveredCompatibility.DisableLegacyStartupPromos && m_saleDialog != null)
		{
			m_saleDialog.ShowItemsOnSale();
		}
		Bedrock.AnalyticsLogEvent("UI.Lobby.View");
		if (!RecoveredCompatibility.DisableLegacyStartupPromos)
		{
			DialogManager.Instance.LaunchTitleDialogs();
		}
		if (m_pendingCheckpointRestart > 0f)
		{
			OnLaunchGameFromCheckpoint(m_pendingCheckpointRestart);
			base.gameObject.BroadcastMessage("SetOffScreen", SendMessageOptions.DontRequireReceiver);
		}
		else if (m_pendingTutorialLaunch)
		{
			OnLaunchGameFromOptions();
			base.gameObject.BroadcastMessage("SetOffScreen", SendMessageOptions.DontRequireReceiver);
		}
		m_pendingCheckpointRestart = 0f;
		m_pendingTutorialLaunch = false;
		if (OutfitOfTheDayManager.Instance.IsOOTD(SecureStorage.Instance.GetCurrentCostumeType()))
		{
			AchievementManager.Instance.SetCompleted("oneshot.pose");
		}
		if (!initialLaunch)
		{
			SwrveServerVariables.Instance.RefreshSelectedVars();
		}
	}

	protected override void ShowState()
	{
		base.ShowState();
		UIManager.instance.blockInput = false;
		if (m_buildInfo != null)
		{
			m_buildInfo.gameObject.SetActiveRecursively(true);
		}
		if (ChartBoostWrapper.Instance != null)
		{
			ChartBoostWrapper.Instance.ShowAd("Main Menu");
		}
	}

	protected override void HideState()
	{
		base.HideState();
		if (m_buildInfo != null)
		{
			m_buildInfo.gameObject.SetActiveRecursively(false);
		}
	}

	private IEnumerator LaunchGame(string AuthoredSection, float checkpointDistance, GameType gameType)
	{
		if (!ThemeManager.Instance.IsReady)
		{
			UnityEngine.Debug.LogWarning("Started the game without all objects loaded");
		}
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		m_saleDialog.Hide();
		yield return new WaitForSeconds(0.2f);
		MenuSFX.Instance.Play2D("MenuBoxSwoosh");
		yield return new WaitForSeconds(0.2f);
		float waitTime = UIMenuBackground.Instance.TitleToGame();
		yield return new WaitForSeconds(waitTime);
		if (TBFUtils.Is256mbDevice())
		{
			Resources.UnloadUnusedAssets();
		}
		GC.Collect();
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.CineCam2, CameraTransitionData.JumpCut);
		GameController.Instance.SetGameType(gameType);
		if (gameType != GameType.GT_TRIALS)
		{
			GameController.Instance.m_CurrentTrialName = null;
		}
		if (AuthoredSection == null)
		{
			GameController.Instance.GameLaunchAuthoredName = "GameLaunch";
		}
		else
		{
			GameController.Instance.GameLaunchAuthoredName = AuthoredSection;
		}
		if (checkpointDistance > 0f)
		{
			SecureStorage.Instance.ChangeItemCount("consumable.macaw", -1);
			GameController.Instance.StartNewGameFromCheckpoint(checkpointDistance);
		}
		else
		{
			GameController.Instance.StartNewGame(0f, 0);
		}
	}

	private void StartGame(float fromCheckpoint)
	{
		if (CanPressButton)
		{
			SwrveUserData.UploadAllAttributes();
			m_SelectionMade = true;
			StartCoroutine(LaunchGame(null, fromCheckpoint, GameType.GT_CLASSIC));
		}
	}

	public void OnStartPressed()
	{
		StartGame(0f);
	}

	public void OnLaunchGameFromOptions()
	{
		StartGame(0f);
	}

	public void OnLaunchTrials(string trialName)
	{
		UnityEngine.Debug.Log("Launch Trials: " + trialName);
		GameController.Instance.m_CurrentTrialName = trialName;
		GameController.Instance.m_TrialDataAtStart = TrialsDataManager.Instance.FindTrialState(trialName).Clone();
		StartCoroutine(LaunchGame(trialName, 0f, GameType.GT_TRIALS));
	}

	public void OnRestartGamePressed()
	{
		StartGame(SecureStorage.Instance.FurthestDistanceTravelled);
	}

	public void OnLaunchGameFromCheckpoint(float checkpointDistance)
	{
		m_pendingCheckpointRestart = 0f;
		StartGame(checkpointDistance);
	}

	public void OnStorePressed()
	{
		if (CanPressButton)
		{
			SwrveEventsUI.StoreButtonTouched();
			StateManager.Instance.LoadAndActivateState("BaseCamp");
			m_SelectionMade = true;
		}
	}

	public void OnFieldGuidePressed()
	{
		if (CanPressButton)
		{
			SwrveEventsUI.FieldGuideTouched();
			FieldGuideController.Instance.RePopulate();
			StateManager.Instance.LoadAndActivateState("FieldGuide");
			m_SelectionMade = true;
		}
	}

	public void OnOptionsPressed()
	{
		if (CanPressButton)
		{
			SwrveEventsUI.OptionsTouched();
			StateManager.Instance.LoadAndActivateState("Options");
			m_SelectionMade = true;
		}
	}

	public void OnAchievementsPressed()
	{
		MobileNetworkManager.Instance.showAchievements();
		SwrveEventsUI.AchievementsButtonTouched();
	}

	public void OnLeaderboardsPressed()
	{
		MobileNetworkManager.Instance.showLeaderboards();
		SwrveEventsUI.LeaderboardsButtonTouched();
	}

	public void OnResetPressed()
	{
		AchievementManager.Instance.ResetAll();
		Bedrock.DeleteAllUserVariables();
	}

	public void OnFacebookPressed()
	{
		SwrveEventsUI.FacebookButtonTouched();
		DialogManager.Instance.TellFacebookFriends();
	}

	public void OnTwitterPressed()
	{
		SwrveEventsUI.TwitterButtonTouched();
		DialogManager.Instance.TellTwitterFollowers();
	}

	public void OnGiftPressed()
	{
		SwrveEventsUI.GiftButtonTouched();
	}

	public void OnMoreAppsPressed()
	{
		SwrveEventsUI.MoreAppsButtonTouched();
		string strURI = string.Format("{0}?{1}={2}&{3}={4}", SwrveServerVariables.Instance.MoreAppsURI, "referrer", "pitfall", "odid", TBFUtils.ODIN);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ShowNoNetworkAlert();
		}
		else if (SwrveServerVariables.Instance != null)
		{
			EtceteraPlatformWrapper.ShowWebPage(strURI);
		}
	}

	public void OnChallengesPressed()
	{
		if (CanPressButton)
		{
			SwrveEventsUI.TrialsButtonTouched();
			StateManager.Instance.LoadAndActivateState("Challenges");
			m_SelectionMade = true;
		}
	}

	private void ShowNoNetworkAlert()
	{
		string strTitle = Language.Get("S_IAP_NO_NET_TITLE");
		string strBody = Language.Get("S_GENERAL_NO_NET_BODY");
		string strOK = Language.Get("S_OK");
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}

	public void OnFreeDiamondsPressed()
	{
		SwrveEventsUI.FreeDiamondsButtonTouched();
		if (CanPressButton)
		{
			m_SelectionMade = true;
			BaseCampController.Instance.LaunchFreeDiamonds();
		}
	}

	private void Update()
	{
		if (StateManager.Instance.CurrentStateName == "Title" && Input.GetKeyDown(KeyCode.Escape))
		{
			EtceteraAndroidManager.alertButtonClickedEvent += ApplicationQuitConfirm;
			EtceteraAndroidManager.promptCancelledEvent += ApplicationQuitCancel;
			string positiveButton = Language.Get("S_YES");
			string negativeButton = Language.Get("S_NO");
			string title = Language.Get("S_QUIT_SURE_TITLE");
			string message = Language.Get("S_QUIT_SURE_BODY");
			EtceteraPlatformWrapper.ShowAlert(title, message, positiveButton, negativeButton);
			PlayerPrefs.Save();
		}
	}

	public static void ApplicationQuitCancel()
	{
		UnityEngine.Debug.Log("Quit Cancelled");
		EtceteraAndroidManager.alertButtonClickedEvent -= ApplicationQuitConfirm;
		EtceteraAndroidManager.promptCancelledEvent -= ApplicationQuitCancel;
	}

	public static void ApplicationQuitConfirm(string text)
	{
		if (text == Language.Get("S_YES"))
		{
		UnityEngine.Debug.Log("Quitting Pitfall : " + text);
			Process.GetCurrentProcess().Kill();
		}
		else
		{
		UnityEngine.Debug.Log("Quit Cancelled : " + text);
			EtceteraAndroidManager.alertButtonClickedEvent -= ApplicationQuitConfirm;
			EtceteraAndroidManager.promptCancelledEvent -= ApplicationQuitCancel;
		}
	}
}
