using System.Collections.Generic;
using UnityEngine;

public class BoostTonicBar : MonoBehaviour
{
	public List<AutoSpriteBase> m_ButtonList;

	public GameObject m_TonicRefreshObject;

	public GameObject m_TonicRefillButton;

	public SpriteText m_TimerText;

	public UIButton m_Button;

	private void Start()
	{
	}

	private void Update()
	{
		TrialsDataManager instance = TrialsDataManager.Instance;
		for (int i = 0; i < instance.MaxBoostsAvailable; i++)
		{
			if (i < instance.NumBoostsAvailable)
			{
				Vector3 localScale = m_ButtonList[i].gameObject.transform.localScale;
				localScale.x = Mathf.Min(localScale.x + 0.02f, 1f);
				localScale.y = Mathf.Min(localScale.x + 0.02f, 1f);
				localScale.z = 1f;
				m_ButtonList[i].gameObject.transform.localScale = localScale;
			}
			else
			{
				m_ButtonList[i].gameObject.transform.localScale = Vector3.zero;
			}
		}
		m_Button.controlIsEnabled = instance.NumBoostsAvailable < instance.MaxBoostsAvailable;
		m_TonicRefreshObject.SetActiveRecursively(instance.MaxBoostsAvailable != instance.NumBoostsAvailable);
		m_TonicRefillButton.SetActiveRecursively(instance.MaxBoostsAvailable != instance.NumBoostsAvailable);
		long num = instance.TimeUntilNextBoostRefill;
		long num2 = num % 60;
		long num3 = num / 60;
		m_TimerText.Text = num3.ToString("00") + ":" + num2.ToString("00");
	}
}
