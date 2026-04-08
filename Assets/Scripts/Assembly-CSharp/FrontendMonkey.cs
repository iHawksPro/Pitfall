using System.Collections.Generic;
using UnityEngine;

public class FrontendMonkey : MonoBehaviour
{
	public enum MonkeyAnim
	{
		MA_TITLE_IDLE = 0,
		MA_VOLCANO_REACT = 1,
		MA_TITLE_TO_GAME = 2,
		MA_TITLE_IDLE_BREAK_0 = 3,
		MA_TITLE_IDLE_BREAK_1 = 4,
		MA_TITLE_IDLE_BREAK_2 = 5,
		MA_TITLE_IDLE_BREAK_3 = 6,
		MA_TITLE_TAP_0 = 7,
		MA_TITLE_TAP_1 = 8,
		MA_TITLE_TAP_2 = 9
	}

	public GameObject m_offScreenPos;

	public float m_idleBreakTime = 5f;

	[HideInInspector]
	public GameObject m_monkeyModel;

	private bool m_started;

	public SoundFXData m_screechSfx;

	public SoundFXData m_scaredSfx;

	public SoundFXData m_tapSfx;

	private string[] m_animNames;

	private void Start()
	{
		if (m_started)
		{
			return;
		}
		bool flag = TBFUtils.Is256mbDevice();
		string path = ((!flag) ? "Frontend/Monkey_FrontEnd_Prefab" : "Frontend/Monkey_FrontEnd_Lite_Prefab");
		m_monkeyModel = null;
		GameObject gameObject = (GameObject)Resources.Load(path);
		if (gameObject != null)
		{
			m_monkeyModel = (GameObject)Object.Instantiate(gameObject);
			m_monkeyModel.transform.position = m_offScreenPos.transform.position;
			m_monkeyModel.transform.rotation = m_offScreenPos.transform.rotation;
			m_monkeyModel.transform.parent = base.transform;
			m_animNames = GetAnimNames(m_monkeyModel.GetComponent<Animation>());
			if (!flag)
			{
				FrontendMonkeyAnimEventHandler frontendMonkeyAnimEventHandler = m_monkeyModel.AddComponent<FrontendMonkeyAnimEventHandler>();
				frontendMonkeyAnimEventHandler.SetupAnimEvents(this);
			}
		}
		m_started = true;
	}

	private static string[] GetAnimNames(Animation anim)
	{
		List<string> list = new List<string>();
		foreach (AnimationState item in anim)
		{
			list.Add(item.name);
		}
		return list.ToArray();
	}

	private void PlayAnim(MonkeyAnim anim, QueueMode queueMode)
	{
		PlayAnim(anim, queueMode, false);
	}

	private void PlayAnim(MonkeyAnim anim, QueueMode queueMode, bool NoTransition)
	{
		if (m_monkeyModel != null)
		{
			if (NoTransition)
			{
				m_monkeyModel.GetComponent<Animation>().Play(m_animNames[(int)anim], PlayMode.StopAll);
			}
			else
			{
				m_monkeyModel.GetComponent<Animation>().CrossFadeQueued(m_animNames[(int)anim], 0.3f, queueMode);
			}
		}
	}

	public AnimationClip GetAnim(MonkeyAnim anim)
	{
		AnimationClip result = null;
		if (m_monkeyModel != null)
		{
			result = m_monkeyModel.GetComponent<Animation>().GetClip(m_animNames[(int)anim]);
		}
		return result;
	}

	public float GetAnimLength(MonkeyAnim anim)
	{
		AnimationClip anim2 = GetAnim(anim);
		if (anim2 == null)
		{
			return 0f;
		}
		return anim2.length;
	}

	public float IpadToTitleLen()
	{
		if (!m_started)
		{
			Start();
		}
		return GetAnimLength(MonkeyAnim.MA_VOLCANO_REACT);
	}

	public float IpadToTitle()
	{
		if (!m_started)
		{
			Start();
		}
		float animLength = GetAnimLength(MonkeyAnim.MA_VOLCANO_REACT);
		PlayAnim(MonkeyAnim.MA_VOLCANO_REACT, QueueMode.PlayNow, true);
		PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.CompleteOthers);
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
		return animLength;
	}

	public void RunOnScreen()
	{
		if (!m_started)
		{
			Start();
		}
		PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.PlayNow, true);
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
	}

	private void IdleBreak()
	{
		if (m_monkeyModel != null && m_monkeyModel.GetComponent<Animation>().IsPlaying(m_animNames[0]))
		{
			int num = 3;
			int num2 = 7;
			MonkeyAnim anim = (MonkeyAnim)Random.Range(num, num + num2);
			PlayAnim(anim, QueueMode.PlayNow);
			PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.CompleteOthers);
		}
	}

	public void TitleToGame()
	{
		PlayAnim(MonkeyAnim.MA_TITLE_TO_GAME, QueueMode.PlayNow);
		CancelInvoke("IdleBreak");
	}

	public void StopAllAnims()
	{
		CancelInvoke("IdleBreak");
		if (m_monkeyModel != null)
		{
			m_monkeyModel.GetComponent<Animation>().Stop();
		}
	}
}
