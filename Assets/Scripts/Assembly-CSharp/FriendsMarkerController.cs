using UnityEngine;

public class FriendsMarkerController : MonoBehaviour
{
	public GameObject mFriendMarker;

	public PassedCheckpointDlg m_FriendDlgPrefab;

	private MarkerInfo mMarkerCurrent = default(MarkerInfo);

	private float m_fNextDistCheck;

	private bool m_bDoneFriends;

	private bool m_bDialogShown;

	private static FriendsMarkerController instance;

	private float m_fDistCheck = 100f;

	public static FriendsMarkerController Instance()
	{
		return instance;
	}

	public void Awake()
	{
		instance = this;
		m_bDialogShown = false;
	}

	public void Update()
	{
		if (StateManager.Instance.CurrentStateName != "Game" || (GameController.Instance != null && GameController.Instance.IsPlayingTrialsMode))
		{
			return;
		}
		if (!m_bDoneFriends && !mMarkerCurrent.m_bActive)
		{
			SetNextMarker();
		}
		if (!mMarkerCurrent.m_bActive)
		{
			return;
		}
		if (!mMarkerCurrent.m_bPlaced)
		{
			if (!(PlayerController.Instance().Score().DistanceTravelled() + m_fDistCheck > mMarkerCurrent.mMarkerDistance))
			{
				return;
			}
			PieceDescriptor pieceDescriptor = LevelGenerator.Instance().GetCurrentPiece();
			float num = 0f;
			float num2 = mMarkerCurrent.mMarkerDistance - PlayerController.Instance().Score().DistanceTravelled();
			if (!(pieceDescriptor != null))
			{
				return;
			}
			num += pieceDescriptor.GetCachedLength() - LevelGenerator.Instance().GetCurrentPathDistTravelled();
			if (num < 0f)
			{
				return;
			}
			for (; num < num2; num += pieceDescriptor.GetCachedLength())
			{
				if (!(pieceDescriptor != null))
				{
					break;
				}
				pieceDescriptor = pieceDescriptor.GetNextPiece();
				if (pieceDescriptor == null)
				{
					return;
				}
			}
			float distance = pieceDescriptor.GetCachedLength() - (num - num2);
			GameObject gameObject = Object.Instantiate(mFriendMarker) as GameObject;
			Vector3 worldPosition = pieceDescriptor.GetWorldPosition(distance, 0f);
			gameObject.transform.position = worldPosition;
			gameObject.transform.rotation = pieceDescriptor.transform.rotation;
			gameObject.transform.parent = pieceDescriptor.transform;
			Vector3 localEulerAngles = default(Vector3);
			localEulerAngles.Set(-90f, 90f, 0f);
			gameObject.transform.localEulerAngles = localEulerAngles;
			mMarkerCurrent.m_bPlaced = true;
		}
		else
		{
			if (!m_bDialogShown && PlayerController.Instance().Score().DistanceTravelled() > mMarkerCurrent.mMarkerDistance && PlayerController.Instance().Score().DistanceTravelled() < mMarkerCurrent.mMarkerDistance + 30f)
			{
				ShowFriendDialog(mMarkerCurrent.mMarkerName, (int)mMarkerCurrent.mMarkerDistance);
				m_bDialogShown = true;
			}
			if (PlayerController.Instance().Score().DistanceTravelled() - 100f > mMarkerCurrent.mMarkerDistance)
			{
				mMarkerCurrent.m_bActive = false;
			}
		}
	}

	private void SetNextMarker()
	{
		bool valid = false;
		ScoreRetriever.FriendDetails friendDetails = ScoreRetriever.Instance().RetrieveFriendAtDistance(m_fNextDistCheck, out valid);
		if (valid)
		{
			float markerDist = friendDetails.distance;
			CreateMarker(friendDetails.name, markerDist);
			m_fNextDistCheck += ScoreRetriever.Instance().GetChunkSize();
		}
	}

	public void CreateMarker(string markerName, float markerDist)
	{
		if (!mMarkerCurrent.m_bActive)
		{
			mMarkerCurrent.mMarkerName = markerName;
			mMarkerCurrent.mMarkerDistance = markerDist;
			mMarkerCurrent.m_bPlaced = false;
			mMarkerCurrent.m_bActive = true;
			Debug.Log("Marker dist = " + markerDist);
			m_bDialogShown = false;
		}
	}

	public MarkerInfo GetCurrentMarkerInfo()
	{
		return mMarkerCurrent;
	}

	public void Reset()
	{
		mMarkerCurrent.m_bActive = false;
		mMarkerCurrent.m_bPlaced = false;
		m_fNextDistCheck = 0f;
		m_bDoneFriends = false;
	}

	private void ShowFriendDialog(string name, int distance)
	{
		PassedCheckpointDlg passedCheckpointDlg = Object.Instantiate(m_FriendDlgPrefab) as PassedCheckpointDlg;
		if (passedCheckpointDlg != null)
		{
			string message = string.Format("{0}\n{1}m", name, distance);
			StartCoroutine(passedCheckpointDlg.Display(message, 4f));
		}
	}
}
