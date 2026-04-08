using System.Collections;
using UnityEngine;

public class ChallengesController : StateController
{
	public SpriteText m_gemsText;

	public SpriteText m_coinsText;

	public AutoSpriteControlBase[] m_buttons;

	public RefillBoostTonicDialog m_RefillDialogPrefab;

	public TrialPlayInfoDialog m_PlayInfoDialogPrefab;

	public MessageBox m_messageBoxPrefab;

	private static ChallengesController m_instance;

	public static ChallengesController Instance
	{
		get
		{
			return m_instance;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (m_instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			m_instance = this;
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < m_buttons.Length; i++)
		{
			if (m_buttons[i] != null)
			{
				m_buttons[i].AddValueChangedDelegate(OnChallengePressed);
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < m_buttons.Length; i++)
		{
			if (m_buttons[i] != null)
			{
				m_buttons[i].RemoveValueChangedDelegate(OnChallengePressed);
			}
		}
	}

	protected override void ShowState()
	{
		base.ShowState();
		ShowTrialsInfoDialogs();
		ChartBoostWrapper.Instance.ShowAd("Level Select Screen");
	}

	private void ShowTrialsInfoDialogs()
	{
		if (!SecureStorage.Instance.HasVisitedTrials)
		{
			StartCoroutine(SpawnTrialsIntroDialog());
			SecureStorage.Instance.HasVisitedTrials = true;
		}
		else if (!SecureStorage.Instance.HasRefilledTrialsBoost && TrialsDataManager.Instance.NumBoostsAvailable < TrialsDataManager.Instance.MaxBoostsAvailable)
		{
			StartCoroutine(SpawnTrialsBoostDialog());
		}
	}

	public IEnumerator SpawnTrialsIntroDialog()
	{
		MessageBox messageBox = (MessageBox)Object.Instantiate(m_messageBoxPrefab);
		string Title = Language.Get("S_TRIALS_TITLE");
		string Body = Language.Get("S_TRIALS_BURST_INTRO");
		yield return StartCoroutine(messageBox.Display(Title, Body));
	}

	public IEnumerator SpawnTrialsBoostDialog()
	{
		MessageBox messageBox = (MessageBox)Object.Instantiate(m_messageBoxPrefab);
		string Title = Language.Get("S_TRIALS_BURST_TONIC");
		string Body = Language.Get("S_TRIALS_BURST_REFILL");
		yield return StartCoroutine(messageBox.Display(Title, Body));
	}

	private void OnChallengePressed(IUIObject obj)
	{
		if (DialogManager.DialogActive)
		{
			return;
		}
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)obj;
		if (autoSpriteControlBase != null)
		{
			GameObject gameObject = autoSpriteControlBase.gameObject.transform.parent.parent.gameObject;
			ChallengeButton component = gameObject.GetComponent<ChallengeButton>();
			if (component != null)
			{
				StartCoroutine(LaunchTrial(component.m_Level));
			}
			else
			{
				Debug.LogError("Could not find challenge button script");
			}
		}
	}

	public void OnBuyStuff()
	{
		BaseCampController.Instance.LaunchWithExchangeFocus();
	}

	public void OnRefillTonics()
	{
		StartCoroutine(RefillDialog());
		SecureStorage.Instance.HasRefilledTrialsBoost = true;
	}

	protected IEnumerator RefillDialog()
	{
		RefillBoostTonicDialog dialog = (RefillBoostTonicDialog)Object.Instantiate(m_RefillDialogPrefab);
		yield return StartCoroutine(dialog.Display(OnPurchaseConfirmed, OnPurchaseCancelled));
		yield return null;
	}

	protected IEnumerator LaunchTrial(string strTrial)
	{
		TrialPlayInfoDialog dialog = (TrialPlayInfoDialog)Object.Instantiate(m_PlayInfoDialogPrefab);
		yield return StartCoroutine(dialog.Display(strTrial));
		yield return null;
	}

	public void LateUpdate()
	{
		m_gemsText.Text = string.Format(" {0}", SecureStorage.Instance.GetGems());
		m_coinsText.Text = string.Format(" {0}", SecureStorage.Instance.GetCoins());
	}

	protected virtual void OnPurchaseConfirmed()
	{
		Debug.Log("Confirmed Refill");
		TrialsDataManager.Instance.ResetBoosts();
	}

	protected virtual void OnPurchaseCancelled()
	{
		Debug.Log("Cancelled Refill");
	}

	protected override void OnStateActivate(string OldStateName)
	{
		DialogManager.ResetCounter();
		base.OnStateActivate(OldStateName);
		MusicManager.Instance.PlayTitleMusic();
	}
}
