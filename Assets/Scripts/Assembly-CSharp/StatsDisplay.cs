using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
	public string ScoreText()
	{
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			PlayerScore playerScore = playerController.Score();
			if (playerScore != null)
			{
				return playerScore.TotalScoreXPAdjusted().ToString();
			}
		}
		return string.Empty;
	}

	public void Start()
	{
	}
}
