using System;
using UnityEngine;

public class ExchangeItem : MonoBehaviour
{
	public SpriteText m_Cost;

	public GameObject m_Sale;

	public GameObject m_Sold;

	public GameObject m_Loyalty;

	public GameObject m_MoneyTab;

	public GameObject m_NameTab;

	public GameObject m_Icon;

	public SpriteText m_AmountText;

	public SpriteText m_SaleText;

	public SpriteText m_BestValueText;

	private GemStoreProduct.GemStoreProductData m_ProductData;

	public GameObject m_SparkleOverlay;

	private bool m_bestvalue;

	public void Awake()
	{
	}

	public void Populate(GemStoreProduct.GemStoreProductData Data)
	{
		m_ProductData = Data;
		if (m_Icon != null)
		{
			string text = Data.Identifier;
			if (text.Contains("dle1a"))
			{
				text = text.Replace("dle1a", "dle1");
			}
			else if (text.Contains("dle3a"))
			{
				text = text.Replace("dle3a", "dle3");
			}
			else if (text.Contains("dle4a"))
			{
				text = text.Replace("dle4a", "dle4");
			}
			else if (text.Contains("dle4b"))
			{
				text = text.Replace("dle4b", "dle4");
			}
			else if (text.Contains("dle5a"))
			{
				text = text.Replace("dle5a", "dle5");
			}
			string text2 = null;
			text2 = ((!TBFUtils.Is256mbDevice()) ? string.Format("Textures/{0}", text) : string.Format("Textures/LowQual/{0}", text));
			if (m_Icon.GetComponent<Renderer>().material.mainTexture.name != text2)
			{
				m_Icon.GetComponent<Renderer>().material.mainTexture = Resources.Load(text2) as Texture;
			}
		}
		string title = Data.Title;
		if (title.StartsWith("S_"))
		{
			title = Language.Get(title);
		}
		m_Cost.Text = Data.CurrencySymbol + Data.Price;
		if (Data.AmountOfGems > 0)
		{
			int num = Data.AmountOfGems * (int)DailyDoubleController.Instance.DD_diamondsMultiplier;
			m_AmountText.Text = num.ToString();
		}
		else
		{
			m_AmountText.Text = string.Empty;
		}
		m_bestvalue = false;
	}

	public void Update()
	{
	}

	public void LateUpdate()
	{
		if (m_ProductData != null)
		{
			if (GetPercentageOffAmount() != 0)
			{
				if (!m_Sale.active)
				{
					m_Sale.SetActiveRecursively(true);
				}
				m_SaleText.Text = string.Format(Language.Get("S_SALES_REDUCED"), GetPercentageOffAmount());
				m_BestValueText.gameObject.active = m_bestvalue;
			}
			else if (m_Sale.active)
			{
				m_Sale.SetActiveRecursively(false);
			}
			if (m_Loyalty.active)
			{
				m_Loyalty.SetActiveRecursively(false);
			}
			if (m_ProductData.Identifier == StoreProductManager.TreasureUpgradeIdentifier)
			{
				if (!m_NameTab.active)
				{
					m_NameTab.SetActiveRecursively(true);
				}
			}
			else if (m_NameTab.active)
			{
				m_NameTab.SetActiveRecursively(false);
			}
			if (!m_MoneyTab.active)
			{
				m_MoneyTab.SetActiveRecursively(true);
			}
		}
		if (m_ProductData.Identifier != StoreProductManager.FreeGemsIdentifier && !m_MoneyTab.active)
		{
			m_MoneyTab.SetActiveRecursively(true);
		}
	}

	private void SetOwnedCount()
	{
	}

	private void OnItemSelected()
	{
		GemStoreController.Instance.OnProductSelected(m_ProductData);
	}

	public float GetUnitPrice()
	{
		float num = Convert.ToSingle(m_ProductData.Price);
		return num / (float)m_ProductData.AmountOfGems;
	}

	public int GetPercentageOffAmount()
	{
		if (SwrveServerVariables.Instance.DD_enabled && SwrveServerVariables.Instance.DD_diamondsMultiplier != 1f)
		{
			return 0;
		}
		return m_ProductData.PercentageOffAmount;
	}

	public void SetBestValue()
	{
		m_bestvalue = true;
	}
}
