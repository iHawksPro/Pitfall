using UnityEngine;

public class SwrvePayload
{
	public static string LastEvent;

	public static string CurrentGemTotal
	{
		get
		{
			return SecureStorage.Instance.GetGems().ToString();
		}
	}

	public static string CurrentCoinTotal
	{
		get
		{
			return SecureStorage.Instance.GetCoins().ToString();
		}
	}

	public static string TotalCoinsCollected
	{
		get
		{
			return SecureStorage.Instance.GetTotalCoins().ToString();
		}
	}

	public static string TotalGemsCollected
	{
		get
		{
			return SecureStorage.Instance.GetTotalGems().ToString();
		}
	}

	public static string TotalCoinsSpent
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.treasurespent").ToString();
		}
	}

	public static string TotalGemsSpent
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.gemsspent").ToString();
		}
	}

	public static string TotalMTX
	{
		get
		{
			return SecureStorage.Instance.GetTotalMTX().ToString();
		}
	}

	public static string CostumesBought
	{
		get
		{
			return SecureStorage.Instance.GetCostumesBought().ToString();
		}
	}

	public static string CheckPointsBought
	{
		get
		{
			return SecureStorage.Instance.GetCheckPointsBought().ToString();
		}
	}

	public static string PlayerLevel
	{
		get
		{
			return (SecureStorage.Instance.GetXPLevel() + SecureStorage.Instance.GetXPDistinction() * 50 + 1).ToString();
		}
	}

	public static string PlayerTimesPrestiged
	{
		get
		{
			return SecureStorage.Instance.GetXPDistinction().ToString();
		}
	}

	public static string GamesPlayed
	{
		get
		{
			return SecureStorage.Instance.GamesPlayed.ToString();
		}
	}

	public static string CurrentDistance
	{
		get
		{
			int num = 0;
			if (PlayerController.Instance() != null)
			{
				num = (int)PlayerController.Instance().Score().DistanceTravelled();
			}
			return num.ToString();
		}
	}

	public static string CurrentScore
	{
		get
		{
			int num = 0;
			if (PlayerController.Instance() != null)
			{
				num = PlayerController.Instance().Score().TotalScoreXPAdjusted();
			}
			return num.ToString();
		}
	}

	public static string LastDistance
	{
		get
		{
			return SecureStorage.Instance.LastDistanceTravelled.ToString();
		}
	}

	public static string LastScore
	{
		get
		{
			return SecureStorage.Instance.LastScore.ToString();
		}
	}

	public static string JaguarUpgradeLevel
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("upgrade.jaguar").ToString();
		}
	}

	public static string LifeTonicUpgradeLevel
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("upgrade.jungle").ToString();
		}
	}

	public static string PoisonUpgradeLevel
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("upgrade.poison").ToString();
		}
	}

	public static string HasteTonicUpgradeLevel
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("upgrade.speedincrease").ToString();
		}
	}

	public static string MacawStock
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("consumable.macaw").ToString();
		}
	}

	public static string TotalMacawsUsed
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.prettyboy").ToString();
		}
	}

	public static string LifeTonicStock
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("consumable.jungle").ToString();
		}
	}

	public static string TotalLifeTonicUsed
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.lazarus").ToString();
		}
	}

	public static string AntiVenomStock
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("consumable.antivenom").ToString();
		}
	}

	public static string TotalAntiVenomUsed
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.antivenom").ToString();
		}
	}

	public static string HasteTonicStock
	{
		get
		{
			return SecureStorage.Instance.GetItemCount("consumable.speedincrease").ToString();
		}
	}

	public static string TotalHasteTonicUsed
	{
		get
		{
			return SecureStorage.Instance.GetInt("player.accum.haste").ToString();
		}
	}

	public static string LastDeathType
	{
		get
		{
			return SecureStorage.Instance.LastDeathType.ToString();
		}
	}

	public static string LastTileTheme
	{
		get
		{
			return SecureStorage.Instance.LastTileTheme.ToString();
		}
	}

	public static string LastTimesPoisoned
	{
		get
		{
			return SecureStorage.Instance.LastTimesPoisoned.ToString();
		}
	}

	public static string PlayerOutfit
	{
		get
		{
			return SecureStorage.Instance.GetCurrentCostume();
		}
	}

	public static string MatchesOOTD
	{
		get
		{
			return OutfitOfTheDayManager.Instance.IsOOTD(SecureStorage.Instance.GetCurrentCostumeType()).ToString();
		}
	}

	public static string LastRunTime
	{
		get
		{
			return SecureStorage.Instance.LastGameTime.ToString();
		}
	}

	public static string SnakesSeen
	{
		get
		{
			return nBaddiesSeen(BaddieController.Type.Snake).ToString();
		}
	}

	public static string SnakesKilled
	{
		get
		{
			return nBaddiesKilled(BaddieController.Type.Snake).ToString();
		}
	}

	public static string SnakesKilledPercent
	{
		get
		{
			return nBaddiesKilledPercent(BaddieController.Type.Snake).ToString();
		}
	}

	public static string CrocsSeen
	{
		get
		{
			return nBaddiesSeen(BaddieController.Type.Crocodile).ToString();
		}
	}

	public static string CrocsKilled
	{
		get
		{
			return nBaddiesKilled(BaddieController.Type.Crocodile).ToString();
		}
	}

	public static string CrocsKilledPercent
	{
		get
		{
			return nBaddiesKilledPercent(BaddieController.Type.Crocodile).ToString();
		}
	}

	public static string SpidersSeen
	{
		get
		{
			return nBaddiesSeen(BaddieController.Type.Spider).ToString();
		}
	}

	public static string SpidersKilled
	{
		get
		{
			return nBaddiesKilled(BaddieController.Type.Spider).ToString();
		}
	}

	public static string SpidersKilledPercent
	{
		get
		{
			return nBaddiesKilledPercent(BaddieController.Type.Spider).ToString();
		}
	}

	public static string ScorpionsSeen
	{
		get
		{
			return nBaddiesSeen(BaddieController.Type.Scorpion).ToString();
		}
	}

	public static string ScorpionsKilled
	{
		get
		{
			return nBaddiesKilled(BaddieController.Type.Scorpion).ToString();
		}
	}

	public static string ScorpionsKilledPercent
	{
		get
		{
			return nBaddiesKilledPercent(BaddieController.Type.Scorpion).ToString();
		}
	}

	public static string BarsCollected
	{
		get
		{
			return nCoinsCollected().ToString();
		}
	}

	public static string BarsCollectedPercent
	{
		get
		{
			float num = nCoinsCollected();
			float num2 = nCoinsSpawned();
			float num3 = 0f;
			if (num2 > 0f)
			{
				num3 = num / num2;
			}
			return num3.ToString("P0");
		}
	}

	public static string XPGained
	{
		get
		{
			return SecureStorage.Instance.LastXPGained.ToString();
		}
	}

	public static string Audio
	{
		get
		{
			if (TBFUtils.IsMusicPlaying)
			{
				return "Playing Custom Music";
			}
			float volumeLevel = TBFUtils.VolumeLevel;
			if (volumeLevel == 0f)
			{
				return "Mute";
			}
			return "Game Audio";
		}
	}

	public static string Device
	{
		get
		{
			return TBFUtils.iPhoneGen;
		}
	}

	public static string DeviceManufacturer
	{
		get
		{
			return TBFUtils.DeviceManfacturer;
		}
	}

	public static string DeviceModel
	{
		get
		{
			return TBFUtils.DeviceModel;
		}
	}

	public static string OSVersion
	{
		get
		{
			return TBFUtils.OSVersion;
		}
	}

	public static string CurrentLanguage
	{
		get
		{
			return Language.CurrentLanguage().ToString();
		}
	}

	public static string Version
	{
		get
		{
			return StartGameSettings.Instance.VERSION;
		}
	}

	public static string IsCheater
	{
		get
		{
			bool isCracked = TBFUtils.IsCracked;
			bool hasFailedReceiptVerification = SecureStorage.Instance.HasFailedReceiptVerification;
			return (isCracked || hasFailedReceiptVerification).ToString();
		}
	}

	public static string TrialsBoostTonicStock
	{
		get
		{
			return TrialsDataManager.Instance.NumBoostsAvailable.ToString();
		}
	}

	public static string TrialsBoostRefillTime
	{
		get
		{
			uint timeUntilNextBoostRefill = TrialsDataManager.Instance.TimeUntilNextBoostRefill;
			float f = (float)timeUntilNextBoostRefill / 60f;
			return Mathf.FloorToInt(f).ToString();
		}
	}

	public static string CostumeMatchesOOTD(Costume testCostume)
	{
		return OutfitOfTheDayManager.Instance.IsOOTD(testCostume).ToString();
	}

	private static int nBaddiesSeen(BaddieController.Type baddieType)
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = PlayerController.Instance().Score().BaddiesSeen(baddieType);
		}
		return result;
	}

	private static int nBaddiesKilled(BaddieController.Type baddieType)
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = PlayerController.Instance().Score().BaddiesKilled(baddieType);
		}
		return result;
	}

	private static int nBaddiesKilledPercent(BaddieController.Type baddieType)
	{
		int result = 0;
		if (PlayerController.Instance() != null && nBaddiesSeen(baddieType) != 0)
		{
			result = PlayerController.Instance().Score().BaddiesKilled(baddieType) / nBaddiesSeen(baddieType) * 100;
		}
		return result;
	}

	private static int nCoinsCollected()
	{
		int num = 0;
		if (PlayerController.Instance() != null)
		{
			num = PlayerController.Instance().Score().CoinsCollected();
			if (SecureStorage.Instance.GetItemCount(StoreProductManager.TreasureUpgradeIdentifier) > 0)
			{
				num /= 2;
			}
		}
		return num;
	}

	private static int nCoinsSpawned()
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = PlayerController.Instance().Score().CoinsSpawned();
		}
		return result;
	}
}
