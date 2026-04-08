using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum SwipeBuffer
	{
		None = 0,
		Up = 1,
		Down = 2,
		Left = 3,
		Right = 4,
		LongUp = 5
	}

	public enum Potion_Type
	{
		None = 0,
		AntiVenom = 1,
		Spirit = 2,
		SpeedBoost = 3
	}

	public enum WhipRenderMode
	{
		Away = 0,
		Hand = 1,
		Extended = 2
	}

	private const float mMinCollectPitch = -3f;

	private const float mMaxCollectPitch = 1.5f;

	private const float mIncCollectPitch = 0.1f;

	public GameObject PlayerModel;

	public GameObject CatModel;

	public GameObject BirdModel;

	public GameObject MineCartModel;

	public GameObject BikeModel;

	public SkinnedMeshRenderer WhipMeshBelt;

	public SkinnedMeshRenderer WhipMeshHand;

	public SkinnedMeshRenderer WhipMeshExtend;

	public Animation mAnimation;

	public AnimationLoader mAnimationLoader;

	public List<CostumeDetail> mCostumeDetails = new List<CostumeDetail>();

	public Costume StartCostume;

	private Costume mCurrentCostume;

	private WorldConstructionHelper.Theme mCachedLastTheme = WorldConstructionHelper.Theme.Menu;

	private int m_DiamondsToContinueAttempts;

	public PlayerTweaks Settings = new PlayerTweaks();

	public GameObject GameLocation;

	public GameObject DebugBarLongUp;

	public GameObject DebugBarSwipeUp;

	public GameObject DebugBarSwipeDown;

	public GameObject DebugBarSwipeLeft;

	public GameObject DebugBarSwipeRight;

	public GameObject DebugBufferLongUp;

	public GameObject DebugBufferSwipeUp;

	public GameObject DebugBufferSwipeDown;

	public GameObject DebugBufferSwipeLeft;

	public GameObject DebugBufferSwipeRight;

	public GameObject SpiritOfTheJungleParticles;

	private SpiritOfTheJungle mSpiritOfTheJungle;

	private PlayerTrialsBoost mTrialsBoost;

	public bool DebugInvincible;

	public bool DisplaySpeed;

	public PlayerTheme m_PlayerTheme;

	private PlayerTheme.ThemeType mNextTheme;

	public PlayerTheme.ThemeType DebugThemeType;

	private GameObject mPairedModel;

	private AnimationLoader mPairedAnimationLoader;

	private bool m_TurningDisabled;

	public HingedRope mHingedRope;

	public SkinnedMeshRenderer CharacterRenderer;

	private SkinnedMeshRenderer PairedModelRenderer;

	public static PlayerController instance;

	private PlayerYoke Yoke;

	private bool mSlowingDown;

	private bool mSideStepLeft;

	private bool mSideStepRight;

	private float mCurrentSpeed;

	private float mMaxSpeed;

	private bool mGameRunning;

	private bool mWaitingForTrialsCountdown;

	private bool mHasFinishedTrial;

	public float FlickerTimeAfterJaguar = 2f;

	private float TimeSinceJaguarEnded;

	private bool mInCutScene;

	public bool mOutfitResurrectionAvailable;

	private Vector3 mStartPosition;

	private PlayerScore mScore = new PlayerScore();

	private float mPlayTime;

	public PlayerSFX SFX;

	public AudioSource PickupAudio;

	public AudioSource PlayerAudio;

	private int mLastPlayFrame;

	private Coin.CoinType mLastType = Coin.CoinType.Max;

	private float mCollectPitch;

	private int mCoinsMissed;

	private bool mQueueAnim;

	private bool mBeenOnADuckTile;

	private float mPressAction;

	private float mSwipeUpAction;

	private float mSwipeDownAction;

	private PlayerAnimationController mAnimationController;

	private float mTiltAmount;

	private float mLeftRight;

	private float mDodgeTime;

	private bool mAllowTilt;

	private bool mDodgeLeft;

	private bool mDodgeRight;

	private bool mDead;

	private PieceDescriptor.KillType mLastKillType;

	private SwipeBuffer mSwipeBuffer;

	private bool mIsInTurnBufferZoneLeft;

	private bool mIsInTurnBufferZoneRight;

	private bool mNoDodgeZone;

	private float mDebugBarScaleY;

	private TileLookaheadCache mPerceptionInfo = new TileLookaheadCache();

	private bool mCoinCollectionBonusActive;

	private float mInvincibleBonusTimeRemaining;

	private float mSpeedBoostBonusTimeRemaining;

	private float mFlickerTimer;

	private bool mCallOfTheMacaw;

	private SpeedIncrease mSpeedIncrease;

	private float mCurrentScreenFlashTime;

	private bool mAttacking;

	private float mPoisoned;

	private float mInjured;

	private bool mReturnToCentre;

	private WhipRenderMode mWhipRenderMode;

	private LevelGenerator.TurnType mHasTurned;

	public Costume CurrentCostume
	{
		get
		{
			return mCurrentCostume;
		}
		set
		{
			bool flag = false;
			foreach (CostumeDetail mCostumeDetail in mCostumeDetails)
			{
				if (mCostumeDetail.Type == value)
				{
					flag = true;
					SetUpNewCostume(mCostumeDetail);
				}
			}
			if (!flag)
			{
				SetUpNewCostume(mCostumeDetails[0]);
			}
		}
	}

	public int DiamondsToContinueAttempts
	{
		get
		{
			return m_DiamondsToContinueAttempts;
		}
		set
		{
			m_DiamondsToContinueAttempts = value;
		}
	}

	public PlayerTrialsBoost TrialsBoost
	{
		get
		{
			return mTrialsBoost;
		}
	}

	public bool TurningDisabled
	{
		get
		{
			return m_TurningDisabled;
		}
		set
		{
			m_TurningDisabled = value;
		}
	}

	public bool mAllowPlayerInput { get; set; }

	public bool mIgnoreThemeAndReturnToCentre { get; set; }

	public Potion_Type PotionToUse { get; set; }

	private void SetUpNewCostume(CostumeDetail newCostumeDetails)
	{
		UnityEngine.Object.Destroy(PlayerModel);
		mCurrentCostume = newCostumeDetails.Type;
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(newCostumeDetails.Costume);
		if (!(PlayerAnimationController)gameObject.GetComponent(typeof(PlayerAnimationController)))
		{
			PlayerAnimationController playerAnimationController = (PlayerAnimationController)gameObject.AddComponent(typeof(PlayerAnimationController));
			GameObject gameObject2 = GameObject.Find("Player");
			playerAnimationController.SFX = gameObject2.GetComponent<PlayerSFX>();
			playerAnimationController.PlayerController = gameObject2.GetComponent<PlayerController>();
			playerAnimationController.PlayerAnimation = gameObject.GetComponent<Animation>();
			playerAnimationController.AnimationLoader = mAnimationLoader;
			playerAnimationController.PlayerFXController = gameObject2.GetComponent<PlayerFXController>();
			playerAnimationController.PlayerShadow = gameObject2.transform.Find("Shadow").gameObject;
			playerAnimationController.PlayerShadow.SetActiveRecursively(true);
			mAnimationController = playerAnimationController;
		}
		gameObject.SetActiveRecursively(true);
		PlayerModel = gameObject;
		CharacterRenderer = (SkinnedMeshRenderer)gameObject.transform.Find(newCostumeDetails.CharacterRendererName).gameObject.GetComponent(typeof(SkinnedMeshRenderer));
		PlayerModel.transform.parent = base.transform;
		PlayerModel.transform.localPosition = Vector3.zero;
		PlayerModel.transform.localRotation = Quaternion.identity;
		PlayerModel.transform.localScale = new Vector3(1f, 1f, 1f);
		Animation animation = gameObject.GetComponent<Animation>();
		foreach (AnimationState item in mAnimation)
		{
			animation.AddClip(item.clip, item.name);
		}
		PlayerModel.GetComponent<Animation>().enabled = true;
		WhipMeshBelt = null;
		WhipMeshBelt = (SkinnedMeshRenderer)PlayerModel.transform.Find("WhipHand001").gameObject.GetComponent(typeof(SkinnedMeshRenderer));
		WhipMeshHand = null;
		WhipMeshHand = (SkinnedMeshRenderer)PlayerModel.transform.Find("WhipHand").gameObject.GetComponent(typeof(SkinnedMeshRenderer));
		WhipMeshExtend = FindSkinnedMeshRendererInAllChildren(PlayerModel, "WhipAnimated 1");
		SFX.SetCostumeSFX(newCostumeDetails.Sfx);
	}

	public SkinnedMeshRenderer FindSkinnedMeshRendererInAllChildren(GameObject currentGameObject, string name)
	{
		SkinnedMeshRenderer[] componentsInChildren = currentGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer.name.Equals(name))
			{
				return skinnedMeshRenderer;
			}
		}
		return null;
	}

	public SpiritOfTheJungle SpiritOfTheJungle()
	{
		return mSpiritOfTheJungle;
	}

	public PlayerTheme.ThemeType NextTheme()
	{
		return mNextTheme;
	}

	public PlayerTheme.ThemeType GetThemeType()
	{
		return m_PlayerTheme.Current();
	}

	public PlayerThemeSettings CurrentTheme()
	{
		return m_PlayerTheme.CurrentTheme();
	}

	public PlayerThemeSettings GetTheme(PlayerTheme.ThemeType themeType)
	{
		return m_PlayerTheme.GetTheme(themeType);
	}

	public void SetRopeState(HingedRope.RopeState newState)
	{
		mHingedRope.SetState(newState);
	}

	public bool HasValidRopeNode()
	{
		return mHingedRope.HasValidNode(LevelGenerator.Instance().GetCurrentPiece());
	}

	public static PlayerController Instance()
	{
		return instance;
	}

	public bool IsAnyInput()
	{
		return Yoke.AnyInput;
	}

	public bool IsDebug()
	{
		return Yoke.Debug;
	}

	public float GetRawTilt()
	{
		return Yoke.RawTiltAmount + ((!Yoke.TiltLeft) ? 0f : (-1f)) + ((!Yoke.TiltRight) ? 0f : 1f);
	}

	public bool IsSlowingDown()
	{
		return mSlowingDown;
	}

	public bool IsSideStepLeft()
	{
		return mSideStepLeft;
	}

	public void SideStepLeft()
	{
		mSideStepLeft = true;
	}

	public bool IsSideStepRight()
	{
		return mSideStepRight;
	}

	public void SideStepRight()
	{
		mSideStepRight = true;
	}

	public void ClearSideStep()
	{
		mSideStepRight = (mSideStepLeft = false);
	}

	public bool IsRunning()
	{
		return mGameRunning && !IsDead();
	}

	public bool PlayerGameOver()
	{
		return IsDead() && !SpiritOfTheJungle().Active() && !GameTutorial.Instance.IsEnabled && !IsOutfitResurrectionActive();
	}

	public bool IsWaitingForTrialsCountdown()
	{
		return mWaitingForTrialsCountdown;
	}

	public bool HasFinishedTrial()
	{
		return mHasFinishedTrial;
	}

	public bool SpeedBoostAffectsSpeed()
	{
		return mSpeedBoostBonusTimeRemaining > 0f;
	}

	public bool FlickerJaguar()
	{
		float num = mSpeedBoostBonusTimeRemaining / UpgradeHelper.CallOfTheJaguarDuration();
		return num < 0.25f && IsInSpeedBoostBonus();
	}

	public bool IsImmuneFromDamage()
	{
		return TimeSinceJaguarEnded < FlickerTimeAfterJaguar;
	}

	public void SetInCutScene(bool val)
	{
		mInCutScene = val;
	}

	public float GetCurrentSpeed()
	{
		float num = 0f;
		WorldConstructionController worldConstructionController = WorldConstructionController.Instance();
		if (worldConstructionController.DebugOverrides.OverrideSettings)
		{
			return worldConstructionController.DebugOverrides.PlayerSpeed;
		}
		float speedMultiplier = m_PlayerTheme.CurrentTheme().SpeedMultiplier;
		if (IsInSpeedBoostBonus() && SpeedBoostAffectsSpeed())
		{
			return mCurrentSpeed * Settings.SpeedBoostBonusMultiplier * speedMultiplier;
		}
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			if (TrialsBoost.IsActive)
			{
				if (!mAllowPlayerInput)
				{
					TrialsBoost.Deactivate();
					return mCurrentSpeed * speedMultiplier;
				}
				return mCurrentSpeed * speedMultiplier * TrialsDataManager.Instance.BoostSpeedMultiplier;
			}
		}
		else if (mSpeedIncrease != null && mSpeedIncrease.IsActive())
		{
			return mSpeedIncrease.GetCurrentSpeed() * speedMultiplier;
		}
		return mCurrentSpeed * speedMultiplier;
	}

	public float GetDefaultSpeedForTrials()
	{
		float speedMultiplier = m_PlayerTheme.CurrentTheme().SpeedMultiplier;
		return mCurrentSpeed * speedMultiplier;
	}

	public float GetCurrentSpeedWithoutSpeedBonuses()
	{
		return mCurrentSpeed;
	}

	public float GetSpeedAsPercent()
	{
		float num = Settings.MaxSpeed - Settings.InitialSpeed;
		float num2 = mCurrentSpeed - Settings.InitialSpeed;
		return num2 / num;
	}

	public float GetDistanceMoved()
	{
		return GetCurrentSpeed() * Time.deltaTime;
	}

	public float GetJumpDuration()
	{
		if (mAnimationController != null)
		{
			return mAnimationController.GetJumpLength();
		}
		return 0f;
	}

	public float GetJumpTime()
	{
		return Settings.ActionWarmDownTimeSwipeUp;
	}

	public float GetSlideTime()
	{
		return Settings.ActionWarmDownTimeSwipeDown;
	}

	public float GetMinimumSlideTime()
	{
		return Settings.MinimumSlideTime;
	}

	public float GetMaxSpeed()
	{
		return mMaxSpeed;
	}

	public Vector3 GetStartPosition()
	{
		return mStartPosition;
	}

	public PlayerScore Score()
	{
		return mScore;
	}

	public float GetPlayTime()
	{
		return mPlayTime;
	}

	public void CollectCoin(Coin.CoinType type)
	{
		mScore.AddTreasure(type);
		if (mLastType != type)
		{
			mLastType = type;
			mCollectPitch = -3f;
		}
		else if (Time.renderedFrameCount - mLastPlayFrame > 100)
		{
			mCollectPitch = -3f;
		}
		else
		{
			mCollectPitch = Mathf.Min(mCollectPitch + 0.1f, 1.5f);
		}
		mLastPlayFrame = Time.renderedFrameCount;
		SFX.PlayCollect(mCollectPitch);
	}

	public int GetCoinsMissed()
	{
		return mCoinsMissed;
	}

	public void MissCoins(int count)
	{
		mCoinsMissed += count;
	}

	public bool IsAnimQueued()
	{
		bool result = mQueueAnim;
		mQueueAnim = false;
		return result;
	}

	public bool IsPressing()
	{
		return mPressAction > 0f;
	}

	public bool IsSwipingUp()
	{
		return mSwipeUpAction > 0f;
	}

	public void ClearSwipeUp()
	{
		mSwipeUpAction = 0f;
	}

	public bool IsSwipingDown()
	{
		return mSwipeDownAction > 0f;
	}

	public PlayerAnimationController GetPlayerAnimationController()
	{
		return mAnimationController;
	}

	public bool IsSliding()
	{
		if (mAnimationController != null)
		{
			return mAnimationController.IsSliding();
		}
		return false;
	}

	public bool IsJumping()
	{
		if (mAnimationController != null)
		{
			return mAnimationController.IsJumping();
		}
		return false;
	}

	public bool IsDoubleSwipingUp()
	{
		return mSwipeUpAction > 0f && mSwipeBuffer == SwipeBuffer.LongUp;
	}

	public void ClearSwipeBuffer()
	{
		mSwipeBuffer = SwipeBuffer.None;
	}

	public float GetLeftRight()
	{
		return mLeftRight;
	}

	public void SetLeftRight(float leftRight)
	{
		if (mAllowTilt)
		{
			mLeftRight = leftRight;
		}
	}

	public float GetTiltAmount()
	{
		return Mathf.Clamp(Settings.PlayerHorizontalMovementScale * mTiltAmount, -1f, 1f);
	}

	public bool AllowTilt()
	{
		return mAllowTilt;
	}

	public void DodgeLeft()
	{
		FinishDodge();
		mDodgeLeft = true;
		if (CurrentTheme().CanSideStep)
		{
			mSideStepLeft = true;
		}
		FinishTurns();
	}

	public void DodgeRight()
	{
		FinishDodge();
		mDodgeRight = true;
		if (CurrentTheme().CanSideStep)
		{
			mSideStepRight = true;
		}
		FinishTurns();
	}

	public bool IsDodging()
	{
		return mDodgeLeft || mDodgeRight;
	}

	public PieceDescriptor.KillType LastKillType()
	{
		return mLastKillType;
	}

	public bool IsTurningLeft()
	{
		return mSwipeBuffer == SwipeBuffer.Left;
	}

	public bool IsTurningRight()
	{
		return mSwipeBuffer == SwipeBuffer.Right;
	}

	public bool IsInTurnBufferZoneLeft()
	{
		return mIsInTurnBufferZoneLeft;
	}

	public bool IsInTurnBufferZoneRight()
	{
		return mIsInTurnBufferZoneRight;
	}

	public bool IsInTurnBufferZoneLeftRight()
	{
		return mIsInTurnBufferZoneLeft && mIsInTurnBufferZoneRight;
	}

	public void SetInTurnBufferZoneLeft()
	{
		mIsInTurnBufferZoneLeft = true;
	}

	public void SetInTurnBufferZoneRight()
	{
		mIsInTurnBufferZoneRight = true;
	}

	public void SetInTurnBufferZoneLeftRight(bool active)
	{
		mIsInTurnBufferZoneLeft = (mIsInTurnBufferZoneRight = active);
	}

	public void SetInNoDodgeZone(bool val)
	{
		mNoDodgeZone = val;
	}

	public bool InNoDodgeZone()
	{
		return mNoDodgeZone;
	}

	public TileLookaheadCache GetPerceptionInfo()
	{
		return mPerceptionInfo;
	}

	public bool IsInCoinCollectionBonus()
	{
		return mCoinCollectionBonusActive;
	}

	public void SetInvincibleBonus()
	{
		mInvincibleBonusTimeRemaining = Settings.InvincibleBonusTime;
	}

	public bool IsInInvincibleBonus()
	{
		return mInvincibleBonusTimeRemaining > 0f;
	}

	public float GetInvincibleBonusTimeRemaining()
	{
		return mInvincibleBonusTimeRemaining;
	}

	public void SetInvincibleBonus(float time)
	{
		mInvincibleBonusTimeRemaining = time;
	}

	public void SetSpeedBoostBonus()
	{
		mSpeedBoostBonusTimeRemaining = UpgradeHelper.CallOfTheJaguarDuration();
		SwitchTheme(PlayerTheme.ThemeType.Jaguar);
		SwitchToVehicleCam();
		mScore.IncJaguar();
		if (mAnimationController != null)
		{
			SFX.CatRideStart();
			mAnimationController.PlayerFXController.StartJaguarEffect();
		}
	}

	public bool IsInSpeedBoostBonus()
	{
		return mSpeedBoostBonusTimeRemaining > 0f;
	}

	public float GetSpeedBoostBonusTimeRemaining()
	{
		return mSpeedBoostBonusTimeRemaining;
	}

	public bool IsDead()
	{
		return mDead;
	}

	public void CallOfTheMacaw(bool val)
	{
		mCallOfTheMacaw = val;
		if (val)
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.MacawCam, CameraTransitionData.JumpCut);
		}
		else
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.DefaultCam);
		}
	}

	public bool CallOfTheMacawActive()
	{
		return mCallOfTheMacaw;
	}

	public SpeedIncrease GetSpeedIncreasePowerup()
	{
		return mSpeedIncrease;
	}

	public bool IsAttacking()
	{
		return mAttacking;
	}

	public bool IsPoisoned()
	{
		return mPoisoned > 0f;
	}

	public float GetPoisonedTimeRemaining()
	{
		return mPoisoned;
	}

	public void Poison(BaddieController.Type type)
	{
		mPoisoned = UpgradeHelper.PoisonDuration();
		if (type == BaddieController.Type.Scorpion)
		{
			mScore.IncStung();
		}
		else
		{
			mScore.IncPoisoned();
		}
		PoisonHUDController.Instance().Show();
		mAnimationController.SetPoisoned();
		SFX.Poisoned();
	}

	public void ClearPoison()
	{
		mPoisoned = 0f;
		PoisonHUDController.Instance().Hide();
		SFX.PoisonRecovery();
	}

	public bool UseAntiVenom()
	{
		SecureStorage.Instance.ChangeItemCount("consumable.antivenom", -1);
		PotionToUse = Potion_Type.AntiVenom;
		GetPlayerAnimationController().PlayerFXController.PlayAntiVenomEffect();
		return true;
	}

	public Potion_Type InitiatedDrink()
	{
		Potion_Type potionToUse = PotionToUse;
		PotionToUse = Potion_Type.None;
		return potionToUse;
	}

	public bool IsInjured()
	{
		return mInjured > 0f;
	}

	public float GetInjuredTimeRemaining()
	{
		return mInjured;
	}

	public void Injure()
	{
		mInjured = 5f;
	}

	public void ClearInjury()
	{
		mInjured = 0f;
	}

	public void OverrideReturnToCentre(bool newValue)
	{
		mReturnToCentre = newValue;
	}

	public bool AllowReturnToCentre()
	{
		return mReturnToCentre;
	}

	public bool IsOnBaddiePiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece())
		{
			return LevelGenerator.Instance().GetCurrentPiece().IsBaddiePiece();
		}
		return false;
	}

	public bool IsNearRopePiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece() && (bool)LevelGenerator.Instance().GetCurrentPiece().GetNextPiece() && LevelGenerator.Instance().GetCurrentPiece().GetNextPiece()
			.IsRopePiece())
		{
			return true;
		}
		return false;
	}

	public bool IsNearJumpPiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece() && (bool)LevelGenerator.Instance().GetCurrentPiece().GetNextPiece() && LevelGenerator.Instance().GetCurrentPiece().GetNextPiece()
			.IsJumpPiece())
		{
			return true;
		}
		return false;
	}

	public bool IsOnRopePiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece())
		{
			return LevelGenerator.Instance().GetCurrentPiece().IsRopePiece();
		}
		return false;
	}

	public bool IsOnJumpPiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece() && WorldConstructionHelper.IsRopeSwing(LevelGenerator.Instance().GetCurrentPiece().GetPieceType()))
		{
			return true;
		}
		return false;
	}

	public bool IsOnPitPiece()
	{
		if ((bool)LevelGenerator.Instance().GetCurrentPiece() && WorldConstructionHelper.IsPit(LevelGenerator.Instance().GetCurrentPiece().GetPieceType()))
		{
			return true;
		}
		return false;
	}

	public void SetIdle()
	{
		mDead = false;
		mGameRunning = false;
		mWaitingForTrialsCountdown = false;
		mHasFinishedTrial = false;
	}

	public void SetWaitingForTrialStart()
	{
		mDead = false;
		mGameRunning = false;
		mWaitingForTrialsCountdown = true;
		mHasFinishedTrial = false;
	}

	public void SetTrialComplete()
	{
		mWaitingForTrialsCountdown = false;
		mHasFinishedTrial = true;
		mGameRunning = false;
	}

	private void Awake()
	{
		instance = this;
		DebugInvincible = false;
		DisplaySpeed = false;
		mSpiritOfTheJungle = new SpiritOfTheJungle();
		Score().ResetOnce();
		PotionToUse = Potion_Type.None;
		mTrialsBoost = new PlayerTrialsBoost();
	}

	private void Start()
	{
		mSpeedIncrease = new SpeedIncrease(this);
		mDebugBarScaleY = DebugBarSwipeUp.transform.localScale.y;
		base.transform.forward = new Vector3(0f, 0f, 1f);
		mStartPosition = base.transform.position;
		Reset();
		Yoke = GetComponent(typeof(PlayerYoke)) as PlayerYoke;
		mAnimationController = PlayerModel.GetComponentInChildren<PlayerAnimationController>();
		BikeModel.GetComponent<Animation>().cullingType = AnimationCullingType.BasedOnClipBounds;
		MineCartModel.GetComponent<Animation>().cullingType = AnimationCullingType.BasedOnClipBounds;
	}

	private void Update()
	{
		if (!IsRunning() || GameController.Instance.Paused())
		{
			return;
		}
		UpdateSwipeBuffer();
		if (!mAllowPlayerInput)
		{
			Yoke.Reset();
		}
		if (Yoke.Jump && GetCurrentSpeed() > 0f && GetThemeType() != PlayerTheme.ThemeType.Jaguar)
		{
			if (mSwipeUpAction > 0f)
			{
				Buffer(SwipeBuffer.Up);
			}
			else
			{
				mSwipeDownAction = 0f;
				Swipe(SwipeBuffer.Up);
			}
		}
		if (Yoke.Slide && GetCurrentSpeed() > 0f)
		{
			if (!IsInAction())
			{
				Swipe(SwipeBuffer.Down);
			}
			else if (mSwipeBuffer == SwipeBuffer.None && mSwipeUpAction > 0f)
			{
				Buffer(SwipeBuffer.Down);
			}
		}
		if (Yoke.TurnLeft)
		{
			Buffer(SwipeBuffer.Left);
			mSideStepRight = false;
			SideStepLeft();
		}
		if (Yoke.TurnRight)
		{
			Buffer(SwipeBuffer.Right);
			mSideStepLeft = false;
			SideStepRight();
		}
		UpdateCooldownTimers();
		UpdateSwipeBufferForPress();
		if (!mDodgeLeft && !mDodgeRight)
		{
			mDodgeTime = 0f;
		}
		else
		{
			mDodgeTime += Time.deltaTime;
		}
		mAllowTilt = true;
		if ((IsNearRopePiece() && IsSwipingUp()) || IsOnRopePiece() || CallOfTheMacawActive())
		{
			mAllowTilt = false;
			mLeftRight = Mathf.Lerp(mLeftRight, 0f, 5f * Time.deltaTime);
		}
		if (IsNearRopePiece() || IsOnRopePiece())
		{
			mDodgeLeft = (mDodgeRight = false);
		}
		if (IsOnBaddiePiece())
		{
			InputController.Instance().SetTapBias(2f);
		}
		else
		{
			InputController.Instance().SetTapBias(1f);
		}
		PlayerThemeSettings playerThemeSettings = Instance().CurrentTheme();
		float minimumDodgeTime = CurrentTheme().MinimumDodgeTime;
		if (mDodgeLeft)
		{
			if (playerThemeSettings.ReturnToCentre && AllowReturnToCentre())
			{
				mTiltAmount = Mathf.Max(mTiltAmount - Time.deltaTime * playerThemeSettings.DodgeTiltSpeed, -1f);
			}
			else
			{
				mTiltAmount = -1f;
			}
			if (mDodgeTime > minimumDodgeTime && mDodgeTime >= playerThemeSettings.DodgeTiltTime)
			{
				FinishDodge();
			}
		}
		else if (mDodgeRight)
		{
			if (playerThemeSettings.ReturnToCentre && AllowReturnToCentre())
			{
				mTiltAmount = Mathf.Min(mTiltAmount + Time.deltaTime * playerThemeSettings.DodgeTiltSpeed, 1f);
			}
			else
			{
				mTiltAmount = 1f;
			}
			if (mDodgeTime > minimumDodgeTime && mDodgeTime >= playerThemeSettings.DodgeTiltTime)
			{
				FinishDodge();
			}
		}
		else if (Yoke.TiltLeft && mAllowTilt)
		{
			mTiltAmount = Mathf.Max(mTiltAmount - Time.deltaTime * playerThemeSettings.TiltSpeed, -1f);
		}
		else if (Yoke.TiltRight && mAllowTilt)
		{
			mTiltAmount = Mathf.Min(mTiltAmount + Time.deltaTime * playerThemeSettings.TiltSpeed, 1f);
		}
		else if (Yoke.Tilt && mAllowTilt)
		{
			mTiltAmount += Time.deltaTime * playerThemeSettings.TiltSmoothing * (Yoke.TiltAmount - mTiltAmount);
		}
		else
		{
			float num = Time.deltaTime * playerThemeSettings.TiltSpeed;
			if (!(mTiltAmount > 0f))
			{
				mTiltAmount = Mathf.Min(mTiltAmount + num, 0f);
			}
		}
		mAttacking = false;
		if ((!IsSwipingDown() && !IsSwipingUp()) || GetThemeType() == PlayerTheme.ThemeType.Jaguar)
		{
			bool flag = false;
			if (GetThemeType() != PlayerTheme.ThemeType.Cart && GetThemeType() != PlayerTheme.ThemeType.Bike && GetThemeType() != PlayerTheme.ThemeType.Jaguar)
			{
				flag = Yoke.Tap;
				if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Ninja) && BaddieController.Instance().AreAnyBaddiesInRadius(base.transform.position, 25f))
				{
					flag = true;
				}
			}
			if (flag)
			{
				BaddieController.Instance().KillAllInRadius(base.transform.position, 25f);
				mAttacking = true;
			}
			if (GetThemeType() == PlayerTheme.ThemeType.Jaguar && !mAttacking)
			{
				BaddieController.Instance().KillAllInRadius(base.transform.position, 10f);
			}
		}
		mCurrentSpeed += Settings.Acceleration * Time.deltaTime;
		mCurrentSpeed = Mathf.Min(mCurrentSpeed, mMaxSpeed);
		if (LevelGenerator.Instance().CurrentGameType != GameType.GT_TRIALS)
		{
			mMaxSpeed += GetDistanceMoved() * Settings.SpeedIncrease;
			mMaxSpeed = Mathf.Clamp(mMaxSpeed, Settings.InitialSpeed, Settings.MaxSpeed);
		}
		if (mInvincibleBonusTimeRemaining > 0f)
		{
			mInvincibleBonusTimeRemaining -= Time.deltaTime;
		}
		if (mSpeedBoostBonusTimeRemaining > 0f)
		{
			mSpeedBoostBonusTimeRemaining -= Time.deltaTime;
		}
		if (!SpeedBoostAffectsSpeed() && GetThemeType() == PlayerTheme.ThemeType.Jaguar)
		{
			if (mAnimationController != null)
			{
				TimeSinceJaguarEnded = 0f;
				mAnimationController.EndJaguar();
			}
		}
		else
		{
			TimeSinceJaguarEnded += Time.deltaTime;
			if (!IsImmuneFromDamage())
			{
				EnableCharacterRenderer(true);
			}
		}
		if ((SpeedBoostAffectsSpeed() && UpgradeHelper.CallOfTheJaguarAutoCoinCollect()) || OutfitOfTheDayManager.Instance.BonusApplies(Costume.Pirate))
		{
			mCoinCollectionBonusActive = true;
		}
		else
		{
			mCoinCollectionBonusActive = false;
		}
		if (IsFlickering())
		{
			mFlickerTimer -= Time.deltaTime;
			if (mFlickerTimer <= 0f)
			{
				mFlickerTimer = 0.2f;
				EnableCharacterRenderer(!CharacterRendererEnabled());
			}
		}
		else if (!CharacterRendererEnabled())
		{
			EnableCharacterRenderer(true);
		}
		mPlayTime += Time.deltaTime;
		SpiritOfTheJungle().Update();
		SpiritOfTheJungleParticles.active = false;
		mSpeedIncrease.Update();
		CheckPointController.Instance().PlayerCheckpointChecks(base.transform.position, base.transform.forward, mScore.DistanceTravelled());
		UpdatePlayerDebugTheme();
		if (mPoisoned > 0f)
		{
			mPoisoned -= Time.deltaTime;
			if (mPoisoned <= 0f)
			{
				ClearPoison();
			}
		}
		if (mInjured > 0f)
		{
			mInjured -= Time.deltaTime;
			if (mInjured <= 0f)
			{
				mInjured = 0f;
			}
		}
		if (LevelGenerator.Instance() != null && LevelGenerator.Instance().GetCurrentPiece() != null && m_PlayerTheme != null && CameraManager.Instance != null)
		{
			if (LevelGenerator.Instance().GetCurrentPiece().Theme != WorldConstructionHelper.Theme.Mountain && m_PlayerTheme.Current() == PlayerTheme.ThemeType.Jaguar && !CameraManager.Instance.CurrentlyUsingPowerupCam)
			{
				SwitchToVehicleCam();
			}
			if (mCachedLastTheme != LevelGenerator.Instance().GetCurrentPiece().Theme)
			{
				mCachedLastTheme = LevelGenerator.Instance().GetCurrentPiece().Theme;
				SwrveEventsGameplay.TileSetChange(mCachedLastTheme);
			}
		}
		DrawDebug();
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			mTrialsBoost.Update();
		}
	}

	public int GetCurrentSpeedIndex()
	{
		int num = (int)(mCurrentSpeed / 5.5f);
		if (num > 4)
		{
			num = 4;
		}
		return num;
	}

	private void UpdatePlayerDebugTheme()
	{
		if (DebugThemeType != PlayerTheme.ThemeType.None && DebugThemeType != m_PlayerTheme.Current())
		{
			SwitchTheme(DebugThemeType);
		}
	}

	public void StartThemeTransition()
	{
		mNextTheme = PlayerTheme.ThemeType.None;
		PieceDescriptor pieceDescriptor = LevelGenerator.Instance().GetCurrentPiece();
		do
		{
			if (pieceDescriptor != null)
			{
				mNextTheme = ConvertLevelThemeToPlayerTheme(pieceDescriptor.Theme);
				if (mNextTheme == GetThemeType())
				{
					pieceDescriptor = pieceDescriptor.GetNextPiece();
				}
			}
		}
		while ((mNextTheme == GetThemeType() || mNextTheme == PlayerTheme.ThemeType.None) && pieceDescriptor != null);
	}

	public void SwitchPlayerTheme()
	{
		if (mNextTheme != m_PlayerTheme.Current())
		{
			SwitchTheme(mNextTheme);
			mNextTheme = PlayerTheme.ThemeType.None;
		}
	}

	public PlayerTheme.ThemeType ConvertLevelThemeToPlayerTheme(WorldConstructionHelper.Theme worldTheme)
	{
		switch (worldTheme)
		{
		case WorldConstructionHelper.Theme.Minecart:
			return PlayerTheme.ThemeType.Cart;
		case WorldConstructionHelper.Theme.Bike:
			return PlayerTheme.ThemeType.Bike;
		case WorldConstructionHelper.Theme.SlippedMountain:
			return PlayerTheme.ThemeType.Slope;
		default:
			return PlayerTheme.ThemeType.Run;
		}
	}

	public void EnableCharacterRenderer(bool enable)
	{
		CharacterRenderer.enabled = enable;
		if (mPairedModel != null && PairedModelRenderer != null)
		{
			PairedModelRenderer.enabled = enable;
		}
		UpdateWhipVisibility();
	}

	public bool CharacterRendererEnabled()
	{
		return CharacterRenderer.enabled;
	}

	public void SetWhipRenderMode(WhipRenderMode mode)
	{
		mWhipRenderMode = mode;
		UpdateWhipVisibility();
	}

	private void UpdateWhipVisibility()
	{
		if (WhipMeshBelt != null)
		{
			WhipMeshBelt.enabled = mWhipRenderMode == WhipRenderMode.Away && CharacterRenderer.enabled;
		}
		if (WhipMeshHand != null)
		{
			WhipMeshHand.enabled = mWhipRenderMode == WhipRenderMode.Hand && CharacterRenderer.enabled;
		}
		if (WhipMeshExtend != null)
		{
			WhipMeshExtend.enabled = mWhipRenderMode == WhipRenderMode.Extended && CharacterRenderer.enabled;
		}
	}

	public void Reset()
	{
		mPerceptionInfo.Reset();
		mBeenOnADuckTile = false;
		mSwipeUpAction = (mSwipeDownAction = (mPressAction = 0f));
		mSwipeBuffer = SwipeBuffer.None;
		mTiltAmount = 0f;
		mDodgeLeft = (mDodgeRight = false);
		mIsInTurnBufferZoneLeft = (mIsInTurnBufferZoneRight = false);
		mNoDodgeZone = false;
		mAttacking = false;
		mPoisoned = 0f;
		mHasTurned = LevelGenerator.TurnType.None;
		SetWhipRenderMode(WhipRenderMode.Hand);
		mCoinsMissed = 0;
		mCoinCollectionBonusActive = false;
		mInvincibleBonusTimeRemaining = (mSpeedBoostBonusTimeRemaining = 0f);
		mPlayTime = 0f;
		mLeftRight = 0f;
		mAllowTilt = false;
		mDodgeTime = 0f;
		mCallOfTheMacaw = false;
		mPairedModel = null;
		mPairedAnimationLoader = null;
		mSpiritOfTheJungle.Reset();
		mCurrentSpeed = (mMaxSpeed = Settings.InitialSpeed);
		mInCutScene = false;
		mSlowingDown = false;
		mSideStepLeft = false;
		mSideStepRight = false;
		mQueueAnim = false;
		mDead = false;
		mFlickerTimer = 0f;
		DebugThemeType = PlayerTheme.ThemeType.None;
		mNextTheme = PlayerTheme.ThemeType.None;
		SwitchTheme(Instance().ConvertLevelThemeToPlayerTheme(LevelGenerator.Instance().StartTheme));
		SetRopeState(HingedRope.RopeState.WaitForNode);
		mAllowPlayerInput = true;
		mIgnoreThemeAndReturnToCentre = false;
		mSpeedIncrease.Reset();
		mScore.Reset();
		mReturnToCentre = CurrentTheme().ReturnToCentre;
		LevelGenerator.Instance().SetPlayerPositionFromLeftRightValue(base.transform.forward);
		mAnimationController = PlayerModel.GetComponentInChildren<PlayerAnimationController>();
		mTrialsBoost.Reset();
		mWaitingForTrialsCountdown = false;
		mHasFinishedTrial = false;
		Debug.Log("(ewh) resetting diamonds to continue attempts on Reset");
		m_DiamondsToContinueAttempts = 0;
	}

	public void SwitchTheme(PlayerTheme.ThemeType theme)
	{
		PlayerTheme.ThemeType themeType = GetThemeType();
		TBFUtils.DebugLog("Switch theme to: " + theme);
		m_PlayerTheme.SetThemeType(theme);
		UpdatePairedModelVisibility();
		MineCartModel.SetActiveRecursively(false);
		BikeModel.SetActiveRecursively(false);
	}

	public void UpdatePairedModelVisibility()
	{
		switch (m_PlayerTheme.Current())
		{
		case PlayerTheme.ThemeType.Run:
		case PlayerTheme.ThemeType.Slope:
			mPairedModel = null;
			PairedModelRenderer = null;
			break;
		case PlayerTheme.ThemeType.Bike:
			mPairedModel = BikeModel;
			PairedModelRenderer = (SkinnedMeshRenderer)mPairedModel.transform.Find("bike").gameObject.GetComponent(typeof(SkinnedMeshRenderer));
			break;
		case PlayerTheme.ThemeType.Cart:
			mPairedModel = MineCartModel;
			PairedModelRenderer = (SkinnedMeshRenderer)mPairedModel.transform.Find("Cart").gameObject.GetComponent(typeof(SkinnedMeshRenderer));
			break;
		case PlayerTheme.ThemeType.Jaguar:
			mPairedModel = CatModel;
			PairedModelRenderer = (SkinnedMeshRenderer)mPairedModel.transform.Find("Cat").gameObject.GetComponent(typeof(SkinnedMeshRenderer));
			break;
		}
		if (PairedModelRenderer != null)
		{
			PairedModelRenderer.enabled = true;
		}
		MineCartModel.SetActiveRecursively(m_PlayerTheme.Current() == PlayerTheme.ThemeType.Cart && !SpeedBoostAffectsSpeed());
		BikeModel.SetActiveRecursively(m_PlayerTheme.Current() == PlayerTheme.ThemeType.Bike && !SpeedBoostAffectsSpeed());
		CatModel.SetActiveRecursively(m_PlayerTheme.Current() == PlayerTheme.ThemeType.Jaguar);
		if (mPairedModel == null)
		{
			mPairedAnimationLoader = null;
		}
		else
		{
			mPairedAnimationLoader = mPairedModel.GetComponent<AnimationLoader>();
		}
	}

	public GameObject PairedModel()
	{
		return mPairedModel;
	}

	public AnimationLoader PairedAnimationLoader()
	{
		return mPairedAnimationLoader;
	}

	public void OverrideDistance(float dist)
	{
		mScore.OverrideDistance(dist);
	}

	public void RegisterTurn(LevelGenerator.TurnType turnType)
	{
		mHasTurned = turnType;
	}

	public void ClearHasTurned()
	{
		mHasTurned = LevelGenerator.TurnType.None;
	}

	public LevelGenerator.TurnType HasTurned()
	{
		LevelGenerator.TurnType result = mHasTurned;
		ClearHasTurned();
		return result;
	}

	public void FinishDodge()
	{
		mDodgeLeft = (mDodgeRight = false);
		mDodgeTime = 0f;
	}

	public void FinishTurns()
	{
		mSwipeBuffer = SwipeBuffer.None;
	}

	public void Kill()
	{
		Kill(PieceDescriptor.KillType.Wall);
	}

	public void Kill(PieceDescriptor piece)
	{
		TBFUtils.DebugLog("Killed by piece: " + piece.name);
		PieceDescriptor.KillType killType = piece.KilledBy;
		if (killType == PieceDescriptor.KillType.None)
		{
			killType = ((!WorldConstructionHelper.IsPit(piece.TypeId)) ? PieceDescriptor.KillType.Wall : PieceDescriptor.KillType.Pit);
		}
		Kill(killType);
	}

	public void Kill(PieceDescriptor.KillType type)
	{
		if (!DebugInvincible)
		{
			mCurrentSpeed = Settings.InitialSpeed;
			mSpeedIncrease.Reset();
			mDead = true;
			mLastKillType = type;
			MonoBehaviour.print("Last Kill Type: " + type);
			if (LevelGenerator.Instance().GetCurrentPiece() != null)
			{
				LevelGenerator.Instance().UpdateDeathReport(LevelGenerator.Instance().GetCurrentPiece());
			}
			SFX.KillSoundLoops();
			CameraManager.Instance.SwitchToDeathCamera(type, GetThemeType());
		}
	}

	public void ActOnPerception()
	{
		SetInTurnBufferZoneLeftRight(false);
		SetInNoDodgeZone(false);
		float distance = 0f;
		bool flag = false;
		if (GetPerceptionInfo().IsEventRegistered(Tile.ResponseType.SwipeLeft, out distance))
		{
			if (distance < Settings.TurnBufferLength)
			{
				SetInTurnBufferZoneLeft();
				flag = true;
				SetInNoDodgeZone(true);
			}
			else if (distance < Settings.NoDodgeZoneLength)
			{
				SetInNoDodgeZone(true);
				FinishTurns();
			}
		}
		if (GetPerceptionInfo().IsEventRegistered(Tile.ResponseType.SwipeRight, out distance))
		{
			if (distance < Settings.TurnBufferLength)
			{
				SetInTurnBufferZoneRight();
				flag = true;
				SetInNoDodgeZone(true);
			}
			else if (distance < Settings.NoDodgeZoneLength)
			{
				SetInNoDodgeZone(true);
				FinishTurns();
			}
		}
		bool flag2 = false;
		if (GetPerceptionInfo().IsEventRegistered(Tile.ResponseType.SwipeUp, out distance))
		{
			flag2 = true;
		}
		else if (GetPerceptionInfo().IsEventRegistered(Tile.ResponseType.SwipeDown, out distance))
		{
			flag2 = true;
		}
		if (flag)
		{
			InputController.Instance().SetSwipeBias(InputController.SwipeBias.Horizontal);
		}
		else if (flag2)
		{
			InputController.Instance().SetSwipeBias(InputController.SwipeBias.Vertical);
		}
		else
		{
			InputController.Instance().SetSwipeBias(InputController.SwipeBias.None);
		}
	}

	public void StartGame(float distance)
	{
		TimeSinceJaguarEnded = FlickerTimeAfterJaguar;
		mCurrentSpeed = (mMaxSpeed = Settings.InitialSpeed);
		if (distance > 0f)
		{
			mMaxSpeed += distance * Settings.SpeedIncrease;
			mCurrentSpeed = mMaxSpeed - Settings.RestartSpeedOffset;
			mCurrentSpeed = Mathf.Clamp(mCurrentSpeed, Settings.InitialSpeed, mMaxSpeed);
			mScore.RunStartDistance = distance;
		}
		mOutfitResurrectionAvailable = false;
		if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Fairy))
		{
			mOutfitResurrectionAvailable = true;
		}
		mGameRunning = true;
		Debug.Log("(ewh) resetting diamonds to continue attempts on StartGame");
		m_DiamondsToContinueAttempts = 0;
	}

	public void StartTrialsGame(float trialSpeed)
	{
		mCurrentSpeed = (mMaxSpeed = trialSpeed);
		mOutfitResurrectionAvailable = false;
		mGameRunning = true;
		Debug.Log("(ewh) resetting diamonds to continue attempts on StartTrialsGame");
		m_DiamondsToContinueAttempts = 0;
	}

	public void StartGameFromContinue()
	{
		TimeSinceJaguarEnded = FlickerTimeAfterJaguar;
		mDead = false;
		SetInvincibleBonus(1f);
		if (GameTutorial.Instance.IsEnabled)
		{
			LevelGenerator.Instance().RecoverInTutorial();
		}
		else
		{
			if (IsOutfitResurrectionActive() && !SpiritOfTheJungle().Active())
			{
				UseOutfitResurrection();
			}
			mCurrentSpeed = mMaxSpeed - Settings.RestartSpeedOffset;
			mCurrentSpeed = Mathf.Clamp(mCurrentSpeed, Settings.InitialSpeed, mMaxSpeed);
			LevelGenerator.Instance().RecoverAfterRessurection();
			ClearPoison();
		}
		SetRopeState(HingedRope.RopeState.WaitForNode);
	}

	public void StartGameFromMacaw()
	{
		TimeSinceJaguarEnded = FlickerTimeAfterJaguar;
		mDead = false;
		SetRopeState(HingedRope.RopeState.WaitForNode);
		Debug.Log("(ewh) resetting diamonds to continue attempts on StartGameFromMacaw");
		m_DiamondsToContinueAttempts = 0;
	}

	public float GetDefaultCoinCollectionRange()
	{
		float num = Settings.CoinCollectionRangeDefault;
		if (GetThemeType() == PlayerTheme.ThemeType.Bike)
		{
			num *= Settings.CoinBikeCollectionMultiplier;
		}
		return num;
	}

	public float GetCoinCollectionRange()
	{
		if (mCoinCollectionBonusActive)
		{
			return Settings.CoinCollectionRangeBonus;
		}
		return GetDefaultCoinCollectionRange();
	}

	public bool IsInvincible()
	{
		return IsInSpeedBoostBonus() || IsInInvincibleBonus() || IsImmuneFromDamage();
	}

	public bool IsOutfitResurrectionActive()
	{
		return mOutfitResurrectionAvailable;
	}

	public void UseOutfitResurrection()
	{
		mOutfitResurrectionAvailable = false;
	}

	public void GameEnded()
	{
		int num = (int)mScore.DistanceTravelled();
		int treasureCollected = mScore.CoinsCollected();
		int timesUsed = mScore.JaguarsCollected();
		int timesUsed2 = mScore.NumMacawsUsed();
		int timesPoisoned = mScore.TimesPoisoned();
		int nKilled = mScore.ScorpionsKilled();
		int nTimes = mScore.TimesStung();
		int nKilled2 = mScore.BaddiesKilled(BaddieController.Type.Crocodile);
		int nKilled3 = mScore.BaddiesKilled(BaddieController.Type.Snake);
		int timesUsed3 = mScore.RopeSwingsUsed();
		int timesUsed4 = mScore.NumHasteTonicsUsed();
		int timesUsed5 = mScore.NumAntiVenomsUsed();
		int timesUsed6 = mScore.NumLifeTonicsUsed();
		int amount = mScore.NumStylishDismounts();
		int amount2 = mScore.TimesTutorialPlayed();
		int distance = (int)mScore.DistanceInMineCart();
		int distance2 = (int)mScore.DistanceOnBike();
		SecureStorage secureStorage = SecureStorage.Instance;
		int num2 = num - (int)mScore.RunStartDistance;
		int nPassed = CheckPointController.Instance().NumCheckpointsPassed(num2);
		if (!GameController.Instance.IsPlayingTrialsMode)
		{
			secureStorage.UpdateFurthestDistanceRun(num);
			if (MobileNetworkManager.Instance != null && MobileNetworkManager.Instance.IsLoggedIn)
			{
				ScoreRetriever.Instance().UpdateMyRun(num);
				MobileNetworkManager.Instance.reportScore(num, "runner.distance");
				MobileNetworkManager.Instance.reportScore(mScore.TotalScoreXPAdjusted(), "runner.score");
				MobileNetworkManager.Instance.reportScore(mScore.AnimalsKilled(), "runner.enemies");
			}
			SwrveEventsGameplay.GameEnded();
		}
		secureStorage.UpdateDistanceRun(num2);
		secureStorage.UpdateNumRuns(1);
		secureStorage.UpdateTimeSpent(mScore.TimeSpentRunning());
		secureStorage.UpdateTreasureCollected(treasureCollected);
		secureStorage.UpdateJaguarCount(timesUsed);
		secureStorage.UpdateMacawCount(timesUsed2);
		secureStorage.UpdatePoisonedCount(timesPoisoned);
		secureStorage.UpdateTimesStungByScorpions(nTimes);
		secureStorage.UpdateScorpionsKilled(nKilled);
		secureStorage.UpdateCrocodilesKilled(nKilled2);
		secureStorage.UpdateSnakesKilled(nKilled3);
		secureStorage.UpdateRopeSwingCount(timesUsed3);
		secureStorage.UpdateCheckpointsPassed(nPassed);
		secureStorage.UpdateStylishDismounts(amount);
		secureStorage.UpdateTimesTutorialPlayed(amount2);
		secureStorage.UpdateLifeTonicsUsed(timesUsed6);
		secureStorage.UpdateHasteTonicsUsed(timesUsed4, Score().WasteOfHasteCheckPassed());
		secureStorage.UpdateAntiVenomsUsed(timesUsed5);
		secureStorage.UpdateScore(mScore.TotalScoreXPAdjusted());
		secureStorage.UpdateHighestScore(mScore.TotalScoreXPAdjusted());
		secureStorage.UpdateXP(mScore.XPEarned());
		secureStorage.UpdateDistanceInMineCart(distance);
		secureStorage.UpdateDistanceOnBike(distance2);
		secureStorage.IncDeathCount(LastKillType());
		SecureStorage.Instance.LastTileTheme = mCachedLastTheme;
		mCachedLastTheme = WorldConstructionHelper.Theme.Menu;
		GetPlayerAnimationController().PlayerFXController.StopHasteEffect();
		GetPlayerAnimationController().PlayerFXController.StopSpiritEffect();
		SFX.KillSoundLoops();
		if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Pilot))
		{
			uint ootdAviatorAwardTime = SecureStorage.Instance.OotdAviatorAwardTime;
			uint secondsSinceUnixEpoch = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
			uint secondsSince = TimeUtils.GetSecondsSince(ootdAviatorAwardTime, secondsSinceUnixEpoch);
			if (secondsSince >= TimeUtils.SecondsPerDay)
			{
				SecureStorage.Instance.AwardFreeMacaws(3);
				SecureStorage.Instance.OotdAviatorAwardTime = secondsSinceUnixEpoch;
			}
		}
	}

	public void DrawSpeedGraph()
	{
		mSpeedIncrease.DrawSpeedGraph();
	}

	private bool IsInAction()
	{
		return mSwipeUpAction > 0f || mSwipeDownAction > 0f || mPressAction > 0f;
	}

	private void UpdateSwipeBuffer()
	{
		if (!IsInAction() && (mSwipeBuffer == SwipeBuffer.Up || mSwipeBuffer == SwipeBuffer.Down))
		{
			Swipe(mSwipeBuffer);
		}
	}

	private void UpdateSwipeBufferForPress()
	{
		if (!IsInAction() && mSwipeBuffer == SwipeBuffer.LongUp)
		{
			Swipe(mSwipeBuffer);
		}
	}

	private void UpdateCooldownTimers()
	{
		if (mSwipeUpAction > 0f)
		{
			mSwipeUpAction -= Time.deltaTime;
		}
		if (mPressAction > 0f)
		{
			mPressAction -= Time.deltaTime;
		}
		if (mSwipeDownAction > 0f)
		{
			mSwipeDownAction -= Time.deltaTime;
			bool flag = false;
			Tile tileUnderPlayer = LevelGenerator.Instance().GetTileUnderPlayer();
			if (tileUnderPlayer != null)
			{
				flag = tileUnderPlayer.IsOfType(Tile.ResponseType.SwipeDown);
			}
			if (flag)
			{
				mBeenOnADuckTile = true;
			}
			if (mSwipeDownAction <= 0f)
			{
				if (flag)
				{
					Swipe(SwipeBuffer.Down);
				}
			}
			else if (!flag && mBeenOnADuckTile)
			{
				mSwipeDownAction = 0f;
			}
		}
		else
		{
			mBeenOnADuckTile = false;
		}
	}

	public void Swipe(SwipeBuffer direction)
	{
		switch (direction)
		{
		case SwipeBuffer.Up:
			if ((IsNearJumpPiece() && CurrentTheme().AllowPitJump) || CurrentTheme().AllowNormalJump)
			{
				mSwipeUpAction = Settings.ActionWarmDownTimeSwipeUp;
				mQueueAnim = true;
				mSwipeBuffer = SwipeBuffer.None;
			}
			break;
		case SwipeBuffer.Down:
			mSwipeDownAction = Settings.ActionWarmDownTimeSwipeDown;
			mQueueAnim = true;
			mSwipeBuffer = SwipeBuffer.None;
			break;
		case SwipeBuffer.LongUp:
			mPressAction = Settings.ActionWarmDownTimeLongUp;
			mQueueAnim = true;
			mSwipeBuffer = SwipeBuffer.None;
			break;
		case SwipeBuffer.Left:
		case SwipeBuffer.Right:
			break;
		}
	}

	public void Buffer(SwipeBuffer direction)
	{
		if (direction != SwipeBuffer.None)
		{
			mSwipeBuffer = direction;
		}
	}

	private bool IsFlickering()
	{
		if (FlickerJaguar() || IsImmuneFromDamage())
		{
			return true;
		}
		return false;
	}

	private void DrawDebug()
	{
		if (DisplaySpeed)
		{
			DebugSpam.Output("Current Speed " + GetCurrentSpeed().ToString() + " (Max " + mMaxSpeed + ")");
			LevelGenerator levelGenerator = LevelGenerator.Instance();
			if (levelGenerator != null)
			{
				PieceDescriptor currentPiece = levelGenerator.GetCurrentPiece();
				DebugSpam.OutputAuthored(currentPiece != null && currentPiece.name.Contains("SetPiece"));
			}
		}
		float r = 0f;
		float g = 0f;
		float b = 0f;
		float num = 0f;
		Vector3 localScale = DebugBarSwipeUp.transform.localScale;
		if (mSwipeUpAction > 0f)
		{
			r = mSwipeUpAction;
			DebugBarSwipeUp.GetComponent<Renderer>().material.color = Color.red;
			localScale.y = mSwipeUpAction * mDebugBarScaleY;
		}
		else
		{
			Material material = DebugBarSwipeUp.GetComponent<Renderer>().material;
			Color black = Color.black;
			DebugBufferSwipeUp.GetComponent<Renderer>().material.color = black;
			material.color = black;
			localScale.y = mDebugBarScaleY;
		}
		DebugBarSwipeUp.transform.localScale = localScale;
		localScale = DebugBarSwipeDown.transform.localScale;
		if (mSwipeDownAction > 0f)
		{
			b = mSwipeDownAction;
			Material material2 = DebugBarSwipeDown.GetComponent<Renderer>().material;
			Color black = Color.blue;
			DebugBufferSwipeDown.GetComponent<Renderer>().material.color = black;
			material2.color = black;
			localScale.y = mSwipeDownAction * mDebugBarScaleY;
		}
		else
		{
			Material material3 = DebugBarSwipeDown.GetComponent<Renderer>().material;
			Color black = Color.black;
			DebugBufferSwipeDown.GetComponent<Renderer>().material.color = black;
			material3.color = black;
			localScale.y = mDebugBarScaleY;
		}
		DebugBarSwipeDown.transform.localScale = localScale;
		localScale = DebugBarLongUp.transform.localScale;
		if (mPressAction > 0f)
		{
			num = mPressAction;
			Material material4 = DebugBarLongUp.GetComponent<Renderer>().material;
			Color black = Color.yellow;
			DebugBufferLongUp.GetComponent<Renderer>().material.color = black;
			material4.color = black;
			localScale.y = mPressAction * mDebugBarScaleY;
		}
		else
		{
			Material material5 = DebugBarLongUp.GetComponent<Renderer>().material;
			Color black = Color.black;
			DebugBufferLongUp.GetComponent<Renderer>().material.color = black;
			material5.color = black;
			localScale.y = mDebugBarScaleY;
		}
		DebugBarLongUp.transform.localScale = localScale;
		if (IsInTurnBufferZoneLeft() || IsInTurnBufferZoneLeftRight())
		{
			DebugBarSwipeLeft.GetComponent<Renderer>().material.color = Color.yellow;
		}
		else
		{
			DebugBarSwipeLeft.GetComponent<Renderer>().material.color = Color.black;
		}
		if (IsInTurnBufferZoneRight() || IsInTurnBufferZoneLeftRight())
		{
			DebugBarSwipeRight.GetComponent<Renderer>().material.color = Color.yellow;
		}
		else
		{
			DebugBarSwipeRight.GetComponent<Renderer>().material.color = Color.black;
		}
		DebugBufferLongUp.GetComponent<Renderer>().material.color = Color.black;
		DebugBufferSwipeUp.GetComponent<Renderer>().material.color = Color.black;
		DebugBufferSwipeDown.GetComponent<Renderer>().material.color = Color.black;
		DebugBufferSwipeLeft.GetComponent<Renderer>().material.color = Color.black;
		DebugBufferSwipeRight.GetComponent<Renderer>().material.color = Color.black;
		switch (mSwipeBuffer)
		{
		case SwipeBuffer.LongUp:
			DebugBufferLongUp.GetComponent<Renderer>().material.color = Color.yellow;
			break;
		case SwipeBuffer.Up:
			DebugBufferSwipeUp.GetComponent<Renderer>().material.color = Color.red;
			break;
		case SwipeBuffer.Down:
			DebugBufferSwipeDown.GetComponent<Renderer>().material.color = Color.blue;
			break;
		case SwipeBuffer.Left:
			DebugBufferSwipeLeft.GetComponent<Renderer>().material.color = Color.green;
			break;
		case SwipeBuffer.Right:
			DebugBufferSwipeRight.GetComponent<Renderer>().material.color = Color.green;
			break;
		}
		base.GetComponent<Renderer>().material.color = new Color(r, g, b, 1f);
	}

	public void SwitchToVehicleCam()
	{
		if (GetThemeType() == PlayerTheme.ThemeType.Bike)
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.BikeCam);
		}
		else if (GetThemeType() == PlayerTheme.ThemeType.Cart)
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.MineCartCam);
		}
		else if (GetThemeType() == PlayerTheme.ThemeType.Jaguar && LevelGenerator.Instance().GetCurrentPiece().Theme != WorldConstructionHelper.Theme.Mountain)
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.JaguarCam);
		}
	}

	public void SwitchToBikeCineCam1()
	{
		CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.BikeCineCam1, CameraTransitionData.JumpCut);
	}
}
