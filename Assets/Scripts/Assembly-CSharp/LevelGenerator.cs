using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public enum TurnType
	{
		None = 0,
		Left = 1,
		Right = 2
	}

	public PlayerController Player;

	public GameObject CameraTarget;

	public GameObject PlayerLookAhead;

	public float PlayerLookAheadDistance;

	public GameObject ChaseDude;

	public GameObject SmoothedTarget;

	public GameEventController GameEventHandler;

	public Light MainLight;

	public float LightAngleThreshold = 5f;

	public float LightChangeSpeed = 20f;

	public MeteorController MeteorController;

	public bool GenerateMeteors = true;

	public SoundFXData QuakeSFX;

	public WorldConstructionHelper.Theme StartTheme;

	public WorldConstructionHelper.Group StartGroup;

	public static LevelGenerator instance = null;

	public static float GENERATION_DEPTH = 120f;

	private float mLeftBound;

	private float mRightBound;

	private float mPlayerLeftBound;

	private float mPlayerRightBound;

	private float PathWidth;

	private bool PathWidthDirty = true;

	private Vector3 PathCentre = default(Vector3);

	private Vector3 PathCentreAtCurrentPosition = default(Vector3);

	private bool PathCentreDirty = true;

	private Hashtable activePDHashTable = new Hashtable();

	private GameType m_currentGameType;

	private bool mFoundRopeObject;

	private Transform mCurrentRopeNode;

	private bool mFoundCheckPointObject;

	private Transform mCheckPointObject;

	private Transform mCheckPointBird;

	private static List<PieceDescriptor> startPieces = new List<PieceDescriptor>();

	private static List<PieceDescriptor> coinRunPieces = new List<PieceDescriptor>();

	private PieceDescriptor mCurrentPiece;

	private float mBranchSwitchTime;

	private Tile mLastTile;

	private List<PieceDescriptor> mPreviousPieces;

	private List<PieceDescriptor> mActivePieces;

	private float mPathDistance;

	private float mLookAheadDistance;

	private Vector3 mLightAngles;

	private WorldConstructionHelper.Theme mPreviousTheme;

	public List<string> DeathReport = new List<string>();

	private static int DEATH_REPORT_LOG_SIZE = 8;

	private Color mMainLightColourDefault;

	private float mMainLightColourIntensity;

	private float mCheckpointStartDistance;

	private int mCheckpointStartNum;

	public GameType CurrentGameType
	{
		get
		{
			return m_currentGameType;
		}
		set
		{
			m_currentGameType = value;
		}
	}

	public static LevelGenerator Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mLastTile = null;
		mPreviousPieces = new List<PieceDescriptor>();
		mActivePieces = new List<PieceDescriptor>();
		mPreviousTheme = WorldConstructionHelper.Theme.Jungle;
		activePDHashTable.Clear();
		m_currentGameType = GameType.GT_CLASSIC;
	}

	public WorldConstructionHelper.PieceType CurrentPieceType()
	{
		if ((bool)mCurrentPiece)
		{
			if (mCurrentPiece.IsBaddiePiece())
			{
				return WorldConstructionHelper.PieceType.Baddie;
			}
			return WorldConstructionHelper.GetSimpleType(mCurrentPiece.TypeId);
		}
		return WorldConstructionHelper.PieceType.Straight;
	}

	public WorldConstructionHelper.Theme CurrentPieceTheme()
	{
		WorldConstructionHelper.Theme result = WorldConstructionHelper.Theme.Jungle;
		if (mCurrentPiece != null)
		{
			result = mCurrentPiece.Theme;
		}
		return result;
	}

	private void Start()
	{
		Vector3 vector = ((!(Player != null)) ? Vector3.zero : Player.transform.eulerAngles);
		Vector3 vector2 = ((!(MainLight != null)) ? Vector3.zero : MainLight.transform.eulerAngles);
		mLightAngles = vector2 - vector;
		mMainLightColourDefault = MainLight.color;
		mMainLightColourIntensity = MainLight.intensity;
		ResetCheckpointStart();
	}

	public void ClearRopeNode()
	{
		mCurrentRopeNode = null;
	}

	public bool GetRopeNode(out Transform ropeNode)
	{
		ropeNode = null;
		if (mCurrentRopeNode == null)
		{
			mFoundRopeObject = false;
		}
		if (mFoundRopeObject)
		{
			ropeNode = mCurrentRopeNode;
		}
		return mFoundRopeObject;
	}

	public void UpdateRopeObject()
	{
		if (mFoundRopeObject)
		{
			return;
		}
		PieceDescriptor pieceDescriptor = mCurrentPiece;
		if (!pieceDescriptor)
		{
			return;
		}
		pieceDescriptor = pieceDescriptor.GetNextPiece();
		while ((bool)pieceDescriptor && !mFoundRopeObject)
		{
			if (pieceDescriptor.mHasRopeNode == PieceDescriptor.HasNode.Unsure || pieceDescriptor.mHasRopeNode == PieceDescriptor.HasNode.Yes)
			{
				Transform transform = pieceDescriptor.transform.Find("ropeNode");
				if ((bool)transform)
				{
					mCurrentRopeNode = transform;
					mFoundRopeObject = true;
					pieceDescriptor.mHasRopeNode = PieceDescriptor.HasNode.Yes;
				}
				else
				{
					pieceDescriptor.mHasRopeNode = PieceDescriptor.HasNode.No;
				}
			}
			if (!mFoundRopeObject)
			{
				pieceDescriptor = pieceDescriptor.GetNextPiece();
			}
		}
	}

	public bool GetCheckPointObject(out Transform checkPointObject, out Transform checkPointBird)
	{
		checkPointObject = (checkPointBird = null);
		if (mCheckPointObject == null || mCheckPointBird == null)
		{
			mFoundCheckPointObject = false;
		}
		if (mFoundCheckPointObject)
		{
			checkPointObject = mCheckPointObject;
			checkPointBird = mCheckPointBird;
		}
		return mFoundCheckPointObject;
	}

	public void UpdateCheckPointObject()
	{
		if (mFoundCheckPointObject)
		{
			return;
		}
		PieceDescriptor nextPiece = mCurrentPiece;
		while ((bool)nextPiece && !mCheckPointObject)
		{
			if (nextPiece.mHasCheckPointNode == PieceDescriptor.HasNode.Unsure || nextPiece.mHasCheckPointNode == PieceDescriptor.HasNode.Yes)
			{
				Transform transform = nextPiece.transform.Find("Node_Chkpnt_Gong");
				if ((bool)transform)
				{
					mCheckPointObject = transform;
					Transform transform2 = nextPiece.transform.Find("Node_Chkpnt_Macaw");
					if ((bool)transform2)
					{
						mCheckPointBird = transform2;
						mFoundCheckPointObject = true;
						nextPiece.mHasCheckPointNode = PieceDescriptor.HasNode.Yes;
					}
					else
					{
						nextPiece.mHasCheckPointNode = PieceDescriptor.HasNode.No;
					}
				}
				else
				{
					nextPiece.mHasCheckPointNode = PieceDescriptor.HasNode.No;
				}
			}
			if (!mFoundCheckPointObject)
			{
				nextPiece = nextPiece.GetNextPiece();
			}
		}
	}

	private void RecentreWorld()
	{
		if (Player.IsRunning() && (double)Time.timeScale > 0.0)
		{
			foreach (PieceDescriptor mActivePiece in mActivePieces)
			{
				if (mActivePiece != null)
				{
					mActivePiece.transform.Translate(-Player.transform.position, Space.World);
				}
			}
		}
		CameraManager.Instance.RepositionCameraWithWorldRecentre(Player.transform.position);
		DebugWorldTracker.RepositionWithWorldRecentre(Player.transform.position);
		Player.mHingedRope.transform.position -= Player.transform.position;
		Player.transform.position = Vector3.zero;
		mFoundRopeObject = false;
		mCurrentRopeNode = null;
		Player.mHingedRope.SetState(HingedRope.RopeState.WaitForNode);
		UpdatePathCentreAtCurrentPoint();
	}

	public void SetPlayerPositionFromLeftRightValue(Vector3 movementDirection)
	{
		float leftRight = Player.GetLeftRight();
		if (Player.GetThemeType() == PlayerTheme.ThemeType.Cart)
		{
			leftRight = 0f;
		}
		if (mCurrentPiece != null)
		{
			Vector3 position = Player.transform.position;
			Vector3 worldPosition = mCurrentPiece.GetWorldPosition(mPathDistance, leftRight);
			Player.transform.position = Vector3.Lerp(worldPosition, position, mBranchSwitchTime);
			Vector3 forward = Player.transform.forward;
			bool flag = Player.CurrentTheme().TurnRate == 0f || Player.TurningDisabled;
			Vector3 worldDirection = mCurrentPiece.GetWorldDirection(mPathDistance, leftRight);
			Vector3 vector = ((!flag) ? movementDirection : worldDirection);
			Vector3 worldUp = mCurrentPiece.GetWorldUp(mPathDistance, leftRight);
			Player.transform.LookAt(Player.transform.position + vector, worldUp);
			CameraTarget.transform.forward = worldDirection;
			if (!(Vector3.Dot(forward, Player.transform.forward) < 0.3f))
			{
			}
		}
	}

	public void CentrePlayerPositionOnCurrentPiece(float distanceIntoPiece)
	{
		if (!(mCurrentPiece != null))
		{
			return;
		}
		Vector3 position = Player.transform.position;
		Vector3 worldPosition = mCurrentPiece.GetWorldPosition(distanceIntoPiece, 0f);
		Player.transform.position = Vector3.Lerp(worldPosition, position, 0f);
		Vector3 worldDirection = mCurrentPiece.GetWorldDirection(distanceIntoPiece, 0f);
		Vector3 worldUp = mCurrentPiece.GetWorldUp(distanceIntoPiece, 0f);
		Player.transform.LookAt(Player.transform.position + worldDirection, worldUp);
		CameraTarget.transform.forward = worldDirection;
		if (ChaseDude != null)
		{
			float leftRight = 0f;
			ChaseDude.transform.position = mCurrentPiece.GetWorldPositionAutoBranch(PlayerLookAheadDistance + distanceIntoPiece, leftRight);
			ChaseDude.transform.forward = mCurrentPiece.GetWorldDirectionAutoBranch(PlayerLookAheadDistance + distanceIntoPiece, leftRight);
		}
		if (SmoothedTarget != null)
		{
			mCurrentPiece.EvaluatePath(0f, mCurrentPiece.SelectedBranch, delegate(PieceDescriptor piece, TileDescriptor path, float distance)
			{
				float length = path.GetLength();
				Vector3 worldPosition2 = path.GetWorldPosition(0f, 0f);
				Vector3 worldPosition3 = path.GetWorldPosition(length, 0f);
				SmoothedTarget.transform.position = Vector3.Lerp(worldPosition2, worldPosition3, distance / length);
				SmoothedTarget.transform.forward = (worldPosition3 - worldPosition2).normalized;
			});
		}
		PlayerLookAhead.transform.position = mCurrentPiece.GetWorldPosition(PlayerLookAheadDistance + distanceIntoPiece, 0f);
		PathCentreAtCurrentPosition = Player.transform.position;
	}

	public void DoQuake(float ferocity, float duration)
	{
		if (mCurrentPiece != null)
		{
			CameraManager.Instance.ShakeCurrentCamera(ferocity, duration);
			SoundManager.Instance.Play2D(QuakeSFX);
			StartCoroutine(EndQuakeDelayed(duration));
		}
	}

	private IEnumerator EndQuakeDelayed(float delay)
	{
		yield return new WaitForSeconds(delay);
		SoundManager.Instance.Stop2D(QuakeSFX);
	}

	public void CreateTargetedMeteor(Meteor.TARGET target, Meteor.METEOR_TYPE type, MeteorController.DIRECTION direction, bool destroyObject)
	{
		if (mCurrentPiece != null)
		{
			MeteorController.CreateTargetedMeteor(mCurrentPiece, target, type, direction, destroyObject);
		}
	}

	private void Update()
	{
		CleanupPreviousPieces(40f);
		PathWidthDirty = true;
		PathCentreDirty = true;
		if (!Player.IsRunning())
		{
			return;
		}
		mBranchSwitchTime = Mathf.Max(mBranchSwitchTime - Time.deltaTime, 0f);
		if (!Player.PlayerGameOver())
		{
			MeteorController.GenerateMeteors(mCurrentPiece, mPathDistance, GenerateMeteors);
		}
		float num = Player.GetDistanceMoved();
		Vector3 forward = Player.transform.forward;
		float leftRight = Player.GetLeftRight();
		Player.Score().AddDistance(num);
		WorldConstructionController.Instance().AddDistance(num);
		if (mCurrentPiece != null)
		{
			float num2 = -1.49f;
			float num3 = 1.49f;
			for (float num4 = Player.GetLeftRight(); num4 > num2; num4 -= 1f)
			{
				Tile tile = mCurrentPiece.GetTile(mPathDistance, num4);
				if (tile != null && tile.IsOfType(Tile.ResponseType.BlockLeft))
				{
					num2 = Mathf.Max(Mathf.Round(num4) - 0.49f, -1.49f);
				}
			}
			for (float num5 = Player.GetLeftRight(); num5 < num3; num5 += 1f)
			{
				Tile tile2 = mCurrentPiece.GetTile(mPathDistance, num5);
				if (tile2 != null && tile2.IsOfType(Tile.ResponseType.BlockRight))
				{
					num3 = Mathf.Min(Mathf.Round(num5) + 0.49f, 1.49f);
				}
			}
			PlayerThemeSettings playerThemeSettings = PlayerController.Instance().CurrentTheme();
			float num6 = playerThemeSettings.TiltSpeed;
			if (Player.IsDodging())
			{
				num6 = playerThemeSettings.DodgeTiltSpeed;
			}
			if (Player.mIgnoreThemeAndReturnToCentre)
			{
				float value = 0f;
				if (Player.GetLeftRight() > 0.1f)
				{
					value = Player.GetLeftRight() - Time.deltaTime * playerThemeSettings.TiltSmoothing;
				}
				else if (Player.GetLeftRight() < -0.1f)
				{
					value = Player.GetLeftRight() + Time.deltaTime * playerThemeSettings.TiltSmoothing;
				}
				Player.SetLeftRight(Mathf.Clamp(value, num2, num3));
			}
			else if (Player.GetThemeType() == PlayerTheme.ThemeType.Run || Player.GetThemeType() == PlayerTheme.ThemeType.Slope || Player.GetThemeType() == PlayerTheme.ThemeType.Jaguar)
			{
				if (Player.GetCurrentSpeed() > 0f)
				{
					float num7 = 1f;
					float num8 = 1f;
					float num9 = 5f;
					mLeftBound = Mathf.Lerp(mLeftBound, num2, Time.deltaTime * num7);
					mRightBound = Mathf.Lerp(mRightBound, num3, Time.deltaTime * num7);
					mPlayerLeftBound = Mathf.Lerp(mPlayerLeftBound, num2, Time.deltaTime * ((!(mPlayerLeftBound < num2)) ? num8 : num9));
					mPlayerRightBound = Mathf.Lerp(mPlayerRightBound, num3, Time.deltaTime * ((!(mPlayerRightBound > num3)) ? num8 : num9));
					float num10 = num6 * Time.deltaTime * Player.GetTiltAmount();
					float num11 = 3f / GetPathWidthAtCurrentPoint();
					float value2 = Player.GetLeftRight() + num10 * num11;
					if (playerThemeSettings.ReturnToCentre && Player.AllowReturnToCentre())
					{
						value2 = Mathf.Lerp(mPlayerLeftBound, mPlayerRightBound, (Player.GetTiltAmount() + 1f) / 2f);
					}
					Player.SetLeftRight(Mathf.Clamp(value2, num2, num3));
				}
			}
			else if (Player.GetThemeType() == PlayerTheme.ThemeType.Cart)
			{
				if (Player.GetCurrentSpeed() > 0f)
				{
					float tiltAmount = Player.GetTiltAmount();
					Player.SetLeftRight(Mathf.Clamp(tiltAmount, num2, num3));
				}
			}
			else if (Player.GetCurrentSpeed() > 0f)
			{
				if (Player.IsSliding() && !Player.IsDodging())
				{
					num6 *= playerThemeSettings.SlideTiltSpeedModifier;
				}
				float num12 = num6 * Time.deltaTime * Player.GetTiltAmount();
				float num13 = 3f / GetPathWidthAtCurrentPoint();
				float value3 = Player.GetLeftRight() + num12 * num13;
				if (Mathf.Abs(Player.GetRawTilt()) > playerThemeSettings.TiltSideStepTolerance || Player.IsSideStepLeft() || Player.IsSideStepRight())
				{
					Player.SFX.BikeRideTurn();
				}
				float a = -1.25f;
				float a2 = 1.25f;
				num2 = Mathf.Max(a, num2);
				num3 = Mathf.Min(a2, num3);
				mLeftBound = Mathf.Lerp(mLeftBound, num2, Time.deltaTime);
				mRightBound = Mathf.Lerp(mRightBound, num3, Time.deltaTime);
				Player.SetLeftRight(Mathf.Clamp(value3, num2, num3));
			}
		}
		float leftRight2 = (Player.GetLeftRight() - leftRight) * Player.CurrentTheme().TurnRate + leftRight;
		forward = ((!(mCurrentPiece != null)) ? Player.transform.forward : (mCurrentPiece.GetWorldPosition(mPathDistance + 0.2f, leftRight2) - mCurrentPiece.GetWorldPosition(mPathDistance, leftRight)).normalized);
		while (num > 0f)
		{
			if (mCurrentPiece == null)
			{
				if (Player.IsRunning())
				{
					if (mPreviousPieces.Count > 0 && GameEventHandler.CanSavePlayer(Player, mPathDistance, mPreviousPieces[mPreviousPieces.Count - 1]))
					{
						mCurrentPiece = mPreviousPieces[mPreviousPieces.Count - 1];
						mPreviousPieces.Remove(mCurrentPiece);
						mPathDistance = mCurrentPiece.GetCachedLength();
						bool endOfPathDetected;
						bool flag = mCurrentPiece.IsTileOfResponseType(mPathDistance, Player.GetLeftRight(), Tile.ResponseType.SwipeLeft, out endOfPathDetected);
						mCurrentPiece.SwitchBranch((!flag) ? 1 : (-1));
						Player.FinishTurns();
						RecentreWorld();
					}
					else if (mPreviousPieces.Count > 0 && mPreviousPieces[mPreviousPieces.Count - 1] != null)
					{
						Player.Kill(mPreviousPieces[mPreviousPieces.Count - 1]);
						UpdateDeathReport(mPreviousPieces[mPreviousPieces.Count - 1]);
					}
					else
					{
						Player.Kill();
					}
				}
				if (mCurrentPiece == null)
				{
					return;
				}
			}
			Advance(Mathf.Min(num, 1f));
			if (mCurrentPiece == null)
			{
				return;
			}
			Tile tile3 = mCurrentPiece.GetTile(mPathDistance, Player.GetLeftRight());
			if (tile3 != mLastTile)
			{
				UpdatePlayerPerception(Player.GetPerceptionInfo(), 10);
			}
			Player.ActOnPerception();
			mLastTile = tile3;
			if (tile3 != null && GameEventHandler.ProcessCurrentTile(tile3, Player))
			{
				GameEventHandler.ProcessAheadHazards(mPathDistance, mCurrentPiece);
				float num14 = 0.001f;
				if (GameEventHandler.HasInitiatedTurnLeft())
				{
					mLookAheadDistance = 0f;
					mCurrentPiece.SwitchBranch(-1);
					mPathDistance = mCurrentPiece.EntryPath.GetLength() + num14;
					Player.FinishTurns();
					Player.RegisterTurn(TurnType.Left);
					RecentreWorld();
					mBranchSwitchTime = ((mCurrentPiece.Theme != WorldConstructionHelper.Theme.Minecart) ? 1f : 0.5f);
				}
				else if (GameEventHandler.HasInitiatedTurnRight())
				{
					mLookAheadDistance = 0f;
					mCurrentPiece.SwitchBranch(1);
					mPathDistance = mCurrentPiece.EntryPath.GetLength() + num14;
					Player.FinishTurns();
					Player.RegisterTurn(TurnType.Right);
					RecentreWorld();
					mBranchSwitchTime = ((mCurrentPiece.Theme != WorldConstructionHelper.Theme.Minecart) ? 1f : 0.5f);
				}
				if (GameEventHandler.HasInitiatedSwipeLeft())
				{
					Player.Buffer(PlayerController.SwipeBuffer.Left);
					GameEventHandler.SetInitiatedSwipeLeft(false);
				}
				else if (GameEventHandler.HasInitiatedSwipeRight())
				{
					Player.Buffer(PlayerController.SwipeBuffer.Right);
					GameEventHandler.SetInitiatedSwipeRight(false);
				}
				if (GameEventHandler.HasInitiatedSlide())
				{
					Player.Swipe(PlayerController.SwipeBuffer.Down);
					GameEventHandler.SetInitiatedSlide(false);
				}
				else if (GameEventHandler.HasInitiatedJump())
				{
					Player.Swipe(PlayerController.SwipeBuffer.Up);
					GameEventHandler.SetInitiatedJump(false);
				}
			}
			else
			{
				Player.Kill(mCurrentPiece);
			}
			num -= 1f;
		}
		SetPlayerPositionFromLeftRightValue(forward);
		if (mCurrentPiece != null)
		{
			float b = PlayerLookAheadDistance;
			if (mCurrentPiece.GetNextTheme(false, false) == WorldConstructionHelper.Theme.Mountain)
			{
				b = 5f;
			}
			mLookAheadDistance = Mathf.Min(mLookAheadDistance + 2f * Time.deltaTime, b);
			float num15 = Mathf.Clamp01((mLookAheadDistance - 1.5f) / 9.5f);
			PlayerLookAhead.transform.position = (mCurrentPiece.GetWorldPosition(mPathDistance + mLookAheadDistance, Player.GetLeftRight()) + mCurrentPiece.GetWorldPosition(mPathDistance + (mLookAheadDistance + num15 * 1f), Player.GetLeftRight()) + mCurrentPiece.GetWorldPosition(mPathDistance + (mLookAheadDistance + num15 * 2f), Player.GetLeftRight())) / 3f;
			if (ChaseDude != null)
			{
				float leftRight3 = 0.5f * (mLeftBound + mRightBound);
				ChaseDude.transform.position = mCurrentPiece.GetWorldPositionAutoBranch(mPathDistance + PlayerLookAheadDistance, leftRight3);
				ChaseDude.transform.forward = mCurrentPiece.GetWorldDirectionAutoBranch(mPathDistance + PlayerLookAheadDistance, leftRight3);
			}
			if (SmoothedTarget != null)
			{
				mCurrentPiece.EvaluatePath(mPathDistance, mCurrentPiece.SelectedBranch, delegate(PieceDescriptor piece, TileDescriptor path, float distance)
				{
					float length = path.GetLength();
					Vector3 worldPosition = path.GetWorldPosition(0f, Player.GetLeftRight());
					Vector3 worldPosition2 = path.GetWorldPosition(length, Player.GetLeftRight());
					SmoothedTarget.transform.position = Vector3.Lerp(worldPosition, worldPosition2, distance / length);
					SmoothedTarget.transform.forward = (worldPosition2 - worldPosition).normalized;
				});
			}
			float lightAngleThreshold = LightAngleThreshold;
			float b2 = Time.deltaTime * LightChangeSpeed;
			Quaternion quaternion = Quaternion.Euler(CameraTarget.transform.eulerAngles + mLightAngles);
			float num16 = Mathf.Abs(Quaternion.Angle(MainLight.transform.rotation, quaternion));
			if (num16 > lightAngleThreshold)
			{
				float maxDegreesDelta = Mathf.Min(num16 - lightAngleThreshold, b2);
				MainLight.transform.rotation = Quaternion.RotateTowards(MainLight.transform.rotation, quaternion, maxDegreesDelta);
			}
		}
		UpdatePathCentreAtCurrentPoint();
	}

	public void RecoverInTutorial()
	{
		base.gameObject.SetActiveRecursively(true);
		LevelGeneratorTutorialHelper.instance.Reset(true);
		ResetTutorialContinue();
		Player.Reset();
		CameraManager.Instance.ResetCameraTutorial();
		GameTutorial.Instance.Notify_TryAgain();
	}

	public void RecoverAfterRessurection()
	{
		PieceDescriptor pieceDescriptor = null;
		if (mPreviousPieces.Count > 0)
		{
			pieceDescriptor = mPreviousPieces[mPreviousPieces.Count - 1];
		}
		mFoundRopeObject = false;
		mCurrentRopeNode = null;
		if ((bool)pieceDescriptor && WorldConstructionHelper.IsJunction(pieceDescriptor.GetPieceType()))
		{
			bool flag = pieceDescriptor.HasAnyTileOfType(Tile.ResponseType.SwipeLeft);
			mCurrentPiece = pieceDescriptor;
			mPreviousPieces.Remove(mCurrentPiece);
			mPathDistance = mCurrentPiece.GetLength();
			mCurrentPiece.SwitchBranch((!flag) ? 1 : (-1));
			Player.FinishTurns();
			RecentreWorld();
		}
		else if (mCurrentPiece == null)
		{
			if (pieceDescriptor != null)
			{
				mCurrentPiece = pieceDescriptor;
				mPreviousPieces.Remove(mCurrentPiece);
				mPathDistance = 0f;
			}
		}
		else
		{
			while (WorldConstructionHelper.IsMultiTilePieceExcludingEntry(mCurrentPiece.TypeId) || WorldConstructionHelper.IsMultiTilePieceEntry(mCurrentPiece.TypeId))
			{
				Advance(mCurrentPiece.GetCachedLength());
				if (mCurrentPiece == null)
				{
					if (pieceDescriptor != null)
					{
						mCurrentPiece = pieceDescriptor;
						mPreviousPieces.Remove(mCurrentPiece);
						mPathDistance = 0f;
					}
					break;
				}
			}
			GameController.Instance.SetupResurrectionCamera();
		}
		CameraManager.Instance.RestoreFromResurrection();
	}

	public void Advance(float distance)
	{
		mCurrentPiece.FireTriggers(mPathDistance, mPathDistance + distance, Player.GetLeftRight(), Player.GetCurrentSpeed());
		mPathDistance += distance;
		float length = mCurrentPiece.GetLength();
		bool flag = false;
		if (mCurrentPiece.PathEndsEarly)
		{
			Tile tile = mCurrentPiece.GetTile(mPathDistance, Player.GetLeftRight());
			if (tile != null && tile.Types.Contains(Tile.ResponseType.Kill))
			{
				flag = true;
			}
		}
		while (mPathDistance > length || flag)
		{
			mPreviousPieces.Add(mCurrentPiece);
			mPathDistance -= length;
			PieceDescriptor pieceDescriptor = mCurrentPiece;
			mCurrentPiece = mCurrentPiece.GetNextPiece();
			if (mCurrentPiece == null)
			{
				return;
			}
			CheckXPActions(pieceDescriptor);
			ResetCurrentThemeSettings(false);
			if (pieceDescriptor != null && (pieceDescriptor.BranchLeftPath != null || pieceDescriptor.BranchRightPath != null) && pieceDescriptor.GetStraightAheadPiece() == mCurrentPiece)
			{
				pieceDescriptor.SetWorldControllerBranchSwitch(0);
			}
			if (pieceDescriptor != null)
			{
				UpdateDeathReport(pieceDescriptor);
			}
		}
		switch (Time.frameCount % 4)
		{
		case 0:
			mCurrentPiece.GenerateNextPieces(GENERATION_DEPTH, 0);
			break;
		case 1:
			UpdateRopeObject();
			break;
		case 2:
			SpawnCoins();
			break;
		default:
			UpdateCheckPointObject();
			break;
		}
	}

	public void CheckXPActions(PieceDescriptor piece)
	{
		if ((bool)piece)
		{
			if (piece.GetPieceType() == WorldConstructionHelper.PieceType.RopeSwing_Exit)
			{
				Player.Score().RegisterEvent_RopeSwing();
			}
			else if (piece.IsBaddiePiece())
			{
				Player.Score().RegisterEvent_PassedEnemy(piece.BaddieType);
			}
			else if (piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type1_Exit || piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type2_Exit || piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type3_Exit || piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type4_Exit || piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type5_Exit || piece.GetPieceType() == WorldConstructionHelper.PieceType.Pit_Type6_Exit)
			{
				Player.Score().RegisterEvent_AvoidPit();
			}
		}
	}

	public void CleanPreviousRun()
	{
		foreach (PieceDescriptor mActivePiece in mActivePieces)
		{
			if (ThemeManager.UnloadThemeAsset(mActivePiece.Theme))
			{
				Component[] componentsInChildren = mActivePiece.GetComponentsInChildren(typeof(MeshFilter), true);
				Component[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					MeshFilter meshFilter = (MeshFilter)array[i];
					if (!meshFilter.name.Contains("ropeNode"))
					{
						Resources.UnloadAsset(meshFilter.sharedMesh);
					}
				}
			}
			Object.Destroy(mActivePiece.gameObject);
		}
		mActivePieces.Clear();
		foreach (PieceDescriptor mPreviousPiece in mPreviousPieces)
		{
			if (ThemeManager.UnloadThemeAsset(mPreviousPiece.Theme))
			{
				Component[] componentsInChildren2 = mPreviousPiece.GetComponentsInChildren(typeof(MeshFilter), true);
				Component[] array2 = componentsInChildren2;
				for (int j = 0; j < array2.Length; j++)
				{
					MeshFilter meshFilter2 = (MeshFilter)array2[j];
					if (!meshFilter2.name.Contains("ropeNode"))
					{
						Resources.UnloadAsset(meshFilter2.sharedMesh);
					}
				}
			}
			Object.Destroy(mPreviousPiece.gameObject);
		}
		mPreviousPieces.Clear();
	}

	public void Reset()
	{
		CleanPreviousRun();
		activePDHashTable.Clear();
		mLastTile = null;
		mActivePieces.Clear();
		CheckPointController.Instance().Reset();
		AuthoredLevelController.Instance().Reset();
		Generate();
		GameEventHandler.Reset();
		Player.SetLeftRight(0f);
		SetPlayerPositionFromLeftRightValue(Player.transform.forward);
		PlayerLookAhead.transform.position = Player.transform.position + Player.transform.forward * PlayerLookAheadDistance;
		ResetLight();
		mPreviousTheme = mCurrentPiece.Theme;
		mLeftBound = -1.5f;
		mRightBound = 1.5f;
		mPlayerLeftBound = -1.5f;
		mPlayerRightBound = 1.5f;
		PathWidthDirty = true;
		PathCentreDirty = true;
		mFoundRopeObject = false;
		mCurrentRopeNode = null;
		Player.mHingedRope.SetState(HingedRope.RopeState.WaitForNode);
		GameEventHandler.ResetAheadHazard();
	}

	public void ResetTutorialContinue()
	{
		activePDHashTable.Clear();
		mLastTile = null;
		mActivePieces.Clear();
		CheckPointController.Instance().Reset();
		AuthoredLevelController.Instance().Reset();
		GenerateFromLastFail();
		GameEventHandler.Reset();
		Player.SetLeftRight(0f);
		SetPlayerPositionFromLeftRightValue(Player.transform.forward);
		PlayerLookAhead.transform.position = Player.transform.position + Player.transform.forward * PlayerLookAheadDistance;
		ResetLight();
		mPreviousTheme = mCurrentPiece.Theme;
		mLeftBound = -1.5f;
		mRightBound = 1.5f;
		mPlayerLeftBound = -1.5f;
		mPlayerRightBound = 1.5f;
		mFoundRopeObject = false;
		mCurrentRopeNode = null;
		Player.mHingedRope.SetState(HingedRope.RopeState.WaitForNode);
	}

	public void GenerateFromLastFail()
	{
		if (mCurrentPiece != null)
		{
			mCurrentPiece.DestroyRecursive();
		}
		CleanupPreviousPieces(0f);
		mPathDistance = 0f;
		PieceDescriptor pieceDescriptor = null;
		AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
		if (authoredLevelController != null && authoredLevelController.IsGlobalWorldActive())
		{
			pieceDescriptor = authoredLevelController.CreateNextPiece(LevelGeneratorTutorialHelper.instance.m_nLastSectionID, -1, 0);
			if (pieceDescriptor == null)
			{
				authoredLevelController.SetGlobalWorld(null);
				Debug.LogError("Tutorial code fail");
			}
		}
		else
		{
			Debug.LogError("Tutorial code fail");
		}
		pieceDescriptor.gameObject.SetActiveRecursively(true);
		pieceDescriptor.Generator = this;
		mActivePieces.Add(pieceDescriptor);
		AddPieceDescToHashTable(pieceDescriptor);
		LevelGeneratorTutorialHelper.instance.CheckForTutorialHelper(pieceDescriptor);
		mCurrentPiece = pieceDescriptor;
		mCurrentPiece.transform.parent = base.transform;
		mCurrentPiece.Facing = PieceDescriptor.Compass.North;
		mCurrentPiece.TotalDistance = mCheckpointStartDistance;
		mCurrentPiece.CurrentCheckPointNum = mCheckpointStartNum;
		ResetCheckpointStart();
		mCurrentPiece.GenerateNextPieces(GENERATION_DEPTH, 0);
	}

	public void SetCheckpointRestart(float startDistance, int startCheckpoint)
	{
		mCheckpointStartDistance = startDistance;
		mCheckpointStartNum = startCheckpoint;
		GameEventHandler.ResetAheadHazard();
	}

	public void Generate()
	{
		if (mCurrentPiece != null)
		{
			mCurrentPiece.DestroyRecursive();
		}
		CleanupPreviousPieces(0f);
		mPathDistance = 0f;
		mCurrentPiece = GeneratePiece(StartTheme, StartGroup, null, 0);
		if (mCurrentPiece == null)
		{
			Debug.LogError("NULL PIECE GENERATED");
		}
		mCurrentPiece.transform.parent = base.transform;
		mCurrentPiece.Facing = PieceDescriptor.Compass.North;
		mCurrentPiece.TotalDistance = mCheckpointStartDistance;
		mCurrentPiece.CurrentCheckPointNum = mCheckpointStartNum;
		ResetCheckpointStart();
		mCurrentPiece.GenerateNextPieces(GENERATION_DEPTH, 0);
		SpawnCoins();
	}

	public PieceDescriptor GeneratePiece(WorldConstructionHelper.Theme parentTheme, WorldConstructionHelper.Group parentGroup, PieceDescriptor parentPiece, int parentExitBranch)
	{
		if (!ThemeManager.Instance.HasThemeLoaded(parentTheme))
		{
			return null;
		}
		PieceDescriptor pieceDescriptor = null;
		AuthoredLevelController authoredLevelController = AuthoredLevelController.Instance();
		if (authoredLevelController != null && authoredLevelController.IsGlobalWorldActive())
		{
			int parentSection = 0;
			int parentElement = -1;
			if (parentPiece != null && (parentPiece.AuthoredSection != -1 || parentPiece.AuthoredElement != -1))
			{
				parentSection = parentPiece.AuthoredSection;
				parentElement = parentPiece.AuthoredElement;
			}
			pieceDescriptor = authoredLevelController.CreateNextPiece(parentSection, parentElement, parentExitBranch);
			if (pieceDescriptor == null)
			{
				authoredLevelController.SetGlobalWorld(null);
			}
		}
		if (pieceDescriptor == null)
		{
			pieceDescriptor = WorldConstructionController.Instance().CreateNextPiece(ThemeManager.Instance.LoadedPieces, parentTheme, parentGroup, Player.IsInSpeedBoostBonus() && Player.SpeedBoostAffectsSpeed(), parentPiece, parentExitBranch, m_currentGameType);
		}
		pieceDescriptor.gameObject.SetActiveRecursively(true);
		pieceDescriptor.Generator = this;
		mActivePieces.Add(pieceDescriptor);
		AddPieceDescToHashTable(pieceDescriptor);
		LevelGeneratorTutorialHelper.instance.CheckForTutorialHelper(pieceDescriptor);
		return pieceDescriptor;
	}

	public void RemoveActivePiece(PieceDescriptor piece)
	{
		RemovePieceDescFromHashTableAndDeleteMesh(piece);
		mActivePieces.Remove(piece);
	}

	public bool ThemeIsBeingUsed(WorldConstructionHelper.Theme PieceTheme)
	{
		bool result = false;
		foreach (PieceDescriptor mActivePiece in mActivePieces)
		{
			if (mActivePiece.Theme == PieceTheme)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void UpdateDeathReport(PieceDescriptor piece)
	{
		string item = piece.name;
		DeathReport.Add(item);
		if (DeathReport.Count > DEATH_REPORT_LOG_SIZE)
		{
			DeathReport.RemoveAt(0);
		}
	}

	private void ResetCheckpointStart()
	{
		mCheckpointStartDistance = 0f;
		mCheckpointStartNum = 0;
	}

	private static bool IsValidForCoinStart(PieceDescriptor piece)
	{
		if (WorldConstructionHelper.IsMultiTilePieceExcludingEntry(piece.TypeId))
		{
			return false;
		}
		return true;
	}

	private void SearchCoinStartPieces(PieceDescriptor startPiece, List<PieceDescriptor> startPieces)
	{
		if (startPiece == null)
		{
			MonoBehaviour.print("ERROR - shouldn't be calling SearchCoinStartPieces with null PieceDescriptor");
		}
		else if (startPiece.CoinsRegistered)
		{
			PieceDescriptor straightAheadPiece = startPiece.GetStraightAheadPiece();
			if (straightAheadPiece != null)
			{
				if (straightAheadPiece.CoinsRegistered)
				{
					SearchCoinStartPieces(straightAheadPiece, startPieces);
				}
				else
				{
					startPieces.Add(straightAheadPiece);
				}
			}
			PieceDescriptor branchLeftPiece = startPiece.GetBranchLeftPiece();
			if (branchLeftPiece != null)
			{
				if (branchLeftPiece.CoinsRegistered)
				{
					SearchCoinStartPieces(branchLeftPiece, startPieces);
				}
				else
				{
					startPieces.Add(branchLeftPiece);
				}
			}
			PieceDescriptor branchRightPiece = startPiece.GetBranchRightPiece();
			if (branchRightPiece != null)
			{
				if (branchRightPiece.CoinsRegistered)
				{
					SearchCoinStartPieces(branchRightPiece, startPieces);
				}
				else
				{
					startPieces.Add(branchRightPiece);
				}
			}
		}
		else
		{
			startPieces.Add(startPiece);
		}
	}

	private void SearchCoinRunPieces(PieceDescriptor checkPiece, List<PieceDescriptor> coinRunPieces)
	{
		if (checkPiece == null)
		{
			MonoBehaviour.print("ERROR - shouldn't be calling SearchCoinRunPieces with null PieceDescriptor");
			return;
		}
		float distanceToNextCoinRun = checkPiece.DistanceToNextCoinRun;
		distanceToNextCoinRun = (checkPiece.DistanceToNextCoinRun = distanceToNextCoinRun - checkPiece.GetCachedLength());
		if (distanceToNextCoinRun <= 0f)
		{
			coinRunPieces.Add(checkPiece);
			return;
		}
		checkPiece.CoinsRegistered = true;
		PieceDescriptor straightAheadPiece = checkPiece.GetStraightAheadPiece();
		if (straightAheadPiece != null)
		{
			straightAheadPiece.DistanceToNextCoinRun = distanceToNextCoinRun;
			SearchCoinRunPieces(straightAheadPiece, coinRunPieces);
		}
		PieceDescriptor branchLeftPiece = checkPiece.GetBranchLeftPiece();
		if (branchLeftPiece != null)
		{
			branchLeftPiece.DistanceToNextCoinRun = distanceToNextCoinRun;
			SearchCoinRunPieces(branchLeftPiece, coinRunPieces);
		}
		PieceDescriptor branchRightPiece = checkPiece.GetBranchRightPiece();
		if (branchRightPiece != null)
		{
			branchRightPiece.DistanceToNextCoinRun = distanceToNextCoinRun;
			SearchCoinRunPieces(branchRightPiece, coinRunPieces);
		}
	}

	private void SpawnCoins()
	{
		if (GameTutorial.Instance.IsEnabled || mCurrentPiece == null)
		{
			return;
		}
		PickupController pickupController = PickupController.Instance();
		startPieces.Clear();
		SearchCoinStartPieces(mCurrentPiece, startPieces);
		foreach (PieceDescriptor startPiece in startPieces)
		{
			coinRunPieces.Clear();
			SearchCoinRunPieces(startPiece, coinRunPieces);
			foreach (PieceDescriptor coinRunPiece in coinRunPieces)
			{
				coinRunPiece.CoinsRegistered = true;
				coinRunPiece.CoinsDebugInfo = string.Empty;
				if (!IsValidForCoinStart(coinRunPiece))
				{
					coinRunPiece.CoinsDebugInfo += " !!Tried to start coins from here but piece unsuitable!!";
					return;
				}
				coinRunPiece.DistanceToNextCoinRun = pickupController.DistanceBetweenCoinRuns;
				coinRunPiece.CoinsDebugInfo += " [COIN RUN START]";
				PieceDescriptor straightAheadPiece = coinRunPiece.GetStraightAheadPiece();
				if (straightAheadPiece != null)
				{
					straightAheadPiece.DistanceToNextCoinRun = pickupController.DistanceBetweenCoinRuns;
				}
				PieceDescriptor branchLeftPiece = coinRunPiece.GetBranchLeftPiece();
				if (branchLeftPiece != null)
				{
					branchLeftPiece.DistanceToNextCoinRun = pickupController.DistanceBetweenCoinRuns;
				}
				PieceDescriptor branchRightPiece = coinRunPiece.GetBranchRightPiece();
				if (branchRightPiece != null)
				{
					branchRightPiece.DistanceToNextCoinRun = pickupController.DistanceBetweenCoinRuns;
				}
				pickupController.SpawnNewPieceCoins(coinRunPiece);
			}
		}
	}

	private void ResetLight()
	{
		if (Player != null && MainLight != null)
		{
			MainLight.transform.eulerAngles = CameraTarget.transform.eulerAngles + mLightAngles;
		}
	}

	private float GetPreviousPiecesCombinedLength()
	{
		float num = 0f;
		for (int i = 0; i < mPreviousPieces.Count; i++)
		{
			num += mPreviousPieces[i].GetCachedLength();
		}
		return num;
	}

	public void CleanupPreviousPieces(float maxLengthOfPreviousPieces)
	{
		while (GetPreviousPiecesCombinedLength() > maxLengthOfPreviousPieces)
		{
			bool flag = false;
			if (mPreviousPieces[0] != null)
			{
				if (maxLengthOfPreviousPieces != 0f)
				{
					MeshFilter[] componentsInChildren = mPreviousPieces[0].gameObject.GetComponentsInChildren<MeshFilter>();
					MeshFilter[] array = componentsInChildren;
					foreach (MeshFilter meshFilter in array)
					{
						if (meshFilter.GetComponent<Renderer>().isVisible)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
				if (mPreviousPieces[0].TypeId == WorldConstructionHelper.PieceType.Pickup)
				{
					WorldConstructionController.Instance().ResetPickupState();
				}
				mPreviousPieces[0].TrimDeadBranches();
				mActivePieces.Remove(mPreviousPieces[0]);
				RemovePieceDescFromHashTableAndDeleteMesh(mPreviousPieces[0]);
				Object.Destroy(mPreviousPieces[0].gameObject);
			}
			mPreviousPieces.RemoveAt(0);
		}
	}

	private void SetVisible(GameObject piece, bool Visible)
	{
		Renderer[] componentsInChildren = piece.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = Visible;
		}
	}

	public void SetPiecesVisibility(bool Visible)
	{
		foreach (PieceDescriptor mActivePiece in mActivePieces)
		{
			if (!Visible)
			{
				mActivePiece.ClearTriggers();
			}
			SetVisible(mActivePiece.gameObject, Visible);
		}
		foreach (PieceDescriptor mPreviousPiece in mPreviousPieces)
		{
			if (!Visible)
			{
				mPreviousPiece.ClearTriggers();
			}
			SetVisible(mPreviousPiece.gameObject, Visible);
		}
	}

	public Tile GetTileUnderPlayer()
	{
		if (mCurrentPiece != null)
		{
			return mCurrentPiece.GetTile(mPathDistance, Player.GetLeftRight());
		}
		return null;
	}

	public PathMaterial GetPathMaterialUnderPlayer()
	{
		if (mCurrentPiece != null)
		{
			return mCurrentPiece.GetPathMaterial(mPathDistance, Player.GetLeftRight());
		}
		return PathMaterial.None;
	}

	private void UpdatePlayerPerception(TileLookaheadCache cache, int range)
	{
		cache.Reset();
		PieceDescriptor nextPiece = mCurrentPiece;
		float num = mPathDistance;
		float num2 = 0f;
		while (nextPiece != null)
		{
			Tile.ResponseType responseType = Tile.ResponseType.Empty;
			Tile.ResponseType responseType2 = Tile.ResponseType.Empty;
			Tile.ResponseType responseType3 = Tile.ResponseType.Empty;
			Tile.ResponseType secondaryType = Tile.ResponseType.Empty;
			Tile.ResponseType secondaryType2 = Tile.ResponseType.Empty;
			Tile.ResponseType secondaryType3 = Tile.ResponseType.Empty;
			while (responseType != Tile.ResponseType.EndOfPath || responseType2 != Tile.ResponseType.EndOfPath || responseType3 != Tile.ResponseType.EndOfPath)
			{
				responseType = GetPerceptedTileType(nextPiece, num, -1f, out secondaryType);
				responseType2 = GetPerceptedTileType(nextPiece, num, 1f, out secondaryType2);
				responseType3 = GetPerceptedTileType(nextPiece, num, 0f, out secondaryType3);
				if (responseType != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(responseType, num2);
				}
				if (responseType2 != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(responseType2, num2);
				}
				if (responseType3 != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(responseType3, num2);
				}
				if (secondaryType != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(secondaryType, num2);
				}
				if (secondaryType2 != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(secondaryType2, num2);
				}
				if (secondaryType3 != Tile.ResponseType.Empty)
				{
					cache.RegisterEvent(secondaryType3, num2);
				}
				num += 2f;
				num2 += 2f;
				range--;
				if (range <= 0)
				{
					return;
				}
			}
			nextPiece = nextPiece.GetNextPiece();
			num = 0f;
		}
	}

	private Tile.ResponseType GetPerceptedTileType(PieceDescriptor currentSearchPiece, float currentDistance, float leftRight, out Tile.ResponseType secondaryType)
	{
		Tile.ResponseType responseType = (secondaryType = Tile.ResponseType.Empty);
		Tile tile = currentSearchPiece.GetTile(currentDistance, leftRight);
		if (tile != null)
		{
			for (int i = 0; i < tile.Types.Count; i++)
			{
				if (tile.Types[i] == Tile.ResponseType.SwipeUp)
				{
					if (responseType == Tile.ResponseType.Empty)
					{
						responseType = Tile.ResponseType.SwipeUp;
					}
					else
					{
						secondaryType = Tile.ResponseType.SwipeUp;
					}
				}
				else if (tile.Types[i] == Tile.ResponseType.SwipeDown)
				{
					if (responseType == Tile.ResponseType.Empty)
					{
						responseType = Tile.ResponseType.SwipeDown;
					}
					else
					{
						secondaryType = Tile.ResponseType.SwipeDown;
					}
				}
				else if (tile.Types[i] == Tile.ResponseType.SwipeLeft)
				{
					if (responseType == Tile.ResponseType.Empty)
					{
						responseType = Tile.ResponseType.SwipeLeft;
					}
					else
					{
						secondaryType = Tile.ResponseType.SwipeLeft;
					}
				}
				else if (tile.Types[i] == Tile.ResponseType.SwipeRight)
				{
					if (responseType == Tile.ResponseType.Empty)
					{
						responseType = Tile.ResponseType.SwipeRight;
					}
					else
					{
						secondaryType = Tile.ResponseType.SwipeRight;
					}
				}
			}
		}
		else
		{
			responseType = Tile.ResponseType.EndOfPath;
		}
		return responseType;
	}

	private void ResetCurrentThemeSettings(bool fullReset)
	{
		WorldConstructionHelper.Theme theme;
		if (mCurrentPiece != null)
		{
			theme = mCurrentPiece.Theme;
			mPreviousTheme = mCurrentPiece.Theme;
		}
		else
		{
			theme = mPreviousTheme;
		}
		ThemeManager.Instance.SetRenderTheme(theme, fullReset);
		if (fullReset)
		{
			CameraManager.Instance.SwitchToCameraMode(CameraManager.Instance.DefaultCam, CameraTransitionData.JumpCut);
		}
	}

	public float GetPathWidthAtCurrentPoint()
	{
		PathCentreDirty = true;
		if (PathWidthDirty)
		{
			if (mCurrentPiece != null)
			{
				PathWidth = (mCurrentPiece.GetWorldPosition(mPathDistance, mRightBound) - mCurrentPiece.GetWorldPosition(mPathDistance, mLeftBound)).magnitude;
			}
			else
			{
				PathWidth = 0f;
			}
			PathWidthDirty = false;
		}
		return PathWidth;
	}

	public Vector3 GetPathCentreAtCurrentPoint()
	{
		return PathCentreAtCurrentPosition;
	}

	public void UpdatePathCentreAtCurrentPoint()
	{
		float leftRight = 0.5f * (mLeftBound + mRightBound);
		if (mCurrentPiece != null)
		{
			PathCentreAtCurrentPosition = Vector3.Lerp(mCurrentPiece.GetWorldPosition(mPathDistance + 0.01f, leftRight), Player.transform.position, mBranchSwitchTime);
		}
		else
		{
			PathCentreAtCurrentPosition = Vector3.zero;
		}
	}

	public Vector3 GetPathCentreAt(float leftRight)
	{
		return GetPathCentreAt(leftRight, 0f);
	}

	public Vector3 GetPathCentreAt(float leftRight, float additionalOffset)
	{
		PathCentreDirty = true;
		if (PathCentreDirty)
		{
			if (mCurrentPiece != null)
			{
				PathCentre = Vector3.Lerp(mCurrentPiece.GetWorldPosition(mPathDistance + 0.01f + additionalOffset, leftRight), Player.transform.position, mBranchSwitchTime);
			}
			else
			{
				PathCentre = Vector3.zero;
			}
			PathCentreDirty = false;
		}
		return PathCentre;
	}

	public bool IsInActiveList(PieceDescriptor pd)
	{
		return activePDHashTable.ContainsValue(pd.GetHashCodePD());
	}

	public void AddPieceDescToHashTable(PieceDescriptor pD)
	{
		if (!(pD == null) && (TBFUtils.Is256mbDevice() || pD.Theme == WorldConstructionHelper.Theme.Bike || pD.Theme == WorldConstructionHelper.Theme.Minecart) && !activePDHashTable.ContainsKey(pD.name))
		{
			activePDHashTable.Add(pD.name, pD.GetHashCodePD());
		}
	}

	public void RemovePieceDescFromHashTableAndDeleteMesh(PieceDescriptor pD)
	{
		if (pD == null || !ThemeManager.UnloadThemeAsset(pD.Theme) || !activePDHashTable.ContainsKey(pD.name))
		{
			return;
		}
		foreach (PieceDescriptor mActivePiece in mActivePieces)
		{
			if (mActivePiece.name == pD.name)
			{
				return;
			}
		}
		MeshFilter component = PickupController.Instance().CoinModel.GetComponent<MeshFilter>();
		MeshFilter component2 = PickupController.Instance().CoinLowValueModel.GetComponent<MeshFilter>();
		activePDHashTable.Remove(pD.name);
		if (IsInActiveList(pD))
		{
			return;
		}
		Component[] componentsInChildren = pD.GetComponentsInChildren(typeof(MeshFilter), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			MeshFilter meshFilter = (MeshFilter)array[i];
			if (!(component.sharedMesh == meshFilter.sharedMesh) && !(component2.sharedMesh == meshFilter.sharedMesh) && !meshFilter.name.Contains("ropeNode"))
			{
				Resources.UnloadAsset(meshFilter.sharedMesh);
			}
		}
	}

	public float GetCurrentPathDistTravelled()
	{
		return mPathDistance;
	}

	public PieceDescriptor FindPreviousPiece(PieceDescriptor currentPiece)
	{
		PieceDescriptor result = null;
		if (activePDHashTable.ContainsKey(currentPiece.name))
		{
			foreach (PieceDescriptor mActivePiece in mActivePieces)
			{
				if (mActivePiece == currentPiece || (!(mActivePiece.GetStraightAheadPiece() == currentPiece) && !(mActivePiece.GetBranchLeftPiece() == currentPiece) && !(mActivePiece.GetBranchRightPiece() == currentPiece)))
				{
					continue;
				}
				result = mActivePiece;
				break;
			}
		}
		return result;
	}

	public PieceDescriptor GetCurrentPiece()
	{
		return mCurrentPiece;
	}

	public List<PieceDescriptor> GetPreviousPieces()
	{
		return mPreviousPieces;
	}
}
