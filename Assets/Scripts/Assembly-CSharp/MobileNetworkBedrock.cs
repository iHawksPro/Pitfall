using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MobileNetworkBedrock : MonoBehaviour, IMobileNetworkImpl
{
	private bool m_initialized;

	private bool m_authenticated;

	private string m_username = string.Empty;

	private string m_pendingBoardName;

	private uint m_pendingBoardID;

	private int m_pendingScore;

	private ulong m_pendingStartRank;

	private ulong m_pendingEndRank;

	private Bedrock.brLeaderboardWriteType m_pendingWriteType;

	public void Init()
	{
		TBFUtils.DebugLog("MobileNetworkBedrock Initialized");
		m_initialized = true;
	}

	public bool IsMobileNetworkAvailable()
	{
		return m_initialized;
	}

	public bool AreTurnBasedMatchesAvailable()
	{
		return false;
	}

	public void AuthenticateLocalPlayer()
	{
		if (!m_authenticated)
		{
			StartCoroutine("PlayerAuthenticate");
		}
	}

	public bool IsPlayerAuthenticated()
	{
		return m_authenticated;
	}

	public string PlayerAlias()
	{
		return m_username;
	}

	public string PlayerIdentifier()
	{
		return "DummyPlayerIdentifier";
	}

	public void reportScore(long score, string leaderboardID)
	{
		if (!ScoreRetriever.Instance().CanUseLeaderboards)
		{
			return;
		}
		bool flag = false;
		if (m_initialized && m_authenticated)
		{
			m_pendingBoardID = Bedrock.Instance.Config.GetBedrockLeaderboardIDFor(leaderboardID);
			TBFUtils.DebugLog("POST: Leaderboard ID for " + leaderboardID + " = " + m_pendingBoardID);
			if (m_pendingBoardID != uint.MaxValue)
			{
				m_pendingBoardName = leaderboardID;
				m_pendingScore = (int)score;
				m_pendingWriteType = Bedrock.Instance.Config.GetWriteTypeFor(leaderboardID);
				StartCoroutine("PostLeaderboardScore");
				flag = true;
			}
			if (!flag)
			{
				MobileNetworkManager.Instance._OnReportScoreFailed("Error: No matching leaderboard");
			}
		}
		else
		{
			MobileNetworkManager.Instance._OnReportScoreFailed("Error: User not logged in");
		}
	}

	public void retrieveScores(bool friendsOnly, string leaderboardID, int startRank, int endRank)
	{
		if (!ScoreRetriever.Instance().CanUseLeaderboards)
		{
			return;
		}
		bool flag = false;
		if (m_initialized && m_authenticated)
		{
			m_pendingBoardID = Bedrock.Instance.Config.GetBedrockLeaderboardIDFor(leaderboardID);
			TBFUtils.DebugLog("GET: Leaderboard ID for " + leaderboardID + " = " + m_pendingBoardID);
			if (m_pendingBoardID != uint.MaxValue)
			{
				m_pendingBoardName = leaderboardID;
				m_pendingStartRank = (ulong)startRank;
				m_pendingEndRank = (ulong)endRank;
				StartCoroutine("GetLeaderboardScores");
				flag = true;
			}
			if (!flag)
			{
				MobileNetworkManager.Instance._OnReportScoreFailed("Error: No matching leaderboard");
			}
		}
		else
		{
			MobileNetworkManager.Instance._OnReportScoreFailed("Error: User not logged in");
		}
	}

	public void showLeaderboards()
	{
	}

	public bool supportsAchievements()
	{
		return false;
	}

	public void resetAchievements()
	{
	}

	public void reportAchievement(string achievementId, float percentComplete, bool showCompletionBanner)
	{
	}

	public void getAchievements()
	{
		MobileNetworkManager.Instance._OnAchievementLoadFailed("Achievements not supported");
	}

	public void showAchievements()
	{
	}

	private IEnumerator PlayerAuthenticate()
	{
		m_authenticated = false;
		MobileNetworkManager.Instance._OnPlayerAuthenticated();
		m_authenticated = true;
		yield return 0;
	}

	private IEnumerator PostLeaderboardScore()
	{
		BedrockWorker.Instance.WriteLeaderboardValue(m_pendingBoardID, m_pendingScore, m_pendingWriteType);
		yield return null;
	}

	private IEnumerator GetLeaderboardScores()
	{
		BedrockWorker.Instance.GetLeaderboardValuesByRank(m_pendingBoardID, m_pendingStartRank);
		while (!BedrockWorker.Instance.IsPullingScores)
		{
			yield return null;
		}
		Bedrock.brLeaderboardRow[] Results = BedrockWorker.Instance.ResultStore;
		List<MobileNetworkLeaderboardScore> OutScores = new List<MobileNetworkLeaderboardScore>();
		int CurrentRank = (int)m_pendingStartRank + 1;
		Bedrock.brLeaderboardRow[] array = Results;
		for (int i = 0; i < array.Length; i++)
		{
			Bedrock.brLeaderboardRow Res = array[i];
			MobileNetworkLeaderboardScore NewScore = default(MobileNetworkLeaderboardScore);
			if (Res._entityName != null)
			{
				NewScore.PlayerID = Encoding.Default.GetString(Res._entityName);
			}
			else
			{
				NewScore.PlayerID = "unknown";
			}
			if (Res._integerFields != null)
			{
				NewScore.Value = Res._integerFields[0];
			}
			else
			{
				NewScore.Value = 0L;
			}
			NewScore.Leaderboard = m_pendingBoardName;
			NewScore.IsFriend = false;
			NewScore.Rank = CurrentRank;
			OutScores.Add(NewScore);
			CurrentRank++;
		}
	}
}
