using UnityEngine;

public class BaseCampOutfitsPanel : StorePanel
{
	public GameObject m_scrollItemPrefab;

	private void Awake()
	{
		PopulateProductList();
	}

	private void PopulateProductList()
	{
		m_scrollList.ClearList(true);
		int num = 0;
		GameObject gameObject = null;
		BaseCampProducts products = StoreProductManager.Instance.GetProducts(BaseCampProducts.StoreCategory.BCP_OUTFIT);
		BaseCampProducts.ProductData[] products2 = products.products;
		foreach (BaseCampProducts.ProductData productData in products2)
		{
			AddScrollPosition(productData.Identifier, num);
			gameObject = new GameObject("BtnContainer2");
			gameObject.AddComponent<UIListItemContainer>();
			GameObject gameObject2 = Object.Instantiate(m_scrollItemPrefab) as GameObject;
			OutfitItem component = gameObject2.GetComponent<OutfitItem>();
			if (component == null)
			{
				Debug.LogError("Couldn't get product item");
			}
			component.PopulateItem(productData);
			component.transform.localPosition = new Vector3(0f, 0f, 0f);
			component.transform.parent = gameObject.transform;
			m_scrollList.AddItem(gameObject);
			num++;
		}
	}
}
