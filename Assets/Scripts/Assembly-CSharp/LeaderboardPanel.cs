using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPanel : UIPanel
{
	public UIScrollList m_scrollList;

	public GameObject m_itemPrefab;

	private List<MobileNetworkLeaderboardScore> m_scores;

	public override void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		base.StartTransition(mode);
		if (mode == UIPanelManager.SHOW_MODE.BringInForward)
		{
			m_scrollList.ClearList(true);
			FetchScores();
		}
	}

	private void FetchScores()
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (instance != null && instance.IsLoggedIn)
		{
			EtceteraPlatformWrapper.ShowBlankWaiting();
			MobileNetworkManager.scoresLoaded += ScoresLoaded;
			instance.retrieveScores(false, "runner.distance", 1, 100);
			m_scores = null;
			StartCoroutine("WaitForScores");
		}
	}

	private void ScoresLoaded(List<MobileNetworkLeaderboardScore> Scores)
	{
		m_scores = Scores;
	}

	private IEnumerator WaitForScores()
	{
		while (m_scores == null)
		{
			yield return null;
		}
		foreach (MobileNetworkLeaderboardScore Score in m_scores)
		{
			UIListItemContainer NewItem = m_scrollList.CreateItem(m_itemPrefab) as UIListItemContainer;
			NewItem.GetTextElement("ScoreName").Text = Score.PlayerID;
			NewItem.GetTextElement("ScoreNumber").Text = Score.FormattedValue;
		}
		MobileNetworkManager.scoresLoaded -= ScoresLoaded;
		EtceteraPlatformWrapper.HideWaitingDialog();
	}
}
