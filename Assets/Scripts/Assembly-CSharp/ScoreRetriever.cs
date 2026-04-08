using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRetriever : MonoBehaviour
{
	public class TrialScore
	{
		public string name;

		public float m_fTime;
	}

	public class TrialBlock
	{
		public string m_Identifier;

		public List<TrialScore> m_Scores = new List<TrialScore>();

		public uint m_uLastTimePulled;
	}

	public struct FriendDetails
	{
		public string name;

		public int distance;
	}

	private const int FriendDistanceChunkSize = 2000;

	private List<MobileNetworkLeaderboardScore> m_CurrentTrialScoreDownload;

	private List<MobileNetworkLeaderboardScore> m_scores;

	private List<FriendDetails> m_friends = new List<FriendDetails>();

	private List<TrialBlock> m_friendsTrials = new List<TrialBlock>();

	private static ScoreRetriever mInstance;

	private bool m_TestScoresAvailable;

	private bool m_ScoresReturned;

	private int NumFriendDistanceChunks = 100;

	private float m_DelayBeforeFetch;

	private bool m_Locked;

	private int[] m_ChosenFriends;

	public bool CanUseLeaderboards
	{
		get
		{
			return false;
		}
	}

	public bool IsLocked
	{
		get
		{
			return m_Locked;
		}
	}

	public static ScoreRetriever Instance()
	{
		return mInstance;
	}

	public bool ScoresAvailable()
	{
		return m_scores != null || m_TestScoresAvailable;
	}

	public bool ScoresReturned()
	{
		return m_ScoresReturned;
	}

	private void Awake()
	{
		if (TBFUtils.Is256mbDevice())
		{
			NumFriendDistanceChunks = 40;
		}
		mInstance = this;
		m_ChosenFriends = new int[NumFriendDistanceChunks];
		m_Locked = false;
	}

	private void Start()
	{
		m_DelayBeforeFetch = 10f;
		if (CanUseLeaderboards)
		{
			StartCoroutine(FetchScores());
		}
	}

	public void GetScoresForTrial(string strTrial, bool bForce)
	{
		if (!CanUseLeaderboards)
		{
			return;
		}
		bool flag = true;
		uint secondsSinceUnixEpoch = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
		foreach (TrialBlock friendsTrial in m_friendsTrials)
		{
			if (friendsTrial.m_Identifier == strTrial && (bForce || secondsSinceUnixEpoch - friendsTrial.m_uLastTimePulled < 300))
			{
				flag = false;
			}
		}
		if (flag)
		{
			StartCoroutine(FetchForTrial(strTrial));
		}
	}

	private IEnumerator FetchForTrial(string strTrial)
	{
		bool bFinished = false;
		while (true)
		{
			if (!StateManager.Instance.CanUpdateFriends())
			{
				yield return new WaitForEndOfFrame();
				continue;
			}
			if (RequestLock())
			{
				TBFUtils.DebugLog("ScoreTrialRetreiver --- Update");
				m_DelayBeforeFetch = 300f;
				MobileNetworkManager MN = MobileNetworkManager.Instance;
				if (MN != null)
				{
					if (MN.IsLoggedIn)
					{
						MobileNetworkManager.scoresLoaded += TrialsScoresLoaded;
						MN.retrieveScores(true, strTrial, 1, NumFriendDistanceChunks);
						m_CurrentTrialScoreDownload = null;
						m_ScoresReturned = false;
						StartCoroutine(WaitForTrialsScores(strTrial));
					}
					else
					{
						m_DelayBeforeFetch = 10f;
						ReleaseLock();
					}
				}
				else
				{
					ReleaseLock();
				}
				bFinished = true;
			}
			else
			{
				yield return new WaitForSeconds(2f);
			}
			if (!bFinished)
			{
				continue;
			}
			break;
		}
	}

	private IEnumerator FetchScores()
	{
		while (true)
		{
			yield return new WaitForSeconds(m_DelayBeforeFetch);
			while (!StateManager.Instance.CanUpdateFriends())
			{
				yield return new WaitForEndOfFrame();
			}
			if (!RequestLock())
			{
				continue;
			}
			TBFUtils.DebugLog("ScoreRetreiver --- Update");
			m_DelayBeforeFetch = 300f;
			MobileNetworkManager MN = MobileNetworkManager.Instance;
			if (MN != null)
			{
				if (MN.IsLoggedIn)
				{
					MobileNetworkManager.scoresLoaded += ScoresLoaded;
					MN.retrieveScores(true, "runner.distance", 1, NumFriendDistanceChunks);
					m_scores = null;
					m_ScoresReturned = false;
					StartCoroutine("WaitForScores");
				}
				else
				{
					m_DelayBeforeFetch = 10f;
					ReleaseLock();
				}
			}
			else
			{
				ReleaseLock();
			}
		}
	}

	private void CreateTestNames()
	{
		FriendDetails item = default(FriendDetails);
		int num = 0;
		item.name = "Harry";
		item.distance = 6080;
		m_friends.Insert(num++, item);
		item.name = "Barry";
		item.distance = 5990;
		m_friends.Insert(num++, item);
		item.name = "Garry";
		item.distance = 2480;
		m_friends.Insert(num++, item);
		item.name = "Larry";
		item.distance = 270;
		m_friends.Insert(num++, item);
		item.name = "Jill";
		item.distance = 260;
		m_friends.Insert(num++, item);
		item.name = "Bill";
		item.distance = 150;
		m_friends.Insert(num++, item);
		m_ScoresReturned = true;
		m_TestScoresAvailable = true;
	}

	private void CreateTestNamesForTrial(TrialBlock aBlock)
	{
		TrialScore trialScore = new TrialScore();
		int num = 0;
		trialScore.name = "Harry";
		trialScore.m_fTime = 60f;
		aBlock.m_Scores.Insert(num++, trialScore);
		trialScore.name = "Barry";
		trialScore.m_fTime = 50f;
		aBlock.m_Scores.Insert(num++, trialScore);
		trialScore.name = "Garry";
		trialScore.m_fTime = 40f;
		aBlock.m_Scores.Insert(num++, trialScore);
		trialScore.name = "Larry";
		trialScore.m_fTime = 27f;
		aBlock.m_Scores.Insert(num++, trialScore);
		trialScore.name = "Jill";
		trialScore.m_fTime = 26f;
		aBlock.m_Scores.Insert(num++, trialScore);
		trialScore.name = "Bill";
		trialScore.m_fTime = 10f;
		aBlock.m_Scores.Insert(num++, trialScore);
		m_ScoresReturned = true;
		m_TestScoresAvailable = true;
	}

	private void ScoresLoaded(List<MobileNetworkLeaderboardScore> Scores)
	{
		m_scores = Scores;
		m_ScoresReturned = true;
	}

	private void TrialsScoresLoaded(List<MobileNetworkLeaderboardScore> Scores)
	{
		m_CurrentTrialScoreDownload = Scores;
		m_ScoresReturned = true;
	}

	private void CreateChosenFriendsList()
	{
		int num = GetFriendQty();
		for (int i = 0; i < NumFriendDistanceChunks; i++)
		{
			int num2 = 2000 * (i + 1);
			List<int> list = new List<int>();
			while (num > 0)
			{
				num--;
				if (RetreiveFriendAt(num).distance < num2)
				{
					list.Add(num);
					continue;
				}
				num++;
				break;
			}
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				m_ChosenFriends[i] = list[index];
			}
			else
			{
				m_ChosenFriends[i] = -1;
			}
		}
	}

	public bool RequestLock()
	{
		if (m_Locked)
		{
			return false;
		}
		m_Locked = true;
		return true;
	}

	public void ReleaseLock()
	{
		m_Locked = false;
	}

	public FriendDetails RetrieveFriendAtDistance(float distance, out bool valid)
	{
		valid = false;
		if (CanUseLeaderboards && m_ScoresReturned && distance >= 0f && distance < (float)((NumFriendDistanceChunks - 1) * 2000))
		{
			int indexForDistance = GetIndexForDistance(distance);
			if (m_ChosenFriends[indexForDistance] >= 0)
			{
				valid = true;
				return RetreiveFriendAt(m_ChosenFriends[indexForDistance]);
			}
		}
		return default(FriendDetails);
	}

	public int GetIndexForDistance(float dist)
	{
		return (int)dist / 2000;
	}

	public int GetMyIndex()
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (instance != null && instance.IsLoggedIn)
		{
			for (int i = 0; i < m_friends.Count; i++)
			{
				if (m_friends[i].name.ToLower() == instance.PlayerAlias.ToLower())
				{
					return i;
				}
			}
		}
		return -1;
	}

	public int GetMyTrialIndex(string strTrialID)
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (instance != null && instance.IsLoggedIn)
		{
			foreach (TrialBlock friendsTrial in m_friendsTrials)
			{
				if (!(friendsTrial.m_Identifier == strTrialID))
				{
					continue;
				}
				for (int i = 0; i < friendsTrial.m_Scores.Count; i++)
				{
					if (friendsTrial.m_Scores[i].name.ToLower() == instance.PlayerAlias.ToLower())
					{
						return i;
					}
				}
				return -1;
			}
		}
		return -1;
	}

	public int GetChunkSize()
	{
		return 2000;
	}

	public int GetMaxDist()
	{
		return 2000 * NumFriendDistanceChunks;
	}

	private IEnumerator WaitForScores()
	{
		while (m_scores == null)
		{
			yield return null;
		}
		List<FriendDetails> newFriendList = new List<FriendDetails>();
		FriendDetails newFriend = default(FriendDetails);
		foreach (MobileNetworkLeaderboardScore Score in m_scores)
		{
			newFriend.name = Score.PlayerID;
			newFriend.distance = (int)Score.Value;
			newFriendList.Add(newFriend);
		}
		m_friends = newFriendList;
		TBFUtils.DebugLog("ScoreRetreiver ------ m_friend list -----");
		foreach (FriendDetails friend in m_friends)
		{
			TBFUtils.DebugLog("Name = " + friend.name.ToString() + "      Distance = " + friend.distance);
		}
		CreateChosenFriendsList();
		ReleaseLock();
		MobileNetworkManager.scoresLoaded -= ScoresLoaded;
	}

	private IEnumerator WaitForTrialsScores(string strIdentifier)
	{
		while (m_CurrentTrialScoreDownload == null && !m_ScoresReturned)
		{
			yield return null;
		}
		TrialBlock newTrialBlock = new TrialBlock
		{
			m_Identifier = strIdentifier,
			m_uLastTimePulled = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow)
		};
		foreach (MobileNetworkLeaderboardScore Score in m_CurrentTrialScoreDownload)
		{
			TrialScore newFriend = new TrialScore
			{
				name = Score.PlayerID,
				m_fTime = (float)Score.Value / 100f
			};
			newTrialBlock.m_Scores.Add(newFriend);
		}
		bool bHasTrialAlready = false;
		for (int iBlock = 0; iBlock < m_friendsTrials.Count; iBlock++)
		{
			if (m_friendsTrials[iBlock].m_Identifier == newTrialBlock.m_Identifier)
			{
				m_friendsTrials[iBlock] = newTrialBlock;
				bHasTrialAlready = true;
			}
		}
		if (!bHasTrialAlready)
		{
			m_friendsTrials.Add(newTrialBlock);
		}
		TrialsDataManager.TrialState aState = TrialsDataManager.Instance.FindTrialState(strIdentifier);
		if (aState.m_timeInSecs > 0f)
		{
			UpdateMyScoreForTrial(strIdentifier, aState.m_timeInSecs);
		}
		TBFUtils.DebugLog("ScoreTrialsRetreiver for " + newTrialBlock.m_Identifier + " ------ m_friend list -----");
		foreach (TrialScore friend in newTrialBlock.m_Scores)
		{
			TBFUtils.DebugLog("Name = " + friend.name.ToString() + "      Time = " + friend.m_fTime);
		}
		ReleaseLock();
		MobileNetworkManager.scoresLoaded -= TrialsScoresLoaded;
	}

	public FriendDetails RetreiveFriendAt(int index)
	{
		FriendDetails result = new FriendDetails
		{
			name = string.Empty,
			distance = 0
		};
		if (m_friends != null && index < m_friends.Count && index >= 0)
		{
			result.name = m_friends[index].name;
			result.distance = m_friends[index].distance;
		}
		return result;
	}

	public TrialScore RetreiveFriendAtForTrial(string strTrial, int index)
	{
		TrialScore trialScore = new TrialScore();
		trialScore.name = string.Empty;
		trialScore.m_fTime = -1f;
		foreach (TrialBlock friendsTrial in m_friendsTrials)
		{
			if (friendsTrial.m_Identifier == strTrial && index < friendsTrial.m_Scores.Count && index >= 0)
			{
				trialScore.name = friendsTrial.m_Scores[index].name;
				trialScore.m_fTime = friendsTrial.m_Scores[index].m_fTime;
				return trialScore;
			}
		}
		return trialScore;
	}

	public void UpdateMyRun(int iDistance)
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (!(instance != null) || m_friends == null)
		{
			return;
		}
		for (int i = 0; i < m_friends.Count; i++)
		{
			if (m_friends[i].name.ToLower() == instance.PlayerAlias.ToLower())
			{
				int distance = Mathf.Max(m_friends[i].distance, iDistance);
				FriendDetails value = new FriendDetails
				{
					name = m_friends[i].name,
					distance = distance
				};
				m_friends[i] = value;
				break;
			}
		}
		m_friends.Sort((FriendDetails p1, FriendDetails p2) => p2.distance.CompareTo(p1.distance));
	}

	public void UpdateMyScoreForTrial(string strTrial, float fTime)
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (!(instance != null) || string.IsNullOrEmpty(instance.PlayerAlias))
		{
			return;
		}
		TrialScore trialScore = new TrialScore();
		trialScore.name = instance.PlayerAlias;
		trialScore.m_fTime = fTime;
		for (int i = 0; i < m_friendsTrials.Count; i++)
		{
			TrialBlock trialBlock = m_friendsTrials[i];
			if (!(trialBlock.m_Identifier == strTrial))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < trialBlock.m_Scores.Count; j++)
			{
				TrialScore trialScore2 = trialBlock.m_Scores[j];
				if (!string.IsNullOrEmpty(trialScore2.name) && trialScore2.name.ToLower() == instance.PlayerAlias.ToLower())
				{
					trialScore2.m_fTime = Mathf.Min(trialScore2.m_fTime, fTime);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int k = 0; k < trialBlock.m_Scores.Count; k++)
				{
					if (trialBlock.m_Scores[k].m_fTime > fTime)
					{
						trialBlock.m_Scores.Insert(k, trialScore);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				trialBlock.m_Scores.Add(trialScore);
			}
			trialBlock.m_Scores.Sort((TrialScore p1, TrialScore p2) => p1.m_fTime.CompareTo(p2.m_fTime));
			return;
		}
		TrialBlock trialBlock2 = new TrialBlock();
		trialBlock2.m_Identifier = strTrial;
		trialBlock2.m_Scores.Add(trialScore);
		trialBlock2.m_uLastTimePulled = 0u;
		m_friendsTrials.Add(trialBlock2);
	}

	public string RetrieveFriendNameAt(int index)
	{
		if (m_friends != null && index < m_friends.Count)
		{
			return m_friends[index].name;
		}
		return string.Empty;
	}

	public int RetrieveFriendDistanceAt(int index)
	{
		if (m_friends != null && index < m_friends.Count)
		{
			return m_friends[index].distance;
		}
		return 0;
	}

	public FriendDetails GetLastFriend()
	{
		return RetreiveFriendAt(m_friends.Count - 1);
	}

	public int GetFriendQty()
	{
		if (m_friends != null)
		{
			return m_friends.Count;
		}
		return 0;
	}
}
