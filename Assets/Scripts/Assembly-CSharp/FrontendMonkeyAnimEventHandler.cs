using UnityEngine;

public class FrontendMonkeyAnimEventHandler : MonoBehaviour
{
	private class MonkeyEvent
	{
		public FrontendMonkey.MonkeyAnim m_anim;

		public float m_time;

		public string m_func;
	}

	private FrontendMonkey m_monkey;

	private MonkeyEvent[] m_eventDefs = new MonkeyEvent[3]
	{
		new MonkeyEvent
		{
			m_anim = FrontendMonkey.MonkeyAnim.MA_TITLE_IDLE_BREAK_0,
			m_time = 2.6333334f,
			m_func = "Screech"
		},
		new MonkeyEvent
		{
			m_anim = FrontendMonkey.MonkeyAnim.MA_TITLE_IDLE_BREAK_0,
			m_time = 3.1333334f,
			m_func = "Screech"
		},
		new MonkeyEvent
		{
			m_anim = FrontendMonkey.MonkeyAnim.MA_TITLE_IDLE_BREAK_0,
			m_time = 4.5333333f,
			m_func = "Screech"
		}
	};

	public void SetupAnimEvents(FrontendMonkey monkey)
	{
		m_monkey = monkey;
		for (int i = 0; i < m_eventDefs.Length; i++)
		{
			MonkeyEvent monkeyEvent = m_eventDefs[i];
			AnimationClip anim = m_monkey.GetAnim(monkeyEvent.m_anim);
			if (anim != null)
			{
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.time = monkeyEvent.m_time;
				animationEvent.functionName = monkeyEvent.m_func;
				anim.AddEvent(animationEvent);
			}
		}
	}

	private void Screech()
	{
		m_monkey.m_screechSfx.Play(m_monkey.gameObject);
	}

	private void Scared()
	{
		m_monkey.m_scaredSfx.Play(m_monkey.gameObject);
	}

	private void Tap()
	{
		m_monkey.m_tapSfx.Play(m_monkey.gameObject);
	}
}
