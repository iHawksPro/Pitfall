using System;
using UnityEngine;

public class Achievements : ScriptableObject
{
	[Serializable]
	public class AchievementData
	{
		public string Identifier = string.Empty;

		public string Name = string.Empty;

		public string PreEarnedDesc = string.Empty;

		public string EarnedDesc = string.Empty;

		public int nPoints;

		public int nSteps = 1;

		public int Step;

		public int Version = 1;
	}

	public AchievementData[] achievements;
}
