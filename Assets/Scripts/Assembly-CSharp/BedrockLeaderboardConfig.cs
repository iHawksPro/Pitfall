using System;
using UnityEngine;

public class BedrockLeaderboardConfig : ScriptableObject
{
	[Serializable]
	public class BoardConfig
	{
		public string Name = string.Empty;

		public int BedrockID = int.MinValue;

		public Bedrock.brLeaderboardWriteType WriteType = Bedrock.brLeaderboardWriteType.BR_STAT_WRITE_MIN;
	}

	public BoardConfig[] BoardConfigs;

	public uint GetBedrockLeaderboardIDFor(string strName)
	{
		for (int i = 0; i < BoardConfigs.Length; i++)
		{
			if (BoardConfigs[i].Name == strName)
			{
				return (uint)BoardConfigs[i].BedrockID;
			}
		}
		Debug.LogError("Cannot Find Leaderboard ID for " + strName);
		return uint.MaxValue;
	}

	public Bedrock.brLeaderboardWriteType GetWriteTypeFor(string strName)
	{
		for (int i = 0; i < BoardConfigs.Length; i++)
		{
			if (BoardConfigs[i].Name == strName)
			{
				return BoardConfigs[i].WriteType;
			}
		}
		Debug.LogError("Cannot Find Leaderboard Write Type for " + strName);
		return Bedrock.brLeaderboardWriteType.BR_STAT_WRITE_MAX;
	}
}
