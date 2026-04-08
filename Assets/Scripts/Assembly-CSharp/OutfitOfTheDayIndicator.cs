using UnityEngine;

public class OutfitOfTheDayIndicator : MonoBehaviour
{
	public UIButton m_button;

	public SpriteRoot m_outfitIcon;

	public NewIndicator m_newIndicator;

	private Costume m_currentOutfit;

	private bool OutfitHasChanged
	{
		get
		{
			Costume oOTD = OutfitOfTheDayManager.Instance.OOTD;
			return oOTD != m_currentOutfit;
		}
	}

	private void Awake()
	{
		m_currentOutfit = Costume.None;
	}

	private void OnEnable()
	{
		UpdateOutfit();
	}

	private void UpdateOutfit()
	{
		Costume oOTD = OutfitOfTheDayManager.Instance.OOTD;
		if (oOTD != m_currentOutfit)
		{
			Texture ootdTexture = OutfitOfTheDayManager.Instance.OotdTexture;
			if (ootdTexture != null)
			{
				m_outfitIcon.GetComponent<Renderer>().material.mainTexture = ootdTexture;
			}
			m_currentOutfit = oOTD;
			m_newIndicator.IsNew = true;
		}
	}

	private void OnButtonPressed()
	{
		if (!DialogManager.DialogActive)
		{
			m_newIndicator.IsNew = false;
			DialogManager.Instance.LaunchOOTDDialog();
		}
	}
}
