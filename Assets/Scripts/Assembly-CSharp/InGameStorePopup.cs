using System.Collections;
using UnityEngine;

public class InGameStorePopup : MonoBehaviour
{
	public static InGameStorePopup Instance;

	public SpriteText AdditionalAmountText;

	public UIButton CancelButton;

	public GameObject ParentNode;

	private void Awake()
	{
		Instance = this;
		DailyDoubleController.Instance.SetBottomTicker(true);
	}

	public void SetAdditionalAmount(int Amount, bool HighlightPulse)
	{
		if (Amount != 1)
		{
			AdditionalAmountText.Text = string.Format(Language.Get("S_ADDITIONAL_AMOUNT_STORE"), Amount);
		}
		else
		{
			AdditionalAmountText.Text = Language.Get("S_ADDITIONAL_AMOUNT_STORE_SOLO");
		}
		if (HighlightPulse)
		{
			StartCoroutine(PulseAndHighlightAmount());
		}
	}

	private IEnumerator PulseAndHighlightAmount()
	{
		float ScaleFactor = 1.25f;
		float PulseTime = 0.4f;
		Color CurrColor = AdditionalAmountText.color;
		float CurrScale = AdditionalAmountText.gameObject.transform.localScale.x;
		float newScale = ScaleFactor * CurrScale;
		MenuSFX.Instance.Play2D("FlashAdditionalGemsNeeded");
		AdditionalAmountText.gameObject.ScaleTo(new Vector3(newScale, newScale, newScale), PulseTime / 2f, 0f);
		yield return new WaitForSeconds(PulseTime / 2f);
		AdditionalAmountText.gameObject.ScaleTo(new Vector3(CurrScale, CurrScale, CurrScale), PulseTime / 2f, 0f);
	}

	public void HideCancelButton()
	{
		CancelButton.gameObject.active = false;
	}

	public void ShowCancelButton()
	{
		CancelButton.gameObject.active = true;
	}

	public void OnCancelPressed()
	{
		MonoBehaviour.print("IN-GAME STORE CANCEL BUTTON PRESSED " + Time.time);
		MenuSFX.Instance.Play2D("MenuCancel");
		StartCoroutine(HideForCancel(true));
	}

	public void ExitInGameStore()
	{
		StartCoroutine(HideForCancel(false));
	}

	private IEnumerator HideForCancel(bool redisplay)
	{
		float speed = 0.2f;
		base.gameObject.ScaleTo(Vector3.zero, speed, 0f, EaseType.easeOutCubic);
		yield return new WaitForSeconds(speed);
		DailyDoubleController.Instance.SetBottomTicker(false);
		ParentNode.SetActiveRecursively(false);
		HideCancelButton();
		if (redisplay)
		{
			StartCoroutine(DiamondsToContinueDialog.Instance.Display(false));
		}
	}
}
