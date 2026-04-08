using System;
using UnityEngine;

public class DiamondsOrAdvertsToContinueDialog : DiamondsToContinueDialog
{
	private const int kSecondsPerDay = 86400;

	private readonly DateTime kUTCStart = new DateTime(1970, 1, 1);

	private void Start()
	{
		VungleWrapper instance = VungleWrapper.Instance;
		instance.ClosingVungleAd = (Action)Delegate.Combine(instance.ClosingVungleAd, new Action(OnClosingVungleAd));
		Debug.Log("DTC_ADS: Registering for Vungle ad closure");
		Localisation_EnoughDiamondsSolo = "S_DIAMONDS_TO_CONTINUE_OR_ADVERT_ENOUGH_SOLO";
		Localisation_EnoughDiamonds = "S_DIAMONDS_TO_CONTINUE_OR_ADVERT_ENOUGH";
		Localisation_NotEnoughDiamondsSolo = "S_DIAMONDS_TO_CONTINUE_OR_ADVERT_NOTENOUGH_SOLO";
		Localisation_NotEnoughDiamonds = "S_DIAMONDS_TO_CONTINUE_OR_ADVERT_NOTENOUGH";
	}

	private void OnDestroy()
	{
		VungleWrapper instance = VungleWrapper.Instance;
		instance.ClosingVungleAd = (Action)Delegate.Remove(instance.ClosingVungleAd, new Action(OnClosingVungleAd));
		Debug.Log("DTC_ADS: Unregistering from Vungle ad closure");
	}

	private void OnClosingVungleAd()
	{
		Debug.Log("DTC_ADS: Vungle ad closed, continuing with game");
		if (m_allowbuttons)
		{
			MonoBehaviour.print("CONTINUE BUTTON PRESSED " + Time.time);
			StartTheExit(true);
			GameController.Instance.ContinueFromAdvert();
			MenuSFX.Instance.Play2D("MenuConfirm");
			SecureStorage.Instance.DiamondsTutorialViewed = true;
			Debug.Log("DTC_ADS: Resetting the countdown");
			GameController.Instance.DiamondsToContinueCountdownIsRunning = true;
		}
	}

	public void OnWatchAdvertPressed()
	{
		Debug.Log("DTC_ADS: OnWatchAdvert pressed");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			EtceteraPlatformWrapper.ShowAlert(Language.Get("S_IAP_NO_NET_TITLE"), Language.Get("S_GENERAL_NO_NET_BODY"), Language.Get("S_IAP_OK"));
			Debug.LogWarning("DTC_ADS: No internet connection");
		}
		else if ((bool)VungleWrapper.Instance && VungleWrapper.Instance.AdIsAvailable)
		{
			Debug.Log("DTC_ADS: Pausing the countdown");
			GameController.Instance.DiamondsToContinueCountdownIsRunning = false;
			if (VungleWrapper.Instance.ShowAd())
			{
				double totalSeconds = SynchronisedClock.Instance.ServerTime.Subtract(kUTCStart).TotalSeconds;
				SecureStorage.Instance.RecordVungleAdWatch(totalSeconds.ToString());
				int numberOfAdvertsWatchedToday = GameController.Instance.GetNumberOfAdvertsWatchedToday();
				GameController.Instance.VungleAdsWatchedToday = numberOfAdvertsWatchedToday;
				Debug.Log("DTC_ADS: Vungle ads watched today: " + numberOfAdvertsWatchedToday);
			}
		}
	}
}
