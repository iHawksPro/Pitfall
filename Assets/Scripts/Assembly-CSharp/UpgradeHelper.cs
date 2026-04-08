public class UpgradeHelper
{
	private static float[] poisonTimes = new float[4] { 30f, 25f, 20f, 15f };

	private static UpgradeTable mPoisonUpgrades = new UpgradeTable(poisonTimes);

	private static float[] spiritOfTheJungleDurations = new float[4] { 10f, 15f, 20f, 20f };

	private static UpgradeTable mSpiritOfTheJungleUpgrades = new UpgradeTable(spiritOfTheJungleDurations);

	private static float[] callOfTheJaguarDurations = new float[4] { 10f, 15f, 15f, 20f };

	private static UpgradeTable mCallOfTheJaguarUpgrades = new UpgradeTable(callOfTheJaguarDurations);

	private static float[] hasteTonicDurations = new float[4] { 20f, 30f, 40f, 50f };

	private static UpgradeTable mHasteTonicUpgrades = new UpgradeTable(hasteTonicDurations);

	public static float PoisonDuration()
	{
		return mPoisonUpgrades.Lookup(SecureStorage.Instance.GetItemCount("upgrade.poison"));
	}

	public static bool CallOfTheMacawAutoCheckPoint()
	{
		return SecureStorage.Instance.GetItemCount("upgrade.jungle") > 0;
	}

	public static float SpiritOfTheJungleDuration()
	{
		return mSpiritOfTheJungleUpgrades.Lookup(SecureStorage.Instance.GetItemCount("upgrade.jungle"));
	}

	public static float SpiritOfTheJungleCoolDownDuration()
	{
		if (SecureStorage.Instance.GetItemCount("upgrade.jungle") >= 3)
		{
			return 5f;
		}
		return 10f;
	}

	public static float CallOfTheJaguarDuration()
	{
		return mCallOfTheJaguarUpgrades.Lookup(SecureStorage.Instance.GetItemCount("upgrade.jaguar"));
	}

	public static bool CallOfTheJaguarAutoCoinCollect()
	{
		if (SecureStorage.Instance.GetItemCount("upgrade.jaguar") >= 2)
		{
			return true;
		}
		return false;
	}

	public static float HasteTonicDuration()
	{
		return mHasteTonicUpgrades.Lookup(SecureStorage.Instance.GetItemCount("upgrade.speedincrease"));
	}
}
