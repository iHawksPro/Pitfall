using System.Collections;
using UnityEngine;

public class OutfitItem : ConsumableItem
{
	public GameObject m_OutfitOfTheDay;

	public SpriteText m_ownedText;

	public GameObject m_locked;

	public SpriteText m_lockedText;

	public static readonly string PirateIdentifier = "outfit.pirate";

	public static readonly string NinjaIdentifier = "outfit.ninja";

	public static readonly string SuperIdentifier = "outfit.super";

	public override void PopulateItem(BaseCampProducts.ProductData Data)
	{
		base.PopulateItem(Data);
		UpdateOwnedIndicator();
	}

	public override void OnItemBought()
	{
		base.OnItemBought();
		UpdateOwnedIndicator();
		if (SecureStorage.Instance.NinjaPirateStatus == 0)
		{
			if (m_productData.Identifier == NinjaIdentifier)
			{
				SecureStorage.Instance.NinjaPirateStatus = 1;
			}
			else if (m_productData.Identifier == PirateIdentifier)
			{
				SecureStorage.Instance.NinjaPirateStatus = 2;
			}
		}
	}

	private void UpdateOwnedIndicator()
	{
	}

	private void SetCostState(bool newActive)
	{
		if (m_Cost.active != newActive)
		{
			m_Cost.SetActiveRecursively(newActive);
		}
	}

	private void SetWornState(bool newActive)
	{
		if (m_Worn.active != newActive)
		{
			m_Worn.SetActiveRecursively(newActive);
		}
	}

	private void SetLockedState(bool newActive)
	{
		if (m_locked.active != newActive)
		{
			m_locked.SetActiveRecursively(newActive);
		}
	}

	public override void LateUpdate()
	{
		if (m_productData.HasZeroValue(m_Level))
		{
			SetCostState(false);
		}
		if (m_productData.Identifier == SuperIdentifier)
		{
			SetCostState(false);
			if (IsWornOutfit())
			{
				SetWornState(true);
				SetLockedState(false);
			}
			else
			{
				SetWornState(false);
				SetLockedState(true);
				if (IsOutfitLocked())
				{
					string text = Language.Get("S_SUPERHARRY_LOCKED");
					if (m_lockedText.Text != string.Empty)
					{
						m_lockedText.Text = string.Empty;
					}
					if (m_title.Text != text)
					{
						m_title.Text = text;
					}
				}
				else
				{
					string text2 = Language.Get("S_SUPERHARRY_UNLOCKED");
					string title = m_productData.GetTitle();
					if (m_title.Text != title)
					{
						m_title.Text = title;
					}
					if (m_lockedText.Text != text2)
					{
						m_lockedText.Text = text2;
					}
				}
			}
		}
		else
		{
			SetLockedState(false);
			if (CanOutfitBeWorn())
			{
				if (m_Cost.active)
				{
					m_Cost.SetActiveRecursively(false);
				}
				if (IsWornOutfit())
				{
					if (!m_Worn.active)
					{
						m_Worn.SetActiveRecursively(true);
					}
				}
				else if (m_Worn.active)
				{
					m_Worn.SetActiveRecursively(false);
				}
			}
			else if (m_Worn.active)
			{
				m_Worn.SetActiveRecursively(false);
			}
		}
		UpdatePopular();
		UpdateSaleItem();
		UpdateOOTD();
	}

	private void UpdateOOTD()
	{
		if (!m_OutfitOfTheDay)
		{
			return;
		}
		bool flag = OutfitOfTheDayManager.Instance.IsOOTD(m_productData.Identifier);
		m_OutfitOfTheDay.SetActiveRecursively(flag);
		if (flag)
		{
			if (m_Popular != null)
			{
				m_Popular.SetActiveRecursively(false);
			}
			if (m_Sale != null)
			{
				m_Sale.SetActiveRecursively(false);
			}
		}
	}

	protected override void OnItemPressed()
	{
		Debug.Log("OUTFIT Pressed " + m_productData.Identifier);
		if (!BaseCampProductItem.m_BuyDialogActive)
		{
			BaseCampProductItem.m_BuyDialogActive = true;
			StartCoroutine(HandleItemPressedWithCostumeChange());
		}
	}

	private IEnumerator HandleItemPressedWithCostumeChange()
	{
		bool shouldDisplayDialog = true;
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT && IsWornOutfit())
		{
			shouldDisplayDialog = false;
		}
		Costume newCostume = SecureStorage.Instance.TranslateCostumeType(m_productData.Identifier);
		UIMenuBackground.Instance.m_harry.ChangeCostume(newCostume);
		if (shouldDisplayDialog)
		{
			CommonAnimations.AnimateButton(m_ItemBtn.gameObject);
			yield return StartCoroutine(BuyItemDialog());
		}
		BaseCampProductItem.m_BuyDialogActive = false;
	}

	protected override void OnPurchaseCancelled()
	{
		ChangeToWornCostume();
		base.OnPurchaseCancelled();
	}

	private void ChangeToWornCostume()
	{
		if (m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT)
		{
			Costume currentCostumeType = SecureStorage.Instance.GetCurrentCostumeType();
			UIMenuBackground.Instance.m_harry.ChangeCostume(currentCostumeType);
		}
	}

	private bool CanOutfitBeWorn()
	{
		int itemCount = SecureStorage.Instance.GetItemCount(m_productData.Identifier);
		return m_productData.Category == BaseCampProducts.StoreCategory.BCP_OUTFIT && (itemCount > 0 || m_productData.HasZeroValue(m_Level));
	}

	private bool IsWornOutfit()
	{
		return SecureStorage.Instance.GetCurrentCostumeType() == SecureStorage.Instance.TranslateCostumeType(m_productData.Identifier);
	}

	private bool IsOutfitLocked()
	{
		bool result = false;
		if (m_productData.Identifier == SuperIdentifier)
		{
			result = !TrialsDataManager.Instance.HaveCollectedAllRelics;
		}
		return result;
	}
}
