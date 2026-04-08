using UnityEngine;

public class HudMovement : MonoBehaviour
{
	private enum Mode
	{
		OffScreen = 0,
		ComingOnScreen = 1,
		OnScreen = 2,
		GoingOffScreen = 3,
		Finished = 4,
		Waiting = 5
	}

	public enum Direction
	{
		FromLeft = 0,
		FromRight = 1,
		FromTop = 2,
		FromBottom = 3
	}

	public float m_DelayBeforeStart;

	public float m_DelayBeforeExit;

	public float m_StartDistOffScreen = 2600f;

	public Direction m_TransitionDirection;

	public bool m_HasMovement = true;

	public bool m_WaitForResults;

	private Mode m_Mode;

	private float m_Delay;

	private Vector3 m_InitialPos;

	private Vector3 m_OffScreenPos;

	private float m_Direction;

	private double m_LastRealTime;

	public Vector3 InitialPos
	{
		get
		{
			return m_InitialPos;
		}
	}

	public virtual void Awake()
	{
		m_Mode = Mode.Finished;
		m_Direction = 1f;
		if (m_TransitionDirection == Direction.FromRight || m_TransitionDirection == Direction.FromTop)
		{
			m_Direction = -1f;
		}
		m_InitialPos = base.transform.localPosition;
		SetupOffScreenPos();
	}

	public void SetDirection(Direction d)
	{
		m_TransitionDirection = d;
		m_Direction = 1f;
		if (m_TransitionDirection == Direction.FromRight || m_TransitionDirection == Direction.FromTop)
		{
			m_Direction = -1f;
		}
	}

	private void SetupOffScreenPos()
	{
		m_OffScreenPos = m_InitialPos;
		if (m_TransitionDirection == Direction.FromLeft || m_TransitionDirection == Direction.FromRight)
		{
			m_OffScreenPos.x = m_InitialPos.x - m_StartDistOffScreen * m_Direction;
		}
		else
		{
			m_OffScreenPos.y = m_InitialPos.y - m_StartDistOffScreen * m_Direction;
		}
		base.transform.localPosition = m_OffScreenPos;
	}

	public void SetInitialPos(Vector3 pos)
	{
		pos.z = m_InitialPos.z;
		m_InitialPos = pos;
		SetupOffScreenPos();
	}

	public void OnDisable()
	{
		iTween.Stop(base.gameObject);
		m_Delay = m_DelayBeforeStart;
		m_Mode = Mode.OffScreen;
		base.transform.localPosition = m_InitialPos;
	}

	public void OnEnable()
	{
		m_LastRealTime = Time.realtimeSinceStartup;
		m_Delay = m_DelayBeforeStart;
		m_Mode = Mode.OffScreen;
		if (base.gameObject.name == "ResultTicker" && DailyDoubleController.Instance != null)
		{
			DailyDoubleController.Instance.TickerDiamond.SetActiveRecursively(false);
		}
	}

	public void GoOffScreen()
	{
		if (m_Mode == Mode.OnScreen)
		{
			m_Mode = Mode.GoingOffScreen;
		}
	}

	private void GoOffScreenSlow()
	{
		if (m_Mode == Mode.OnScreen || m_Mode == Mode.ComingOnScreen)
		{
			m_Mode = Mode.GoingOffScreen;
		}
	}

	private void SetOffScreen()
	{
		m_Mode = Mode.Waiting;
	}

	private void BringOnScreen()
	{
		m_Delay = m_DelayBeforeStart;
		m_Mode = Mode.OffScreen;
		base.transform.localPosition = m_OffScreenPos;
	}

	public virtual void Update()
	{
		if (!m_HasMovement)
		{
			return;
		}
		double num = Time.realtimeSinceStartup;
		float value = (float)(num - m_LastRealTime);
		value = Mathf.Clamp(value, 0f, 0.05f);
		m_LastRealTime = num;
		bool flag = false;
		if (m_WaitForResults && !ResultsDisplay.Instance.ResultsFinished())
		{
			flag = true;
		}
		switch (m_Mode)
		{
		case Mode.OffScreen:
			base.transform.localPosition = m_OffScreenPos;
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
			if (base.transform.gameObject != null)
			{
				iTween.MoveTo(base.transform.gameObject, iTween.Hash("islocal", true, "ignoretimescale", true, "x", m_InitialPos.x, "y", m_InitialPos.y, "time", 0.7f, "easeType", iTween.EaseType.easeOutElastic));
			}
			m_Mode = Mode.OnScreen;
			break;
		case Mode.OnScreen:
			m_Delay = m_DelayBeforeExit;
			break;
		case Mode.GoingOffScreen:
			m_Delay -= value;
			if (m_Delay <= 0f)
			{
				if (base.transform.gameObject != null)
				{
					iTween.MoveTo(base.transform.gameObject, iTween.Hash("islocal", true, "ignoretimescale", true, "x", m_OffScreenPos.x, "y", m_OffScreenPos.y, "time", 0.3f, "easeType", iTween.EaseType.easeInBack));
				}
				m_Mode = Mode.Finished;
			}
			break;
		case Mode.Finished:
			m_Delay = m_DelayBeforeStart;
			break;
		case Mode.Waiting:
			base.transform.localPosition = m_OffScreenPos;
			break;
		}
	}
}
