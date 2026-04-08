using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemStoreController : StorePanel
{
	private enum SpenderType
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	private const float EUROTODOLLARS = 1.3f;

	public SpriteText m_gemsCount;

	public GameObject m_productItemPrefab;

	public GameObject m_productItemVerticalPrefab;

	public BigMessage m_thanksDialogPrefab;

	public ConfirmDialog m_confirmDialogPrefab;

	[HideInInspector]
	public bool m_waitingForProducts;

	private GemStoreProduct.GemStoreProductData m_productToPurchase;

	private bool m_panelVisible;

	private bool m_bPopulated;

	private static GemStoreController m_instance;

	private bool m_inbasecamp;

	private bool m_productFetchSuccess;

	private bool m_waitingForPurchase;

	public static GemStoreController Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			m_instance = this;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public void ForcePopulate()
	{
		StartCoroutine(PopulateProductList(true));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		m_panelVisible = false;
	}

	public void PanelVisible()
	{
		m_panelVisible = true;
		MonoBehaviour.print("GEMSTORECONTROLLER PanelVisible: " + m_panelVisible + " " + Time.time);
		if (BaseCampController.Instance.m_StorePanelManager.CurrentPanel != null && BaseCampController.Instance.m_StorePanelManager.CurrentPanel.name == "4 - Panel Exchange")
		{
			StartCoroutine(DirectToExchange());
		}
		StartCoroutine(PopulateProductList(true));
	}

	public void PanelNotVisible()
	{
		m_panelVisible = false;
		DailyDoubleController.Instance.SetBottomTicker(false);
	}

	private IEnumerator DirectToExchange()
	{
		float Timer = Time.time;
		float Length = 0.3f;
		while (Time.time - Timer < Length)
		{
			if (BaseCampController.Instance.m_UpArrow.active)
			{
				BaseCampController.Instance.m_UpArrow.SetActiveRecursively(false);
				BaseCampController.Instance.m_DownArrow.SetActiveRecursively(false);
			}
			yield return null;
		}
	}

	public IEnumerator PopulateProductList(bool FromBaseCamp)
	{
		MonoBehaviour.print("PopulateProductList, FromBaseCamp: " + FromBaseCamp + " " + Time.time);
		if (FromBaseCamp)
		{
			BaseCampController.Instance.m_UpArrow.SetActiveRecursively(false);
			BaseCampController.Instance.m_DownArrow.SetActiveRecursively(false);
		}
		m_waitingForProducts = false;
		m_productFetchSuccess = false;
		StoreProductManager.OnExchangeProductsReceived += OnProductListReceived;
		if (StoreProductManager.Instance.GetExchangeProducts() == null)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				StoreProductManager.Instance.FetchExchangeProducts();
				m_waitingForProducts = true;
				EtceteraPlatformWrapper.ShowWaitingWithLabel(Language.Get("S_IAP_CONNECTING_DROID"));
				UIManager.instance.LockInput();
				while (m_waitingForProducts)
				{
					yield return null;
				}
				EtceteraPlatformWrapper.HideWaitingDialog();
				UIManager.instance.UnlockInput();
			}
			else if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				BaseCampController.Instance.ShowWifiErrorMessageAndSwitchPanel();
			}
		}
		else
		{
			m_productFetchSuccess = true;
		}
		StoreProductManager.OnExchangeProductsReceived -= OnProductListReceived;
		if (!m_productFetchSuccess)
		{
			BaseCampController.Instance.ShowWifiErrorMessageAndSwitchPanel();
			yield break;
		}
		PopulateProducts(FromBaseCamp);
		if (FromBaseCamp)
		{
			DailyDoubleController.Instance.SetBottomTicker(true);
		}
	}

	private void OnProductListReceived(bool Success)
	{
		m_waitingForProducts = false;
		m_productFetchSuccess = Success;
	}

	private void PopulateProducts(bool FromBaseCamp)
	{
		m_inbasecamp = FromBaseCamp;
		MonoBehaviour.print("POPULATE PRODUCTS " + Time.time);
		if (FromBaseCamp)
		{
			BaseCampController.Instance.ExchangeLink.SetActiveRecursively(true);
			GameObject gameObject = GameObject.FindWithTag("BC_GSScrollList");
			if (gameObject != null)
			{
				m_scrollList = gameObject.GetComponent("UIScrollList") as UIScrollList;
			}
			else
			{
				Debug.LogError("TempObj is null for BC_GSScrollList");
			}
		}
		else
		{
			GameObject gameObject = GameObject.FindWithTag("IG_GSScrollList");
			if (gameObject != null)
			{
				m_scrollList = gameObject.GetComponent("UIScrollList") as UIScrollList;
			}
			else
			{
				Debug.LogError("TempObj is null for IG_GSScrollList");
			}
		}
		m_bPopulated = false;
		if (m_bPopulated)
		{
			return;
		}
		m_scrollList.ClearList(true);
		GemStoreProduct exchangeProducts = StoreProductManager.Instance.GetExchangeProducts();
		if (!(exchangeProducts != null))
		{
			return;
		}
		SpenderType spenderType = GetSpenderType(exchangeProducts);
		string text;
		string[] array;
		if (!FromBaseCamp || SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) > 0)
		{
			switch (spenderType)
			{
			case SpenderType.Low:
				text = "B";
				array = new string[5]
				{
					StoreProductManager.Bundle1aIndentifier,
					StoreProductManager.Bundle2Indentifier,
					StoreProductManager.Bundle3Indentifier,
					StoreProductManager.Bundle4Indentifier,
					StoreProductManager.Bundle5Indentifier
				};
				break;
			case SpenderType.Medium:
				text = "D";
				array = new string[5]
				{
					StoreProductManager.Bundle1Indentifier,
					StoreProductManager.Bundle2Indentifier,
					StoreProductManager.Bundle3Indentifier,
					StoreProductManager.Bundle4Indentifier,
					StoreProductManager.Bundle5Indentifier
				};
				break;
			default:
				text = "E";
				array = new string[5]
				{
					StoreProductManager.Bundle1Indentifier,
					StoreProductManager.Bundle2Indentifier,
					StoreProductManager.Bundle3Indentifier,
					StoreProductManager.Bundle4bIndentifier,
					StoreProductManager.Bundle5aIndentifier
				};
				break;
			}
		}
		else if (spenderType == SpenderType.Low)
		{
			text = "A";
			array = new string[5]
			{
				StoreProductManager.Bundle1aIndentifier,
				StoreProductManager.Bundle2Indentifier,
				StoreProductManager.Bundle3aIndentifier,
				StoreProductManager.Bundle4aIndentifier,
				StoreProductManager.TreasureUpgradeIdentifier
			};
		}
		else
		{
			text = "C";
			array = new string[5]
			{
				StoreProductManager.Bundle1Indentifier,
				StoreProductManager.Bundle2Indentifier,
				StoreProductManager.Bundle3Indentifier,
				StoreProductManager.Bundle4Indentifier,
				StoreProductManager.TreasureUpgradeIdentifier
			};
		}
		Dictionary<string, string> resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources("UserSegment" + text, out resourceDictionary) && resourceDictionary != null)
		{
			Debug.Log("Slot1name: " + Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot1Name", "none"));
			Debug.Log("Slot2name: " + Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot2Name", "none"));
			Debug.Log("Slot3name: " + Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot3Name", "none"));
			Debug.Log("Slot4name: " + Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot4Name", "none"));
			Debug.Log("Slot5name: " + Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot5Name", "none"));
			array[0] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot1Name", array[0]);
			array[1] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot2Name", array[1]);
			array[2] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot3Name", array[2]);
			array[3] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot4Name", array[3]);
			array[4] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ProductSlot5Name", array[4]);
		}
		array = ((SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) <= 0) ? new string[5]
		{
			StoreProductManager.Bundle1Indentifier,
			StoreProductManager.Bundle2Indentifier,
			StoreProductManager.Bundle3Indentifier,
			StoreProductManager.Bundle4Indentifier,
			StoreProductManager.TreasureUpgradeIdentifier
		} : new string[4]
		{
			StoreProductManager.Bundle1Indentifier,
			StoreProductManager.Bundle2Indentifier,
			StoreProductManager.Bundle3Indentifier,
			StoreProductManager.Bundle4Indentifier
		});
		string[][] array2 = new string[1][] { array };
		string[] array3 = array;
		foreach (string text2 in array3)
		{
			GemStoreProduct.GemStoreProductData gemStoreProductData = exchangeProducts.FindGemProduct(text2);
			Dictionary<string, string> resourceDictionary2 = null;
			if (Bedrock.GetRemoteUserResources(text2, out resourceDictionary2) && resourceDictionary2 != null)
			{
				gemStoreProductData.UpdateFromBedrock(resourceDictionary2);
			}
		}
		int num = array2.Length;
		for (int j = 0; j < num; j++)
		{
			int num2 = array2[j].Length;
			MonoBehaviour.print("CREATING EXCHANGE ITEM CONTAINER " + Time.time);
			GameObject gameObject2 = new GameObject("ExchangeContainer");
			if (FromBaseCamp)
			{
				gameObject2.transform.localPosition = new Vector3(0f, -915f, 0f);
			}
			else
			{
				gameObject2.transform.localPosition = new Vector3(0f, -725f, 0f);
			}
			gameObject2.AddComponent<UIListItemContainer>();
			int num3 = 0;
			float num4 = (num2 - 1) / 2;
			if (num2 % 2 == 0)
			{
				num4 += 0.5f;
			}
			float num5 = 385f;
			float num6 = 430f;
			ExchangeItem[] array4 = new ExchangeItem[num2];
			for (int k = 0; k < num2; k++)
			{
				array4[k] = null;
				string identifier = array2[j][k];
				GemStoreProduct.GemStoreProductData gemStoreProductData2 = exchangeProducts.FindGemProduct(identifier);
				if (gemStoreProductData2 != null)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate(m_productItemVerticalPrefab) as GameObject;
					ExchangeItem component = gameObject3.GetComponent<ExchangeItem>();
					component.Populate(gemStoreProductData2);
					array4[k] = component;
					gameObject3.transform.localPosition = new Vector3(((float)k - num4) * num5, 0f - num6, 0f);
					gameObject3.transform.parent = gameObject2.transform;
					num3++;
				}
			}
			if (num3 <= 0)
			{
				continue;
			}
			m_scrollList.AddItem(gameObject2);
			int num7 = -1;
			int num8 = 0;
			for (int l = 0; l < num2; l++)
			{
				if (array4[l] != null && array4[l].GetPercentageOffAmount() != 0)
				{
					Debug.Log("GetPercentageOffAmount != 0 for: " + l);
					if (num8 == 0)
					{
						num7 = l;
					}
					else if (array4[l].GetUnitPrice() <= array4[num7].GetUnitPrice())
					{
						num7 = l;
					}
					num8++;
				}
				else if (array4[l] == null)
				{
					Debug.Log("ExchangeArray null for: " + l);
				}
				else
				{
					Debug.Log("GetPercentageOffAmount: " + array4[l].GetPercentageOffAmount() + " for: " + l);
				}
			}
			Debug.Log("bestvalueindex: " + num7 + ", percentcount: " + num8);
			if (num7 != -1 && num8 >= 2)
			{
				Debug.Log("setting best value");
				array4[num7].SetBestValue();
			}
		}
		m_bPopulated = true;
		if (!FromBaseCamp && InGameStorePopup.Instance != null)
		{
			InGameStorePopup.Instance.ShowCancelButton();
		}
	}

	private SpenderType GetSpenderType(GemStoreProduct storeProducts)
	{
		if (SwrveServerVariables.Instance.LowSpender == -1 || SwrveServerVariables.Instance.MediumSpender == -1)
		{
			if (SwrveServerVariables.Instance.LowSpender == -1 && SwrveServerVariables.Instance.MediumSpender == -1)
			{
				return SpenderType.High;
			}
			if (SwrveServerVariables.Instance.LowSpender == -1)
			{
				return SpenderType.Low;
			}
			return SpenderType.Medium;
		}
		string text = string.Empty;
		int num = SwrveServerVariables.Instance.LowSpender;
		int num2 = SwrveServerVariables.Instance.MediumSpender;
		GemStoreProduct.GemStoreProductData gemStoreProductData = storeProducts.FindGemProduct(StoreProductManager.Bundle1Indentifier);
		if (gemStoreProductData == null)
		{
			Debug.LogError("Could not find reference product");
		}
		else
		{
			text = gemStoreProductData.CurrencySymbol;
		}
		if (text == "$")
		{
			Debug.Log("Currency is in dollars");
		}
		else
		{
			if (!(text == "€"))
			{
				Debug.Log("Unkown currency, force medium spender");
				return SpenderType.Medium;
			}
			num = (int)((float)num * 1.3f);
			num2 = (int)((float)num2 * 1.3f);
			Debug.Log("Currency is in euros, low was: " + SwrveServerVariables.Instance.LowSpender + ", now: " + num + ", medium was: " + SwrveServerVariables.Instance.MediumSpender + ", now: " + num2);
		}
		SpenderType spenderType = SpenderType.High;
		if (SecureStorage.Instance.GetTotalMTX() < num)
		{
			spenderType = SpenderType.Low;
		}
		else if (SecureStorage.Instance.GetTotalMTX() < num2)
		{
			spenderType = SpenderType.Medium;
		}
		Debug.Log("(ewh) spender type: " + spenderType);
		return spenderType;
	}

	public void OnProductSelected(GemStoreProduct.GemStoreProductData Data)
	{
		Debug.Log("OnProductSelected: " + Data.Identifier);
		BuyProduct(Data);
	}

	private void BuyProduct(GemStoreProduct.GemStoreProductData ProdData)
	{
		if (ProdData.Identifier == StoreProductManager.TreasureUpgradeIdentifier)
		{
			if (SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) <= 0)
			{
				ConfirmDialog confirmDialog = UnityEngine.Object.Instantiate(m_confirmDialogPrefab) as ConfirmDialog;
				string title = Language.Get("S_TREASURE_UPG_T");
				string message = Language.Get("S_TREASURE_UPG_D");
				string opt = Language.Get("S_BUY");
				string opt2 = Language.Get("S_NO");
				m_productToPurchase = ProdData;
				StartCoroutine(confirmDialog.Display(title, message, opt, opt2, TreasureUpgradePurchaseConfirmed, TreasureUpgradePurchaseCancelled, TreasureUpgradePurchaseCancelled));
			}
		}
		else
		{
			StartCoroutine(PurchaseProduct(ProdData));
		}
	}

	private void TreasureUpgradePurchaseConfirmed()
	{
		if (m_productToPurchase != null)
		{
			StartCoroutine(PurchaseProduct(m_productToPurchase));
		}
	}

	private void TreasureUpgradePurchaseCancelled()
	{
		m_productToPurchase = null;
	}

	private IEnumerator PurchaseProduct(GemStoreProduct.GemStoreProductData ProdData)
	{
		UIManager.instance.LockInput();
		m_productToPurchase = ProdData;
		m_waitingForPurchase = true;
		StoreProductManager.OnBuyExchangeSuccess += OnBuySuccess;
		StoreProductManager.OnBuyExchangeFailed += OnBuyFailed;
		StoreProductManager.Instance.BuyExchangeProduct(ProdData.Identifier);
		while (m_waitingForPurchase)
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				SwrveEventsPurchase.ExchangePurchaseFailed(m_productToPurchase.Identifier);
				EtceteraPlatformWrapper.HideWaitingDialog();
				BaseCampController.Instance.DelayedWifiErrorMessage();
				m_waitingForPurchase = false;
			}
			else if (!StoreProductManager.Instance.CanBuyExchangeProducts)
			{
				SwrveEventsPurchase.ExchangePurchaseFailed(m_productToPurchase.Identifier);
				EtceteraPlatformWrapper.HideWaitingDialog();
				BaseCampController.Instance.DelayedRestrictionMessage();
				m_waitingForPurchase = false;
			}
			yield return null;
		}
		StoreProductManager.OnBuyExchangeSuccess -= OnBuySuccess;
		StoreProductManager.OnBuyExchangeFailed -= OnBuyFailed;
		UIManager.instance.UnlockInput();
	}

	private void OnBuySuccess(string Id)
	{
		SwrveEventsPurchase.ExchangePurchaseSuccess(m_productToPurchase.Identifier);
		float num = Convert.ToSingle(m_productToPurchase.Price);
		ulong cost = (ulong)(num * 100f);
		SecureStorage.Instance.IncreaseTotalMTX((int)num);
		Debug.Log("(ewh) OnBuySuccess: " + m_productToPurchase.Identifier + ", in base camp: " + m_inbasecamp);
		bool flag = false;
		if (m_productToPurchase.Identifier == StoreProductManager.TreasureUpgradeIdentifier)
		{
			SecureStorage.Instance.ChangeItemCount(StoreProductManager.TreasureUpgradeIdentifier, 1);
			if (m_inbasecamp)
			{
				PopulateProducts(true);
			}
		}
		else
		{
			int num2 = m_productToPurchase.AmountOfGems * (int)DailyDoubleController.Instance.DD_diamondsMultiplier;
			SwrveEventsPurchase.GemPack(m_productToPurchase.Identifier, cost, (ulong)num2, m_productToPurchase.CurrencyCode);
			SecureStorage.Instance.ChangeGems(num2);
			if (!m_inbasecamp)
			{
				Debug.Log("(ewh) purchase in game for diamonds to continue");
				DiamondsToContinueDialog.Instance.UpdateDiamondCountOnGameHud();
				DiamondsToContinueDialog.Instance.AdditionalGemsRequiredForContinue = DiamondsToContinueDialog.Instance.TotalGemsRequiredForContinue - SecureStorage.Instance.GetGems();
				if (DiamondsToContinueDialog.Instance.AdditionalGemsRequiredForContinue <= 0)
				{
					flag = true;
				}
				else
				{
					InGameStorePopup.Instance.SetAdditionalAmount(DiamondsToContinueDialog.Instance.AdditionalGemsRequiredForContinue, true);
				}
			}
		}
		EtceteraPlatformWrapper.HideWaitingDialog();
		ShowAlertDialog("S_IAP_PURCHASE_SUCCESS_TITLE", "S_IAP_PURCHASE_SUCCESS_BODY", "S_IAP_OK");
		m_waitingForPurchase = false;
		m_productToPurchase = null;
		if (flag)
		{
			DiamondsToContinueDialog.Instance.PurchaseToContinue();
		}
	}

	private IEnumerator ShowThanksDialog(int num)
	{
		string Title = Language.Get("S_THANKS");
		string Body = "x " + num;
		BigMessage levelUpMessage = (BigMessage)UnityEngine.Object.Instantiate(m_thanksDialogPrefab);
		yield return StartCoroutine(levelUpMessage.Display(Title, Body, 5f));
	}

	private void OnBuyFailed(string error)
	{
		if (m_productToPurchase != null)
		{
			SwrveEventsPurchase.ExchangePurchaseFailed(m_productToPurchase.Identifier);
		}
		EtceteraPlatformWrapper.HideWaitingDialog();
		ShowAlertDialog("S_IAP_PURCHASE_FAILED_TITLE", "S_IAP_PURCHASE_FAILED_BODY", "S_IAP_OK");
		m_waitingForPurchase = false;
		m_productToPurchase = null;
	}

	private void ShowAlertDialog(string TitleKey, string BodyKey, string ButtonKey)
	{
		string strTitle = Language.Get(TitleKey);
		string strBody = Language.Get(BodyKey);
		string strOK = Language.Get(ButtonKey);
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}
}
