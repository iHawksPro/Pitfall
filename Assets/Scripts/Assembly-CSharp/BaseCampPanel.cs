using System.Collections;
using UnityEngine;

public class BaseCampPanel : UIPanel
{
	public BaseCampProducts m_productDefinitions;

	public SpriteText m_coinsText;

	public SpriteText m_gemsText;

	public UIScrollList m_productList;

	public GameObject m_productItemPrefab;

	public override void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		base.StartTransition(mode);
		if (mode == UIPanelManager.SHOW_MODE.BringInForward)
		{
			StartCoroutine("PopulateProductList");
		}
		UpdateCurrencies();
	}

	private void UpdateCurrencies()
	{
		m_gemsText.Text = SecureStorage.Instance.GetGems().ToString();
	}

	private IEnumerator PopulateProductList()
	{
		yield return null;
		m_productList.ClearList(true);
		BaseCampProducts.ProductData[] products = m_productDefinitions.products;
		foreach (BaseCampProducts.ProductData Product in products)
		{
			UIListItemContainer NewItem = m_productList.CreateItem(m_productItemPrefab) as UIListItemContainer;
			int ProdLevel = 0;
			NewItem.GetTextElement("Title").Text = Product.Title;
			NewItem.GetTextElement("GemsPrice").Text = Product.Levels[ProdLevel].GemPrice + " Gems";
			NewItem.GetTextElement("CoinsPrice").Text = Product.Levels[ProdLevel].CoinPrice + " Coins";
			NewItem.GetTextElement("Description").Text = Product.Levels[ProdLevel].Description;
			BaseCampProductButton ProdButton = NewItem.GetElement("Background") as BaseCampProductButton;
			ProdButton.m_productIdentifier = Product.Identifier;
			ProdButton.m_productLevel = ProdLevel;
		}
	}

	private void OnProductSelected()
	{
		BaseCampProductButton baseCampProductButton = m_productList.LastClickedControl as BaseCampProductButton;
		if (baseCampProductButton != null)
		{
			TBFUtils.DebugLog("Bought: " + baseCampProductButton.m_productIdentifier);
		}
	}
}
