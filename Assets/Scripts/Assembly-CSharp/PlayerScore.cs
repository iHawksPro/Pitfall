using UnityEngine;

public class PlayerScore
{
	public struct BaddieStats
	{
		public BaddieController.Type m_baddieType;

		public int m_timesSeen;

		public int m_timesKilled;
	}

	public const int NUMXPLEVELS = 50;

	private const int mDistanceScorePerMetre = 10;

	private BaddieStats[] m_baddieStats = new BaddieStats[4]
	{
		default(BaddieStats),
		new BaddieStats
		{
			m_baddieType = BaddieController.Type.Scorpion
		},
		new BaddieStats
		{
			m_baddieType = BaddieController.Type.Snake
		},
		new BaddieStats
		{
			m_baddieType = BaddieController.Type.Spider
		}
	};

	private int mTotalScore;

	private float mAccumulatedDistance;

	private float mTotalDistanceTravelled;

	private int mCoinsCollected;

	private int mCoinsSpawned;

	private int mAnimalsKilled;

	private int mConsecutiveSuccessfulCoinRuns;

	private int mAdditionalXP;

	private bool mJungleFever;

	private float m100metreCount;

	private float m1000metreCount;

	private int mJaguarCount;

	private int mPoisonedCount;

	private int mStungCount;

	private int mRopeSwingsUsed;

	private int mHasteTonicsUsed;

	private int mAntiVenomsUsed;

	private int mLifeTonicsUsed;

	private int mMacawsUsed;

	private int mStylishDismounts;

	private int mTimesTutorialPlayed;

	private float mDistanceInMineCart;

	private float mDistanceOnBike;

	private float mTimeSpentRunning;

	private float mTimeOfLastHaste;

	private bool mDisabled;

	private int[] XPRequired = new int[50];

	private int[] XPToNextLevel = new int[50]
	{
		800, 1100, 1200, 1800, 2200, 2500, 2800, 3200, 3600, 3900,
		4400, 4900, 5400, 5900, 6400, 6900, 7400, 7900, 8400, 8900,
		9400, 9900, 10400, 10900, 11400, 11900, 12400, 12900, 13400, 14000,
		14600, 15200, 15800, 16400, 17000, 17600, 18200, 18800, 19400, 20000,
		20600, 21200, 21800, 22400, 23000, 23600, 24200, 24800, 25400, 26150
	};

	public float RunStartDistance { get; set; }

	public PlayerScore()
	{
		Reset();
	}

	public void Disabled(bool val)
	{
		mDisabled = val;
	}

	public int XPEarned()
	{
		return mAdditionalXP;
	}

	public void ResetOnce()
	{
	}

	public void Reset()
	{
		mTotalScore = 0;
		mAccumulatedDistance = 0f;
		mTotalDistanceTravelled = 0f;
		mAnimalsKilled = 0;
		mCoinsCollected = 0;
		mCoinsSpawned = 0;
		mConsecutiveSuccessfulCoinRuns = 0;
		mJungleFever = false;
		mAdditionalXP = 0;
		m100metreCount = 0f;
		m1000metreCount = 0f;
		mJaguarCount = 0;
		mPoisonedCount = 0;
		mStungCount = 0;
		mRopeSwingsUsed = 0;
		mDisabled = false;
		mStylishDismounts = 0;
		mTimesTutorialPlayed = 0;
		mTimeSpentRunning = 0f;
		mHasteTonicsUsed = 0;
		mAntiVenomsUsed = 0;
		mLifeTonicsUsed = 0;
		mMacawsUsed = 0;
		mTimeOfLastHaste = 0f;
		mDistanceInMineCart = 0f;
		mDistanceOnBike = 0f;
		XPRequired[0] = XPToNextLevel[0];
		for (int i = 1; i < 50; i++)
		{
			XPRequired[i] = XPRequired[i - 1] + XPToNextLevel[i];
		}
		for (int j = 0; j < m_baddieStats.Length; j++)
		{
			m_baddieStats[j].m_timesKilled = 0;
			m_baddieStats[j].m_timesSeen = 0;
		}
		RunStartDistance = 0f;
	}

	public void OverrideDistance(float dist)
	{
		mTotalDistanceTravelled = dist;
	}

	public void CoinSpawned()
	{
		mCoinsSpawned++;
	}

	public void CoinRunSuccess()
	{
		RegisterEvent_CollectCoinRun();
		mConsecutiveSuccessfulCoinRuns++;
		if (mConsecutiveSuccessfulCoinRuns == 1)
		{
			mTotalScore += 100;
		}
		else if (mConsecutiveSuccessfulCoinRuns == 2)
		{
			mTotalScore += 200;
		}
		else if (mConsecutiveSuccessfulCoinRuns == 3)
		{
			mJungleFever = true;
			RegisterEvent_EnteredJungleFever();
		}
	}

	public void CoinRunFail()
	{
		mJungleFever = false;
		mConsecutiveSuccessfulCoinRuns = 0;
	}

	public int GetTreasureBonus()
	{
		if (mConsecutiveSuccessfulCoinRuns < 3)
		{
			return mConsecutiveSuccessfulCoinRuns;
		}
		return 3;
	}

	public bool JungleFever()
	{
		return mJungleFever;
	}

	public int TotalScoreXPAdjusted()
	{
		return mTotalScore * XPScoreMultiplier();
	}

	public int CoinsCollected()
	{
		return mCoinsCollected;
	}

	public int CoinsSpawned()
	{
		return mCoinsSpawned;
	}

	public int AnimalsKilled()
	{
		return mAnimalsKilled;
	}

	public int ScorpionsKilled()
	{
		return BaddiesKilled(BaddieController.Type.Scorpion);
	}

	public float DistanceTravelled()
	{
		return mTotalDistanceTravelled;
	}

	public int XPLevel()
	{
		return SecureStorage.Instance.GetXPLevel() + 1;
	}

	public int XPToAdd()
	{
		return mAdditionalXP;
	}

	public int JaguarsCollected()
	{
		return mJaguarCount;
	}

	public int TimesPoisoned()
	{
		return mPoisonedCount;
	}

	public int TimesStung()
	{
		return mStungCount;
	}

	public int NumHasteTonicsUsed()
	{
		return mHasteTonicsUsed;
	}

	public int NumAntiVenomsUsed()
	{
		return mAntiVenomsUsed;
	}

	public int NumLifeTonicsUsed()
	{
		return mLifeTonicsUsed;
	}

	public int NumMacawsUsed()
	{
		return mMacawsUsed;
	}

	public void HasteTonicUsed()
	{
		mHasteTonicsUsed++;
		mTimeOfLastHaste = Time.realtimeSinceStartup;
	}

	public void AntiVenomUsed()
	{
		mAntiVenomsUsed++;
	}

	public void LifeTonicUsed()
	{
		mLifeTonicsUsed++;
	}

	public void IncMacawsUsed()
	{
		mMacawsUsed++;
	}

	public int NumStylishDismounts()
	{
		return mStylishDismounts;
	}

	public int TimesTutorialPlayed()
	{
		return mTimesTutorialPlayed;
	}

	public void IncStylishDismounts()
	{
		mStylishDismounts++;
	}

	public void TutorialPlayed()
	{
		mTimesTutorialPlayed++;
	}

	public float DistanceInMineCart()
	{
		return mDistanceInMineCart;
	}

	public float DistanceOnBike()
	{
		return mDistanceOnBike;
	}

	public int BaddiesKilled(BaddieController.Type baddieType)
	{
		int result = 0;
		if ((int)baddieType < m_baddieStats.Length)
		{
			result = m_baddieStats[(int)baddieType].m_timesKilled;
		}
		return result;
	}

	public int BaddiesSeen(BaddieController.Type baddieType)
	{
		int result = 0;
		if ((int)baddieType < m_baddieStats.Length)
		{
			result = m_baddieStats[(int)baddieType].m_timesSeen;
		}
		return result;
	}

	public int RopeSwingsUsed()
	{
		return mRopeSwingsUsed;
	}

	public bool WasteOfHasteCheckPassed()
	{
		return mHasteTonicsUsed > 0 && Time.realtimeSinceStartup - mTimeOfLastHaste < 5f;
	}

	public int XPScoreMultiplier()
	{
		return 1;
	}

	public void AddDistance(float distanceSinceLastFrame)
	{
		if (!mDisabled)
		{
			mTotalDistanceTravelled += distanceSinceLastFrame;
			mAccumulatedDistance += distanceSinceLastFrame;
			if (mAccumulatedDistance >= 1f)
			{
				int num = (int)mAccumulatedDistance;
				mTotalScore += num * 10;
				mAccumulatedDistance -= num;
			}
			m100metreCount += distanceSinceLastFrame;
			if (m100metreCount > 100f)
			{
				m100metreCount -= 100f;
				RegisterEvent_100metres();
			}
			m1000metreCount += distanceSinceLastFrame;
			if (m1000metreCount > 1000f)
			{
				m1000metreCount -= 1000f;
				RegisterEvent_1000metres();
			}
			mTimeSpentRunning += Time.deltaTime;
			if (ThemeManager.Instance.IsInMineCart())
			{
				mDistanceInMineCart += distanceSinceLastFrame;
			}
			if (ThemeManager.Instance.IsOnBike())
			{
				mDistanceOnBike += distanceSinceLastFrame;
			}
		}
	}

	public float TimeSpentRunning()
	{
		return mTimeSpentRunning / 60f;
	}

	public void AddAnimalKill(BaddieController.Type AnimalType)
	{
		mAnimalsKilled++;
		if ((int)AnimalType < m_baddieStats.Length)
		{
			m_baddieStats[(int)AnimalType].m_timesKilled++;
			if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Swat))
			{
				AddXP(50);
			}
		}
	}

	public void AddTreasure(Coin.CoinType type)
	{
		if (mDisabled)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		if (DailyDoubleController.Instance.DD_treasureMultiplier == 1f)
		{
			if (type != Coin.CoinType.ValueLow)
			{
				num2++;
			}
		}
		else
		{
			num2 *= (int)DailyDoubleController.Instance.DD_treasureMultiplier;
			type = Coin.CoinType.ValueHigh;
		}
		mCoinsCollected += num2;
		switch (type)
		{
		case Coin.CoinType.ValueLow:
			num = 1;
			break;
		case Coin.CoinType.ValueMedium:
			num = 10;
			break;
		case Coin.CoinType.ValueHigh:
			num = 50;
			break;
		}
		int num3 = 1;
		if (mJungleFever)
		{
			num3 = 3;
		}
		mTotalScore += num * num3;
	}

	public void IncJaguar()
	{
		mJaguarCount++;
	}

	public void IncPoisoned()
	{
		mPoisonedCount++;
	}

	public void IncStung()
	{
		mStungCount++;
	}

	public string GetXPDescription()
	{
		int num = (SecureStorage.Instance.GetXPLevel() + 1) / 10;
		if (num > 5)
		{
			num = 5;
		}
		string key = "S_XP_DESCRIPTION_" + num;
		return Language.Get(key);
	}

	public int CurrentXP()
	{
		return SecureStorage.Instance.GetXP();
	}

	public int NextXPLimit()
	{
		return XPRequired[SecureStorage.Instance.GetXPLevel()];
	}

	public int CurrentXPLimit()
	{
		if (SecureStorage.Instance.GetXPLevel() == 0)
		{
			return 0;
		}
		return XPRequired[SecureStorage.Instance.GetXPLevel() - 1];
	}

	public void IncrementXP(int num, out bool earnedDistinction, out bool levelUp)
	{
		earnedDistinction = false;
		levelUp = false;
		if (mAdditionalXP == 0)
		{
			return;
		}
		mAdditionalXP -= num;
		if (mAdditionalXP < 0)
		{
			num += mAdditionalXP;
			mAdditionalXP = 0;
		}
		if (SecureStorage.Instance.GetXPLevel() == 48 && SecureStorage.Instance.GetXPDistinction() == 4)
		{
			return;
		}
		SecureStorage.Instance.IncreaseXP(num);
		earnedDistinction = false;
		if (SecureStorage.Instance.GetXP() < XPRequired[SecureStorage.Instance.GetXPLevel()])
		{
			return;
		}
		SecureStorage.Instance.IncreaseXPLevel(1);
		levelUp = true;
		SwrveEventsProgression.PlayerLevelUp();
		SwrveEventsProgression.PlayerLevelMilestone(SecureStorage.Instance.GetXPLevel());
		if (SecureStorage.Instance.GetXPLevel() >= 49)
		{
			SecureStorage.Instance.IncreaseXPDistinction(1);
			earnedDistinction = true;
			SwrveEventsProgression.PlayerPrestiged();
			if (SecureStorage.Instance.GetXPDistinction() == 4)
			{
				SecureStorage.Instance.SetXPLevel(48);
				SecureStorage.Instance.SetXP(NextXPLimit());
			}
			else
			{
				SecureStorage.Instance.SetXP(0);
				SecureStorage.Instance.SetXPLevel(0);
			}
			switch (SecureStorage.Instance.GetXPDistinction())
			{
			case 1:
				AchievementManager.Instance.SetCompleted("distinction.bronze");
				SecureStorage.Instance.UpdateAccumAchievement("complete", 1);
				break;
			case 2:
				AchievementManager.Instance.SetCompleted("distinction.silver");
				SecureStorage.Instance.UpdateAccumAchievement("complete", 1);
				break;
			case 3:
				AchievementManager.Instance.SetCompleted("distinction.gold");
				SecureStorage.Instance.UpdateAccumAchievement("complete", 1);
				break;
			case 4:
				AchievementManager.Instance.SetCompleted("distinction.platinum");
				SecureStorage.Instance.UpdateAccumAchievement("complete", 1);
				break;
			}
		}
	}

	public int NumGemsForLevelUp()
	{
		int num = SecureStorage.Instance.GetXPLevel() + 1;
		if (SecureStorage.Instance.GetXPDistinction() == 0)
		{
			return SwrveServerVariables.Instance.GetGemsLevelReward(num);
		}
		if (num == 0)
		{
			return SwrveServerVariables.Instance.GetDistinctionReward();
		}
		return 1;
	}

	public void RegisterEvent_100metres()
	{
		AddXP(10);
	}

	public void RegisterEvent_1000metres()
	{
		AddXP(100);
	}

	public void RegisterEvent_CollectedPowerUp()
	{
		AddXP(10);
	}

	public void RegisterEvent_CollectCoinRun()
	{
		AddXP(5);
	}

	public void RegisterEvent_EnteredJungleFever()
	{
		AddXP(15);
	}

	public void RegisterEvent_RopeSwing()
	{
		AddXP(10);
		if (OutfitOfTheDayManager.Instance.BonusApplies(Costume.Modern))
		{
			AddXP(100);
		}
		mRopeSwingsUsed++;
	}

	public void RegisterEvent_AvoidPit()
	{
		AddXP(5);
	}

	public void RegisterEvent_PassedEnemy(BaddieController.Type BaddieType)
	{
		AddXP(5);
		if ((int)BaddieType < m_baddieStats.Length)
		{
			m_baddieStats[(int)BaddieType].m_timesSeen++;
		}
	}

	public void RegisterEvent_CollectedRelic()
	{
		AddXP(20);
	}

	public void AddXP(int amount)
	{
		mAdditionalXP += (int)(SwrveServerVariables.Instance.XPMultiplier * (float)amount * DailyDoubleController.Instance.DD_xpMultiplier);
	}
}
