using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
	public enum CoinMode
	{
		AllowCoins = 0,
		DisableCoins = 1,
		ForceCoins = 2
	}

	private enum PieceTrackReductionType
	{
		None = 0,
		ForceLeft = 1,
		ForceCentre = 2,
		ForceRight = 3
	}

	private const float JuncPathAllowance = 3f;

	public static PickupController instance = null;

	public GameObject CoinModel;

	public GameObject CoinLowValueModel;

	public GameObject CoinCollectPFX;

	public GameObject PowerupModel;

	public float PowerupPeriodMinimum = 40f;

	public float PowerupPeriodMaximum = 90f;

	public int CoinSineGradientMax;

	public int CoinRunDepth;

	public float CoinSpacingStep = 1.5f;

	public float DistanceBetweenCoinRuns;

	public float MaxDistanceCanSpawnAheadOfCheckpoint = 1700f;

	public Transform PlayerAnimatedTransform;

	private CoinMode mCoinMode;

	public List<WorldConstructionHelper.Theme> ThemesThatExcludePickups;

	private static List<Coin> recycleBin = new List<Coin>();

	private static List<int> uniqueRunIndexes = new List<int>();

	private static List<int> uniqueElements = new List<int>();

	private static List<Powerup> recyclePowerUpsBin = new List<Powerup>();

	private List<Coin> mCoins = new List<Coin>();

	private List<Powerup> mPowerups = new List<Powerup>();

	private List<CoinMultiplierData> mCollectionMultiplierList = new List<CoinMultiplierData>();

	private float mTimeSinceLastCoinCollection;

	private List<CoinRun> mCoinRunBuffer;

	private Coin.CoinType mCurrentRunType;

	private int mCoinRunNextIndex;

	private float mTimeToNextPowerup;

	private bool mSineStepUp;

	private float mSineStep;

	private float mSineStepRate;

	private float[] JumpCurve = new float[25]
	{
		0.9903f, 1.8043f, 2.4763f, 2.8998f, 3.2524f, 3.5339f, 3.7442f, 3.9009f, 4.0235f, 4.1144f,
		4.1762f, 4.2115f, 4.2227f, 4.2115f, 4.1762f, 4.1144f, 4.0235f, 3.9009f, 3.7442f, 3.5464f,
		3.2858f, 2.9374f, 2.4763f, 1.6375f, 0.59f
	};

	public static PickupController Instance()
	{
		return instance;
	}

	public void SetCoinMode(CoinMode mode)
	{
		mCoinMode = mode;
	}

	private void Awake()
	{
		instance = this;
		mCoinRunBuffer = new List<CoinRun>();
	}

	private void Start()
	{
		PlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Spine1");
		mCoinMode = CoinMode.AllowCoins;
	}

	private void Update()
	{
		if (PlayerController.instance.IsRunning())
		{
			if (mCoinMode != CoinMode.DisableCoins)
			{
				UpdateCoinSpawnBuffer();
			}
			PlayerController player = PlayerController.Instance();
			UpdateCoins(player);
			UpdatePowerups(player);
			if (mTimeToNextPowerup > 0f)
			{
				mTimeToNextPowerup -= Time.deltaTime;
			}
		}
	}

	public void Reset()
	{
		mCoinRunBuffer.Clear();
		mCoins.Clear();
		mTimeToNextPowerup = Random.Range(PowerupPeriodMinimum, PowerupPeriodMaximum);
		mCoinRunNextIndex = 0;
		if (SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) <= 0)
		{
			mCurrentRunType = Coin.CoinType.ValueLow;
		}
		else
		{
			mCurrentRunType = Coin.CoinType.ValueHigh;
		}
		mSineStep = 0f;
		mSineStepUp = true;
		mSineStepRate = Random.Range(1, CoinSineGradientMax);
		mCollectionMultiplierList.Clear();
		mTimeSinceLastCoinCollection = 0f;
		PlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Spine1");
	}

	public void SpawnNewPieceCoins(PieceDescriptor piece)
	{
		BufferCoinRun(mCoinRunNextIndex, 0, piece, CoinRunDepth, null, 0, 0f, 0, 0, 0, 0f, false, false, false);
		mCoinRunNextIndex++;
	}

	public void SpawnNewPieceCoinsWithPos(PieceDescriptor piece, bool bCentre, bool bLeft, bool bRight)
	{
		BufferCoinRun(mCoinRunNextIndex, 0, piece, CoinRunDepth, null, 0, 0f, 0, 0, 0, 0f, bCentre, bLeft, bRight);
		mCoinRunNextIndex++;
	}

	public void SpawnNewPiecePowerup(PieceDescriptor piece)
	{
		mTimeToNextPowerup = Random.Range(PowerupPeriodMinimum, PowerupPeriodMaximum);
		if (piece.Theme != WorldConstructionHelper.Theme.SlippedMountain)
		{
			Vector3 worldPosition = piece.GetWorldPosition(piece.GetLength() * 0.5f, 0f);
			worldPosition.y += 3f;
			int type = Random.Range(0, 3);
			SpawnPowerup(worldPosition, (Powerup.PowerupType)type, piece.transform);
		}
	}

	public bool CanSpawnPowerup()
	{
		bool result = false;
		float distanceTravelled = PlayerController.Instance().Score().DistanceTravelled();
		if (mTimeToNextPowerup <= 0f)
		{
			if (CheckPointController.Instance().IsSafeToSpawnPickup(distanceTravelled, MaxDistanceCanSpawnAheadOfCheckpoint))
			{
				result = true;
			}
			else
			{
				mTimeToNextPowerup = Random.Range(PowerupPeriodMinimum, PowerupPeriodMaximum);
			}
		}
		return result;
	}

	public bool ThemeExcludesPowerup(WorldConstructionHelper.Theme parentTheme)
	{
		bool result = false;
		foreach (WorldConstructionHelper.Theme themesThatExcludePickup in ThemesThatExcludePickups)
		{
			if (themesThatExcludePickup == parentTheme)
			{
				result = true;
			}
		}
		return result;
	}

	private void BufferCoinRun(int coinRunIndex, int coinRunCount, PieceDescriptor piece, int runLength, PieceDescriptor lastStreamedPiece, int exitBranch, float distanceIntoPath, int yCurve, int yCurveStart, int yCurveEnd, float yCurveStartDistanceIntoPiece, bool isForcedLeft, bool isForcedCentre, bool isForcedRight)
	{
		CoinRun coinRun = new CoinRun();
		coinRun.CoinRunIndex = coinRunIndex;
		coinRun.CoinRunCount = coinRunCount;
		coinRun.Piece = piece;
		coinRun.CurrentRunDepthRemaining = runLength;
		coinRun.LastStreamedPiece = lastStreamedPiece;
		coinRun.ExitBranch = exitBranch;
		coinRun.DistanceIntoPath = distanceIntoPath;
		coinRun.YCurve = yCurve;
		coinRun.YCurveStart = yCurveStart;
		coinRun.YCurveEnd = yCurveEnd;
		coinRun.YCurveStartDistanceIntoPiece = yCurveStartDistanceIntoPiece;
		coinRun.IsForcedLeft = isForcedLeft;
		coinRun.IsForcedCentre = isForcedCentre;
		coinRun.IsForcedRight = isForcedRight;
		mCoinRunBuffer.Add(coinRun);
	}

	private void UpdateCoinSpawnBuffer()
	{
		for (int num = mCoinRunBuffer.Count - 1; num >= 0; num--)
		{
			if (mCoinRunBuffer[num].Piece == null && mCoinRunBuffer[num].LastStreamedPiece == null)
			{
				mCoinRunBuffer.RemoveAt(num);
			}
		}
		for (int num2 = mCoinRunBuffer.Count - 1; num2 >= 0; num2--)
		{
			PieceDescriptor piece = mCoinRunBuffer[num2].Piece;
			if (piece != null)
			{
				SpawnCoinRun(mCoinRunBuffer[num2]);
				mCoinRunBuffer.RemoveAt(num2);
			}
			else if (mCoinRunBuffer[num2].LastStreamedPiece != null)
			{
				if (mCoinRunBuffer[num2].ExitBranch == -1)
				{
					mCoinRunBuffer[num2].Piece = mCoinRunBuffer[num2].LastStreamedPiece.GetBranchLeftPiece();
				}
				else if (mCoinRunBuffer[num2].ExitBranch == 1)
				{
					mCoinRunBuffer[num2].Piece = mCoinRunBuffer[num2].LastStreamedPiece.GetBranchRightPiece();
				}
				else
				{
					mCoinRunBuffer[num2].Piece = mCoinRunBuffer[num2].LastStreamedPiece.GetStraightAheadPiece();
				}
			}
		}
	}

	private bool IsDuckPiece(PieceDescriptor piece)
	{
		if (piece.TypeId == WorldConstructionHelper.PieceType.Duck || piece.TypeId == WorldConstructionHelper.PieceType.JumpOrDuck)
		{
			return true;
		}
		return false;
	}

	private bool IsJumpPiece(PieceDescriptor piece)
	{
		if (WorldConstructionHelper.IsExclusiveJump(piece.TypeId))
		{
			return true;
		}
		return false;
	}

	private bool IsAnimatedPiece(PieceDescriptor piece)
	{
		bool result = false;
		Animation[] componentsInChildren = piece.gameObject.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			if (animation.clip != null)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private float GetPlayerJumpCurveLength()
	{
		float result = 18f;
		if (PlayerController.Instance() != null)
		{
			PlayerAnimationController playerAnimationController = PlayerController.Instance().GetPlayerAnimationController();
			if (playerAnimationController != null && playerAnimationController.AnimationLoader != null)
			{
				int chosenIndex = 0;
				AnimationClip anim = playerAnimationController.AnimationLoader.Run.GetAnim(AnimationGroup.AnimType.Jump, ref chosenIndex, -1);
				result = anim.length * PlayerController.Instance().GetCurrentSpeed();
			}
		}
		return result;
	}

	private PieceDescriptor[] GatherCoinRunPieces(CoinRun coinRun, out int nSpawnedCoins)
	{
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		float num = 1f + WorldConstructionHelper.GetCurrentSpeedDelta();
		float num2 = CoinSpacingStep * num;
		PieceDescriptor pieceDescriptor = coinRun.Piece;
		float num3 = coinRun.DistanceIntoPath;
		int i;
		for (i = 0; i < CoinRunDepth; i++)
		{
			TileDescriptor entryPath = pieceDescriptor.EntryPath;
			float length = entryPath.GetLength();
			PieceDescriptor pieceDescriptor2 = null;
			if (list.Count != 0)
			{
				pieceDescriptor2 = list[list.Count - 1];
			}
			if (pieceDescriptor != pieceDescriptor2)
			{
				list.Add(pieceDescriptor);
			}
			num3 += num2;
			if (!(num3 > length))
			{
				continue;
			}
			PieceDescriptor pieceDescriptor3 = null;
			if (WorldConstructionHelper.IsJunction(pieceDescriptor.TypeId))
			{
				if (entryPath == pieceDescriptor.BranchLeftPath)
				{
					pieceDescriptor3 = pieceDescriptor.GetBranchLeftPiece();
				}
				else if (entryPath == pieceDescriptor.BranchRightPath)
				{
					pieceDescriptor3 = pieceDescriptor.GetBranchRightPiece();
				}
			}
			else
			{
				pieceDescriptor3 = pieceDescriptor.GetStraightAheadPiece();
			}
			if (pieceDescriptor3 == null)
			{
				break;
			}
			num3 -= entryPath.GetLength();
			pieceDescriptor = pieceDescriptor3;
		}
		nSpawnedCoins = i;
		return list.ToArray();
	}

	private bool IsAllowedToSpawnHere(CoinRun coinRun, PieceDescriptor[] runPieces)
	{
		bool result = true;
		int num = 0;
		foreach (PieceDescriptor pieceDescriptor in runPieces)
		{
			if (pieceDescriptor.CoinsSpawned)
			{
				result = false;
				break;
			}
			if (IsAnimatedPiece(pieceDescriptor) && (IsJumpPiece(pieceDescriptor) || IsDuckPiece(pieceDescriptor)))
			{
				result = false;
				break;
			}
			if (pieceDescriptor.IsBaddiePiece())
			{
				result = false;
				break;
			}
			if (pieceDescriptor.CanSpawnMeteor())
			{
				result = false;
				break;
			}
			if (WorldConstructionHelper.IsRopeSwing(pieceDescriptor.TypeId))
			{
				result = false;
				break;
			}
			if (IsJumpPiece(pieceDescriptor))
			{
				num++;
				if (num > 1)
				{
					result = false;
					break;
				}
			}
			if (pieceDescriptor.GetPieceType() == WorldConstructionHelper.PieceType.ThemeTransition)
			{
				result = false;
				break;
			}
			if (pieceDescriptor.GetPieceType() == WorldConstructionHelper.PieceType.Checkpoint)
			{
				result = false;
				break;
			}
			if (pieceDescriptor.GetPieceType() == WorldConstructionHelper.PieceType.Pickup)
			{
				result = false;
				break;
			}
			if (pieceDescriptor.m_specificGameType != GameType.GT_NONE)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private void DelayCoinRun(CoinRun coinRun)
	{
		BufferCoinRun(coinRun.CoinRunIndex, coinRun.CoinRunCount, null, coinRun.CurrentRunDepthRemaining, coinRun.Piece, 0, 0f, 0, 0, 0, 0f, false, false, false);
	}

	private bool FindFirstJumpDistance(CoinRun coinRun, PieceDescriptor[] runPieces, out float outFirstJumpDist, out float outFirstJumpEndDist)
	{
		bool result = false;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		foreach (PieceDescriptor pieceDescriptor in runPieces)
		{
			TileDescriptor entryPath = pieceDescriptor.EntryPath;
			if (IsJumpPiece(pieceDescriptor))
			{
				result = true;
				num2 = num3 + (float)pieceDescriptor.HazardRowEnd + 5f;
				float playerJumpCurveLength = GetPlayerJumpCurveLength();
				num = num2 - playerJumpCurveLength;
				break;
			}
			num3 += entryPath.GetLength();
		}
		outFirstJumpDist = num;
		outFirstJumpEndDist = num2;
		return result;
	}

	private bool BackupCoinRun(CoinRun coinRun, PieceDescriptor firstPiece, float backupDistance)
	{
		bool result = true;
		float num = 0f;
		PieceDescriptor pieceDescriptor = null;
		PieceDescriptor currentPiece = firstPiece;
		float num2 = Mathf.Abs(backupDistance);
		while (num2 > 0f)
		{
			PieceDescriptor pieceDescriptor2 = LevelGenerator.Instance().FindPreviousPiece(currentPiece);
			if (pieceDescriptor2 == null)
			{
				result = false;
				break;
			}
			if (WorldConstructionHelper.GetSimpleType(pieceDescriptor2.TypeId) != WorldConstructionHelper.PieceType.Straight)
			{
				result = false;
				break;
			}
			TileDescriptor entryPath = pieceDescriptor2.EntryPath;
			float length = entryPath.GetLength();
			if (length > num2)
			{
				pieceDescriptor = pieceDescriptor2;
				num = length - num2;
				num2 = 0f;
			}
			else
			{
				num2 -= length;
			}
			currentPiece = pieceDescriptor2;
		}
		if (pieceDescriptor != null)
		{
			BufferCoinRun(coinRun.CoinRunIndex, coinRun.CoinRunCount, pieceDescriptor, coinRun.CurrentRunDepthRemaining, null, 0, 0f, 0, 0, 0, 0f, false, false, false);
		}
		return result;
	}

	private PieceTrackReductionType ClassifyPieceTrackReduction(PieceDescriptor piece)
	{
		PieceTrackReductionType result = PieceTrackReductionType.None;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		for (int i = 0; i < piece.EntryMarkup.Length; i++)
		{
			flag = false;
			flag2 = false;
			flag3 = false;
			flag4 = false;
			flag5 = false;
			flag6 = false;
			foreach (Tile.ResponseType type in piece.EntryMarkup[i].Left.Types)
			{
				if (type == Tile.ResponseType.SwipeUp)
				{
					flag = true;
				}
				if (type == Tile.ResponseType.Kill)
				{
					flag4 = true;
				}
			}
			foreach (Tile.ResponseType type2 in piece.EntryMarkup[i].Centre.Types)
			{
				if (type2 == Tile.ResponseType.SwipeUp)
				{
					flag2 = true;
				}
				if (type2 == Tile.ResponseType.Kill)
				{
					flag5 = true;
				}
			}
			foreach (Tile.ResponseType type3 in piece.EntryMarkup[i].Right.Types)
			{
				if (type3 == Tile.ResponseType.SwipeUp)
				{
					flag3 = true;
				}
				if (type3 == Tile.ResponseType.Kill)
				{
					flag6 = true;
				}
			}
			if (!flag && (flag2 || flag3))
			{
				result = PieceTrackReductionType.ForceLeft;
			}
			if (!flag2 && (flag || flag3))
			{
				result = PieceTrackReductionType.ForceLeft;
			}
			if (!flag3 && (flag || flag2))
			{
				result = PieceTrackReductionType.ForceRight;
			}
			if (!flag4 && (flag5 || flag6))
			{
				result = PieceTrackReductionType.ForceLeft;
			}
			if (!flag5 && (flag4 || flag6))
			{
				result = PieceTrackReductionType.ForceCentre;
			}
			if (!flag6 && (flag4 || flag5))
			{
				result = PieceTrackReductionType.ForceRight;
			}
		}
		return result;
	}

	private PieceTrackReductionType FindStartingTrackType(PieceDescriptor[] runPieces)
	{
		PieceTrackReductionType result = PieceTrackReductionType.None;
		for (int i = 0; i < runPieces.Length; i++)
		{
			PieceTrackReductionType pieceTrackReductionType = ClassifyPieceTrackReduction(runPieces[i]);
			if (pieceTrackReductionType != PieceTrackReductionType.None)
			{
				result = pieceTrackReductionType;
				break;
			}
		}
		return result;
	}

	private void SpawnCoinRun(CoinRun coinRun)
	{
		coinRun.runBranchDirChoice = ((Random.Range(0, 2) != 1) ? 1 : (-1));
		coinRun.jumpDuckDirChoice = ((Random.Range(0, 2) != 1) ? 1 : (-1));
		float num = 1f + WorldConstructionHelper.GetCurrentSpeedDelta();
		float num2 = CoinSpacingStep * num;
		float num3 = (float)CoinRunDepth * num2;
		int nSpawnedCoins = 0;
		PieceDescriptor[] array = GatherCoinRunPieces(coinRun, out nSpawnedCoins);
		if (nSpawnedCoins < 5)
		{
			DelayCoinRun(coinRun);
			return;
		}
		coinRun.CoinRunCount = 0;
		coinRun.CurrentRunDepthRemaining = nSpawnedCoins;
		if (!IsAllowedToSpawnHere(coinRun, array))
		{
			return;
		}
		float outFirstJumpDist = -1f;
		float outFirstJumpEndDist = -1f;
		if (FindFirstJumpDistance(coinRun, array, out outFirstJumpDist, out outFirstJumpEndDist) && outFirstJumpDist < 0f)
		{
			BackupCoinRun(coinRun, array[0], outFirstJumpDist);
			return;
		}
		PieceTrackReductionType pieceTrackReductionType = FindStartingTrackType(array);
		if (pieceTrackReductionType == PieceTrackReductionType.None)
		{
			pieceTrackReductionType = ((!(Random.Range(0f, 1f) <= 0.6f)) ? ((Random.Range(0, 2) == 0) ? PieceTrackReductionType.ForceLeft : PieceTrackReductionType.ForceRight) : PieceTrackReductionType.None);
		}
		if (coinRun.IsForcedLeft)
		{
			pieceTrackReductionType = PieceTrackReductionType.ForceLeft;
		}
		else if (coinRun.IsForcedCentre)
		{
			pieceTrackReductionType = PieceTrackReductionType.ForceCentre;
		}
		else if (coinRun.IsForcedRight)
		{
			pieceTrackReductionType = PieceTrackReductionType.ForceRight;
		}
		float num4 = coinRun.DistanceIntoPath;
		float num5 = 0f;
		float num6 = 0f;
		int num7 = 0;
		for (int i = 0; i < nSpawnedCoins; i++)
		{
			PieceDescriptor pieceDescriptor = array[num7];
			TileDescriptor entryPath = pieceDescriptor.EntryPath;
			float length = entryPath.GetLength();
			float num8 = num4 - num5;
			if (num7 > 0)
			{
				PieceTrackReductionType pieceTrackReductionType2 = ClassifyPieceTrackReduction(pieceDescriptor);
				if (pieceTrackReductionType2 != PieceTrackReductionType.None && pieceTrackReductionType2 != pieceTrackReductionType)
				{
					pieceTrackReductionType = pieceTrackReductionType2;
				}
			}
			Vector3 worldPosition;
			switch (pieceTrackReductionType)
			{
			default:
				worldPosition = entryPath.GetWorldPosition(num8, 0f);
				break;
			case PieceTrackReductionType.ForceLeft:
				worldPosition = entryPath.GetWorldPosition(num8, -1f);
				break;
			case PieceTrackReductionType.ForceRight:
				worldPosition = entryPath.GetWorldPosition(num8, 1f);
				break;
			}
			float num9 = 0.75f;
			if (outFirstJumpDist != -1f && num6 >= outFirstJumpDist && num6 < outFirstJumpEndDist)
			{
				float curveDistance = (num6 - outFirstJumpDist) / (outFirstJumpEndDist - outFirstJumpDist);
				float b = EvaluateJumpCurve(curveDistance);
				num9 = Mathf.Max(num9, b);
			}
			else if (IsDuckPiece(pieceDescriptor))
			{
				float num10 = Mathf.Max(0f, (float)pieceDescriptor.HazardRowStart - num2);
				float num11 = Mathf.Min(length, (float)pieceDescriptor.HazardRowEnd + num2);
				if (num8 >= num10 && num8 < num11)
				{
					float num12 = num11 - num10;
					float num13 = (num8 - num10) / num12;
					num9 = ((!(num13 <= 0.5f)) ? Mathf.Lerp(0.35f, 0.5f, (num13 - 0.5f) * 2f) : Mathf.Lerp(0.5f, 0.35f, num13 * 2f));
				}
			}
			worldPosition.y += num9;
			GameObject original = Instance().CoinModel;
			Color colour = new Color(1f, 0.8f, 0f, 1f);
			if (mCurrentRunType == Coin.CoinType.ValueLow)
			{
				original = Instance().CoinLowValueModel;
				colour = new Color(1f, 1f, 1f, 1f);
			}
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.name = coinRun.CoinRunIndex + "_" + coinRun.CoinRunCount + "_" + gameObject.name;
			Coin item = new Coin(gameObject, CoinCollectPFX, colour, worldPosition, pieceDescriptor.transform, mCurrentRunType, coinRun.CoinRunIndex, coinRun.CoinRunCount);
			mCoins.Add(item);
			if (PlayerController.Instance() != null)
			{
				PlayerController.Instance().Score().CoinSpawned();
			}
			coinRun.CoinRunCount++;
			coinRun.CurrentRunDepthRemaining--;
			pieceDescriptor.CoinsSpawned = true;
			num6 += num2;
			num4 += num2;
			if (num4 - num5 > length)
			{
				num7++;
				num5 += entryPath.GetLength();
			}
		}
	}

	private void UpdateCoins(PlayerController player)
	{
		mTimeSinceLastCoinCollection += Time.deltaTime;
		if (mTimeSinceLastCoinCollection > 5f && mCollectionMultiplierList.Count > 0)
		{
			mCollectionMultiplierList.Clear();
			player.Score().CoinRunFail();
		}
		recycleBin.Clear();
		foreach (Coin mCoin in mCoins)
		{
			if (mCoin.IsValid())
			{
				mCoin.Update(player);
				if (!mCoin.Collect(player, player.GetCoinCollectionRange()))
				{
					continue;
				}
				player.CollectCoin(mCoin.GetCoinType());
				GameController.Instance.OnCoinCollected();
				mTimeSinceLastCoinCollection = 0f;
				CoinMultiplierData item = new CoinMultiplierData(mCoin.GetRunIndex(), mCoin.GetRunElementIndex());
				mCollectionMultiplierList.Add(item);
				if (mCollectionMultiplierList.Count < CoinRunDepth)
				{
					continue;
				}
				uniqueRunIndexes.Clear();
				for (int i = 0; i < mCollectionMultiplierList.Count; i++)
				{
					if (!uniqueRunIndexes.Contains(mCollectionMultiplierList[i].RunIndex))
					{
						uniqueRunIndexes.Add(mCollectionMultiplierList[i].RunIndex);
					}
				}
				for (int j = 0; j < uniqueRunIndexes.Count; j++)
				{
					uniqueElements.Clear();
					for (int k = 0; k < mCollectionMultiplierList.Count; k++)
					{
						if (mCollectionMultiplierList[k].RunIndex == uniqueRunIndexes[j] && !uniqueElements.Contains(mCollectionMultiplierList[k].ElementIndex))
						{
							uniqueElements.Add(mCollectionMultiplierList[k].ElementIndex);
						}
					}
					if (uniqueElements.Count == CoinRunDepth)
					{
						player.Score().CoinRunSuccess();
						mCollectionMultiplierList.Clear();
					}
				}
			}
			else
			{
				recycleBin.Add(mCoin);
			}
		}
		for (int l = 0; l < recycleBin.Count; l++)
		{
			mCoins.Remove(recycleBin[l]);
		}
	}

	private void UpdatePowerups(PlayerController player)
	{
		recyclePowerUpsBin.Clear();
		for (int i = 0; i < mPowerups.Count; i++)
		{
			if (mPowerups[i].IsValid())
			{
				mPowerups[i].Update(player);
				float collectionDistance = player.GetDefaultCoinCollectionRange() * 2f;
				if (mPowerups[i].Collect(player, collectionDistance))
				{
					player.Score().RegisterEvent_CollectedPowerUp();
					switch (mPowerups[i].GetPowerupType())
					{
					case Powerup.PowerupType.Invincible:
						player.SetInvincibleBonus();
						break;
					case Powerup.PowerupType.SpeedBoost:
						player.SetSpeedBoostBonus();
						break;
					}
				}
			}
			else
			{
				recyclePowerUpsBin.Add(mPowerups[i]);
			}
		}
		for (int j = 0; j < recyclePowerUpsBin.Count; j++)
		{
			mPowerups.Remove(recyclePowerUpsBin[j]);
		}
	}

	private void SpawnPowerup(Vector3 position, Powerup.PowerupType type, Transform parent)
	{
		GameObject model = Object.Instantiate(Instance().PowerupModel) as GameObject;
		Powerup item = new Powerup(model, position, type, parent);
		mPowerups.Add(item);
	}

	private float EvaluateJumpCurve(float CurveDistance)
	{
		float num = JumpCurve[0];
		int num2 = Mathf.Clamp(Mathf.FloorToInt(CurveDistance * (float)JumpCurve.Length), 0, JumpCurve.Length - 1);
		int num3 = Mathf.Clamp(Mathf.CeilToInt(CurveDistance * (float)JumpCurve.Length), 0, JumpCurve.Length - 1);
		if (num2 == num3)
		{
			num = JumpCurve[num2];
		}
		else
		{
			float num4 = JumpCurve[num2];
			float to = JumpCurve[num3];
			float num5 = (float)num2 / (float)JumpCurve.Length;
			float num6 = (float)num3 / (float)JumpCurve.Length;
			num = Mathf.Lerp(num4, to, (CurveDistance - num5) / (num6 - num5));
		}
		return Mathf.Max(0f, num - JumpCurve[0]);
	}
}
