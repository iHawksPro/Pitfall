using System.Collections;
using UnityEngine;

public class OutfitOfTheDayMatchDialog : MonoBehaviour
{
	public SpriteText m_matchText;

	public SpriteText m_bonusText;

	public SpriteRoot m_outfitIcon;

	public GameObject m_visibleContents;

	public GameObject m_onscreenPos;

	public GameObject m_offscreenPos;

	private void Awake()
	{
		m_visibleContents.transform.position = m_offscreenPos.transform.position;
	}

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

	private void SetupDialog()
	{
		OutfitOfTheDayManager instance = OutfitOfTheDayManager.Instance;
		m_outfitIcon.GetComponent<Renderer>().material.mainTexture = instance.OotdTexture;
		Costume currentCostumeType = SecureStorage.Instance.GetCurrentCostumeType();
		string empty = string.Empty;
		string text = string.Empty;
		if (instance.IsOOTD(currentCostumeType))
		{
			empty = string.Format("{0}: {1}", Language.Get("S_OOTD_TITLE"), Language.Get("S_OOTD_MATCH"));
			text = instance.OotdBonusText;
		}
		else
		{
			empty = string.Format("{0}: {1}", Language.Get("S_OOTD_TITLE"), Language.Get("S_OOTD_NOMATCH"));
		}
		m_matchText.Text = empty;
		m_bonusText.Text = text;
	}

	public IEnumerator Display(float Timeout)
	{
		SetupDialog();
		yield return new WaitForSeconds(0.33f);
		if (m_visibleContents != null && m_visibleContents.gameObject != null)
		{
			Costume playerCostume = SecureStorage.Instance.GetCurrentCostumeType();
			if (OutfitOfTheDayManager.Instance.IsOOTD(playerCostume))
			{
				MenuSFX.Instance.Play2D("OOTDMatch");
			}
			else
			{
				MenuSFX.Instance.Play2D("OOTDNoMatch");
			}
			m_visibleContents.gameObject.MoveTo(m_onscreenPos.transform.position, 0.33f, 0f, EaseType.easeInOutCubic);
		}
		yield return new WaitForSeconds(Timeout + 0.33f);
		if (m_visibleContents != null && m_visibleContents.gameObject != null)
		{
			m_visibleContents.gameObject.MoveTo(m_offscreenPos.transform.position, 0.33f, 0f, EaseType.easeInOutCubic);
		}
		yield return new WaitForSeconds(0.33f);
		if ((bool)this && (bool)base.gameObject)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
