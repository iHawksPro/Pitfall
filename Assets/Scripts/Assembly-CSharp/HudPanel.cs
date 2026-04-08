using System.Collections;
using UnityEngine;

public class HudPanel : MonoBehaviour
{
	public SpriteText m_scoreText;

	public SpriteText m_treasureText;

	public SpriteText m_distanceText;

	public SpriteText m_diamondsText;

	public AutoSpriteBase m_SpiritButtonBack;

	public UIButton m_SpiritButtonFront;

	public AutoSpriteBase m_SpeedIncreaseButtonBack;

	public UIButton m_SpeedIncreaseButtonFront;

	public AutoSpriteBase m_PauseButtonBack;

	public UIButton m_PauseButtonFront;

	public HudButton m_VenomButton;

	public HudButton m_SpeedTonicButton;

	public GameObject m_StandardTreasureIcon;

	public GameObject m_X2;

	public RotatingElement m_GemEffect;

	public GameObject m_checkpointMap;

	public GameObject m_trialsBoost;

	private float m_PauseIconAlpha = -1f;

	private float m_SpeedBoostPowerUpIconAlpha = -1f;

	private float m_SpiritOfTheJungleIconAlpha = -1f;

	private int m_DistanceTotalCache = -1;

	private int m_TreasureTotalCache = -1;

	public GameObject DD_Treasure_X2;

	public GameObject DD_Treasure_X3;

	public GameObject DD_Treasure_X4;

	public GameObject DD_XP_X2;

	public GameObject DD_XP_X3;

	public GameObject DD_XP_X4;

	public SpriteText XP_Icon_Text;

	public GameObject XP_Icon;

	public GameObject XPScaler;

	public GameObject XPBackingImage;

	public void Awake()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (gameController != null)
		{
			gameController.GameHUD = this;
		}
		Hide();
	}

	private void OnApplicationPause(bool Paused)
	{
		if (!Paused && StateManager.Instance.CurrentStateName == "Game")
		{
			OnHudPause();
		}
	}

	public void OnHudPause()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (!PlayerController.Instance().IsDead() && !((gameController != null) & gameController.IsPaused))
		{
			if (gameController != null)
			{
				gameController.PauseGame();
			}
			MenuSFX.Instance.Play2D("MenuCancel");
			base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
			StartCoroutine(DoPause());
		}
	}

	public void Hide()
	{
		base.transform.localPosition = new Vector3(0f, 10000f, 0f);
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(false);
		}
	}

	public void Show()
	{
		base.transform.localPosition = Vector3.zero;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(true);
		}
		if (SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) > 0)
		{
			m_X2.SetActiveRecursively(true);
		}
		else
		{
			m_X2.SetActiveRecursively(false);
		}
		m_StandardTreasureIcon.active = true;
		DD_Treasure_X2.SetActiveRecursively(false);
		DD_Treasure_X3.SetActiveRecursively(false);
		DD_Treasure_X4.SetActiveRecursively(false);
		DD_XP_X2.SetActiveRecursively(false);
		DD_XP_X3.SetActiveRecursively(false);
		DD_XP_X4.SetActiveRecursively(false);
		XP_Icon_Text.gameObject.SetActiveRecursively(false);
		XPBackingImage.SetActiveRecursively(false);
		XPScaler.transform.localScale = new Vector3(0f, 1f, 1f);
		StartCoroutine(DDIconUpdate());
		if (GameController.Instance.IsPlayingTrialsMode)
		{
			m_trialsBoost.SetActiveRecursively(true);
			m_checkpointMap.SetActiveRecursively(false);
		}
		else
		{
			m_trialsBoost.SetActiveRecursively(false);
			m_checkpointMap.SetActiveRecursively(true);
		}
		m_VenomButton.gameObject.SetActiveRecursively(!GameController.Instance.IsPlayingTrialsMode);
		m_SpeedTonicButton.gameObject.SetActiveRecursively(!GameController.Instance.IsPlayingTrialsMode);
		SetSpiritOfTheJungleIconAlpha(0f);
		SetSpeedBoostPowerUpIconAlpha(0f);
	}

	private IEnumerator DDIconUpdate()
	{
		yield return new WaitForSeconds(1.5f);
		DailyDoubleController.Instance.SetDDTimeWindow();
		GameObject CurrIcon = m_StandardTreasureIcon;
		if (DailyDoubleController.Instance.TreasureMultiplier == DailyDoubleController.TreasureMultipliers.NONE && DailyDoubleController.Instance.XPMultiplier == DailyDoubleController.XPMultipliers.NONE)
		{
			yield break;
		}
		if (DailyDoubleController.Instance.TreasureMultiplier != DailyDoubleController.TreasureMultipliers.NONE)
		{
			m_StandardTreasureIcon.active = false;
			m_X2.SetActiveRecursively(false);
			switch (DailyDoubleController.Instance.TreasureMultiplier)
			{
			case DailyDoubleController.TreasureMultipliers.TREASURE_X2:
				DD_Treasure_X2.SetActiveRecursively(true);
				CurrIcon = DD_Treasure_X2;
				break;
			case DailyDoubleController.TreasureMultipliers.TREASURE_X3:
				DD_Treasure_X3.SetActiveRecursively(true);
				CurrIcon = DD_Treasure_X3;
				break;
			case DailyDoubleController.TreasureMultipliers.TREASURE_X4:
				DD_Treasure_X4.SetActiveRecursively(true);
				CurrIcon = DD_Treasure_X4;
				break;
			}
		}
		if (DailyDoubleController.Instance.XPMultiplier != DailyDoubleController.XPMultipliers.NONE)
		{
			XP_Icon_Text.gameObject.SetActiveRecursively(true);
			if (!XP_Icon_Text.Text.Contains("  "))
			{
				XP_Icon_Text.Text = "  " + XP_Icon_Text.Text;
			}
			XPBackingImage.SetActiveRecursively(true);
			XPScaler.ScaleTo(Vector3.one, 0.25f, 0f);
			switch (DailyDoubleController.Instance.XPMultiplier)
			{
			case DailyDoubleController.XPMultipliers.XP_X2:
				DD_XP_X2.SetActiveRecursively(true);
				break;
			case DailyDoubleController.XPMultipliers.XP_X3:
				DD_XP_X3.SetActiveRecursively(true);
				break;
			case DailyDoubleController.XPMultipliers.XP_X4:
				DD_XP_X4.SetActiveRecursively(true);
				break;
			}
		}
		MenuSFX.Instance.Play2D("DailyDoubleMarker");
		Vector3 CurrScale = CurrIcon.transform.localScale;
		float ScaleFactor = 1.3f;
		float UpTime = 0.07f;
		float NumLoops = 4f;
		for (float LoopCounter = 0f; LoopCounter <= NumLoops; LoopCounter += 1f)
		{
			if (DailyDoubleController.Instance.TreasureMultiplier != DailyDoubleController.TreasureMultipliers.NONE)
			{
				StartCoroutine(ScaleIcon(CurrIcon, CurrScale, ScaleFactor, UpTime));
			}
			if (DailyDoubleController.Instance.XPMultiplier != DailyDoubleController.XPMultipliers.NONE)
			{
				StartCoroutine(ScaleIcon(XP_Icon, Vector3.one, ScaleFactor, UpTime));
			}
			yield return new WaitForSeconds(UpTime * 2f);
		}
	}

	private IEnumerator ScaleIcon(GameObject CurrIcon, Vector3 CurrScale, float ScaleFactor, float UpTime)
	{
		CurrIcon.ScaleTo(CurrScale * ScaleFactor, UpTime, 0f);
		yield return new WaitForSeconds(UpTime);
		CurrIcon.ScaleTo(CurrScale, UpTime, 0f);
	}

	public IEnumerator DoPause()
	{
		float pauseEndTime = Time.realtimeSinceStartup + 0.3f;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		Hide();
		StateManager.Instance.LoadAndActivateState("Pause");
	}

	public void Update()
	{
		if ((bool)PlayerController.Instance() && PlayerController.Instance().IsDead())
		{
			SetPauseIconAlpha(0.5f);
		}
		else
		{
			SetPauseIconAlpha(1f);
		}
	}

	public void OnGameOver()
	{
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		m_DistanceTotalCache = -1;
		m_TreasureTotalCache = -1;
	}

	public void SetPauseIconAlpha(float alpha)
	{
		if (m_PauseIconAlpha != alpha)
		{
			m_PauseIconAlpha = alpha;
			m_PauseButtonBack.SetColor(new Color(m_PauseButtonBack.color.r, m_PauseButtonBack.color.g, m_PauseButtonBack.color.b, alpha));
			m_PauseButtonFront.SetColor(new Color(m_PauseButtonFront.color.r, m_PauseButtonFront.color.g, m_PauseButtonFront.color.b, alpha));
		}
	}

	public void SetSpiritOfTheJungleIconAlpha(float alpha)
	{
		if (m_SpiritOfTheJungleIconAlpha != alpha)
		{
			m_SpiritButtonBack.gameObject.SetActiveRecursively(alpha > 0f);
			m_SpiritButtonFront.gameObject.SetActiveRecursively(alpha > 0f);
			m_SpiritOfTheJungleIconAlpha = alpha;
			m_SpiritButtonBack.SetColor(new Color(m_SpiritButtonBack.color.r, m_SpiritButtonBack.color.g, m_SpiritButtonBack.color.b, alpha));
			m_SpiritButtonFront.SetColor(new Color(m_SpiritButtonFront.color.r, m_SpiritButtonFront.color.g, m_SpiritButtonFront.color.b, alpha));
		}
	}

	public void SetSpeedBoostPowerUpIconAlpha(float alpha)
	{
		if (m_SpeedBoostPowerUpIconAlpha != alpha)
		{
			m_SpeedIncreaseButtonBack.gameObject.SetActiveRecursively(alpha > 0f);
			m_SpeedIncreaseButtonFront.gameObject.SetActiveRecursively(alpha > 0f);
			m_SpeedBoostPowerUpIconAlpha = alpha;
			m_SpeedIncreaseButtonBack.SetColor(new Color(m_SpeedIncreaseButtonBack.color.r, m_SpeedIncreaseButtonBack.color.g, m_SpeedIncreaseButtonBack.color.b, alpha));
			m_SpeedIncreaseButtonFront.SetColor(new Color(m_SpeedIncreaseButtonFront.color.r, m_SpeedIncreaseButtonFront.color.g, m_SpeedIncreaseButtonFront.color.b, alpha));
		}
	}

	public void SetVenomIconVisible(bool isVisible)
	{
		m_VenomButton.SetVisible(isVisible);
	}

	public void SetHasteTonicVisible(bool isVisible)
	{
		m_SpeedTonicButton.SetVisible(isVisible);
	}

	public void SetScore(int Score)
	{
	}

	public void SetTreasure(int Treasure)
	{
		if (m_TreasureTotalCache != Treasure)
		{
			m_treasureText.Text = string.Format(" {0}", Treasure);
			m_TreasureTotalCache = Treasure;
		}
	}

	public void SetDiamonds(int Diamonds)
	{
		m_diamondsText.Text = string.Format(" {0}", Diamonds);
	}

	public void SetDistance(int Distance)
	{
		if (m_DistanceTotalCache != Distance)
		{
			m_distanceText.Text = string.Format(" {0}m", Distance);
			m_DistanceTotalCache = Distance;
		}
	}

	public void OnSpiritOfTheJungle()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (gameController != null && gameController.SpiritOfTheJungleCanBeUsed() && gameController.UseSpiritOfTheJungle())
		{
			SwrveEventsGameplay.LifeTonicUsed();
			PlayerController.Instance().Score().LifeTonicUsed();
		}
	}

	public void OnSpeedIncrease()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (gameController != null && gameController.SpeedBoostPowerUpCanBeUsed() && gameController.UseSpeedIncrease())
		{
			SwrveEventsGameplay.HasteTonicUsed();
			PlayerController.Instance().Score().HasteTonicUsed();
		}
	}

	public void OnAntiVenomUsed()
	{
		GameController gameController = Object.FindObjectOfType(typeof(GameController)) as GameController;
		if (gameController != null && gameController.VenomCanBeUsed() && gameController.UseAntiVenom())
		{
			SwrveEventsGameplay.AntiVenomUsed();
			PlayerController.Instance().Score().AntiVenomUsed();
		}
	}

	public void OnCoinCollected()
	{
		m_GemEffect.StartRotating(0.5f);
	}
}
