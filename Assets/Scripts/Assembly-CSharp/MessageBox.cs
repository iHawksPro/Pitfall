using System.Collections;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public GameObject m_button;

	public GameObject m_visibleContents;

	private bool? m_userConfirmed;

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
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(string Title, string Message)
	{
		yield return StartCoroutine(Display(Title, Message, null));
	}

	public IEnumerator Display(string Title, string Message, string Option)
	{
		m_titleText.Text = Title;
		m_bodyText.Text = Message;
		if (Option != null)
		{
			UIButton button = m_button.GetComponent<UIButton>();
			if (button != null)
			{
				button.Text = Option;
			}
		}
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userConfirmed.HasValue)
		{
			yield return new WaitForEndOfFrame();
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		Object.Destroy(base.gameObject);
	}

	private void OnButtonPressed()
	{
		m_userConfirmed = true;
	}
}
