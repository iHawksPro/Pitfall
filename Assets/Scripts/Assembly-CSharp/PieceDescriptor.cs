using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceDescriptor : MonoBehaviour
{
	public enum Compass
	{
		North = 0,
		East = 1,
		South = 2,
		West = 3,
		Max = 4
	}

	public enum KillType
	{
		None = 0,
		Wall = 1,
		Pit = 2,
		Crocodile = 3,
		Poison = 4,
		Meteor = 5,
		PitShallow = 6,
		SlideWall = 7,
		FallEdgeDie_R = 8,
		FallEdgeDie_L = 9,
		FallCliffCornerDie = 10,
		DeathUndergrowth_L = 11,
		DeathUndergrowth_R = 12,
		DeathImpaleDoor = 13
	}

	public enum HasNode
	{
		Unsure = 0,
		No = 1,
		Yes = 2
	}

	public delegate int BranchSelectDelegate(PieceDescriptor piece);

	public delegate void EvaluatePathDelegate(PieceDescriptor piece, TileDescriptor path, float distance);

	public TileDescriptor EntryPath;

	public TileDescriptor StraightAheadPath;

	public TileDescriptor BranchLeftPath;

	public TileDescriptor BranchRightPath;

	public GameObject EntryAnchor;

	public GameObject StraightAheadAnchor;

	public GameObject BranchLeftAnchor;

	public GameObject BranchRightAnchor;

	public TileMarkup[] EntryMarkup;

	public TileMarkup[] StraightAheadMarkup;

	public TileMarkup[] BranchLeftMarkup;

	public TileMarkup[] BranchRightMarkup;

	public WorldConstructionHelper.Theme StraightAheadTheme;

	public WorldConstructionHelper.Theme BranchLeftTheme;

	public WorldConstructionHelper.Theme BranchRightTheme;

	public WorldConstructionHelper.PieceType TypeId;

	public WorldConstructionHelper.PieceTutorialType TutorialTypeId = WorldConstructionHelper.PieceTutorialType.Exclude;

	public float TutorialSpawnDistanceOveride;

	public WorldConstructionHelper.Group Group;

	public WorldConstructionHelper.Group GroupTransitionEntry;

	public WorldConstructionHelper.Group GroupTransitionExit;

	public WorldConstructionHelper.Theme Theme;

	public WorldConstructionHelper.Theme ThemeTransitionEntry;

	public int HazardRowStart;

	public int HazardRowEnd;

	public int Bend;

	public float MeteorFlightTime = 1f;

	public float AdditionalLengthBeforeThisPiece;

	public float AdditionalLengthAfterThisPiece;

	public WorldConstructionHelper.DifficultyType DifficultyCategory;

	public List<int> DifficultyGroups = new List<int> { 0, 40, 50, 60, 70, 80, 90, 100 };

	public int RarityFactor = 1;

	public bool FeaturePiece;

	public List<PieceDescriptor> DiscludeEntries = new List<PieceDescriptor>();

	public List<PieceDescriptor> DiscludeExits = new List<PieceDescriptor>();

	public bool CoinsRegistered;

	public bool CoinsSpawned;

	public string CoinsDebugInfo;

	public float DistanceToNextCoinRun;

	public BaddieController.Type BaddieType;

	public float TotalDistance;

	public int CurrentCheckPointNum;

	public int AuthoredSection = -1;

	public int AuthoredElement = -1;

	public bool PathEndsEarly;

	private int iHashTag;

	public int m_iBaseNameHash;

	public GameType m_specificGameType;

	[HideInInspector]
	public int mTimesUsedThisRun;

	private bool mSpawnMeteor;

	private static bool gfxtest = true;

	public Compass Facing;

	public string DebugInfo;

	public PieceTriggerPoint[] TriggerPoints;

	private PieceTriggerFunctions mTriggerFunctions;

	public KillType KilledBy;

	private static Shader mLowShadowShader;

	private static Shader mHighShadowShader;

	public HasNode mHasRopeNode;

	public HasNode mHasCheckPointNode;

	private bool mLoaded;

	private PieceDescriptor mBranchLeftPiece;

	private PieceDescriptor mStraightAheadPiece;

	private PieceDescriptor mBranchRightPiece;

	private int mBranchSwitch;

	private bool mDeferGeneration;

	private float mDeferredDepth;

	private int mDeferredBranch;

	private float mCachedLength;

	private bool mLengthCached;

	private float mCachedLeftLength;

	private float mCachedAheadLength;

	private float mCachedRightLength;

	private bool mCachedLeftLengthSet;

	private bool mCachedAheadLengthSet;

	private bool mCachedRightLengthSet;

	public static bool DrawGizmos = true;

	public LevelGenerator Generator { get; set; }

	public int GetHashCodePD()
	{
		return iHashTag;
	}

	public void Awake()
	{
		mDeferGeneration = false;
		if (PickupController.Instance() != null)
		{
			CoinsRegistered = false;
			CoinsSpawned = false;
			DistanceToNextCoinRun = PickupController.Instance().DistanceBetweenCoinRuns;
		}
		BaddieType = BaddieController.Type.Max;
		mTriggerFunctions = base.gameObject.GetComponent<PieceTriggerFunctions>() ?? ((TriggerPoints == null || TriggerPoints.Length <= 0) ? null : base.gameObject.AddComponent<PieceTriggerFunctions>());
		string text = base.name;
		bool flag = false;
		if (base.name.Contains("(Clone)"))
		{
			text = text.Replace("(Clone)", string.Empty);
			flag = true;
		}
		iHashTag = text.GetHashCode();
		if (!flag)
		{
			return;
		}
		if (SecureStorage.Instance.HasSetLowGFXOption)
		{
			if (mLowShadowShader == null)
			{
				mLowShadowShader = GameController.Instance.LowGFXShaderRef;
			}
			if (mHighShadowShader == null)
			{
				mHighShadowShader = Shader.Find("Pitfall/TilesWithShadow");
			}
			Component[] componentsInChildren = GetComponentsInChildren(typeof(MeshFilter), true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshFilter meshFilter = (MeshFilter)array[i];
				if (meshFilter.GetComponent<Renderer>().material.shader == mHighShadowShader)
				{
					meshFilter.GetComponent<Renderer>().material.shader = mLowShadowShader;
				}
			}
		}
		bool flag2 = GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode;
		TrialsCollectableRelic[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<TrialsCollectableRelic>();
		foreach (TrialsCollectableRelic trialsCollectableRelic in componentsInChildren2)
		{
			if (!flag2)
			{
				UnityEngine.Object.Destroy(trialsCollectableRelic.gameObject);
			}
		}
	}

	private void Update()
	{
		if (mDeferGeneration)
		{
			mDeferGeneration = false;
			GenerateNextPieces(mDeferredDepth, mDeferredBranch);
		}
	}

	public void FireTriggers(float start, float end, float leftRight, float speed)
	{
		FireTriggers(start, end, leftRight, speed, true);
	}

	public void ClearTriggers()
	{
		if (mTriggerFunctions != null)
		{
			mTriggerFunctions.StopLoopingSFX();
		}
	}

	private void FireTriggers(float start, float end, float leftRight, float speed, bool activeBranch)
	{
		float num = 10f;
		if (start < (0f - num) * speed)
		{
			return;
		}
		if (mTriggerFunctions != null)
		{
			PieceTriggerPoint[] triggerPoints = TriggerPoints;
			foreach (PieceTriggerPoint pieceTriggerPoint in triggerPoints)
			{
				if (pieceTriggerPoint.Fired)
				{
					continue;
				}
				float num2 = speed;
				if (GameController.Instance.IsPlayingTrialsMode && pieceTriggerPoint.MethodToInvoke == "ChangeTheme")
				{
					num2 = PlayerController.Instance().GetDefaultSpeedForTrials();
				}
				PieceTriggerParameters pieceTriggerParameters = new PieceTriggerParameters();
				pieceTriggerParameters.Param1 = pieceTriggerPoint.Param1;
				pieceTriggerParameters.Param2 = pieceTriggerPoint.Param2;
				float num3 = pieceTriggerPoint.Distance - num2 * pieceTriggerPoint.Time;
				if (!pieceTriggerPoint.IgnoreBranchSwitch && mBranchSwitch != (int)pieceTriggerPoint.Branch && !(num3 < EntryPath.GetLength()))
				{
					continue;
				}
				if (num3 < start && start < pieceTriggerPoint.Distance)
				{
					if (leftRight >= pieceTriggerPoint.GetLeftBound() && leftRight <= pieceTriggerPoint.GetRightBound())
					{
						pieceTriggerParameters.IsLate = true;
						mTriggerFunctions.SendMessage(pieceTriggerPoint.MethodToInvoke, pieceTriggerParameters, SendMessageOptions.DontRequireReceiver);
						pieceTriggerPoint.Fired = true;
					}
				}
				else if (num3 >= start && num3 < end && leftRight >= pieceTriggerPoint.GetLeftBound() && leftRight <= pieceTriggerPoint.GetRightBound())
				{
					mTriggerFunctions.SendMessage(pieceTriggerPoint.MethodToInvoke, pieceTriggerParameters, SendMessageOptions.DontRequireReceiver);
					pieceTriggerPoint.Fired = true;
				}
			}
		}
		if ((bool)mBranchLeftPiece)
		{
			float cachedBranchLength = GetCachedBranchLength(-1);
			mBranchLeftPiece.FireTriggers(start - cachedBranchLength, end - cachedBranchLength, leftRight, speed, activeBranch && mBranchSwitch == -1);
		}
		if ((bool)mStraightAheadPiece)
		{
			float cachedBranchLength2 = GetCachedBranchLength(0);
			mStraightAheadPiece.FireTriggers(start - cachedBranchLength2, end - cachedBranchLength2, leftRight, speed, activeBranch && mBranchSwitch == -1);
		}
		if ((bool)mBranchRightPiece)
		{
			float cachedBranchLength3 = GetCachedBranchLength(1);
			mBranchRightPiece.FireTriggers(start - cachedBranchLength3, end - cachedBranchLength3, leftRight, speed, activeBranch && mBranchSwitch == -1);
		}
	}

	public void OnLoad()
	{
		if (mLoaded)
		{
			return;
		}
		try
		{
			GameObject gameObject = base.transform.Find(base.name).gameObject;
			if (gameObject != null)
			{
				LinkPieceToModel(this, gameObject);
			}
			SetCachedLeftLength();
			SetCachedRightLength();
			SetCachedAheadLength();
		}
		catch (Exception ex)
		{
			Debug.Log(base.name);
			Debug.Log(ex.ToString());
			mLoaded = true;
		}
		if (EntryPath == null)
		{
			throw new Exception("Piece with no entry path: " + base.transform.name);
		}
		if (EntryPath.transform.localScale.x < 0f)
		{
			throw new Exception("Inverse Scale on EntryPath in: " + base.transform.name);
		}
		if (StraightAheadPath == null && BranchLeftPath == null && BranchRightPath == null)
		{
			throw new Exception("Piece with no exit path " + base.transform.name);
		}
		if (StraightAheadAnchor == null && BranchLeftAnchor == null && BranchRightAnchor == null)
		{
			throw new Exception("Piece with no exit anchor " + base.transform.name);
		}
		CreateTileMarkup(EntryMarkup);
		CreateTileMarkup(StraightAheadMarkup);
		CreateTileMarkup(BranchLeftMarkup);
		CreateTileMarkup(BranchRightMarkup);
		if (EntryPath != null)
		{
			InitialisePieceType(EntryPath, EntryMarkup);
		}
		if (StraightAheadPath != null)
		{
			InitialisePieceType(StraightAheadPath, StraightAheadMarkup);
		}
		if (BranchLeftPath != null)
		{
			InitialisePieceType(BranchLeftPath, BranchLeftMarkup);
		}
		if (BranchRightPath != null)
		{
			InitialisePieceType(BranchRightPath, BranchRightMarkup);
		}
		HazardRowStart = -1;
		HazardRowEnd = -1;
		if (EntryPath != null)
		{
			switch (TypeId)
			{
			case WorldConstructionHelper.PieceType.Duck:
				HazardRowStart = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeDown);
				HazardRowEnd = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeDown);
				break;
			case WorldConstructionHelper.PieceType.Jump:
			case WorldConstructionHelper.PieceType.JumpOrDuck:
			case WorldConstructionHelper.PieceType.Pit_Type1_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type1_Loop:
			case WorldConstructionHelper.PieceType.Pit_Type2_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type2_Loop:
			case WorldConstructionHelper.PieceType.Pit_Type3_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type3_Loop:
			case WorldConstructionHelper.PieceType.Pit_Type4_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type4_Loop:
			case WorldConstructionHelper.PieceType.Pit_Type5_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type5_Loop:
			case WorldConstructionHelper.PieceType.Pit_Type6_Entry:
			case WorldConstructionHelper.PieceType.Pit_Type6_Loop:
			case WorldConstructionHelper.PieceType.RopeSwing_Entry:
				HazardRowStart = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeUp);
				HazardRowEnd = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeUp);
				if (HazardRowStart == -1)
				{
					HazardRowStart = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeDown);
					HazardRowEnd = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeDown);
				}
				if (HazardRowStart == -1)
				{
					HazardRowStart = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.Kill);
					HazardRowEnd = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.Kill);
				}
				break;
			case WorldConstructionHelper.PieceType.Junction_L:
			case WorldConstructionHelper.PieceType.Junction_R:
			case WorldConstructionHelper.PieceType.Junction_T:
			case WorldConstructionHelper.PieceType.Junction_Optional_L:
			case WorldConstructionHelper.PieceType.Junction_Optional_R:
			case WorldConstructionHelper.PieceType.Junction_Optional_T:
			{
				int rowTileOfTypeStart3 = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeLeft);
				if (rowTileOfTypeStart3 != -1)
				{
					HazardRowStart = rowTileOfTypeStart3;
				}
				int rowTileOfTypeStart4 = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeRight);
				if (rowTileOfTypeStart4 != -1 && (rowTileOfTypeStart4 < HazardRowStart || HazardRowStart == -1))
				{
					HazardRowStart = rowTileOfTypeStart4;
				}
				int rowTileOfTypeEnd3 = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeLeft);
				if (rowTileOfTypeEnd3 != -1)
				{
					HazardRowEnd = rowTileOfTypeEnd3;
				}
				int rowTileOfTypeEnd4 = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeRight);
				if (rowTileOfTypeEnd4 != -1 && (rowTileOfTypeEnd4 > HazardRowEnd || HazardRowEnd == -1))
				{
					HazardRowEnd = rowTileOfTypeEnd4;
				}
				break;
			}
			case WorldConstructionHelper.PieceType.TrackReduction:
			{
				int rowTileOfTypeStart = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.SwipeUp);
				if (rowTileOfTypeStart != -1)
				{
					HazardRowStart = rowTileOfTypeStart;
				}
				int rowTileOfTypeStart2 = GetRowTileOfTypeStart(EntryMarkup, Tile.ResponseType.Kill);
				if (rowTileOfTypeStart2 != -1 && (rowTileOfTypeStart2 < HazardRowStart || HazardRowStart == -1))
				{
					HazardRowStart = rowTileOfTypeStart2;
				}
				int rowTileOfTypeEnd = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.SwipeUp);
				if (rowTileOfTypeEnd != -1)
				{
					HazardRowEnd = rowTileOfTypeEnd;
				}
				int rowTileOfTypeEnd2 = GetRowTileOfTypeEnd(EntryMarkup, Tile.ResponseType.Kill);
				if (rowTileOfTypeEnd2 != -1 && (rowTileOfTypeEnd2 > HazardRowEnd || HazardRowEnd == -1))
				{
					HazardRowEnd = rowTileOfTypeEnd2;
				}
				break;
			}
			}
		}
		InitialisePieceTypeBend(base.name);
		InitialisePieceTypeEarlyPathEnd();
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			animation.playAutomatically = false;
		}
		mCachedLength = GetLengthSlow();
		m_iBaseNameHash = base.name.GetHashCode();
		mLoaded = true;
	}

	public void OnDrawGizmos()
	{
		if (!DrawGizmos)
		{
			return;
		}
		Renderer componentInChildren = GetComponentInChildren<Renderer>();
		if (componentInChildren != null && (Camera.current.transform.position - componentInChildren.bounds.center).sqrMagnitude > 10000f)
		{
			return;
		}
		DrawPathGizmos(EntryPath, EntryMarkup);
		DrawPathGizmos((!(EntryPath != StraightAheadPath)) ? null : StraightAheadPath, StraightAheadMarkup);
		DrawPathGizmos(BranchLeftPath, BranchLeftMarkup);
		DrawPathGizmos(BranchRightPath, BranchRightMarkup);
		if (TriggerPoints != null)
		{
			PieceTriggerPoint[] triggerPoints = TriggerPoints;
			foreach (PieceTriggerPoint pieceTriggerPoint in triggerPoints)
			{
				Gizmos.color = Color.yellow;
				float leftBound = pieceTriggerPoint.GetLeftBound();
				float rightBound = pieceTriggerPoint.GetRightBound();
				Vector3 worldPosition = GetWorldPosition(pieceTriggerPoint.Distance, leftBound, (int)pieceTriggerPoint.Branch);
				Vector3 worldPosition2 = GetWorldPosition(pieceTriggerPoint.Distance, rightBound, (int)pieceTriggerPoint.Branch);
				Gizmos.DrawSphere(worldPosition, 0.2f);
				Gizmos.DrawSphere(worldPosition2, 0.2f);
				Gizmos.DrawLine(worldPosition, worldPosition2);
			}
		}
	}

	public void DrawPathGizmos(TileDescriptor path, TileMarkup[] markupArray)
	{
		if (path == null)
		{
			return;
		}
		if (markupArray == null || markupArray.Length != path.Rows.Count - 1)
		{
			throw new Exception(string.Format("Markup array length does not correspond to path length on piece {0}", base.name));
		}
		try
		{
			if (path.Rows.Count != 0)
			{
				Matrix4x4 localToWorldMatrix = path.transform.localToWorldMatrix;
				Vector3 vector = localToWorldMatrix.MultiplyPoint(path.Rows[0].Left0);
				Vector3 vector2 = localToWorldMatrix.MultiplyPoint(path.Rows[0].Left1);
				Vector3 vector3 = localToWorldMatrix.MultiplyPoint(path.Rows[0].Centre0);
				Vector3 vector4 = localToWorldMatrix.MultiplyPoint(path.Rows[0].Centre1);
				Vector3 vector5 = localToWorldMatrix.MultiplyPoint(path.Rows[0].Right0);
				Vector3 vector6 = localToWorldMatrix.MultiplyPoint(path.Rows[0].Right1);
				DrawStartPoints(vector, vector2, vector3, vector4, vector5, vector6);
				for (int i = 1; i < path.Rows.Count; i++)
				{
					Vector3 vector7 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Left0);
					Vector3 vector8 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Left1);
					Vector3 vector9 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Centre0);
					Vector3 vector10 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Centre1);
					Vector3 vector11 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Right0);
					Vector3 vector12 = localToWorldMatrix.MultiplyPoint(path.Rows[i].Right1);
					Gizmos.color = Color.black;
					DrawEdgeBlock(vector, vector2, 0.05f);
					DrawEdgeBlock(vector3, vector4, 0.05f);
					DrawEdgeBlock(vector5, vector6, 0.05f);
					Gizmos.color = Color.white;
					DrawEdgeBlock(vector, vector7, 0.05f);
					Gizmos.color = Color.black;
					DrawEdgeBlock(vector2, vector8, 0.05f);
					DrawEdgeBlock(vector3, vector9, 0.05f);
					DrawEdgeBlock(vector4, vector10, 0.05f);
					DrawEdgeBlock(vector5, vector11, 0.05f);
					Gizmos.color = Color.black;
					DrawEdgeBlock(vector6, vector12, 0.05f);
					DrawTileTypes(vector, vector2, vector7, vector8, markupArray[i - 1].LeftType);
					DrawTileTypes(vector3, vector4, vector9, vector10, markupArray[i - 1].CentreType);
					DrawTileTypes(vector5, vector6, vector11, vector12, markupArray[i - 1].RightType);
					vector = vector7;
					vector2 = vector8;
					vector3 = vector9;
					vector4 = vector10;
					vector5 = vector11;
					vector6 = vector12;
				}
				DrawEndPoints(vector, vector2, vector3, vector4, vector5, vector6);
			}
		}
		catch (Exception)
		{
			Debug.Log("Bad Piece: " + base.name + " Path:" + path.name);
		}
	}

	public void DrawTileTypes(Vector3 a, Vector3 b, Vector3 c, Vector3 d, string typeString)
	{
		Vector3 tileCenter = TileDescriptor.GetTileCenter(a, b, c, d);
		string[] array = typeString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.ToLowerInvariant() == "down")
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere(tileCenter + Vector3.up * 0.1f, 0.1f);
			}
			else if (text.ToLowerInvariant() == "up")
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(tileCenter + Vector3.up * 0.3f, 0.1f);
			}
			else if (text.ToLowerInvariant() == "left" || text.ToLowerInvariant() == "right")
			{
				Gizmos.color = Color.green;
				Vector3 edgeCenter = TileDescriptor.GetEdgeCenter(a, c);
				Vector3 normalized = (edgeCenter - tileCenter).normalized;
				Gizmos.DrawLine(tileCenter + 0.1f * normalized, tileCenter + 0.3f * normalized);
			}
			else if (text.ToLowerInvariant() == "rope")
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawSphere(tileCenter + Vector3.up * 0.3f, 0.1f);
			}
			else if (text.ToLowerInvariant() == "kill")
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(tileCenter + Vector3.up * 0.3f, 0.1f);
			}
			else if (text.ToLowerInvariant() == "kill_left")
			{
				Gizmos.color = new Color(1f, 0f, 0f, 1f);
				DrawEdgeBlock(a, c, 1f);
			}
			else if (text.ToLowerInvariant() == "kill_right")
			{
				Gizmos.color = new Color(1f, 0f, 0f, 1f);
				DrawEdgeBlock(b, d, 1f);
			}
			else if (text.ToLowerInvariant() == "block_left")
			{
				Gizmos.color = new Color(0f, 0f, 0f, 1f);
				DrawEdgeBlock(a, c, 1f);
			}
			else if (text.ToLowerInvariant() == "block_right")
			{
				Gizmos.color = new Color(0f, 0f, 0f, 1f);
				DrawEdgeBlock(b, d, 1f);
			}
		}
	}

	public void DrawEdgeBlock(Vector3 a, Vector3 b, float height)
	{
		Vector3 vector = b - a;
		Matrix4x4 matrix = Gizmos.matrix;
		Matrix4x4 identity = Matrix4x4.identity;
		Vector3 up = Vector3.up;
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = Vector3.Cross(up, normalized);
		identity.SetColumn(0, vector2);
		identity.SetColumn(1, up);
		identity.SetColumn(2, normalized);
		identity.SetColumn(3, TileDescriptor.GetEdgeCenter(a, b));
		Gizmos.matrix = identity;
		Gizmos.DrawCube(new Vector3(-0.05f, 0.5f * height, 0f), new Vector3(0.1f, height, vector.magnitude));
		Gizmos.matrix = matrix;
	}

	public void DrawStartPoints(Vector3 left0, Vector3 left1, Vector3 centre0, Vector3 centre1, Vector3 right0, Vector3 right1)
	{
		Gizmos.color = Color.white;
		DrawEdgePoint(left0, left1);
		DrawEdgePoint(centre0, centre1);
		DrawEdgePoint(right0, right1);
	}

	public void DrawEndPoints(Vector3 left0, Vector3 left1, Vector3 centre0, Vector3 centre1, Vector3 right0, Vector3 right1)
	{
		Gizmos.color = Color.black;
		DrawEdgePoint(left0, left1);
		DrawEdgePoint(centre0, centre1);
		DrawEdgePoint(right0, right1);
	}

	private void DrawEdgePoint(Vector3 a, Vector3 b)
	{
		Gizmos.DrawSphere(TileDescriptor.GetEdgeCenter(a, b), 0.05f);
	}

	public bool HasAnyTileOfType(Tile.ResponseType responseType)
	{
		bool wholeRow = false;
		if (EntryPath != null && IsAnyTileOfType(EntryMarkup, responseType, out wholeRow))
		{
			return true;
		}
		if (StraightAheadPath != null && IsAnyTileOfType(StraightAheadMarkup, responseType, out wholeRow))
		{
			return true;
		}
		if (BranchLeftPath != null && IsAnyTileOfType(BranchLeftMarkup, responseType, out wholeRow))
		{
			return true;
		}
		if (BranchRightPath != null && IsAnyTileOfType(BranchRightMarkup, responseType, out wholeRow))
		{
			return true;
		}
		return false;
	}

	public int GetRowTileOfTypeStart(TileMarkup[] markupArray, Tile.ResponseType type)
	{
		for (int i = 0; i < markupArray.Length; i++)
		{
			if (markupArray[i].Left != null && markupArray[i].Left.IsOfType(type))
			{
				return i;
			}
			if (markupArray[i].Centre != null && markupArray[i].Centre.IsOfType(type))
			{
				return i;
			}
			if (markupArray[i].Right != null && markupArray[i].Right.IsOfType(type))
			{
				return i;
			}
		}
		return -1;
	}

	public int GetRowTileOfTypeEnd(TileMarkup[] markupArray, Tile.ResponseType type)
	{
		for (int num = markupArray.Length - 1; num >= 0; num--)
		{
			if (markupArray[num].Left != null && markupArray[num].Left.IsOfType(type))
			{
				return num + 1;
			}
			if (markupArray[num].Centre != null && markupArray[num].Centre.IsOfType(type))
			{
				return num + 1;
			}
			if (markupArray[num].Right != null && markupArray[num].Right.IsOfType(type))
			{
				return num + 1;
			}
		}
		return -1;
	}

	public WorldConstructionHelper.PieceType GetPieceType()
	{
		return TypeId;
	}

	public void SwitchBranch(int branch)
	{
		mBranchSwitch = branch;
		SetWorldControllerBranchSwitch(mBranchSwitch);
		mCachedLength = GetLengthSlow();
		GenerateNextPieces(LevelGenerator.GENERATION_DEPTH, mBranchSwitch);
		CheckPointController.instance.IsBirdAtCheckpoint = false;
	}

	public void SetWorldControllerBranchSwitch(int branch)
	{
		WorldConstructionController.Branch branch2;
		switch (branch)
		{
		case -1:
			branch2 = WorldConstructionController.Branch.Left;
			break;
		case 1:
			branch2 = WorldConstructionController.Branch.Right;
			break;
		default:
			branch2 = WorldConstructionController.Branch.StraightOn;
			break;
		}
		WorldConstructionController.Instance().SwitchBranch(branch2);
	}

	public bool IsRopePiece()
	{
		return TypeId >= WorldConstructionHelper.PieceType.RopeSwing_Entry && TypeId <= WorldConstructionHelper.PieceType.RopeSwing_Exit;
	}

	public bool IsJumpPiece()
	{
		return WorldConstructionHelper.GetSimpleType(TypeId) == WorldConstructionHelper.PieceType.Jump;
	}

	public bool IsDuckPiece()
	{
		return WorldConstructionHelper.GetSimpleType(TypeId) == WorldConstructionHelper.PieceType.Duck;
	}

	public bool IsJunctionPiece()
	{
		return WorldConstructionHelper.GetSimpleType(TypeId) == WorldConstructionHelper.PieceType.Junction;
	}

	public bool IsTrackReductionPiece()
	{
		return WorldConstructionHelper.GetSimpleType(TypeId) == WorldConstructionHelper.PieceType.TrackReduction;
	}

	public bool IsBaddiePiece()
	{
		return WorldConstructionHelper.GetSimpleType(TypeId) == WorldConstructionHelper.PieceType.Baddie || BaddieType != BaddieController.Type.Max;
	}

	public bool IsExcludedForGameType(GameType currentType)
	{
		bool result = false;
		if (m_specificGameType != GameType.GT_NONE)
		{
			result = currentType != m_specificGameType;
		}
		return result;
	}

	public float GetLength()
	{
		if (!mLengthCached)
		{
			mCachedLength = GetLengthSlow();
			mLengthCached = true;
		}
		return mCachedLength;
	}

	public float GetLengthSlow()
	{
		TileDescriptor currentExitPath = GetCurrentExitPath();
		float num = ((!(currentExitPath != null) || !(currentExitPath == StraightAheadPath)) ? 0f : currentExitPath.GetClosestDistance(EntryPath.GetWorldPosition(EntryPath.GetLength(), 0f)));
		return EntryPath.GetLength() + ((!(currentExitPath != null) || !(currentExitPath != EntryPath)) ? 0f : (currentExitPath.GetLength() - num));
	}

	public int SelectedBranch(PieceDescriptor piece)
	{
		return piece.GetBranchSwitch();
	}

	public int AutoBranch(PieceDescriptor piece)
	{
		if (GetBranchPath(mBranchSwitch) != null)
		{
			return mBranchSwitch;
		}
		if (piece.StraightAheadPath != null || (piece.BranchLeftPath != null && piece.BranchRightPath != null))
		{
			return 0;
		}
		if (piece.BranchLeftPath != null)
		{
			return -1;
		}
		if (piece.BranchRightPath != null)
		{
			return 1;
		}
		throw new Exception("Piece should have at least one exit path");
	}

	public Vector3 GetWorldPositionXZ(float distance, float leftRight)
	{
		Vector3 worldPosition = GetWorldPosition(distance, leftRight);
		return new Vector3(worldPosition.x, 0f, worldPosition.z);
	}

	public Vector3 GetWorldPosition(float distance, float leftRight, int branchSelect)
	{
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branchSelect);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance <= num)
		{
			return EntryPath.GetWorldPosition(distance, leftRight);
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			num -= exitPath.GetClosestDistance(EntryPath.GetWorldPosition(num, leftRight));
		}
		if (exitPath != null && exitPath != EntryPath && distance - num <= num2)
		{
			return exitPath.GetWorldPosition(distance - num, leftRight);
		}
		if (GetBranchPiece(branchSelect) != null)
		{
			return GetBranchPiece(branchSelect).GetWorldPosition(distance - GetCachedBranchLength(branchSelect), leftRight, branchSelect);
		}
		return (!(exitPath != null)) ? EntryPath.GetWorldPosition(num - 0.1f, leftRight) : exitPath.GetWorldPosition(num2 - 0.1f, leftRight);
	}

	public Vector3 GetWorldPosition(float distance, float leftRight)
	{
		return GetWorldPosition(distance, leftRight, mBranchSwitch);
	}

	public Vector3 GetWorldPositionAutoBranch(float distance, float leftRight)
	{
		return GetWorldPosition(distance, leftRight, AutoBranch(this));
	}

	public Vector3 GetWorldDirection(float distance, float leftRight, int branchSelect)
	{
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branchSelect);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance <= num)
		{
			return EntryPath.GetWorldDirection(distance, leftRight);
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			num -= exitPath.GetClosestDistance(EntryPath.GetWorldPosition(num, leftRight));
		}
		if (exitPath != null && exitPath != EntryPath && distance - num <= num2)
		{
			return exitPath.GetWorldDirection(distance - num, leftRight);
		}
		if (GetBranchPiece(branchSelect) != null)
		{
			return GetBranchPiece(branchSelect).GetWorldDirection(distance - GetCachedBranchLength(branchSelect), leftRight, branchSelect);
		}
		return (!(exitPath != null)) ? EntryPath.GetWorldDirection(num - 0.1f, leftRight) : exitPath.GetWorldDirection(num2 - 0.1f, leftRight);
	}

	public Vector3 GetWorldDirection(float distance, float leftRight)
	{
		return GetWorldDirection(distance, leftRight, mBranchSwitch);
	}

	public Vector3 GetWorldDirectionAutoBranch(float distance, float leftRight)
	{
		return GetWorldDirection(distance, leftRight, AutoBranch(this));
	}

	public Vector3 GetWorldUp(float distance, float leftRight, int branchSelect)
	{
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branchSelect);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance <= num)
		{
			return EntryPath.GetWorldUp(distance, leftRight);
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			num -= exitPath.GetClosestDistance(EntryPath.GetWorldPosition(num, leftRight));
		}
		if (exitPath != null && exitPath != EntryPath && distance - num <= num2)
		{
			return exitPath.GetWorldUp(distance - num, leftRight);
		}
		if (GetBranchPiece(branchSelect) != null)
		{
			return GetBranchPiece(branchSelect).GetWorldUp(distance - GetCachedBranchLength(branchSelect), leftRight, branchSelect);
		}
		return (!(exitPath != null)) ? EntryPath.GetWorldUp(num - 0.1f, leftRight) : exitPath.GetWorldUp(num2 - 0.1f, leftRight);
	}

	public Vector3 GetWorldUp(float distance, float leftRight)
	{
		return GetWorldUp(distance, leftRight, mBranchSwitch);
	}

	public Vector3 GetWorldUpAutoBranch(float distance, float leftRight)
	{
		return GetWorldUp(distance, leftRight, AutoBranch(this));
	}

	public void EvaluatePath(float distance, BranchSelectDelegate branchSelect, EvaluatePathDelegate evaluatePath)
	{
		int branch = branchSelect(this);
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branch);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance <= num)
		{
			evaluatePath(this, EntryPath, distance);
			return;
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			num -= exitPath.GetClosestDistance(EntryPath.GetWorldPosition(num, 0f));
		}
		if (exitPath != null && exitPath != EntryPath && distance - num <= num2)
		{
			evaluatePath(this, exitPath, distance - num);
		}
		else if (GetBranchPiece(branch) != null)
		{
			GetBranchPiece(branch).EvaluatePath(distance - GetCachedBranchLength(branch), branchSelect, evaluatePath);
		}
		else if (exitPath != null)
		{
			evaluatePath(this, exitPath, num2 - 0.1f);
		}
		else
		{
			evaluatePath(this, EntryPath, num - 0.1f);
		}
	}

	public PathMaterial GetPathMaterial(float distance, float leftRight, BranchSelectDelegate branchSelect)
	{
		int branch = branchSelect(this);
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branch);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance <= num)
		{
			return EntryPath.GetPathMaterial(distance, leftRight);
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			num -= exitPath.GetClosestDistance(EntryPath.GetWorldPosition(num, leftRight));
		}
		if (exitPath != null && exitPath != EntryPath && distance - num <= num2)
		{
			return exitPath.GetPathMaterial(distance - num, leftRight);
		}
		if (GetBranchPiece(branch) != null)
		{
			return GetBranchPiece(branch).GetPathMaterial(distance - GetCachedBranchLength(branch), leftRight, branchSelect);
		}
		return (!(exitPath != null)) ? EntryPath.GetPathMaterial(num - 0.1f, leftRight) : exitPath.GetPathMaterial(num2 - 0.1f, leftRight);
	}

	public PathMaterial GetPathMaterial(float distance, float leftRight)
	{
		return GetPathMaterial(distance, leftRight, SelectedBranch);
	}

	public PathMaterial GetPathMaterialAutoBranch(float distance, float leftRight)
	{
		return GetPathMaterial(distance, leftRight, AutoBranch);
	}

	public bool IsTileOfResponseType(float distance, float leftRight, Tile.ResponseType type, out bool endOfPathDetected)
	{
		float length = EntryPath.GetLength();
		TileDescriptor currentExitPath = GetCurrentExitPath();
		float num = ((!(currentExitPath != null)) ? 0f : currentExitPath.GetLength());
		endOfPathDetected = false;
		if (distance < length)
		{
			return IsTileOfResponseType(EntryMarkup, distance, leftRight, type);
		}
		if (currentExitPath != null && currentExitPath != EntryPath && distance - length < num)
		{
			return IsTileOfResponseType(GetCurrentExitMarkup(), distance - length, leftRight, type);
		}
		endOfPathDetected = true;
		return false;
	}

	public bool IsTileOfResponseType(TileMarkup[] markupArray, float distance, float leftRight, Tile.ResponseType type)
	{
		int num = Mathf.FloorToInt(distance);
		if (leftRight < -1.5f)
		{
			throw new Exception("Left/Right position out of range");
		}
		if (leftRight < -0.5f)
		{
			return markupArray[num].Left.IsOfType(type);
		}
		if (leftRight <= 0.5f)
		{
			return markupArray[num].Centre.IsOfType(type);
		}
		if (leftRight <= 1.5f)
		{
			return markupArray[num].Right.IsOfType(type);
		}
		throw new Exception("Left/Right position out of range");
	}

	public Tile GetTile(float distance, float leftRight, int branchSelect)
	{
		float num = EntryPath.GetLength();
		TileDescriptor exitPath = GetExitPath(branchSelect);
		float num2 = ((!(exitPath != null)) ? 0f : exitPath.GetLength());
		if (distance < num)
		{
			return GetTile(EntryMarkup, distance, leftRight);
		}
		if (exitPath != null && exitPath == StraightAheadPath)
		{
			if (!exitPath.IsExitPath)
			{
				exitPath.CacheClosestDist(exitPath.GetClosestDistance(EntryPath.GetWorldPosEntryPathWithEntryLength(-1f)), exitPath.GetClosestDistance(EntryPath.GetWorldPosEntryPathWithEntryLength(0f)), exitPath.GetClosestDistance(EntryPath.GetWorldPosEntryPathWithEntryLength(1f)));
				exitPath.IsExitPath = true;
			}
			num -= exitPath.GetClosestDistEntryExit(leftRight);
		}
		if (exitPath != null && exitPath != EntryPath && distance - num < num2)
		{
			return GetTile(GetExitMarkup(branchSelect), distance - num, leftRight);
		}
		if (GetBranchPiece(branchSelect) != null)
		{
			return GetBranchPiece(branchSelect).GetTile(distance - GetCachedBranchLength(branchSelect), leftRight, branchSelect);
		}
		return null;
	}

	public Tile GetTile(float distance, float leftRight)
	{
		return GetTile(distance, leftRight, mBranchSwitch);
	}

	public Tile GetTile(TileMarkup[] markupArray, float distance, float leftRight)
	{
		int num = Mathf.FloorToInt(distance);
		if (leftRight < -1.5f)
		{
			throw new Exception("Left/Right position out of range");
		}
		if (leftRight < -0.5f)
		{
			return markupArray[num].Left;
		}
		if (leftRight <= 0.5f)
		{
			return markupArray[num].Centre;
		}
		if (leftRight <= 1.5f)
		{
			return markupArray[num].Right;
		}
		throw new Exception("Left/Right position out of range");
	}

	public bool IsAnyTileOfType(TileMarkup[] markupArray, Tile.ResponseType type, out bool wholeRow)
	{
		for (int i = 0; i < markupArray.Length; i++)
		{
			int num = 0;
			if (markupArray[i].Left != null && markupArray[i].Left.IsOfType(type))
			{
				num++;
			}
			if (markupArray[i].Centre != null && markupArray[i].Centre.IsOfType(type))
			{
				num++;
			}
			if (markupArray[i].Right != null && markupArray[i].Right.IsOfType(type))
			{
				num++;
			}
			if (num > 0)
			{
				wholeRow = num == 3;
				return true;
			}
		}
		wholeRow = false;
		return false;
	}

	public bool IsAnyRowTypeOrKill(TileMarkup[] markupArray, Tile.ResponseType type)
	{
		for (int i = 0; i < markupArray.Length; i++)
		{
			int num = 0;
			if (markupArray[i].Left != null && (markupArray[i].Left.IsOfType(type) || markupArray[i].Left.IsOfType(Tile.ResponseType.Kill)))
			{
				num++;
			}
			if ((markupArray[i].Centre != null && markupArray[i].Centre.IsOfType(type)) || markupArray[i].Centre.IsOfType(Tile.ResponseType.Kill))
			{
				num++;
			}
			if ((markupArray[i].Right != null && markupArray[i].Right.IsOfType(type)) || markupArray[i].Right.IsOfType(Tile.ResponseType.Kill))
			{
				num++;
			}
			if (num == 3)
			{
				return true;
			}
		}
		return false;
	}

	public PieceDescriptor GetNextPiece()
	{
		return GetBranchPiece(mBranchSwitch);
	}

	public PieceDescriptor GetBranchPiece(int branch)
	{
		switch (branch)
		{
		case -1:
			return mBranchLeftPiece;
		case 0:
			return mStraightAheadPiece;
		case 1:
			return mBranchRightPiece;
		default:
			throw new Exception("BranchSwitch must be -1, 0 or +1");
		}
	}

	public TileMarkup[] GetCurrentExitMarkup()
	{
		return GetExitMarkup(mBranchSwitch);
	}

	public TileMarkup[] GetExitMarkup(int branch)
	{
		switch (branch)
		{
		case -1:
			return BranchLeftMarkup;
		case 0:
			return StraightAheadMarkup;
		case 1:
			return BranchRightMarkup;
		default:
			throw new Exception("BranchSwitch must be -1, 0 or +1");
		}
	}

	public TileDescriptor GetCurrentExitPath()
	{
		return GetExitPath(mBranchSwitch);
	}

	public TileDescriptor GetExitPath(int branch)
	{
		switch (branch)
		{
		case -1:
			return BranchLeftPath;
		case 0:
			return StraightAheadPath;
		case 1:
			return BranchRightPath;
		default:
			throw new Exception("BranchSwitch must be -1, 0 or +1");
		}
	}

	public IList<TileDescriptor> GetExitPaths()
	{
		List<TileDescriptor> list = new List<TileDescriptor>();
		if (StraightAheadPath != null)
		{
			list.Add(StraightAheadPath);
		}
		if (BranchLeftPath != null)
		{
			list.Add(BranchLeftPath);
		}
		if (BranchRightPath != null)
		{
			list.Add(BranchRightPath);
		}
		return list;
	}

	public void GenerateNextPieces(float depth, int branch)
	{
		if (PieceDescriptorBuffer.Instance() != null && PieceDescriptorBuffer.Instance().IsGenerating())
		{
			mDeferredDepth = depth;
			mDeferredBranch = branch;
			mDeferGeneration = true;
		}
		else
		{
			if (depth <= 0f)
			{
				return;
			}
			bool flag = BranchLeftPath != null || BranchRightPath != null;
			if (flag && branch == 0)
			{
				GenerateDiscardableBranchPaths();
				return;
			}
			float depth2 = depth - GetLength();
			if (BranchLeftPath != null && (!flag || (flag && branch == -1)))
			{
				if (mBranchLeftPiece == null)
				{
					mBranchLeftPiece = Generator.GeneratePiece(GetNextTheme(true, false), GetNextGroup(), this, -1);
					mBranchLeftPiece.transform.parent = base.transform.parent;
					AttachPiece(BranchLeftAnchor, mBranchLeftPiece.EntryAnchor, mBranchLeftPiece, -1);
				}
				mBranchLeftPiece.GenerateNextPieces(depth2, 0);
			}
			if (StraightAheadPath != null && (!flag || (flag && branch == 0)))
			{
				if (mStraightAheadPiece == null)
				{
					mStraightAheadPiece = Generator.GeneratePiece(GetNextTheme(false, false), GetNextGroup(), this, 0);
					mStraightAheadPiece.transform.parent = base.transform.parent;
					AttachPiece(StraightAheadAnchor, mStraightAheadPiece.EntryAnchor, mStraightAheadPiece, 0);
				}
				mStraightAheadPiece.GenerateNextPieces(depth2, 0);
			}
			if (BranchRightPath != null && (!flag || (flag && branch == 1)))
			{
				if (mBranchRightPiece == null)
				{
					mBranchRightPiece = Generator.GeneratePiece(GetNextTheme(false, true), GetNextGroup(), this, 1);
					mBranchRightPiece.transform.parent = base.transform.parent;
					AttachPiece(BranchRightAnchor, mBranchRightPiece.EntryAnchor, mBranchRightPiece, 1);
				}
				mBranchRightPiece.GenerateNextPieces(depth2, 0);
			}
		}
	}

	public void TrimDeadBranches()
	{
		if (mBranchSwitch != -1 && mBranchLeftPiece != null)
		{
			mBranchLeftPiece.DestroyRecursive();
		}
		if (mBranchSwitch != 0 && mStraightAheadPiece != null)
		{
			mStraightAheadPiece.DestroyRecursive();
		}
		if (mBranchSwitch != 1 && mBranchRightPiece != null)
		{
			mBranchRightPiece.DestroyRecursive();
		}
	}

	public void DestroyRecursive()
	{
		if (mBranchLeftPiece != null)
		{
			mBranchLeftPiece.DestroyRecursive();
		}
		if (mStraightAheadPiece != null)
		{
			mStraightAheadPiece.DestroyRecursive();
		}
		if (mBranchRightPiece != null)
		{
			mBranchRightPiece.DestroyRecursive();
		}
		LevelGenerator.Instance().RemoveActivePiece(this);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public WorldConstructionHelper.Theme GetNextTheme(bool fromBranchLeft, bool fromBranchRight)
	{
		WorldConstructionHelper.Theme result = Theme;
		if (TypeId == WorldConstructionHelper.PieceType.ThemeTransition)
		{
			result = (fromBranchLeft ? BranchLeftTheme : ((!fromBranchRight) ? StraightAheadTheme : BranchRightTheme));
		}
		return result;
	}

	public WorldConstructionHelper.Group GetNextGroup()
	{
		WorldConstructionHelper.Group obj = Group;
		WorldConstructionHelper.PieceType typeId = TypeId;
		if (obj == WorldConstructionHelper.Group.Transition)
		{
			obj = GroupTransitionExit;
		}
		if (typeId == WorldConstructionHelper.PieceType.ThemeTransition)
		{
			obj = CreateNextPieceHelper.FindGroupForTransitionExit(ThemeTransitionEntry, Theme);
		}
		return obj;
	}

	private void CreateTileMarkup(TileMarkup[] markupArray)
	{
		if (markupArray != null)
		{
			foreach (TileMarkup tileMarkup in markupArray)
			{
				tileMarkup.Left = new Tile(tileMarkup.LeftType);
				tileMarkup.Centre = new Tile(tileMarkup.CentreType);
				tileMarkup.Right = new Tile(tileMarkup.RightType);
			}
		}
	}

	private void GenerateDiscardableBranchPaths()
	{
		if (StraightAheadPath != null && mStraightAheadPiece == null)
		{
			PieceDescriptorBuffer.Instance().GenerateDiscardableBranchPath(this, WorldConstructionController.Branch.StraightOn);
		}
		if (BranchLeftPath != null && mBranchLeftPiece == null)
		{
			PieceDescriptorBuffer.Instance().GenerateDiscardableBranchPath(this, WorldConstructionController.Branch.Left);
		}
		if (BranchRightPath != null && mBranchRightPiece == null)
		{
			PieceDescriptorBuffer.Instance().GenerateDiscardableBranchPath(this, WorldConstructionController.Branch.Right);
		}
	}

	private void InitialisePieceType(TileDescriptor tileSection, TileMarkup[] markupArray)
	{
		InitialisePieceTypeTheme(tileSection);
		InitialisePieceTypeGroup(base.name);
		if (base.name.Equals("JNG_CrocPit_In_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Crocodile_Entry;
		}
		else if (base.name.Equals("JNG_CrocPit_Tile_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Crocodile_Loop;
		}
		else if (base.name.Equals("JNG_CrocPit_Out_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Crocodile_Exit;
		}
		else if (base.name.ToLower().Contains("pickup"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pickup;
		}
		else if (base.name.ToLower().Contains("checkpoint"))
		{
			TypeId = WorldConstructionHelper.PieceType.Checkpoint;
		}
		else if (base.name.Contains("Pit_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type1_Entry;
		}
		else if (base.name.Contains("Pit_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type1_Loop;
		}
		else if (base.name.Contains("Pit_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type1_Exit;
		}
		else if (base.name.Contains("Pit_L_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type2_Entry;
		}
		else if (base.name.Contains("Pit_L_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type2_Loop;
		}
		else if (base.name.Contains("Pit_L_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type2_Exit;
		}
		else if (base.name.Contains("Pit_R_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type3_Entry;
		}
		else if (base.name.Contains("Pit_R_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type3_Loop;
		}
		else if (base.name.Contains("Pit_R_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type3_Exit;
		}
		else if (base.name.Contains("Reduce_L_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Entry;
		}
		else if (base.name.Contains("Reduce_L_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Loop;
		}
		else if (base.name.Contains("Reduce_L_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Exit;
		}
		else if (base.name.Contains("Reduce_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type5_Entry;
		}
		else if (base.name.Contains("Reduce_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type5_Loop;
		}
		else if (base.name.Contains("Reduce_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type5_Exit;
		}
		else if (base.name.Contains("Reduce_R_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type6_Entry;
		}
		else if (base.name.Contains("Reduce_R_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type6_Loop;
		}
		else if (base.name.Contains("Reduce_R_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type6_Exit;
		}
		else if (base.name.Contains("DropOff_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Entry;
		}
		else if (base.name.Contains("DropOff_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Loop;
		}
		else if (base.name.Contains("DropOff_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type4_Exit;
		}
		else if (base.name.Equals("JNG_Pit_InDown_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type1_Entry_Down;
		}
		else if (base.name.Equals("JNG_Pit_OutUp_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Pit_Type1_Exit_Up;
		}
		else if (base.name.Contains("Pit_Swing_In"))
		{
			TypeId = WorldConstructionHelper.PieceType.RopeSwing_Entry;
		}
		else if (base.name.Contains("Pit_Swing_Node"))
		{
			TypeId = WorldConstructionHelper.PieceType.RopeSwing_Node;
		}
		else if (base.name.Contains("Pit_Swing_Tile"))
		{
			TypeId = WorldConstructionHelper.PieceType.RopeSwing_Tile;
		}
		else if (base.name.Contains("Pit_Swing_Out"))
		{
			TypeId = WorldConstructionHelper.PieceType.RopeSwing_Exit;
		}
		else if (base.name.Contains("BIKE_DEMO_Meteor_ON_A_1") || base.name.Contains("BIKE_DEMO_Meteor_OFF_A_1") || base.name.Contains("JNG_DEMO_Meteor_ON_A_1") || base.name.Contains("JNG_DEMO_Meteor_OFF_A_1"))
		{
			TypeId = WorldConstructionHelper.PieceType.Exclude;
		}
		else if (base.name.Contains("_Trans_"))
		{
			TypeId = WorldConstructionHelper.PieceType.ThemeTransition;
			ThemeTransitionEntry = DetermineTileThemeEntry(base.name);
			int num = 0;
			if (StraightAheadPath != null)
			{
				num++;
			}
			if (BranchLeftPath != null)
			{
				num++;
			}
			if (BranchRightPath != null)
			{
				num++;
			}
			if (num == 1)
			{
				if (StraightAheadPath != null)
				{
					StraightAheadTheme = Theme;
				}
				else if (BranchLeftPath != null)
				{
					BranchLeftTheme = Theme;
				}
				else if (BranchRightPath != null)
				{
					BranchRightTheme = Theme;
				}
			}
		}
		else
		{
			WorldConstructionHelper.PieceType pieceType = WorldConstructionHelper.PieceType.Straight;
			if (FeaturePiece)
			{
				pieceType = WorldConstructionHelper.PieceType.Straight_Feature;
			}
			bool wholeRow = false;
			bool wholeRow2 = false;
			bool wholeRow3 = false;
			bool flag = IsAnyTileOfType(markupArray, Tile.ResponseType.SwipeLeft, out wholeRow);
			bool flag2 = IsAnyTileOfType(markupArray, Tile.ResponseType.SwipeRight, out wholeRow);
			if (flag || flag2)
			{
				pieceType = ((StraightAheadPath != null) ? ((flag && flag2) ? WorldConstructionHelper.PieceType.Junction_Optional_T : ((!flag) ? WorldConstructionHelper.PieceType.Junction_Optional_R : WorldConstructionHelper.PieceType.Junction_Optional_L)) : ((flag && flag2) ? WorldConstructionHelper.PieceType.Junction_T : ((!flag) ? WorldConstructionHelper.PieceType.Junction_R : WorldConstructionHelper.PieceType.Junction_L)));
			}
			else if (IsAnyTileOfType(markupArray, Tile.ResponseType.SwipeUp, out wholeRow2) || IsAnyTileOfType(markupArray, Tile.ResponseType.Kill, out wholeRow3))
			{
				bool wholeRow4;
				bool flag3 = IsAnyTileOfType(markupArray, Tile.ResponseType.SwipeDown, out wholeRow4);
				pieceType = ((!wholeRow4 && IsAnyRowTypeOrKill(markupArray, Tile.ResponseType.SwipeDown)) ? WorldConstructionHelper.PieceType.Duck : (flag3 ? WorldConstructionHelper.PieceType.JumpOrDuck : ((!wholeRow2 && !wholeRow3) ? ((!IsAnyRowTypeOrKill(markupArray, Tile.ResponseType.SwipeUp)) ? WorldConstructionHelper.PieceType.TrackReduction : WorldConstructionHelper.PieceType.Jump) : WorldConstructionHelper.PieceType.Jump)));
			}
			else if (IsAnyTileOfType(markupArray, Tile.ResponseType.SwipeDown, out wholeRow))
			{
				pieceType = WorldConstructionHelper.PieceType.Duck;
			}
			if (pieceType > TypeId)
			{
				TypeId = pieceType;
			}
		}
	}

	private void InitialisePieceTypeGroup(string name)
	{
		Group = PieceDescriptorGroupMap.ParseGroupData(name, out GroupTransitionEntry, out GroupTransitionExit);
	}

	private void InitialisePieceTypeBend(string name)
	{
		Bend = 0;
		if (name.Contains("_L90_") || (name.Contains("Bend") && name.Contains("_L_")))
		{
			Bend = -90;
		}
		else if (name.Contains("_R90_") || (name.Contains("Bend") && name.Contains("_R_")))
		{
			Bend = 90;
		}
		else if (name == "JNG_Trans_FromMnt_A_1")
		{
			Bend = -90;
		}
	}

	public void InitialisePieceTypeEarlyPathEnd()
	{
		bool flag = true;
		bool flag2 = true;
		for (int i = 0; i < EntryMarkup.Length; i++)
		{
			if (flag)
			{
				if (EntryMarkup[i].Left == null || !EntryMarkup[i].Left.IsOfType(Tile.ResponseType.BlockRight))
				{
					flag = false;
				}
				if (flag && EntryMarkup[i].Left != null && EntryMarkup[i].Left.IsOfType(Tile.ResponseType.Kill))
				{
					PathEndsEarly = true;
				}
			}
			if (flag2)
			{
				if (EntryMarkup[i].Right == null || !EntryMarkup[i].Right.IsOfType(Tile.ResponseType.BlockLeft))
				{
					flag2 = false;
				}
				if (flag2 && EntryMarkup[i].Right != null && EntryMarkup[i].Right.IsOfType(Tile.ResponseType.Kill))
				{
					PathEndsEarly = true;
				}
			}
		}
	}

	public static WorldConstructionHelper.Theme DetermineTileTheme(string TileName)
	{
		WorldConstructionHelper.Theme result = WorldConstructionHelper.Theme.Jungle;
		if (TileName.Contains("JNG_"))
		{
			result = WorldConstructionHelper.Theme.Jungle;
		}
		else if (TileName.Contains("MNT_"))
		{
			result = WorldConstructionHelper.Theme.Mountain;
		}
		else if (TileName.Contains("CAVE_"))
		{
			result = WorldConstructionHelper.Theme.Cave;
		}
		else if (TileName.Contains("SLIP_"))
		{
			result = WorldConstructionHelper.Theme.SlippedMountain;
		}
		else if (TileName.Contains("CART_"))
		{
			result = WorldConstructionHelper.Theme.Minecart;
		}
		else if (TileName.Contains("BIKE_"))
		{
			result = WorldConstructionHelper.Theme.Bike;
		}
		else if (TileName.Contains("MENU_"))
		{
			result = WorldConstructionHelper.Theme.Menu;
		}
		else
		{
			Debug.LogError("ERROR - Unknown Theme for piece " + TileName);
		}
		return result;
	}

	public static WorldConstructionHelper.Theme DetermineTileThemeEntry(string TileName)
	{
		if (TileName.Contains("_Trans_"))
		{
			if (TileName.Contains("FromBike"))
			{
				return WorldConstructionHelper.Theme.Bike;
			}
			if (TileName.Contains("FromCart"))
			{
				return WorldConstructionHelper.Theme.Minecart;
			}
			if (TileName.Contains("FromCave"))
			{
				return WorldConstructionHelper.Theme.Cave;
			}
			if (TileName.Contains("FromJung"))
			{
				return WorldConstructionHelper.Theme.Jungle;
			}
			if (TileName.Contains("FromMnt"))
			{
				return WorldConstructionHelper.Theme.Mountain;
			}
			if (TileName.Contains("FromMenu"))
			{
				return WorldConstructionHelper.Theme.Menu;
			}
			if (TileName.Contains("FromSlip"))
			{
				return WorldConstructionHelper.Theme.SlippedMountain;
			}
			MonoBehaviour.print("ERROR - InitialisePieceType - Transition Piece has invalid naming convention in the 'From' bit =" + TileName);
		}
		return DetermineTileTheme(TileName);
	}

	private void InitialisePieceTypeTheme(TileDescriptor tileSection)
	{
		Theme = DetermineTileTheme(base.name);
	}

	public void AttachPiece(GameObject previousAnchor, GameObject newAnchor, PieceDescriptor exitPiece, int attachToBranch)
	{
		if (!(previousAnchor != null))
		{
			return;
		}
		if (exitPiece == null || newAnchor == null)
		{
			MonoBehaviour.print("ERROR - EntryAnchor not setup. Bad PieceDescriptor = " + base.name);
			return;
		}
		LevelUtils.ModifyTransformToAlignPoints(exitPiece.transform, previousAnchor.transform, newAnchor.transform);
		exitPiece.DistanceToNextCoinRun = DistanceToNextCoinRun;
		exitPiece.TotalDistance = TotalDistance + GetCachedBranchLength(attachToBranch);
		if (CurrentCheckPointNum > exitPiece.CurrentCheckPointNum)
		{
			exitPiece.CurrentCheckPointNum = CurrentCheckPointNum;
		}
		Compass compass = Facing;
		if (attachToBranch == -1 || Bend == -90)
		{
			int num = (int)compass;
			num--;
			if (num < 0)
			{
				num = 3;
			}
			compass = (Compass)num;
		}
		else if (attachToBranch == 1 || Bend == 90)
		{
			int num2 = (int)compass;
			num2++;
			if (num2 >= 4)
			{
				num2 = 0;
			}
			compass = (Compass)num2;
		}
		exitPiece.Facing = compass;
	}

	public TileDescriptor GetBranchPath(int branch)
	{
		switch (branch)
		{
		case -1:
			return BranchLeftPath;
		case 0:
			return StraightAheadPath;
		case 1:
			return BranchRightPath;
		default:
			return null;
		}
	}

	public float GetBranchLength(int branch)
	{
		TileDescriptor exitPath = GetExitPath(branch);
		float num = ((!(exitPath != null) || !(exitPath == StraightAheadPath)) ? 0f : exitPath.GetClosestDistance(EntryPath.GetWorldPosition(EntryPath.GetLength(), 0f)));
		if (EntryPath == null)
		{
			Debug.LogWarning("This piece:" + base.name + "has no entry path");
			return 0f;
		}
		return EntryPath.GetLength() + ((!(exitPath != null) || !(exitPath != EntryPath)) ? 0f : (exitPath.GetLength() - num));
	}

	public static void LinkPieceToModel(PieceDescriptor pieceDescriptor, GameObject modelInstance)
	{
		TileDescriptor[] componentsInChildren = modelInstance.GetComponentsInChildren<TileDescriptor>();
		Dictionary<string, TileDescriptor> dictionary = new Dictionary<string, TileDescriptor>();
		TileDescriptor[] array = componentsInChildren;
		foreach (TileDescriptor tileDescriptor in array)
		{
			dictionary[tileDescriptor.transform.name.ToUpperInvariant()] = tileDescriptor;
		}
		dictionary.TryGetValue("ENTRYPATH", out pieceDescriptor.EntryPath);
		dictionary.TryGetValue("STRAIGHTAHEADPATH", out pieceDescriptor.StraightAheadPath);
		dictionary.TryGetValue("BRANCHLEFTPATH", out pieceDescriptor.BranchLeftPath);
		dictionary.TryGetValue("BRANCHRIGHTPATH", out pieceDescriptor.BranchRightPath);
		if (pieceDescriptor.EntryPath == null && pieceDescriptor.StraightAheadPath == null)
		{
			dictionary.TryGetValue("PATH", out pieceDescriptor.EntryPath);
			dictionary.TryGetValue("PATH", out pieceDescriptor.StraightAheadPath);
		}
		TileMarkup[] newMarkup = ((!(pieceDescriptor.EntryPath != null)) ? new TileMarkup[0] : CreateTileMarkupArray(pieceDescriptor.EntryPath.Rows.Count - 1));
		TileMarkup[] newMarkup2 = ((!(pieceDescriptor.StraightAheadPath != null) || !(pieceDescriptor.EntryPath != pieceDescriptor.StraightAheadPath)) ? new TileMarkup[0] : CreateTileMarkupArray(pieceDescriptor.StraightAheadPath.Rows.Count - 1));
		TileMarkup[] newMarkup3 = ((!(pieceDescriptor.BranchLeftPath != null)) ? new TileMarkup[0] : CreateTileMarkupArray(pieceDescriptor.BranchLeftPath.Rows.Count - 1));
		TileMarkup[] newMarkup4 = ((!(pieceDescriptor.BranchRightPath != null)) ? new TileMarkup[0] : CreateTileMarkupArray(pieceDescriptor.BranchRightPath.Rows.Count - 1));
		pieceDescriptor.EntryMarkup = CopyMarkup(pieceDescriptor.EntryMarkup, newMarkup);
		pieceDescriptor.StraightAheadMarkup = CopyMarkup(pieceDescriptor.StraightAheadMarkup, newMarkup2);
		pieceDescriptor.BranchLeftMarkup = CopyMarkup(pieceDescriptor.BranchLeftMarkup, newMarkup3);
		pieceDescriptor.BranchRightMarkup = CopyMarkup(pieceDescriptor.BranchRightMarkup, newMarkup4);
		pieceDescriptor.EntryAnchor = FindAnchor(modelInstance.transform, "ENTRY");
		pieceDescriptor.StraightAheadAnchor = FindAnchor(modelInstance.transform, "EXIT");
		pieceDescriptor.BranchLeftAnchor = FindAnchor(modelInstance.transform, "EXIT_LEFT");
		pieceDescriptor.BranchRightAnchor = FindAnchor(modelInstance.transform, "EXIT_RIGHT");
		if (pieceDescriptor.BranchLeftPath != null && pieceDescriptor.StraightAheadPath == null && pieceDescriptor.BranchRightPath == null && pieceDescriptor.BranchLeftAnchor == null)
		{
			pieceDescriptor.BranchLeftAnchor = FindAnchor(modelInstance.transform, "EXIT");
			pieceDescriptor.StraightAheadAnchor = null;
		}
		else if (pieceDescriptor.BranchRightPath != null && pieceDescriptor.StraightAheadPath == null && pieceDescriptor.BranchLeftPath == null && pieceDescriptor.BranchRightAnchor == null)
		{
			pieceDescriptor.BranchRightAnchor = FindAnchor(modelInstance.transform, "EXIT");
			pieceDescriptor.StraightAheadAnchor = null;
		}
	}

	private static TileMarkup[] CopyMarkup(TileMarkup[] oldMarkup, TileMarkup[] newMarkup)
	{
		if (oldMarkup != null && newMarkup != null)
		{
			for (int i = 0; i < newMarkup.Length && i < oldMarkup.Length; i++)
			{
				newMarkup[i].LeftType = oldMarkup[i].LeftType;
				newMarkup[i].CentreType = oldMarkup[i].CentreType;
				newMarkup[i].RightType = oldMarkup[i].RightType;
			}
		}
		return newMarkup;
	}

	private static TileMarkup[] CreateTileMarkupArray(int size)
	{
		TileMarkup[] array = new TileMarkup[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = new TileMarkup();
		}
		return array;
	}

	private static GameObject FindAnchor(Transform root, string anchorName)
	{
		Transform transform = root.Find(anchorName);
		return (!(transform != null)) ? null : transform.gameObject;
	}

	public PieceDescriptor GetBranchLeftPiece()
	{
		return mBranchLeftPiece;
	}

	public PieceDescriptor GetStraightAheadPiece()
	{
		return mStraightAheadPiece;
	}

	public PieceDescriptor GetBranchRightPiece()
	{
		return mBranchRightPiece;
	}

	public void SetBranchLeftPiece(PieceDescriptor piece)
	{
		mBranchLeftPiece = piece;
	}

	public void SetStraightAheadPiece(PieceDescriptor piece)
	{
		mStraightAheadPiece = piece;
	}

	public void SetBranchRightPiece(PieceDescriptor piece)
	{
		mBranchRightPiece = piece;
	}

	public int GetBranchSwitch()
	{
		return mBranchSwitch;
	}

	public float GetCachedLength()
	{
		return GetLength();
	}

	public float GetCachedBranchLength(int branch)
	{
		switch (branch)
		{
		case -1:
			return GetCachedLeftLength();
		case 0:
			return GetCachedAheadLength();
		case 1:
			return GetCachedRightLength();
		default:
			throw new Exception("Invalid branch switch");
		}
	}

	public float GetCachedLeftLength()
	{
		if (!mCachedLeftLengthSet)
		{
			SetCachedLeftLength();
		}
		return mCachedLeftLength;
	}

	public float GetCachedAheadLength()
	{
		if (!mCachedAheadLengthSet)
		{
			SetCachedAheadLength();
		}
		return mCachedAheadLength;
	}

	public float GetCachedRightLength()
	{
		if (!mCachedRightLengthSet)
		{
			SetCachedRightLength();
		}
		return mCachedRightLength;
	}

	public void SetCachedLeftLength()
	{
		mCachedLeftLengthSet = true;
		mCachedLeftLength = GetBranchLength(-1);
	}

	public void SetCachedAheadLength()
	{
		mCachedAheadLengthSet = true;
		mCachedAheadLength = GetBranchLength(0);
	}

	public void SetCachedRightLength()
	{
		mCachedRightLengthSet = true;
		mCachedRightLength = GetBranchLength(1);
	}

	public void ResetTimesUsedThisRun()
	{
		mTimesUsedThisRun = 0;
	}

	public void IncrementTimesUsed()
	{
		mTimesUsedThisRun++;
	}

	public int GetTimesUsedThisRun()
	{
		return mTimesUsedThisRun;
	}

	public void SpawnMeteorHere()
	{
		mSpawnMeteor = true;
	}

	public bool CanSpawnMeteor()
	{
		return mSpawnMeteor;
	}

	public void SpawnedMeteor()
	{
		mSpawnMeteor = false;
	}
}
