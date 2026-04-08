using System.Collections;
using UnityEngine;

public class TutorialDialog : MonoBehaviour
{
	public SpriteText m_bodyText;

	public GameObject m_visibleContents;

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
		if (StateName != "Pause" && StateName != "Game")
		{
			Object.Destroy(base.gameObject);
		}
	}

	public IEnumerator Display(string Message, float Timeout)
	{
		if (m_visibleContents != null)
		{
			m_bodyText.Text = Message;
			m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		}
		yield return new WaitForSeconds(Timeout);
		if (m_visibleContents != null)
		{
			m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		}
		yield return new WaitForSeconds(0.33f);
		if (m_visibleContents != null)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
