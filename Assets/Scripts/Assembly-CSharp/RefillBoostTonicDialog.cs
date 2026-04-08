using System;
using System.Collections;
using UnityEngine;

public class RefillBoostTonicDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public UIButton m_GemsButton;

	public UIButton m_CoinsButton;

	public UIButton m_cancelButton;

	public GameObject m_visibleContents;

	public SpriteText m_ownedCount;

	public GameObject m_Icon;

	public GameObject m_Owned;

	public GameObject m_CoinsWhole;

	public GameObject m_GemsWhole;

	private Action m_OnPurchaseConfirmed;

	private Action m_OnPurchaseCancelled;

	private bool m_userCancelled;

	private BaseCampProducts.ProductData m_ProductData;

	private void Awake()
	{
		m_ProductData = StoreProductManager.Instance.FindProduct("tonic.refill");
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		MenuSFX.Instance.Play2D("MenuPopup");
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public IEnumerator Display(Action OnPurchaseConfirmed, Action OnPurchaseCancelled)
	{
		m_OnPurchaseConfirmed = OnPurchaseConfirmed;
		m_OnPurchaseCancelled = OnPurchaseCancelled;
		yield return StartCoroutine(DoDisplay());
	}

	public IEnumerator DoDisplay()
	{
		DialogManager.DialogLock();
		SwrveEventsPurchase.ProductViewed(m_ProductData.Identifier, 0);
		UpdateContents();
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		SwrveEventsPurchase.ProductClosed(m_ProductData.Identifier, 0);
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		UnityEngine.Object.Destroy(base.gameObject);
		DialogManager.DialogUnlock();
	}

	private void UpdateContents()
	{
		m_titleText.Text = Language.Get("S_BURST_REFILL");
		long num = TrialsDataManager.Instance.TimeUntilNextBoostRefill;
		long num2 = num % 60;
		string text = (num / 60).ToString("00") + ":" + num2.ToString("00");
		m_bodyText.Text = Language.Get("S_BURST_BODY_1") + "\n\n[#FFA11B]" + text + "\n\n[#FFFFFF]" + Language.Get("S_BURST_BODY_2");
		m_userCancelled = false;
		m_GemsButton.Text = "    " + m_ProductData.GemsPrice(0);
		m_CoinsButton.Text = "    " + m_ProductData.CoinPrice(0);
		if (!m_ProductData.CanPlayerAffordItem(false, 0))
		{
			m_CoinsButton.spriteText.SetColor(new Color(0.7f, 0f, 0f, 1f));
		}
		else
		{
			m_CoinsButton.spriteText.SetColor(new Color(1f, 1f, 1f, 1f));
		}
		if (m_ProductData.HasZeroValue(0))
		{
			m_GemsWhole.SetActiveRecursively(false);
			m_CoinsWhole.gameObject.SetActiveRecursively(false);
		}
		else if (m_ProductData.CoinPrice(0) == 0)
		{
			m_GemsWhole.transform.localPosition = new Vector3(90f, m_GemsWhole.transform.localPosition.y, m_GemsWhole.transform.localPosition.z);
			m_CoinsWhole.gameObject.SetActiveRecursively(false);
		}
	}

	private void Update()
	{
		long num = TrialsDataManager.Instance.TimeUntilNextBoostRefill;
		long num2 = num % 60;
		string text = (num / 60).ToString("00") + ":" + num2.ToString("00");
		m_bodyText.Text = Language.Get("S_BURST_BODY_1") + "\n\n[#FFA11B]" + text + "\n\n[#FFFFFF]" + Language.Get("S_BURST_BODY_2");
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnCancelPressed();
		}
	}

	private bool CanPlayerBuyItem()
	{
		int itemCount = SecureStorage.Instance.GetItemCount(m_ProductData.Identifier);
		int maxBoostsAvailable = TrialsDataManager.Instance.MaxBoostsAvailable;
		return itemCount < maxBoostsAvailable;
	}

	private void DoBuyItem(bool UsingGems)
	{
		if (CanPlayerBuyItem())
		{
			BaseCampController.Instance.BuyItem(m_ProductData, 0, UsingGems, m_OnPurchaseConfirmed, OnCancelPressed);
			SwrveEventsPurchase.TrialsBoostRefilled(UsingGems);
		}
	}

	private void OnGemsPressed()
	{
		DoBuyItem(true);
		UpdateContents();
		MenuSFX.Instance.Play2D("MenuBuy1");
		m_userCancelled = true;
	}

	private void OnCoinsPressed()
	{
		DoBuyItem(false);
		UpdateContents();
		MenuSFX.Instance.Play2D("MenuBuy1");
		m_userCancelled = true;
	}

	private void OnCancelPressed()
	{
		if (!BaseCampController.Instance.TutorialActive())
		{
			m_userCancelled = true;
			MenuSFX.Instance.Play2D("MenuCancel");
			m_OnPurchaseCancelled();
		}
	}
}
