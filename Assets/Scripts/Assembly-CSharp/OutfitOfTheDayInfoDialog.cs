using System.Collections;
using UnityEngine;

public class OutfitOfTheDayInfoDialog : MonoBehaviour
{
	public SpriteText m_titleText;

	public SpriteText m_matchingText;

	public SpriteRoot m_outfitIcon;

	public SpriteText m_bonusText;

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
		m_titleText.Text = Language.Get("S_OOTD_TITLE");
		OutfitOfTheDayManager ootdManager = OutfitOfTheDayManager.Instance;
		Costume playerCostume = SecureStorage.Instance.GetCurrentCostumeType();
		if (ootdManager.IsOOTD(playerCostume))
		{
			m_matchingText.Text = Language.Get("S_OOTD_WEARING_MSG");
			m_button.Text = Language.Get("S_OK");
		}
		else
		{
			string noMatchMessage = string.Format(arg0: ootdManager.OotdProduct.GetTitle(), format: Language.Get("S_OOTD_NOT_WEARING_MSG"));
			m_matchingText.Text = noMatchMessage;
			m_button.Text = Language.Get("S_SHOW_ME");
		}
		if (m_outfitIcon != null)
		{
			m_outfitIcon.GetComponent<Renderer>().material.mainTexture = ootdManager.OotdTexture;
		}
		m_bonusText.Text = ootdManager.OotdBonusText;
		m_visibleContents.ScaleFrom(Vector3.zero, 0.33f, 0f, EaseType.spring);
		while (!m_userConfirmed.HasValue && !m_userCancelled)
		{
			yield return new WaitForEndOfFrame();
		}
		m_visibleContents.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		Object.Destroy(base.gameObject);
		if (!m_userCancelled && !ootdManager.IsOOTD(playerCostume) && BaseCampController.Instance != null)
		{
			BaseCampController.Instance.LaunchWithProductFocusImmediate(ootdManager.OotdProduct);
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
		SwrveEventsUI.MessageFollowed("S_OOTD_TITLE");
		m_userConfirmed = true;
	}

	private void OnCancelPressed()
	{
		SwrveEventsUI.MessageIgnored("S_OOTD_TITLE");
		m_userCancelled = true;
	}
}
