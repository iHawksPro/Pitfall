using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
	public enum DIRECTION
	{
		LEFT = 0,
		RIGHT = 1,
		STRAIGHT_DOWN = 2,
		LEFT_OR_RIGHT = 3,
		RANDOM = 4,
		TOWARD_PLAYER = 5
	}

	private const int MaxCachedMeteors = 18;

	public PlayerController Player;

	public GameObject PlayerObj;

	public GameObject SmallMeteorPrefab;

	public GameObject LargeMeteorPrefab;

	public GameObject HugeMeteorPrefab;

	public GameObject SafeMeteorPrefab;

	private PieceDescriptor mCurrentPiece;

	private float mPathDistance;

	public float YSpawnPosition = 10f;

	public float Life = 3f;

	public AnimationCurve SpeedCurve;

	public float MinDelayBetweenMeteors = 0.5f;

	public float MaxDelayBetweenMeteors = 4f;

	public float MeteorSpawnTimeAheadOfPlayer = 2f;

	public float TimeToImpact = 0.5f;

	public int HugeMeteorPossiblility = 75;

	private float CurrentOnTrackLargeMeteorInterval;

	private float CurrentOnTrackLargeMeteorDelay = 3f;

	private float CurrentOffTrackLargeMeteorInterval;

	private float CurrentOffTrackLargeMeteorDelay = 3f;

	private float CurrentSmallMeteorInterval;

	private float CurrentSmallMeteorDelay = 3f;

	public GameObject ImpactPrefab_Large;

	public GameObject ImpactPrefab_Small;

	public GameObject MidAirExplosionPrefab;

	public SoundFXData WhizzSfx;

	public SoundFXData ImpactSfx;

	private List<Meteor> CachedMeteors = new List<Meteor>();

	private void Start()
	{
		CurrentOnTrackLargeMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
		CurrentOffTrackLargeMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
		CurrentSmallMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
		Meteor meteor = null;
		uint num = 0u;
		while (true)
		{
			switch (num)
			{
			case 0u:
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
				meteor = new Meteor(Object.Instantiate(SmallMeteorPrefab) as GameObject, Meteor.METEOR_TYPE.SMALL, ImpactPrefab_Small, SpeedCurve);
				break;
			case 6u:
			case 7u:
			case 8u:
			case 9u:
			case 10u:
			case 11u:
			case 12u:
				meteor = new Meteor(Object.Instantiate(LargeMeteorPrefab) as GameObject, Meteor.METEOR_TYPE.LARGE, ImpactPrefab_Small, SpeedCurve);
				break;
			case 13u:
			case 14u:
			case 15u:
			case 16u:
				meteor = new Meteor(Object.Instantiate(HugeMeteorPrefab) as GameObject, Meteor.METEOR_TYPE.HUGE, ImpactPrefab_Small, SpeedCurve);
				break;
			case 17u:
				meteor = new Meteor(Object.Instantiate(SafeMeteorPrefab) as GameObject, Meteor.METEOR_TYPE.DISINTEGRATING, ImpactPrefab_Small, SpeedCurve);
				break;
			default:
				return;
			}
			CachedMeteors.Add(meteor);
			meteor = null;
			num++;
		}
	}

	private void Update()
	{
		for (int i = 0; i < 18; i++)
		{
			if (!CachedMeteors[i].IsValid())
			{
				CachedMeteors[i].RemoveFromPlay();
			}
			else
			{
				CachedMeteors[i].Update();
			}
		}
	}

	public void GenerateMeteors(PieceDescriptor currentPiece, float pathDistance, bool generateDeadlyMeteors)
	{
		mCurrentPiece = currentPiece;
		mPathDistance = pathDistance;
		GenerateMeteor(generateDeadlyMeteors);
	}

	public void SpawnMeteor(Vector3 landPos, bool canKillPlayer, Meteor.METEOR_TYPE meteorType, Meteor.TARGET target, DIRECTION direction, float timeToImpact, bool destroyOnImpact)
	{
		Vector3 startPosition = Vector3.zero;
		SetUpMeteorPositions(target, ref startPosition, ref landPos, direction);
		for (int i = 0; i < 18; i++)
		{
			if (!CachedMeteors[i].IsValid() || CachedMeteors[i].GetMeteorType() != meteorType || CachedMeteors[i].Active())
			{
				continue;
			}
			if (target == Meteor.TARGET.PATH || target == Meteor.TARGET.OFF_PATH)
			{
				timeToImpact = TimeToImpact;
			}
			GameObject impactPrefab = CachedMeteors[i].GetImpactPrefab();
			if (target == Meteor.TARGET.TO_NODE || destroyOnImpact)
			{
				impactPrefab = MidAirExplosionPrefab;
			}
			CachedMeteors[i].SetUpMeteor(startPosition, landPos, timeToImpact, Life, canKillPlayer, PlayerObj, target, destroyOnImpact, impactPrefab);
			if (meteorType != Meteor.METEOR_TYPE.SMALL)
			{
				SoundManager.Instance.Play(WhizzSfx, CachedMeteors[i].GetModel());
				if (timeToImpact > 0f)
				{
					SoundManager.Instance.Play(ImpactSfx, CachedMeteors[i].GetModel(), timeToImpact);
				}
			}
			i = 18;
			break;
		}
	}

	private void SetUpMeteorPositions(Meteor.TARGET target, ref Vector3 startPosition, ref Vector3 landPosition, DIRECTION direction)
	{
		switch (target)
		{
		case Meteor.TARGET.PATH:
		case Meteor.TARGET.TO_NODE:
		case Meteor.TARGET.THROUGH_NODE:
		{
			startPosition = landPosition + Vector3.up * YSpawnPosition;
			Transform transform2 = PlayerController.Instance().transform;
			startPosition += transform2.forward * Random.Range(-20f, 0f);
			switch (direction)
			{
			case DIRECTION.LEFT:
				startPosition += transform2.right * Random.Range(0f, 10f);
				break;
			case DIRECTION.RIGHT:
				startPosition -= transform2.right * Random.Range(0f, 10f);
				break;
			case DIRECTION.STRAIGHT_DOWN:
				break;
			case DIRECTION.LEFT_OR_RIGHT:
				startPosition += transform2.right * Random.Range(-10f, 10f);
				break;
			case DIRECTION.RANDOM:
			{
				int num3 = Random.Range(1, 100);
				if (num3 < 40)
				{
					startPosition += transform2.right * 25f;
				}
				else if (num3 < 80)
				{
					startPosition -= transform2.right * 25f;
				}
				break;
			}
			case DIRECTION.TOWARD_PLAYER:
				startPosition += transform2.forward * 25f;
				break;
			}
			break;
		}
		case Meteor.TARGET.OFF_PATH:
		{
			startPosition = landPosition + Vector3.up * 15f;
			Transform transform = PlayerController.Instance().transform;
			startPosition += transform.forward * Random.Range(0f, 20f);
			switch (direction)
			{
			case DIRECTION.LEFT:
				startPosition -= transform.right * 10f;
				landPosition += transform.right * 40f;
				break;
			case DIRECTION.RIGHT:
				startPosition += transform.right * 10f;
				landPosition -= transform.right * 40f;
				break;
			case DIRECTION.STRAIGHT_DOWN:
				break;
			case DIRECTION.LEFT_OR_RIGHT:
			{
				int num2 = Random.Range(1, 100);
				if (num2 > 50)
				{
					startPosition -= transform.right * 10f;
					landPosition += transform.right * 40f;
				}
				else
				{
					startPosition += transform.right * 10f;
					landPosition -= transform.right * 40f;
				}
				break;
			}
			case DIRECTION.RANDOM:
			{
				int num = Random.Range(1, 100);
				if (num < 33)
				{
					startPosition -= transform.right * 10f;
					landPosition += transform.right * 40f;
				}
				else if (num < 66)
				{
					startPosition += transform.right * 10f;
					landPosition -= transform.right * 40f;
				}
				break;
			}
			case DIRECTION.TOWARD_PLAYER:
				startPosition += transform.forward * 25f;
				break;
			}
			break;
		}
		}
	}

	private void GenerateMeteor(bool generateDeadlyMeteors)
	{
		if ((TBFUtils.Is256mbDevice() && SecureStorage.Instance.HasSetLowGFXOption) || !(mCurrentPiece != null))
		{
			return;
		}
		bool flag = IsValidTheme(mCurrentPiece) && IsValidTheme(mCurrentPiece.GetNextPiece());
		PieceDescriptor pieceDescriptor = null;
		if (!flag)
		{
			return;
		}
		CurrentSmallMeteorInterval += Time.deltaTime;
		if (CurrentSmallMeteorInterval >= CurrentSmallMeteorDelay)
		{
			bool flag2 = true;
			float num = Random.Range(1f, 1.5f);
			if (Random.Range(0, 2) == 0)
			{
				num *= -1f;
			}
			float distance = mPathDistance + MeteorSpawnTimeAheadOfPlayer * Player.GetCurrentSpeed();
			pieceDescriptor = GetPieceAheadAtDistance(distance);
			if (flag2 && (bool)pieceDescriptor)
			{
				if (IsValidPieceToLandOn(pieceDescriptor))
				{
					Vector3 worldPosition = mCurrentPiece.GetWorldPosition(distance, num);
					DIRECTION direction = DIRECTION.RANDOM;
					if (mCurrentPiece.Theme == WorldConstructionHelper.Theme.Mountain || mCurrentPiece.Theme == WorldConstructionHelper.Theme.SlippedMountain)
					{
						direction = DIRECTION.STRAIGHT_DOWN;
					}
					Meteor.TARGET target = Meteor.TARGET.PATH;
					if (IsTileMaterialNone(distance, num))
					{
						target = Meteor.TARGET.OFF_PATH;
					}
					SpawnMeteor(worldPosition, false, Meteor.METEOR_TYPE.SMALL, target, direction, 1f, false);
					CurrentSmallMeteorInterval = 0f;
					CurrentSmallMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
				}
				else
				{
					CurrentSmallMeteorInterval = 0f;
					CurrentSmallMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
				}
			}
		}
		if (generateDeadlyMeteors)
		{
			float distance2 = MeteorSpawnTimeAheadOfPlayer * Player.GetCurrentSpeed();
			PieceDescriptor pieceAheadAtDistance = GetPieceAheadAtDistance(distance2);
			if (pieceAheadAtDistance != null && pieceAheadAtDistance.CanSpawnMeteor())
			{
				float leftRight = Random.Range(-0.5f, 0.5f);
				float distance3 = pieceAheadAtDistance.GetCachedLength() * 0.25f;
				Vector3 worldPosition2 = pieceAheadAtDistance.GetWorldPosition(distance3, leftRight);
				DIRECTION direction2 = DIRECTION.RANDOM;
				if (pieceAheadAtDistance.Theme == WorldConstructionHelper.Theme.Mountain || pieceAheadAtDistance.Theme == WorldConstructionHelper.Theme.SlippedMountain)
				{
					direction2 = DIRECTION.STRAIGHT_DOWN;
				}
				Meteor.TARGET target2 = Meteor.TARGET.PATH;
				SpawnMeteor(worldPosition2, true, Meteor.METEOR_TYPE.HUGE, target2, direction2, 1f, false);
				pieceAheadAtDistance.SpawnedMeteor();
			}
		}
		if (mCurrentPiece.Theme == WorldConstructionHelper.Theme.SlippedMountain)
		{
			return;
		}
		CurrentOffTrackLargeMeteorInterval += Time.deltaTime;
		if (CurrentOffTrackLargeMeteorInterval >= CurrentOffTrackLargeMeteorDelay)
		{
			float leftRight2 = Random.Range(-1.5f, 1.5f);
			float distance4 = mPathDistance + MeteorSpawnTimeAheadOfPlayer * Player.GetCurrentSpeed();
			Vector3 worldPosition3 = mCurrentPiece.GetWorldPosition(distance4, leftRight2);
			DIRECTION direction3 = DIRECTION.LEFT_OR_RIGHT;
			if (mCurrentPiece.Theme == WorldConstructionHelper.Theme.Mountain)
			{
				direction3 = DIRECTION.LEFT;
			}
			SpawnMeteor(worldPosition3, false, Meteor.METEOR_TYPE.LARGE, Meteor.TARGET.OFF_PATH, direction3, 1f, false);
			CurrentOffTrackLargeMeteorInterval = 0f;
			CurrentOffTrackLargeMeteorDelay = Random.Range(MinDelayBetweenMeteors, MaxDelayBetweenMeteors);
		}
	}

	private bool IsValidTile(float distance, float leftRight)
	{
		if (mCurrentPiece != null)
		{
			Tile tile = mCurrentPiece.GetTile(distance, leftRight, 0);
			if (tile != null)
			{
				return !tile.IsOfType(Tile.ResponseType.SwipeUp) && !tile.IsOfType(Tile.ResponseType.SwipeDown) && !tile.IsOfType(Tile.ResponseType.Kill);
			}
			return false;
		}
		return false;
	}

	private bool IsValidTheme(PieceDescriptor piece)
	{
		if (piece != null)
		{
			return piece.Theme == WorldConstructionHelper.Theme.Mountain || piece.Theme == WorldConstructionHelper.Theme.Bike || piece.Theme == WorldConstructionHelper.Theme.Jungle || piece.Theme == WorldConstructionHelper.Theme.SlippedMountain;
		}
		return false;
	}

	private PieceDescriptor GetPieceAheadAtDistance(float distance)
	{
		if (mCurrentPiece != null)
		{
			float num = 0f;
			PieceDescriptor nextPiece = mCurrentPiece;
			while (num + nextPiece.GetLength() < distance)
			{
				num += nextPiece.GetLength();
				nextPiece = nextPiece.GetNextPiece();
				if (nextPiece == null)
				{
					return null;
				}
			}
			if ((bool)nextPiece)
			{
				return nextPiece;
			}
		}
		return null;
	}

	private bool IsTurnPiece(PieceDescriptor piece)
	{
		if ((bool)piece)
		{
			return piece.GetPieceType() == WorldConstructionHelper.PieceType.Straight && (double)piece.Bend != 0.0;
		}
		return false;
	}

	private bool IsValidPieceToLandOn(PieceDescriptor piece)
	{
		if ((bool)piece)
		{
			return piece.GetPieceType() == WorldConstructionHelper.PieceType.Straight && !WorldConstructionHelper.IsPit(piece.GetPieceType()) && !WorldConstructionHelper.IsHazard(piece.GetPieceType()) && !WorldConstructionHelper.IsRopeSwing(piece.GetPieceType()) && !WorldConstructionHelper.IsTrackReduction(piece.GetPieceType()) && !WorldConstructionHelper.IsBlindHazard(piece.GetPieceType());
		}
		return false;
	}

	private bool IsThemeTransitionPiece(PieceDescriptor piece)
	{
		if ((bool)piece)
		{
			return piece.GetPieceType() == WorldConstructionHelper.PieceType.ThemeTransition;
		}
		return false;
	}

	private bool IsTileMaterialNone(float distance, float leftRight)
	{
		if (mCurrentPiece != null)
		{
			PathMaterial pathMaterial = mCurrentPiece.GetPathMaterial(distance, leftRight);
			return pathMaterial == PathMaterial.None;
		}
		return true;
	}

	private bool IsPlayingFair(PieceDescriptor currentPiece, float distance, float leftRight)
	{
		bool result = true;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		if ((bool)currentPiece)
		{
			PieceDescriptor pieceAheadAtDistance = GetPieceAheadAtDistance(distance);
			if ((bool)pieceAheadAtDistance)
			{
				PieceDescriptor pieceAheadAtDistance2 = GetPieceAheadAtDistance(distance - pieceAheadAtDistance.GetLength());
				if ((bool)pieceAheadAtDistance2)
				{
					flag = WorldConstructionHelper.IsRopeSwing(pieceAheadAtDistance2.GetPieceType());
					flag2 = pieceAheadAtDistance2.GetPieceType() == WorldConstructionHelper.PieceType.Straight_Feature;
					flag4 = WorldConstructionHelper.IsHazard(pieceAheadAtDistance2.GetPieceType());
					PieceDescriptor nextPiece = pieceAheadAtDistance.GetNextPiece();
					if ((bool)nextPiece)
					{
						flag5 = WorldConstructionHelper.IsHazard(pieceAheadAtDistance.GetNextPiece().GetPieceType());
					}
				}
				flag6 = Player.IsInSpeedBoostBonus();
				if (pieceAheadAtDistance.Theme == WorldConstructionHelper.Theme.Bike)
				{
					Tile tile = pieceAheadAtDistance.GetTile(distance, leftRight, 0);
					Tile tile2 = pieceAheadAtDistance.GetTile(distance, -1.5f, 0);
					Tile tile3 = pieceAheadAtDistance.GetTile(distance, 0f, 0);
					Tile tile4 = pieceAheadAtDistance.GetTile(distance, 1.5f, 0);
					if (tile2 != null && tile3 != null && tile4 != null)
					{
						int num = 0;
						if (tile2.IsOfType(Tile.ResponseType.Empty))
						{
							num++;
						}
						if (tile3.IsOfType(Tile.ResponseType.Empty))
						{
							num++;
						}
						if (tile4.IsOfType(Tile.ResponseType.Empty))
						{
							num++;
						}
						flag3 = (!tile.IsOfType(Tile.ResponseType.BlockLeft) || !tile.IsOfType(Tile.ResponseType.BlockRight)) && num <= 1;
					}
					else
					{
						flag3 = true;
					}
				}
			}
			result = !flag && !flag2 && !flag6 && !flag3 && !flag4 && !flag5;
		}
		return result;
	}

	public void DestroyAllMeteors()
	{
		for (int i = 0; i < 18; i++)
		{
			if (CachedMeteors[i].IsValid())
			{
				CachedMeteors[i].Destroy();
			}
		}
	}

	public void CreateTargetedMeteor(PieceDescriptor currentPiece, Meteor.TARGET target, Meteor.METEOR_TYPE type, DIRECTION direction, bool destroyObject)
	{
		if (!currentPiece)
		{
			return;
		}
		bool flag = false;
		PieceDescriptor pieceDescriptor = currentPiece;
		while ((bool)pieceDescriptor && !flag)
		{
			Transform transform = pieceDescriptor.transform.Find("Node_MeteorTarget");
			if ((bool)transform)
			{
				flag = true;
				Vector3 position = transform.position;
				SpawnMeteor(position, true, type, target, direction, pieceDescriptor.MeteorFlightTime, destroyObject);
				transform.parent = null;
				Object.Destroy(transform.gameObject);
			}
			if (!flag)
			{
				pieceDescriptor = pieceDescriptor.GetNextPiece();
			}
		}
	}
}
