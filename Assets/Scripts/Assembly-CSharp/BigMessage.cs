using System.Collections;
using UnityEngine;

public class BigMessage : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public GameObject m_visibleContents;

	private bool m_Cancel;

	private void Awake()
	{
		m_Cancel = false;
	}

	private void OnEnable()
	{
		m_Cancel = false;
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("BigMessage");
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	public void OnCancelPressed()
	{
		m_Cancel = true;
	}

	private void HandleStateDeactivated(string StateName)
	{
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(string Title, string Message, float time)
	{
		m_titleText.Text = Title;
		m_bodyText.Text = Message;
		if (m_visibleContents != null)
		{
			m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		}
		float pauseEndTime = Time.realtimeSinceStartup + time;
		while (Time.realtimeSinceStartup < pauseEndTime && !m_Cancel)
		{
			yield return 0;
		}
		if (m_visibleContents != null)
		{
			m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		}
		yield return new WaitForSeconds(0.33f);
		if ((bool)this)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
