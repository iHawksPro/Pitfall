using System.Collections.Generic;

public class SwrveEventsSocial
{
	private static Bedrock.KeyValueArray socialPayload
	{
		get
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
			dictionary["GamesPlayed"] = SwrvePayload.GamesPlayed;
			dictionary["Score"] = SwrvePayload.LastScore;
			dictionary["Distance"] = SwrvePayload.LastDistance;
			return BedrockUtils.Hash(dictionary);
		}
	}

	public static void GameCentreConnected()
	{
		Bedrock.AnalyticsLogEvent("Metagame.Social.GameCentreConnected");
	}

	public static void TwitterBoast()
	{
		Bedrock.AnalyticsLogEvent("Metagame.Social.TwitterBoast", socialPayload, false);
	}

	public static void FacebookBoast()
	{
		Bedrock.AnalyticsLogEvent("Metagame.Social.FacebookBoast", socialPayload, false);
	}
}
