using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDisplay : StateController
{
	public StatsDisplay m_statsDisplay;

	public ConfirmDialog m_QuitDialogPrefab;

	public UIButton m_ResumeButton;

	public List<GameObject> m_DisableNodes;

	private bool m_InputLockout;

	public override void Awake()
	{
		base.Awake();
	}

	protected override void OnStateLoaded()
	{
		HideState();
	}

	protected override IEnumerator AnimateStateIn()
	{
		base.gameObject.BroadcastMessage("BringOnScreen", SendMessageOptions.DontRequireReceiver);
		MenuSFX.Instance.Play2D("MenuBoxSwoosh");
		yield return 0;
		ShowState();
		m_ResumeButton.controlIsEnabled = true;
		m_InputLockout = false;
		for (int iNode = 0; iNode < m_DisableNodes.Count; iNode++)
		{
			if (m_DisableNodes[iNode] != null)
			{
				m_DisableNodes[iNode].SetActiveRecursively(false);
			}
		}
	}

	protected override IEnumerator AnimateStateOut()
	{
		yield return 0;
		HideState();
	}

	protected override void ShowState()
	{
		base.ShowState();
	}

	protected override void HideState()
	{
		base.HideState();
	}

	private void OnQuitPressed()
	{
		if (!m_InputLockout)
		{
			m_InputLockout = true;
			StartCoroutine(DoQuit());
		}
	}

	private IEnumerator DoQuit()
	{
		ConfirmDialog quitDialog = (ConfirmDialog)Object.Instantiate(m_QuitDialogPrefab);
		string Opt1 = Language.Get("S_YES");
		string Opt2 = Language.Get("S_NO");
		string Title = Language.Get("S_QUIT_SURE_TITLE");
		string Body = Language.Get("S_QUIT_SURE_BODY");
		yield return StartCoroutine(quitDialog.Display(Title, Body, Opt1, Opt2, OnQuitYes, OnQuitNo, OnQuitNo));
	}

	public void OnQuitYes()
	{
		SwrveEventsUI.PauseQuitButtonTouched();
		StartCoroutine(DoActualQuit());
	}

	private IEnumerator DoActualQuit()
	{
		MenuSFX.Instance.Play2D("MenuCancel");
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		float pauseEndTime = Time.realtimeSinceStartup + 0.5f;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		GameController.Instance.QuitGame();
		GameController.Instance.ReturnToTitleMenus();
	}

	public void OnQuitNo()
	{
		m_InputLockout = false;
	}

	private void OnResumePressed()
	{
		if (!m_InputLockout)
		{
			m_InputLockout = true;
			MenuSFX.Instance.Play2D("MenuConfirm");
			base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
			m_ResumeButton.controlIsEnabled = false;
			DialogManager.Instance.LaunchCountdown(OnCountdownComplete);
		}
	}

	private void OnCountdownComplete()
	{
		GameController.Instance.ResumeGame();
		GameController.Instance.ShowHUD(-1f);
	}
}
