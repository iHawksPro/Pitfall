using System.Collections;
using UnityEngine;

public class BaseCampProductItem : MonoBehaviour
{
	public UIListItemContainer m_container;

	public SpriteText m_gemsText;

	public SpriteText m_coinsText;

	public SpriteText m_title;

	public SpriteText m_description;

	public UIButton m_ItemBtn;

	public BuyDialog m_BuyItemDialogPrefab;

	[HideInInspector]
	public BaseCampProducts.ProductData m_productData;

	public int m_Level;

	protected static bool m_BuyDialogActive;

	public static BaseCampProducts.ProductData m_FocusProductDiscount;

	public UIListItemContainer GetContainer()
	{
		return m_container;
	}

	public virtual void PopulateItem(BaseCampProducts.ProductData Data)
	{
		m_productData = Data;
		UpdateCost();
		if ((bool)m_title)
		{
			int num = m_productData.Quantity(m_Level);
			if (num > 1)
			{
				m_title.Text = string.Format("{0} x{1}", Language.Get(Data.Title), num.ToString());
			}
			else
			{
				m_title.Text = Language.Get(Data.Title);
			}
		}
		m_ItemBtn.scriptWithMethodToInvoke = this;
		m_ItemBtn.methodToInvoke = "OnItemPressed";
	}

	public void UpdateCost()
	{
		int value = 0;
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
		{
			value = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		}
		m_Level = Mathf.Clamp(value, 0, m_productData.Levels.Length - 1);
		if (m_gemsText != null)
		{
			m_gemsText.Text = m_productData.GemsPrice(m_Level).ToString();
		}
		if (m_coinsText != null)
		{
			m_coinsText.Text = m_productData.CoinPrice(m_Level).ToString();
			if (!m_productData.CanPlayerAffordItem(false, m_Level))
			{
				m_coinsText.SetColor(new Color(0.7f, 0f, 0f, 1f));
			}
			else
			{
				m_coinsText.SetColor(new Color(1f, 1f, 1f, 1f));
			}
		}
	}

	protected virtual void UpdateIcon()
	{
	}

	public virtual void OnItemBought()
	{
	}

	private bool CanPlayerBuyItem()
	{
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		int num = m_productData.MaxNumberPlayerCanBuy();
		return itemCount < num;
	}

	protected virtual void OnItemPressed()
	{
		Debug.Log("Pressed " + m_productData.Identifier);
		StartCoroutine(HandleItemPressed());
	}

	private IEnumerator HandleItemPressed()
	{
		if (!m_BuyDialogActive)
		{
			m_BuyDialogActive = true;
			CommonAnimations.AnimateButton(m_ItemBtn.gameObject);
			yield return StartCoroutine(BuyItemDialog());
		}
	}

	protected IEnumerator BuyItemDialog()
	{
		if (m_FocusProductDiscount != null && m_FocusProductDiscount.Identifier == m_productData.Identifier)
		{
			BuyDialog dialog = (BuyDialog)Object.Instantiate(m_BuyItemDialogPrefab);
			yield return StartCoroutine(dialog.Display(m_FocusProductDiscount, m_Level, OnPurchaseConfirmed, OnPurchaseCancelled));
		}
		else
		{
			BuyDialog dialog2 = (BuyDialog)Object.Instantiate(m_BuyItemDialogPrefab);
			yield return StartCoroutine(dialog2.Display(m_productData, m_Level, OnPurchaseConfirmed, OnPurchaseCancelled));
		}
	}

	protected virtual void OnPurchaseConfirmed()
	{
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_UPGRADE)
		{
			if (m_productData.HasZeroValue(m_Level))
			{
				MenuSFX.Instance.Play2D("Upgrade2");
			}
			else
			{
				MenuSFX.Instance.Play2D("Upgrade1");
			}
		}
		OnItemBought();
		UpdateCost();
		UpdateIcon();
		m_BuyDialogActive = false;
	}

	protected virtual void OnPurchaseCancelled()
	{
		m_BuyDialogActive = false;
	}
}
