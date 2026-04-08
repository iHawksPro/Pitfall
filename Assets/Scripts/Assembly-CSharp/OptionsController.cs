using System.Collections;
using UnityEngine;

public class OptionsController : StateController
{
	public SpriteText m_coinsText;

	public SpriteText m_gemsText;

	public UIStateToggleBtn m_sfxButton;

	public UIStateToggleBtn m_musicButton;

	public UIStateToggleBtn m_graphicsButton;

	public SpriteText m_versionText;

	public SpriteText m_leftyText;

	public ConfirmDialog m_lowGFXDialogPrefab;

	private bool m_leavingState;

	private bool m_waitingForRestore;

	private bool m_restoreSuccess;

	protected override void OnStateActivate(string OldStateName)
	{
		base.OnStateActivate(OldStateName);
		m_gemsText.Text = string.Format(" {0}", SecureStorage.Instance.GetGems());
		m_coinsText.Text = string.Format(" {0}", SecureStorage.Instance.GetCoins());
		m_sfxButton.SetToggleState((!SoundManager.Instance.IsMuted) ? "On" : "Off");
		m_musicButton.SetToggleState((!MusicManager.Instance.IsMuted) ? "On" : "Off");
		m_graphicsButton.SetToggleState((!SecureStorage.Instance.HasSetLowGFXOption) ? "Off" : "On");
		if (m_versionText != null)
		{
			m_versionText.Text = TBFUtils.BuildVersion;
		}
		UpdateLeftyText();
		m_leavingState = false;
	}

	public void OnCreditsPressed()
	{
		SwrveEventsUI.CreditsTouched();
		StateManager.Instance.LoadAndActivateState("Credits");
	}

	public void OnPrivacyPolicyPressed()
	{
		StateManager.Instance.LoadAndActivateState("PrivacyPolicy");
	}

	public void OnHelpPressed()
	{
		SwrveEventsUI.HelpTouched();
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			EtceteraPlatformWrapper.ShowAlert("S_IAP_NO_NET_TITLE", "S_GENERAL_NO_NET_BODY", "S_OK");
		}
		else
		{
			EtceteraPlatformWrapper.ShowWebPage(SwrveServerVariables.Instance.FaqURI);
		}
	}

	public void OnTutorialPressed()
	{
		SwrveEventsUI.TutorialReplay();
		TitleController.m_pendingTutorialLaunch = true;
		StateManager.Instance.LoadAndActivateState("Title");
		SecureStorage.Instance.TutorialViewed = false;
		MenuSFX.Instance.Play2D("MenuConfirm");
	}

	private void UpdateLeftyText()
	{
		m_leftyText.Text = ((!SecureStorage.Instance.LeftyControls) ? Language.Get("S_LEFT_HAND_OFF") : Language.Get("S_LEFT_HAND_ON"));
	}

	public void OnLeftHandedPressed()
	{
		bool leftyControls = SecureStorage.Instance.LeftyControls;
		SecureStorage.Instance.LeftyControls = !leftyControls;
		UpdateLeftyText();
	}

	public void OnMusicPressed()
	{
		bool isMuted = MusicManager.Instance.IsMuted;
		MusicManager.Instance.IsMuted = !isMuted;
		if (MusicManager.Instance.IsMuted)
		{
			SwrveEventsUI.MusicOff();
		}
		else
		{
			SwrveEventsUI.MusicOn();
		}
	}

	public void OnSFXPressed()
	{
		bool isMuted = SoundManager.Instance.IsMuted;
		SoundManager.Instance.IsMuted = !isMuted;
		if (SoundManager.Instance.IsMuted)
		{
			SwrveEventsUI.SfxOff();
		}
		else
		{
			SwrveEventsUI.SfxOn();
		}
	}

	public void OnLowGFXPressed()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		if (!SecureStorage.Instance.HasSetLowGFXOption)
		{
			if (m_graphicsButton.StateName == "Confirm")
			{
				ConfirmDialog confirmDialog = Object.Instantiate(m_lowGFXDialogPrefab) as ConfirmDialog;
				string title = Language.Get("S_COMPATIBILITY_MODE");
				string message = Language.Get("S_COMPATIBILITY_MODE_BODY");
				string opt = Language.Get("S_OK");
				StartCoroutine(confirmDialog.Display(title, message, opt, null, OnLowGFXConfirm, null, OnLowGFXCancelled));
			}
			else
			{
				SwrveEventsUI.LowGFXOptionOn();
				SecureStorage.Instance.HasSetLowGFXOption = true;
			}
		}
		else
		{
			SwrveEventsUI.LowGFXOptionOff();
			SecureStorage.Instance.HasSetLowGFXOption = false;
		}
	}

	public void OnLowGFXConfirm()
	{
	}

	public void OnLowGFXCancelled()
	{
	}

	public void OnBuyStuff()
	{
		if (!m_leavingState)
		{
			BaseCampController.Instance.LaunchWithExchangeFocus();
			m_leavingState = true;
		}
	}

	public override void OnBackPressed()
	{
		if (!m_leavingState)
		{
			base.OnBackPressed();
			m_leavingState = true;
		}
		else
		{
			MenuSFX.Instance.Play2D("MenuCancel");
		}
	}

	public void BlastFurnaceTouched()
	{
		SwrveEventsUI.CreditsTouched();
		TBFUtils.LaunchURL("http://www.activision.com");
	}

	public void OnRestorePurchasesPressed()
	{
		Debug.Log("Restore Purchases");
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			BaseCampController.Instance.DelayedWifiErrorMessage();
		}
		else if (!StoreProductManager.Instance.CanBuyExchangeProducts)
		{
			BaseCampController.Instance.DelayedRestrictionMessage();
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			StartCoroutine("RestorePreviousPurchases");
		}
	}

	private void ShowAlertDialog(string TitleKey, string BodyKey, string ButtonKey)
	{
		string strTitle = Language.Get(TitleKey);
		string strBody = Language.Get(BodyKey);
		string strOK = Language.Get(ButtonKey);
		EtceteraPlatformWrapper.ShowAlert(strTitle, strBody, strOK);
	}

	private void PurchasesRestoredDialog()
	{
		ShowAlertDialog("S_RESTORE_PURCHASES", "S_IAP_PURCHASES_RESTORED", "S_IAP_OK");
	}

	private void RestoreFailedDialog()
	{
		ShowAlertDialog("S_RESTORE_PURCHASES", "S_IAP_PURCHASE_RESTORE_FAILED", "S_IAP_OK");
	}

	private IEnumerator RestorePreviousPurchases()
	{
		EtceteraPlatformWrapper.ShowWaitingWithLabel(Language.Get("S_IAP_CONNECTING_DROID"));
		UIManager.instance.LockInput();
		m_waitingForRestore = true;
		m_restoreSuccess = false;
		StoreProductManager.OnBuyExchangeSuccess += OnPurchaseRestored;
		StoreProductManager.OnRestorePurchasesFailed += OnRestoreFailed;
		StoreProductManager.OnRestorePurchasesFinished += OnRestoreFinished;
		StoreProductManager.Instance.RestorePreviousPurchases();
		while (m_waitingForRestore)
		{
			yield return null;
		}
		EtceteraPlatformWrapper.HideWaitingDialog();
		UIManager.instance.UnlockInput();
		StoreProductManager.OnBuyExchangeSuccess -= OnPurchaseRestored;
		StoreProductManager.OnRestorePurchasesFailed -= OnRestoreFailed;
		StoreProductManager.OnRestorePurchasesFinished -= OnRestoreFinished;
		if (m_restoreSuccess)
		{
			InvokeHelper.InvokeSafe(PurchasesRestoredDialog, 0.25f, this);
		}
		else
		{
			InvokeHelper.InvokeSafe(RestoreFailedDialog, 0.25f, this);
		}
	}

	private void OnPurchaseRestored(string Id)
	{
		if (Id == StoreProductManager.TreasureUpgradeIdentifier)
		{
			Debug.Log("Restored treasure upgrade");
			SecureStorage.Instance.SetItemCount(StoreProductManager.TreasureUpgradeIdentifier, 1);
		}
	}

	private void OnRestoreFinished()
	{
		m_waitingForRestore = false;
		m_restoreSuccess = true;
	}

	private void OnRestoreFailed(string error)
	{
		m_waitingForRestore = false;
		m_restoreSuccess = false;
	}
}
