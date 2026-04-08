using System.Collections.Generic;
using UnityEngine;

public class SwrveEventsProgression
{
	public static void AchievementCompleted(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("Progression.Achievements.AchievementAwarded", "Achievement", Identifier, false);
	}

	public static void PlayerLevelUp()
	{
		Bedrock.AnalyticsLogEvent("Progression.Player.Levelup", "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void PlayerPrestiged()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["TimesPrestiged"] = SwrvePayload.PlayerTimesPrestiged;
		dictionary["TotalMTX"] = SwrvePayload.TotalMTX;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Progression.Player.PlayerPrestiged", parameters, false);
	}

	public static void PlayerLevelMilestone(int Level)
	{
		if (Level <= 50)
		{
			string text = "Progression.LevelDetail.Level";
			text += Level;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
			dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
			dictionary["GamesPlayed"] = SwrvePayload.GamesPlayed;
			dictionary["LifeTonicStock"] = SwrvePayload.LifeTonicStock;
			dictionary["HasteTonicStock"] = SwrvePayload.HasteTonicStock;
			dictionary["AntiVenonStock"] = SwrvePayload.AntiVenomStock;
			dictionary["MacawStock"] = SwrvePayload.MacawStock;
			dictionary["JaguarUpgradeLevel"] = SwrvePayload.JaguarUpgradeLevel;
			dictionary["LifeTonicUpgradeLevel"] = SwrvePayload.LifeTonicUpgradeLevel;
			dictionary["PoisonUpgradeLevel"] = SwrvePayload.PoisonUpgradeLevel;
			dictionary["HasteTonicUpgradeLevel"] = SwrvePayload.HasteTonicUpgradeLevel;
			dictionary["TotalMTX"] = SwrvePayload.TotalMTX;
			dictionary["CostumesBought"] = SwrvePayload.CostumesBought;
			Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
			Bedrock.AnalyticsLogEvent(text, parameters, false);
		}
	}

	public static void TutorialFailed(string Section)
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.Failed", "Section", Section, false);
	}

	public static void TutorialJumpCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.JumpCompleted");
	}

	public static void TutorialDuckCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.DuckCompleted");
	}

	public static void TutorialJumpDuckCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.JumpDuckCompleted");
	}

	public static void TutorialTurnCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.TurnCompleted");
	}

	public static void TutorialEnemiesCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.EnemiesCompleted");
	}

	public static void TutorialTreasureCompleted()
	{
		Bedrock.AnalyticsLogEvent("Progression.Tutorial.TreasureCompleted");
	}

	public static void TrialCompleted(string trialName, float timeInSeconds, int nBoostsUsed)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["LastRunTime"] = Mathf.CeilToInt(timeInSeconds).ToString();
		dictionary["BoostsUsed"] = nBoostsUsed.ToString();
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("Progression.Trials.CompletedAny", parameters, false);
		string name = "Progression.Trials.Completed." + trialName;
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void TrialUnlocked(int TrialsUnlockedCount)
	{
		Bedrock.AnalyticsLogEvent("Progression.Trials.Unlocked", "TrialsUnlocked", TrialsUnlockedCount.ToString(), false);
	}

	public static void RelicCollected(string strTrialID, int iRelicIndex, int RelicsCollected)
	{
		Bedrock.AnalyticsLogEvent("Progression.Relic.Collected", "RelicCollected", strTrialID + "." + iRelicIndex, "TotalRelicsCollected", RelicsCollected.ToString(), false);
	}
}
