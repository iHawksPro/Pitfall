using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HTTP;
using UnityEngine;

public class MobileNetworkGoogleAppImpl : MonoBehaviour, IMobileNetworkImpl
{
	private struct GALeaderboard
	{
		public int m_id;

		public string m_name;

		public bool m_isActive;
	}

	private bool m_initialized;

	private bool m_authenticated;

	private string m_username = string.Empty;

	private int m_productID = -1;

	private List<GALeaderboard> m_leaderboards;

	private string m_pendingBoardName;

	private int m_pendingBoardID;

	private long m_pendingScore;

	private int m_pendingStartRank;

	private int m_pendingEndRank;

	private static string m_apiKey = "d59dfa8629e048119783b9f617c130a5";

	private static string m_hostURI = "http://mobiboards.appspot.com/";

	private static string m_userIdHeader = "X-User-Id";

	public void Init()
	{
		Debug.Log("MobileNetworkGoogleAppImpl Initialized");
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
		bool flag = false;
		if (m_initialized && m_authenticated)
		{
			foreach (GALeaderboard leaderboard in m_leaderboards)
			{
				if (leaderboard.m_name == leaderboardID)
				{
					m_pendingBoardName = leaderboard.m_name;
					m_pendingBoardID = leaderboard.m_id;
					m_pendingScore = score;
					StartCoroutine("PostLeaderboardScore");
					flag = true;
					break;
				}
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
		bool flag = false;
		if (m_initialized && m_authenticated)
		{
			foreach (GALeaderboard leaderboard in m_leaderboards)
			{
				if (leaderboard.m_name == leaderboardID)
				{
					m_pendingBoardName = leaderboard.m_name;
					m_pendingBoardID = leaderboard.m_id;
					m_pendingStartRank = startRank;
					m_pendingEndRank = endRank;
					StartCoroutine("GetLeaderboardScores");
					flag = true;
					break;
				}
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

	private Request DoHttpGET(string URI)
	{
		Request request = new Request("get", URI);
		request.SetHeader(m_userIdHeader, Environment.UserName);
		request.Send();
		return request;
	}

	private Request DoHttpPOST(string URI, string Fields)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(Fields);
		Request request = new Request("post", URI, bytes);
		request.AddHeader(m_userIdHeader, Environment.UserName);
		request.AddHeader("content-type", "application/x-www-form-urlencoded");
		request.Send();
		return request;
	}

	private IEnumerator PlayerAuthenticate()
	{
		m_authenticated = false;
		string ProdUri = string.Format("{0}/client/product/{1}", m_hostURI, m_apiKey);
		int ProductID = -1;
		string RegisteredUsername = string.Empty;
		IList LeaderboardInfo = null;
		Request Req = DoHttpGET(ProdUri);
		while (!Req.isDone)
		{
			yield return null;
		}
		if (Req.response.status == 200)
		{
			IDictionary ProductInfo = (IDictionary)MiniJSON.jsonDecode(Req.response.Text);
			if (ProductInfo.Contains("name") && ProductInfo.Contains("id"))
			{
				Debug.Log(string.Format("Product: {0}, ID: {1}", ProductInfo["name"], ProductInfo["id"]));
				ProductID = int.Parse(ProductInfo["id"].ToString());
				RegisteredUsername = (string)ProductInfo["username"];
				LeaderboardInfo = (IList)ProductInfo["leaderboards"];
			}
		}
		if (ProductID != -1)
		{
			bool Success = true;
			if (RegisteredUsername == string.Empty)
			{
				string PostFields = string.Format("product_id={0}&username={1}", ProductID, Environment.UserName);
				string RegisterUri = string.Format("{0}/client/register", m_hostURI);
				Req = DoHttpPOST(RegisterUri, PostFields);
				while (!Req.isDone)
				{
					yield return null;
				}
				if (Req.response.status == 200)
				{
					IDictionary Registration = (IDictionary)MiniJSON.jsonDecode(Req.response.Text);
					Debug.Log(string.Format("Registered new user {0}", Registration["username"]));
				}
				else
				{
					Success = false;
				}
			}
			if (Success)
			{
				m_username = RegisteredUsername;
				m_productID = ProductID;
				m_leaderboards = new List<GALeaderboard>();
				GALeaderboard Board = default(GALeaderboard);
				foreach (IDictionary Item in LeaderboardInfo)
				{
					Board.m_id = int.Parse(Item["id"].ToString());
					Board.m_name = (string)Item["name"];
					Board.m_isActive = true;
					m_leaderboards.Add(Board);
				}
				m_authenticated = true;
				MobileNetworkManager.Instance._OnPlayerAuthenticated();
			}
			else
			{
				MobileNetworkManager.Instance._OnPlayerNotAuthenticated("Invalid user");
			}
		}
		else
		{
			MobileNetworkManager.Instance._OnPlayerNotAuthenticated("Invalid product");
		}
	}

	private IEnumerator PostLeaderboardScore()
	{
		string PostFields = string.Format("leaderboard_id={0}&score={1}", m_pendingBoardID, m_pendingScore);
		string Uri = string.Format("{0}/client/postscore", m_hostURI);
		Request Req = DoHttpPOST(Uri, PostFields);
		while (!Req.isDone)
		{
			yield return null;
		}
		if (Req.response.status == 200)
		{
			MobileNetworkManager.Instance._OnReportScoreFinished(Req.response.Text);
		}
		else
		{
			MobileNetworkManager.Instance._OnReportScoreFailed(Req.response.Text);
		}
	}

	private IEnumerator GetLeaderboardScores()
	{
		int nRanks = m_pendingEndRank - m_pendingStartRank;
		string Uri = string.Format("{0}/client/scorelist/{1}/{2}/{3}", m_hostURI, m_pendingBoardID, m_pendingStartRank - 1, nRanks);
		Request Req = DoHttpGET(Uri);
		while (!Req.isDone)
		{
			yield return null;
		}
		if (Req.response.status == 200)
		{
			IDictionary ScoreInfo = (IDictionary)MiniJSON.jsonDecode(Req.response.Text);
			int StartScoreRank = int.Parse(ScoreInfo["startRank"].ToString());
			IList ScoreList = (IList)ScoreInfo["scores"];
			List<MobileNetworkLeaderboardScore> OutScores = new List<MobileNetworkLeaderboardScore>();
			int CurrentRank = StartScoreRank + 1;
			foreach (IDictionary Item in ScoreList)
			{
				MobileNetworkLeaderboardScore NewScore = default(MobileNetworkLeaderboardScore);
				NewScore.FormattedValue = Item["score"].ToString();
				NewScore.Value = long.Parse(NewScore.FormattedValue);
				NewScore.Leaderboard = m_pendingBoardName;
				NewScore.IsFriend = false;
				NewScore.Rank = CurrentRank;
				NewScore.PlayerID = Item["name"].ToString();
				OutScores.Add(NewScore);
				CurrentRank++;
			}
			MobileNetworkManager.Instance._OnScoresLoaded(OutScores);
		}
		else
		{
			MobileNetworkManager.Instance._OnRetrieveScoresFailed("ERROR: HTTP request failed");
		}
	}
}
