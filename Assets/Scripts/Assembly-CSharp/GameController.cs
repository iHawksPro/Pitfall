using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public enum GameState
	{
		GAME_LOADING = 0,
		GAME_NOTPLAYING = 1,
		GAME_RUNNING = 2,
		GAME_DIAMONDSTOCONTINUE = 3,
		GAME_OVER = 4,
		GAME_TRIALSINTRO = 5,
		GAME_TRIALSOUTRO = 6
	}

	[Serializable]
	public class GameAuthoredSection
	{
		public string m_name;

		public AuthoredLevelLayoutController m_prefab;
	}

	public class FPSCounter
	{
		private float mUpdateInterval = 0.5f;

		private double mLastInterval;

		private int mFrames;

		private float mFPS;

		public float Value()
		{
			return mFPS;
		}

		public void Start()
		{
			mLastInterval = Time.realtimeSinceStartup;
			mFrames = 0;
		}

		public void Update()
		{
			mFrames++;
			double num = Time.realtimeSinceStartup;
			if (num > mLastInterval + (double)mUpdateInterval)
			{
				mFPS = (float)mFrames / (float)(num - mLastInterval);
				mFrames = 0;
				mLastInterval = num;
			}
		}
	}

	private const int DIAMONDSCONTINUECOUNT = 5;

	private const float DIAMONDSCONTINUEWAIT = 1f;

	private const float DIAMONDSCONTINUEINITIALWAIT = 2f;

	private const string TutorialAuthoredName = "GameTutorial";

	private const float kTrialOutroTime = 1.5f;

	public LevelGenerator Level;

	public PlayerController Player;

	public GameObject PlayerFeet;

	public LegacyGUIText Score;

	public bool IsPaused;

	public GameState CurrentState;

	public HudPanel GameHUD;

	public bool DoMemTest;

	private FPSCounter FPS = new FPSCounter();

	private float mRealDeltaTime;

	private float mLastRealTime;

	public bool DebugKillMe;

	private float mLastCheckpoint;

	private float mShowResultsCountdown;

	private bool mActuallyShowResults;

	private bool mHasQuit;

	private float m_currentGameTime;

	private int m_trialsModeCrossedLineCount;

	private int m_iBoostsUsed;

	public string m_CurrentTrialName;

	public TrialsDataManager.TrialState m_TrialDataAtStart;

	public Shader LowGFXShaderRef;

	private int m_DiamondstoContinueCountdown;

	private float m_DiamondstoContinueWait;

	private float m_DiamondstoContinueInitialWait;

	private bool m_DiamondstoContinueInTutorial;

	private bool m_RestoreHarry;

	[HideInInspector]
	public DiamondsToContinueDialog m_diamondsdialog;

	public bool DiamondsToContinueCountdownIsRunning = true;

	public bool EwhShowState;

	public static Costume m_lastCheckedCostume = Costume.Max_Costumes;

	public int VungleAdsWatchedToday;

	public GameAuthoredSection[] m_authoredGameSections;

	private static GameController mInstance;

	[HideInInspector]
	public string GameLaunchAuthoredName = "GameLaunch";

	private float m_trialOutroCountdown;

	public bool RestoreHarry
	{
		get
		{
			return m_RestoreHarry;
		}
		set
		{
			m_RestoreHarry = value;
		}
	}

	public static GameController Instance
	{
		get
		{
			return mInstance;
		}
	}

	public int NumBoostsUsedLastGame
	{
		get
		{
			return m_iBoostsUsed;
		}
	}

	public int TotalBoostUsedLastGame
	{
		get
		{
			int num = Mathf.Max(0, m_iBoostsUsed - 1) * 100;
			float num2 = PlayerController.Instance().TrialsBoost.TimeRemaining / TrialsDataManager.Instance.BoostTime;
			return num + (100 - (int)(100f * num2));
		}
	}

	public float TimerValue
	{
		get
		{
			return m_currentGameTime;
		}
	}

	public bool IsLoading
	{
		get
		{
			return CurrentState == GameState.GAME_LOADING;
		}
	}

	public bool IsPlayingTrialsMode
	{
		get
		{
			bool result = false;
			if (LevelGenerator.Instance() != null)
			{
				result = LevelGenerator.Instance().CurrentGameType == GameType.GT_TRIALS;
			}
			return result;
		}
	}

	public void UsedBoost()
	{
		m_iBoostsUsed++;
	}

	public static string[] GetPieceSetScenes()
	{
		return new string[5] { "tilesJungle", "tilesMountain", "tilesCave", "tilesMinecart", "tilesBike" };
	}

	public static string[] GetThemeSetPieceScenes()
	{
		return new string[5] { "Jungle01", "MountainSide01", "Caves01", "MineCart01", "MotorBike01" };
	}

	private void Awake()
	{
		mLastRealTime = 0f;
		mRealDeltaTime = 0f;
		mInstance = this;
	}

	private IEnumerator LoadSceneList(string[] scenesToLoad)
	{
		foreach (string sceneName in scenesToLoad)
		{
			yield return Application.LoadLevelAdditiveAsync(sceneName);
			while (true)
			{
				GameObject preview = GameObject.Find("_PREVIEW");
				if (preview == null)
				{
					break;
				}
				UnityEngine.Object.DestroyImmediate(preview);
				TBFUtils.DebugLog(string.Format("Authored scene {0} contains preview, removing.", sceneName));
			}
		}
	}

	private IEnumerator Start()
	{
		CurrentState = GameState.GAME_LOADING;
		mHasQuit = false;
		Time.maximumDeltaTime = 0.05f;
		yield return Application.LoadLevelAdditiveAsync("HUD");
		if (TBFUtils.Is256mbDevice())
		{
			Resources.UnloadUnusedAssets();
			while (!ThemeManager.Instance.IsReady)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		else
		{
			while (!ThemeManager.Instance.HasThemeLoaded(WorldConstructionHelper.Theme.Jungle) || !ThemeManager.Instance.HasThemeLoaded(WorldConstructionHelper.Theme.Mountain))
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		yield return Application.LoadLevelAdditiveAsync("gameLogic");
		CurrentState = GameState.GAME_NOTPLAYING;
		m_DiamondstoContinueCountdown = 0;
		m_DiamondstoContinueWait = 0f;
		m_DiamondstoContinueInitialWait = 0f;
		m_RestoreHarry = false;
		SecureStorage.Instance.LoadVungleAdData();
		MonoBehaviour.print("(ewh) not a debug build");
	}

	private void OnDestroy()
	{
		if (mInstance == this)
		{
			mInstance = null;
		}
	}

	private void Update()
	{
		UpdateRealTimeDelta();
		if (EwhShowState)
		{
			EwhShowState = false;
			Debug.Log("(ewh) currentstate: " + CurrentState);
		}
		switch (CurrentState)
		{
		case GameState.GAME_LOADING:
		case GameState.GAME_NOTPLAYING:
			break;
		case GameState.GAME_RUNNING:
			GameLoop();
			break;
		case GameState.GAME_DIAMONDSTOCONTINUE:
			GameDiamondsToContinue();
			break;
		case GameState.GAME_OVER:
			GameOverLoop();
			break;
		case GameState.GAME_TRIALSINTRO:
			GameTrialsIntro();
			break;
		case GameState.GAME_TRIALSOUTRO:
			GameTrialsOutro();
			break;
		}
	}

	private void UpdateRealTimeDelta()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mRealDeltaTime = realtimeSinceStartup - mLastRealTime;
		mLastRealTime = realtimeSinceStartup;
	}

	private void LateUpdate()
	{
		Shader.SetGlobalVector("_PlayerPosition", PlayerFeet.transform.position);
		Shader.SetGlobalVector("_ShadowOffset", 0.01f * Noise.Smooth(Time.time));
	}

	public void Reset()
	{
		GameHUD.SetTreasure(0);
		GameHUD.SetDistance(0);
		GameHUD.SetDiamonds(SecureStorage.Instance.GetGems());
		Level.gameObject.SetActiveRecursively(true);
		Level.Reset();
		Player.Reset();
		if (IsPlayingTrialsMode)
		{
			m_TrialDataAtStart = TrialsDataManager.Instance.FindTrialState(m_CurrentTrialName).Clone();
		}
		mLastCheckpoint = 0f;
		m_iBoostsUsed = 0;
	}

	public int GetNumSpiritOfTheJungleHeld()
	{
		return Player.SpiritOfTheJungle().NumHeld();
	}

	public bool SpiritOfTheJungleCanBeUsed()
	{
		return !PlayerController.Instance().IsDead() && !IsPaused && !Player.SpiritOfTheJungle().Active() && !Player.SpiritOfTheJungle().CoolDownActive() && !Player.IsInSpeedBoostBonus() && !GameTutorial.Instance.IsEnabled;
	}

	public bool VenomCanBeUsed()
	{
		return !PlayerController.Instance().IsDead() && !IsPaused && Player.IsPoisoned() && GetNumVenomHeld() > 0 && !GameTutorial.Instance.IsEnabled;
	}

	public bool SpeedBoostPowerUpCanBeUsed()
	{
		return !PlayerController.Instance().IsDead() && !IsPaused && !Player.GetSpeedIncreasePowerup().IsActive() && !GameTutorial.Instance.IsEnabled;
	}

	public int GetNumSpeedBoostPowerUpHeld()
	{
		return SecureStorage.Instance.GetItemCount("consumable.speedincrease");
	}

	public int GetNumVenomHeld()
	{
		return SecureStorage.Instance.GetItemCount("consumable.antivenom");
	}

	public void SetCostume()
	{
		PlayerController playerController = PlayerController.Instance();
		if ((bool)playerController)
		{
			playerController.CurrentCostume = SecureStorage.Instance.GetCurrentCostumeType();
		}
	}

	private void ClearTestingAuthoredSections()
	{
		AuthoredLevelLayoutController[] array = UnityEngine.Object.FindObjectsOfType(typeof(AuthoredLevelLayoutController)) as AuthoredLevelLayoutController[];
		AuthoredLevelLayoutController[] array2 = array;
		foreach (AuthoredLevelLayoutController authoredLevelLayoutController in array2)
		{
			UnityEngine.Object.Destroy(authoredLevelLayoutController.gameObject);
		}
	}

	private AuthoredLevelLayoutController FindAuthoredSection(string AuthoredName)
	{
		AuthoredLevelLayoutController result = null;
		for (int i = 0; i < m_authoredGameSections.Length; i++)
		{
			if (m_authoredGameSections[i].m_name == AuthoredName)
			{
				result = (AuthoredLevelLayoutController)UnityEngine.Object.Instantiate(m_authoredGameSections[i].m_prefab);
			}
		}
		return result;
	}

	private void EnableAuthoredSection(string AuthoredName)
	{
		ClearTestingAuthoredSections();
		AuthoredLevelLayoutController authoredLevelLayoutController = FindAuthoredSection(AuthoredName);
		if (authoredLevelLayoutController != null)
		{
			TBFUtils.DebugLog("Setting authored section " + AuthoredName + " to launch");
			authoredLevelLayoutController.MakeReady();
			AuthoredLevelController.Instance().SetGlobalWorld(authoredLevelLayoutController);
		}
	}

	private void DisableAuthoredSection(string AuthoredName)
	{
		AuthoredLevelLayoutController authoredLevelLayoutController = FindAuthoredSection(AuthoredName);
		if (authoredLevelLayoutController != null && !authoredLevelLayoutController.TestWorld)
		{
			AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
			if (authoredLevelController.GetGlobalWorld() == authoredLevelLayoutController)
			{
				authoredLevelController.SetGlobalWorld(null);
			}
		}
	}

	public void EnableTutorial()
	{
		EnableAuthoredSection("GameTutorial");
	}

	public void DisableTutorial()
	{
		DisableAuthoredSection("GameTutorial");
	}

	public void EnableGameLaunch()
	{
		bool flag = false;
		AuthoredLevelLayoutController[] array = UnityEngine.Object.FindObjectsOfType(typeof(AuthoredLevelLayoutController)) as AuthoredLevelLayoutController[];
		AuthoredLevelLayoutController[] array2 = array;
		foreach (AuthoredLevelLayoutController authoredLevelLayoutController in array2)
		{
			if (authoredLevelLayoutController.TestWorld)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			EnableAuthoredSection(GameLaunchAuthoredName);
		}
	}

	public void DisableGameLaunch()
	{
		DisableAuthoredSection(GameLaunchAuthoredName);
	}

	public void SetGameType(GameType gameType)
	{
		SecureStorage.Instance.ResetMacawsInRun();
		LevelGenerator.Instance().CurrentGameType = gameType;
	}

	public void StartNewGame(float distance, int checkpointNumber)
	{
		ScreenTimeout.Instance.AllowTimeout = false;
		GameTutorial.Instance.Reset();
		LevelGeneratorTutorialHelper.instance.Reset(false);
		AuthoredLevelController.Instance().SetGlobalWorld(null);
		m_iBoostsUsed = 0;
		if (distance == 0f)
		{
			SecureStorage.Instance.ResetMacawsInRun();
			if (!SecureStorage.Instance.TutorialViewed && !IsPlayingTrialsMode)
			{
				DisableGameLaunch();
				EnableTutorial();
			}
			else
			{
				DisableTutorial();
				EnableGameLaunch();
			}
		}
		SetCostume();
		Restart(distance, checkpointNumber);
		StateManager.Instance.LoadAndActivateState("Game");
		ShowHUD(2.5f, distance);
		if (distance == 0f)
		{
			PlayerController.Instance().SFX.PlayRunStart();
		}
		SecureStorage.Instance.IncGamesPlayed();
		m_currentGameTime = 0f;
		if (IsPlayingTrialsMode)
		{
			SwrveEventsGameplay.TrialStarted(m_CurrentTrialName);
		}
		else
		{
			SwrveEventsGameplay.GameStarted((int)distance);
		}
	}

	public void StartNewGameFromCheckpoint(float runDistance)
	{
		CheckPointController checkPointController = CheckPointController.Instance();
		checkPointController.Reset();
		float num = checkPointController.FindFurthestAvailableCheckpointForDistance(runDistance);
		int validCheckpointNumberFor = checkPointController.GetValidCheckpointNumberFor(num);
		Level.SetCheckpointRestart(num, validCheckpointNumberFor);
		StartNewGame(num, validCheckpointNumberFor);
		SecureStorage.Instance.IncMacawsInRun();
		Player.Score().IncMacawsUsed();
		Player.CallOfTheMacaw(true);
		Player.OverrideDistance(num);
		checkPointController.SetCurrentMacawCheckPointNum(validCheckpointNumberFor);
		checkPointController.SetLastMacawCheckPointDistance(num);
		mLastCheckpoint = num;
	}

	public void StartNewGameFromCheckpoint()
	{
		float runDistance = SecureStorage.Instance.FurthestDistanceTravelled;
		StartNewGameFromCheckpoint(runDistance);
	}

	public bool UseSpiritOfTheJungle()
	{
		bool result = Player.SpiritOfTheJungle().Use();
		CameraManager instance = CameraManager.Instance;
		if (instance.GetQueuedCamera() != null && instance.IsValidRessurectionCamera(instance.GetQueuedCamera()))
		{
			instance.SetResurrectionCamera(instance.GetQueuedCamera());
		}
		else if (instance.GetNextCamera() != null && instance.IsValidRessurectionCamera(instance.GetNextCamera()))
		{
			instance.SetResurrectionCamera(instance.GetNextCamera());
		}
		else
		{
			instance.SetResurrectionCamera(instance.GetCurrentCamera());
		}
		return result;
	}

	public void SetupResurrectionCamera()
	{
		CameraManager instance = CameraManager.Instance;
		if (instance.GetQueuedCamera() != null && instance.IsValidRessurectionCamera(instance.GetQueuedCamera()))
		{
			instance.SetResurrectionCamera(instance.GetQueuedCamera());
		}
		else if (instance.GetNextCamera() != null && instance.IsValidRessurectionCamera(instance.GetNextCamera()))
		{
			instance.SetResurrectionCamera(instance.GetNextCamera());
		}
		else if (instance.IsValidRessurectionCamera(instance.GetCurrentCamera()))
		{
			instance.SetResurrectionCamera(instance.GetCurrentCamera());
		}
	}

	public bool UseSpeedIncrease()
	{
		return Player.GetSpeedIncreasePowerup().Use();
	}

	public bool UseAntiVenom()
	{
		return Player.UseAntiVenom();
	}

	private void UpdateMacawCheckpoint()
	{
		float lastMacawCheckPointDistance = CheckPointController.Instance().GetLastMacawCheckPointDistance();
		if (mLastCheckpoint < lastMacawCheckPointDistance)
		{
			mLastCheckpoint = lastMacawCheckPointDistance;
		}
	}

	private void UpdateVenom()
	{
		if (GameHUD != null)
		{
			if (VenomCanBeUsed() && !IsPlayingTrialsMode)
			{
				GameHUD.SetVenomIconVisible(true);
			}
			else
			{
				GameHUD.SetVenomIconVisible(false);
			}
		}
	}

	private void UpdateSpiritOfTheJungle()
	{
		float spiritOfTheJungleIconAlpha = 0f;
		if (GetNumSpiritOfTheJungleHeld() > 0)
		{
			spiritOfTheJungleIconAlpha = ((!SpiritOfTheJungleCanBeUsed()) ? 0.25f : 1f);
		}
		if (GameHUD != null)
		{
			GameHUD.SetSpiritOfTheJungleIconAlpha(spiritOfTheJungleIconAlpha);
		}
	}

	private void UpdateSpeedBoostPowerUp()
	{
		float speedBoostPowerUpIconAlpha = 0f;
		if (GetNumSpeedBoostPowerUpHeld() > 0)
		{
			speedBoostPowerUpIconAlpha = ((!SpeedBoostPowerUpCanBeUsed()) ? 0.25f : 1f);
		}
		if (GameHUD != null)
		{
			GameHUD.SetSpeedBoostPowerUpIconAlpha(speedBoostPowerUpIconAlpha);
		}
	}

	public bool Paused()
	{
		return IsPaused;
	}

	public void PauseGame()
	{
		Time.timeScale = 0f;
		IsPaused = true;
		ScreenTimeout.Instance.AllowTimeout = true;
		SoundManager.Instance.Pause();
		MusicManager.Instance.Pause();
	}

	public void UnpauseThings()
	{
		Time.timeScale = 1f;
		IsPaused = false;
		ScreenTimeout.Instance.AllowTimeout = false;
		SoundManager.Instance.UnPause();
		MusicManager.Instance.UnPause();
	}

	public void ResumeGame()
	{
		UnpauseThings();
		StateManager.Instance.LoadAndActivateState("Game");
	}

	public void ShowHUD(float Delay, float Distance)
	{
		StartCoroutine(DelayedShowHUD(Delay, Distance));
	}

	private IEnumerator DelayedShowHUD(float Delay, float Distance)
	{
		float startTime = Time.realtimeSinceStartup;
		yield return null;
		while (Time.realtimeSinceStartup < startTime + Delay)
		{
			yield return null;
		}
		ShowHUD(Distance);
	}

	public void ShowHUD(float Distance)
	{
		if (IsPaused)
		{
			return;
		}
		GameHUD.Show();
		if (Distance >= 0f)
		{
			GameObject gameObject = GameObject.Find("DistanceMapPrefab");
			if (gameObject != null)
			{
				gameObject.BroadcastMessage("RestartRun", Distance, SendMessageOptions.DontRequireReceiver);
			}
		}
		GameHUD.BroadcastMessage("BringOnScreen", SendMessageOptions.DontRequireReceiver);
		if (!IsPlayingTrialsMode)
		{
			Costume currentCostumeType = SecureStorage.Instance.GetCurrentCostumeType();
			if (m_lastCheckedCostume != currentCostumeType)
			{
				OutfitOfTheDayManager.Instance.DisplayOutfitMatchDialog();
				m_lastCheckedCostume = currentCostumeType;
			}
		}
	}

	public void QuitGame()
	{
		mHasQuit = true;
		Player.SFX.KillSoundLoops();
		SoundManager.Instance.StopAll();
		MusicManager.Instance.StopMusic();
	}

	private void Restart(float currentDistance, int checkpointNumber)
	{
		ThemeManager.Instance.OnLevelReset(currentDistance);
		WorldConstructionController.Instance().ResetState(currentDistance, checkpointNumber);
		Reset();
		UnpauseThings();
		PickupController.Instance().Reset();
		BaddieController.Instance().Reset();
		PoisonHUDController.Instance().Reset();
		FriendsMarkerController.Instance().Reset();
		Player.StartGame(currentDistance);
		if (LevelGenerator.Instance().CurrentGameType == GameType.GT_TRIALS)
		{
			StartTrialsGame();
		}
		else
		{
			CurrentState = GameState.GAME_RUNNING;
			MusicManager.Instance.PlayGameMusic();
		}
		InputController.Instance().Reset();
		DebugKillMe = false;
		FPS.Start();
		CameraManager.Instance.ResetCameras();
	}

	private void GameLoop()
	{
		m_currentGameTime += Time.deltaTime;
		UpdateMacawCheckpoint();
		UpdateSpiritOfTheJungle();
		UpdateSpeedBoostPowerUp();
		UpdateVenom();
		if (DebugKillMe)
		{
			Player.Kill();
			DebugKillMe = false;
		}
		if (Player.PlayerGameOver() || mHasQuit)
		{
			if (mHasQuit)
			{
				EndGame();
				mShowResultsCountdown = 0.0001f;
				mActuallyShowResults = false;
				mHasQuit = false;
			}
			else if (SecureStorage.Instance.GetItemCount("checkpoint.2000") == 0 || IsPlayingTrialsMode)
			{
				EndGame();
				MusicManager.Instance.PlayGameOverMusic();
			}
			else
			{
				m_DiamondstoContinueCountdown = 5;
				m_DiamondstoContinueWait = 1f;
				m_DiamondstoContinueInitialWait = 2f;
				CurrentState = GameState.GAME_DIAMONDSTOCONTINUE;
				m_DiamondstoContinueInTutorial = false;
				if (!SecureStorage.Instance.DiamondsTutorialViewed)
				{
					SecureStorage.Instance.ChangeGems(3);
					GameHUD.m_diamondsText.Text = SecureStorage.Instance.GetGems().ToString();
					m_DiamondstoContinueInTutorial = true;
				}
			}
		}
		FPS.Update();
		UpdateHUD();
		if (Input.GetKeyDown(KeyCode.J))
		{
			Player.SetSpeedBoostBonus();
		}
	}

	private void GameDiamondsToContinue()
	{
		if (m_RestoreHarry)
		{
			if (!Player.PlayerGameOver() || mHasQuit)
			{
				m_RestoreHarry = false;
				CurrentState = GameState.GAME_RUNNING;
			}
		}
		else
		{
			if (!DiamondsToContinueCountdownIsRunning)
			{
				return;
			}
			if (m_DiamondstoContinueInitialWait > 0f)
			{
				m_DiamondstoContinueInitialWait -= mRealDeltaTime;
				if (!(m_DiamondstoContinueInitialWait <= 0f))
				{
					return;
				}
				bool flag = false;
				int vungleAdsPerDay = SwrveServerVariables.Instance.VungleAdsPerDay;
				if (vungleAdsPerDay > 0 && VungleAdsWatchedToday < vungleAdsPerDay)
				{
					flag = SecureStorage.Instance.DiamondsTutorialViewed && Application.internetReachability != NetworkReachability.NotReachable && (bool)VungleWrapper.Instance && VungleWrapper.Instance.AdIsAvailable;
				}
				m_diamondsdialog = ((!flag) ? DialogManager.Instance.LaunchDiamondsToContinueDialog(true) : DialogManager.Instance.LaunchDiamondsOrAdvertsToContinueDialog(true));
				m_diamondsdialog.SetContinueNumberVal(m_DiamondstoContinueCountdown);
				int num = PlayerController.Instance().DiamondsToContinueAttempts;
				if (num >= SwrveServerVariables.Instance.DTC_maxAttempts)
				{
					num = SwrveServerVariables.Instance.DTC_maxAttempts - 1;
				}
				m_diamondsdialog.TotalGemsRequiredForContinue = SwrveServerVariables.Instance.DTC_initialNeeded;
				if (num > 0)
				{
					float num2 = m_diamondsdialog.TotalGemsRequiredForContinue;
					for (int i = 0; i < num; i++)
					{
						num2 *= SwrveServerVariables.Instance.DTC_attemptMultiplier;
					}
					num2 += 0.5f;
					m_diamondsdialog.TotalGemsRequiredForContinue = (int)num2;
				}
				Debug.Log("(ewh) Attempt #: " + num + ", Diamonds needed to continue: " + m_diamondsdialog.TotalGemsRequiredForContinue);
				m_diamondsdialog.AdditionalGemsRequiredForContinue = m_diamondsdialog.TotalGemsRequiredForContinue - SecureStorage.Instance.GetGems();
				Debug.Log("(ewh) Additional Diamonds needed to continue: " + m_diamondsdialog.AdditionalGemsRequiredForContinue);
			}
			else
			{
				if (!(m_diamondsdialog != null) || !(m_diamondsdialog.InGameStore == null) || !m_diamondsdialog.CountDownEnabled || m_DiamondstoContinueInTutorial || m_DiamondstoContinueCountdown <= 0)
				{
					return;
				}
				if (m_DiamondstoContinueWait <= 0f)
				{
					m_DiamondstoContinueWait = 1f;
					m_DiamondstoContinueCountdown--;
					m_diamondsdialog.SetContinueNumberVal(m_DiamondstoContinueCountdown);
					if (m_DiamondstoContinueCountdown == 0)
					{
						m_diamondsdialog.StartTheExit(true);
						m_diamondsdialog = null;
						StartCoroutine(DelayEndGame(0.75f));
					}
				}
				else
				{
					m_DiamondstoContinueWait -= mRealDeltaTime;
				}
			}
		}
	}

	public IEnumerator DelayEndGame(float delay)
	{
		yield return new WaitForSeconds(delay);
		EndGame();
		MusicManager.Instance.PlayGameOverMusic();
	}

	public void ContinueFromDiamonds(int gemsneeded)
	{
		m_diamondsdialog = null;
		StartCoroutine(DelayRestoreHarry(1f));
		int diamondsToContinueAttempts = PlayerController.Instance().DiamondsToContinueAttempts;
		diamondsToContinueAttempts++;
		PlayerController.Instance().DiamondsToContinueAttempts = diamondsToContinueAttempts;
		Debug.Log("(ewh) Attempt incremented to: " + PlayerController.Instance().DiamondsToContinueAttempts);
		Debug.Log("(ewh) taking away " + gemsneeded + " gems");
		SecureStorage.Instance.ChangeGems(-gemsneeded);
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash("NumberOfDiamonds", gemsneeded.ToString(), "AttemptNumber", diamondsToContinueAttempts.ToString());
		Bedrock.AnalyticsLogEvent("DiamondsToContinue.UsedDiamonds", parameters, false);
	}

	public void ContinueFromAdvert()
	{
		m_diamondsdialog = null;
		StartCoroutine(DelayRestoreHarry(1f));
		int diamondsToContinueAttempts = PlayerController.Instance().DiamondsToContinueAttempts;
		diamondsToContinueAttempts++;
		PlayerController.Instance().DiamondsToContinueAttempts = diamondsToContinueAttempts;
		Debug.Log("(ewh) Attempt incremented to: " + PlayerController.Instance().DiamondsToContinueAttempts);
	}

	public void CancelFromDiamonds()
	{
		m_diamondsdialog = null;
		m_DiamondstoContinueCountdown = 0;
		m_DiamondstoContinueWait = 0f;
		StartCoroutine(DelayEndGame(0.75f));
	}

	private IEnumerator DelayRestoreHarry(float delay)
	{
		yield return new WaitForSeconds(delay);
		MenuSFX.Instance.Play2D("CheckPointPurchaseAvailable");
		m_RestoreHarry = true;
	}

	private void GameOverLoop()
	{
		if (mShowResultsCountdown > 0f)
		{
			float num = mShowResultsCountdown;
			mShowResultsCountdown -= mRealDeltaTime;
			if (num > 0.5f && mShowResultsCountdown <= 0.5f && GameHUD != null)
			{
				GameHUD.OnGameOver();
				CameraManager.Instance.StopCameraShake();
			}
			if (mShowResultsCountdown <= 0f && mActuallyShowResults)
			{
				ShowResults();
			}
		}
	}

	public void EndGame()
	{
		ScreenTimeout.Instance.AllowTimeout = true;
		SecureStorage.Instance.LastGameTime = m_currentGameTime;
		CurrentState = GameState.GAME_OVER;
		Player.GameEnded();
		mShowResultsCountdown = 2f;
		mActuallyShowResults = true;
		GetComponent<InputDebug>().enabled = false;
		BaddieController.Instance().Reset();
		ThemeManager.Instance.OnGameEnd();
		if (Instance.IsPlayingTrialsMode && Player.IsDead())
		{
			SwrveEventsGameplay.TrialFailed(Instance.m_CurrentTrialName, Instance.TimerValue, Player.Score().DistanceTravelled(), Instance.TotalBoostUsedLastGame);
		}
	}

	private void ShowResults()
	{
		StateManager.Instance.LoadAndActivateState("Results");
	}

	public void ReturnToTitleMenus()
	{
		Time.timeScale = 1f;
		IsPaused = false;
		ScreenTimeout.Instance.AllowTimeout = true;
		Player.SetIdle();
		StateManager.Instance.LoadAndActivateState("Title");
	}

	public void HideHUD()
	{
		if (GameHUD != null)
		{
			GameHUD.Hide();
		}
	}

	private void UpdateHUD()
	{
		if (GameHUD != null)
		{
			GameHUD.SetTreasure(Player.Score().CoinsCollected());
			GameHUD.SetDistance((int)Player.Score().DistanceTravelled());
			GameHUD.SetDiamonds(SecureStorage.Instance.GetGems());
		}
	}

	public void OnCoinCollected()
	{
		if (GameHUD != null)
		{
			GameHUD.OnCoinCollected();
		}
	}

	private void StartTrialsGame()
	{
		MusicManager.Instance.StopMusic();
		DialogManager.Instance.LaunchCountdown(TrialIntroCountdownComplete);
		Player.SetWaitingForTrialStart();
		LevelGenerator.Instance().CentrePlayerPositionOnCurrentPiece(2f);
		LevelGenerator.Instance().Advance(2f);
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.GordonsCam, CameraTransitionData.JumpCut);
		CurrentState = GameState.GAME_TRIALSINTRO;
		m_trialsModeCrossedLineCount = 0;
	}

	private void GameTrialsIntro()
	{
	}

	private void TrialIntroCountdownComplete()
	{
		float trialSpeed = PlayerController.Instance().Settings.InitialSpeed;
		AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
		if (authoredLevelController != null)
		{
			AuthoredLevelLayoutController globalWorld = authoredLevelController.GetGlobalWorld();
			if (globalWorld != null)
			{
				trialSpeed = globalWorld.SpeedMinimum;
			}
		}
		Player.StartTrialsGame(trialSpeed);
		CurrentState = GameState.GAME_RUNNING;
		MusicManager.Instance.PlayGameMusic();
	}

	private void GameTrialsOutro()
	{
		m_trialOutroCountdown -= Time.deltaTime;
		if (m_trialOutroCountdown <= 0f)
		{
			EndGame();
		}
	}

	public void TrialsModeCrossedLine()
	{
		m_trialsModeCrossedLineCount++;
		TBFUtils.DebugLog("TRIALS MODE: CROSSED LINE" + m_trialsModeCrossedLineCount);
		if (m_trialsModeCrossedLineCount >= 2)
		{
			TrialsDataManager.Instance.MarkTrialCompleted(m_CurrentTrialName, TimerValue);
			Player.SetTrialComplete();
			m_trialOutroCountdown = 1.5f;
			CurrentState = GameState.GAME_TRIALSOUTRO;
			HideHUD();
		}
	}

	public int GetNumberOfAdvertsWatchedToday()
	{
		int num = 0;
		string text = SynchronisedClock.Instance.ServerTime.Date.ToString("d");
		int i = 0;
		for (int count = SecureStorage.Instance.RecordedVungleAds.Count; i < count; i++)
		{
			long result = 0L;
			if (long.TryParse(SecureStorage.Instance.RecordedVungleAds[i], out result))
			{
				string text2 = SynchronisedClock.ConvertUTCTimestampInSecondsToDateTime(result).Date.ToString("d");
				if (text == text2)
				{
					num++;
				}
			}
		}
		return num;
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			m_lastCheckedCostume = Costume.Max_Costumes;
		}
	}
}
