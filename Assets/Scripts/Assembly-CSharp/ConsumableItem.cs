using UnityEngine;

public class ConsumableItem : BaseCampProductItem
{
	public SpriteText m_ownedCount;

	public SpriteText m_levelText;

	public GameObject m_Icon;

	public GameObject m_Owned;

	public GameObject m_Cost;

	public GameObject m_Worn;

	public GameObject m_Popular;

	public GameObject m_Sale;

	public GameObject m_FullyUpgraded;

	public SpriteText m_SaleText;

	public SpriteText m_SaleDurationText;

	public UpgradeBlips m_UpgradeBlips;

	public GameObject m_SparkleOverlayPrefab;

	private GameObject m_SparkleOverlay;

	private bool m_Populated;

	private bool m_bLastSaleState;

	private void Awake()
	{
		m_Populated = false;
		SecureStorage.playerItemsChanged += UpdateOwnedCount;
	}

	private void OnDestroy()
	{
		SecureStorage.playerItemsChanged -= UpdateOwnedCount;
	}

	private void OnEnable()
	{
		if (m_Populated)
		{
			UpdateIcon();
			UpdateCost();
		}
	}

	public override void PopulateItem(BaseCampProducts.ProductData Data)
	{
		base.PopulateItem(Data);
		m_Populated = true;
		SetOwnedCount();
		UpdateIcon();
		if (Data.Identifier == "consumable.macaw" || Data.Identifier == "upgrade.jaguar")
		{
			m_SparkleOverlay = (GameObject)Object.Instantiate(m_SparkleOverlayPrefab);
			m_SparkleOverlay.transform.parent = m_Icon.transform;
		}
	}

	protected override void UpdateIcon()
	{
		if (m_Icon != null)
		{
			int suffix = 1;
			if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_CONSUMABLE)
			{
				suffix = SecureStorage.Instance.GetItemCount(m_productData.AssociatedUpgradeID) + 1;
			}
			if (m_Icon.GetComponent<Renderer>().material.mainTexture != null && m_Icon.GetComponent<Renderer>().material.mainTexture.name != base.name)
			{
				m_Icon.GetComponent<Renderer>().material.mainTexture = StoreProductManager.LoadTextureFor(m_productData.Identifier, suffix);
			}
		}
	}

	public virtual void LateUpdate()
	{
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		if (m_Owned != null)
		{
			if (itemCount == 0 || m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
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
		}
		if (m_UpgradeBlips != null)
		{
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
			else if (m_UpgradeBlips != null && m_UpgradeBlips.gameObject.active)
			{
				m_UpgradeBlips.gameObject.SetActiveRecursively(false);
			}
		}
		if (m_FullyUpgraded != null)
		{
			m_FullyUpgraded.SetActiveRecursively(false);
		}
		if (m_Cost != null && m_productData.HasZeroValue(m_Level))
		{
			if (m_Cost.active)
			{
				m_Cost.SetActiveRecursively(false);
			}
			if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE && m_FullyUpgraded != null)
			{
				m_FullyUpgraded.SetActiveRecursively(true);
			}
		}
		UpdatePopular();
		UpdateSaleItem();
	}

	protected void UpdatePopular()
	{
		if (m_Popular != null && m_productData.IsPopular != m_Popular.active)
		{
			m_Popular.SetActiveRecursively(m_productData.IsPopular);
		}
	}

	protected void UpdateSaleItem()
	{
		m_Sale.SetActiveRecursively(false);
		if (!(StoreProductManager.Instance != null))
		{
			return;
		}
		if (StoreProductManager.Instance.IsSaleActive)
		{
			m_bLastSaleState = true;
			if (!(m_Sale != null))
			{
				return;
			}
			BaseCampProducts.ProductData saleItem = StoreProductManager.Instance.SaleItem;
			if (saleItem != null && saleItem.Identifier == m_productData.Identifier)
			{
				if (m_Popular != null)
				{
					m_Popular.SetActiveRecursively(false);
				}
				m_Sale.SetActiveRecursively(true);
				uint saleTimeLeft = StoreProductManager.Instance.SaleTimeLeft;
				string shortFuzzyTimeStringFromSeconds = TimeUtils.GetShortFuzzyTimeStringFromSeconds(saleTimeLeft);
				m_SaleText.Text = Language.Get("S_SALES_ONSALE");
				m_SaleDurationText.Text = string.Format(Language.Get("S_TIME_TO_GO"), shortFuzzyTimeStringFromSeconds);
			}
		}
		else
		{
			if (m_bLastSaleState)
			{
				UpdateCost();
			}
			m_bLastSaleState = false;
		}
	}

	public override void OnItemBought()
	{
		base.OnItemBought();
		SetOwnedCount();
		UpdateIcon();
	}

	public void SetOwnedCount()
	{
		if (m_ownedCount != null)
		{
			m_ownedCount.Text = SecureStorage.Instance.GetItemCount(m_productData.Identifier).ToString();
		}
		if (m_UpgradeBlips != null)
		{
			m_UpgradeBlips.SetLevel(SecureStorage.Instance.GetItemCount(m_productData.Identifier));
		}
	}

	public void UpdateOwnedCount(string dummyString, int dummyInt)
	{
		SetOwnedCount();
	}
}
