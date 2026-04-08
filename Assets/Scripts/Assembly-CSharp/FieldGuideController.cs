using UnityEngine;

public class FieldGuideController : StateController
{
	public FieldGuideStatisticsPanel m_StatsPanel;

	public FieldGuidePatchesPanel m_PatchPanel;

	public SpriteText m_gemsText;

	public SpriteText m_coinsText;

	public GameObject m_GCAchievementButton;

	private bool m_inputLockout;

	private static FieldGuideController m_instance;

	public static FieldGuideController Instance
	{
		get
		{
			return m_instance;
		}
	}

	public void RePopulate()
	{
		m_StatsPanel.RePopulate();
		m_PatchPanel.RePopulate();
		m_gemsText.Text = string.Format(" {0}", SecureStorage.Instance.GetGems());
		m_coinsText.Text = string.Format(" {0}", SecureStorage.Instance.GetCoins());
	}

	public override void Awake()
	{
		base.Awake();
		if (m_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		if (m_GCAchievementButton != null)
		{
			Object.Destroy(m_GCAchievementButton);
			m_GCAchievementButton = null;
		}
	}

	protected override void OnStateLoaded()
	{
	}

	public void OnBuyStuff()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			BaseCampController.Instance.LaunchWithExchangeFocus();
		}
	}

	private void OnPanelChanged(UIPanelBase newPanel)
	{
		MenuSFX.Instance.Play2D("TabChange");
		switch (newPanel.name)
		{
		case "0 - Panel Patches":
			SwrveEventsUI.ViewedFieldPanel("Patches");
			break;
		case "1 - Panel Statistics":
			SwrveEventsUI.ViewedFieldPanel("Stats");
			break;
		}
	}

	public override void OnBackPressed()
	{
		if (!m_inputLockout)
		{
			m_inputLockout = true;
			base.OnBackPressed();
		}
	}

	protected override void OnStateActivate(string OldStateName)
	{
		base.OnStateActivate(OldStateName);
		UIPanelManager.instance.AddPanelChangedDelegate(OnPanelChanged);
		if (UIPanelManager.instance.CurrentPanel != null)
		{
			OnPanelChanged(UIPanelManager.instance.CurrentPanel);
		}
		m_inputLockout = false;
	}

	protected override void OnStateDeactivate(string NewStateName)
	{
		Debug.Log("BaseCampController: Deactivate");
		base.OnStateDeactivate(NewStateName);
		UIPanelManager.instance.RemovePanelChangedDelegate(OnPanelChanged);
	}

	public void OnLeaderboardsPressed()
	{
		MobileNetworkManager.Instance.showLeaderboards();
		SwrveEventsUI.LeaderboardsButtonTouched();
	}

	public void OnAchievementsPressed()
	{
		MobileNetworkManager.Instance.showAchievements();
		SwrveEventsUI.AchievementsButtonTouched();
	}
}
