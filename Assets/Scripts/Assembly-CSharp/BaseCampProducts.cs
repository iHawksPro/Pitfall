using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCampProducts : ScriptableObject
{
	public enum StoreCategory
	{
		BCP_CONSUMABLE = 0,
		BCP_UPGRADE = 1,
		BCP_OUTFIT = 2,
		BCP_CHECKPOINT = 3,
		BCP_TRIALS = 4
	}

	[Serializable]
	public class ProductLevel
	{
		public string Description = string.Empty;

		public int CoinPrice;

		public int GemPrice;

		public int Quantity = 1;

		public string GetDesc()
		{
			return Language.Get(Description);
		}
	}

	[Serializable]
	public class ProductData
	{
		public string Identifier = string.Empty;

		public string AssociatedUpgradeID = string.Empty;

		public StoreCategory Category;

		public string Title = string.Empty;

		public ProductLevel[] Levels;

		public int LevelUnlock;

		public int Discount;

		public bool IsPopular
		{
			get
			{
				bool result = false;
				if (SwrveServerVariables.Instance != null)
				{
					switch (Category)
					{
					case StoreCategory.BCP_CONSUMABLE:
						result = SwrveServerVariables.Instance.MostPopularPowerup == Identifier;
						break;
					case StoreCategory.BCP_OUTFIT:
						result = SwrveServerVariables.Instance.MostPopularOutfit == Identifier;
						break;
					case StoreCategory.BCP_UPGRADE:
						result = SwrveServerVariables.Instance.MostPopularUpgrade == Identifier;
						break;
					}
				}
				return result;
			}
		}

		public void CopyTo(ProductData toProd)
		{
			toProd.Identifier = Identifier;
			toProd.AssociatedUpgradeID = AssociatedUpgradeID;
			toProd.Category = Category;
			toProd.Title = Title;
			toProd.LevelUnlock = LevelUnlock;
			toProd.Levels = new ProductLevel[Levels.Length];
			for (int i = 0; i < Levels.Length; i++)
			{
				toProd.Levels[i] = new ProductLevel();
				toProd.Levels[i].Description = Levels[i].Description;
				toProd.Levels[i].CoinPrice = Levels[i].CoinPrice;
				toProd.Levels[i].GemPrice = Levels[i].GemPrice;
				toProd.Levels[i].Quantity = Levels[i].Quantity;
			}
		}

		public string GetTitle()
		{
			return Language.Get(Title);
		}

		public int MaxNumberPlayerCanBuy()
		{
			switch (Category)
			{
			case StoreCategory.BCP_CONSUMABLE:
				return int.MaxValue;
			case StoreCategory.BCP_UPGRADE:
				return Levels.Length;
			case StoreCategory.BCP_OUTFIT:
			case StoreCategory.BCP_CHECKPOINT:
				return 1;
			case StoreCategory.BCP_TRIALS:
				return 0;
			default:
				Debug.LogError("Invalid product category");
				return -1;
			}
		}

		public int CoinPrice(int Level)
		{
			switch (Category)
			{
			case StoreCategory.BCP_CONSUMABLE:
			case StoreCategory.BCP_OUTFIT:
			case StoreCategory.BCP_CHECKPOINT:
			case StoreCategory.BCP_TRIALS:
				return Levels[0].CoinPrice;
			case StoreCategory.BCP_UPGRADE:
				return Levels[Level].CoinPrice;
			default:
				Debug.LogError("Invalid product category");
				return -1;
			}
		}

		public int Quantity(int level)
		{
			if (Category == StoreCategory.BCP_CONSUMABLE)
			{
				return Levels[level].Quantity;
			}
			return 1;
		}

		public int GemsPrice(int Level)
		{
			switch (Category)
			{
			case StoreCategory.BCP_CONSUMABLE:
			case StoreCategory.BCP_OUTFIT:
			case StoreCategory.BCP_CHECKPOINT:
			case StoreCategory.BCP_TRIALS:
				return Levels[0].GemPrice;
			case StoreCategory.BCP_UPGRADE:
				return Levels[Level].GemPrice;
			default:
				Debug.LogError("Invalid product category");
				return -1;
			}
		}

		public bool CanPlayerAffordItem(bool gems, int level)
		{
			int num;
			int num2;
			if (gems)
			{
				num = GemsPrice(level);
				num2 = SecureStorage.Instance.GetGems();
			}
			else
			{
				num = CoinPrice(level);
				num2 = SecureStorage.Instance.GetCoins();
			}
			return num <= num2;
		}

		public bool HasZeroValue(int level)
		{
			return CoinPrice(level) == 0 && GemsPrice(level) == 0;
		}

		public void UpdateFromBedrock(Dictionary<string, string> swrveResource)
		{
			int result;
			if (swrveResource.ContainsKey("cost_diamonds") && int.TryParse(swrveResource["cost_diamonds"], out result))
			{
				Levels[0].GemPrice = result;
			}
			if (swrveResource.ContainsKey("cost_treasure") && int.TryParse(swrveResource["cost_treasure"], out result))
			{
				Levels[0].CoinPrice = result;
			}
			if (swrveResource.ContainsKey("quantity") && int.TryParse(swrveResource["quantity"], out result))
			{
				Levels[0].Quantity = result;
			}
			if (swrveResource.ContainsKey("level1_cost_diamonds") && int.TryParse(swrveResource["level1_cost_diamonds"], out result))
			{
				Levels[0].GemPrice = result;
			}
			if (swrveResource.ContainsKey("level1_cost_treasure") && int.TryParse(swrveResource["level1_cost_treasure"], out result))
			{
				Levels[0].CoinPrice = result;
			}
			if (swrveResource.ContainsKey("level2_cost_diamonds") && int.TryParse(swrveResource["level2_cost_diamonds"], out result))
			{
				Levels[1].GemPrice = result;
			}
			if (swrveResource.ContainsKey("level2_cost_treasure") && int.TryParse(swrveResource["level2_cost_treasure"], out result))
			{
				Levels[1].CoinPrice = result;
			}
			if (swrveResource.ContainsKey("level3_cost_diamonds") && int.TryParse(swrveResource["level3_cost_diamonds"], out result))
			{
				Levels[2].GemPrice = result;
			}
			if (swrveResource.ContainsKey("level3_cost_treasure") && int.TryParse(swrveResource["level3_cost_treasure"], out result))
			{
				Levels[2].CoinPrice = result;
			}
		}
	}

	public ProductData[] products;

	public int CountProductsAvailableToBuy()
	{
		int num = 0;
		int gems = SecureStorage.Instance.GetGems();
		int coins = SecureStorage.Instance.GetCoins();
		ProductData[] array = products;
		foreach (ProductData productData in array)
		{
			int itemCount = SecureStorage.Instance.GetItemCount(productData.Identifier);
			if (itemCount >= productData.MaxNumberPlayerCanBuy())
			{
				continue;
			}
			int level = 0;
			if (productData.Category == StoreCategory.BCP_UPGRADE)
			{
				level = Mathf.Clamp(itemCount, 0, productData.Levels.Length - 1);
			}
			bool flag = false;
			if (productData.CoinPrice(level) > 0)
			{
				flag = coins >= productData.CoinPrice(level);
			}
			if (productData.GemsPrice(level) > 0)
			{
				flag = flag || gems >= productData.GemsPrice(level);
			}
			if (productData.Category == StoreCategory.BCP_CHECKPOINT)
			{
				string text = "checkpoint.";
				int length = text.Length;
				string value = productData.Identifier.Substring(length, productData.Identifier.Length - length);
				int num2 = Convert.ToInt32(value);
				CheckPointController checkPointController = CheckPointController.Instance();
				if (checkPointController != null)
				{
					CheckPointController.CHECKPOINT_TYPE checkPointTypeAt = checkPointController.GetCheckPointTypeAt(num2);
					if (checkPointTypeAt != CheckPointController.CHECKPOINT_TYPE.PURCHASABLE)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				num++;
			}
		}
		return num;
	}
}
