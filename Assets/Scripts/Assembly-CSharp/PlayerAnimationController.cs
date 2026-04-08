using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : FSM
{
	private class State
	{
		public const int Idle = 1;

		public const int WalkRunCycle = 2;

		public const int Jump = 3;

		public const int Slide = 4;

		public const int Dead = 5;

		public const int CatRide = 6;

		public const int RopeJump = 7;

		public const int RopeSomersault = 8;

		public const int Ressurection = 9;

		public const int MacawDrop = 10;

		public const int SideStepLeft = 11;

		public const int SideStepRight = 12;

		public const int Cutscene = 13;

		public const int Turn = 14;

		public const int PreTurn = 15;

		public const int VehicleStart = 16;

		public const int VehicleStop = 17;

		public const int JumpFromVehicle = 18;

		public const int TrialsRunStart = 19;

		public const int TrialsRunEnd = 20;
	}

	private enum SlideState
	{
		In = 0,
		Loop = 1,
		Out = 2
	}

	private const float mSlideFadeTime = 0.1f;

	private const float SideStepBlendOutSpeed = 0.4f;

	private const float TurnBlendOutTime = 0.2f;

	public PlayerSFX SFX;

	public PlayerController PlayerController;

	public Animation PlayerAnimation;

	public AnimationLoader AnimationLoader;

	public PlayerFXController PlayerFXController;

	public GameObject PlayerShadow;

	private Vector3 mShadowMaxScale = new Vector3(20f, 20f, 20f);

	private bool mCutSceneEnd;

	private bool mPartialsSetUp;

	private PlayerTheme.ThemeType mAnimThemeType;

	private int mNumJumps;

	private float mTimeOfLastJump;

	private float mBikeEndSoundDelay = 1.3f;

	private ParticleSystem mMineCartLeftSparkFX;

	private ParticleSystem mMineCartRightSparkFX;

	private string idleName;

	private float mSlideTime;

	private int slideVariationIndex;

	private string slideAnimName;

	private string pairedSlideAnimName;

	private SlideState mSlideState;

	private float mFadeTimeFromJumpToWalkRun = 0.033f;

	private float mJumpTimeFeetOnFloor = 0.6f;

	private float mSyncTimeFromJumpToWalkRun = 0.75f;

	private string jumpName;

	private string pairedJumpName;

	private float mJumpTime;

	private bool mDeathFalling;

	private float mFallSpeed;

	private Vector3 mEntryLocalPosition;

	private float mLeanAmount;

	private float mLeanLast;

	private AnimationGroup.AnimType mCurrentRunType;

	private string mRunName;

	private string mPairedRunName;

	private string mLeanLeftName;

	private string mLeanRightName;

	private string mPairedLeanLeftName;

	private string mPairedLeanRightName;

	private float mPoisonAnimTimer;

	private float mDrinkPotionAnimTimer;

	private List<PlayerController.Potion_Type> mDrinks = new List<PlayerController.Potion_Type>();

	private PlayerController.Potion_Type mCurrentDrink;

	private bool mDrinkingPotion;

	private float mSyncTimeFromRopeJumpToWalkRun = 0.75f;

	private float mSyncTimeToSomersault = 0.51f;

	private float mAnimationSpeed = 0.45f;

	private string swingName;

	private bool mDidStylishDismout;

	private float mSyncTimeFromRopeSomersaultToWalkRun = 0.4666f;

	private string mSideStepLeftName;

	private string mPairedSideStepLeftName;

	private string mSideStepRightName;

	private string mPairedSideStepRightName;

	private string mTurnName;

	private string mPairedTurnName;

	private LevelGenerator.TurnType mLastTurnType;

	private string mPreTurnName;

	private string mPairedPreTurnName;

	private int mTurnIndex;

	private string cutsceneName;

	private string pairedCutsceneName;

	private string vehicleStartName;

	private string vehicleStopName;

	private string pairedVehicleStopName;

	private string stopSpareName;

	private string jumpFromVehicleName;

	public float GetJumpLength()
	{
		string text = null;
		if (AnimationLoader != null)
		{
			text = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Jump);
		}
		if (PlayerAnimation != null && AnimationLoader != null && text != null)
		{
			return PlayerAnimation[text].length;
		}
		return 0f;
	}

	public void AudioEvent_LeftFoot()
	{
		if (CurrentState() == 2)
		{
			SFX.PlayLeftFoot();
			PlayerFXController.CreateFootstep();
		}
	}

	public void AudioEvent_RightFoot()
	{
		if (CurrentState() == 2)
		{
			SFX.PlayRightFoot();
			PlayerFXController.CreateFootstep();
		}
	}

	public bool IsJumping()
	{
		return CurrentState() == 3;
	}

	public bool IsSliding()
	{
		return CurrentState() == 4;
	}

	public bool IsInRopeSwing()
	{
		return CurrentState() == 7 || CurrentState() == 8;
	}

	public bool IsCelebratingTrial()
	{
		return CurrentState() == 20;
	}

	private void Start()
	{
		mPartialsSetUp = false;
		StopAllAnims();
		mStateTable.Add(1, new StateFunctions(IdleAnimation_Enter, IdleAnimation_Update, IdleAnimation_Exit, "Idle"));
		mStateTable.Add(2, new StateFunctions(WalkRunAnimation_Enter, WalkRunAnimation_Update, WalkRunAnimation_Exit, "Walkrun"));
		mStateTable.Add(3, new StateFunctions(JumpAnimation_Enter, JumpAnimation_Update, JumpAnimation_Exit, "Jump"));
		mStateTable.Add(4, new StateFunctions(SlideAnimation_Enter, SlideAnimation_Update, SlideAnimation_Exit, "Slide"));
		mStateTable.Add(5, new StateFunctions(DeadAnimation_Enter, DeadAnimation_Update, DeadAnimation_Exit, "Dead"));
		mStateTable.Add(7, new StateFunctions(RopeAnimation_Enter, RopeAnimation_Update, RopeAnimation_Exit, "RopeJump"));
		mStateTable.Add(8, new StateFunctions(RopeSomersaultAnimation_Enter, RopeSomersaultAnimation_Update, RopeSomersaultAnimation_Exit, "RopeSomersault"));
		mStateTable.Add(9, new StateFunctions(RessurectionAnimation_Enter, RessurectionAnimation_Update, RessurectionAnimation_Exit, "Ressurection"));
		mStateTable.Add(10, new StateFunctions(MacawDropAnimation_Enter, MacawDropAnimation_Update, MacawDropAnimation_Exit, "MacawDrop"));
		mStateTable.Add(11, new StateFunctions(SideStepLeftAnimation_Enter, SideStepLeftAnimation_Update, SideStepLeftAnimation_Exit, "SideStep"));
		mStateTable.Add(12, new StateFunctions(SideStepRightAnimation_Enter, SideStepRightAnimation_Update, SideStepRightAnimation_Exit, "SideStep"));
		mStateTable.Add(13, new StateFunctions(CutsceneAnimation_Enter, CutsceneAnimation_Update, CutsceneAnimation_Exit, "Cutscene"));
		mStateTable.Add(14, new StateFunctions(TurnAnimation_Enter, TurnAnimation_Update, TurnAnimation_Exit, "Turn"));
		mStateTable.Add(15, new StateFunctions(PreTurnAnimation_Enter, PreTurnAnimation_Update, PreTurnAnimation_Exit, "PreTurn"));
		mStateTable.Add(16, new StateFunctions(VehicleStartAnimation_Enter, VehicleStartAnimation_Update, VehicleStartAnimation_Exit, "VehicleStart"));
		mStateTable.Add(17, new StateFunctions(VehicleStopAnimation_Enter, VehicleStopAnimation_Update, VehicleStopAnimation_Exit, "VehicleStop"));
		mStateTable.Add(18, new StateFunctions(JumpFromVehicleAnimation_Enter, JumpFromVehicleAnimation_Update, JumpFromVehicleAnimation_Exit, "JumpFromVehicle"));
		mStateTable.Add(19, new StateFunctions(TrialsRunStart_Enter, TrialsRunStart_Update, TrialsRunStart_Exit, "TrialsRunStart"));
		mStateTable.Add(20, new StateFunctions(TrialsRunEnd_Enter, TrialsRunEnd_Update, TrialsRunEnd_Enter, "TrialsRunEnd"));
		SwitchState(1);
		mAnimThemeType = PlayerController.GetThemeType();
		SetUpParticleFX();
	}

	private void StopAllAnims()
	{
		PlayerAnimation.Stop();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Stop();
		}
		mNumJumps = 0;
		mTimeOfLastJump = 0f;
	}

	private void StopAllFX()
	{
		mMineCartLeftSparkFX.Stop();
		mMineCartRightSparkFX.Stop();
		PlayerFXController.StopSlideEffect();
	}

	private void SetUpParticleFX()
	{
		mMineCartLeftSparkFX = PlayerController.MineCartModel.transform.Find("FX_Cart_Sparks_Left_Prefab").GetComponent<ParticleSystem>();
		mMineCartRightSparkFX = PlayerController.MineCartModel.transform.Find("FX_Cart_Sparks_Right_Prefab").GetComponent<ParticleSystem>();
	}

	private void SetupPartialAnimations()
	{
		if (!mPartialsSetUp)
		{
			Transform transform = PlayerHelper.SearchHierarchyForBone(PlayerController.PlayerModel.transform, "Bip001 Spine1");
			string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Whip);
			if ((bool)transform && (bool)PlayerAnimation[animName])
			{
				TBFUtils.DebugLog("Added mixing transform whip");
				PlayerAnimation[animName].AddMixingTransform(transform, true);
			}
			mPartialsSetUp = true;
		}
	}

	protected override string FSMName()
	{
		return "PlayerAnimationController";
	}

	public override void Update()
	{
		SetupPartialAnimations();
		if (PlayerController.CallOfTheMacawActive())
		{
			SwitchState(10);
		}
		if (PlayerController.IsDead() && CurrentState() != 9 && CurrentState() != 5)
		{
			SwitchState(5);
		}
		if (!PlayerController.IsDead() && !PlayerController.IsRunning() && CurrentState() != 9)
		{
			if (PlayerController.IsWaitingForTrialsCountdown())
			{
				SwitchState(19);
			}
			else if (PlayerController.HasFinishedTrial())
			{
				SwitchState(20);
			}
			else
			{
				SwitchState(1);
			}
		}
		base.Update();
		UpdateWhip();
		UpdateMacaw();
		UpdateSFX();
		UpdateThemeChange();
	}

	private void UpdateThemeChange()
	{
		if (PlayerController.NextTheme() == PlayerTheme.ThemeType.None || CurrentState() == 13 || PlayerController.NextTheme() == mAnimThemeType || CurrentState() == 17 || CurrentState() == 18)
		{
			return;
		}
		if (PlayerController.CurrentTheme().HasTransition)
		{
			if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Bike)
			{
				SFX.BikeRideEnd(mBikeEndSoundDelay);
			}
			else if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Cart)
			{
				SFX.CartEnd();
			}
			mCutSceneEnd = true;
			SwitchState(17);
			return;
		}
		if (PlayerController.NextTheme() == PlayerTheme.ThemeType.Bike)
		{
			SFX.BikeRideStart();
		}
		else if (PlayerController.NextTheme() == PlayerTheme.ThemeType.Cart)
		{
			SFX.CartStart();
		}
		PlayerController.SwitchPlayerTheme();
		mAnimThemeType = PlayerController.GetThemeType();
		if (PlayerController.CurrentTheme().HasTransition)
		{
			mCutSceneEnd = false;
			if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Bike || PlayerController.GetThemeType() == PlayerTheme.ThemeType.Cart)
			{
				SwitchState(16);
			}
			else
			{
				SwitchState(17);
			}
		}
		else
		{
			SwitchState(1);
		}
	}

	private void UpdateSFX()
	{
	}

	private void UpdateMacaw()
	{
		if (!PlayerController.BirdModel.GetComponent<Animation>().IsPlaying("DropOff"))
		{
			PlayerController.BirdModel.SetActiveRecursively(false);
		}
	}

	public void IdleAnimation_Enter(int fromState)
	{
		if (fromState == 5)
		{
			return;
		}
		idleName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Idle);
		PlayerAnimation.CrossFade(idleName);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			string animName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.Idle);
			if (animName != null)
			{
				gameObject.GetComponent<Animation>().CrossFade(animName);
			}
		}
	}

	public void IdleAnimation_Update()
	{
		if (PlayerController.GetCurrentSpeed() > 0f || PlayerController.GetThemeType() == PlayerTheme.ThemeType.Bike)
		{
			SwitchState(2);
		}
	}

	public void IdleAnimation_Exit(int toState)
	{
	}

	public void SlideAnimation_Enter(int fromState)
	{
		mSlideState = SlideState.In;
		if (fromState == 18)
		{
			slideAnimName = "SlideRoll";
		}
		else
		{
			slideAnimName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SlideEnter, out slideVariationIndex);
		}
		bool isRolling = slideAnimName == "SlideRoll";
		mSlideTime = 0f;
		switch (mAnimThemeType)
		{
		case PlayerTheme.ThemeType.Bike:
			SFX.BikeRideSlide();
			break;
		default:
			SFX.PlaySlide(isRolling);
			break;
		case PlayerTheme.ThemeType.Cart:
		case PlayerTheme.ThemeType.Jaguar:
			break;
		}
		PlayerAnimation.CrossFade(slideAnimName, 0.1f, PlayMode.StopAll);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			pairedSlideAnimName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.SlideEnter);
			if (pairedSlideAnimName != null)
			{
				gameObject.GetComponent<Animation>().CrossFade(pairedSlideAnimName, 0.1f, PlayMode.StopAll);
			}
		}
	}

	public void SlideAnimation_Update()
	{
		switch (mSlideState)
		{
		case SlideState.In:
		{
			if (slideAnimName == "LoopSlidev2")
			{
				PlayerFXController.StartSlideEffect(PlayerFXController.SlideDirection.eFaceFirst);
			}
			else
			{
				PlayerFXController.StartSlideEffect(PlayerFXController.SlideDirection.eLegsFirst);
			}
			AnimationState animationState2 = PlayerAnimation[slideAnimName];
			if ((bool)animationState2 && animationState2.time > animationState2.length - 0.1f)
			{
				mSlideState = SlideState.Loop;
				slideAnimName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Slide, slideVariationIndex);
				PlayerAnimation.CrossFade(slideAnimName, 0.1f);
			}
			break;
		}
		case SlideState.Loop:
		{
			mSlideTime += Time.deltaTime;
			if (PlayerController.IsSwipingDown() || !(mSlideTime > PlayerController.GetMinimumSlideTime()))
			{
				break;
			}
			mSlideState = SlideState.Out;
			slideAnimName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SlideExit, slideVariationIndex);
			PlayerAnimation.CrossFade(slideAnimName, 0.1f);
			GameObject gameObject = PlayerController.Instance().PairedModel();
			if (gameObject != null)
			{
				pairedSlideAnimName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.SlideExit);
				if (pairedSlideAnimName != null)
				{
					gameObject.GetComponent<Animation>().CrossFade(pairedSlideAnimName, 0.1f);
				}
			}
			break;
		}
		case SlideState.Out:
		{
			AnimationState animationState = PlayerAnimation[slideAnimName];
			if ((bool)animationState && animationState.time > animationState.length - 0.1f)
			{
				SwitchState(2);
			}
			break;
		}
		}
		if (CheckForDuckOrJump())
		{
			SlideAnimation_Enter(CurrentState());
		}
	}

	public void SlideAnimation_Exit(int toState)
	{
		PlayerAnimation.Blend(slideAnimName, 0f, 0.1f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null && pairedSlideAnimName != null)
		{
			gameObject.GetComponent<Animation>().Blend(pairedSlideAnimName, 0f, 0.1f);
		}
		PlayerFXController.StopSlideEffect();
	}

	public void JumpAnimation_Enter(int fromState)
	{
		mJumpTime = 0f;
		if (Time.realtimeSinceStartup - mTimeOfLastJump > 1.5f)
		{
			mNumJumps = 0;
		}
		AnimationGroup.AnimType type;
		if (PlayerController.CurrentTheme().AllowPitJump && (PlayerController.IsNearJumpPiece() || !PlayerController.CurrentTheme().AllowNormalJump))
		{
			mNumJumps = 0;
			type = AnimationGroup.AnimType.JumpPit;
		}
		else
		{
			if (PlayerController.IsNearRopePiece() || !PlayerController.CurrentTheme().AllowExtraJump)
			{
				mNumJumps = 0;
			}
			mTimeOfLastJump = Time.realtimeSinceStartup;
			mNumJumps++;
			if (mNumJumps < 3)
			{
				type = AnimationGroup.AnimType.Jump;
			}
			else
			{
				type = AnimationGroup.AnimType.ExtraJump;
				mNumJumps = 0;
			}
		}
		jumpName = AnimationLoader.GetAnimName(type);
		SFX.PlayJump();
		PlayerAnimation[jumpName].time = 0f;
		PlayerAnimation[jumpName].speed = PlayerAnimation[jumpName].length / PlayerController.GetJumpTime();
		if (fromState == 4 || fromState == 11 || fromState == 12)
		{
			PlayerAnimation.CrossFade(jumpName, 0.2f);
		}
		else
		{
			PlayerAnimation.Play(jumpName);
		}
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (!(gameObject != null))
		{
			return;
		}
		pairedJumpName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type);
		if (pairedJumpName != null)
		{
			gameObject.GetComponent<Animation>()[pairedJumpName].time = PlayerAnimation[jumpName].time;
			gameObject.GetComponent<Animation>()[pairedJumpName].speed = PlayerAnimation[jumpName].speed;
			if (fromState == 4)
			{
				gameObject.GetComponent<Animation>().CrossFade(pairedJumpName, 0.2f);
			}
			else
			{
				gameObject.GetComponent<Animation>().Play(pairedJumpName);
			}
		}
	}

	public void JumpAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[jumpName];
		if (PlayerAnimation[jumpName].time > PlayerAnimation[jumpName].length - 0.1f || !PlayerAnimation.IsPlaying(jumpName))
		{
			SFX.PlayLanding();
			SwitchState(2);
		}
		else if (PlayerController.IsOnRopePiece() && PlayerController.HasValidRopeNode() && (bool)animationState && animationState.time > 0.08f)
		{
			SwitchState(7);
		}
		if (!PlayerAnimation.IsPlaying(jumpName))
		{
			return;
		}
		if (PlayerController.IsOnPitPiece() || PlayerController.IsOnRopePiece() || PlayerController.IsOnJumpPiece())
		{
			PlayerShadow.SetActiveRecursively(false);
			return;
		}
		float num = PlayerAnimation.GetClip(jumpName).length * 0.5f;
		mJumpTime += Time.deltaTime;
		if (mJumpTime < num)
		{
			PlayerShadow.transform.localScale *= 0.9f;
		}
		else
		{
			PlayerShadow.transform.localScale *= 1.1f;
		}
	}

	public void JumpAnimation_Exit(int toState)
	{
		PlayerShadow.SetActiveRecursively(true);
		PlayerShadow.transform.localScale = mShadowMaxScale;
		PlayerAnimation.Blend(jumpName, 0f, 0.1f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null && pairedJumpName != null)
		{
			gameObject.GetComponent<Animation>().Blend(pairedJumpName, 0f, 0.1f);
		}
	}

	private bool AllAnimationsFinished()
	{
		foreach (AnimationState item in PlayerAnimation)
		{
			if (item.enabled && item.time < item.length)
			{
				return false;
			}
		}
		return true;
	}

	public void DeadAnimation_Enter(int fromState)
	{
		mDeathFalling = false;
		mFallSpeed = 0.2f;
		mEntryLocalPosition = base.transform.localPosition;
		PieceDescriptor.KillType killType = PlayerController.LastKillType();
		SFX.PlayDeath(killType);
		StopAllFX();
		GameObject gameObject = PlayerController.PairedModel();
		AnimationGroup.AnimType type;
		float fadeLength;
		switch (killType)
		{
		case PieceDescriptor.KillType.Pit:
			type = AnimationGroup.AnimType.JumpFall;
			fadeLength = 0.05f;
			mDeathFalling = true;
			PlayerShadow.SetActiveRecursively(false);
			break;
		case PieceDescriptor.KillType.PitShallow:
			type = AnimationGroup.AnimType.PitShallowHit;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.FallEdgeDie_R:
			type = ((!(PlayerController.GetLeftRight() < 0.3f)) ? AnimationGroup.AnimType.FallEdgeDie_Right : AnimationGroup.AnimType.FallEdgeDie_FrontRight);
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.FallEdgeDie_L:
			type = ((!(PlayerController.GetLeftRight() > -0.3f)) ? AnimationGroup.AnimType.FallEdgeDie_Left : AnimationGroup.AnimType.FallEdgeDie_FrontLeft);
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.DeathUndergrowth_L:
			type = AnimationGroup.AnimType.DeathUndergrowth_Left;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.DeathImpaleDoor:
			type = AnimationGroup.AnimType.DeathImpaleDoor;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.DeathUndergrowth_R:
			type = AnimationGroup.AnimType.DeathUndergrowth_Right;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.FallCliffCornerDie:
			type = AnimationGroup.AnimType.FallCliffCornerDie;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.Crocodile:
		{
			type = AnimationGroup.AnimType.EatenByCrocodile;
			fadeLength = 0.1f;
			Baddie killBaddie = BaddieController.Instance().KillBaddie;
			if (killBaddie != null)
			{
				GameObject model = killBaddie.GetModel();
				Transform transform = model.transform;
				string attackAnimation = killBaddie.GetAttackAnimation();
				model.GetComponent<Animation>().CrossFade(attackAnimation, fadeLength);
				base.transform.parent.position = transform.position;
				base.transform.parent.rotation = transform.rotation;
				type = ((!attackAnimation.Contains("Right")) ? AnimationGroup.AnimType.EatenByCrocodile : AnimationGroup.AnimType.EatenByCrocodile_Right);
			}
			break;
		}
		case PieceDescriptor.KillType.Poison:
			type = AnimationGroup.AnimType.PoisonDeath;
			fadeLength = 0.1f;
			break;
		case PieceDescriptor.KillType.SlideWall:
			type = ((fromState != 3) ? AnimationGroup.AnimType.SlideRunHit : AnimationGroup.AnimType.SlideHitWall);
			fadeLength = 0.1f;
			break;
		default:
			if (fromState == 4)
			{
				type = AnimationGroup.AnimType.SlideHitWall;
				fadeLength = 0.1f;
			}
			else
			{
				type = AnimationGroup.AnimType.HitWall;
				fadeLength = 0.1f;
			}
			break;
		}
		string animName = AnimationLoader.GetAnimName(type);
		PlayerAnimation.CrossFade(animName, fadeLength);
		if (gameObject != null)
		{
			string animName2 = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type);
			if (animName2 != null)
			{
				gameObject.GetComponent<Animation>().CrossFade(animName2, fadeLength);
			}
			else
			{
				gameObject.GetComponent<Animation>().Stop();
			}
		}
	}

	public void DeadAnimation_Update()
	{
		if (mDeathFalling)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y - Time.timeScale * mFallSpeed, base.transform.localPosition.z);
		}
		if (PlayerController.SpiritOfTheJungle().Active() || GameTutorial.Instance.IsEnabled || PlayerController.IsOutfitResurrectionActive() || GameController.Instance.RestoreHarry)
		{
			if (AllAnimationsFinished() || base.transform.localPosition.y > 20f)
			{
				GameController.Instance.SetupResurrectionCamera();
				SwitchState(9);
			}
		}
		else if (!PlayerController.IsDead())
		{
			SwitchState(1);
		}
	}

	public void DeadAnimation_Exit(int toState)
	{
		StopAllAnims();
		if (toState == 1)
		{
			if (PlayerController.NextTheme() == PlayerTheme.ThemeType.Run)
			{
				PlayerController.SwitchPlayerTheme();
			}
			mAnimThemeType = PlayerController.GetThemeType();
		}
		base.transform.localPosition = mEntryLocalPosition;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, 0f, base.transform.localPosition.z);
	}

	public void SetPoisoned()
	{
		mPoisonAnimTimer = 1f;
	}

	private AnimationGroup.AnimType GetRunType()
	{
		string empty = string.Empty;
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Whip);
		if (PlayerAnimation.IsPlaying(animName))
		{
			return AnimationGroup.AnimType.Run;
		}
		PlayerController.Potion_Type potion_Type = PlayerController.InitiatedDrink();
		if (potion_Type != PlayerController.Potion_Type.None)
		{
			mDrinks.Add(potion_Type);
		}
		if (!mDrinkingPotion && mDrinks.Count > 0)
		{
			mCurrentDrink = mDrinks[0];
			mDrinks.RemoveAt(0);
			mDrinkingPotion = true;
			mDrinkPotionAnimTimer = PlayerAnimation[AnimationLoader.GetAnimName(AnimationGroup.AnimType.RunDrinkSpeed)].length;
			if (mCurrentDrink == PlayerController.Potion_Type.AntiVenom)
			{
				PlayerController.ClearPoison();
			}
		}
		else
		{
			if (PlayerController.IsPoisoned() && mPoisonAnimTimer > 0f)
			{
				mPoisonAnimTimer = Mathf.Max(mPoisonAnimTimer - Time.deltaTime, 0f);
				return AnimationGroup.AnimType.RunPoisoned;
			}
			if (mDrinkPotionAnimTimer > 0f)
			{
				mDrinkPotionAnimTimer = Mathf.Max(mDrinkPotionAnimTimer - Time.deltaTime, 0f);
				switch (mCurrentDrink)
				{
				case PlayerController.Potion_Type.SpeedBoost:
					return AnimationGroup.AnimType.RunDrinkSpeed;
				case PlayerController.Potion_Type.Spirit:
					return AnimationGroup.AnimType.RunDrinkSpirit;
				default:
					return AnimationGroup.AnimType.RunDrinkMedicine;
				}
			}
			mDrinkingPotion = false;
		}
		return AnimationGroup.AnimType.Run;
	}

	public void WalkRunAnimation_Enter(int fromState)
	{
		PlayerController.ClearHasTurned();
		mCurrentRunType = GetRunType();
		mRunName = AnimationLoader.GetAnimName(mCurrentRunType);
		mLeanLeftName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.LeanLeft);
		mLeanRightName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.LeanRight);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			mPairedRunName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.Run);
			mPairedLeanLeftName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.LeanLeft);
			mPairedLeanRightName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.LeanRight);
		}
		mLeanAmount = 0f;
		mLeanLast = PlayerController.GetLeftRight();
		mTurnIndex = -1;
		switch (fromState)
		{
		case 14:
			if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Cart)
			{
				PlayerAnimation.CrossFade(mRunName, 0.2f);
				if (gameObject != null)
				{
					gameObject.GetComponent<Animation>().CrossFade(mPairedRunName, 0.2f);
				}
				return;
			}
			if (mLastTurnType == LevelGenerator.TurnType.Left)
			{
				PlayerAnimation[mRunName].time = 0.08f;
			}
			else
			{
				PlayerAnimation[mRunName].time = 0.39f;
			}
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
			}
			return;
		case 4:
			PlayerAnimation.CrossFade(mRunName, 0.2f);
			return;
		}
		if (!PlayerAnimation.IsPlaying(mRunName))
		{
			PlayerAnimation.Play(mRunName);
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>().Play(mPairedRunName);
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
			}
		}
	}

	private void UpdateLeaning_Cart()
	{
		float targetWeight = 0f;
		float targetWeight2 = 0f;
		if (PlayerController.GetLeftRight() > 0.5f)
		{
			targetWeight2 = (PlayerController.GetLeftRight() - 0.5f) * 2f;
			mMineCartLeftSparkFX.Stop();
			SFX.CartSparks(true);
		}
		else if (PlayerController.GetLeftRight() < -0.5f)
		{
			targetWeight = (0f - PlayerController.GetLeftRight() - 0.5f) * 2f;
			mMineCartRightSparkFX.Stop();
			SFX.CartSparks(true);
		}
		else
		{
			mMineCartLeftSparkFX.Play();
			mMineCartRightSparkFX.Play();
			SFX.CartSparks(false);
		}
		PlayerAnimation.Blend(mLeanLeftName, targetWeight, 0.1f);
		PlayerAnimation.Blend(mLeanRightName, targetWeight2, 0.1f);
		PlayerController.MineCartModel.GetComponent<Animation>().Blend(mPairedLeanLeftName, targetWeight, 0.1f);
		PlayerController.MineCartModel.GetComponent<Animation>().Blend(mPairedLeanRightName, targetWeight2, 0.1f);
		PlayerController.MineCartModel.GetComponent<Animation>()[mPairedLeanLeftName].enabled = true;
		PlayerController.MineCartModel.GetComponent<Animation>()[mPairedLeanRightName].enabled = true;
	}

	private void UpdateLeaning_Normal(float runWeight)
	{
		if (PlayerController.IsPoisoned() && mPoisonAnimTimer > 0f)
		{
			return;
		}
		float to = 8f * (PlayerController.GetLeftRight() - mLeanLast);
		mLeanAmount = Mathf.Lerp(mLeanAmount, to, 0.1f);
		mLeanLast = PlayerController.GetLeftRight();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Whip);
		if (mLeanAmount >= 0f)
		{
			if (!PlayerAnimation.IsPlaying(animName))
			{
				PlayerAnimation[mLeanRightName].weight = runWeight * mLeanAmount;
			}
			else
			{
				PlayerAnimation[mLeanRightName].weight = 0f;
			}
			PlayerAnimation[mLeanRightName].enabled = true;
			PlayerAnimation[mLeanRightName].time = PlayerAnimation[mRunName].time;
			if (!PlayerAnimation.IsPlaying(animName))
			{
				PlayerAnimation[mRunName].weight = runWeight * (1f - mLeanAmount);
			}
			else
			{
				PlayerAnimation[mRunName].weight = 0.2f;
			}
			PlayerAnimation[mRunName].enabled = true;
			PlayerAnimation[mLeanLeftName].enabled = false;
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].weight = PlayerAnimation[mLeanRightName].weight;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].time = PlayerAnimation[mLeanRightName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].weight = PlayerAnimation[mRunName].weight;
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].enabled = false;
			}
		}
		else
		{
			if (!PlayerAnimation.IsPlaying(animName))
			{
				PlayerAnimation[mLeanRightName].weight = runWeight * (0f - mLeanAmount);
			}
			else
			{
				PlayerAnimation[mLeanRightName].weight = 0f;
			}
			PlayerAnimation[mLeanLeftName].enabled = true;
			PlayerAnimation[mLeanLeftName].time = PlayerAnimation[mRunName].time;
			if (!PlayerAnimation.IsPlaying(animName))
			{
				PlayerAnimation[mRunName].weight = runWeight * (1f + mLeanAmount);
			}
			else
			{
				PlayerAnimation[mRunName].weight = 0.2f;
			}
			PlayerAnimation[mRunName].enabled = true;
			PlayerAnimation[mLeanRightName].enabled = false;
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].weight = PlayerAnimation[mLeanLeftName].weight;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].time = PlayerAnimation[mLeanLeftName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].weight = PlayerAnimation[mRunName].weight;
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].enabled = false;
			}
		}
	}

	private void UpdateLeaning_Bike(float runWeight)
	{
		float to = 12f * (PlayerController.GetLeftRight() - mLeanLast);
		mLeanAmount = Mathf.Lerp(mLeanAmount, to, 0.1f);
		mLeanLast = PlayerController.GetLeftRight();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (mLeanAmount > 0f)
		{
			float num = runWeight * mLeanAmount;
			PlayerAnimation[mLeanRightName].weight = num;
			PlayerAnimation[mLeanRightName].enabled = true;
			PlayerAnimation[mLeanRightName].time = PlayerAnimation[mRunName].time;
			PlayerAnimation[mRunName].weight = 1f - num;
			PlayerAnimation[mRunName].enabled = true;
			PlayerAnimation[mLeanLeftName].enabled = false;
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].weight = PlayerAnimation[mLeanRightName].weight;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].time = PlayerAnimation[mLeanRightName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].weight = PlayerAnimation[mRunName].weight;
				gameObject.GetComponent<Animation>()[mPairedRunName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].enabled = false;
			}
		}
		else
		{
			float num2 = runWeight * (0f - mLeanAmount);
			PlayerAnimation[mLeanLeftName].weight = num2;
			PlayerAnimation[mLeanLeftName].enabled = true;
			PlayerAnimation[mLeanLeftName].time = PlayerAnimation[mRunName].time;
			PlayerAnimation[mRunName].weight = 1f - num2;
			PlayerAnimation[mRunName].enabled = true;
			PlayerAnimation[mLeanRightName].enabled = false;
			if (gameObject != null)
			{
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].weight = PlayerAnimation[mLeanLeftName].weight;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanLeftName].time = PlayerAnimation[mLeanLeftName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].weight = PlayerAnimation[mRunName].weight;
				gameObject.GetComponent<Animation>()[mPairedRunName].time = PlayerAnimation[mRunName].time;
				gameObject.GetComponent<Animation>()[mPairedRunName].enabled = true;
				gameObject.GetComponent<Animation>()[mPairedLeanRightName].enabled = false;
			}
		}
	}

	private bool HasPreTurnAnim()
	{
		return AnimationLoader.HasSpecificAnim(AnimationGroup.AnimType.PreTurnLeft);
	}

	public void WalkRunAnimation_Update()
	{
		float runWeight = 1f;
		if (mCurrentRunType != GetRunType())
		{
			mCurrentRunType = GetRunType();
			mRunName = AnimationLoader.GetAnimName(mCurrentRunType);
			PlayerAnimation.Play(mRunName);
		}
		if (mAnimThemeType == PlayerTheme.ThemeType.Cart)
		{
			UpdateLeaning_Cart();
		}
		else if (mAnimThemeType == PlayerTheme.ThemeType.Bike)
		{
			UpdateLeaning_Bike(runWeight);
		}
		else if (PlayerController.GetThemeType() != PlayerTheme.ThemeType.Jaguar)
		{
			UpdateLeaning_Normal(runWeight);
		}
		bool flag = false;
		bool flag2 = false;
		LevelGenerator.TurnType turnType = PlayerController.HasTurned();
		if (turnType != LevelGenerator.TurnType.None)
		{
			flag2 = true;
		}
		else if (PlayerController.InNoDodgeZone())
		{
			if (PlayerController.IsInTurnBufferZoneLeft() && PlayerController.IsTurningLeft())
			{
				flag = true;
				turnType = LevelGenerator.TurnType.Left;
			}
			if (PlayerController.IsInTurnBufferZoneRight() && PlayerController.IsTurningRight())
			{
				flag = true;
				turnType = LevelGenerator.TurnType.Right;
			}
		}
		if (flag && PlayerController.CurrentTheme().HasTurnAnimations && HasPreTurnAnim())
		{
			mLastTurnType = turnType;
			SwitchState(15);
			PlayerController.ClearSideStep();
		}
		else if (flag2 && turnType != LevelGenerator.TurnType.None && PlayerController.CurrentTheme().HasTurnAnimations)
		{
			mLastTurnType = turnType;
			SwitchState(14);
			PlayerController.ClearSideStep();
		}
		else if (PlayerController.IsSideStepLeft())
		{
			if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Run)
			{
				SwitchState(11);
			}
			PlayerController.ClearSideStep();
		}
		else if (PlayerController.IsSideStepRight())
		{
			if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Run)
			{
				SwitchState(12);
			}
			PlayerController.ClearSideStep();
		}
		else if (!CheckForDuckOrJump() && PlayerController.IsAttacking() && !GameController.Instance.Paused() && mAnimThemeType != PlayerTheme.ThemeType.Cart && mAnimThemeType != PlayerTheme.ThemeType.Bike && mAnimThemeType != PlayerTheme.ThemeType.Jaguar && mCurrentRunType != AnimationGroup.AnimType.RunDrinkMedicine && mCurrentRunType != AnimationGroup.AnimType.RunPoisoned)
		{
			StartWhip();
		}
	}

	private bool CheckForDuckOrJump()
	{
		bool flag = PlayerController.IsAnimQueued();
		if (PlayerController.IsSwipingDown() && !PlayerController.IsAttacking() && flag && PlayerController.GetThemeType() != PlayerTheme.ThemeType.Jaguar)
		{
			SwitchState(4);
			return true;
		}
		if (PlayerController.IsSwipingUp() && flag && PlayerController.GetThemeType() != PlayerTheme.ThemeType.Cart)
		{
			SwitchState(3);
			return true;
		}
		return false;
	}

	private void StartWhip()
	{
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Whip);
		if ((bool)PlayerAnimation[animName] && !PlayerAnimation[animName].enabled)
		{
			SFX.PlayWhip();
			PlayerAnimation[animName].weight = 0.8f;
			PlayerAnimation[animName].time = 0.06667f;
			PlayerAnimation.Play(animName);
			PlayerFXController.ShowWhipEffectInit();
		}
	}

	private void UpdateWhip()
	{
		if (mAnimThemeType == PlayerTheme.ThemeType.Bike || mAnimThemeType == PlayerTheme.ThemeType.Cart || mAnimThemeType == PlayerTheme.ThemeType.Jaguar)
		{
			PlayerController.SetWhipRenderMode(PlayerController.WhipRenderMode.Away);
			return;
		}
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.Whip);
		AnimationState animationState = PlayerAnimation[animName];
		if ((bool)animationState && animationState.time >= 0.0333f && animationState.time <= 0.2f)
		{
			PlayerController.SetWhipRenderMode(PlayerController.WhipRenderMode.Extended);
		}
		else
		{
			PlayerController.SetWhipRenderMode(PlayerController.WhipRenderMode.Hand);
		}
	}

	public void WalkRunAnimation_Exit(int toState)
	{
		if (mAnimThemeType == PlayerTheme.ThemeType.Cart && toState != 13)
		{
			if (PlayerController.MineCartModel.GetComponent<Animation>()[mPairedLeanLeftName] != null)
			{
				PlayerController.MineCartModel.GetComponent<Animation>().Blend(mPairedLeanLeftName, 0f, 0.1f);
			}
			if (PlayerController.MineCartModel.GetComponent<Animation>()[mPairedLeanRightName] != null)
			{
				PlayerController.MineCartModel.GetComponent<Animation>().Blend(mPairedLeanRightName, 0f, 0.1f);
			}
			SFX.CartSparks(false);
		}
	}

	public void ScreenFlash(float duration)
	{
		Fade fade = Object.FindObjectOfType(typeof(Fade)) as Fade;
		if ((bool)fade)
		{
			fade.SetScreenOverlayColor(new Color(1f, 1f, 1f, 1f));
			fade.StartFade(new Color(1f, 1f, 1f, 0f), duration);
		}
	}

	public void EndJaguar()
	{
		if (CurrentState() != 3)
		{
			if (LevelGenerator.Instance().GetCurrentPiece().Theme != WorldConstructionHelper.Theme.Mountain)
			{
				CameraManager.Instance.RestoreCameraFromPowerUp();
			}
			PlayerController.SwitchTheme(PlayerTheme.ThemeType.Run);
			SwitchState(1);
			PlayerFXController.EndJaguarEffect();
			SFX.CatRideEnd();
		}
	}

	public void RopeAnimation_Enter(int fromState)
	{
		PlayerShadow.SetActiveRecursively(false);
		swingName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SwingLong);
		AnimationState animationState = PlayerAnimation[jumpName];
		if ((bool)animationState)
		{
			float num = animationState.time;
			if (num > mSyncTimeToSomersault - 0.2f)
			{
				num = mSyncTimeToSomersault - 0.2f;
			}
			PlayerAnimation[swingName].time = num;
			PlayerAnimation[swingName].speed = mAnimationSpeed;
		}
		PlayerAnimation.CrossFade(swingName, 0.2f);
		SFX.PlayRope();
		mDidStylishDismout = false;
	}

	public void RopeAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[swingName];
		if (((bool)animationState && animationState.time >= mSyncTimeFromRopeJumpToWalkRun) || !PlayerAnimation.IsPlaying(swingName))
		{
			PlayerController.SetRopeState(HingedRope.RopeState.PlayerLetGo);
			SwitchState(2);
			SFX.PlayLanding();
			return;
		}
		if ((bool)animationState && animationState.time >= mSyncTimeToSomersault)
		{
			PlayerController.SetRopeState(HingedRope.RopeState.PlayerLetGo);
		}
		if ((bool)animationState && animationState.time >= mSyncTimeToSomersault && PlayerController.IsSwipingUp())
		{
			SwitchState(8);
			PlayerController.Instance().Score().IncStylishDismounts();
			mDidStylishDismout = true;
			return;
		}
		float num = animationState.time - mSyncTimeToSomersault;
		if ((bool)animationState && num > 0f && num < Time.deltaTime)
		{
			SFX.PlayJump();
		}
	}

	public void RopeAnimation_Exit(int toState)
	{
		PlayerShadow.SetActiveRecursively(true);
		PlayerShadow.transform.localScale = mShadowMaxScale;
		if (mDidStylishDismout)
		{
			SFX.PlayStylishDismount();
		}
	}

	public void RopeSomersaultAnimation_Enter(int fromState)
	{
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SwingLong2);
		AnimationState animationState = PlayerAnimation[animName];
		if ((bool)animationState && animationState.wrapMode != WrapMode.ClampForever)
		{
			Debug.LogWarning("Swing animation should be set to ClampForever");
		}
		PlayerAnimation.Play(animName);
		PlayerController.SetRopeState(HingedRope.RopeState.PlayerLetGo);
		SFX.PlayJump();
	}

	public void RopeSomersaultAnimation_Update()
	{
		PlayerController.SetRopeState(HingedRope.RopeState.PlayerLetGo);
		string animName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SwingLong2);
		AnimationState animationState = PlayerAnimation[animName];
		if ((bool)animationState && animationState.time >= mSyncTimeFromRopeSomersaultToWalkRun)
		{
			SwitchState(2);
		}
	}

	public void RopeSomersaultAnimation_Exit(int toState)
	{
		PlayerController.ClearSwipeUp();
	}

	public void RessurectionAnimation_Enter(int fromState)
	{
		PlayerAnimation.Stop();
		SFX.RecoverFromDeath();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Stop();
		}
	}

	public void RessurectionAnimation_Update()
	{
		SwitchState(2);
		PlayerController.StartGameFromContinue();
	}

	public void RessurectionAnimation_Exit(int toState)
	{
		ScreenFlash(2f);
	}

	public void MacawDropAnimation_Enter(int fromState)
	{
		PlayerAnimation.Play("BirdDropOff");
		PlayerController.BirdModel.GetComponent<Animation>().Play("DropOff");
		PlayerController.BirdModel.SetActiveRecursively(true);
	}

	public void MacawDropAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation["BirdDropOff"];
		if ((bool)animationState && animationState.time >= animationState.length - 0.1f)
		{
			SwitchState(2);
			PlayerController.CallOfTheMacaw(false);
			PlayerController.StartGameFromMacaw();
		}
	}

	public void MacawDropAnimation_Exit(int toState)
	{
	}

	public void SideStepLeftAnimation_Enter(int fromState)
	{
		mSideStepLeftName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SidestepLeft);
		PlayerController.TurningDisabled = true;
		PlayerAnimation.Play(mSideStepLeftName);
		SFX.PlaySideStep();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			mPairedSideStepLeftName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.SidestepLeft);
			gameObject.GetComponent<Animation>().CrossFade(mPairedSideStepLeftName);
		}
	}

	public void SideStepLeftAnimation_Update()
	{
		if (PlayerAnimation[mSideStepLeftName].time >= PlayerAnimation[mSideStepLeftName].length - 0.1f && !CheckForDuckOrJump())
		{
			if (PlayerController.IsSideStepRight())
			{
				PlayerController.ClearSideStep();
				SwitchState(12);
			}
			else
			{
				SwitchState(2);
			}
		}
	}

	public void SideStepLeftAnimation_Exit(int toState)
	{
		PlayerController.TurningDisabled = false;
		PlayerAnimation.Blend(mSideStepLeftName, 0f, 0.4f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Blend(mPairedSideStepLeftName, 0f, 0.4f);
		}
	}

	public void SideStepRightAnimation_Enter(int fromState)
	{
		mSideStepRightName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.SidestepRight);
		PlayerController.TurningDisabled = true;
		PlayerAnimation.Play(mSideStepRightName);
		SFX.PlaySideStep();
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			mPairedSideStepRightName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.SidestepRight);
			gameObject.GetComponent<Animation>().CrossFade(mPairedSideStepRightName);
		}
	}

	public void SideStepRightAnimation_Update()
	{
		if (PlayerAnimation[mSideStepRightName].time >= PlayerAnimation[mSideStepRightName].length - 0.1f && !CheckForDuckOrJump())
		{
			if (PlayerController.IsSideStepLeft())
			{
				PlayerController.ClearSideStep();
				SwitchState(11);
			}
			else
			{
				SwitchState(2);
			}
		}
	}

	public void SideStepRightAnimation_Exit(int toState)
	{
		PlayerController.TurningDisabled = false;
		PlayerAnimation.Blend(mSideStepRightName, 0f, 0.4f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Blend(mPairedSideStepRightName, 0f, 0.4f);
		}
	}

	public void TurnAnimation_Enter(int fromState)
	{
		AnimationGroup.AnimType type = AnimationGroup.AnimType.TurnLeft;
		if (mLastTurnType == LevelGenerator.TurnType.Right)
		{
			type = AnimationGroup.AnimType.TurnRight;
		}
		mTurnName = AnimationLoader.GetAnimName(type, mTurnIndex);
		PlayerAnimation.Play(mTurnName);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			mPairedTurnName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type, mTurnIndex);
			gameObject.GetComponent<Animation>().CrossFade(mPairedTurnName);
		}
	}

	public void TurnAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[mTurnName];
		if (!animationState || animationState.time >= animationState.length - 0.2f)
		{
			SwitchState(2);
		}
		if (!GameController.Instance.Paused())
		{
			PlayerFXController.CreateFootstep();
		}
	}

	public void TurnAnimation_Exit(int toState)
	{
		PlayerAnimation.Blend(mTurnName, 0f, 0.2f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Blend(mPairedTurnName, 0f, 0.2f);
		}
		mTurnIndex = -1;
	}

	public void PreTurnAnimation_Enter(int fromState)
	{
		AnimationGroup.AnimType type = AnimationGroup.AnimType.PreTurnLeft;
		if (mLastTurnType == LevelGenerator.TurnType.Right)
		{
			type = AnimationGroup.AnimType.PreTurnRight;
		}
		mPreTurnName = AnimationLoader.GetAnimName(type, out mTurnIndex);
		PlayerAnimation.Rewind(mPreTurnName);
		PlayerAnimation.Play(mPreTurnName);
		switch (mAnimThemeType)
		{
		case PlayerTheme.ThemeType.Bike:
			SFX.BikeRideTurn();
			break;
		default:
			SFX.PlaySlide(false);
			break;
		case PlayerTheme.ThemeType.Cart:
		case PlayerTheme.ThemeType.Jaguar:
			break;
		}
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			mPairedPreTurnName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type, mTurnIndex);
			gameObject.GetComponent<Animation>().Rewind(mPreTurnName);
			gameObject.GetComponent<Animation>().Play(mPairedPreTurnName);
		}
	}

	public void PreTurnAnimation_Update()
	{
		if (PlayerController.HasTurned() != LevelGenerator.TurnType.None)
		{
			SwitchStateImmediate(14);
		}
		if (!GameController.Instance.Paused())
		{
			PlayerFXController.CreateFootstep();
		}
	}

	public void PreTurnAnimation_Exit(int toState)
	{
		PlayerAnimation.Blend(mPreTurnName, 0f, 0.2f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Blend(mPairedPreTurnName, 0f, 0.2f);
		}
	}

	public void CutsceneAnimation_Enter(int fromState)
	{
		GameObject gameObject = PlayerController.Instance().PairedModel();
		AnimationGroup.AnimType type = AnimationGroup.AnimType.Cutscene;
		if (mCutSceneEnd)
		{
			type = AnimationGroup.AnimType.CutsceneExit;
		}
		cutsceneName = AnimationLoader.GetAnimName(type);
		PlayerAnimation.Play(cutsceneName);
		PlayerAnimation.Rewind(cutsceneName);
		Vector3 localPosition = new Vector3(0f, 0f, 0f);
		if (!mCutSceneEnd)
		{
			AnimationState animationState = PlayerAnimation[cutsceneName];
			animationState.time = animationState.length;
			PlayerAnimation.Sample();
			Transform transform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001");
			animationState.time = 0f;
			localPosition = -transform.localPosition;
			localPosition.y = 0f;
			PlayerController.PlayerModel.transform.localPosition = localPosition;
		}
		if (gameObject != null)
		{
			pairedCutsceneName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type);
			gameObject.GetComponent<Animation>().Rewind(pairedCutsceneName);
			gameObject.GetComponent<Animation>().Play(pairedCutsceneName);
			gameObject.transform.localPosition = localPosition;
		}
		PlayerController.SetInCutScene(true);
		if (!mCutSceneEnd)
		{
			PlayerController.SwitchToBikeCineCam1();
		}
	}

	public void CutsceneAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[cutsceneName];
		if ((bool)animationState && animationState.time > animationState.length - 0.1f)
		{
			SwitchState(18);
			PlayerController.Instance().mAllowPlayerInput = true;
		}
	}

	public void CutsceneAnimation_Exit(int toState)
	{
		PlayerAnimation.Stop(cutsceneName);
		PlayerController.PlayerModel.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Stop(pairedCutsceneName);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		if (PlayerController.NextTheme() == PlayerTheme.ThemeType.Run)
		{
			PlayerController.SwitchPlayerTheme();
			mAnimThemeType = PlayerController.GetThemeType();
		}
		else
		{
			PlayerController.SwitchToVehicleCam();
		}
		PlayerController.SetInCutScene(false);
	}

	public void VehicleStartAnimation_Enter(int fromState)
	{
		PlayerController.Instance().mAllowPlayerInput = false;
		PlayerController.mIgnoreThemeAndReturnToCentre = true;
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.SetActiveRecursively(false);
		}
		AnimationGroup.AnimType type = AnimationGroup.AnimType.VehicleTransitionStart;
		vehicleStartName = AnimationLoader.GetAnimName(type);
		PlayerAnimation.Play(vehicleStartName);
		PlayerAnimation.Rewind(vehicleStartName);
		Vector3 localPosition = new Vector3(0f, 0f, 0f);
		if (vehicleStartName != null)
		{
			AnimationState animationState = PlayerAnimation[vehicleStartName];
			animationState.time = animationState.length;
			PlayerAnimation.Sample();
			Transform transform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001");
			animationState.time = 0f;
			localPosition = -transform.localPosition;
			localPosition.y = 0f;
		}
		PlayerController.PlayerModel.transform.localPosition = localPosition;
	}

	public void VehicleStartAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[vehicleStartName];
		if (((bool)animationState && animationState.time > animationState.length - 0.1f) || vehicleStartName == null)
		{
			SwitchState(2);
			PlayerController.Instance().mAllowPlayerInput = true;
			PieceDescriptor currentPiece = LevelGenerator.Instance().GetCurrentPiece();
			GameObject gameObject = GameObjectUtils.FindChildWithTag(currentPiece.gameObject, "TransitionThemeObject");
			if (gameObject != null)
			{
				gameObject.SetActiveRecursively(false);
			}
			PlayerController.UpdatePairedModelVisibility();
		}
	}

	public void VehicleStartAnimation_Exit(int toState)
	{
		if (vehicleStartName != null)
		{
			PlayerAnimation.Stop(vehicleStartName);
		}
		PlayerController.PlayerModel.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (PlayerController.NextTheme() == PlayerTheme.ThemeType.Bike || PlayerController.NextTheme() == PlayerTheme.ThemeType.Cart)
		{
			PlayerController.SwitchPlayerTheme();
			mAnimThemeType = PlayerController.GetThemeType();
		}
		PlayerController.mIgnoreThemeAndReturnToCentre = false;
	}

	public void VehicleStopAnimation_Enter(int fromState)
	{
		PlayerController.Instance().mAllowPlayerInput = false;
		PlayerController.mIgnoreThemeAndReturnToCentre = true;
		GameObject gameObject = PlayerController.Instance().PairedModel();
		AnimationGroup.AnimType type = AnimationGroup.AnimType.VehicleTransitionStop;
		vehicleStopName = AnimationLoader.GetAnimName(type);
		if (vehicleStopName == null)
		{
			return;
		}
		PlayerAnimation.Play(vehicleStopName);
		PlayerAnimation.Rewind(vehicleStopName);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		AnimationState animationState = PlayerAnimation[vehicleStopName];
		animationState.time = animationState.length;
		PlayerAnimation.Sample();
		Transform transform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001");
		animationState.time = 0f;
		vector = -transform.localPosition;
		vector.y = 0f;
		PlayerController.PlayerModel.transform.localPosition = vector;
		if (gameObject != null)
		{
			pairedVehicleStopName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(type);
			if (pairedVehicleStopName != null)
			{
				gameObject.GetComponent<Animation>().Rewind(pairedVehicleStopName);
				gameObject.GetComponent<Animation>().Play(pairedVehicleStopName);
				gameObject.transform.localPosition = vector;
			}
		}
	}

	public void VehicleStopAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[vehicleStopName];
		if (((bool)animationState && animationState.time > animationState.length - 0.1f) || vehicleStopName == null)
		{
			SwitchState(18);
			PlayerController.Instance().mAllowPlayerInput = true;
		}
	}

	public void VehicleStopAnimation_Exit(int toState)
	{
		PlayerAnimation.Stop(vehicleStopName);
		PlayerController.PlayerModel.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject gameObject = PlayerController.Instance().PairedModel();
		if (gameObject != null)
		{
			gameObject.GetComponent<Animation>().Stop(vehicleStopName);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		GameObject gameObject2 = null;
		if (PlayerController.GetThemeType() == PlayerTheme.ThemeType.Bike || PlayerController.NextTheme() == PlayerTheme.ThemeType.Bike || PlayerController.GetThemeType() == PlayerTheme.ThemeType.Cart || PlayerController.NextTheme() == PlayerTheme.ThemeType.Cart)
		{
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		if (gameObject2 != null)
		{
			gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
			gameObject2.transform.position = PlayerController.PlayerModel.transform.position;
			gameObject2.transform.forward = PlayerController.PlayerModel.transform.forward;
			stopSpareName = PlayerController.Instance().PairedAnimationLoader().GetAnimName(AnimationGroup.AnimType.HitWall);
			gameObject2.GetComponent<Animation>().Rewind(stopSpareName);
			gameObject2.GetComponent<Animation>().Play(stopSpareName);
			gameObject2.active = true;
			FXAutoDestroyScript fXAutoDestroyScript = gameObject2.AddComponent<FXAutoDestroyScript>();
			fXAutoDestroyScript.Duration = 2f;
		}
	}

	public void JumpFromVehicleAnimation_Enter(int fromState)
	{
		jumpFromVehicleName = AnimationLoader.GetAnimName(AnimationGroup.AnimType.JumpFromVehicle);
		if (jumpFromVehicleName != null)
		{
			SFX.PlayJump();
			PlayerAnimation[jumpFromVehicleName].time = 0f;
			PlayerAnimation[jumpFromVehicleName].speed = PlayerAnimation[jumpFromVehicleName].length / PlayerController.GetJumpTime();
			PlayerAnimation.Play(jumpFromVehicleName);
		}
		PlayerController.SwitchPlayerTheme();
		mAnimThemeType = PlayerController.GetThemeType();
	}

	public void JumpFromVehicleAnimation_Update()
	{
		AnimationState animationState = PlayerAnimation[jumpFromVehicleName];
		if (((bool)animationState && animationState.time > animationState.length - 0.1f) || jumpFromVehicleName == null)
		{
			SFX.PlayLanding();
			SwitchState(4);
			PlayerAnimation.Blend(jumpFromVehicleName, 0f, mFadeTimeFromJumpToWalkRun);
		}
	}

	public void JumpFromVehicleAnimation_Exit(int toState)
	{
		PlayerController.mIgnoreThemeAndReturnToCentre = false;
	}

	public void TrialsRunStart_Enter(int fromState)
	{
		PlayerAnimation.Play("Trial_Start_ReadyIdle");
	}

	public void TrialsRunStart_Update()
	{
		if (PlayerController.IsRunning())
		{
			SwitchState(2);
		}
	}

	public void TrialsRunStart_Exit(int toState)
	{
	}

	public void TrialsRunEnd_Enter(int fromState)
	{
		PlayerAnimation.Play("Trial_Finish");
		SFX.PlayStylishDismount();
	}

	public void TrialsRunEnd_Update()
	{
	}

	public void TrialsRunEnd_Exit(int toState)
	{
	}
}
