using System;
using System.Collections;
using UnityEngine;

public class UpdateXPBar : MonoBehaviour
{
	public SpriteText m_XPLowText;

	public SpriteText m_XPHighText;

	public UIStretchingPackedSprite m_XPBar;

	public ResultsDisplay m_Results;

	public UpdateXPDescription m_XPDescription;

	public SpriteText m_XPAmount;

	private float m_XPInc;

	private float m_XPPulseTime;

	private float m_WaitBeforeXPCountsUp;

	public float m_WaitBeforeUpdate = 1f;

	public float m_TotalTime = 2f;

	public int m_XPLastRun;

	private bool m_PauseForDialog;

	private bool m_Finished;

	private float m_fMaxOffsetForXPBarRight;

	public void Awake()
	{
		if ((bool)m_XPBar)
		{
			m_fMaxOffsetForXPBarRight = m_XPBar.m_Right.transform.position.x - m_XPBar.m_Left.transform.position.x;
		}
	}

	public void Start()
	{
		m_XPPulseTime = 0f;
		m_PauseForDialog = false;
		m_Finished = false;
	}

	public bool Finished()
	{
		return m_Finished;
	}

	public void OnEnable()
	{
		m_WaitBeforeXPCountsUp = m_WaitBeforeUpdate;
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null && GameController.Instance != null && (StateManager.Instance.ResultsFromGame() || GameController.Instance.IsPaused))
		{
			m_XPLastRun = playerController.Score().XPToAdd();
			m_XPInc = playerController.Score().NextXPLimit() - playerController.Score().CurrentXPLimit();
		}
		if (m_XPInc <= 1f)
		{
			m_XPInc = 1f;
		}
		m_Finished = false;
	}

	private void Update()
	{
		PlayerController playerController = PlayerController.Instance();
		if (!(playerController != null))
		{
			return;
		}
		PlayerScore playerScore = playerController.Score();
		if (playerScore == null)
		{
			return;
		}
		UpdatePulsingNumbers(playerScore);
		if (m_PauseForDialog || m_Finished)
		{
			return;
		}
		if (m_WaitBeforeXPCountsUp > 0f)
		{
			m_WaitBeforeXPCountsUp -= Time.deltaTime;
		}
		else
		{
			bool earnedDistinction = false;
			bool levelUp = false;
			int num = (int)(m_XPInc * Time.deltaTime);
			playerScore.IncrementXP(num, out earnedDistinction, out levelUp);
			if (levelUp)
			{
				m_XPPulseTime = (float)Math.PI * 4f / 5f;
			}
			if (levelUp)
			{
				m_Results.BroadcastMessage("OnLevelUp");
				m_XPDescription.BroadcastMessage("UpdateDescription");
				m_PauseForDialog = true;
			}
		}
		float t = ((float)playerScore.CurrentXP() - (float)playerScore.CurrentXPLimit()) / ((float)playerScore.NextXPLimit() - (float)playerScore.CurrentXPLimit());
		if ((bool)m_XPBar)
		{
			Vector3 position = m_XPBar.m_Right.transform.position;
			float x = m_XPBar.m_Left.transform.position.x;
			float to = x + m_fMaxOffsetForXPBarRight;
			position.x = Mathf.Lerp(x, to, t);
			m_XPBar.m_Right.transform.position = position;
			m_XPBar.Rebuild();
		}
		if (!(m_XPAmount != null))
		{
			return;
		}
		int num2 = playerScore.XPToAdd();
		if (DailyDoubleController.Instance.XPMultiplier != DailyDoubleController.XPMultipliers.NONE)
		{
			float f = 0f;
			switch (DailyDoubleController.Instance.XPMultiplier)
			{
			case DailyDoubleController.XPMultipliers.XP_X2:
				f = (float)num2 / 2f;
				break;
			case DailyDoubleController.XPMultipliers.XP_X3:
				f = (float)num2 / 3f;
				break;
			case DailyDoubleController.XPMultipliers.XP_X4:
				f = num2 / 4;
				break;
			}
			num2 = Mathf.RoundToInt(f);
		}
		if (DailyDoubleController.Instance.ShowResultsDDXPMult)
		{
			DailyDoubleController.Instance.ShowResultsDDXPMult = false;
			StartCoroutine(DisplayDDXPMultiplier());
		}
		m_XPAmount.Text = " " + string.Format(Language.Get("S_XP_INCREMENT"), m_XPLastRun - num2);
		if (num2 == 0)
		{
			m_Finished = true;
		}
	}

	private IEnumerator DisplayDDXPMultiplier()
	{
		yield return new WaitForSeconds(1f);
		switch (DailyDoubleController.Instance.XPMultiplier)
		{
		case DailyDoubleController.XPMultipliers.XP_X2:
			m_Results.XP_X2.active = true;
			break;
		case DailyDoubleController.XPMultipliers.XP_X3:
			m_Results.XP_X3.active = true;
			break;
		case DailyDoubleController.XPMultipliers.XP_X4:
			m_Results.XP_X4.active = true;
			break;
		}
		iTween.Stop(m_Results.XPScaler);
		m_Results.XPScaler.transform.localScale = Vector3.one;
		iTween.ScaleTo(m_Results.XPScaler, iTween.Hash("scale", new Vector3(1.3f, 1.3f, 1f), "time", 0.15f, "looptype", "pingPong"));
		MenuSFX.Instance.Play2D("DailyDoubleMarker");
	}

	private void UpdatePulsingNumbers(PlayerScore score)
	{
		if (m_XPPulseTime > 0f)
		{
			m_XPPulseTime = Mathf.Max(m_XPPulseTime - Time.deltaTime, 0f);
		}
		m_XPLowText.color.a = Mathf.Cos(m_XPPulseTime * 10f);
		m_XPHighText.color.a = Mathf.Cos(m_XPPulseTime * 10f);
		m_XPLowText.Text = string.Format(Language.Get("S_LVL_X"), score.XPLevel());
		m_XPHighText.Text = string.Format("{0}", score.XPLevel() + 1);
	}

	public void OnLevelUpDialogDismissed()
	{
		m_PauseForDialog = false;
	}
}
