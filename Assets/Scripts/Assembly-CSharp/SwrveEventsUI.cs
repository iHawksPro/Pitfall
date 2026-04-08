using System.Collections.Generic;

public class SwrveEventsUI
{
	private static string m_lastViewedStorePanel = string.Empty;

	private static string m_lastViewedFieldPanel = string.Empty;

	public static void GameCentreButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.GameCentreButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void StoreButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.StoreButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void FieldGuideTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.FieldGuideButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void OptionsTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.OptionsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void LeaderboardsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.LeaderboardsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void AchievementsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.AchievementsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void TwitterButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.TwitterButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void FacebookButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.FacebookButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void GiftButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.GiftButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void MoreAppsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.MoreAppsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void TrialsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.TrialsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void FreeDiamondsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Title.FreeDiamondsButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void ViewedStore()
	{
		Bedrock.AnalyticsLogEvent("UI.Store.ViewedStore", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void ViewedStorePanel(string panelName)
	{
		m_lastViewedStorePanel = panelName;
		string text = "UI.Store.Viewed" + panelName;
		SecureStorage instance = SecureStorage.Instance;
		if (!instance.HasIssuedSwrveEvent(text))
		{
			string name = "UI.Store.FirstViewed" + panelName;
			Bedrock.AnalyticsLogEvent(name, "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
		}
		Bedrock.AnalyticsLogEvent(text, "PlayerLevel", SwrvePayload.PlayerLevel, false);
		instance.SwrveEventIssued(text);
	}

	public static void StoreViewedAllPanelProducts()
	{
		string name = "UI.Store.ViewedAll" + m_lastViewedStorePanel;
		Bedrock.AnalyticsLogEvent(name, "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void ViewedFieldPanel(string panelName)
	{
		m_lastViewedFieldPanel = panelName;
		string text = "UI.FieldGuide.Viewed" + panelName;
		SecureStorage instance = SecureStorage.Instance;
		if (!instance.HasIssuedSwrveEvent(text))
		{
			string name = "UI.FieldGuide.FirstViewed" + panelName;
			Bedrock.AnalyticsLogEvent(name, "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
		}
		Bedrock.AnalyticsLogEvent(text, "PlayerLevel", SwrvePayload.PlayerLevel, false);
		instance.SwrveEventIssued(text);
	}

	public static void FieldViewedAllPanel()
	{
		string name = "UI.FieldGuide.ViewedAll" + m_lastViewedFieldPanel;
		Bedrock.AnalyticsLogEvent(name, "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void PauseQuitButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Pause.QuitPressed");
	}

	public static void ResultsRetryTouched()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["Distance"] = SwrvePayload.LastDistance;
		dictionary["Score"] = SwrvePayload.LastScore;
		string name;
		if (SecureStorage.Instance.HasMacaws)
		{
			name = "UI.GameOver.RetryHasMacaws";
			dictionary["MacawStock"] = SwrvePayload.MacawStock;
		}
		else
		{
			name = "UI.GameOver.RetryNoMacaws";
		}
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void ResultsMacawTouched()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["Distance"] = SwrvePayload.LastDistance;
		dictionary["Score"] = SwrvePayload.LastScore;
		dictionary["MacawStock"] = SwrvePayload.MacawStock;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("UI.GameOver.UseMacaw", parameters, false);
	}

	public static void ResultsStoreTouched()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["HardBalance"] = SwrvePayload.CurrentGemTotal;
		dictionary["SoftBalance"] = SwrvePayload.CurrentCoinTotal;
		dictionary["Distance"] = SwrvePayload.LastDistance;
		dictionary["Score"] = SwrvePayload.LastScore;
		dictionary["MacawStock"] = SwrvePayload.MacawStock;
		Bedrock.KeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsLogEvent("UI.GameOver.GoToStore", parameters, false);
	}

	public static void ResultsGameCentreTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.GameOver.GameCentreButtonTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void ResultsFacebookTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.GameOver.FacebookTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void ResultsTwitterTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.GameOver.TwitterTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void CreditsTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.CreditsTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void MusicOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.MusicTurnedOn", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void MusicOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.MusicTurnedOff", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void SfxOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.SfxTurnedOn", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void SfxOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.SfxTurnedOff", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void TutorialReplay()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.TutorialReplay", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void BlastFurnaceTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.BlastFurnaceTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void LowGFXOptionOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.LowGFXOptionOn", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, false);
	}

	public static void LowGFXOptionOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.LowGFXOptionOff", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, false);
	}

	public static void HelpTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.HelpTouched", "PlayerLevel", SwrvePayload.PlayerLevel, "GamesPlayed", SwrvePayload.GamesPlayed, false);
	}

	public static void MessageFollowed(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("UI.Messaging.FollowedMessage", "Message", Identifier, false);
	}

	public static void MessageIgnored(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("UI.Messaging.IgnoredMessage", "Message", Identifier, false);
	}
}
