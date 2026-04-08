using UnityEngine;

public class NewIndicator : MonoBehaviour
{
	public float m_pulseInterval = 3f;

	private bool m_isNew;

	public bool IsNew
	{
		get
		{
			return m_isNew;
		}
		set
		{
			if (m_isNew != value)
			{
				m_isNew = value;
				UpdateIndicator();
			}
		}
	}

	private void OnEnable()
	{
		UpdateIndicator();
	}

	private void OnDisable()
	{
		ClearNew();
	}

	private void UpdateIndicator()
	{
		if (m_isNew)
		{
			SetNew();
		}
		else
		{
			ClearNew();
		}
	}

	private void PulseIndicator()
	{
		CommonAnimations.AnimateButton(base.gameObject);
	}

	private void SetNew()
	{
		GetComponent<SpriteRoot>().Hide(false);
		InvokeRepeating("PulseIndicator", m_pulseInterval, m_pulseInterval);
	}

	private void ClearNew()
	{
		CancelInvoke("PulseIndicator");
		GetComponent<SpriteRoot>().Hide(true);
	}
}
