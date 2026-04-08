using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsDisplay : StateController
{
	public StatsDisplay m_statsDisplay;

	public UIButton m_macawButton;

	public UIButton m_storeButton;

	public ConfirmDialog m_confirmDialogPrefab;

	public BigMessage m_levelUpDialogPrefab;

	public ConfirmDialog m_TutorialEndDialogPrefab;

	public UpdateXPBar m_XPBar;

	public UpdateXPBar m_XPBarTrials;

	public GameObject m_twitterButton;

	public GameObject m_facebookButton;

	public GameObject m_WorkingNode;

	public List<GameObject> m_ButtonsToDelete = new List<GameObject>();

	private bool m_inputLockout;

	private bool m_macawWarnDone;

	private bool m_checkpointWarnDone;

	public GameObject m_aNewHighscore;

	public GameObject m_aNewLongRun;

	public SpriteText m_TitleText;

	public GameObject m_aTrialNameRoot;

	public SpriteText m_aTrialNameText;

	public TrialResultControls m_aTrialsControls;

	public GameObject m_aNormalResultsNode;

	public GameObject m_aTrialResultsNode;

	public GameObject m_aLeaderboardRoot;

	public SlidingButtonBar m_aBar;

	private float m_fTonicRefillDelay;

	private bool m_bTrialAwardsActive;

	public GameObject XPScaler;

	public GameObject XP_X2;

	public GameObject XP_X3;

	public GameObject XP_X4;

	private static ResultsDisplay m_instance;

	public static ResultsDisplay Instance
	{
		get
		{
			return m_instance;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (m_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		if (m_twitterButton != null)
		{
			Object.Destroy(m_twitterButton);
			m_twitterButton = null;
		}
		if (TBFUtils.Is256mbDevice() && m_facebookButton != null)
		{
			Object.Destroy(m_facebookButton);
			m_facebookButton = null;
		}
		for (int i = 0; i < m_ButtonsToDelete.Count; i++)
		{
			GameObject obj = m_ButtonsToDelete[i];
			Object.Destroy(obj);
			obj = null;
		}
		m_ButtonsToDelete.Clear();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	protected override void OnStateLoaded()
	{
		HideState();
	}

	private void OnEnable()
	{
	}

	protected override IEnumerator AnimateStateIn()
	{
		if ((bool)m_aNewLongRun)
		{
			if (PlayerController.Instance().Score().DistanceTravelled() > (float)SecureStorage.Instance.FurthestDistanceTravelled)
			{
				m_aNewLongRun.transform.localPosition = new Vector3(0f, 0f, -3f);
			}
			else
			{
				m_aNewLongRun.transform.localPosition = new Vector3(99999f, 0f, 0f);
			}
		}
		if ((bool)m_aNewHighscore)
		{
			if (PlayerController.Instance().Score().TotalScoreXPAdjusted() >= SecureStorage.Instance.HighestScore)
			{
				m_aNewHighscore.transform.localPosition = new Vector3(0f, 0f, -3f);
			}
			else
			{
				m_aNewHighscore.transform.localPosition = new Vector3(99999f, 0f, 0f);
			}
		}
		bool MacawAvailable = IsMacawAvailable();
		m_macawButton.gameObject.SetActiveRecursively(MacawAvailable);
		base.gameObject.BroadcastMessage("BringOnScreen", SendMessageOptions.DontRequireReceiver);
		MenuSFX.Instance.Play2D("MenuBoxSwoosh");
		yield return null;
		ShowState();
		StartCoroutine("DisplayGameOverDialogs");
		if (BaseCampController.Instance.TutorialActive())
		{
			GameObject macawButton = GameObject.Find("MacawButton");
			StartCoroutine(BaseCampController.Instance.SpawnTutorialDialog(m_TutorialEndDialogPrefab, "S_CHECK_TUT_5_TITLE", "S_CHECK_TUT_5_BODY", macawButton));
		}
		m_inputLockout = false;
		m_macawWarnDone = false;
		m_checkpointWarnDone = false;
		m_TitleText.Text = Language.Get("S_RESULTS");
		m_aLeaderboardRoot.SetActiveRecursively(true);
		if (m_WorkingNode != null)
		{
			m_WorkingNode.SetActiveRecursively(false);
		}
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			m_aTrialNameRoot.SetActiveRecursively(true);
			if (GameController.Instance.Player.IsDead())
			{
				m_WorkingNode.SetActiveRecursively(false);
				m_TitleText.Text = Language.Get("S_GAMEOVER");
				m_aLeaderboardRoot.SetActiveRecursively(false);
				TrialsDataManager.Instance.Save();
			}
			m_aTrialResultsNode.SetActiveRecursively(true);
			m_aNormalResultsNode.SetActiveRecursively(false);
			m_aTrialsControls.m_TrialsTimeText.Text = TimeUtils.FloatToMMSShhString(GameController.Instance.TimerValue);
			TrialsDataManager.TrialState aTrialState = TrialsDataManager.Instance.FindTrialState(GameController.Instance.m_CurrentTrialName);
			m_aTrialNameText.Text = Language.Get(aTrialState.m_srcData.m_title);
			m_aTrialsControls.m_aNewTrialRank.SetActiveRecursively(aTrialState.BestMedalType > GameController.Instance.m_TrialDataAtStart.BestMedalType);
			m_aTrialsControls.m_aNewTrialTime.SetActiveRecursively(aTrialState.m_completed && aTrialState.m_timeInSecs < GameController.Instance.m_TrialDataAtStart.m_timeInSecs);
			foreach (TrialResultsIconBlock aIconBlock in m_aTrialsControls.m_Icons)
			{
				if (aTrialState.m_srcData.m_identifier.Contains(aIconBlock.m_Difficulty))
				{
					if (aTrialState.m_completed)
					{
						aIconBlock.m_NoMedalIcon.SetActiveRecursively(false);
						aIconBlock.m_BronzeIcon.SetActiveRecursively(false);
						aIconBlock.m_SilverIcon.SetActiveRecursively(false);
						aIconBlock.m_GoldIcon.SetActiveRecursively(false);
						float fTimer = GameController.Instance.TimerValue;
						if (GameController.Instance.Player.IsDead())
						{
							fTimer = aTrialState.m_timeInSecs;
						}
						switch (aTrialState.MedalForTime(fTimer))
						{
						case TrialsDataManager.TrialState.eMedalType.eMEDAL_GOLD:
							aIconBlock.m_GoldIcon.SetActiveRecursively(true);
							break;
						case TrialsDataManager.TrialState.eMedalType.eMEDAL_SILVER:
							aIconBlock.m_SilverIcon.SetActiveRecursively(true);
							break;
						case TrialsDataManager.TrialState.eMedalType.eMEDAL_BRONZE:
							aIconBlock.m_BronzeIcon.SetActiveRecursively(true);
							break;
						default:
							aIconBlock.m_NoMedalIcon.SetActiveRecursively(true);
							break;
						}
					}
					else
					{
						aIconBlock.m_NoMedalIcon.SetActiveRecursively(true);
						aIconBlock.m_BronzeIcon.SetActiveRecursively(false);
						aIconBlock.m_SilverIcon.SetActiveRecursively(false);
						aIconBlock.m_GoldIcon.SetActiveRecursively(false);
					}
				}
				else
				{
					aIconBlock.m_NoMedalIcon.SetActiveRecursively(false);
					aIconBlock.m_BronzeIcon.SetActiveRecursively(false);
					aIconBlock.m_SilverIcon.SetActiveRecursively(false);
					aIconBlock.m_GoldIcon.SetActiveRecursively(false);
				}
			}
			StartCoroutine(DoTrialAwardSequence());
		}
		else
		{
			m_WorkingNode.SetActiveRecursively(false);
			m_aTrialNameRoot.SetActiveRecursively(false);
			m_aTrialResultsNode.SetActiveRecursively(false);
			m_aNormalResultsNode.SetActiveRecursively(true);
		}
		m_aLeaderboardRoot.SetActiveRecursively(false);
		PlayerPrefs.Save();
		StartCoroutine("AutopopBar");
	}

	private IEnumerator AutopopBar()
	{
		while (!ResultsFinished())
		{
			yield return new WaitForEndOfFrame();
		}
		m_aBar.ForceExpansion(true);
	}

	private IEnumerator DoTrialAwardSequence()
	{
		m_bTrialAwardsActive = true;
		TrialsDataManager.TrialState aTrialState = TrialsDataManager.Instance.FindTrialState(GameController.Instance.m_CurrentTrialName);
		while (!m_XPBarTrials.Finished())
		{
			yield return new WaitForEndOfFrame();
		}
		if (!GameController.Instance.m_TrialDataAtStart.m_completed && aTrialState.m_completed)
		{
			SecureStorage.Instance.ChangeGems(aTrialState.m_srcData.m_rewardGems);
			string Title = Language.Get("S_CHECKPOINT_UNLOCK_TITLE");
			string Body = "x " + aTrialState.m_srcData.m_rewardGems;
			BigMessage levelUpMessage = (BigMessage)Object.Instantiate(m_levelUpDialogPrefab);
			yield return StartCoroutine(levelUpMessage.Display(Title, Body, 3f));
			while (levelUpMessage != null)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(0.3f);
			GameController.Instance.m_TrialDataAtStart.m_completed = true;
		}
		if (aTrialState.BestMedalType > GameController.Instance.m_TrialDataAtStart.BestMedalType)
		{
			GameObject aObjectToBounce = null;
			foreach (TrialResultsIconBlock aBlock in m_aTrialsControls.m_Icons)
			{
				if (aTrialState.m_srcData.m_identifier.Contains(aBlock.m_Difficulty))
				{
					switch (aTrialState.MedalForTime(GameController.Instance.TimerValue))
					{
					case TrialsDataManager.TrialState.eMedalType.eMEDAL_GOLD:
						aObjectToBounce = aBlock.m_GoldIcon.gameObject;
						break;
					case TrialsDataManager.TrialState.eMedalType.eMEDAL_SILVER:
						aObjectToBounce = aBlock.m_SilverIcon.gameObject;
						break;
					case TrialsDataManager.TrialState.eMedalType.eMEDAL_BRONZE:
						aObjectToBounce = aBlock.m_BronzeIcon.gameObject;
						break;
					default:
						aObjectToBounce = aBlock.m_NoMedalIcon.gameObject;
						break;
					}
				}
			}
			aObjectToBounce.PunchScale(new Vector3(0.75f, 0.75f, 0f), 0.5f, 0f);
			MenuSFX.Instance.Play2D("TrialImprovement");
			yield return new WaitForSeconds(0.3f);
		}
		m_bTrialAwardsActive = false;
	}

	private IEnumerator DisplayGameOverDialogs()
	{
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			while (!m_XPBarTrials.Finished())
			{
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			while (!m_XPBar.Finished())
			{
				yield return new WaitForEndOfFrame();
			}
		}
		base.gameObject.BroadcastMessage("ResultsFinished", SendMessageOptions.DontRequireReceiver);
		if (DailyDoubleController.Instance.BottomTicker.gameObject.active)
		{
			DailyDoubleController.Instance.BottomTicker.gameObject.SetActiveRecursively(false);
		}
		yield return new WaitForSeconds(0.5f);
		if (!m_inputLockout)
		{
			DialogManager.Instance.LaunchGameEndDialogs();
		}
		MusicManager.Instance.PlayTitleMusic();
	}

	public void OnRestartPressed()
	{
		if (m_inputLockout)
		{
			return;
		}
		m_inputLockout = true;
		SwrveEventsUI.ResultsRetryTouched();
		if (!GameController.Instance.IsPlayingTrialsMode)
		{
			if (!DialogManager.Instance.AskLoseMacawQuestion(RestartConfirmed, RestartCancelled, !m_macawWarnDone))
			{
				RestartConfirmed();
			}
		}
		else
		{
			RestartConfirmed();
		}
	}

	private void MacawConfirmed()
	{
		m_inputLockout = false;
		OnMacawPressed();
	}

	private void RestartCancelled()
	{
		m_inputLockout = false;
		m_macawWarnDone = true;
	}

	private void RestartConfirmed()
	{
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		StartCoroutine("DoRestart");
	}

	private IEnumerator DoRestart()
	{
		yield return new WaitForSeconds(0.5f);
		GameController GC = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (GC != null)
		{
			GC.StartNewGame(0f, 0);
		}
		StateManager.Instance.LoadAndActivateState("Game");
	}

	private void OnQuitPressed()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
			if (gameController != null)
			{
				gameController.ReturnToTitleMenus();
			}
		}
	}

	public void OnFieldGuidePressed()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			MenuSFX.Instance.Play2D("MenuConfirm");
			SwrveEventsUI.FieldGuideTouched();
			FieldGuideController.Instance.RePopulate();
			StateManager.Instance.LoadAndActivateState("FieldGuide");
		}
	}

	public void OnChallengesPressed()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			MenuSFX.Instance.Play2D("MenuConfirm");
			StateManager.Instance.LoadAndActivateState("Challenges");
		}
	}

	public void LateUpdate()
	{
		m_fTonicRefillDelay = Mathf.Max(0f, m_fTonicRefillDelay - Time.deltaTime);
	}

	public void OnRefillTonicsPressed()
	{
		if (!m_inputLockout && m_fTonicRefillDelay == 0f)
		{
			m_fTonicRefillDelay = 1.5f;
			ChallengesController.Instance.OnRefillTonics();
		}
	}

	private void ShowNoNetworkAlert()
	{
		string strTitle = Language.Get("S_IAP_NO_NET_TITLE");
		string strBody = Language.Get("S_GENERAL_NO_NET_BODY");
		string strOK = Language.Get("S_OK");
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}

	public void OnGiftThisApp()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		SwrveEventsUI.GiftButtonTouched();
	}

	private bool IsMacawAvailable()
	{
		if ((bool)CheckPointController.Instance() && CheckPointController.Instance().HasPlayerPassedValidCheckpoint())
		{
			return true;
		}
		return false;
	}

	private void LaunchMacaw()
	{
		BaseCampController.Instance.EndTutorial();
		SecureStorage.Instance.ChangeItemCount("consumable.macaw", -1);
		GameController.Instance.StartNewGameFromCheckpoint();
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
	}

	public void OnMacawPressed()
	{
		if (m_inputLockout)
		{
			return;
		}
		m_inputLockout = true;
		MenuSFX.Instance.Play2D("MenuConfirm");
		if (!DialogManager.Instance.AskBuyNewCheckpointQuestion(!m_checkpointWarnDone))
		{
			if (IsMacawAvailable())
			{
				if (SecureStorage.Instance.HasMacaws)
				{
					SwrveEventsUI.ResultsMacawTouched();
					LaunchMacaw();
				}
				else
				{
					StartCoroutine(NeedToBuyMacawDialog());
				}
			}
			else
			{
				StartCoroutine(MacawIntroDialog());
			}
		}
		else
		{
			m_checkpointWarnDone = true;
			m_inputLockout = false;
		}
	}

	private IEnumerator NeedToBuyMacawDialog()
	{
		string Title = Language.Get("S_BUY_MACAW_TITLE");
		string Body = Language.Get("S_BUY_MACAW_BODY");
		string Opt1 = Language.Get("S_OK");
		string Opt2 = Language.Get("S_OK");
		ConfirmDialog confirmDialog = (ConfirmDialog)Object.Instantiate(m_confirmDialogPrefab);
		yield return StartCoroutine(confirmDialog.Display(Title, Body, Opt1, Opt2, OnOK, null, OnOK));
	}

	private IEnumerator MacawIntroDialog()
	{
		string Title = Language.Get("S_WHAT_IS_MACAW_TITLE");
		string Body = Language.Get("S_WHAT_IS_MACAW_BODY");
		string Opt1 = Language.Get("S_OK");
		string Opt2 = Language.Get("S_OK");
		ConfirmDialog confirmDialog = (ConfirmDialog)Object.Instantiate(m_confirmDialogPrefab);
		yield return StartCoroutine(confirmDialog.Display(Title, Body, Opt1, Opt2, OnOK, null, OnOK));
	}

	public void OnLevelUp()
	{
		int num = PlayerController.Instance().Score().NumGemsForLevelUp();
		SecureStorage.Instance.ChangeGems(num);
		StartCoroutine(LevelUpDialog(num));
	}

	private IEnumerator LevelUpDialog(int numGems)
	{
		string Title = Language.Get("S_LEVEL_UP");
		string Body = "x " + numGems;
		BigMessage levelUpMessage = (BigMessage)Object.Instantiate(m_levelUpDialogPrefab);
		yield return StartCoroutine(levelUpMessage.Display(Title, Body, 3f));
		m_statsDisplay.BroadcastMessage("OnLevelUpDialogDismissed");
	}

	private void OnOK()
	{
		m_inputLockout = false;
	}

	public void OnFacebookPressed()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		SwrveEventsUI.ResultsFacebookTouched();
		string text = " ";
		PlayerScore playerScore = PlayerController.Instance().Score();
		if (!GameController.Instance.IsPlayingTrialsMode)
		{
			string format = Language.Get("S_BOAST_FACEBOOK");
			text = string.Format(format, (int)playerScore.DistanceTravelled(), playerScore.TotalScoreXPAdjusted());
		}
		else
		{
			string format2 = Language.Get("S_BOAST_TRIALS_FACEBOOK");
			text = string.Format(format2, Language.Get(GameController.Instance.m_TrialDataAtStart.m_srcData.m_title), TimeUtils.FloatToMMSShhString(GameController.Instance.TimerValue));
		}
		string appStoreURI = SwrveServerVariables.Instance.AppStoreURI;
		string linkName = Language.Get("S_FACEBOOK_POST_NAME");
		string description = Language.Get("S_FACEBOOK_POST_DESC");
		string imageURI = SwrveServerVariables.Instance.ImageURI;
		MobileNetworkManager.Instance.PostToFacebook(text, appStoreURI, linkName, description, imageURI);
	}

	public void OnTwitterPressed()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		SwrveEventsUI.ResultsTwitterTouched();
		PlayerScore playerScore = PlayerController.Instance().Score();
		string text = " ";
		text = (GameController.Instance.IsPlayingTrialsMode ? string.Format(Language.Get("S_BOAST_TRIALS_TWEET"), Language.Get(GameController.Instance.m_TrialDataAtStart.m_srcData.m_title), TimeUtils.FloatToMMSShhString(GameController.Instance.TimerValue), SwrveServerVariables.Instance.AppShortURI) : string.Format(Language.Get("S_BOAST_TWEET"), (int)playerScore.DistanceTravelled(), playerScore.TotalScoreXPAdjusted(), SwrveServerVariables.Instance.AppShortURI));
		MobileNetworkManager.Instance.PostToTwitter(text);
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

	public void OnStorePressed()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			MenuSFX.Instance.Play2D("MenuConfirm");
			base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
			StartCoroutine("DoStore");
		}
	}

	private IEnumerator DoStore()
	{
		float pauseEndTime = Time.realtimeSinceStartup + 0.5f;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		StateManager.Instance.LoadAndActivateState("BaseCamp");
	}

	public bool ResultsFinished()
	{
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			return m_XPBarTrials.Finished() && !m_bTrialAwardsActive;
		}
		return m_XPBar.Finished();
	}
}
