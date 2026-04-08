using System.Collections.Generic;
using UnityEngine;

public class WorldConstructionController : MonoBehaviour
{
	public enum Branch
	{
		Left = 0,
		Right = 1,
		StraightOn = 2
	}

	public GroupTransitionTimeManager GroupTransitionManager = new GroupTransitionTimeManager();

	public MultipieceTypeRuleManager MultipieceRuleManager = new MultipieceTypeRuleManager();

	public AppearanceRulesController AppearenceRules;

	public WorldConstructionOverrideSettings DebugOverrides;

	public float MinimumTimeBetweenOptionalJunctions = 60f;

	public float MinimumTimeBetweenRopeSwings = 5f;

	public float MinecartTransitionInJumpTime = 1f;

	public float MinecartTransitionOutJumpTime = 1f;

	public float BikeTransitionInJumpTime = 1f;

	public float BikeTransitionOutJumpTime = 1f;

	public int MinimumNumberOfNextType = 1;

	public int MaximumNumberOfNextType = 3;

	public int StraightLengthForBaddie = 12;

	public ReactionTimeSettings ReactionTime;

	public WorldLayout LevelLayout;

	public bool PreventLayoutReset;

	public bool PreventThemeTransition;

	public bool OutputDebugSpam;

	public static float BlindHazardOutAdditiveLengthMultiplier = 1f;

	public static WorldConstructionController instance = null;

	private static List<PieceDescriptor> validLoopPieces = new List<PieceDescriptor>();

	private static List<PieceDescriptor> validLoopPiecesIncludingRarity = new List<PieceDescriptor>();

	private static List<PieceDescriptor> validPieces = new List<PieceDescriptor>();

	private static List<int> difficultiesForType = new List<int>();

	private static List<int> difficultyChoiceList = new List<int>();

	private static List<PieceDescriptor> validPiecesIncRarity = new List<PieceDescriptor>();

	private List<int> mCachedApperanceIDs;

	private int mCurrentPieceIndex;

	private WorldLayoutElement mActiveElement;

	private bool mSpawnMeteor;

	private bool mSpawnBaddie;

	private BaddieController.Type mBaddieType;

	private bool mSpawnCheckpoint;

	private float mTimeSinceLastOptionalJunction;

	private float mTimeSinceLastRopeSwing;

	private float mCurrentReactionTime;

	private float mTimeTillNextReactonDecrease;

	private float mReactionDecreaseTimeRemaining;

	private float mForcedStraightLength;

	private float mLastKnownDistance;

	private PieceDescriptor.Compass mParentPieceFacing;

	private bool mParentPieceDirectionChange;

	private PieceDescriptor mSpecificRequestedPiece;

	private WorldConstructionHelper.PieceType mMultiPieceStage;

	private int mMultiPieceLoopCount;

	private int mOverallPieceIndex;

	private int mSequencesBuilt;

	private int mInitialCheckpoint;

	private int mBranchPieceIndex;

	private int mBranchLeftStartIndex;

	private int mBranchRightStartIndex;

	private int mBranchStraightOnStartIndex;

	private WorldLayoutElement mBranchActiveElement;

	private WorldLayoutElement mBranchActiveElementLeft;

	private WorldLayoutElement mBranchActiveElementRight;

	private WorldLayoutElement mBranchActiveElementStraightOn;

	private bool mIsBranching;

	private bool mForcedGroupChange;

	private bool mScorpionsEnabled;

	private bool mCrocodilesEnabled;

	private bool mSnakesEnabled;

	private bool mMeteorsEnabled;

	private bool mPlacedPickup;

	private bool mPlacedPickupSetLastCall;

	private bool mSetPiecePlacedLastTime;

	private bool mCanGoLeft;

	private bool mCanGoRight;

	public float LastKnownDistance
	{
		get
		{
			return mLastKnownDistance;
		}
	}

	public static WorldConstructionController Instance()
	{
		return instance;
	}

	public void Awake()
	{
		instance = this;
		LevelLayout = new WorldLayout();
		PreventLayoutReset = false;
		PreventThemeTransition = false;
		DebugOverrides.OverrideSettings = false;
	}

	public void Start()
	{
		ResetState(0f, 0);
	}

	private void Update()
	{
		mTimeSinceLastOptionalJunction += Time.deltaTime;
		mTimeSinceLastRopeSwing += Time.deltaTime;
		if (GroupTransitionManager.CurrentTimeInGroup > 0f)
		{
			GroupTransitionManager.CurrentTimeInGroup -= Time.deltaTime;
		}
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			UpdateReactionTime(playerController.GetSpeedAsPercent());
		}
	}

	public void ResetState(float currentDistance, int checkpointNumber)
	{
		if (!PreventLayoutReset)
		{
			BuildLevel(currentDistance);
		}
		mLastKnownDistance = currentDistance;
		mInitialCheckpoint = checkpointNumber;
		mOverallPieceIndex = (mBranchLeftStartIndex = (mBranchRightStartIndex = (mBranchStraightOnStartIndex = 0)));
		mIsBranching = false;
		GroupTransitionManager.ResetToDefaultTime();
		mSpecificRequestedPiece = null;
		mForcedStraightLength = 0f;
		mTimeSinceLastOptionalJunction = 0f;
		mTimeSinceLastRopeSwing = 0f;
		mMultiPieceLoopCount = 0;
		mMultiPieceStage = WorldConstructionHelper.PieceType.Straight;
		mCurrentPieceIndex = 0;
		ResetActiveElement();
		mReactionDecreaseTimeRemaining = 0f;
		mTimeTillNextReactonDecrease = 0f;
		UpdateReactionTime(0f);
		ThemeManager themeManager = ThemeManager.Instance;
		themeManager.SetPieceController.Reset(currentDistance);
		PieceSet loadedPieces = themeManager.LoadedPieces;
		foreach (PieceDescriptor piece in loadedPieces.Pieces)
		{
			piece.ResetTimesUsedThisRun();
		}
		ResetPickupState();
		mPlacedPickupSetLastCall = false;
	}

	public void AddDistance(float distanceToMove)
	{
		mLastKnownDistance += distanceToMove;
	}

	public void ResetPickupState()
	{
		mPlacedPickup = false;
	}

	public PieceDescriptor CreateNextPiece(PieceSet pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup, bool isInSpeedBoost, PieceDescriptor parentPiece, int parentExitBranch, GameType currentGameType)
	{
		mSpawnBaddie = (mSpawnCheckpoint = (mSpawnMeteor = false));
		List<PieceDescriptor> piecesFor = pieces.GetPiecesFor(parentTheme, parentGroup);
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		mCachedApperanceIDs = AppearenceRules.GetAvailableDifficultyIds((!DebugOverrides.OverrideSettings) ? mLastKnownDistance : ((float)DebugOverrides.MetreMark));
		bool flag = mPlacedPickup || isInSpeedBoost;
		foreach (PieceDescriptor item in piecesFor)
		{
			bool flag2 = item.Theme == WorldConstructionHelper.Theme.Jungle && item.Group == WorldConstructionHelper.Group.Transition && item.GroupTransitionExit == WorldConstructionHelper.Group.D;
			if (((item.TypeId != WorldConstructionHelper.PieceType.Straight && item.TypeId != WorldConstructionHelper.PieceType.Jump && !WorldConstructionHelper.IsJunction(item.TypeId)) || flag2) && flag)
			{
				continue;
			}
			bool flag3 = item.DifficultyGroups.Contains(0);
			if (!flag3)
			{
				foreach (int mCachedApperanceID in mCachedApperanceIDs)
				{
					if (item.DifficultyGroups.Contains(mCachedApperanceID))
					{
						flag3 = true;
						break;
					}
				}
			}
			if (item.IsExcludedForGameType(currentGameType))
			{
				flag3 = false;
			}
			if (flag3)
			{
				list.Add(item);
			}
		}
		if (parentPiece == null)
		{
			PieceDescriptor straightAtRequiredLength = CreateNextPieceHelper.GetStraightAtRequiredLength(pieces, parentTheme, parentGroup, 12f, false, false, currentGameType);
			mParentPieceFacing = PieceDescriptor.Compass.North;
			mParentPieceDirectionChange = false;
			mCanGoLeft = true;
			mCanGoRight = true;
			PieceDescriptor component = (Object.Instantiate(straightAtRequiredLength.gameObject) as GameObject).GetComponent<PieceDescriptor>();
			component.CurrentCheckPointNum = mInitialCheckpoint;
			return component;
		}
		PieceDescriptor authoredPiece = GetAuthoredPiece(parentPiece, list);
		if (authoredPiece != null)
		{
			return authoredPiece;
		}
		PieceDescriptor pieceDescriptor = ForceStraightsWhenNecessary(pieces, list, parentTheme, parentGroup);
		if (pieceDescriptor == null)
		{
			pieceDescriptor = SpawnSpecificRequestedPiece(pieces, list, parentTheme, parentGroup);
		}
		mParentPieceFacing = parentPiece.Facing;
		mParentPieceDirectionChange = parentPiece.IsJunctionPiece() || parentPiece.Bend != 0;
		mCanGoLeft = false;
		mCanGoRight = false;
		if (mParentPieceFacing == PieceDescriptor.Compass.North || mParentPieceFacing == PieceDescriptor.Compass.South)
		{
			mCanGoLeft = true;
			mCanGoRight = true;
		}
		else if (mParentPieceFacing == PieceDescriptor.Compass.East)
		{
			mCanGoLeft = true;
		}
		else if (mParentPieceFacing == PieceDescriptor.Compass.West)
		{
			mCanGoRight = true;
		}
		int num = 0;
		ThemeSetPieceController setPieceController = ThemeManager.Instance.SetPieceController;
		if (pieceDescriptor == null)
		{
			if (setPieceController.IsActive)
			{
				pieceDescriptor = setPieceController.CreateNextPiece(pieces, list, GetCurrentDesiredLength());
				pieceDescriptor.name = mOverallPieceIndex + "_" + pieceDescriptor.name + "_SetPiece";
				mSetPiecePlacedLastTime = true;
			}
			else
			{
				bool flag4 = parentPiece.TypeId == WorldConstructionHelper.PieceType.ThemeTransition;
				bool flag5 = parentPiece.Group == WorldConstructionHelper.Group.Transition;
				bool flag6 = !setPieceController.IsActive && mSetPiecePlacedLastTime;
				if (mCurrentPieceIndex >= LevelLayout.GetSize() || flag4 || flag5 || mPlacedPickup != mPlacedPickupSetLastCall || flag6)
				{
					bool flag7 = true;
					if (!flag4 && !flag6 && GroupTransitionManager.CurrentTimeInGroup <= 0f)
					{
						PieceDescriptor pieceDescriptor2 = CreatePieceTypeGroupTransition(list, parentTheme, parentGroup);
						if (pieceDescriptor2 != null)
						{
							flag7 = false;
							pieceDescriptor = (Object.Instantiate(pieceDescriptor2.gameObject) as GameObject).GetComponent<PieceDescriptor>();
						}
					}
					if (flag7)
					{
						mCurrentPieceIndex = 0;
						RefreshEnabledFlagsOnSpecialTypes(parentTheme, mCachedApperanceIDs);
						BuildLevelHeuristics(list);
					}
				}
				mPlacedPickupSetLastCall = mPlacedPickup;
				if (pieceDescriptor == null)
				{
					WorldConstructionHelper.PieceType type = SelectNextPieceType();
					PieceDescriptor pieceDescriptor3 = CreatePiece(pieces, list, parentTheme, parentGroup, type, parentPiece, isInSpeedBoost);
					pieceDescriptor3.IncrementTimesUsed();
					num = pieceDescriptor3.GetTimesUsedThisRun();
					if (pieceDescriptor3.AdditionalLengthBeforeThisPiece <= 0f)
					{
						pieceDescriptor = (Object.Instantiate(pieceDescriptor3.gameObject) as GameObject).GetComponent<PieceDescriptor>();
						CreateNextPieceBaddie(pieceDescriptor, parentTheme);
						CreateNextPieceMeteor(pieceDescriptor, parentTheme);
					}
					else if (mForcedStraightLength <= 0f)
					{
						mSpecificRequestedPiece = pieceDescriptor3;
						mForcedStraightLength = GetCurrentAdditionalLength(pieceDescriptor3.AdditionalLengthBeforeThisPiece);
						pieceDescriptor = ForceStraightsWhenNecessary(pieces, list, parentTheme, parentGroup);
					}
				}
				mSetPiecePlacedLastTime = false;
			}
		}
		if (mSpawnCheckpoint)
		{
			CheckPointController.Instance().Generate(pieceDescriptor);
		}
		else
		{
			pieceDescriptor.CurrentCheckPointNum = parentPiece.CurrentCheckPointNum;
		}
		if (pieceDescriptor.AdditionalLengthAfterThisPiece > 0f)
		{
			mForcedStraightLength = GetCurrentAdditionalLength(pieceDescriptor.AdditionalLengthAfterThisPiece);
		}
		if (!mSetPiecePlacedLastTime)
		{
			pieceDescriptor.name = mOverallPieceIndex + "_" + pieceDescriptor.name + "_" + mCurrentPieceIndex;
			if (mIsBranching)
			{
				pieceDescriptor.name += "_Branch";
			}
			mActiveElement.ReduceLength((int)pieceDescriptor.GetCachedLength());
			mActiveElement.LastPiecePlaced = pieceDescriptor;
			if (ShouldIncrementPieceIndex(list))
			{
				mCurrentPieceIndex++;
				ResetActiveElement();
				if (pieceDescriptor.TypeId == WorldConstructionHelper.PieceType.ThemeTransition && pieceDescriptor.ThemeTransitionEntry == WorldConstructionHelper.Theme.Jungle && pieceDescriptor.Theme == WorldConstructionHelper.Theme.Mountain)
				{
					mActiveElement.Length += (int)GetCurrentDesiredLength();
				}
			}
		}
		if (pieceDescriptor.Group == WorldConstructionHelper.Group.Transition)
		{
			GroupTransitionManager.ResetTime(pieceDescriptor.Theme, pieceDescriptor.GroupTransitionExit);
		}
		if (pieceDescriptor.TypeId == WorldConstructionHelper.PieceType.ThemeTransition)
		{
			GroupTransitionManager.ResetTime(pieceDescriptor.Theme, pieceDescriptor.Group);
		}
		mOverallPieceIndex++;
		return pieceDescriptor;
	}

	public PieceDescriptor ForceStraightsWhenNecessary(PieceSet pieces, List<PieceDescriptor> modifiedPiecesList, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		PieceDescriptor pieceDescriptor = null;
		if (mForcedStraightLength > 0f)
		{
			PieceDescriptor straightAtRequiredLength = CreateNextPieceHelper.GetStraightAtRequiredLength(pieces, parentTheme, parentGroup, mForcedStraightLength, false, false, LevelGenerator.Instance().CurrentGameType);
			if (straightAtRequiredLength != null)
			{
				pieceDescriptor = (Object.Instantiate(straightAtRequiredLength.gameObject) as GameObject).GetComponent<PieceDescriptor>();
				mForcedStraightLength -= pieceDescriptor.GetCachedLength();
			}
			else
			{
				PieceDescriptor pieceDescriptor2 = CreatePieceTypeGroupTransition(modifiedPiecesList, parentTheme, parentGroup);
				if (pieceDescriptor2 != null)
				{
					pieceDescriptor = (Object.Instantiate(pieceDescriptor2.gameObject) as GameObject).GetComponent<PieceDescriptor>();
					mForcedStraightLength -= pieceDescriptor.GetCachedLength();
				}
			}
		}
		return pieceDescriptor;
	}

	public PieceDescriptor SpawnSpecificRequestedPiece(PieceSet pieces, List<PieceDescriptor> modifiedPiecesList, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		PieceDescriptor pieceDescriptor = null;
		if (mSpecificRequestedPiece != null)
		{
			if (mSpecificRequestedPiece.Group == parentGroup)
			{
				PieceDescriptor component = (Object.Instantiate(mSpecificRequestedPiece.gameObject) as GameObject).GetComponent<PieceDescriptor>();
				pieceDescriptor = component;
				if (mSpecificRequestedPiece.AdditionalLengthAfterThisPiece > 0f)
				{
					mForcedStraightLength = GetCurrentAdditionalLength(mSpecificRequestedPiece.AdditionalLengthAfterThisPiece);
				}
				mSpecificRequestedPiece = null;
			}
			else
			{
				PieceDescriptor pieceDescriptor2 = CreatePieceTypeSpecificGroupTransition(pieces, modifiedPiecesList, parentTheme, parentGroup, mSpecificRequestedPiece.Group);
				if (pieceDescriptor2 != null)
				{
					PieceDescriptor component2 = (Object.Instantiate(pieceDescriptor2.gameObject) as GameObject).GetComponent<PieceDescriptor>();
					pieceDescriptor = component2;
				}
			}
			if (pieceDescriptor != null)
			{
				if (pieceDescriptor.TypeId == WorldConstructionHelper.PieceType.ThemeTransition)
				{
					CreateNextPieceVehicle(pieceDescriptor);
					ThemeManager.Instance.OnThemeChange(pieceDescriptor.Theme);
				}
				else if (pieceDescriptor.TypeId == WorldConstructionHelper.PieceType.Pickup)
				{
					PickupController.Instance().SpawnNewPiecePowerup(pieceDescriptor);
					mPlacedPickup = true;
				}
			}
		}
		return pieceDescriptor;
	}

	private float GetCurrentAdditionalLength(float additional)
	{
		PlayerController playerController = PlayerController.Instance();
		float result = 0f;
		if ((bool)playerController)
		{
			float currentSpeed = playerController.GetCurrentSpeed();
			result = currentSpeed * additional;
		}
		else
		{
			Log("Player is invalid!!! Cannot calculate additional length!");
		}
		return result;
	}

	private float GetCurrentDesiredLength()
	{
		PlayerController playerController = PlayerController.Instance();
		float result = 0f;
		if ((bool)playerController)
		{
			float currentSpeed = playerController.GetCurrentSpeed();
			float num = playerController.GetJumpTime() * currentSpeed;
			float num2 = GetCurrentReactionTime() * currentSpeed;
			result = num + num2;
		}
		else
		{
			Log("Player is invalid!!! Cannot calculate desired length!");
		}
		return result;
	}

	private float GetCurrentDesiredTransitionLength(WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Theme nextTheme)
	{
		PlayerController playerController = PlayerController.Instance();
		float result = 0f;
		if ((bool)playerController)
		{
			float currentSpeed = playerController.GetCurrentSpeed();
			if (parentTheme == WorldConstructionHelper.Theme.Bike)
			{
				result = BikeTransitionOutJumpTime * currentSpeed;
			}
			else if (nextTheme == WorldConstructionHelper.Theme.Bike)
			{
				result = BikeTransitionInJumpTime * currentSpeed;
			}
			else if (parentTheme == WorldConstructionHelper.Theme.Minecart)
			{
				result = MinecartTransitionOutJumpTime * currentSpeed;
			}
			else if (nextTheme == WorldConstructionHelper.Theme.Minecart)
			{
				result = MinecartTransitionInJumpTime * currentSpeed;
			}
		}
		else
		{
			Log("Player is invalid!!! Cannot calculate desired transition length!");
		}
		return result;
	}

	public void SwitchBranch(Branch branch)
	{
		switch (branch)
		{
		case Branch.Left:
			mCurrentPieceIndex = mBranchLeftStartIndex;
			mActiveElement = mBranchActiveElementLeft;
			break;
		case Branch.Right:
			mCurrentPieceIndex = mBranchRightStartIndex;
			mActiveElement = mBranchActiveElementRight;
			break;
		case Branch.StraightOn:
			mCurrentPieceIndex = mBranchStraightOnStartIndex;
			mActiveElement = mBranchActiveElementStraightOn;
			break;
		}
	}

	public void StartBranchIndexing()
	{
		mBranchPieceIndex = mCurrentPieceIndex;
		mBranchActiveElement = new WorldLayoutElement(mActiveElement.GetElementType(), mActiveElement.GetLength(), false);
		mIsBranching = true;
	}

	public void StopBranchIndexing(Branch branch)
	{
		switch (branch)
		{
		case Branch.Left:
			mBranchLeftStartIndex = mCurrentPieceIndex;
			mBranchActiveElementLeft = new WorldLayoutElement(mActiveElement.GetElementType(), mActiveElement.GetLength(), false);
			break;
		case Branch.Right:
			mBranchRightStartIndex = mCurrentPieceIndex;
			mBranchActiveElementRight = new WorldLayoutElement(mActiveElement.GetElementType(), mActiveElement.GetLength(), false);
			break;
		case Branch.StraightOn:
			mBranchStraightOnStartIndex = mCurrentPieceIndex;
			mBranchActiveElementStraightOn = new WorldLayoutElement(mActiveElement.GetElementType(), mActiveElement.GetLength(), false);
			break;
		}
		mCurrentPieceIndex = mBranchPieceIndex;
		mActiveElement = mBranchActiveElement;
		mIsBranching = false;
	}

	private static WorldConstructionHelper.PieceType GetNextMultiPieceType(WorldConstructionHelper.PieceType type)
	{
		switch (type)
		{
		case WorldConstructionHelper.PieceType.Pit_Type1_Entry:
		case WorldConstructionHelper.PieceType.Pit_Type1_Entry_Down:
			return WorldConstructionHelper.PieceType.Pit_Type1_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type1_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type1_Exit;
		case WorldConstructionHelper.PieceType.RopeSwing_Entry:
			return WorldConstructionHelper.PieceType.RopeSwing_Node;
		case WorldConstructionHelper.PieceType.RopeSwing_Node:
			return WorldConstructionHelper.PieceType.RopeSwing_Tile;
		case WorldConstructionHelper.PieceType.RopeSwing_Tile:
			return WorldConstructionHelper.PieceType.RopeSwing_Exit;
		case WorldConstructionHelper.PieceType.Pit_Type2_Entry:
			return WorldConstructionHelper.PieceType.Pit_Type2_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type2_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type2_Exit;
		case WorldConstructionHelper.PieceType.Pit_Type3_Entry:
			return WorldConstructionHelper.PieceType.Pit_Type3_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type3_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type3_Exit;
		case WorldConstructionHelper.PieceType.Pit_Type4_Entry:
			return WorldConstructionHelper.PieceType.Pit_Type4_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type4_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type4_Exit;
		case WorldConstructionHelper.PieceType.Pit_Type5_Entry:
			return WorldConstructionHelper.PieceType.Pit_Type5_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type5_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type5_Exit;
		case WorldConstructionHelper.PieceType.Pit_Type6_Entry:
			return WorldConstructionHelper.PieceType.Pit_Type6_Loop;
		case WorldConstructionHelper.PieceType.Pit_Type6_Loop:
			return WorldConstructionHelper.PieceType.Pit_Type6_Exit;
		case WorldConstructionHelper.PieceType.Crocodile_Entry:
			return WorldConstructionHelper.PieceType.Crocodile_Loop;
		case WorldConstructionHelper.PieceType.Crocodile_Loop:
			return WorldConstructionHelper.PieceType.Crocodile_Exit;
		case WorldConstructionHelper.PieceType.Straight_Feature:
			return WorldConstructionHelper.PieceType.Straight;
		default:
			return WorldConstructionHelper.PieceType.Exclude;
		}
	}

	private PieceDescriptor GetAuthoredPiece(PieceDescriptor parentPiece, List<PieceDescriptor> modifiedPiecesList)
	{
		AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
		return authoredLevelController.CreateNextSuitablePiece(parentPiece, modifiedPiecesList);
	}

	private void CreateNextPieceBaddie(PieceDescriptor clonedPiece, WorldConstructionHelper.Theme parentTheme)
	{
		bool flag = mSpawnBaddie && mBaddieType != BaddieController.Type.Crocodile;
		if (!flag && mBaddieType == BaddieController.Type.Crocodile && clonedPiece.TypeId == WorldConstructionHelper.PieceType.Crocodile_Loop && mMultiPieceLoopCount <= 1)
		{
			flag = true;
		}
		if (flag && IsBaddieAllowedInTheme(mBaddieType, parentTheme))
		{
			BaddieController.Instance().SpawnNewPiece(clonedPiece, mBaddieType);
		}
	}

	private void CreateNextPieceVehicle(PieceDescriptor clonedPiece)
	{
		if (clonedPiece.Theme == WorldConstructionHelper.Theme.Bike)
		{
			GameObject gameObject = GameObjectUtils.FindChildWithName(clonedPiece.gameObject, "TransitionObjectNode");
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(PlayerController.Instance().BikeModel) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActiveRecursively(true);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.Rotate(new Vector3(0f, 1f, 0f), -90f);
					gameObject2.tag = "TransitionThemeObject";
				}
			}
		}
		else
		{
			if (clonedPiece.Theme != WorldConstructionHelper.Theme.Minecart)
			{
				return;
			}
			GameObject gameObject3 = GameObjectUtils.FindChildWithName(clonedPiece.gameObject, "TransitionObjectNode");
			if ((bool)gameObject3)
			{
				GameObject gameObject4 = Object.Instantiate(PlayerController.Instance().MineCartModel, gameObject3.transform.position, Quaternion.identity) as GameObject;
				if ((bool)gameObject4)
				{
					gameObject4.SetActiveRecursively(true);
					gameObject4.transform.parent = gameObject3.transform;
					gameObject4.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
					gameObject4.transform.localPosition = Vector3.zero;
					gameObject4.transform.localRotation = Quaternion.identity;
					gameObject4.transform.Rotate(new Vector3(0f, 1f, 0f), -90f);
					gameObject4.tag = "TransitionThemeObject";
				}
			}
		}
	}

	public void CreateNextPieceMeteor(PieceDescriptor clonedPiece, WorldConstructionHelper.Theme parentTheme)
	{
		if (mSpawnMeteor)
		{
			clonedPiece.SpawnMeteorHere();
		}
	}

	public void BuildLevel(float currentDistance)
	{
		LevelLayout.Reset();
		PlayerController playerController = PlayerController.Instance();
		float num = playerController.Settings.InitialSpeed + playerController.Settings.SpeedIncrease * currentDistance;
		LevelLayout.Add(WorldConstructionHelper.PieceType.Straight, (int)(num * 4f), false);
		mSequencesBuilt = 0;
	}

	private void BuildLevelHeuristics(List<PieceDescriptor> pieceList)
	{
		int currentStraightsLength = (int)GetCurrentDesiredLength();
		List<WorldConstructionHelper.PieceType> list = new List<WorldConstructionHelper.PieceType>();
		LevelLayout.Reset();
		bool flag = false;
		bool flag2 = false;
		string text = "Available types: ";
		foreach (PieceDescriptor piece in pieceList)
		{
			bool flag3 = WorldConstructionHelper.IsHazard(piece.TypeId);
			bool flag4 = WorldConstructionHelper.IsJunction(piece.TypeId);
			if (flag3 && !flag4 && !flag2 && !list.Contains(piece.TypeId))
			{
				list.Add(piece.TypeId);
				text = text + piece.TypeId.ToString() + " ";
			}
			if (flag4 && !list.Contains(WorldConstructionHelper.PieceType.Junction))
			{
				list.Add(WorldConstructionHelper.PieceType.Junction);
				text += "Junctions ";
			}
			flag = flag || WorldConstructionHelper.IsOptionalJunction(piece.TypeId);
			flag2 = flag2 || piece.TypeId == WorldConstructionHelper.PieceType.RopeSwing_Entry;
		}
		if (mScorpionsEnabled)
		{
			list.Add(WorldConstructionHelper.PieceType.Scorpion);
			text += "Scorpions ";
		}
		if (mCrocodilesEnabled)
		{
			list.Add(WorldConstructionHelper.PieceType.Crocodile);
			text += "Crocodiles ";
		}
		if (mSnakesEnabled)
		{
			list.Add(WorldConstructionHelper.PieceType.Snake);
			text += "Snakes ";
		}
		if (mMeteorsEnabled)
		{
			list.Add(WorldConstructionHelper.PieceType.Meteor);
			text += "Meteors ";
		}
		if (OutputDebugSpam)
		{
			DebugSpam.Output(text);
		}
		LevelLayout.Add(WorldConstructionHelper.PieceType.Straight, 2, false);
		if (flag2 && mTimeSinceLastRopeSwing > MinimumTimeBetweenRopeSwings)
		{
			mTimeSinceLastRopeSwing = 0f;
			AddSingleHazardToLevel(WorldConstructionHelper.PieceType.RopeSwing, currentStraightsLength);
		}
		int num = 0;
		switch ((mSequencesBuilt >= 3) ? Random.Range(0, 2) : mSequencesBuilt)
		{
		case 0:
			BuildSimpleHazardSequence(list, currentStraightsLength);
			break;
		case 1:
			BuildStageTwoHazardSequence(list, currentStraightsLength);
			break;
		case 2:
			BuildStageThreeHazardSequence(list, currentStraightsLength);
			break;
		}
		if (flag && mTimeSinceLastOptionalJunction > MinimumTimeBetweenOptionalJunctions)
		{
			mTimeSinceLastOptionalJunction = 0f;
			AddSingleHazardToLevel(WorldConstructionHelper.PieceType.Junction_Optional, currentStraightsLength);
		}
		mSequencesBuilt++;
	}

	private void BuildSimpleHazardSequence(List<WorldConstructionHelper.PieceType> typeList, int currentStraightsLength)
	{
		if (typeList.Count > 0)
		{
			WorldConstructionHelper.PieceType pieceType = typeList[Random.Range(0, typeList.Count)];
			int num = Random.Range(MinimumNumberOfNextType, MaximumNumberOfNextType);
			for (int i = 0; i < num; i++)
			{
				AddSingleHazardToLevel(pieceType, currentStraightsLength);
			}
			typeList.Remove(pieceType);
		}
	}

	private void BuildStageTwoHazardSequence(List<WorldConstructionHelper.PieceType> typeList, int currentStraightsLength)
	{
		if (typeList.Count > 1)
		{
			WorldConstructionHelper.PieceType pieceType = typeList[Random.Range(0, typeList.Count)];
			typeList.Remove(pieceType);
			WorldConstructionHelper.PieceType pieceType2 = typeList[Random.Range(0, typeList.Count)];
			typeList.Remove(pieceType2);
			int num = Random.Range(MinimumNumberOfNextType, MaximumNumberOfNextType);
			for (int i = 0; i < num; i++)
			{
				AddSingleHazardToLevel(pieceType, currentStraightsLength);
				AddSingleHazardToLevel(pieceType2, currentStraightsLength);
			}
			WorldConstructionHelper.PieceType type = pieceType2;
			if (typeList.Count > 0)
			{
				type = typeList[Random.Range(0, typeList.Count)];
				typeList.Remove(pieceType2);
			}
			AddSingleHazardToLevel(type, currentStraightsLength);
		}
	}

	private void BuildStageThreeHazardSequence(List<WorldConstructionHelper.PieceType> typeList, int currentStraightsLength)
	{
		List<WorldConstructionHelper.PieceType> list = new List<WorldConstructionHelper.PieceType>();
		for (int i = 0; i < 3; i++)
		{
			if (typeList.Count > 0)
			{
				WorldConstructionHelper.PieceType item = typeList[Random.Range(0, typeList.Count)];
				list.Add(item);
				typeList.Remove(item);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		int num = Random.Range(MinimumNumberOfNextType, MaximumNumberOfNextType);
		for (int j = 0; j < num; j++)
		{
			WorldConstructionHelper.PieceType type;
			for (int k = 0; k < list.Count; k++)
			{
				type = list[k];
				AddSingleHazardToLevel(type, currentStraightsLength);
			}
			type = list[Random.Range(0, list.Count)];
			AddSingleHazardToLevel(type, currentStraightsLength);
		}
	}

	private void AddSingleHazardToLevel(WorldConstructionHelper.PieceType type, int currentStraightsLength)
	{
		if (WorldConstructionHelper.IsBaddie(type))
		{
			LevelLayout.Add(type, StraightLengthForBaddie, false);
		}
		else
		{
			LevelLayout.Add(type);
		}
		LevelLayout.Add(WorldConstructionHelper.PieceType.Straight, currentStraightsLength, !WorldConstructionHelper.IsBlindHazard(type));
	}

	private bool ShouldIncrementPieceIndex(List<PieceDescriptor> pieces)
	{
		if (mActiveElement.GetElementType() == WorldConstructionHelper.PieceType.Straight && mActiveElement.GetLength() > GetNextPieceTypeMaximumEntryLength(pieces))
		{
			return false;
		}
		if (WorldConstructionHelper.IsMultiTilePiecePartial(mMultiPieceStage) || mMultiPieceStage == WorldConstructionHelper.PieceType.Straight_Feature)
		{
			return false;
		}
		if (mForcedGroupChange)
		{
			mForcedGroupChange = false;
			return false;
		}
		return true;
	}

	private float GetLastPieceExitLength(PieceDescriptor piece)
	{
		return piece.GetLength() - (float)piece.HazardRowEnd;
	}

	private int GetNextPieceTypeMaximumEntryLength(List<PieceDescriptor> pieces)
	{
		int num = 0;
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.HazardRowStart > num)
			{
				num = piece.HazardRowStart;
			}
		}
		return num;
	}

	private WorldConstructionHelper.PieceType SelectNextPieceType()
	{
		if (mCurrentPieceIndex >= LevelLayout.GetSize())
		{
			mCurrentPieceIndex = 0;
		}
		return LevelLayout.GetElementType(mCurrentPieceIndex);
	}

	private PieceDescriptor GetSpecifiedRandomPieceType(PieceSet allPieces, List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup, WorldConstructionHelper.PieceType type)
	{
		if (WorldConstructionHelper.IsBaddie(type))
		{
			if (type == WorldConstructionHelper.PieceType.Crocodile)
			{
				type = WorldConstructionHelper.PieceType.Crocodile_Entry;
				mMultiPieceStage = type;
				mSpawnBaddie = true;
				mBaddieType = BaddieController.Type.Crocodile;
			}
			else
			{
				switch (type)
				{
				case WorldConstructionHelper.PieceType.Snake:
					mBaddieType = BaddieController.Type.Snake;
					mSpawnBaddie = true;
					break;
				case WorldConstructionHelper.PieceType.Scorpion:
					mBaddieType = BaddieController.Type.Scorpion;
					mSpawnBaddie = true;
					break;
				default:
					mSpawnMeteor = true;
					break;
				}
				type = WorldConstructionHelper.PieceType.Straight;
			}
		}
		bool flag = WorldConstructionHelper.IsOptionalJunction(type);
		bool flag2 = !flag && WorldConstructionHelper.IsJunction(type);
		if (flag2 || flag)
		{
			if (mCanGoLeft && mCanGoRight)
			{
				switch (Random.Range(0, 3))
				{
				case 0:
					type = ((!flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_L : WorldConstructionHelper.PieceType.Junction_L);
					break;
				case 1:
					type = ((!flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_R : WorldConstructionHelper.PieceType.Junction_R);
					break;
				default:
					type = ((!flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_T : WorldConstructionHelper.PieceType.Junction_T);
					break;
				}
			}
			else if (mCanGoLeft)
			{
				type = ((!flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_L : WorldConstructionHelper.PieceType.Junction_L);
			}
			else if (mCanGoRight)
			{
				type = ((!flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_R : WorldConstructionHelper.PieceType.Junction_R);
			}
		}
		if ((type == WorldConstructionHelper.PieceType.Pit_Type2_Loop || type == WorldConstructionHelper.PieceType.Pit_Type3_Loop || type == WorldConstructionHelper.PieceType.Pit_Type4_Loop || type == WorldConstructionHelper.PieceType.Pit_Type5_Loop || type == WorldConstructionHelper.PieceType.Pit_Type6_Loop) && mMultiPieceLoopCount > 1)
		{
			validLoopPieces.Clear();
			PieceDescriptor pieceDescriptor = null;
			foreach (PieceDescriptor piece in pieces)
			{
				if (piece.TypeId == type)
				{
					validLoopPieces.Add(piece);
					if (pieceDescriptor == null || piece.GetCachedLength() > pieceDescriptor.GetCachedLength())
					{
						pieceDescriptor = piece;
					}
				}
			}
			if (pieceDescriptor != null && validLoopPieces.Count > 1)
			{
				for (int num = validLoopPieces.Count - 1; num >= 0; num--)
				{
					PieceDescriptor pieceDescriptor2 = validLoopPieces[num];
					if (pieceDescriptor2.GetCachedLength() < pieceDescriptor.GetCachedLength())
					{
						validLoopPieces.RemoveAt(num);
					}
				}
				if (validLoopPieces.Count > 0)
				{
					validLoopPiecesIncludingRarity.Clear();
					foreach (PieceDescriptor validLoopPiece in validLoopPieces)
					{
						int num2 = 6 - validLoopPiece.RarityFactor;
						if (num2 < 1)
						{
							num2 = 1;
						}
						num2 *= num2 * num2;
						for (int i = 0; i < num2; i++)
						{
							validLoopPiecesIncludingRarity.Add(validLoopPiece);
						}
					}
					mMultiPieceLoopCount--;
					PieceDescriptor pieceDescriptor3 = validLoopPiecesIncludingRarity[Random.Range(0, validLoopPiecesIncludingRarity.Count)];
				}
			}
		}
		int num3 = -1;
		validPieces.Clear();
		difficultiesForType.Clear();
		string text = string.Empty;
		foreach (PieceDescriptor piece2 in pieces)
		{
			if (piece2.TypeId != type)
			{
				continue;
			}
			validPieces.Add(piece2);
			foreach (int difficultyGroup in piece2.DifficultyGroups)
			{
				if (!difficultiesForType.Contains(difficultyGroup))
				{
					difficultiesForType.Add(difficultyGroup);
				}
			}
		}
		if (difficultiesForType.Count > 1)
		{
			difficultyChoiceList.Clear();
			foreach (int item in difficultiesForType)
			{
				int num4 = 0;
				foreach (int mCachedApperanceID in mCachedApperanceIDs)
				{
					if (mCachedApperanceID == item)
					{
						difficultyChoiceList.Add(item);
						num4++;
					}
				}
				if (OutputDebugSpam && num4 > 0)
				{
					text = text + item + ((num4 <= 1) ? " " : ("x" + num4 + " "));
				}
			}
			if (difficultyChoiceList.Count > 0)
			{
				num3 = difficultyChoiceList[Random.Range(0, difficultyChoiceList.Count)];
				if (OutputDebugSpam)
				{
					DebugSpam.Output("Difficulty Choice: " + text);
					DebugSpam.Output("Chosen Difficulty: " + num3);
				}
			}
		}
		if (num3 != -1)
		{
			for (int num5 = validPieces.Count - 1; num5 >= 0; num5--)
			{
				if (validPieces[num5].DifficultyGroups.Count != 0 && !validPieces[num5].DifficultyGroups.Contains(num3))
				{
					validPieces.RemoveAt(num5);
				}
			}
		}
		if (type != WorldConstructionHelper.PieceType.Straight)
		{
			int num6 = int.MaxValue;
			foreach (PieceDescriptor validPiece in validPieces)
			{
				int timesUsedThisRun = validPiece.GetTimesUsedThisRun();
				if (timesUsedThisRun < num6)
				{
					num6 = timesUsedThisRun;
					if (num6 == 0)
					{
						break;
					}
				}
			}
			for (int num7 = validPieces.Count - 1; num7 >= 0; num7--)
			{
				int timesUsedThisRun2 = validPieces[num7].GetTimesUsedThisRun();
				if (timesUsedThisRun2 > num6)
				{
					validPieces.RemoveAt(num7);
				}
			}
		}
		if (validPieces.Count == 0)
		{
			if (type != WorldConstructionHelper.PieceType.Straight)
			{
				if (WorldConstructionHelper.IsHazard(type))
				{
					return GetAnyAvailableHazard(allPieces, pieces, parentTheme, parentGroup);
				}
				return GetSpecifiedRandomPieceType(allPieces, pieces, parentTheme, parentGroup, WorldConstructionHelper.PieceType.Straight);
			}
			return pieces[0];
		}
		if (validPieces.Count == 1)
		{
			return validPieces[0];
		}
		validPiecesIncRarity.Clear();
		foreach (PieceDescriptor validPiece2 in validPieces)
		{
			int num8 = 6 - validPiece2.RarityFactor;
			if (num8 < 1)
			{
				num8 = 1;
			}
			num8 *= num8 * num8;
			for (int j = 0; j < num8; j++)
			{
				validPiecesIncRarity.Add(validPiece2);
			}
		}
		return validPiecesIncRarity[Random.Range(0, validPiecesIncRarity.Count)];
	}

	private PieceDescriptor CreatePiece(PieceSet allPieces, List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup, WorldConstructionHelper.PieceType type, PieceDescriptor parentPiece, bool isInSpeedBoost)
	{
		WorldConstructionHelper.PieceType nextMultiPieceType = GetNextMultiPieceType(mMultiPieceStage);
		if (nextMultiPieceType != WorldConstructionHelper.PieceType.Exclude)
		{
			bool flag = false;
			if (mMultiPieceLoopCount > 0)
			{
				flag = true;
				mMultiPieceLoopCount--;
				if (mMultiPieceLoopCount <= 0)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if ((WorldConstructionHelper.IsMultiTilePieceEntry(mMultiPieceStage) && mMultiPieceStage != WorldConstructionHelper.PieceType.RopeSwing_Entry) || mMultiPieceStage == WorldConstructionHelper.PieceType.RopeSwing_Node)
				{
					mMultiPieceLoopCount = MultipieceRuleManager.FindLoopIterationsFor(parentTheme, parentGroup, type);
				}
				mMultiPieceStage = nextMultiPieceType;
			}
			return GetSpecifiedRandomPieceType(allPieces, pieces, parentTheme, parentGroup, mMultiPieceStage);
		}
		if (CheckPointController.Instance() != null && CheckPointController.Instance().IsAvailable(parentPiece))
		{
			PieceDescriptor checkpointFor = allPieces.GetCheckpointFor(parentTheme);
			if (checkpointFor != null)
			{
				bool flag2 = true;
				if (checkpointFor.Group != parentGroup)
				{
					flag2 = false;
					PieceDescriptor pieceDescriptor = CreatePieceTypeSpecificGroupTransition(allPieces, pieces, parentTheme, parentGroup, checkpointFor.Group);
					if (pieceDescriptor != null)
					{
						return pieceDescriptor;
					}
				}
				if (flag2)
				{
					mSpawnCheckpoint = true;
					return checkpointFor;
				}
			}
		}
		if (WorldConstructionHelper.IsHazard(type) && !mPlacedPickup && !isInSpeedBoost && !mIsBranching)
		{
			bool flag3 = false;
			if (PickupController.Instance() != null && GameTutorial.Instance != null && PickupController.Instance().CanSpawnPowerup())
			{
				if (!PickupController.Instance().ThemeExcludesPowerup(parentTheme))
				{
					mForcedStraightLength = GetCurrentDesiredLength();
					mSpecificRequestedPiece = allPieces.GetPickupFor(parentTheme);
					PieceDescriptor straightAtRequiredLength = CreateNextPieceHelper.GetStraightAtRequiredLength(allPieces, parentTheme, parentGroup, mForcedStraightLength, false, false, LevelGenerator.Instance().CurrentGameType);
					mForcedStraightLength -= straightAtRequiredLength.GetCachedLength();
					return straightAtRequiredLength;
				}
				flag3 = true;
			}
			if (!PreventThemeTransition && !PlayerController.Instance().IsInSpeedBoostBonus())
			{
				WorldConstructionHelper.Theme transitionTheme = ThemeManager.Instance.TransitionTheme;
				if (transitionTheme != parentTheme && (!flag3 || !PickupController.Instance().ThemeExcludesPowerup(transitionTheme)))
				{
					mForcedStraightLength = GetCurrentDesiredTransitionLength(parentTheme, transitionTheme);
					mSpecificRequestedPiece = allPieces.GetThemeTransition(parentTheme, transitionTheme);
					PieceDescriptor straightAtRequiredLength2 = CreateNextPieceHelper.GetStraightAtRequiredLength(allPieces, parentTheme, parentGroup, mForcedStraightLength, false, false, LevelGenerator.Instance().CurrentGameType);
					mForcedStraightLength -= straightAtRequiredLength2.GetCachedLength();
					return straightAtRequiredLength2;
				}
			}
		}
		switch (type)
		{
		case WorldConstructionHelper.PieceType.Jump:
			if (Random.Range(0, 3) == 1)
			{
				PieceDescriptor pieceDescriptor3 = CreateMultiPieceTypeJump(pieces, parentTheme, parentGroup);
				if (pieceDescriptor3 != null)
				{
					return pieceDescriptor3;
				}
			}
			break;
		case WorldConstructionHelper.PieceType.RopeSwing:
		{
			PieceDescriptor pieceDescriptor5 = CreateMultiPieceTypeRopeSwing(pieces, parentTheme, parentGroup);
			if (pieceDescriptor5 != null)
			{
				return pieceDescriptor5;
			}
			break;
		}
		case WorldConstructionHelper.PieceType.TrackReduction:
		{
			PieceDescriptor pieceDescriptor4 = CreateMultiPieceTypeTrackReduction(pieces, parentTheme, parentGroup);
			if (pieceDescriptor4 != null)
			{
				return pieceDescriptor4;
			}
			break;
		}
		case WorldConstructionHelper.PieceType.Straight:
		{
			int num = 54;
			if (mActiveElement.GetElementType() == WorldConstructionHelper.PieceType.Straight && mActiveElement.GetLength() >= num && !mIsBranching)
			{
				return CreatePieceTypeFeatureStraight(allPieces, pieces, parentTheme, parentGroup);
			}
			ThemeSetPieceController setPieceController = ThemeManager.Instance.SetPieceController;
			bool flag4 = !WorldConstructionHelper.IsMultiTilePieceEntry(type) && !mIsBranching && !mPlacedPickup && !isInSpeedBoost;
			if (mParentPieceFacing == PieceDescriptor.Compass.North && !mParentPieceDirectionChange && flag4 && setPieceController.IsTimeForSetPiece(parentTheme, mLastKnownDistance))
			{
				WorldConstructionHelper.Group firstGroup = setPieceController.GetFirstGroup();
				if (parentGroup == firstGroup)
				{
					setPieceController.Activate();
				}
				else
				{
					PieceDescriptor pieceDescriptor2 = CreatePieceTypeSpecificGroupTransition(allPieces, pieces, parentTheme, parentGroup, firstGroup);
					if (pieceDescriptor2 != null)
					{
						return pieceDescriptor2;
					}
				}
			}
			float requiredLength = mActiveElement.GetLength();
			PieceDescriptor straightAtRequiredLength3 = CreateNextPieceHelper.GetStraightAtRequiredLength(allPieces, parentTheme, parentGroup, requiredLength, mCanGoLeft && !mParentPieceDirectionChange, mCanGoRight && !mParentPieceDirectionChange, LevelGenerator.Instance().CurrentGameType);
			if (straightAtRequiredLength3 != null)
			{
				if (straightAtRequiredLength3.Bend != 0)
				{
					mForcedStraightLength = 2f;
				}
				return straightAtRequiredLength3;
			}
			break;
		}
		}
		return GetSpecifiedRandomPieceType(allPieces, pieces, parentTheme, parentGroup, type);
	}

	private bool IsBaddieAllowedInTheme(BaddieController.Type baddie, WorldConstructionHelper.Theme theme)
	{
		bool result = false;
		if (theme == WorldConstructionHelper.Theme.Jungle && (baddie == BaddieController.Type.Crocodile || baddie == BaddieController.Type.Snake))
		{
			result = true;
		}
		else if (theme == WorldConstructionHelper.Theme.Mountain && baddie == BaddieController.Type.Snake)
		{
			result = true;
		}
		else if (theme == WorldConstructionHelper.Theme.Cave && baddie == BaddieController.Type.Scorpion)
		{
			result = true;
		}
		return result;
	}

	private PieceDescriptor CreatePieceTypeFeatureStraight(PieceSet allPieces, List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.TypeId == WorldConstructionHelper.PieceType.Straight_Feature)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0)
		{
			PieceDescriptor pieceDescriptor = list[Random.Range(0, list.Count)];
			mMultiPieceStage = pieceDescriptor.TypeId;
			return pieceDescriptor;
		}
		Log(string.Concat("WARNING - Tried to place 'Feature' Straight piece but had none to choose from Theme=", parentTheme, " Group=", parentGroup));
		return GetSpecifiedRandomPieceType(allPieces, pieces, parentTheme, parentGroup, WorldConstructionHelper.PieceType.Straight);
	}

	private PieceDescriptor CreatePieceTypeGroupTransition(List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		PieceDescriptor result = null;
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.Group == WorldConstructionHelper.Group.Transition)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0)
		{
			result = list[Random.Range(0, list.Count)];
		}
		return result;
	}

	private PieceDescriptor CreatePieceTypeSpecificGroupTransition(PieceSet allPieces, List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup, WorldConstructionHelper.Group nextGroup)
	{
		PieceDescriptor pieceDescriptor = CreateNextPieceHelper.FindTransitionToGroup(pieces, nextGroup);
		if (pieceDescriptor == null)
		{
			List<PieceDescriptor> list = CreateNextPieceHelper.FindGroupTransitions(pieces);
			List<WorldConstructionHelper.Group> list2 = CreateNextPieceHelper.FindGroupsThatCanTransitionToGroup(allPieces, nextGroup);
			foreach (WorldConstructionHelper.Group item in list2)
			{
				foreach (PieceDescriptor item2 in list)
				{
					if (item == item2.GroupTransitionExit)
					{
						pieceDescriptor = item2;
						break;
					}
				}
			}
		}
		if (pieceDescriptor != null)
		{
			mForcedGroupChange = true;
		}
		return pieceDescriptor;
	}

	private PieceDescriptor CreateMultiPieceTypeJump(List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		WorldConstructionHelper.PieceType pieceType = WorldConstructionHelper.PieceType.Pit_Type1_Entry;
		PieceDescriptor result = null;
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.TypeId == pieceType)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0)
		{
			mMultiPieceStage = pieceType;
			PieceDescriptor pieceDescriptor = list[Random.Range(0, list.Count)];
			result = pieceDescriptor;
		}
		return result;
	}

	private PieceDescriptor CreateMultiPieceTypeTrackReduction(List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		PieceDescriptor pieceDescriptor = null;
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.TypeId == WorldConstructionHelper.PieceType.TrackReduction)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0 && Random.Range(0, 2) == 1)
		{
			pieceDescriptor = list[Random.Range(0, list.Count)];
		}
		if (pieceDescriptor == null)
		{
			List<PieceDescriptor> list2 = new List<PieceDescriptor>();
			foreach (PieceDescriptor piece2 in pieces)
			{
				if (piece2.TypeId == WorldConstructionHelper.PieceType.Pit_Type2_Entry || piece2.TypeId == WorldConstructionHelper.PieceType.Pit_Type3_Entry || piece2.TypeId == WorldConstructionHelper.PieceType.Pit_Type4_Entry || piece2.TypeId == WorldConstructionHelper.PieceType.Pit_Type5_Entry || piece2.TypeId == WorldConstructionHelper.PieceType.Pit_Type6_Entry)
				{
					list2.Add(piece2);
				}
			}
			if (list2.Count > 0)
			{
				PieceDescriptor pieceDescriptor2 = list2[Random.Range(0, list2.Count)];
				mMultiPieceStage = pieceDescriptor2.TypeId;
				pieceDescriptor = pieceDescriptor2;
			}
		}
		return pieceDescriptor;
	}

	private PieceDescriptor CreateMultiPieceTypeRopeSwing(List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		WorldConstructionHelper.PieceType pieceType = WorldConstructionHelper.PieceType.RopeSwing_Entry;
		PieceDescriptor result = null;
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor piece in pieces)
		{
			if (piece.TypeId == pieceType)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0)
		{
			mMultiPieceStage = pieceType;
			PieceDescriptor pieceDescriptor = list[Random.Range(0, list.Count)];
			result = pieceDescriptor;
		}
		return result;
	}

	private PieceDescriptor GetAnyAvailableHazard(PieceSet allPieces, List<PieceDescriptor> pieces, WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup)
	{
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor piece in pieces)
		{
			if (WorldConstructionHelper.IsHazard(piece.TypeId) && piece.TypeId != WorldConstructionHelper.PieceType.Junction && piece.TypeId != WorldConstructionHelper.PieceType.Junction_L && piece.TypeId != WorldConstructionHelper.PieceType.Junction_R && piece.TypeId != WorldConstructionHelper.PieceType.Junction_T)
			{
				list.Add(piece);
			}
		}
		if (list.Count > 0)
		{
			return list[Random.Range(0, list.Count)];
		}
		return GetSpecifiedRandomPieceType(allPieces, pieces, parentTheme, parentGroup, WorldConstructionHelper.PieceType.Straight);
	}

	public int GetCurrentPieceIndex()
	{
		return mCurrentPieceIndex;
	}

	private void ResetActiveElement()
	{
		int num = mCurrentPieceIndex;
		if (num >= LevelLayout.GetSize())
		{
			num = 0;
		}
		int num2 = LevelLayout.GetElementLength(num);
		if (mActiveElement != null && mActiveElement.LastPiecePlaced != null && mActiveElement.LastPiecePlaced.HazardRowEnd != -1)
		{
			int num3 = (int)mActiveElement.LastPiecePlaced.GetCachedLength() - mActiveElement.LastPiecePlaced.HazardRowEnd;
			num2 -= num3;
		}
		WorldConstructionHelper.PieceType elementType = LevelLayout.GetElementType(num);
		mActiveElement = new WorldLayoutElement(elementType, num2, false);
		mActiveElement.MinimumLength = LevelLayout.GetElementMinimumLength(num);
	}

	private ReactionTimePhase FindReactionPhase(List<ReactionTimePhase> phases, float atDistance)
	{
		float num = 0f;
		ReactionTimePhase result = null;
		for (int i = 0; i < phases.Count; i++)
		{
			result = phases[i];
			num += phases[i].PhaseDurationInMetres;
			if (atDistance < num)
			{
				break;
			}
		}
		return result;
	}

	private float GetCurrentReactionTime()
	{
		float num = 0f;
		if (DebugOverrides.OverrideSettings)
		{
			return DebugOverrides.ReactionTime;
		}
		WorldConstructionHelper.Theme transitionTheme = ThemeManager.Instance.TransitionTheme;
		ReactionTimePhase reactionTimePhase = null;
		switch (transitionTheme)
		{
		case WorldConstructionHelper.Theme.Bike:
			reactionTimePhase = FindReactionPhase(ReactionTime.BikePhases, mLastKnownDistance);
			break;
		case WorldConstructionHelper.Theme.Minecart:
			reactionTimePhase = FindReactionPhase(ReactionTime.MinecartPhases, mLastKnownDistance);
			break;
		default:
			reactionTimePhase = FindReactionPhase(ReactionTime.Phases, mLastKnownDistance);
			break;
		}
		if (mTimeTillNextReactonDecrease <= 0f && mReactionDecreaseTimeRemaining > 0f)
		{
			return reactionTimePhase.SmallestReactionTime;
		}
		return mCurrentReactionTime;
	}

	private void UpdateReactionTime(float percent)
	{
		WorldConstructionHelper.Theme theme = WorldConstructionHelper.Theme.Jungle;
		if (mActiveElement != null && mActiveElement.LastPiecePlaced != null)
		{
			theme = mActiveElement.LastPiecePlaced.Theme;
		}
		ReactionTimePhase reactionTimePhase = null;
		switch (theme)
		{
		case WorldConstructionHelper.Theme.Bike:
			reactionTimePhase = FindReactionPhase(ReactionTime.BikePhases, mLastKnownDistance);
			break;
		case WorldConstructionHelper.Theme.Minecart:
			reactionTimePhase = FindReactionPhase(ReactionTime.MinecartPhases, mLastKnownDistance);
			break;
		default:
			reactionTimePhase = FindReactionPhase(ReactionTime.Phases, mLastKnownDistance);
			break;
		}
		mCurrentReactionTime = Mathf.Lerp(reactionTimePhase.StartingReactionTime, reactionTimePhase.SmallestReactionTime, percent);
		if (mReactionDecreaseTimeRemaining <= 0f && mTimeTillNextReactonDecrease <= 0f)
		{
			mTimeTillNextReactonDecrease = Random.Range(ReactionTime.MinimumTimeBetweenReactonDecrease, ReactionTime.MaximumTimeBetweenReactonDecrease);
			mReactionDecreaseTimeRemaining = Random.Range(ReactionTime.MinimumReactonDecreaseTime, ReactionTime.MaximumReactonDecreaseTime);
		}
		else if (mTimeTillNextReactonDecrease > 0f)
		{
			mTimeTillNextReactonDecrease -= Time.deltaTime;
		}
		else if (mReactionDecreaseTimeRemaining > 0f)
		{
			mReactionDecreaseTimeRemaining -= Time.deltaTime;
		}
	}

	private void RefreshEnabledFlagsOnSpecialTypes(WorldConstructionHelper.Theme parentTheme, List<int> availableDifficulties)
	{
		mScorpionsEnabled = false;
		mCrocodilesEnabled = false;
		mSnakesEnabled = false;
		mMeteorsEnabled = false;
		foreach (int availableDifficulty in availableDifficulties)
		{
			if (AppearenceRules.Scorpion == availableDifficulty)
			{
				mScorpionsEnabled = true;
			}
			if (AppearenceRules.Crocodile == availableDifficulty)
			{
				mCrocodilesEnabled = true;
			}
			if (AppearenceRules.Snake == availableDifficulty)
			{
				mSnakesEnabled = true;
			}
			if (AppearenceRules.Meteor == availableDifficulty)
			{
				mMeteorsEnabled = true;
			}
			if (mScorpionsEnabled && mCrocodilesEnabled && mSnakesEnabled && mMeteorsEnabled)
			{
				break;
			}
		}
		mScorpionsEnabled = mScorpionsEnabled && IsBaddieAllowedInTheme(BaddieController.Type.Scorpion, parentTheme);
		mCrocodilesEnabled = mCrocodilesEnabled && IsBaddieAllowedInTheme(BaddieController.Type.Crocodile, parentTheme);
		mSnakesEnabled = mSnakesEnabled && IsBaddieAllowedInTheme(BaddieController.Type.Snake, parentTheme);
		mMeteorsEnabled = mMeteorsEnabled && parentTheme != WorldConstructionHelper.Theme.Cave && parentTheme != WorldConstructionHelper.Theme.Minecart;
	}

	private void Log(string log)
	{
	}

	private void DebugDumpLevel()
	{
		for (int i = 0; i < LevelLayout.GetSize(); i++)
		{
			WorldConstructionHelper.PieceType elementType = LevelLayout.GetElementType(i);
			int elementLength = LevelLayout.GetElementLength(i);
			MonoBehaviour.print(string.Concat("[", i, "] ", elementType, " : ", elementLength));
		}
	}
}
