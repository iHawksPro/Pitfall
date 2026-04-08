using System.Collections;
using UnityEngine;

public class WhatsNewDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public UIScrollList m_scrollList;

	public UIButton m_cancelButton;

	public GameObject m_visibleContents;

	public GameObject m_invisibleContents;

	public GameObject m_upArrow;

	public GameObject m_downArrow;

	private bool m_userCancelled;

	private float m_oldScrollPos;

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("MenuPopup");
		m_cancelButton.scriptWithMethodToInvoke = this;
		m_cancelButton.methodToInvoke = "OnDismiss";
		m_oldScrollPos = -1f;
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
		StartCoroutine(Deactivate());
	}

	private IEnumerator Deactivate()
	{
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display()
	{
		DialogManager.DialogLock();
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		yield return new WaitForSeconds(0.33f);
		m_userCancelled = false;
		while (!m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		DialogManager.DialogUnlock();
		yield return StartCoroutine(Deactivate());
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnDismiss();
		}
	}

	public void OnDismiss()
	{
		m_userCancelled = true;
		MenuSFX.Instance.Play2D("MenuCancel");
	}

	private void LateUpdate()
	{
		if (m_scrollList != null)
		{
			float scrollPosition = m_scrollList.ScrollPosition;
			if (scrollPosition != m_oldScrollPos)
			{
				bool activeRecursively = scrollPosition > 0.2f;
				bool activeRecursively2 = scrollPosition < 0.8f;
				m_upArrow.SetActiveRecursively(activeRecursively);
				m_downArrow.SetActiveRecursively(activeRecursively2);
				m_oldScrollPos = scrollPosition;
			}
		}
	}
}
