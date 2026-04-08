using System.Collections.Generic;

public class SwrveUserData
{
	public static void UploadAllAttributes()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Device"] = SwrvePayload.Device;
		dictionary["AppVersion"] = SwrvePayload.Version;
		dictionary["OsVersion"] = SwrvePayload.OSVersion;
		dictionary["Language"] = SwrvePayload.CurrentLanguage;
		dictionary["FirstGameDateTime"] = SecureStorage.Instance.FirstGameDateTime;
		dictionary["LastPlayerDateTime"] = SecureStorage.Instance.LastPlayedDateTime;
		dictionary["TotalTreasureCollected"] = SwrvePayload.TotalCoinsCollected;
		dictionary["TotalTreasureSpent"] = SwrvePayload.TotalCoinsSpent;
		dictionary["TotalDiamondsCollected"] = SwrvePayload.TotalGemsCollected;
		dictionary["TotalDiamondsSpent"] = SwrvePayload.TotalGemsSpent;
		dictionary["CostumesBought"] = SwrvePayload.CostumesBought;
		dictionary["GamesPlayed"] = SwrvePayload.GamesPlayed;
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["IsCheater"] = SwrvePayload.IsCheater;
		dictionary["MacawsUsed"] = SwrvePayload.TotalMacawsUsed;
		dictionary["AntiVenomUsed"] = SwrvePayload.TotalAntiVenomUsed;
		dictionary["HasteTonicUsed"] = SwrvePayload.TotalHasteTonicUsed;
		dictionary["LifeTonicUsed"] = SwrvePayload.TotalLifeTonicUsed;
		dictionary["TotalCheckpointsBought"] = SwrvePayload.CheckPointsBought;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsSetCustomUserInformation(parameters);
	}
}
