using System;
using UnityEngine;

public class TrialsData : ScriptableObject
{
	public enum Difficulty
	{
		Easy = 0,
		Medium = 1,
		Hard = 2,
		Pro = 3
	}

	[Serializable]
	public class TrialDefinition
	{
		public string m_identifier;

		public string m_title;

		public string m_shorttitle;

		public Difficulty m_difficulty;

		public string m_levelName;

		public int m_numCollectables;

		public int m_rewardGems;

		public float m_fBronzeTime = 99999f;

		public float m_fSilverTime = 9999f;

		public float m_fGoldTime = 999f;

		public string m_unlockedbycompleting;
	}

	[Serializable]
	public class TrialGroup
	{
		public WorldConstructionHelper.Theme m_theme;

		public string m_achievementId;

		public TrialDefinition[] m_trials;
	}

	[Serializable]
	public class BurstTonicParameters
	{
		public int m_maxBurstTonics;

		public float m_burstTonicSpeedMultiplier;

		public float m_burstTonicDuration;

		public float m_burstTonicRefillTime;

		public string m_maxBoostAchivementId;
	}

	public int m_completionReward;

	public string m_completeAchievementId;

	public string m_singleAchievementId;

	public TrialGroup[] m_trialGroups;

	public BurstTonicParameters m_burstTonic;

	public string m_relicAchievementId;
}
