using System.Collections.Generic;
using UnityEngine;

public class SwrveEventsPurchase
{
	public static string LocalCurrencyCode = "USD";

	public static string PaymentProvider = "Google";

	public static void GemPack(string Identifier, ulong Cost, ulong VirtualCurrencyAmount, string currencyCode)
	{
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash("SoftBalance", SwrvePayload.CurrentCoinTotal, "HardBalance", SwrvePayload.CurrentGemTotal, "GemPack", Identifier);
		if (currencyCode == null || currencyCode == string.Empty)
		{
			currencyCode = LocalCurrencyCode;
		}
		Bedrock.AnalyticsLogRealPurchase(Cost, currencyCode, PaymentProvider, VirtualCurrencyAmount, "Gems", parameters);
	}

	public static void TreasureUpgrade(string Identifier, ulong Cost, string currencyCode)
	{
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash("SoftBalance", SwrvePayload.CurrentCoinTotal, "HardBalance", SwrvePayload.CurrentGemTotal, "TreasureUpgrade", Identifier);
		if (currencyCode == null || currencyCode == string.Empty)
		{
			currencyCode = LocalCurrencyCode;
		}
		Bedrock.AnalyticsLogRealPurchase(Cost, currencyCode, PaymentProvider, 0uL, "Gems", parameters);
	}

	public static void StoreCoinPurchase(string Item, ulong Cost)
	{
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash("SoftBalance", SwrvePayload.CurrentCoinTotal, "HardBalance", SwrvePayload.CurrentGemTotal);
		Bedrock.AnalyticsLogVirtualPurchase(Item, Cost, 1uL, "Coins", parameters);
		OnProductPurchase(Item, false);
	}

	public static void StoreGemPurchase(string Item, ulong Cost)
	{
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash("SoftBalance", SwrvePayload.CurrentCoinTotal, "HardBalance", SwrvePayload.CurrentGemTotal);
		Bedrock.AnalyticsLogVirtualPurchase(Item, Cost, 1uL, "Gems", parameters);
		OnProductPurchase(Item, true);
	}

	private static void OnProductPurchase(string Item, bool UseGems)
	{
		BaseCampProducts.ProductData productData = StoreProductManager.Instance.FindProduct(Item);
		if (productData == null)
		{
			return;
		}
		switch (productData.Category)
		{
		case BaseCampProducts.StoreCategory.BCP_OUTFIT:
			SecureStorage.Instance.IncreaseCostumesBought(1);
			PurchaseOutfit(Item, UseGems);
			break;
		case BaseCampProducts.StoreCategory.BCP_CHECKPOINT:
			SecureStorage.Instance.IncreaseCheckPointsBought(1);
			if (SecureStorage.Instance.GetCheckPointsBought() >= CheckPointController.Instance().GetTotalPurchasableCheckPoints())
			{
				PurchaseAllCheckPoints();
			}
			PurchaseCheckPoint(Item);
			break;
		case BaseCampProducts.StoreCategory.BCP_CONSUMABLE:
			PurchasePowerup(Item, UseGems);
			break;
		case BaseCampProducts.StoreCategory.BCP_UPGRADE:
			PurchaseUpgrade(Item, UseGems);
			break;
		}
	}

	public static void ProductViewed(string Item, int Level)
	{
		BaseCampProducts.ProductData productData = StoreProductManager.Instance.FindProduct(Item);
		if (productData != null)
		{
			switch (productData.Category)
			{
			case BaseCampProducts.StoreCategory.BCP_OUTFIT:
				ViewedOutfit(Item);
				break;
			case BaseCampProducts.StoreCategory.BCP_CHECKPOINT:
				break;
			case BaseCampProducts.StoreCategory.BCP_CONSUMABLE:
				ViewedPowerup(Item);
				break;
			case BaseCampProducts.StoreCategory.BCP_UPGRADE:
				ViewedUpgrade(Item);
				break;
			}
		}
	}

	public static void ProductClosed(string Item, int Level)
	{
		BaseCampProducts.ProductData productData = StoreProductManager.Instance.FindProduct(Item);
		if (productData != null)
		{
			switch (productData.Category)
			{
			case BaseCampProducts.StoreCategory.BCP_OUTFIT:
				ClosedOutfit(Item);
				break;
			case BaseCampProducts.StoreCategory.BCP_CHECKPOINT:
				break;
			case BaseCampProducts.StoreCategory.BCP_CONSUMABLE:
				ClosedPowerup(Item);
				break;
			case BaseCampProducts.StoreCategory.BCP_UPGRADE:
				ClosedUpgrade(Item, Level);
				break;
			}
		}
	}

	public static void PurchasePowerup(string Identifier, bool UseGems)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["BoughtWith"] = ((!UseGems) ? "Coins" : "Gems");
		string text = "Purchasing.Powerups.Purchase";
		switch (Identifier)
		{
		case "consumable.macaw":
			text += "Macaw";
			dictionary["MacawStock"] = SwrvePayload.MacawStock;
			break;
		case "consumable.jungle":
			text += "LifeTonic";
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			dictionary["LifeTonicUpgradeLevel"] = SwrvePayload.LifeTonicUpgradeLevel;
			break;
		case "consumable.speedincrease":
			text += "HasteTonic";
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			dictionary["HasteTonicUpgradeLevel"] = SwrvePayload.HasteTonicUpgradeLevel;
			break;
		case "consumable.antivenom":
			text += "AntiVenom";
			dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
			dictionary["PoisonUpgradeLevel"] = SwrvePayload.PoisonUpgradeLevel;
			break;
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void ViewedPowerup(string Identifier)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		string text = "Purchasing.Powerups.View";
		switch (Identifier)
		{
		case "consumable.macaw":
			text += "Macaw";
			dictionary["MacawStock"] = SwrvePayload.MacawStock;
			break;
		case "consumable.jungle":
			text += "LifeTonic";
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			dictionary["LifeTonicUpgradeLevel"] = SwrvePayload.LifeTonicUpgradeLevel;
			break;
		case "consumable.speedincrease":
			text += "HasteTonic";
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			dictionary["HasteTonicUpgradeLevel"] = SwrvePayload.HasteTonicUpgradeLevel;
			break;
		case "consumable.antivenom":
			text += "AntiVenom";
			dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
			break;
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void ClosedPowerup(string Identifier)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		string text = "Purchasing.Powerups.Close";
		switch (Identifier)
		{
		case "consumable.macaw":
			text += "Macaw";
			dictionary["MacawStock"] = SwrvePayload.MacawStock;
			break;
		case "consumable.jungle":
			text += "LifeTonic";
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			dictionary["LifeTonicUpgradeLevel"] = SwrvePayload.LifeTonicUpgradeLevel;
			break;
		case "consumable.speedincrease":
			text += "HasteTonic";
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			dictionary["HasteTonicUpgradeLevel"] = SwrvePayload.HasteTonicUpgradeLevel;
			break;
		case "consumable.antivenom":
			text += "AntiVenom";
			dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
			break;
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void PurchaseUpgrade(string Identifier, bool UseGems)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["BoughtWith"] = ((!UseGems) ? "Coins" : "Gems");
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		string text = "Purchasing.Upgrades.Purchase";
		switch (Identifier)
		{
		case "upgrade.jaguar":
			text += "JaguarUpgrade";
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.PurchaseAnyJaguarUpgrade", "JaguarUpgradeLevel", SwrvePayload.JaguarUpgradeLevel, "BoughtWith", (!UseGems) ? "Coins" : "Gems", false);
			break;
		case "upgrade.jungle":
			text += "LifeTonicUpgrade";
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.PurchaseAnyLifeTonicUpgrade", "LifeTonicUpgradeLevel", SwrvePayload.LifeTonicUpgradeLevel, "BoughtWith", (!UseGems) ? "Coins" : "Gems", false);
			break;
		case "upgrade.speedincrease":
			text += "HasteTonicUpgrade";
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.PurchaseAnyHasteTonicUpgrade", "HasteTonicUpgradeLevel", SwrvePayload.HasteTonicUpgradeLevel, "BoughtWith", (!UseGems) ? "Coins" : "Gems", false);
			break;
		case "upgrade.poison":
			text += "PoisonUpgrade";
			dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.PurchaseAnyPoisonUpgrade", "AntiVenonStock", SwrvePayload.AntiVenomStock, "PoisonUpgradeLevel", SwrvePayload.PoisonUpgradeLevel, "BoughtWith", (!UseGems) ? "Coins" : "Gems", false);
			break;
		}
		text += SecureStorage.Instance.GetItemCount(Identifier);
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void ViewedUpgrade(string Identifier)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		string text = "Purchasing.Upgrades.View";
		switch (Identifier)
		{
		case "upgrade.jaguar":
			text += "JaguarUpgrade";
			dictionary["JaguarUpgradeLevel"] = SwrvePayload.JaguarUpgradeLevel;
			break;
		case "upgrade.jungle":
			text += "LifeTonicUpgrade";
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			dictionary["LifeTonicUpgradeLevel"] = SwrvePayload.LifeTonicUpgradeLevel;
			break;
		case "upgrade.speedincrease":
			text += "HasteTonicUpgrade";
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			dictionary["HasteTonicUpgradeLevel"] = SwrvePayload.HasteTonicUpgradeLevel;
			break;
		case "upgrade.poison":
			text += "PoisonUpgrade";
			dictionary["PoisonUpgradeLevel"] = SwrvePayload.PoisonUpgradeLevel;
			break;
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void ClosedUpgrade(string Identifier, int Level)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		Level = Mathf.Clamp(Level + 1, 1, 3);
		string text = "Purchasing.Upgrades.Close";
		switch (Identifier)
		{
		case "upgrade.jaguar":
			text = text + "JaguarUpgrade" + Level;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.CloseAnyJaguarUpgrade", "JaguarUpgradeLevel", SwrvePayload.JaguarUpgradeLevel, false);
			break;
		case "upgrade.jungle":
			text = text + "LifeTonicUpgrade" + Level;
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.CloseAnyLifeTonicUpgrade", "LifeTonicUpgradeLevel", SwrvePayload.LifeTonicUpgradeLevel, false);
			break;
		case "upgrade.speedincrease":
			text = text + "HasteTonicUpgrade" + Level;
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.CloseAnyHasteTonicUpgrade", "HasteTonicUpgradeLevel", SwrvePayload.HasteTonicUpgradeLevel, false);
			break;
		case "upgrade.poison":
			text = text + "PoisonUpgrade" + Level;
			Bedrock.AnalyticsLogEvent("Purchasing.Upgrades.CloseAnyPoisonUpgrade", "PoisonUpgradeLevel", SwrvePayload.PoisonUpgradeLevel, false);
			break;
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(text, parameters, false);
	}

	public static void PurchaseOutfit(string Identifier, bool UseGems)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["CostumesBought"] = SwrvePayload.CostumesBought;
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["BoughtWith"] = ((!UseGems) ? "Coins" : "Gems");
		dictionary["PlayerOutfit"] = Identifier;
		dictionary["OOTD"] = SwrvePayload.CostumeMatchesOOTD(SecureStorage.Instance.TranslateCostumeType(Identifier));
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Purchasing.Outfits.PurchaseOutfit", parameters, false);
	}

	public static void ViewedOutfit(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Outfits.ViewOutfit", "PlayerOutfit", Identifier, false);
	}

	public static void ClosedOutfit(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Outfits.CloseOutfit", "PlayerOutfit", Identifier, false);
	}

	public static void PurchaseCheckPoint(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.CheckPoint.PurchaseCheckPoint", "CheckPoint", Identifier, "MacawStock", SwrvePayload.MacawStock, "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void PurchaseAllCheckPoints()
	{
		Bedrock.AnalyticsLogEvent("Purchasing.CheckPoint.PurchaseAllCheckPoints", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	private static void ExchangePurchaseResult(string Identifier, bool Success)
	{
		string text = null;
		string text2 = null;
		switch (Identifier)
		{
		case "gems.bundle1":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack1Failed" : "Purchasing.Exchange.PurchaseHardPack1");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle2":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack2Failed" : "Purchasing.Exchange.PurchaseHardPack2");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle3":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack3Failed" : "Purchasing.Exchange.PurchaseHardPack3");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle4":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack4Failed" : "Purchasing.Exchange.PurchaseHardPack4");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle1a":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack1aFailed" : "Purchasing.Exchange.PurchaseHardPack1a");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle3a":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack3aFailed" : "Purchasing.Exchange.PurchaseHardPack3a");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle4a":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack4aFailed" : "Purchasing.Exchange.PurchaseHardPack4a");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle4b":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack4bFailed" : "Purchasing.Exchange.PurchaseHardPack4a");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle5":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack5Failed" : "Purchasing.Exchange.PurchaseHardPack5");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "gems.bundle5a":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseHardPack5aFailed" : "Purchasing.Exchange.PurchaseHardPack5a");
			text2 = ((!Success) ? "Purchasing.Exchange.HardPackAnyFailed" : "Purchasing.Exchange.HardPackAny");
			break;
		case "treasure.upgrade":
		case "treasure.permanentupgrade":
			text = ((!Success) ? "Purchasing.Exchange.PurchaseTreasureMultFailed" : "Purchasing.Exchange.PurchaseTreasureMult");
			break;
		}
		if (text != null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
			dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
			dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
			dictionary["TotalMTX"] = SwrvePayload.TotalMTX;
			Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
			Bedrock.AnalyticsLogEvent(text, parameters, false);
		}
		if (text2 != null)
		{
			Bedrock.AnalyticsLogEvent(text2, "PlayerLevel", SwrvePayload.PlayerLevel, false);
		}
	}

	public static void ExchangePurchaseSuccess(string Identifier)
	{
		ExchangePurchaseResult(Identifier, true);
	}

	public static void ExchangePurchaseFailed(string Identifier)
	{
		ExchangePurchaseResult(Identifier, false);
	}

	public static void ReceiptVerifySuccess(string productId)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Receipt.Verified", "Product", productId, false);
	}

	public static void ReceiptVerifyFailed(string productId, int iTunesError)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Receipt.Failed", "Product", productId, "iTunesError", "_" + iTunesError, false);
	}

	public static void ReceiptVerifyServerError()
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Receipt.Error");
	}

	public static void TrialsBoostRefilled(bool UseGems)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["BoughtWith"] = ((!UseGems) ? "Coins" : "Gems");
		dictionary["BoostTonicStock"] = SwrvePayload.TrialsBoostTonicStock;
		dictionary["RefillTime"] = SwrvePayload.TrialsBoostRefillTime;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Purchasing.Trials.BoostRefill", parameters, false);
	}

	public static void TapjoyLaunchedOfferWall()
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Exchange.Tapjoy.LaunchedOfferWall");
	}

	public static void TapjoyReceivedAward(int nGems)
	{
		Bedrock.AnalyticsLogEvent("Purchasing.Exchange.Tapjoy.ReceivedAward", "HardBalance", "_" + nGems, false);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["AwardReason"] = "Tapjoy";
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogVirtualCurrencyAwarded((ulong)nGems, "Gems", parameters);
	}
}
