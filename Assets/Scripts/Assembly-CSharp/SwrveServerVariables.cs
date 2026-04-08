using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveServerVariables : MonoBehaviour
{
	private static SwrveServerVariables m_instance;

	private int[] m_levelGemsRewards = new int[6] { 2, 2, 1, 1, 1, 1 };

	private int[] m_mileStoneGemsRewards = new int[5] { 2, 2, 2, 3, 3 };

	public static SwrveServerVariables Instance
	{
		get
		{
			return m_instance;
		}
	}

	public string FacebookURI { get; private set; }

	public string AppStoreURI { get; private set; }

	public string BlastFurnaceURI { get; private set; }

	public string TwitterAccountURI { get; private set; }

	public string AppShortURI { get; private set; }

	public string ImageURI { get; private set; }

	public string FaqURI { get; private set; }

	public string MoreAppsURI { get; private set; }

	public string AppVersion { get; private set; }

	public string AppId { get; private set; }

	public int GemsGivenFirstTime { get; private set; }

	public int MaxFriendMarkers { get; private set; }

	public float XPMultiplier { get; private set; }

	public int MacawsPerCheckpoint { get; private set; }

	public string MostPopularExchange { get; private set; }

	public string MostPopularOutfit { get; private set; }

	public string MostPopularPowerup { get; private set; }

	public string MostPopularUpgrade { get; private set; }

	public int ChecksBetweenRating { get; private set; }

	public int RatingReward { get; private set; }

	public int LowSpender { get; private set; }

	public int MediumSpender { get; private set; }

	public int DTC_initialNeeded { get; private set; }

	public float DTC_attemptMultiplier { get; private set; }

	public int DTC_maxAttempts { get; private set; }

	public bool DD_enabled { get; private set; }

	public float DD_treasureMultiplier { get; private set; }

	public float DD_xpMultiplier { get; private set; }

	public float DD_diamondsMultiplier { get; private set; }

	public uint DD_startDate { get; private set; }

	public uint DD_duration { get; private set; }

	public int VungleAdsPerDay { get; private set; }

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		UpdateLocalSettingsFromServerVariables();
	}

	private void OnEnable()
	{
		Bedrock.TitleParametersChanged += HandleTitleParametersChanged;
		Bedrock.UserResourcesChanged += HandleUserResourcesChanged;
	}

	private void OnDisable()
	{
		Bedrock.TitleParametersChanged -= HandleTitleParametersChanged;
		Bedrock.UserResourcesChanged -= HandleUserResourcesChanged;
	}

	private void OnDestroy()
	{
		if (this == m_instance)
		{
			m_instance = null;
		}
	}

	private void HandleTitleParametersChanged(object Sender, EventArgs Args)
	{
	}

	private void HandleUserResourcesChanged(object Sender, EventArgs Args)
	{
		UpdateLocalSettingsFromServerVariables();
	}

	private void UpdateLocalSettingsFromServerVariables()
	{
		TBFUtils.DebugLog("SWRVE: Updating strings");
		Language.UpdateLanguageFromBedrock();
		TBFUtils.DebugLog("SWRVE: Updating products");
		StoreProductManager.Instance.UpdateProductsFromBedrock();
		TBFUtils.DebugLog("SWRVE: Updating interstitials");
		DialogManager.Instance.UpdateFromBedrock();
		AppVersion = Bedrock.GetRemoteVariableAsString("appVersion", TBFUtils.BuildVersion);
		AppId = Bedrock.GetRemoteVariableAsString("appId", "com.activision.pitfall");
		AppShortURI = Bedrock.GetRemoteVariableAsString("appShortURI", "http://bit.ly/RBpfI5");
		AppStoreURI = string.Format("https://play.google.com/store/apps/details?id={0}", AppId);
		AppStoreURI = Bedrock.GetRemoteVariableAsString("appStore", AppStoreURI);
		FacebookURI = Bedrock.GetRemoteVariableAsString("facebook", "http://j.mp/tbf_facebk");
		BlastFurnaceURI = Bedrock.GetRemoteVariableAsString("tbfUrl", "http://www.activision.com");
		TwitterAccountURI = Bedrock.GetRemoteVariableAsString("twitter", "http://j.mp/tbf_tweet");
		ImageURI = Bedrock.GetRemoteVariableAsString("imageLink", "http://cdnm.activision.com/PitfallFacebook/IconSquare_512.png");
		FaqURI = Bedrock.GetRemoteVariableAsString("faqURI", "http://cdnm.activision.com/Pitfall/support/faq/");
		MoreAppsURI = Bedrock.GetRemoteVariableAsString("moreAppsURI", string.Empty);
		MaxFriendMarkers = Bedrock.GetRemoteVariableAsInt("friendMarkers", 6);
		XPMultiplier = Bedrock.GetRemoteVariableAsFloat("xpMult", 2f);
		MacawsPerCheckpoint = Bedrock.GetRemoteVariableAsInt("checkpointMacaw", 5);
		GemsGivenFirstTime = Bedrock.GetRemoteVariableAsInt("firstfreediamonds", 10);
		TBFUtils.DebugLog("SWRVE: Updating most popular");
		MostPopularOutfit = "none";
		MostPopularPowerup = "none";
		MostPopularUpgrade = "none";
		MostPopularExchange = "none";
		Dictionary<string, string> resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources("mostPopular", out resourceDictionary) && resourceDictionary != null)
		{
			MostPopularOutfit = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "outfit", "none");
			MostPopularPowerup = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "powerup", "none");
			MostPopularUpgrade = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "upgrade", "none");
			MostPopularExchange = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "exchange", "none");
		}
		TBFUtils.DebugLog("SWRVE: Updating rate app");
		ChecksBetweenRating = Bedrock.GetRemoteVariableAsInt("rateTrigger", 20);
		RatingReward = Bedrock.GetRemoteVariableAsInt("rateReward", 3000);
		TBFUtils.DebugLog("RatingReward = " + RatingReward);
		TBFUtils.DebugLog("SWRVE: Updating level up rewards");
		resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources("levelUp", out resourceDictionary) && resourceDictionary != null)
		{
			m_levelGemsRewards[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_1_reward", m_levelGemsRewards[0]);
			m_levelGemsRewards[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_2_reward", m_levelGemsRewards[1]);
			m_levelGemsRewards[2] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_3_reward", m_levelGemsRewards[2]);
			m_levelGemsRewards[3] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_4_reward", m_levelGemsRewards[3]);
			m_levelGemsRewards[4] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_5_reward", m_levelGemsRewards[4]);
			m_levelGemsRewards[5] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_6_reward", m_levelGemsRewards[5]);
			m_mileStoneGemsRewards[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_2_milestone", m_mileStoneGemsRewards[0]);
			m_mileStoneGemsRewards[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_3_milestone", m_mileStoneGemsRewards[1]);
			m_mileStoneGemsRewards[2] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_4_milestone", m_mileStoneGemsRewards[2]);
			m_mileStoneGemsRewards[3] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_5_milestone", m_mileStoneGemsRewards[3]);
			m_mileStoneGemsRewards[4] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "band_6_milestone", m_mileStoneGemsRewards[4]);
		}
		resourceDictionary = null;
		LowSpender = 10;
		MediumSpender = 50;
		if (Bedrock.GetRemoteUserResources("SpenderCategories", out resourceDictionary) && resourceDictionary != null)
		{
			LowSpender = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "low", LowSpender);
			MediumSpender = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "medium", MediumSpender);
		}
		resourceDictionary = null;
		DTC_initialNeeded = 1;
		DTC_attemptMultiplier = 2f;
		DTC_maxAttempts = 10;
		VungleAdsPerDay = 1;
		if (Bedrock.GetRemoteUserResources("DiamondsToContinue", out resourceDictionary) && resourceDictionary != null)
		{
			DTC_initialNeeded = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "initialNeeded", DTC_initialNeeded);
			DTC_attemptMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "attemptMultiplier", DTC_attemptMultiplier);
			DTC_maxAttempts = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "maxAttempts", DTC_maxAttempts);
			VungleAdsPerDay = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "vungleAdsPerDay", VungleAdsPerDay);
			Debug.LogWarning("RETREIVED vungleAdsPerDay FROM SWRVE: " + VungleAdsPerDay);
		}
		UpdateDailyDouble();
	}

	public void RefreshSelectedVars()
	{
		UpdateDailyDouble();
	}

	private void UpdateDailyDouble()
	{
		Dictionary<string, string> resourceDictionary = null;
		DD_enabled = false;
		DD_treasureMultiplier = 1f;
		DD_xpMultiplier = 1f;
		DD_diamondsMultiplier = 1f;
		DD_startDate = 0u;
		DD_duration = 0u;
		if (!Bedrock.GetRemoteUserResources("DailyDouble", out resourceDictionary) || resourceDictionary == null)
		{
			return;
		}
		DD_enabled = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "enabled", DD_enabled);
		if (!DD_enabled)
		{
			return;
		}
		MonoBehaviour.print("Daily Double enabled");
		bool flag = false;
		DD_treasureMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "treasureMultiplier", DD_treasureMultiplier);
		DD_xpMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "xpMultiplier", DD_xpMultiplier);
		DD_diamondsMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "diamondsMultiplier", DD_diamondsMultiplier);
		if (DD_treasureMultiplier != 1f && DD_treasureMultiplier != 2f && DD_treasureMultiplier != 3f && DD_treasureMultiplier != 4f)
		{
			flag = true;
		}
		if (DD_xpMultiplier != 1f && DD_xpMultiplier != 2f && DD_xpMultiplier != 3f && DD_xpMultiplier != 4f)
		{
			flag = true;
		}
		if (DD_diamondsMultiplier != 1f && DD_diamondsMultiplier != 2f && DD_diamondsMultiplier != 3f && DD_diamondsMultiplier != 4f)
		{
			flag = true;
		}
		if (flag)
		{
			Debug.LogError("For Daily Double multipliers, only 2,3, or 4 can be used!");
			DD_treasureMultiplier = 1f;
			DD_xpMultiplier = 1f;
			DD_diamondsMultiplier = 1f;
			DD_enabled = false;
			return;
		}
		uint result = 0u;
		uint result2 = 0u;
		string fromResourceDictionaryAsString = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "startDate", string.Empty);
		if (!uint.TryParse(fromResourceDictionaryAsString, out result))
		{
			result = 0u;
		}
		string fromResourceDictionaryAsString2 = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "duration", string.Empty);
		if (!uint.TryParse(fromResourceDictionaryAsString2, out result2))
		{
			result2 = 0u;
		}
		if (result == 0 || result2 == 0)
		{
			Debug.LogError("Daily Double: bad start or duration!");
			DD_enabled = false;
		}
		else
		{
			DD_startDate = result;
			DD_duration = result2;
		}
	}

	public int GetGemsLevelReward(int levelJustReached)
	{
		if (SecureStorage.Instance.GetXPDistinction() > 0)
		{
			return 1;
		}
		if (levelJustReached < 10)
		{
			return m_levelGemsRewards[0];
		}
		if (levelJustReached == 10)
		{
			return m_levelGemsRewards[1] + m_mileStoneGemsRewards[0];
		}
		if (levelJustReached < 20)
		{
			return m_levelGemsRewards[1];
		}
		if (levelJustReached == 20)
		{
			return m_levelGemsRewards[2] + m_mileStoneGemsRewards[1];
		}
		if (levelJustReached < 30)
		{
			return m_levelGemsRewards[2];
		}
		if (levelJustReached == 30)
		{
			return m_levelGemsRewards[3] + m_mileStoneGemsRewards[2];
		}
		if (levelJustReached < 40)
		{
			return m_levelGemsRewards[3];
		}
		if (levelJustReached == 40)
		{
			return m_levelGemsRewards[4] + m_mileStoneGemsRewards[3];
		}
		if (levelJustReached < 50)
		{
			return m_levelGemsRewards[4];
		}
		return 1;
	}

	public int GetDistinctionReward()
	{
		return m_mileStoneGemsRewards[m_mileStoneGemsRewards.Length - 1];
	}
}
