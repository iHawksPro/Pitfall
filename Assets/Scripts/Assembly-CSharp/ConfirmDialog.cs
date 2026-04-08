using System;
using System.Collections;
using UnityEngine;

public class ConfirmDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public UIButton m_yesButton;

	public UIButton m_noButton;

	public UIButton m_cancelButton;

	public GameObject m_visibleContents;

	public GameObject m_invisibleContents;

	public GameObject m_DownArrow;

	private string m_identifier = string.Empty;

	private bool? m_userConfirmed;

	private bool m_userCancelled;

	private Vector3 m_OldButtonPos = new Vector3(0f, 0f, 0f);

	private GameObject m_TutorialButton;

	private GameObject m_DownArrowGameObject;

	private void Awake()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !BaseCampController.Instance.TutorialActive())
		{
			OnCancelPressed();
		}
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("MenuPopup");
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
		if (m_TutorialButton != null)
		{
			m_TutorialButton.transform.localPosition = m_OldButtonPos;
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		if (m_DownArrowGameObject != null)
		{
			m_DownArrowGameObject.SetActiveRecursively(false);
		}
		yield return new WaitForSeconds(0.33f);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(string Title, string Message, Action onYes, Action onNo)
	{
		yield return StartCoroutine(Display(Title, Message, string.Empty, string.Empty, onYes, onNo, onNo, string.Empty, null));
	}

	public IEnumerator Display(string Title, string Message, Action onYes, Action onNo, string Identifier)
	{
		yield return StartCoroutine(Display(Title, Message, string.Empty, string.Empty, onYes, onNo, onNo, Identifier, null));
	}

	public IEnumerator Display(string Title, string Message, string Opt1, string Opt2, Action onYes, Action onNo, Action onCancel)
	{
		yield return StartCoroutine(Display(Title, Message, Opt1, Opt2, onYes, onNo, onCancel, string.Empty, null));
	}

	public IEnumerator Display(string Title, string Message, string Opt1, string Opt2, Action onYes, Action onNo, Action onCancel, string Identifier, GameObject tutorialButton)
	{
		DialogManager.DialogLock();
		m_TutorialButton = tutorialButton;
		m_titleText.Text = Title;
		m_bodyText.Text = Message;
		if (Opt1 != string.Empty && m_yesButton != null)
		{
			m_yesButton.Text = Opt1;
		}
		if (Opt2 != string.Empty && m_noButton != null)
		{
			m_noButton.Text = Opt2;
		}
		m_userConfirmed = null;
		m_userCancelled = false;
		m_identifier = Identifier;
		if (m_invisibleContents != null && tutorialButton != null)
		{
			m_OldButtonPos = tutorialButton.transform.localPosition;
			m_TutorialButton.transform.localPosition = new Vector3(m_OldButtonPos.x, m_OldButtonPos.y, -190f);
			if (m_DownArrow != null)
			{
				if (tutorialButton.GetComponent<FlipPointerWhenPointedAt>() == null)
				{
					m_DownArrowGameObject = UnityEngine.Object.Instantiate(m_DownArrow, tutorialButton.transform.localPosition + new Vector3(base.transform.localPosition.x, 140f, -9f), Quaternion.identity) as GameObject;
				}
				else
				{
					m_DownArrowGameObject = UnityEngine.Object.Instantiate(m_DownArrow, tutorialButton.transform.localPosition + new Vector3(base.transform.localPosition.x, -200f, -9f), Quaternion.identity) as GameObject;
					m_DownArrowGameObject.transform.localScale = new Vector3(1f, -1f, 1f);
				}
				m_DownArrowGameObject.transform.parent = base.transform;
			}
		}
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userConfirmed.HasValue && !m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		if (m_DownArrowGameObject != null)
		{
			m_DownArrowGameObject.SetActiveRecursively(false);
		}
		if (m_TutorialButton != null)
		{
			m_TutorialButton.transform.localPosition = m_OldButtonPos;
		}
		float waitTime = Time.realtimeSinceStartup + 0.33f;
		while (Time.realtimeSinceStartup < waitTime)
		{
			yield return 0;
		}
		if (m_userCancelled)
		{
			if (onCancel != null)
			{
				onCancel();
			}
		}
		else if (m_userConfirmed.Value)
		{
			if (onYes != null)
			{
				onYes();
			}
		}
		else if (onNo != null)
		{
			onNo();
		}
		UnityEngine.Object.Destroy(base.gameObject);
		DialogManager.DialogUnlock();
	}

	private void OnNoPressed()
	{
		if (m_identifier != string.Empty)
		{
			SwrveEventsUI.MessageIgnored(m_identifier);
		}
		m_userConfirmed = false;
		MenuSFX.Instance.Play2D("MenuCancel");
	}

	private void OnYesPressed()
	{
		if (m_identifier != string.Empty)
		{
			SwrveEventsUI.MessageFollowed(m_identifier);
		}
		m_userConfirmed = true;
		MenuSFX.Instance.Play2D("MenuConfirm");
	}

	private void OnCancelPressed()
	{
		if (m_identifier != string.Empty)
		{
			SwrveEventsUI.MessageIgnored(m_identifier);
		}
		m_userCancelled = true;
		MenuSFX.Instance.Play2D("MenuCancel");
	}

	public void Dismiss()
	{
		m_userCancelled = true;
		MenuSFX.Instance.Play2D("MenuCancel");
	}
}
