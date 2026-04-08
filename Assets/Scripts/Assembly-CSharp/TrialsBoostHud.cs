using System.Collections.Generic;
using UnityEngine;

public class TrialsBoostHud : MonoBehaviour
{
	public UIButton m_boostButton;

	public SpriteText m_GameTimer;

	public AutoSpriteBase m_UseBar;

	public AutoSpriteBase m_StaticParticle;

	public AutoSpriteBase m_RotateParticle;

	public List<AutoSpriteBase> m_ButtonList;

	private float m_fAlphaValue;

	private float m_fAlphaDecay = 0.1f;

	private int m_holdTouchId = -1;

	private void OnEnable()
	{
		m_holdTouchId = -1;
		m_boostButton.AddInputDelegate(OnBoostInput);
		UpdateBoostCount();
		UpdateBoostMeter();
		UpdateTimer();
	}

	private void OnDisable()
	{
		if (m_holdTouchId != -1 && InputController.Instance() != null)
		{
			InputController.Instance().DeregisterHoldTouch(m_holdTouchId);
			m_holdTouchId = -1;
		}
		m_boostButton.RemoveInputDelegate(OnBoostInput);
	}

	private void Update()
	{
		UpdateBoostCount();
		UpdateBoostMeter();
		UpdateTimer();
		m_fAlphaValue = Mathf.Max(0f, m_fAlphaValue - m_fAlphaDecay);
	}

	private void UpdateTimer()
	{
		if ((bool)m_GameTimer)
		{
			m_GameTimer.Text = TimeUtils.FloatToMMSShString(GameController.Instance.TimerValue);
		}
	}

	private void UpdateBoostMeter()
	{
		if (PlayerController.Instance() != null)
		{
			float num = PlayerController.Instance().TrialsBoost.TimeRemaining / TrialsDataManager.Instance.BoostTime;
			m_UseBar.SetSize(m_UseBar.width, 100f * num);
			m_StaticParticle.Color = new Color(1f, 1f, 1f, m_fAlphaValue);
			m_RotateParticle.Color = new Color(1f, 1f, 1f, m_fAlphaValue);
			if (num > 0.02f)
			{
				m_StaticParticle.transform.localPosition = new Vector3(0f, 100f * num * m_UseBar.transform.localScale.y + m_UseBar.transform.localPosition.y, -1f);
			}
			else
			{
				m_StaticParticle.transform.localPosition = new Vector3(-1000f, 0f, 0f);
			}
		}
	}

	private void UpdateBoostCount()
	{
		string text = string.Format("{0}/{1}", TrialsDataManager.Instance.NumBoostsAvailable, TrialsDataManager.Instance.MaxBoostsAvailable);
		for (int i = 0; i < TrialsDataManager.Instance.MaxBoostsAvailable; i++)
		{
			m_ButtonList[i].gameObject.SetActiveRecursively(i < TrialsDataManager.Instance.NumBoostsAvailable);
		}
	}

	private void OnBoostInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.NO_CHANGE || ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE || ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG)
		{
			return;
		}
		PlayerController playerController = PlayerController.Instance();
		if (playerController != null)
		{
			switch (ptr.evt)
			{
			case POINTER_INFO.INPUT_EVENT.PRESS:
				TBFUtils.DebugLog("BOOST Pressed");
				m_holdTouchId = ptr.id;
				InputController.Instance().RegisterHoldTouch(m_holdTouchId);
				playerController.TrialsBoost.Activate();
				m_fAlphaValue = 1f;
				m_fAlphaDecay = 0f;
				UpdateBoostCount();
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
			case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
				TBFUtils.DebugLog("BOOST Released");
				InputController.Instance().DeregisterHoldTouch(m_holdTouchId);
				m_holdTouchId = -1;
				playerController.TrialsBoost.Deactivate();
				m_fAlphaDecay = 0.1f;
				break;
			case POINTER_INFO.INPUT_EVENT.MOVE:
			case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
				break;
			}
		}
	}
}
