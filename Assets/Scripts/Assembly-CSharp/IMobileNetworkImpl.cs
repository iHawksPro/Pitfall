public interface IMobileNetworkImpl
{
	void Init();

	bool IsMobileNetworkAvailable();

	bool AreTurnBasedMatchesAvailable();

	void AuthenticateLocalPlayer();

	bool IsPlayerAuthenticated();

	string PlayerAlias();

	string PlayerIdentifier();

	void reportScore(long score, string leaderboardID);

	void retrieveScores(bool friendsOnly, string leaderboardID, int startRank, int endRank);

	void showLeaderboards();

	bool supportsAchievements();

	void resetAchievements();

	void reportAchievement(string achievementId, float percentComplete, bool showCompletionBanner);

	void getAchievements();

	void showAchievements();
}
