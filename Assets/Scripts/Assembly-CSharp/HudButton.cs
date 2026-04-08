using UnityEngine;

public class HudButton : MonoBehaviour
{
	private enum Mode
	{
		OffScreen = 0,
		ComingOnScreen = 1,
		ComingOnScreenBounce = 2,
		OnScreen = 3,
		GoingOffScreen = 4,
		Finished = 5
	}

	public float m_DelayBeforeStart;

	public float m_DelayBeforeExit;

	public GameObject m_BackButton;

	public GameObject m_FrontButton;

	public bool m_WaitForResults;

	private Mode m_Mode;

	private Vector3 m_InitialPos;

	private float m_Delay;

	private double m_LastRealTime;

	public void Awake()
	{
		base.transform.localPosition = new Vector3(GUIHelper.AdjustXForAspect(base.transform.localPosition.x, true), GUIHelper.AdjustYForAspect(base.transform.localPosition.y), base.transform.localPosition.z);
		m_InitialPos = base.transform.localPosition;
	}

	public virtual void Start()
	{
		m_Mode = Mode.OffScreen;
		if (m_BackButton != null)
		{
			m_BackButton.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		m_FrontButton.transform.localScale = new Vector3(0f, 0f, 0f);
	}

	public void OnEnable()
	{
		m_LastRealTime = Time.realtimeSinceStartup;
		m_Mode = Mode.OffScreen;
		if (m_BackButton != null)
		{
			m_BackButton.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		if (m_FrontButton != null)
		{
			m_FrontButton.transform.localScale = new Vector3(0f, 0f, 0f);
		}
		m_Delay = m_DelayBeforeStart;
	}

	private void GoOffScreen()
	{
		if (m_Mode == Mode.OnScreen)
		{
			m_Delay = m_DelayBeforeExit;
			m_Mode = Mode.GoingOffScreen;
		}
	}

	private void BringOnScreen()
	{
		if (m_Mode == Mode.Finished)
		{
			m_Delay = m_DelayBeforeStart;
			m_Mode = Mode.OffScreen;
		}
	}

	public void SetVisible(bool vis)
	{
		if (vis)
		{
			BringOnScreen();
		}
		else if (m_FrontButton.active)
		{
			GoOffScreen();
		}
	}

	public void SetInitialPos(Vector3 pos)
	{
		pos.z = m_InitialPos.z;
		m_InitialPos = pos;
	}

	public virtual void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = GUIHelper.AdjustYForAspect(m_InitialPos.y);
		if (localPosition != base.transform.localPosition)
		{
			base.transform.localPosition = localPosition;
		}
		double num = Time.realtimeSinceStartup;
		float value = (float)(num - m_LastRealTime);
		value = Mathf.Clamp(value, 0f, 0.05f);
		bool flag = false;
		if (m_WaitForResults && !ResultsDisplay.Instance.ResultsFinished())
		{
			flag = true;
		}
		switch (m_Mode)
		{
		case Mode.OffScreen:
			if (m_BackButton != null)
			{
				m_BackButton.transform.localScale = new Vector3(0f, 0f, 0f);
			}
			if (m_FrontButton != null)
			{
				m_FrontButton.transform.localScale = new Vector3(0f, 0f, 0f);
			}
			if (!flag)
			{
				m_Delay -= value;
				if (m_Delay <= 0f)
				{
					m_Mode = Mode.ComingOnScreen;
				}
			}
			break;
		case Mode.ComingOnScreen:
			if (m_BackButton != null)
			{
				iTween.ScaleTo(m_BackButton, iTween.Hash("ignoretimescale", true, "x", 1f, "y", 1f, "z", 1f, "time", 0.7f, "delay", 0f, "easeType", iTween.EaseType.easeOutElastic));
			}
			if (m_FrontButton != null)
			{
				iTween.ScaleTo(m_FrontButton, iTween.Hash("ignoretimescale", true, "x", 1f, "y", 1f, "z", 1f, "time", 1.2f, "delay", 0f, "easeType", iTween.EaseType.easeOutElastic));
			}
			m_Mode = Mode.OnScreen;
			break;
		case Mode.OnScreen:
			break;
		case Mode.GoingOffScreen:
			m_Delay -= value;
			if (m_Delay <= 0f)
			{
				if (m_BackButton != null)
				{
					iTween.ScaleTo(m_BackButton, iTween.Hash("ignoretimescale", true, "x", 0f, "y", 0f, "z", 0f, "time", 0.4f, "delay", 0f, "easeType", iTween.EaseType.easeInBounce));
				}
				if (m_FrontButton != null)
				{
					iTween.ScaleTo(m_FrontButton, iTween.Hash("ignoretimescale", true, "x", 0f, "y", 0f, "z", 0f, "time", 0.3f, "delay", 0f, "easeType", iTween.EaseType.easeInBounce));
				}
				m_Mode = Mode.Finished;
			}
			break;
		case Mode.Finished:
			m_Delay = m_DelayBeforeStart;
			break;
		case Mode.ComingOnScreenBounce:
			break;
		}
	}
}
