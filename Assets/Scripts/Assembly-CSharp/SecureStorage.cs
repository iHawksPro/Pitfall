using System;
using System.Collections.Generic;
using UnityEngine;

public class SecureStorage
{
	private const string GemsKey = "player.gems";

	private const string TotalGemsKey = "player.totalgems";

	private const string CoinsKey = "player.coins";

	private const string TotalCoinsKey = "player.totalcoins";

	private const string OneShotStatKeyBase = "player.oneshot.";

	private const string AccumStatKeyBase = "player.accum.";

	private const string LastStatKeyBase = "player.last.";

	private const string TotalMTXKey = "player.totalMTX";

	private const string MacawsInRun = "player.macawsInRun";

	private const string PurchaseCountKey = "player.purchaseCount";

	private const string XPKey = "player.XP";

	private const string XPLevelKey = "player.XPLevel";

	private const string XPDistinctionKey = "player.XPDistinction";

	private const string LastPlayedDateTimeKey = "player.lastPlayedDateTime";

	private const string FirstGameDateTimeKey = "player.firstGameDateTime";

	private const string GamesPlayedKey = "player.gamesPlayed";

	private const string DialogViewedBase = "player.dialogViewed.";

	private const string LastDialogGamesKey = "player.dialogGamesPlayed";

	private const string DeathTypeBase = "player.deathtype.";

	private const string LastDeathKey = "player.lastDeathType";

	private const string LastRatedVersionKey = "player.rating.lastVersionRated";

	private const string LastRatingCheckedVersionKey = "player.rating.lastCheckedVersionRated";

	private const string TimesRatingCheckedKey = "player.rating.timesRatingChecked";

	private const string ChecksBetweenRatingKey = "player.rating.checksBetweenRating";

	private const string ViewedTutorialKey = "player.tutorial.viewed";

	private const string ViewedDiamondsTutorialKey = "player.diamondstutorial.viewed";

	private const string FurthestDistanceKey = "player.furthestdistance";

	private const string LastDistanceKey = "player.lastdistance";

	private const string LastScoreKey = "player.lastScore";

	private const string HighScoreKey = "player.highScore";

	private const string CurrentCostumeKey = "player.currentCostume";

	private const string CostumesBoughtKey = "player.costumesBought";

	private const string CheckpointUnlockBase = "player.unlockCheckpoint.";

	private const string CheckPointsBoughtKey = "player.checkPointsBought";

	private const string LastThemeKey = "player.lastTheme";

	private const string LastGameTimeKey = "player.lastGameTime";

	private const string LastXPKey = "player.lastXP";

	private const string NinjaPirateKey = "player.ninjaPirateStatus";

	private const string sfxMutedKey = "player.sfxMuted";

	private const string musicMutedKey = "player.musicMuted";

	private const string FirstStoreVisitKey = "player.firstStoreVisit";

	private const string ToldFacebookFriendsKey = "player.social.toldFacebookFriends";

	private const string ToldTwitterFriendsKey = "player.social.toldTwitterFriends";

	private const string LowGFXOption = "player.game.lowGfxOption";

	private const string FailedReceiptVerificationKey = "player.receiptVerifyFailed";

	private const string themeManagerStateKey = "player.themeManagerState";

	private const string trialsStateBaseKey = "player.trialState.";

	private const string trialsBoostStateKey = "player.trialsBoostState";

	private const string aviatorOotdKey = "player.ootd.aviator";

	private const string leftyKey = "player.lefty";

	private const string lastSaleKey = "player.lastSale";

	private const string firstTrialsVisitKey = "player.firstTrialsVisit";

	private const string refilledTrialsBoostKey = "player.hasRefilledBoost";

	private const string kVungleAdCount = "player.vungle.adCount";

	private const string interstitialCountKey = "player.interstitialCountdown";

	private const string interstitialAdKeyBase = "player.interstitial.";

	private const string interstitialViewedAdKeyBase = "player.interstitialViewed.";

	private const string tapjoyGameRunCountKey = "player.tapjoy.runCompleted";

	private const string tapjoyPendingBalanceKey = "player.tapjoy.pendingBalance";

	private const string tapjoyViewedOfferWallKey = "player.tapjoy.viewedOffers";

	private static SecureStorage m_instance;

	public List<string> RecordedVungleAds = new List<string>();

	private Dictionary<string, string> m_stringCache = new Dictionary<string, string>();

	private Dictionary<string, int> m_intCache = new Dictionary<string, int>();

	private Dictionary<string, float> m_floatCache = new Dictionary<string, float>();

	private Dictionary<string, bool> m_boolCache = new Dictionary<string, bool>();

	public static SecureStorage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new SecureStorage();
			}
			return m_instance;
		}
	}

	public bool TutorialViewed
	{
		get
		{
			return GetBool("player.tutorial.viewed");
		}
		set
		{
			SetBool("player.tutorial.viewed", value);
		}
	}

	public bool DiamondsTutorialViewed
	{
		get
		{
			return GetBool("player.diamondstutorial.viewed");
		}
		set
		{
			SetBool("player.diamondstutorial.viewed", value);
		}
	}

	public int FurthestDistanceTravelled
	{
		get
		{
			return GetInt("player.furthestdistance");
		}
		set
		{
			SetInt("player.furthestdistance", value);
		}
	}

	public int LastDistanceTravelled
	{
		get
		{
			return GetInt("player.lastdistance");
		}
		set
		{
			SetInt("player.lastdistance", value);
		}
	}

	public int NinjaPirateStatus
	{
		get
		{
			return GetInt("player.ninjaPirateStatus");
		}
		set
		{
			SetInt("player.ninjaPirateStatus", value);
		}
	}

	public int LastScore
	{
		get
		{
			return GetInt("player.lastScore");
		}
		set
		{
			SetInt("player.lastScore", value);
		}
	}

	public int HighestScore
	{
		get
		{
			return GetInt("player.highScore");
		}
		set
		{
			SetInt("player.highScore", value);
		}
	}

	public float LastGameTime
	{
		get
		{
			return GetFloat("player.lastGameTime");
		}
		set
		{
			SetFloat("player.lastGameTime", value);
		}
	}

	public bool VisitedStore
	{
		get
		{
			return GetBool("player.firstStoreVisit");
		}
		set
		{
			SetBool("player.firstStoreVisit", value);
		}
	}

	public int NumGemPurchases
	{
		get
		{
			return GetInt("player.purchaseCount");
		}
		set
		{
			SetInt("player.purchaseCount", value);
		}
	}

	public int LastXPGained
	{
		get
		{
			return GetInt("player.lastXP");
		}
		set
		{
			SetInt("player.lastXP", value);
		}
	}

	public int LastJaguarCount
	{
		get
		{
			return GetInt("player.last.meow");
		}
	}

	public int LastTimesPoisoned
	{
		get
		{
			return GetInt("player.last.toxicologist");
		}
	}

	public int GetMacawsInRun
	{
		get
		{
			return GetInt("player.macawsInRun");
		}
	}

	public int GamesPlayed
	{
		get
		{
			return GetInt("player.gamesPlayed");
		}
		set
		{
			SetInt("player.gamesPlayed", value);
		}
	}

	public PieceDescriptor.KillType LastDeathType
	{
		get
		{
			PieceDescriptor.KillType result = PieceDescriptor.KillType.None;
			if (HasKey("player.lastDeathType"))
			{
				result = (PieceDescriptor.KillType)GetInt("player.lastDeathType");
			}
			return result;
		}
		set
		{
			SetInt("player.lastDeathType", (int)value);
		}
	}

	public int LastDialogGamesPlayedCount
	{
		get
		{
			return GetInt("player.dialogGamesPlayed");
		}
		set
		{
			SetInt("player.dialogGamesPlayed", value);
		}
	}

	public WorldConstructionHelper.Theme LastTileTheme
	{
		get
		{
			WorldConstructionHelper.Theme result = WorldConstructionHelper.Theme.Jungle;
			if (HasKey("player.lastTheme"))
			{
				result = (WorldConstructionHelper.Theme)GetInt("player.lastTheme");
			}
			return result;
		}
		set
		{
			SetInt("player.lastTheme", (int)value);
		}
	}

	public bool HasMacaws
	{
		get
		{
			return GetItemCount("consumable.macaw") > 0;
		}
	}

	public int NumMacawsHeld
	{
		get
		{
			return GetItemCount("consumable.macaw");
		}
	}

	public string LastPlayedDateTime
	{
		get
		{
			return GetString("player.lastPlayedDateTime");
		}
		set
		{
			SetString("player.lastPlayedDateTime", value);
		}
	}

	public string FirstGameDateTime
	{
		get
		{
			return GetString("player.firstGameDateTime");
		}
	}

	public string LastRatedVersion
	{
		get
		{
			return GetString("player.rating.lastVersionRated");
		}
		set
		{
			SetString("player.rating.lastVersionRated", value);
		}
	}

	public string LastCheckedVersion
	{
		get
		{
			return GetString("player.rating.lastCheckedVersionRated");
		}
		set
		{
			SetString("player.rating.lastCheckedVersionRated", value);
		}
	}

	public int TimesRatingChecked
	{
		get
		{
			return GetInt("player.rating.timesRatingChecked");
		}
		set
		{
			SetInt("player.rating.timesRatingChecked", value);
		}
	}

	public int CheckedBetweenRaing
	{
		get
		{
			return GetInt("player.rating.checksBetweenRating");
		}
		set
		{
			SetInt("player.rating.checksBetweenRating", value);
		}
	}

	public bool MusicMuted
	{
		get
		{
			return GetBool("player.musicMuted");
		}
		set
		{
			SetBool("player.musicMuted", value);
		}
	}

	public bool SfxMuted
	{
		get
		{
			return GetBool("player.sfxMuted");
		}
		set
		{
			SetBool("player.sfxMuted", value);
		}
	}

	public bool HasToldFacebookFriends
	{
		get
		{
			return GetBool("player.social.toldFacebookFriends");
		}
		set
		{
			SetBool("player.social.toldFacebookFriends", value);
		}
	}

	public bool HasToldTwitterFollowers
	{
		get
		{
			return GetBool("player.social.toldTwitterFriends");
		}
		set
		{
			SetBool("player.social.toldTwitterFriends", value);
		}
	}

	public bool LeftyControls
	{
		get
		{
			return GetBool("player.lefty");
		}
		set
		{
			SetBool("player.lefty", value);
		}
	}

	public bool HasSetLowGFXOption
	{
		get
		{
			return GetBool("player.game.lowGfxOption");
		}
		set
		{
			SetBool("player.game.lowGfxOption", value);
		}
	}

	public bool HasFailedReceiptVerification
	{
		get
		{
			return GetBool("player.receiptVerifyFailed");
		}
		set
		{
			SetBool("player.receiptVerifyFailed", value);
		}
	}

	public bool HasThemeManagerState
	{
		get
		{
			return HasKey("player.themeManagerState");
		}
	}

	public string ThemeManagerState
	{
		get
		{
			return GetString("player.themeManagerState");
		}
		set
		{
			SetString("player.themeManagerState", value);
		}
	}

	public string TrialsBoostState
	{
		get
		{
			return GetString("player.trialsBoostState");
		}
		set
		{
			SetString("player.trialsBoostState", value);
		}
	}

	public uint OotdAviatorAwardTime
	{
		get
		{
			uint result = 0u;
			string text = GetString("player.ootd.aviator");
			if (!string.IsNullOrEmpty(text))
			{
				uint.TryParse(text, out result);
			}
			return result;
		}
		set
		{
			uint num = value;
			SetString("player.ootd.aviator", num.ToString());
		}
	}

	public string LastSaleSeen
	{
		get
		{
			return GetString("player.lastSale");
		}
		set
		{
			SetString("player.lastSale", value);
		}
	}

	public bool HasVisitedTrials
	{
		get
		{
			return GetBool("player.firstTrialsVisit");
		}
		set
		{
			SetBool("player.firstTrialsVisit", value);
		}
	}

	public bool HasRefilledTrialsBoost
	{
		get
		{
			return GetBool("player.hasRefilledBoost");
		}
		set
		{
			SetBool("player.hasRefilledBoost", value);
		}
	}

	public bool HasInterstitialCountdown
	{
		get
		{
			return HasKey("player.interstitialCountdown");
		}
	}

	public int InterstitialCountdown
	{
		get
		{
			return GetInt("player.interstitialCountdown");
		}
		set
		{
			SetInt("player.interstitialCountdown", value);
		}
	}

	public int TapjoyGameRunsCompleted
	{
		get
		{
			return GetInt("player.tapjoy.runCompleted");
		}
		set
		{
			SetInt("player.tapjoy.runCompleted", value);
		}
	}

	public int TapjoyPendingBalance
	{
		get
		{
			return GetInt("player.tapjoy.pendingBalance");
		}
		set
		{
			SetInt("player.tapjoy.pendingBalance", value);
		}
	}

	public bool HasViewedTapjoyOffers
	{
		get
		{
			return GetBool("player.tapjoy.viewedOffers");
		}
		set
		{
			SetBool("player.tapjoy.viewedOffers", value);
		}
	}

	public static event Action<int> playerGemsChanged;

	public static event Action<int> playerCoinsChanged;

	public static event Action<string, int> playerItemsChanged;

	private SecureStorage()
	{
	}

	public void SetInt(string Key, int Value)
	{
		Bedrock.SetUserVariableAsInt(Key, Value);
		m_intCache[Key] = Value;
	}

	public int GetInt(string Key)
	{
		int value;
		if (!m_intCache.TryGetValue(Key, out value))
		{
			value = Bedrock.GetUserVariableAsInt(Key, 0);
			m_intCache[Key] = value;
		}
		return value;
	}

	public void IncrementInt(string Key)
	{
		SetInt(Key, GetInt(Key) + 1);
	}

	public void SetBool(string Key, bool Value)
	{
		Bedrock.SetUserVariableAsBool(Key, Value);
		m_boolCache[Key] = Value;
	}

	public bool GetBool(string Key)
	{
		bool value;
		if (!m_boolCache.TryGetValue(Key, out value))
		{
			value = Bedrock.GetUserVariableAsBool(Key, false);
			m_boolCache[Key] = value;
		}
		return value;
	}

	public void SetFloat(string Key, float Value)
	{
		Bedrock.SetUserVariableAsFloat(Key, Value);
		m_floatCache[Key] = Value;
	}

	public float GetFloat(string Key)
	{
		float value;
		if (!m_floatCache.TryGetValue(Key, out value))
		{
			value = Bedrock.GetUserVariableAsFloat(Key, 0f);
			m_floatCache[Key] = value;
		}
		return value;
	}

	public void SetString(string Key, string Value)
	{
		Bedrock.SetUserVariableAsString(Key, Value);
		m_stringCache[Key] = Value;
	}

	private bool HasKey(string Key)
	{
		return m_intCache.ContainsKey(Key) || m_boolCache.ContainsKey(Key) || m_floatCache.ContainsKey(Key) || m_stringCache.ContainsKey(Key) || Bedrock.UserVariableExists(Key);
	}

	public string GetString(string Key)
	{
		string value = string.Empty;
		if (HasKey(Key) && !m_stringCache.TryGetValue(Key, out value))
		{
			value = Bedrock.GetUserVariableAsString(Key, string.Empty);
			m_stringCache[Key] = value;
		}
		return value;
	}

	public void SetCurrentCostume(string newCostume)
	{
		SetString("player.currentCostume", newCostume);
	}

	public string GetCurrentCostume()
	{
		if (!HasKey("player.currentCostume"))
		{
			SetCurrentCostume("outfit.standard");
		}
		return GetString("player.currentCostume");
	}

	public Costume TranslateCostumeType(string CostumeIdentifier)
	{
		Costume costume = Costume.None;
		switch (CostumeIdentifier)
		{
		case "outfit.bear":
			return Costume.Bear;
		case "outfit.fairy":
			return Costume.Fairy;
		case "outfit.adventurer":
			return Costume.Modern;
		case "outfit.ninja":
			return Costume.Ninja;
		case "outfit.aviator":
			return Costume.Pilot;
		case "outfit.pirate":
			return Costume.Pirate;
		case "outfit.swat":
			return Costume.Swat;
		case "outfit.super":
			return Costume.Super;
		default:
			return Costume.None;
		}
	}

	public string TranslateCostumeIdentifier(Costume costumeType)
	{
		string empty = string.Empty;
		switch (costumeType)
		{
		case Costume.Bear:
			return "outfit.bear";
		case Costume.Fairy:
			return "outfit.fairy";
		case Costume.Modern:
			return "outfit.adventurer";
		case Costume.Ninja:
			return "outfit.ninja";
		case Costume.Pilot:
			return "outfit.aviator";
		case Costume.Pirate:
			return "outfit.pirate";
		case Costume.Swat:
			return "outfit.swat";
		case Costume.Super:
			return "outfit.super";
		default:
			return "outfit.standard";
		}
	}

	public Costume GetCurrentCostumeType()
	{
		string currentCostume = GetCurrentCostume();
		return TranslateCostumeType(currentCostume);
	}

	public int GetCostumesBought()
	{
		return GetInt("player.costumesBought");
	}

	public void IncreaseCostumesBought(int Delta)
	{
		if (Delta > 0)
		{
			SetInt("player.costumesBought", GetCostumesBought() + Delta);
		}
	}

	private void UpdateOneShotAchievement(string AchKey, int Value)
	{
		if (!(AchievementManager.Instance != null))
		{
			return;
		}
		AchievementManager instance = AchievementManager.Instance;
		string text = "oneshot." + AchKey;
		bool flag = instance.IsCompleted(text);
		int stepCount = instance.GetStepCount(text);
		if (Value >= stepCount)
		{
			instance.SetCompleted(text);
		}
		if (!flag && instance.IsCompleted(text) && AchKey != "complete" && AchKey != "completedroid")
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				UpdateAccumAchievement("complete", 1);
			}
			else
			{
				UpdateAccumAchievement("completedroid", 1);
			}
		}
	}

	public void UpdateAccumAchievement(string AchKey, int Value)
	{
		if (!(AchievementManager.Instance != null))
		{
			return;
		}
		AchievementManager instance = AchievementManager.Instance;
		string id = "accum." + AchKey;
		bool flag = instance.IsCompleted(id);
		instance.IncrementStepBy(id, Value);
		if (!flag && instance.IsCompleted(id) && AchKey != "complete" && AchKey != "completedroid")
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				UpdateAccumAchievement("complete", 1);
			}
			else
			{
				UpdateAccumAchievement("completedroid", 1);
			}
		}
	}

	public int GetXP()
	{
		return GetInt("player.XP");
	}

	public void SetXP(int Count)
	{
		SetInt("player.XP", Count);
	}

	public int IncreaseXP(int Delta)
	{
		if (Delta > 0)
		{
			int num = GetXP() + Delta;
			SetXP(num);
			return num;
		}
		return 0;
	}

	public int GetXPLevel()
	{
		return GetInt("player.XPLevel");
	}

	public void SetXPLevel(int Count)
	{
		SetInt("player.XPLevel", Count);
	}

	public int IncreaseXPLevel(int Delta)
	{
		if (Delta > 0)
		{
			int num = GetXPLevel() + Delta;
			SetXPLevel(num);
			return num;
		}
		return 0;
	}

	public int GetXPDistinction()
	{
		return GetInt("player.XPDistinction");
	}

	public void SetXPDistinction(int Count)
	{
		SetInt("player.XPDistinction", Count);
	}

	public int IncreaseXPDistinction(int Delta)
	{
		if (Delta > 0)
		{
			int num = GetXPDistinction() + Delta;
			SetXPDistinction(num);
			return num;
		}
		return 0;
	}

	public int GetGems()
	{
		return GetInt("player.gems");
	}

	public void SetGems(int Count)
	{
		SetInt("player.gems", Count);
		OnPlayerGemsChanged();
	}

	public int ChangeGems(int Delta)
	{
		int gems = GetGems();
		int num = gems + Delta;
		if (num < 0)
		{
			num = 0;
		}
		IncreaseTotalGems(Delta);
		if (Delta > 0)
		{
			UpdateGemsEarned(Delta);
		}
		else
		{
			UpdateGemsSpent(-Delta);
		}
		SetGems(num);
		return num;
	}

	public int GetTotalGems()
	{
		return GetInt("player.totalgems");
	}

	private void IncreaseTotalGems(int Delta)
	{
		if (Delta > 0)
		{
			SetInt("player.totalgems", GetTotalGems() + Delta);
		}
	}

	private void OnPlayerGemsChanged()
	{
		if (SecureStorage.playerGemsChanged != null)
		{
			SecureStorage.playerGemsChanged(GetGems());
		}
	}

	public int GetTotalMTX()
	{
		return GetInt("player.totalMTX");
	}

	public void IncreaseTotalMTX(int Delta)
	{
		if (Delta > 0)
		{
			SetInt("player.totalMTX", GetTotalMTX() + Delta);
			NumGemPurchases++;
		}
	}

	public int GetCoins()
	{
		return GetInt("player.coins");
	}

	public void SetCoins(int Count)
	{
		SetInt("player.coins", Count);
		OnPlayerCoinsChanged();
	}

	public int ChangeCoins(int Delta)
	{
		int coins = GetCoins();
		int num = coins + Delta;
		if (num < 0)
		{
			num = 0;
		}
		IncreaseTotalCoins(Delta);
		if (Delta < 0)
		{
			UpdateTreasureSpent(-Delta);
		}
		SetCoins(num);
		return num;
	}

	public int GetTotalCoins()
	{
		return GetInt("player.totalcoins");
	}

	private void IncreaseTotalCoins(int Delta)
	{
		if (Delta > 0)
		{
			SetInt("player.totalcoins", GetTotalCoins() + Delta);
		}
	}

	private void OnPlayerCoinsChanged()
	{
		if (SecureStorage.playerCoinsChanged != null)
		{
			SecureStorage.playerCoinsChanged(GetCoins());
		}
	}

	private void UpdateStatAndAchievement(int Value, string StatKey)
	{
		UpdateStatAndAchievement(Value, StatKey, true);
	}

	private void UpdateStatAndAchievement(int Value, string StatKey, bool updateAchievement)
	{
		string key = "player.last." + StatKey;
		SetInt(key, Value);
		string key2 = "player.oneshot." + StatKey;
		int num = GetInt(key2);
		int num2 = Value;
		if (StatKey == "prettyboy")
		{
			num2 = GetMacawsInRun;
		}
		if (num2 > num)
		{
			SetInt(key2, num2);
		}
		string key3 = "player.accum." + StatKey;
		int num3 = GetInt(key3);
		num3 += Value;
		SetInt(key3, num3);
		if (updateAchievement)
		{
			UpdateOneShotAchievement(StatKey, num2);
			UpdateAccumAchievement(StatKey, Value);
		}
	}

	public int GetStat(string StatKey)
	{
		string key = "player.accum." + StatKey;
		return GetInt(key);
	}

	public int GetStatRaw(string StatKey)
	{
		return GetInt(StatKey);
	}

	public void UpdateDistanceRun(int DistanceRunThisSession)
	{
		LastDistanceTravelled = DistanceRunThisSession;
		UpdateStatAndAchievement(DistanceRunThisSession, "endurance");
	}

	public void UpdateFurthestDistanceRun(int DistanceRunThisSession)
	{
		if (FurthestDistanceTravelled < DistanceRunThisSession)
		{
			FurthestDistanceTravelled = DistanceRunThisSession;
		}
	}

	public void UpdateDistanceInMineCart(int Distance)
	{
		UpdateStatAndAchievement(Distance, "minecart");
	}

	public void UpdateDistanceOnBike(int Distance)
	{
		UpdateStatAndAchievement(Distance, "bike");
	}

	public void UpdateTreasureCollected(int TreasureCollected)
	{
		UpdateStatAndAchievement(TreasureCollected, "hoarder");
		ChangeCoins(TreasureCollected);
	}

	public void UpdateHighestScore(int iScoreThisSession)
	{
		if (iScoreThisSession > HighestScore)
		{
			HighestScore = iScoreThisSession;
		}
	}

	public void UpdateScore(int ScoreThisSession)
	{
		UpdateStatAndAchievement(ScoreThisSession, "highscore");
	}

	public void UpdateXP(int XPGainedThisSession)
	{
		LastXPGained = XPGainedThisSession;
	}

	public void UpdateJaguarCount(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "meow");
	}

	public void UpdateLifeTonicsUsed(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "lazarus");
	}

	public void UpdateHasteTonicsUsed(int TimesUsed, bool updateAchievement)
	{
		UpdateStatAndAchievement(TimesUsed, "haste", updateAchievement);
	}

	public void UpdateAntiVenomsUsed(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "antivenom");
		UpdateStatAndAchievement(TimesUsed, "addicted");
	}

	public void UpdateMacawCount(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "prettyboy");
	}

	public void UpdatePoisonedCount(int TimesPoisoned)
	{
		UpdateStatAndAchievement(TimesPoisoned, "toxicologist");
	}

	public void UpdateTimesStungByScorpions(int nTimes)
	{
		UpdateStatAndAchievement(nTimes, "stung");
	}

	public void UpdateScorpionsKilled(int nKilled)
	{
		UpdateStatAndAchievement(nKilled, "sting");
	}

	public void UpdateTimeSpent(float timeSpent)
	{
		UpdateStatAndAchievement((int)timeSpent, "seconds");
	}

	public void UpdateNumRuns(int nPlayed)
	{
		UpdateStatAndAchievement(nPlayed, "runsfinished");
	}

	public void UpdateCheckpointsPassed(int nPassed)
	{
		UpdateStatAndAchievement(nPassed, "checkpoints");
	}

	public void UpdateCrocodilesKilled(int nKilled)
	{
		UpdateStatAndAchievement(nKilled, "crocs");
	}

	public void UpdateSpiritOfJungleCount(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "lazarus");
	}

	public void UpdateSnakesKilled(int nKilled)
	{
		UpdateStatAndAchievement(nKilled, "venomous");
	}

	public void UpdateRopeSwingCount(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "swinger");
	}

	public void UpdateMonkeyCount(int TimesUsed)
	{
		UpdateStatAndAchievement(TimesUsed, "monkey");
	}

	public void UpdateTreasureSpent(int Amount)
	{
		UpdateStatAndAchievement(Amount, "treasurespent");
	}

	public void UpdateGemsSpent(int Amount)
	{
		UpdateStatAndAchievement(Amount, "gemsspent");
	}

	public void UpdateGemsEarned(int Amount)
	{
		UpdateStatAndAchievement(Amount, "gemsearned");
	}

	public void UpdateOutfitFairy(int TimesBought)
	{
		UpdateStatAndAchievement(TimesBought, "wings");
	}

	public void UpdateOutfitBear(int TimesBought)
	{
		UpdateStatAndAchievement(TimesBought, "bear");
	}

	public void UpdateOutfitAviator(int TimesBought)
	{
		UpdateStatAndAchievement(TimesBought, "aviator");
	}

	public void UpdateOutfitsBought(int TimesBought)
	{
		UpdateStatAndAchievement(TimesBought, "fashionista");
	}

	public void UpdateTimesTutorialPlayed(int Amount)
	{
		UpdateStatAndAchievement(Amount, "educational");
	}

	public void UpdateStylishDismounts(int Amount)
	{
		UpdateStatAndAchievement(Amount, "dismount");
	}

	public void IncMacawsInRun()
	{
		SetInt("player.macawsInRun", GetInt("player.macawsInRun") + 1);
	}

	public void ResetMacawsInRun()
	{
		Debug.Log("** Reset Macaws");
		SetInt("player.macawsInRun", 0);
	}

	public void IncGamesPlayed()
	{
		SetInt("player.gamesPlayed", GetInt("player.gamesPlayed") + 1);
	}

	public void IncDeathCount(PieceDescriptor.KillType DeathType)
	{
		LastDeathType = DeathType;
		string key = "player.deathtype." + DeathType;
		SetInt(key, GetInt(key) + 1);
	}

	public int GetDeathCount(PieceDescriptor.KillType DeathType)
	{
		string key = "player.deathtype." + DeathType;
		return GetInt(key);
	}

	public int GetDialogViewedCount(string DialogName)
	{
		string key = "player.dialogViewed." + DialogName;
		return GetInt(key);
	}

	public void SetDialogViewedCount(string DialogName, int Count)
	{
		string key = "player.dialogViewed." + DialogName;
		SetInt(key, Count);
	}

	public void IncDialogViewedCount(string DialogName)
	{
		string key = "player.dialogViewed." + DialogName;
		SetInt(key, GetInt(key) + 1);
	}

	public int GetItemCount(string Identifier)
	{
		return GetInt(Identifier);
	}

	public void SetItemCount(string Identifier, int Count)
	{
		SetInt(Identifier, Count);
		OnPlayerItemsChanged(Identifier);
	}

	public int ChangeItemCount(string Identifier, int Delta)
	{
		int itemCount = GetItemCount(Identifier);
		int num = itemCount + Delta;
		if (num < 0)
		{
			num = 0;
		}
		SetItemCount(Identifier, num);
		return num;
	}

	private void OnPlayerItemsChanged(string Identifier)
	{
		if (SecureStorage.playerItemsChanged != null)
		{
			SecureStorage.playerItemsChanged(Identifier, GetItemCount(Identifier));
		}
	}

	public void AwardFreeMacaws(int num)
	{
		ChangeItemCount("consumable.macaw", num);
	}

	public bool LogFirstGameDateTime()
	{
		bool result = false;
		if (LastPlayedDateTime == null || LastPlayedDateTime == string.Empty)
		{
			string value = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
			SetString("player.firstGameDateTime", value);
			result = true;
		}
		return result;
	}

	public void LogLastPlayedDateTime()
	{
		string lastPlayedDateTime = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
		LastPlayedDateTime = lastPlayedDateTime;
	}

	public void UnlockCheckpoint(string Identifier)
	{
		string key = "player.unlockCheckpoint." + Identifier;
		SetBool(key, true);
	}

	public bool IsCheckpointUnlocked(string Identifier)
	{
		string key = "player.unlockCheckpoint." + Identifier;
		return GetBool(key);
	}

	public int GetCheckPointsBought()
	{
		return GetInt("player.checkPointsBought");
	}

	public void IncreaseCheckPointsBought(int Delta)
	{
		if (Delta > 0)
		{
			SetInt("player.checkPointsBought", GetCheckPointsBought() + Delta);
		}
	}

	public bool HasIssuedSwrveEvent(string eventName)
	{
		return GetBool(eventName);
	}

	public void SwrveEventIssued(string eventName)
	{
		SetBool(eventName, true);
	}

	public string GetTrialState(string trialId)
	{
		string key = "player.trialState." + trialId;
		return GetString(key);
	}

	public void SetTrialState(string trialId, string jsonState)
	{
		string key = "player.trialState." + trialId;
		SetString(key, jsonState);
	}

	public float GetInterstitialAdvertWeight(string AdName)
	{
		string key = "player.interstitial." + AdName;
		return GetFloat(key);
	}

	public void SetInterstitialAdvertWeight(string AdName, float Val)
	{
		string key = "player.interstitial." + AdName;
		SetFloat(key, Val);
	}

	public int GetInterstitialAdvertViewedCount(string AdName)
	{
		string key = "player.interstitialViewed." + AdName;
		return GetInt(key);
	}

	public void IncIntersitialAdvertViewedCount(string AdName)
	{
		string key = "player.interstitialViewed." + AdName;
		SetInt(key, GetInt(key) + 1);
	}

	public void LoadVungleAdData()
	{
		int num = GetInt("player.vungle.adCount");
		RecordedVungleAds.Clear();
		for (int i = 0; i < num; i++)
		{
			RecordedVungleAds.Add(GetString("player.vungle.ad_" + i));
			Debug.Log("SecureStorage: Recorded vungle ad [" + i + "] = " + RecordedVungleAds[i]);
		}
		SetInt("player.vungle.adCount", SwrveServerVariables.Instance.VungleAdsPerDay);
		GameController.Instance.VungleAdsWatchedToday = GameController.Instance.GetNumberOfAdvertsWatchedToday();
	}

	public void RecordVungleAdWatch(string utcTimestamp)
	{
		Debug.Log("SecureStorage: Recording vungle ad at " + utcTimestamp);
		RecordedVungleAds.Insert(0, utcTimestamp);
		int i = 0;
		for (int count = RecordedVungleAds.Count; i < count; i++)
		{
			SetString("player.vungle.ad_" + i, RecordedVungleAds[i]);
		}
	}
}
