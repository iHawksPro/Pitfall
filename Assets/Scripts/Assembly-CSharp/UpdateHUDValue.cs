using UnityEngine;

public class UpdateHUDValue : MonoBehaviour
{
	protected SpriteText m_ValueText;

	public SpriteText m_CountdownText;

	protected string m_CountdownFormat = "{0}{1}";

	private int m_CurrentDisplay;

	protected int m_ValueToAdd;

	private float m_CurrentSpeed;

	private float m_Wait;

	private int m_ValueThisTime;

	protected void OnEnable()
	{
		m_CurrentDisplay = GetCurrentValue();
		m_ValueToAdd = GetValueToAdd();
		m_CurrentSpeed = m_ValueToAdd;
		if (GameController.Instance != null && StateManager.Instance != null && (GameController.Instance.IsPaused || !StateManager.Instance.ResultsFromGame()))
		{
			m_CurrentDisplay += m_ValueToAdd;
			m_ValueToAdd = 0;
		}
		m_ValueText = base.transform.GetComponent<SpriteText>();
		m_Wait = 1f;
		m_ValueThisTime = GetValueToAdd();
		CustomSetup();
	}

	public virtual int GetValueToAdd()
	{
		return 0;
	}

	public virtual int GetCurrentValue()
	{
		return 0;
	}

	public virtual string GetPadding()
	{
		return " ";
	}

	public virtual string GetUnits()
	{
		return string.Empty;
	}

	public virtual void CustomSetup()
	{
	}

	private void Update()
	{
		if (m_Wait <= 0f)
		{
			if (m_ValueToAdd > 0)
			{
				int b = Mathf.Max(1, (int)(m_CurrentSpeed * Time.deltaTime));
				int num = Mathf.Min(m_ValueToAdd, b);
				m_CurrentDisplay += num;
				m_ValueToAdd -= num;
			}
		}
		else
		{
			m_Wait -= Time.deltaTime;
		}
		if (m_CountdownText != null)
		{
			m_CountdownText.Text = string.Format(m_CountdownFormat, GetPadding(), m_ValueThisTime - m_ValueToAdd);
		}
		if (m_ValueText != null)
		{
			string text = string.Format("{0}", m_CurrentDisplay);
			m_ValueText.Text = GetPadding() + text + GetUnits();
		}
	}
}
