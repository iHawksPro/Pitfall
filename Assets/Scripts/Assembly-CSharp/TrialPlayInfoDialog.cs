using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialPlayInfoDialog : MonoBehaviour
{
	public List<LevelObject> m_LevelObjects;

	public SpriteText m_titleText;

	public SpriteText m_GoldText;

	public SpriteText m_SilverText;

	public SpriteText m_BronzeText;

	public UIButton m_button;

	public GameObject m_visibleContents;

	public LeaderboardWidget m_Leaderboard;

	public SpriteText m_LeaderboardReplacement;

	private bool? m_userConfirmed;

	private bool m_userCancelled;

	private string m_strTrialID;

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(string strTrialID)
	{
		DialogManager.DialogLock();
		m_userCancelled = false;
		m_strTrialID = strTrialID;
		TrialsDataManager.TrialState aTrialState = TrialsDataManager.Instance.FindTrialState(m_strTrialID);
		m_titleText.Text = Language.Get(aTrialState.m_srcData.m_shorttitle);
		m_BronzeText.Text = TimeUtils.FloatToMMSShhString(aTrialState.m_srcData.m_fBronzeTime);
		m_SilverText.Text = TimeUtils.FloatToMMSShhString(aTrialState.m_srcData.m_fSilverTime);
		m_GoldText.Text = TimeUtils.FloatToMMSShhString(aTrialState.m_srcData.m_fGoldTime);
		TrialsDataManager.TrialState.eMedalType eBest = aTrialState.BestMedalType;
		for (int iTrophyBlock = 0; iTrophyBlock < m_LevelObjects.Count; iTrophyBlock++)
		{
			if (m_strTrialID.Contains(m_LevelObjects[iTrophyBlock].m_strContains))
			{
				m_LevelObjects[iTrophyBlock].m_GoldTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_SilverTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_BronzeTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_NoTrophy.SetActiveRecursively(false);
				switch (eBest)
				{
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_GOLD:
					m_LevelObjects[iTrophyBlock].m_GoldTrophy.SetActiveRecursively(true);
					break;
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_SILVER:
					m_LevelObjects[iTrophyBlock].m_SilverTrophy.SetActiveRecursively(true);
					break;
				case TrialsDataManager.TrialState.eMedalType.eMEDAL_BRONZE:
					m_LevelObjects[iTrophyBlock].m_BronzeTrophy.SetActiveRecursively(true);
					break;
				default:
					m_LevelObjects[iTrophyBlock].m_NoTrophy.SetActiveRecursively(true);
					break;
				}
			}
			else
			{
				m_LevelObjects[iTrophyBlock].m_GoldTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_SilverTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_BronzeTrophy.SetActiveRecursively(false);
				m_LevelObjects[iTrophyBlock].m_NoTrophy.SetActiveRecursively(false);
			}
		}
		if (ScoreRetriever.Instance().CanUseLeaderboards)
		{
			m_Leaderboard.GenerateLeaderboardsForTrial(m_strTrialID);
			m_LeaderboardReplacement.gameObject.SetActiveRecursively(false);
		}
		else
		{
			m_Leaderboard.gameObject.SetActiveRecursively(false);
			string aBestTime = "--:--.--";
			if (aTrialState.m_completed)
			{
				aBestTime = TimeUtils.FloatToMMSShhString(aTrialState.m_timeInSecs);
			}
			m_LeaderboardReplacement.Text = string.Format(Language.Get("S_LEADERBOARD_REPLACEMENT_TEXT"), aBestTime);
		}
		while (!m_userConfirmed.HasValue && !m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		Object.Destroy(base.gameObject);
		if (!m_userCancelled)
		{
			UIManager.instance.blockInput = true;
			StateRoot challengeState = StateManager.Instance.GetState("Challenges");
			challengeState.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
			StateRoot titleState = StateManager.Instance.GetState("Title");
			if ((bool)titleState)
			{
				titleState.gameObject.BroadcastMessage("OnLaunchTrials", m_strTrialID);
			}
			yield return new WaitForSeconds(3f);
		}
		DialogManager.DialogUnlock();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnCancelPressed();
		}
	}

	private void OnButtonPressed()
	{
		m_userConfirmed = true;
	}

	private void OnCancelPressed()
	{
		m_userCancelled = true;
	}
}
