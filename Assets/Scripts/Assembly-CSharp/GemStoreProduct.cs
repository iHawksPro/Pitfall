using System;
using System.Collections.Generic;
using UnityEngine;

public class GemStoreProduct : ScriptableObject
{
	[Serializable]
	public class GemStoreProductData
	{
		public string Identifier = string.Empty;

		public string Title = string.Empty;

		public string Description = string.Empty;

		public string Price = string.Empty;

		public string CurrencySymbol = string.Empty;

		public string CurrencyCode = string.Empty;

		public int AmountOfGems;

		public int PercentageOffAmount;

		public bool IsIAP = true;

		public bool IsPopular
		{
			get
			{
				bool result = false;
				if (SwrveServerVariables.Instance != null)
				{
					result = SwrveServerVariables.Instance.MostPopularExchange == Identifier;
				}
				return result;
			}
		}

		public GemStoreProductData()
		{
		}

		public GemStoreProductData(GemStoreProductData Other)
		{
			Identifier = Other.Identifier;
			Title = Other.Title;
			Description = Other.Description;
			Price = Other.Price;
			CurrencySymbol = Other.CurrencySymbol;
			AmountOfGems = Other.AmountOfGems;
			PercentageOffAmount = Other.PercentageOffAmount;
			IsIAP = Other.IsIAP;
		}

		public string GetTitle()
		{
			return string.Format(Language.Get(Title), AmountOfGems * (int)DailyDoubleController.Instance.DD_diamondsMultiplier);
		}

		public string GetDesc()
		{
			return string.Format(Language.Get(Description), AmountOfGems * (int)DailyDoubleController.Instance.DD_diamondsMultiplier);
		}

		public void UpdateFromBedrock(Dictionary<string, string> swrveResource)
		{
			Debug.Log("UpdateFromBedrock: " + Identifier);
			int result = 0;
			if (swrveResource.ContainsKey("diamonds") && int.TryParse(swrveResource["diamonds"], out result))
			{
				AmountOfGems = result;
				Debug.Log("UpdateFromBedrock AmountOfGems: " + AmountOfGems);
			}
			if (swrveResource.ContainsKey("percentoff") && int.TryParse(swrveResource["percentoff"], out result))
			{
				PercentageOffAmount = result;
				Debug.Log("UpdateFromBedrock PercentageOffAmount: " + PercentageOffAmount);
			}
		}
	}

	public GemStoreProductData[] products;

	public GemStoreProductData FindGemProduct(string Identifier)
	{
		GemStoreProductData result = null;
		for (int i = 0; i < products.Length; i++)
		{
			if (products[i].Identifier == Identifier)
			{
				result = products[i];
				break;
			}
		}
		return result;
	}
}
