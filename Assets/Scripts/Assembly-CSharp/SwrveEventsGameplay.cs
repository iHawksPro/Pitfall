using System.Collections.Generic;
using UnityEngine;

public class SwrveEventsGameplay
{
	public static void FirstRun()
	{
		Bedrock.AnalyticsLogEvent("General.FirstRun", "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "OSVersion", SwrvePayload.OSVersion, "Language", SwrvePayload.CurrentLanguage, false);
	}

	public static void GameStarted(int StartDistance)
	{
		Bedrock.AnalyticsLogEvent("Gameplay.Game.GameStarted", "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "Language", SwrvePayload.CurrentLanguage, "Distance", StartDistance.ToString(), false);
	}

	public static void TrialStarted(string strTrialID)
	{
		Bedrock.AnalyticsLogEvent("Gameplay.Trials.TrialStarted." + strTrialID, "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "Language", SwrvePayload.CurrentLanguage, false);
		Bedrock.AnalyticsLogEvent("Gameplay.Trials.TrialStartedAny", "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "Language", SwrvePayload.CurrentLanguage, false);
	}

	public static void GameEnded()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Distance"] = SwrvePayload.LastDistance;
		dictionary["XPGained"] = SwrvePayload.XPGained;
		dictionary["LastRunTime"] = SwrvePayload.LastRunTime;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.GameEnded", parameters, false);
		dictionary.Clear();
		dictionary["SnakesSeen"] = SwrvePayload.SnakesSeen;
		dictionary["SnakesKilledPercent"] = SwrvePayload.SnakesKilledPercent;
		dictionary["ScorpionsSeen"] = SwrvePayload.ScorpionsSeen;
		dictionary["ScorpionsKilledPercent"] = SwrvePayload.ScorpionsKilledPercent;
		dictionary["CrocsSeen"] = SwrvePayload.CrocsSeen;
		dictionary["CrocsKilledPercent"] = SwrvePayload.CrocsKilledPercent;
		dictionary["BarsCollected"] = SwrvePayload.BarsCollected;
		dictionary["BarsCollectPercent"] = SwrvePayload.BarsCollectedPercent;
		dictionary["TimesPoisoned"] = SwrvePayload.LastTimesPoisoned;
		parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.GameEndedEnemies", parameters, false);
		dictionary.Clear();
		dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
		dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
		dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
		dictionary["MacawStock"] = SwrvePayload.MacawStock;
		parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.GameEndedStock", parameters, false);
		dictionary.Clear();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["DeathType"] = SwrvePayload.LastDeathType;
		dictionary["TileSet"] = SwrvePayload.LastTileTheme;
		dictionary["PlayerOutfit"] = SwrvePayload.PlayerOutfit;
		dictionary["Audio"] = SwrvePayload.Audio;
		dictionary["OOTD"] = SwrvePayload.MatchesOOTD;
		parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.GameEndedPlayer", parameters, false);
	}

	public static void TrialFailed(string trialName, float timeInSeconds, float distance, int nBoostsUsed)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["LastRunTime"] = Mathf.CeilToInt(timeInSeconds).ToString();
		dictionary["BoostsUsed"] = nBoostsUsed.ToString();
		dictionary["Distance"] = Mathf.CeilToInt(distance).ToString();
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Progression.Trials.FailedAny", parameters, false);
		string name = "Progression.Trials.Failed." + trialName;
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void LifeTonicUsed()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["Distance"] = SwrvePayload.CurrentDistance;
		dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.LifeTonicUsed", parameters, false);
	}

	public static void HasteTonicUsed()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["Distance"] = SwrvePayload.CurrentDistance;
		dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.HasteTonicUsed", parameters, false);
	}

	public static void AntiVenomUsed()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["Distance"] = SwrvePayload.CurrentDistance;
		dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.AntiVenomUsed", parameters, false);
	}

	public static void TileSetChange(WorldConstructionHelper.Theme tileType)
	{
		Bedrock.AnalyticsLogEvent("Gameplay.Game.TilesetChange", "TileSet", tileType.ToString(), false);
	}
}
