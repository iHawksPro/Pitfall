using System.Collections;
using UnityEngine;

public class NewSaleDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteRoot m_itemIcon;

	public SpriteText m_salesText;

	public UIButton m_button;

	public GameObject m_visibleContents;

	private bool? m_userConfirmed;

	private bool m_userCancelled;

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
		Object.Destroy(base.gameObject);
	}

	public IEnumerator Display()
	{
		DialogManager.DialogLock();
		m_userCancelled = false;
		m_titleText.Text = Language.Get("S_SALES_OFFER_T");
		m_button.Text = Language.Get("S_SHOW_ME");
		BaseCampProducts.ProductData saleItem = null;
		if (StoreProductManager.Instance.IsSaleActive)
		{
			saleItem = StoreProductManager.Instance.SaleItem;
		}
		if (saleItem != null)
		{
			if (m_itemIcon != null)
			{
				m_itemIcon.GetComponent<Renderer>().material.mainTexture = StoreProductManager.LoadTextureFor(saleItem.Identifier, 1);
			}
			string saleText;
			switch (saleItem.Category)
			{
			case BaseCampProducts.StoreCategory.BCP_CONSUMABLE:
				saleText = Language.Get("S_SALES_OFFER_CONSUMABLE");
				break;
			case BaseCampProducts.StoreCategory.BCP_OUTFIT:
				saleText = Language.Get("S_SALES_OFFER_OUTFIT");
				break;
			case BaseCampProducts.StoreCategory.BCP_UPGRADE:
				saleText = Language.Get("S_SALES_OFFER_UPGRADE");
				break;
			default:
				saleText = string.Empty;
				break;
			}
			m_salesText.Text = string.Format(saleText, saleItem.GetTitle());
		}
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userConfirmed.HasValue && !m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		Object.Destroy(base.gameObject);
		if (!m_userCancelled && saleItem != null && BaseCampController.Instance != null)
		{
			BaseCampController.Instance.LaunchWithProductFocus(saleItem);
		}
		DialogManager.DialogUnlock();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnCancelPressed();
		}
	}

	private void OnButtonPressed()
	{
		SwrveEventsUI.MessageFollowed("S_SALES_OFFER_T");
		m_userConfirmed = true;
	}

	private void OnCancelPressed()
	{
		SwrveEventsUI.MessageIgnored("S_SALES_OFFER_T");
		m_userCancelled = true;
	}
}
