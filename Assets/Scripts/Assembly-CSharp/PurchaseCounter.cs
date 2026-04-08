using UnityEngine;

public class PurchaseCounter : MonoBehaviour
{
	public SpriteText m_counter;

	public SpriteRoot m_badge;

	private int m_cachedCoins = -1;

	private int m_cachedGems = -1;

	private void OnEnable()
	{
		SecureStorage.playerCoinsChanged += UpdateProductCount;
		SecureStorage.playerGemsChanged += UpdateProductCount;
		m_cachedCoins = -1;
		m_cachedGems = -1;
		UpdateProductCount(0);
	}

	private void OnDisable()
	{
		SecureStorage.playerCoinsChanged -= UpdateProductCount;
		SecureStorage.playerGemsChanged -= UpdateProductCount;
	}

	private void UpdateProductCount(int Dummy)
	{
		int coins = SecureStorage.Instance.GetCoins();
		int gems = SecureStorage.Instance.GetGems();
		if (coins != m_cachedCoins || gems != m_cachedGems)
		{
			int num = CountProductsAvailableToBuy();
			if (num > 0)
			{
				m_badge.gameObject.SetActiveRecursively(true);
				m_counter.gameObject.SetActiveRecursively(true);
				m_counter.Text = num.ToString();
			}
			else
			{
				m_badge.gameObject.SetActiveRecursively(false);
				m_counter.gameObject.SetActiveRecursively(false);
			}
			m_cachedCoins = coins;
			m_cachedGems = gems;
		}
	}

	public int CountProductsAvailableToBuy()
	{
		int num = 0;
		if (StoreProductManager.Instance != null)
		{
			BaseCampProducts[] allProducts = StoreProductManager.Instance.GetAllProducts();
			if (allProducts != null)
			{
				for (int i = 0; i < allProducts.Length; i++)
				{
					num += allProducts[i].CountProductsAvailableToBuy();
				}
			}
		}
		return num;
	}
}
