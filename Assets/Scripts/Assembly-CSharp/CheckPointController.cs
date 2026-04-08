using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{
	public enum CHECKPOINT_TYPE
	{
		MACAW = 0,
		PURCHASABLE = 1,
		PURCHASED = 2,
		LOCKED = 3,
		NONE = 4
	}

	public static CheckPointController instance;

	public GameObject Gong;

	public GameObject Bird;

	private bool mBirdAtCheckpoint;

	private float mBirdSpawnDist = 120f;

	public GameObject GongEffect;

	private GameObject GongEffectInstance;

	private float mDistanceSinceLastMacawCheckPoint;

	private float mLastMacawCheckPointDistance;

	private int mCurrentMacawCheckPointNum;

	private bool mPassedMacawCheckPoint;

	private int mDistanceThisRun;

	private int mCurrentCheckPointEnabled;

	private GameObject mCurrentCheckPointGong;

	private GameObject mCurrentCheckPointBird;

	private GameObject mPurchaseBird;

	public float PieceSpawnTolerance = 50f;

	public PassedCheckpointDlg m_passedCheckDlgPrefab;

	private float[] mRuntimeDistanceBetweenCheckPoints;

	public CheckpointDefinition[] m_srcCheckpoints;

	public bool IsBirdAtCheckpoint
	{
		get
		{
			return mBirdAtCheckpoint;
		}
		set
		{
			mBirdAtCheckpoint = value;
		}
	}

	public static CheckPointController Instance()
	{
		return instance;
	}

	public int FirstCheckPointDistance()
	{
		return m_srcCheckpoints[0].m_distance;
	}

	public int FinalCheckPointDistance()
	{
		return m_srcCheckpoints[m_srcCheckpoints.Length - 1].m_distance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		mPassedMacawCheckPoint = false;
		mCurrentCheckPointGong = null;
		mLastMacawCheckPointDistance = 0f;
		mCurrentMacawCheckPointNum = 0;
		mDistanceThisRun = 0;
		mBirdAtCheckpoint = false;
		BuildRunTimeCheckpointList();
	}

	private void BuildRunTimeCheckpointList()
	{
		float num = 0f;
		float num2 = FirstCheckPointDistance();
		float num3 = FinalCheckPointDistance();
		List<float> list = new List<float>();
		for (int i = 0; i < m_srcCheckpoints.Length; i++)
		{
			num2 = m_srcCheckpoints[i].m_distance;
			switch (GetCheckPointTypeAt(num2))
			{
			case CHECKPOINT_TYPE.PURCHASED:
				list.Add(num2 - num);
				num = num2;
				break;
			case CHECKPOINT_TYPE.MACAW:
				list.Add(num2 - num);
				num = num2;
				break;
			}
		}
		mRuntimeDistanceBetweenCheckPoints = list.ToArray();
	}

	public int GetTotalPurchasableCheckPoints()
	{
		int num = 0;
		for (int i = 0; i < m_srcCheckpoints.Length; i++)
		{
			if (m_srcCheckpoints[i].m_type == CHECKPOINT_TYPE.PURCHASABLE)
			{
				num++;
			}
		}
		return num;
	}

	public float GetLastMacawCheckPointDistance()
	{
		return mLastMacawCheckPointDistance;
	}

	public float GetLastPurchasableCheckPointDistance()
	{
		return mDistanceThisRun;
	}

	public void SetLastMacawCheckPointDistance(float dist)
	{
		mLastMacawCheckPointDistance = dist;
	}

	public int GetValidCheckpointNumberFor(float distance)
	{
		int num = 0;
		for (int i = 0; i < mRuntimeDistanceBetweenCheckPoints.Length; i++)
		{
			num += (int)mRuntimeDistanceBetweenCheckPoints[i];
			if ((float)(num / 1000) == distance / 1000f)
			{
				return i + 1;
			}
		}
		return 0;
	}

	public int GetCurrentMacawCheckPointNum()
	{
		return mCurrentMacawCheckPointNum;
	}

	public void SetCurrentMacawCheckPointNum(int num)
	{
		mCurrentMacawCheckPointNum = num;
	}

	private void Update()
	{
		if ((!(GameController.Instance != null) || !GameController.Instance.IsPlayingTrialsMode) && mCurrentCheckPointBird != null)
		{
			CheckPointBird component = mCurrentCheckPointBird.GetComponent<CheckPointBird>();
			if (component != null && component.HasFinished())
			{
				Object.Destroy(mCurrentCheckPointBird);
				mCurrentCheckPointBird = null;
			}
		}
	}

	public void CheckAgainstPlayer(Vector3 playerPos, Vector3 playerForwards, float playerDistance)
	{
		Transform checkPointObject = null;
		Transform checkPointBird = null;
		if (!LevelGenerator.Instance().GetCheckPointObject(out checkPointObject, out checkPointBird))
		{
			return;
		}
		if (mCurrentCheckPointGong == null)
		{
			CreateDynamicObjects(checkPointObject, checkPointBird);
		}
		float num = Vector3.Dot(playerPos - checkPointObject.position, playerForwards);
		if (num > 0f)
		{
			if (!mPassedMacawCheckPoint)
			{
				mLastMacawCheckPointDistance = playerDistance;
				mPassedMacawCheckPoint = true;
				ThemeManager.Instance.OnPassedCheckpoint();
				TBFUtils.DebugLog("Check point distance set to " + playerDistance);
				mCurrentMacawCheckPointNum++;
			}
			return;
		}
		mPassedMacawCheckPoint = false;
		float num2 = 10f;
		if (!((playerPos - checkPointObject.position).sqrMagnitude < num2 * num2))
		{
			return;
		}
		CheckPoint component = mCurrentCheckPointGong.GetComponent<CheckPoint>();
		component.enabled = true;
		CheckPointBird component2 = mCurrentCheckPointBird.GetComponent<CheckPointBird>();
		component2.enabled = true;
		mCurrentCheckPointBird.transform.parent = null;
		if (!(GongEffectInstance == null) || !(GongEffect != null))
		{
			return;
		}
		GongEffectInstance = Object.Instantiate(GongEffect) as GameObject;
		GongEffectInstance.transform.position = mCurrentCheckPointGong.transform.position;
		GongEffectInstance.SetActiveRecursively(true);
		Component[] componentsInChildren = GongEffectInstance.GetComponentsInChildren(typeof(ParticleSystem), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = (ParticleSystem)array[i];
			if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
		Object.Destroy(GongEffectInstance, 2f);
	}

	public void CreateDynamicObjects(Transform checkPointTransform, Transform checkPointBirdTransform)
	{
		mCurrentCheckPointGong = Object.Instantiate(Gong) as GameObject;
		mCurrentCheckPointGong.SetActiveRecursively(true);
		mCurrentCheckPointGong.transform.parent = checkPointTransform;
		mCurrentCheckPointGong.transform.localPosition = new Vector3(0f, 0f, 0f);
		mCurrentCheckPointBird = Object.Instantiate(Bird) as GameObject;
		mCurrentCheckPointBird.SetActiveRecursively(true);
		mCurrentCheckPointBird.transform.parent = checkPointBirdTransform;
		mCurrentCheckPointBird.transform.localPosition = new Vector3(0f, 0f, 0f);
		mCurrentCheckPointBird.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	public void Generate(PieceDescriptor checkpointPiece)
	{
		checkpointPiece.CurrentCheckPointNum = mCurrentMacawCheckPointNum + 1;
	}

	public bool IsAvailable(PieceDescriptor fromPiece)
	{
		if (fromPiece == null)
		{
			return false;
		}
		if (mCurrentMacawCheckPointNum >= mRuntimeDistanceBetweenCheckPoints.Length || fromPiece.CurrentCheckPointNum >= mRuntimeDistanceBetweenCheckPoints.Length)
		{
			return false;
		}
		int currentCheckPointNum = fromPiece.CurrentCheckPointNum;
		float num = mRuntimeDistanceBetweenCheckPoints[currentCheckPointNum];
		for (int num2 = currentCheckPointNum - 1; num2 >= 0; num2--)
		{
			num += mRuntimeDistanceBetweenCheckPoints[num2];
		}
		if (fromPiece.TotalDistance + PieceSpawnTolerance >= num)
		{
			return true;
		}
		return false;
	}

	public void PlayerCheckpointChecks(Vector3 playerPos, Vector3 playerForwards, float playerDistance)
	{
		if (!(GameController.Instance != null) || !GameController.Instance.IsPlayingTrialsMode)
		{
			CheckAgainstPlayer(playerPos, playerForwards, playerDistance);
			UpdateForPurchasableCheckPointDistance(playerDistance);
		}
	}

	public void UpdateForPurchasableCheckPointDistance(float playerDistance)
	{
		int num = FindCheckpointForDistance(playerDistance);
		if (num != -1)
		{
			int distance = m_srcCheckpoints[num].m_distance;
			if (distance > mDistanceThisRun)
			{
				CHECKPOINT_TYPE checkPointTypeAt = GetCheckPointTypeAt(distance);
				if (checkPointTypeAt == CHECKPOINT_TYPE.PURCHASABLE || checkPointTypeAt == CHECKPOINT_TYPE.LOCKED)
				{
					mDistanceThisRun = distance;
					ShowPassedPurchaseableCheckpoint(distance);
					mBirdAtCheckpoint = false;
					ThemeManager.Instance.OnPassedCheckpoint();
				}
			}
		}
		UpdateMacawForPurchasableCheckPointDistance(playerDistance);
	}

	public void UpdateMacawForPurchasableCheckPointDistance(float playerDistance)
	{
		if (mBirdAtCheckpoint)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < m_srcCheckpoints.Length; i++)
		{
			if (playerDistance < (float)m_srcCheckpoints[i].m_distance)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		float num2 = m_srcCheckpoints[num].m_distance;
		if (!(playerDistance >= num2 - mBirdSpawnDist) || !(playerDistance <= (float)FinalCheckPointDistance()))
		{
			return;
		}
		CHECKPOINT_TYPE checkPointTypeAt = GetCheckPointTypeAt(num2);
		if (checkPointTypeAt == CHECKPOINT_TYPE.MACAW || checkPointTypeAt == CHECKPOINT_TYPE.PURCHASED)
		{
			return;
		}
		PieceDescriptor pieceDescriptor = LevelGenerator.Instance().GetCurrentPiece();
		float num3 = 0f - LevelGenerator.Instance().GetCurrentPathDistTravelled();
		while (num3 < num2 - playerDistance && pieceDescriptor != null)
		{
			num3 += pieceDescriptor.GetCachedLength();
			pieceDescriptor = pieceDescriptor.GetNextPiece();
		}
		if (pieceDescriptor != null)
		{
			mBirdAtCheckpoint = true;
			mPurchaseBird = Object.Instantiate(Bird) as GameObject;
			mPurchaseBird.SetActiveRecursively(true);
			float leftRight = 1.25f;
			Tile tile = pieceDescriptor.GetTile(0f, leftRight);
			if (tile.IsOfType(Tile.ResponseType.Kill))
			{
				leftRight = -1.25f;
			}
			Vector3 worldPosition = pieceDescriptor.GetWorldPosition(0f, leftRight);
			worldPosition.y += 3f;
			mPurchaseBird.transform.position = worldPosition;
			mPurchaseBird.transform.rotation = pieceDescriptor.transform.rotation;
			mPurchaseBird.transform.parent = pieceDescriptor.transform;
			Vector3 localEulerAngles = default(Vector3);
			localEulerAngles.Set(0f, 90f, 0f);
			mPurchaseBird.transform.localEulerAngles = localEulerAngles;
			mPurchaseBird.GetComponent<Animation>().Play("CheckpointPurchase_HoverIdle");
			mPurchaseBird.GetComponent<Animation>().wrapMode = WrapMode.Loop;
		}
	}

	public CHECKPOINT_TYPE GetCheckPointTypeAt(float distance)
	{
		int furthestDistanceTravelled = SecureStorage.Instance.FurthestDistanceTravelled;
		if (distance >= (float)FirstCheckPointDistance() && distance <= (float)FinalCheckPointDistance())
		{
			for (int i = 0; i < m_srcCheckpoints.Length; i++)
			{
				if (distance != (float)m_srcCheckpoints[i].m_distance)
				{
					continue;
				}
				if (m_srcCheckpoints[i].m_type == CHECKPOINT_TYPE.PURCHASABLE)
				{
					int num = m_srcCheckpoints[i].m_distance;
					if (i == 0)
					{
						num = 2000;
					}
					string identifier = "checkpoint." + num;
					if (distance > (float)furthestDistanceTravelled)
					{
						return CHECKPOINT_TYPE.LOCKED;
					}
					if (SecureStorage.Instance.GetItemCount(identifier) > 0)
					{
						return CHECKPOINT_TYPE.PURCHASED;
					}
					return CHECKPOINT_TYPE.PURCHASABLE;
				}
				if (m_srcCheckpoints[i].m_type == CHECKPOINT_TYPE.MACAW)
				{
					return CHECKPOINT_TYPE.MACAW;
				}
			}
		}
		return CHECKPOINT_TYPE.NONE;
	}

	public int FindCheckpointForDistance(float distance)
	{
		int result = -1;
		for (int i = 0; i < m_srcCheckpoints.Length && !(distance < (float)m_srcCheckpoints[i].m_distance); i++)
		{
			result = i;
		}
		return result;
	}

	public bool HasPlayerPassedValidCheckpoint()
	{
		float playerDist = SecureStorage.Instance.FurthestDistanceTravelled;
		float num = FindFurthestAvailableCheckpointForDistance(playerDist);
		return num != 0f;
	}

	public int GetPurchasableCheckpointImmediatelyPrevious()
	{
		int num = (int)PlayerController.Instance().Score().DistanceTravelled();
		int num2 = FindCheckpointForDistance(num);
		if (num2 != -1)
		{
			CHECKPOINT_TYPE checkPointTypeAt = GetCheckPointTypeAt(m_srcCheckpoints[num2].m_distance);
			if (checkPointTypeAt == CHECKPOINT_TYPE.PURCHASABLE)
			{
				return m_srcCheckpoints[num2].m_distance;
			}
		}
		return 0;
	}

	public bool IsSafeToSpawnPickup(float distanceTravelled, float maxPowerupDistance)
	{
		for (int i = 0; i < m_srcCheckpoints.Length; i++)
		{
			if (m_srcCheckpoints[i].m_type == CHECKPOINT_TYPE.MACAW)
			{
				float num = m_srcCheckpoints[i].m_distance;
				if (distanceTravelled > num - maxPowerupDistance && distanceTravelled < num)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ShowPassedPurchaseableCheckpoint(int Distance)
	{
		if (Distance < 2000)
		{
			Distance = 2000;
		}
		string identifier = "checkpoint." + Distance;
		if (!SecureStorage.Instance.IsCheckpointUnlocked(identifier))
		{
			PassedCheckpointDlg passedCheckpointDlg = (PassedCheckpointDlg)Object.Instantiate(m_passedCheckDlgPrefab);
			if (passedCheckpointDlg != null)
			{
				string message = Language.Get("S_PASSED_PURCHASE_CHECKPOINT");
				StartCoroutine(passedCheckpointDlg.Display(message, 2f));
			}
			SecureStorage.Instance.UnlockCheckpoint(identifier);
		}
	}

	public int NumCheckpointsPassed(float playerDistance)
	{
		int num = 0;
		for (int i = 0; i < m_srcCheckpoints.Length && !(playerDistance < (float)m_srcCheckpoints[i].m_distance); i++)
		{
			num++;
		}
		return num;
	}

	public float FindFurthestAvailableCheckpointForDistance(float playerDist)
	{
		float result = 0f;
		for (int num = m_srcCheckpoints.Length - 1; num >= 0; num--)
		{
			if (playerDist >= (float)m_srcCheckpoints[num].m_distance)
			{
				CHECKPOINT_TYPE checkPointTypeAt = GetCheckPointTypeAt(m_srcCheckpoints[num].m_distance);
				if (checkPointTypeAt == CHECKPOINT_TYPE.PURCHASED || checkPointTypeAt == CHECKPOINT_TYPE.MACAW)
				{
					result = m_srcCheckpoints[num].m_distance;
					break;
				}
			}
		}
		return result;
	}
}
