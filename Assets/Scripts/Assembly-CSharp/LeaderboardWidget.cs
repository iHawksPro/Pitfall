using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardWidget : MonoBehaviour
{
	public List<LeaderboardWidgetItem> m_WidgetList;

	public GameObject m_WorkingIcon;

	private void SetupDefaults()
	{
		foreach (LeaderboardWidgetItem widget in m_WidgetList)
		{
			widget.m_Name.Text = "- - -";
			widget.m_Value.Text = "--:--.--";
			widget.m_GiftButton.enabled = false;
			widget.m_MyBG.SetActiveRecursively(false);
			widget.m_OtherBG.SetActiveRecursively(true);
		}
	}

	public void SetupScores()
	{
		if (m_WorkingIcon != null)
		{
			m_WorkingIcon.SetActiveRecursively(false);
		}
		if (GameController.Instance != null)
		{
			if (TrialsDataManager.Instance.FindTrialState(GameController.Instance.m_CurrentTrialName) == null)
			{
				GenerateLeaderboardsForRun();
			}
			else
			{
				GenerateLeaderboardsForTrial(GameController.Instance.m_CurrentTrialName);
			}
		}
	}

	private void OnEnable()
	{
		SetupScores();
	}

	private void Start()
	{
	}

	private bool CanGetScores()
	{
		MobileNetworkManager instance = MobileNetworkManager.Instance;
		if (instance != null && instance.IsLoggedIn)
		{
			return true;
		}
		return false;
	}

	private void GenerateLeaderboardsForRun()
	{
		SetupDefaults();
		if (PlayerController.Instance() == null)
		{
			return;
		}
		if (m_WorkingIcon != null)
		{
			m_WorkingIcon.SetActiveRecursively(false);
		}
		if (!CanGetScores())
		{
			return;
		}
		int myIndex = ScoreRetriever.Instance().GetMyIndex();
		int num = Mathf.Max(0, myIndex - 2);
		int num2 = 0;
		for (int i = num; i < num + m_WidgetList.Count; i++)
		{
			ScoreRetriever.FriendDetails friendDetails = ScoreRetriever.Instance().RetreiveFriendAt(i);
			if (friendDetails.distance == 0 || num2 == m_WidgetList.Count)
			{
				break;
			}
			m_WidgetList[num2].m_Name.Text = friendDetails.name;
			m_WidgetList[num2].m_Value.Text = friendDetails.distance + "m";
			m_WidgetList[num2].m_Value.Hide(false);
			if (i == myIndex)
			{
				m_WidgetList[num2].m_MyBG.SetActiveRecursively(true);
				m_WidgetList[num2].m_OtherBG.SetActiveRecursively(false);
			}
			else
			{
				m_WidgetList[num2].m_MyBG.SetActiveRecursively(false);
				m_WidgetList[num2].m_OtherBG.SetActiveRecursively(true);
			}
			num2++;
		}
	}

	private IEnumerator TrialLeaderboardUpdate(string strID)
	{
		ScoreRetriever.Instance().GetScoresForTrial(strID, false);
		while (ScoreRetriever.Instance().IsLocked)
		{
			if (m_WorkingIcon != null)
			{
				m_WorkingIcon.SetActiveRecursively(true);
				for (int iIndex = 0; iIndex < m_WidgetList.Count; iIndex++)
				{
					if ((bool)m_WidgetList[iIndex].m_Root)
					{
						m_WidgetList[iIndex].m_Root.SetActiveRecursively(false);
					}
				}
			}
			yield return new WaitForSeconds(2f);
		}
		if (m_WorkingIcon != null)
		{
			m_WorkingIcon.SetActiveRecursively(false);
			for (int i = 0; i < m_WidgetList.Count; i++)
			{
				if ((bool)m_WidgetList[i].m_Root)
				{
					m_WidgetList[i].m_Root.SetActiveRecursively(true);
				}
			}
		}
		int iMyIndex = ScoreRetriever.Instance().GetMyTrialIndex(strID);
		int iStartIndex = Mathf.Max(0, iMyIndex - 2);
		SetupDefaults();
		int iActiveWidget = 0;
		for (int j = iStartIndex; j < iStartIndex + m_WidgetList.Count; j++)
		{
			ScoreRetriever.TrialScore aCurrentDetails = ScoreRetriever.Instance().RetreiveFriendAtForTrial(strID, j);
			Debug.Log(string.Format("TRIAL SCORE {0}: {1}, {2}", j, aCurrentDetails.name, aCurrentDetails.m_fTime));
			if (aCurrentDetails.m_fTime < 0f || iActiveWidget == m_WidgetList.Count)
			{
				break;
			}
			m_WidgetList[iActiveWidget].m_Name.Text = aCurrentDetails.name;
			m_WidgetList[iActiveWidget].m_Value.Text = TimeUtils.FloatToMMSShhString(aCurrentDetails.m_fTime);
			m_WidgetList[iActiveWidget].m_Value.Hide(false);
			if (j == iMyIndex)
			{
				m_WidgetList[iActiveWidget].m_MyBG.SetActiveRecursively(true);
				m_WidgetList[iActiveWidget].m_OtherBG.SetActiveRecursively(false);
			}
			else
			{
				m_WidgetList[iActiveWidget].m_MyBG.SetActiveRecursively(false);
				m_WidgetList[iActiveWidget].m_OtherBG.SetActiveRecursively(true);
			}
			iActiveWidget++;
		}
	}

	public void GenerateLeaderboardsForTrial(string strID)
	{
		SetupDefaults();
		if (PlayerController.Instance() == null)
		{
			return;
		}
		if (CanGetScores())
		{
			StartCoroutine(TrialLeaderboardUpdate(strID));
			return;
		}
		if (m_WorkingIcon != null)
		{
			m_WorkingIcon.SetActiveRecursively(false);
		}
		TrialsDataManager.TrialState trialState = TrialsDataManager.Instance.FindTrialState(strID);
		if (trialState.m_completed)
		{
			m_WidgetList[0].m_OtherBG.SetActiveRecursively(false);
			m_WidgetList[0].m_MyBG.SetActiveRecursively(true);
			m_WidgetList[0].m_Name.Text = PlayerController.instance.name;
			m_WidgetList[0].m_Value.Text = TimeUtils.FloatToMMSShhString(trialState.m_timeInSecs);
		}
	}

	private void Update()
	{
	}
}
