using System;
using System.Collections;
using UnityEngine;

public class BaseCampController : StateController
{
	public UIButton m_gemsButton;

	public SpriteText m_gemsText;

	public SpriteText m_coinsText;

	public ConfirmDialog m_confirmDialogPrefab;

	public MessageBox m_messageBoxPrefab;

	public GameObject m_DownArrow;

	public GameObject m_UpArrow;

	public ConfirmDialog m_NoButtonDialogPrefab;

	public BaseCampProducts.ProductData m_FocusProduct;

	private bool m_GotoFocusProduct;

	public bool m_GotoExchange;

	public UIPanelManager m_StorePanelManager;

	private bool? m_dialogYesOrNo;

	private bool m_DialogInUse;

	private bool m_CheckpointTutorialActive;

	private ConfirmDialog m_TutorialConfirmDialog;

	public UIButton BackButton;

	private bool m_welcomeDialogActive;

	private float mLastWifiMessageTime;

	public GameObject ExchangeLink;

	private static BaseCampController m_instance;

	public static BaseCampController Instance
	{
		get
		{
			return m_instance;
		}
	}

	protected override void OnStateLoaded()
	{
		m_TutorialConfirmDialog = null;
	}

	public override void Awake()
	{
		base.Awake();
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			m_instance = this;
		}
		m_GotoFocusProduct = false;
		m_GotoExchange = false;
		m_DialogInUse = false;
		mLastWifiMessageTime = 0f;
		m_CheckpointTutorialActive = false;
		m_welcomeDialogActive = false;
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void OnEnable()
	{
		SecureStorage.playerCoinsChanged += UpdateCurrencies;
		SecureStorage.playerGemsChanged += UpdateCurrencies;
	}

	private void OnDisable()
	{
		SecureStorage.playerCoinsChanged -= UpdateCurrencies;
		SecureStorage.playerGemsChanged -= UpdateCurrencies;
	}

	public void SetTutorialActive()
	{
		m_CheckpointTutorialActive = true;
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.CheckpointActivated", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public void EndTutorial()
	{
		if (m_CheckpointTutorialActive)
		{
			Bedrock.AnalyticsLogEvent("Progression.Tutorial.CheckpointCompleted", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
		}
		m_CheckpointTutorialActive = false;
	}

	public bool TutorialActive()
	{
		return m_CheckpointTutorialActive;
	}

	public void RegisterItemPressedForTutorial()
	{
		if (m_TutorialConfirmDialog != null)
		{
			m_TutorialConfirmDialog.Dismiss();
		}
	}

	public void Refresh()
	{
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Refresh data for Store");
		base.gameObject.BroadcastMessage("PopulateProductList", SendMessageOptions.DontRequireReceiver);
	}

	private bool LaunchFirstGemsGiven()
	{
		if (!SecureStorage.Instance.VisitedStore)
		{
			SecureStorage.Instance.VisitedStore = true;
			if (SwrveServerVariables.Instance.GemsGivenFirstTime > 0)
			{
				SecureStorage.Instance.ChangeGems(SwrveServerVariables.Instance.GemsGivenFirstTime);
				StartCoroutine(SpawnWelcomeDialog());
				return true;
			}
		}
		return false;
	}

	protected override void ShowState()
	{
		base.ShowState();
		UpdateCurrencies(0);
		if (LoadingScreen.Instance.IsVisible)
		{
			LoadingScreen.Instance.Hide();
		}
		if (m_CheckpointTutorialActive)
		{
			m_GotoFocusProduct = true;
			m_FocusProduct = StoreProductManager.Instance.FindProduct("checkpoint.2000");
		}
		else if (LaunchFirstGemsGiven())
		{
			m_GotoFocusProduct = false;
		}
		base.gameObject.BroadcastMessage("BringOnScreen", SendMessageOptions.DontRequireReceiver);
		if (m_GotoFocusProduct)
		{
			GotoFocusProduct();
			m_GotoFocusProduct = false;
		}
		if (m_GotoExchange)
		{
			GemStoreController.Instance.PanelVisible();
			m_StorePanelManager.BringInImmediate(4);
			m_GotoExchange = false;
		}
		if (m_CheckpointTutorialActive)
		{
			BackButton.gameObject.SetActiveRecursively(false);
			GameObject gameObject = GameObject.Find("CheckpointIcon2000");
			if (gameObject != null)
			{
				CheckpointItem component = gameObject.GetComponent<CheckpointItem>();
				if (component != null)
				{
					gameObject = component.GetPurchaseableCheckpoint();
				}
			}
			StartCoroutine(SpawnTutorialDialog(m_NoButtonDialogPrefab, "S_CHECK_TUT_2_TITLE", "S_CHECK_TUT_2_BODY", gameObject));
		}
		ChartBoostWrapper.Instance.ShowAd("IAP Store");
	}

	private void GotoFocusProduct()
	{
		Debug.Log("GotoFocusProduct");
		if (m_FocusProduct.Category == BaseCampProducts.StoreCategory.BCP_CHECKPOINT)
		{
			m_StorePanelManager.BringInImmediate(3);
		}
		else if (m_FocusProduct.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT)
		{
			m_StorePanelManager.BringInImmediate(2);
		}
		else if (m_FocusProduct.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
		{
			m_StorePanelManager.BringInImmediate(1);
		}
		else
		{
			m_StorePanelManager.BringInImmediate(0);
		}
		UIPanelBase currentPanel = m_StorePanelManager.CurrentPanel;
		StorePanel component = currentPanel.GetComponent<StorePanel>();
		if (component != null)
		{
			component.ScrollToItem(m_FocusProduct);
		}
	}

	private void OnPanelChanged(UIPanelBase newPanel)
	{
		MenuSFX.Instance.Play2D("TabChange");
		switch (newPanel.name)
		{
		case "0 - Power Ups":
			SwrveEventsUI.ViewedStorePanel("Powerups");
			GemStoreController.Instance.PanelNotVisible();
			break;
		case "1 - Upgrades":
			SwrveEventsUI.ViewedStorePanel("Upgrades");
			GemStoreController.Instance.PanelNotVisible();
			break;
		case "2 - Panel Outfits":
			SwrveEventsUI.ViewedStorePanel("Outfits");
			GemStoreController.Instance.PanelNotVisible();
			break;
		case "3 - Panel CheckPoints":
			SwrveEventsUI.ViewedStorePanel("Checkpoints");
			GemStoreController.Instance.PanelNotVisible();
			break;
		case "4 - Panel Exchange":
			SwrveEventsUI.ViewedStorePanel("Exchange");
			GemStoreController.Instance.PanelVisible();
			break;
		}
	}

	public void ForceActivate(string OldStateName)
	{
		OnStateActivate(OldStateName);
	}

	protected override void OnStateActivate(string OldStateName)
	{
		TBFUtils.DebugLog("BaseCampController: Activate");
		base.OnStateActivate(OldStateName);
		m_StorePanelManager.AddPanelChangedDelegate(OnPanelChanged);
		if (m_StorePanelManager.CurrentPanel != null && !m_GotoFocusProduct && !m_GotoExchange)
		{
			OnPanelChanged(m_StorePanelManager.CurrentPanel);
		}
		SwrveEventsUI.ViewedStore();
		MusicManager.Instance.PlayTitleMusic();
	}

	protected override void OnStateDeactivate(string NewStateName)
	{
		TBFUtils.DebugLog("BaseCampController: Deactivate");
		base.OnStateDeactivate(NewStateName);
		m_StorePanelManager.RemovePanelChangedDelegate(OnPanelChanged);
		SwrveUserData.UploadAllAttributes();
	}

	protected override void HideState()
	{
		base.HideState();
	}

	private void UpdateCurrencies(int Dummy)
	{
		m_gemsText.Text = string.Format(" {0}", SecureStorage.Instance.GetGems());
		m_coinsText.Text = string.Format(" {0}", SecureStorage.Instance.GetCoins());
	}

	public void Update()
	{
		UIPanelBase currentPanel = m_StorePanelManager.CurrentPanel;
		if (!(currentPanel != null))
		{
			return;
		}
		StorePanel component = currentPanel.GetComponent<StorePanel>();
		bool flag = currentPanel.index != 4;
		if (!(component != null))
		{
			return;
		}
		float num = 0f;
		if (currentPanel.index == 3)
		{
			num = 300f;
		}
		if (component.GetScrollPosition() < 0.95f && flag)
		{
			if (!m_DownArrow.active)
			{
				m_DownArrow.SetActiveRecursively(true);
			}
		}
		else if (m_DownArrow.active)
		{
			m_DownArrow.SetActiveRecursively(false);
		}
		if (component.GetScrollPosition() > 0.05f && flag)
		{
			if (!m_UpArrow.active)
			{
				m_UpArrow.SetActiveRecursively(true);
			}
		}
		else if (m_UpArrow.active)
		{
			m_UpArrow.SetActiveRecursively(false);
		}
	}

	public void BuyItem(BaseCampProducts.ProductData Item, int ItemLevel, bool UseGems, Action OnPurchaseConfirmed, Action OnPurchaseCancelled)
	{
		StartCoroutine(PurchaseItem(Item, ItemLevel, UseGems, OnPurchaseConfirmed, OnPurchaseCancelled));
	}

	private IEnumerator PurchaseItem(BaseCampProducts.ProductData Item, int ItemLevel, bool UseGems, Action OnPurchaseConfirmed, Action OnPurchaseCancelled)
	{
		bool Success = false;
		int nOwned = SecureStorage.Instance.GetItemCount(Item.Identifier);
		int MaxCanBeOwned = Item.MaxNumberPlayerCanBuy();
		bool LoopOnNotEnoughCoins = false;
		do
		{
			LoopOnNotEnoughCoins = false;
			int CurrencyCost;
			int PlayerCurrency;
			if (UseGems)
			{
				CurrencyCost = Item.GemsPrice(ItemLevel);
				PlayerCurrency = SecureStorage.Instance.GetGems();
			}
			else
			{
				CurrencyCost = Item.CoinPrice(ItemLevel);
				PlayerCurrency = SecureStorage.Instance.GetCoins();
			}
			bool PlayerCanBuy = nOwned < MaxCanBeOwned;
			bool PlayerCanAfford = CurrencyCost <= PlayerCurrency;
			if (Item.Category == BaseCampProducts.StoreCategory.BCP_TRIALS)
			{
				PlayerCanBuy = true;
			}
			if (PlayerCanBuy)
			{
				if (PlayerCanAfford)
				{
					m_dialogYesOrNo = true;
					if (m_dialogYesOrNo == true)
					{
						SecureStorage.Instance.ChangeItemCount(Item.Identifier, Item.Quantity(ItemLevel));
						if (UseGems)
						{
							SecureStorage.Instance.ChangeGems(-CurrencyCost);
							SwrveEventsPurchase.StoreGemPurchase(Item.Identifier, (ulong)CurrencyCost);
						}
						else
						{
							SecureStorage.Instance.ChangeCoins(-CurrencyCost);
							SwrveEventsPurchase.StoreCoinPurchase(Item.Identifier, (ulong)CurrencyCost);
						}
						if (Item.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT)
						{
							SecureStorage.Instance.SetCurrentCostume(Item.Identifier);
							SecureStorage.Instance.UpdateOutfitsBought(1);
							switch (Item.Identifier)
							{
							case "outfit.fairy":
								SecureStorage.Instance.UpdateOutfitFairy(1);
								break;
							case "outfit.bear":
								SecureStorage.Instance.UpdateOutfitBear(1);
								break;
							case "outfit.aviator":
								SecureStorage.Instance.UpdateOutfitAviator(1);
								break;
							}
						}
						OnPurchaseConfirmed();
						Success = true;
					}
					else
					{
						Success = false;
					}
					continue;
				}
				if (!m_DialogInUse)
				{
					if (UseGems)
					{
						yield return StartCoroutine(DisplayNotEnoughGems());
						if (m_dialogYesOrNo == true)
						{
							if (StateManager.Instance.CurrentStateName == "Challenges" || StateManager.Instance.CurrentStateName == "Results")
							{
								StateManager.Instance.LoadAndActivateState("BaseCamp");
								while (StateManager.Instance.CurrentStateName != "BaseCamp")
								{
									yield return null;
								}
								while (!m_isActive)
								{
									yield return null;
								}
								while (!m_StorePanelManager.gameObject.active)
								{
									yield return null;
								}
								yield return new WaitForSeconds(0.5f);
							}
							LoopOnNotEnoughCoins = false;
							m_StorePanelManager.BringInImmediate(4);
							OnPurchaseCancelled();
						}
					}
					else
					{
						yield return StartCoroutine(DisplayNotEnoughCoins());
						if (m_dialogYesOrNo == true)
						{
							LoopOnNotEnoughCoins = true;
							UseGems = true;
						}
					}
				}
				Success = false;
			}
			else
			{
				yield return StartCoroutine(DisplayPurchaseSuccess(Item.Title));
				Success = false;
			}
		}
		while (LoopOnNotEnoughCoins);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator DisplayYesNoDialog(string TitleKey, string BodyKey)
	{
		m_dialogYesOrNo = null;
		m_DialogInUse = true;
		string Title = Language.Get(TitleKey);
		string Body = Language.Get(BodyKey);
		string Opt1 = Language.Get("S_YES");
		string Opt2 = Language.Get("S_NO");
		ConfirmDialog confirmDialog = (ConfirmDialog)UnityEngine.Object.Instantiate(m_confirmDialogPrefab);
		yield return StartCoroutine(confirmDialog.Display(Title, Body, Opt1, Opt2, OnDialogYes, OnDialogNo, OnDialogNo));
		m_DialogInUse = false;
	}

	private void OnDialogYes()
	{
		m_dialogYesOrNo = true;
	}

	private void OnDialogNo()
	{
		m_dialogYesOrNo = false;
	}

	private IEnumerator DisplayNotEnoughGems()
	{
		yield return StartCoroutine(DisplayYesNoDialog("S_NOT_ENOUGH_DIAMONDS_T", "S_NOT_ENOUGH_DIAMONDS_D"));
	}

	private IEnumerator DisplayNotEnoughCoins()
	{
		yield return StartCoroutine(DisplayYesNoDialog("S_NOT_ENOUGH_TREASURE_T", "S_NOT_ENOUGH_TREASURE_D"));
	}

	private IEnumerator DisplayPurchaseSuccess(string ItemName)
	{
		MessageBox messageBox = (MessageBox)UnityEngine.Object.Instantiate(m_messageBoxPrefab);
		string Title = Language.Get("S_PURCHASE_SUCCESS_T");
		string Body = string.Format(Language.Get("S_PURCHASE_SUCCESS_D"), ItemName);
		yield return StartCoroutine(messageBox.Display(Title, Body));
	}

	private IEnumerator ConfirmPurchase(string ItemName, int Value, bool UseGems)
	{
		m_dialogYesOrNo = null;
		string Title = Language.Get("S_CONFIRM_PURCHASE_T");
		string BodyText = string.Format(Language.Get("S_CONFIRM_PURCHASE_D"), ItemName, Value, (!UseGems) ? Language.Get("S_TREASURE") : Language.Get("S_DIAMONDS"));
		string Opt1 = Language.Get("S_YES");
		string Opt2 = Language.Get("S_NO");
		ConfirmDialog confirmDialog = (ConfirmDialog)UnityEngine.Object.Instantiate(m_confirmDialogPrefab);
		yield return StartCoroutine(confirmDialog.Display(Title, BodyText, Opt1, Opt2, OnDialogYes, OnDialogNo, OnDialogNo));
	}

	public void LaunchWithProductFocus(BaseCampProducts.ProductData FocusProduct)
	{
		m_FocusProduct = FocusProduct;
		m_GotoFocusProduct = true;
		StateRoot state = StateManager.Instance.GetState("Results");
		if ((bool)state)
		{
			state.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		}
		StartCoroutine(Store(0.4f));
	}

	public void LaunchWithProductDiscount(BaseCampProducts.ProductData FocusProduct)
	{
		BaseCampProductItem.m_FocusProductDiscount = FocusProduct;
		LaunchWithProductFocusImmediate(FocusProduct);
	}

	public void LaunchWithProductFocusImmediate(BaseCampProducts.ProductData FocusProduct)
	{
		m_FocusProduct = FocusProduct;
		m_GotoFocusProduct = true;
		StateRoot state = StateManager.Instance.GetState("Results");
		if ((bool)state)
		{
			state.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		}
		StartCoroutine(Store(0f));
	}

	public void ShowWifiErrorMessageAndSwitchPanel()
	{
		Debug.LogError("ShowWifiErrorMessageAndSwitchPanel");
		if ((bool)StateManager.Instance.GetState("BaseCamp"))
		{
			m_StorePanelManager.BringInImmediate(0);
		}
		if (Time.realtimeSinceStartup - mLastWifiMessageTime > 1f)
		{
			mLastWifiMessageTime = Time.realtimeSinceStartup;
			InvokeHelper.InvokeSafe(Instance.DelayedWifiErrorMessage, 0.25f, this);
		}
	}

	public void LaunchWithExchangeFocus()
	{
		m_GotoExchange = true;
		StateRoot state = StateManager.Instance.GetState("FieldGuide");
		if ((bool)state)
		{
			state.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		}
		StartCoroutine(Store(0.4f));
	}

	private IEnumerator Store(float Delay)
	{
		if (Delay > 0f)
		{
			yield return new WaitForSeconds(Delay);
		}
		StateManager.Instance.LoadAndActivateState("BaseCamp");
	}

	public void LaunchFreeDiamonds()
	{
		m_GotoExchange = true;
		StartCoroutine(Store(0f));
	}

	public IEnumerator SpawnTutorialDialog(ConfirmDialog dialogPrefab, string TitleKey, string BodyKey, GameObject tutorialButton)
	{
		m_TutorialConfirmDialog = (ConfirmDialog)UnityEngine.Object.Instantiate(dialogPrefab);
		string Title = Language.Get(TitleKey);
		string Body = Language.Get(BodyKey);
		yield return StartCoroutine(m_TutorialConfirmDialog.Display(Title, Body, string.Empty, string.Empty, null, null, null, TitleKey, tutorialButton));
	}

	public IEnumerator SpawnWelcomeDialog()
	{
		m_welcomeDialogActive = true;
		MessageBox messageBox = (MessageBox)UnityEngine.Object.Instantiate(m_messageBoxPrefab);
		string Title = Language.Get("S_DIAMOND_WELCOME_TITLE");
		string Body = Language.Get("S_DIAMOND_WELCOME_BODY");
		yield return StartCoroutine(messageBox.Display(Title, Body));
		m_welcomeDialogActive = false;
	}

	public void DelayedWifiErrorMessage()
	{
		ShowAlertDialog("S_IAP_NO_NET_TITLE", "S_IAP_NO_NET_BODY", "S_IAP_OK");
	}

	public void DelayedRestrictionMessage()
	{
		ShowAlertDialog("S_IAP_NO_AVAIL_TITLE", "S_IAP_NO_AVAIL_BODY_DROID", "S_IAP_OK");
	}

	private void ShowAlertDialog(string TitleKey, string BodyKey, string ButtonKey)
	{
		string strTitle = Language.Get(TitleKey);
		string strBody = Language.Get(BodyKey);
		string strOK = Language.Get(ButtonKey);
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}
}
