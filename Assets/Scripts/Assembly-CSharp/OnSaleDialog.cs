using UnityEngine;

public class OnSaleDialog : MonoBehaviour
{
	public GameObject m_visibleContents;

	public SpriteText m_saleTimeLeft;

	public SpriteRoot m_productIcon;

	public GameObject m_offscreenPos;

	public SpriteText m_saleAmount;

	private bool m_isShowing;

	private uint m_lastTimeLeft;

	private void Awake()
	{
		m_visibleContents.transform.position = m_offscreenPos.transform.position;
		m_isShowing = false;
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		StoreProductManager.OnProductsUpdated += HandleProductUpdate;
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
		StoreProductManager.OnProductsUpdated -= HandleProductUpdate;
	}

	private void HandleProductUpdate()
	{
		if (StoreProductManager.Instance != null)
		{
			if (StoreProductManager.Instance.IsSaleActive)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	private void HandleStateDeactivated(string StateName)
	{
		Hide();
	}

	public void ShowItemsOnSale()
	{
		HandleProductUpdate();
	}

	private void Show()
	{
		if (m_isShowing)
		{
			return;
		}
		BaseCampProducts.ProductData saleItem = StoreProductManager.Instance.SaleItem;
		if (saleItem != null)
		{
			if (m_productIcon.GetComponent<Renderer>().material.mainTexture != null && m_productIcon.GetComponent<Renderer>().material.mainTexture.name != StoreProductManager.GetResourceIDFor(saleItem.Identifier, 1))
			{
				m_productIcon.GetComponent<Renderer>().material.mainTexture = StoreProductManager.LoadTextureFor(saleItem.Identifier, 1);
			}
			m_saleAmount.Text = string.Format(Language.Get("S_SALES_REDUCED"), (int)StoreProductManager.Instance.SalePercentOff);
			m_lastTimeLeft = 0u;
			InvokeRepeating("UpdateTimeLeftText", 0f, 1f);
			MenuSFX.Instance.Play2D("MenuBoxSwoosh");
			iTween.MoveTo(m_visibleContents, iTween.Hash("islocal", true, "position", Vector3.zero, "time", 0.5f));
			m_isShowing = true;
		}
	}

	public void Hide()
	{
		if (m_isShowing)
		{
			iTween.MoveTo(m_visibleContents, iTween.Hash("position", m_offscreenPos.transform.position, "time", 0.5f));
			MenuSFX.Instance.Play2D("MenuBoxSwoosh");
			CancelInvoke("UpdateTimeLeftText");
			m_isShowing = false;
		}
	}

	private void UpdateTimeLeftText()
	{
		if (!(StoreProductManager.Instance != null))
		{
			return;
		}
		if (StoreProductManager.Instance.IsSaleActive)
		{
			uint saleTimeLeft = StoreProductManager.Instance.SaleTimeLeft;
			if (saleTimeLeft >= 1 && saleTimeLeft <= 10 && saleTimeLeft != m_lastTimeLeft)
			{
				MenuSFX.Instance.Play2D("SaleCountdown");
			}
			m_lastTimeLeft = saleTimeLeft;
			string text = "[#FFFFFF]";
			if (saleTimeLeft < TimeUtils.SecondsPerHour)
			{
				text = " [#FFA11B]";
			}
			string shortFuzzyTimeStringFromSeconds = TimeUtils.GetShortFuzzyTimeStringFromSeconds(saleTimeLeft);
			shortFuzzyTimeStringFromSeconds = text + shortFuzzyTimeStringFromSeconds + "[#FFFFFF]";
			string text2 = string.Format(Language.Get("S_TIME_TO_GO"), shortFuzzyTimeStringFromSeconds);
			if (text2 != m_saleTimeLeft.Text)
			{
				m_saleTimeLeft.Text = text2;
			}
		}
		else
		{
			Hide();
		}
	}

	private void OnDialogPressed()
	{
		if (BaseCampController.Instance != null)
		{
			UIManager.instance.blockInput = true;
			BaseCampController.Instance.LaunchWithProductFocus(StoreProductManager.Instance.SaleItem);
		}
	}
}
