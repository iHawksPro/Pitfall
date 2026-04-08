using System.Collections;
using UnityEngine;

public class PassedCheckpointDlg : MonoBehaviour
{
	public SpriteText m_bodyText;

	public GameObject m_visibleContents;

	public GameObject m_onscreenPos;

	public GameObject m_offscreenPos;

	public GameObject m_ray;

	private void Awake()
	{
		m_visibleContents.transform.position = m_offscreenPos.transform.position;
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("CheckPointPurchaseAvailable");
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(string Message, float Timeout)
	{
		m_bodyText.Text = Message;
		yield return new WaitForSeconds(0.33f);
		m_visibleContents.gameObject.MoveTo(m_onscreenPos.transform.position, 0.33f, 0f, EaseType.easeInOutCubic);
		yield return new WaitForSeconds(Timeout + 0.33f);
		if (m_visibleContents != null && m_visibleContents.gameObject != null)
		{
			m_visibleContents.gameObject.MoveTo(m_offscreenPos.transform.position, 0.33f, 0f, EaseType.easeInOutCubic);
		}
		yield return new WaitForSeconds(0.33f);
		if ((bool)this && (bool)base.gameObject)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
