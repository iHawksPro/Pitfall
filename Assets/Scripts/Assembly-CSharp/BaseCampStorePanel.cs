using UnityEngine;

public class BaseCampStorePanel : StorePanel
{
	private const int c_iRepositions = 20;

	public GameObject m_scrollItemPrefab;

	public GameObject m_PowerUpsTitlePrefab;

	public GameObject m_UpgradesTitlePrefab;

	public BaseCampProducts.StoreCategory m_productCategory;

	private int m_iRepositionsThisLife = 20;

	private void Awake()
	{
		PopulateProductList();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_iRepositionsThisLife = 20;
	}

	private void LateUpdate()
	{
		if (m_iRepositionsThisLife > 0)
		{
			m_iRepositionsThisLife--;
			m_scrollList.RepositionItems();
		}
	}

	private void PopulateProductList()
	{
		int row = 0;
		m_scrollList.ClearList(true);
		AddProducts(StoreProductManager.Instance.GetProducts(m_productCategory), ref row);
	}

	protected void AddProducts(BaseCampProducts ProductsToAdd, ref int row)
	{
		GameObject gameObject = null;
		BaseCampProducts.ProductData[] products = ProductsToAdd.products;
		foreach (BaseCampProducts.ProductData productData in products)
		{
			AddScrollPosition(productData.Identifier, row);
			gameObject = new GameObject("ProductContainer");
			gameObject.AddComponent<UIListItemContainer>();
			GameObject gameObject2 = Object.Instantiate(m_scrollItemPrefab) as GameObject;
			ConsumableItem component = gameObject2.GetComponent<ConsumableItem>();
			if (component == null)
			{
				Debug.LogError("Couldn't get product item");
			}
			component.PopulateItem(productData);
			component.transform.localPosition = new Vector3(0f, 0f, 0f);
			component.transform.parent = gameObject.transform;
			m_scrollList.AddItem(gameObject);
			row++;
		}
	}
}
