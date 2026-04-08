using UnityEngine;

public class DistanceMapScript : MonoBehaviour
{
	public SpriteRoot MapSprite;

	public float DistanceMapHeight;

	public GameObject CheckPointPrefab;

	public GameObject LockedCheckPointPrefab;

	public GameObject PurchasedCheckPointPrefab;

	public GameObject MacawPrefab;

	public GameObject FriendTagPrefab;

	private float mDistanceAhead = 4000f;

	private CheckPointController.CHECKPOINT_TYPE lastType = CheckPointController.CHECKPOINT_TYPE.NONE;

	public Vector2 MarkerStartPosition = new Vector2(0f, 0f);

	private int mCurrentFriendIndex;

	private bool mSetUpInitialScores;

	private void Start()
	{
	}

	private void RestartRun(float distance)
	{
		DestroyMarkers("MacawMarkerPrefab(Clone)");
		DestroyMarkers("CheckPointPrefab(Clone)");
		DestroyMarkers("LockedCheckPointPrefab(Clone)");
		DestroyMarkers("OwnedCheckPointPrefab(Clone)");
		DestroyMarkers("FriendMarkerPrefab(Clone)");
		mSetUpInitialScores = false;
		SetUpStartingCheckpoints(distance);
		MarkerStartPosition = new Vector2(0f, DistanceMapHeight);
	}

	private void SetUpStartingCheckpoints(float distance)
	{
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			return;
		}
		float num = 0f;
		float num2 = distance;
		float num3 = num2 + mDistanceAhead;
		float num4 = num2;
		float num5 = 1f;
		if (num4 > mDistanceAhead)
		{
			num4 -= mDistanceAhead;
		}
		num4 = 0f;
		do
		{
			num5 = 1f;
			CheckPointController.CHECKPOINT_TYPE checkPointTypeAt = CheckPointController.Instance().GetCheckPointTypeAt(num2);
			num = num4 / mDistanceAhead;
			float num6 = DistanceMapHeight * num;
			float y = base.transform.position.y + num6;
			Vector2 vector = new Vector2(MarkerStartPosition.x, y);
			if (checkPointTypeAt != lastType)
			{
				MakeCheckpointIcon(checkPointTypeAt, vector, num2);
				if (checkPointTypeAt != CheckPointController.CHECKPOINT_TYPE.NONE)
				{
					num5 = ((!(num2 < 2000f)) ? 1000f : 100f);
				}
				lastType = checkPointTypeAt;
			}
			num4 += num5;
			num2 += num5;
		}
		while (num2 <= num3);
	}

	private void SetUpStartingFriendMarkers()
	{
		if (ScoreRetriever.Instance().ScoresAvailable() && ScoreRetriever.Instance().RequestLock())
		{
			float num = PlayerController.Instance().Score().DistanceTravelled();
			float num2 = num + mDistanceAhead;
			mCurrentFriendIndex = -1;
			for (; num < num2; num += 500f)
			{
				int indexForDistance = ScoreRetriever.Instance().GetIndexForDistance(num);
				if (mCurrentFriendIndex != indexForDistance)
				{
					bool valid = false;
					ScoreRetriever.FriendDetails friendDetails = ScoreRetriever.Instance().RetrieveFriendAtDistance(num, out valid);
					if (valid)
					{
						float friendDistance = friendDetails.distance;
						MakeFriendMarker(friendDetails.name, friendDistance);
						mCurrentFriendIndex = indexForDistance;
					}
				}
			}
		}
		ScoreRetriever.Instance().ReleaseLock();
	}

	private void Update()
	{
		if (GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode)
		{
			return;
		}
		if (ScoreRetriever.Instance().ScoresReturned() && !mSetUpInitialScores)
		{
			SetUpStartingFriendMarkers();
			mSetUpInitialScores = true;
		}
		PlayerController playerController = PlayerController.Instance();
		float num = playerController.Score().DistanceTravelled();
		if (playerController.IsDead() || GameController.Instance.Paused())
		{
			return;
		}
		MapSprite.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-16f, num / 4000f);
		float num2 = PlayerController.Instance().Score().DistanceTravelled();
		float num3 = num2 + mDistanceAhead;
		if (num2 < (float)CheckPointController.Instance().FinalCheckPointDistance())
		{
			int num4 = (int)num3 / 1000;
			int num5 = num4 * 1000;
			CheckPointController.CHECKPOINT_TYPE checkPointTypeAt = CheckPointController.Instance().GetCheckPointTypeAt(num5);
			if (checkPointTypeAt != lastType)
			{
				MakeCheckpointIcon(checkPointTypeAt, MarkerStartPosition, num3);
				lastType = checkPointTypeAt;
			}
		}
		if (ScoreRetriever.Instance().RequestLock())
		{
			int indexForDistance = ScoreRetriever.Instance().GetIndexForDistance(num3);
			if (indexForDistance != mCurrentFriendIndex)
			{
				bool valid = false;
				ScoreRetriever.FriendDetails friendDetails = ScoreRetriever.Instance().RetrieveFriendAtDistance(num3, out valid);
				if (valid)
				{
					float friendDistance = friendDetails.distance;
					MakeFriendMarker(friendDetails.name, friendDistance);
					mCurrentFriendIndex = indexForDistance;
				}
			}
		}
		ScoreRetriever.Instance().ReleaseLock();
	}

	private void MakeFriendMarker(string name, float friendDistance)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(FriendTagPrefab, MarkerStartPosition, Quaternion.identity);
		gameObject.transform.parent = base.transform;
		MarkerScript component = gameObject.GetComponent<MarkerScript>();
		component.SetUp(friendDistance, DistanceMapHeight);
		gameObject.transform.localPosition = new Vector3(0f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
	}

	private void MakeCheckpointIcon(CheckPointController.CHECKPOINT_TYPE type, Vector3 startPos, float distance)
	{
		GameObject gameObject = null;
		switch (type)
		{
		case CheckPointController.CHECKPOINT_TYPE.MACAW:
			gameObject = (GameObject)Object.Instantiate(MacawPrefab, startPos, Quaternion.identity);
			break;
		case CheckPointController.CHECKPOINT_TYPE.PURCHASABLE:
			gameObject = (GameObject)Object.Instantiate(CheckPointPrefab, startPos, Quaternion.identity);
			break;
		case CheckPointController.CHECKPOINT_TYPE.LOCKED:
			gameObject = (GameObject)Object.Instantiate(LockedCheckPointPrefab, startPos, Quaternion.identity);
			break;
		case CheckPointController.CHECKPOINT_TYPE.PURCHASED:
			gameObject = (GameObject)Object.Instantiate(PurchasedCheckPointPrefab, startPos, Quaternion.identity);
			break;
		}
		if (gameObject != null)
		{
			MarkerScript component = gameObject.GetComponent<MarkerScript>();
			component.SetUp(distance, GUIHelper.InvAdjustment() * DistanceMapHeight);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(-4f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
		}
	}

	private void DestroyMarkers(string name)
	{
		GameObject gameObject = null;
		bool flag = false;
		do
		{
			flag = false;
			gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				Object.DestroyImmediate(gameObject, true);
				flag = true;
			}
		}
		while (flag);
	}
}
