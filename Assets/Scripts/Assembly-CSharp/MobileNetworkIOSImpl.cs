using System.Collections.Generic;
using UnityEngine;

public class MobileNetworkIOSImpl : IMobileNetworkImpl
{
	private GameObject m_gameCenter;

	private List<GameCenterPlayer> m_friends = new List<GameCenterPlayer>();

	public void Init()
	{
		if (m_gameCenter == null)
		{
			m_gameCenter = new GameObject("GameCenterManager");
			m_gameCenter.AddComponent<GameCenterManager>();
			AddEventHandlers();
		}
		Debug.Log("MobileNetworkIOSImpl Initialized");
	}

	private void OnDisable()
	{
		if (m_gameCenter != null)
		{
			RemoveEventHandlers();
			Object.Destroy(m_gameCenter);
			m_gameCenter = null;
		}
	}

	public bool IsMobileNetworkAvailable()
	{
		return GameCenterBinding.isGameCenterAvailable();
	}

	public bool AreTurnBasedMatchesAvailable()
	{
		return false;
	}

	public void AuthenticateLocalPlayer()
	{
		if (!GameCenterBinding.isPlayerAuthenticated())
		{
			GameCenterBinding.authenticateLocalPlayer();
		}
	}

	public bool IsPlayerAuthenticated()
	{
		return GameCenterBinding.isPlayerAuthenticated();
	}

	public string PlayerAlias()
	{
		return GameCenterBinding.playerAlias();
	}

	public string PlayerIdentifier()
	{
		return GameCenterBinding.playerIdentifier();
	}

	public void reportScore(long score, string leaderboardID)
	{
		GameCenterBinding.reportScore(score, leaderboardID);
	}

	public void retrieveScores(bool friendsOnly, string leaderboardID, int startRank, int endRank)
	{
		GameCenterBinding.retrieveScores(friendsOnly, GameCenterLeaderboardTimeScope.AllTime, startRank, endRank, leaderboardID);
	}

	public void showLeaderboards()
	{
		GameCenterBinding.showLeaderboardWithTimeScope(GameCenterLeaderboardTimeScope.AllTime);
	}

	public bool supportsAchievements()
	{
		return GameCenterBinding.isGameCenterAvailable();
	}

	public void resetAchievements()
	{
		GameCenterBinding.resetAchievements();
	}

	public void reportAchievement(string achievementId, float percentComplete, bool showCompletionBanner)
	{
		GameCenterBinding.reportAchievement(achievementId, percentComplete, showCompletionBanner);
	}

	public void getAchievements()
	{
		GameCenterBinding.getAchievements();
	}

	public void showAchievements()
	{
		GameCenterBinding.showAchievements();
	}

	private void AddEventHandlers()
	{
		if (m_gameCenter != null)
		{
			GameCenterManager.loadPlayerDataFailed += OnGCLoadPlayerDataFailed;
			GameCenterManager.playerDataLoaded += OnGCPlayerDataLoaded;
			GameCenterManager.playerAuthenticated += OnGCPlayerAuthenticated;
			GameCenterManager.playerFailedToAuthenticate += OnGCPlayerFailedToAuthenticate;
			GameCenterManager.playerLoggedOut += OnGCPlayerLoggedOut;
			GameCenterManager.loadCategoryTitlesFailed += OnGCLoadCategoryTitlesFailed;
			GameCenterManager.categoriesLoaded += OnGCCategoriesLoaded;
			GameCenterManager.reportScoreFailed += OnGCReportScoreFailed;
			GameCenterManager.reportScoreFinished += OnGCReportScoreFinished;
			GameCenterManager.retrieveScoresFailed += OnGCRetrieveScoresFailed;
			GameCenterManager.scoresLoaded += OnGCScoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailed += OnGCRetrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoaded += OnGCScoresForPlayerIdLoaded;
			GameCenterManager.reportAchievementFailed += OnGCReportAchievementFailed;
			GameCenterManager.reportAchievementFinished += OnGCReportAchievementFinished;
			GameCenterManager.loadAchievementsFailed += OnGCLoadAchievementsFailed;
			GameCenterManager.achievementsLoaded += OnGCAchievementsLoaded;
			GameCenterManager.resetAchievementsFailed += OnGCResetAchievementsFailed;
			GameCenterManager.resetAchievementsFinished += OnGCResetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailed += OnGCRetrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoaded += OnGCAchievementMetadataLoaded;
		}
	}

	private void RemoveEventHandlers()
	{
		if (m_gameCenter != null)
		{
			GameCenterManager.loadPlayerDataFailed -= OnGCLoadPlayerDataFailed;
			GameCenterManager.playerDataLoaded -= OnGCPlayerDataLoaded;
			GameCenterManager.playerAuthenticated -= OnGCPlayerAuthenticated;
			GameCenterManager.playerFailedToAuthenticate -= OnGCPlayerFailedToAuthenticate;
			GameCenterManager.playerLoggedOut -= OnGCPlayerLoggedOut;
			GameCenterManager.loadCategoryTitlesFailed -= OnGCLoadCategoryTitlesFailed;
			GameCenterManager.categoriesLoaded -= OnGCCategoriesLoaded;
			GameCenterManager.reportScoreFailed -= OnGCReportScoreFailed;
			GameCenterManager.reportScoreFinished -= OnGCReportScoreFinished;
			GameCenterManager.retrieveScoresFailed -= OnGCRetrieveScoresFailed;
			GameCenterManager.scoresLoaded -= OnGCScoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailed -= OnGCRetrieveScoresForPlayerIdFailed;
			GameCenterManager.scoresForPlayerIdLoaded -= OnGCScoresForPlayerIdLoaded;
			GameCenterManager.reportAchievementFailed -= OnGCReportAchievementFailed;
			GameCenterManager.reportAchievementFinished -= OnGCReportAchievementFinished;
			GameCenterManager.loadAchievementsFailed -= OnGCLoadAchievementsFailed;
			GameCenterManager.achievementsLoaded -= OnGCAchievementsLoaded;
			GameCenterManager.resetAchievementsFailed -= OnGCResetAchievementsFailed;
			GameCenterManager.resetAchievementsFinished -= OnGCResetAchievementsFinished;
			GameCenterManager.retrieveAchievementMetadataFailed -= OnGCRetrieveAchievementMetadataFailed;
			GameCenterManager.achievementMetadataLoaded -= OnGCAchievementMetadataLoaded;
		}
	}

	private void OnGCPlayerAuthenticated()
	{
		MobileNetworkManager.Instance._OnPlayerAuthenticated();
		GameCenterBinding.retrieveFriends(false);
	}

	private void OnGCPlayerFailedToAuthenticate(string error)
	{
		MobileNetworkManager.Instance._OnPlayerNotAuthenticated(error);
	}

	private void OnGCPlayerLoggedOut()
	{
		Debug.Log("GameCenter playerLoggedOut");
		MobileNetworkManager.Instance._OnPlayerLoggedOut();
	}

	private void OnGCPlayerDataLoaded(List<GameCenterPlayer> players)
	{
		Debug.Log("GameCenter playerDataLoaded");
		m_friends.Clear();
		foreach (GameCenterPlayer player in players)
		{
			Debug.Log(player);
			m_friends.Add(player);
		}
	}

	private void OnGCLoadPlayerDataFailed(string error)
	{
		Debug.Log("GameCenter loadPlayerDataFailed: " + error);
	}

	private void OnGCCategoriesLoaded(List<GameCenterLeaderboard> leaderboards)
	{
		Debug.Log("categoriesLoaded");
		foreach (GameCenterLeaderboard leaderboard in leaderboards)
		{
			Debug.Log(leaderboard);
		}
	}

	private void OnGCLoadCategoryTitlesFailed(string error)
	{
		Debug.Log("loadCategoryTitlesFailed: " + error);
	}

	private void OnGCScoresLoaded(List<GameCenterScore> scores)
	{
		Debug.Log("scoresLoaded");
		foreach (GameCenterScore score in scores)
		{
			Debug.Log(score);
		}
		List<MobileNetworkLeaderboardScore> list = new List<MobileNetworkLeaderboardScore>();
		foreach (GameCenterScore score2 in scores)
		{
			list.Add(new MobileNetworkLeaderboardScore
			{
				FormattedValue = score2.formattedValue,
				Value = score2.value,
				Leaderboard = score2.category,
				IsFriend = score2.isFriend,
				Rank = score2.rank,
				PlayerID = score2.alias
			});
		}
		MobileNetworkManager.Instance._OnScoresLoaded(list);
	}

	private void OnGCRetrieveScoresFailed(string error)
	{
		Debug.Log("retrieveScoresFailed: " + error);
		MobileNetworkManager.Instance._OnRetrieveScoresFailed(error);
	}

	private void OnGCRetrieveScoresForPlayerIdFailed(string error)
	{
		Debug.Log("retrieveScoresForPlayerIdFailed: " + error);
	}

	private void OnGCScoresForPlayerIdLoaded(List<GameCenterScore> scores)
	{
		Debug.Log("scoresForPlayerIdLoaded");
		foreach (GameCenterScore score in scores)
		{
			Debug.Log(score);
		}
	}

	private void OnGCReportScoreFinished(string category)
	{
		Debug.Log("reportScoreFinished for category: " + category);
		MobileNetworkManager.Instance._OnReportScoreFinished(category);
	}

	private void OnGCReportScoreFailed(string error)
	{
		Debug.Log("reportScoreFailed: " + error);
		MobileNetworkManager.Instance._OnReportScoreFailed(error);
	}

	private void OnGCAchievementMetadataLoaded(List<GameCenterAchievementMetadata> achievementMetadata)
	{
		Debug.Log("achievementMetadatLoaded");
		foreach (GameCenterAchievementMetadata achievementMetadatum in achievementMetadata)
		{
			Debug.Log(achievementMetadatum);
		}
	}

	private void OnGCRetrieveAchievementMetadataFailed(string error)
	{
		Debug.Log("retrieveAchievementMetadataFailed: " + error);
	}

	private void OnGCResetAchievementsFinished()
	{
		Debug.Log("resetAchievmenetsFinished");
	}

	private void OnGCResetAchievementsFailed(string error)
	{
		Debug.Log("resetAchievementsFailed: " + error);
	}

	private void OnGCAchievementsLoaded(List<GameCenterAchievement> achievements)
	{
		Debug.Log("achievementsLoaded");
		List<MobileNetworkAchievement> list = new List<MobileNetworkAchievement>();
		foreach (GameCenterAchievement achievement in achievements)
		{
			list.Add(new MobileNetworkAchievement
			{
				AchievementID = achievement.identifier,
				IsComplete = achievement.completed,
				IsHidden = achievement.isHidden,
				PercentComplete = achievement.percentComplete
			});
		}
		MobileNetworkManager.Instance._OnAchievementsLoaded(list);
	}

	private void OnGCLoadAchievementsFailed(string error)
	{
		Debug.Log("loadAchievementsFailed: " + error);
		MobileNetworkManager.Instance._OnAchievementLoadFailed(error);
	}

	private void OnGCReportAchievementFinished(string identifier)
	{
		Debug.Log("reportAchievementFinished: " + identifier);
	}

	private void OnGCReportAchievementFailed(string error)
	{
		Debug.Log("reportAchievementFailed: " + error);
	}
}
