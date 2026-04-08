using System;
using System.Collections;
using UnityEngine;

public class BuyDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public UIButton m_GemsButton;

	public UIButton m_CoinsButton;

	public UIButton m_cancelButton;

	public GameObject m_visibleContents;

	public SpriteText m_ownedCount;

	public SpriteText m_SaleText;

	public GameObject m_Icon;

	public GameObject m_Owned;

	public GameObject m_CoinsWhole;

	public GameObject m_GemsWhole;

	public GameObject m_Wear;

	public UpgradeBlips m_UpgradeBlips;

	private Action m_OnPurchaseConfirmed;

	private Action m_OnPurchaseCancelled;

	private bool m_userCancelled;

	private BaseCampProducts.ProductData m_productData;

	private int m_Level;

	public static BuyDialog Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("MenuPopup");
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
		StoreProductManager.Instance.SaleLocked = false;
		Instance = null;
	}

	private void HandleStateDeactivated(string StateName)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(BaseCampProducts.ProductData Data, int Level, Action OnPurchaseConfirmed, Action OnPurchaseCancelled)
	{
		m_OnPurchaseConfirmed = OnPurchaseConfirmed;
		m_OnPurchaseCancelled = OnPurchaseCancelled;
		yield return StartCoroutine(DoDisplay(Data, Level));
	}

	public IEnumerator DoDisplay(BaseCampProducts.ProductData Data, int Level)
	{
		DialogManager.DialogLock();
		if (StoreProductManager.Instance.SaleItem != null && StoreProductManager.Instance.SaleItem.Identifier == Data.Identifier)
		{
			StoreProductManager.Instance.SaleLocked = true;
		}
		m_productData = Data;
		m_Level = Level;
		SwrveEventsPurchase.ProductViewed(m_productData.Identifier, m_Level);
		UpdateContents();
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		SwrveEventsPurchase.ProductClosed(m_productData.Identifier, m_Level);
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		DialogManager.DialogUnlock();
		UnityEngine.Object.Destroy(base.gameObject);
		BaseCampProductItem.m_FocusProductDiscount = null;
	}

	private void UpdateContents()
	{
		int val = 0;
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
		{
			val = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		}
		m_Level = Math.Min(val, m_productData.Levels.Length - 1);
		int num = m_productData.Quantity(m_Level);
		if (num > 1)
		{
			m_titleText.Text = string.Format("{0}\nx{1}", m_productData.GetTitle(), num.ToString());
		}
		else
		{
			m_titleText.Text = m_productData.GetTitle();
		}
		m_bodyText.Text = m_productData.Levels[m_Level].GetDesc();
		m_userCancelled = false;
		int suffix = 1;
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_CONSUMABLE)
		{
			suffix = SecureStorage.Instance.GetItemCount(m_productData.AssociatedUpgradeID) + 1;
		}
		if (m_Icon.GetComponent<Renderer>().material.mainTexture != null && m_Icon.GetComponent<Renderer>().material.mainTexture.name != base.name)
		{
			m_Icon.GetComponent<Renderer>().material.mainTexture = StoreProductManager.LoadTextureFor(m_productData.Identifier, suffix);
		}
		m_SaleText.Text = " ";
		if (m_productData.Discount != 0)
		{
			m_SaleText.Text = string.Format("-{0}%", m_productData.Discount);
		}
		else if (StoreProductManager.Instance.SaleItem != null && StoreProductManager.Instance.SaleItem.Identifier == m_productData.Identifier)
		{
			m_SaleText.Text = string.Format("-{0}%", (int)StoreProductManager.Instance.SalePercentOff);
		}
		SetOwnedCount();
		m_GemsButton.Text = "    " + m_productData.GemsPrice(m_Level);
		m_CoinsButton.Text = "    " + m_productData.CoinPrice(m_Level);
		if (!m_productData.CanPlayerAffordItem(false, m_Level))
		{
			m_CoinsButton.spriteText.SetColor(new Color(0.7f, 0f, 0f, 1f));
		}
		else
		{
			m_CoinsButton.spriteText.SetColor(new Color(1f, 1f, 1f, 1f));
		}
		if (m_productData.HasZeroValue(m_Level))
		{
			m_GemsWhole.SetActiveRecursively(false);
			m_CoinsWhole.gameObject.SetActiveRecursively(false);
		}
		else if (m_productData.CoinPrice(m_Level) == 0)
		{
			m_GemsWhole.transform.localPosition = new Vector3(90f, m_GemsWhole.transform.localPosition.y, m_GemsWhole.transform.localPosition.z);
			m_CoinsWhole.gameObject.SetActiveRecursively(false);
		}
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT)
		{
			if (m_productData.Identifier == OutfitItem.SuperIdentifier)
			{
				m_GemsWhole.SetActiveRecursively(false);
				m_CoinsWhole.gameObject.SetActiveRecursively(false);
				if (!TrialsDataManager.Instance.HaveCollectedAllRelics)
				{
					m_Wear.SetActiveRecursively(false);
					m_titleText.Text = Language.Get("S_SUPERHARRY_LOCKED");
					m_bodyText.Text = Language.Get("S_SUPERHARRY_LOCKED_BODY");
				}
				else
				{
					m_Wear.SetActiveRecursively(true);
					m_bodyText.Text = Language.Get("S_SUPERHARRY_UNLOCKED_BODY");
				}
			}
			else
			{
				int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
				if (m_productData.HasZeroValue(m_Level) || itemCount > 0)
				{
					m_GemsWhole.SetActiveRecursively(false);
					m_CoinsWhole.gameObject.SetActiveRecursively(false);
					m_Wear.SetActiveRecursively(true);
				}
				else
				{
					m_Wear.SetActiveRecursively(false);
				}
			}
		}
		else
		{
			m_Wear.SetActiveRecursively(false);
		}
		if (BaseCampController.Instance.TutorialActive())
		{
			m_bodyText.Text = Language.Get("S_CHECK_TUT_3");
			m_cancelButton.gameObject.SetActiveRecursively(false);
			if (SecureStorage.Instance.GetItemCount("checkpoint.2000") == 0)
			{
				SecureStorage.Instance.ChangeGems(10);
				MonoBehaviour.print("DBERNARD - CHANGEGEMS(10)");
			}
		}
	}

	private void Update()
	{
		if (SecureStorage.Instance.GetItemCount(m_productData.Identifier) == 0 || m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT || m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE || m_productData.Category == BaseCampProducts.StoreCategory.BCP_CHECKPOINT)
		{
			if (m_Owned.active)
			{
				m_Owned.SetActiveRecursively(false);
			}
		}
		else if (!m_Owned.active)
		{
			m_Owned.SetActiveRecursively(true);
		}
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
		{
			if (!m_UpgradeBlips.gameObject.active)
			{
				m_UpgradeBlips.gameObject.SetActiveRecursively(true);
			}
			if (m_UpgradeBlips != null)
			{
				m_UpgradeBlips.SetLevel(SecureStorage.Instance.GetItemCount(m_productData.Identifier));
			}
		}
		else if (m_UpgradeBlips.gameObject.active)
		{
			m_UpgradeBlips.gameObject.SetActiveRecursively(false);
		}
		if (Input.GetKeyDown(KeyCode.Escape) && !BaseCampController.Instance.TutorialActive())
		{
			OnCancelPressed();
		}
	}

	private void SetOwnedCount()
	{
		if ((m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT || m_productData.Category == BaseCampProducts.StoreCategory.BCP_CHECKPOINT) && m_Owned.active)
		{
			m_Owned.SetActiveRecursively(false);
		}
		m_ownedCount.Text = SecureStorage.Instance.GetItemCount(m_productData.Identifier).ToString();
	}

	private bool CanPlayerBuyItem()
	{
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		int num = m_productData.MaxNumberPlayerCanBuy();
		return itemCount < num;
	}

	private void DoBuyItem(bool UsingGems)
	{
		if (CanPlayerBuyItem())
		{
			int itemLevel = 0;
			if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
			{
				itemLevel = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
			}
			BaseCampController.Instance.BuyItem(m_productData, itemLevel, UsingGems, m_OnPurchaseConfirmed, OnCancelPressed);
			BaseCampCheckpointsPanel.Instance.RefreshProductListIcons();
			SetOwnedCount();
		}
	}

	private void OnGemsPressed()
	{
		DoBuyItem(true);
		UpdateContents();
		MenuSFX.Instance.Play2D("MenuBuy1");
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		if ((m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT || m_productData.Category == BaseCampProducts.StoreCategory.BCP_CHECKPOINT) && itemCount > 0)
		{
			m_userCancelled = true;
		}
		if (BaseCampController.Instance.TutorialActive())
		{
			BaseCampController.Instance.BackButton.gameObject.SetActiveRecursively(true);
		}
	}

	private void OnCoinsPressed()
	{
		DoBuyItem(false);
		UpdateContents();
		MenuSFX.Instance.Play2D("MenuBuy1");
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		if ((m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT || m_productData.Category == BaseCampProducts.StoreCategory.BCP_CHECKPOINT) && itemCount > 0)
		{
			m_userCancelled = true;
		}
	}

	public void OnCancelPressed()
	{
		if (!BaseCampController.Instance.TutorialActive())
		{
			m_userCancelled = true;
			MenuSFX.Instance.Play2D("MenuCancel");
			m_OnPurchaseCancelled();
		}
	}

	public void OnWearItem()
	{
		SecureStorage.Instance.SetCurrentCostume(m_productData.Identifier);
		m_userCancelled = true;
		if (OutfitOfTheDayManager.Instance.IsOOTD(m_productData.Identifier))
		{
			AchievementManager.Instance.SetCompleted("oneshot.pose");
		}
	}
}
