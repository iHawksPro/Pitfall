using System.Collections;
using UnityEngine;

public class DiamondsToContinueDialog : MonoBehaviour
{
	public GameObject m_visibleContents;

	public SpriteText Number;

	public SpriteText m_messageBody;

	[HideInInspector]
	public int TotalGemsRequiredForContinue;

	[HideInInspector]
	public int AdditionalGemsRequiredForContinue;

	public GameObject ContinueButton;

	public GameObject GetDiamondsButton;

	public GameObject InGameStorePrefab;

	[HideInInspector]
	public GameObject InGameStore;

	public GameObject InputBlocker;

	public static DiamondsToContinueDialog Instance;

	public UIButton CancelButton;

	public SpriteText DiamondCountValue;

	public SpriteText CostIncreasesText;

	[HideInInspector]
	public bool ToGemStore;

	[HideInInspector]
	public bool CountDownEnabled;

	protected bool m_allowbuttons;

	protected string Localisation_EnoughDiamondsSolo = "S_DIAMONDS_TO_CONTINUE_ENOUGH_SOLO";

	protected string Localisation_EnoughDiamonds = "S_DIAMONDS_TO_CONTINUE_ENOUGH";

	protected string Localisation_NotEnoughDiamondsSolo = "S_DIAMONDS_TO_CONTINUE_NOTENOUGH_SOLO";

	protected string Localisation_NotEnoughDiamonds = "S_DIAMONDS_TO_CONTINUE_NOTENOUGH";

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		StateManager.stateDeactivated += HandleStateDeactivated;
		Debug.Log("DTC: Resetting the countdown");
		GameController.Instance.DiamondsToContinueCountdownIsRunning = true;
	}

	private void OnDisable()
	{
		StateManager.stateDeactivated -= HandleStateDeactivated;
	}

	private void HandleStateDeactivated(string StateName)
	{
	}

	public IEnumerator Display(bool ShowCountDown)
	{
		if (!SecureStorage.Instance.DiamondsTutorialViewed)
		{
			ShowCountDown = false;
		}
		CountDownEnabled = ShowCountDown;
		while (m_visibleContents == null)
		{
			yield return null;
		}
		m_visibleContents.SetActiveRecursively(false);
		yield return new WaitForSeconds(0.1f);
		m_allowbuttons = true;
		Debug.Log("(ewh) AdditionalGemsRequiredForContinue: " + AdditionalGemsRequiredForContinue);
		m_visibleContents.SetActiveRecursively(true);
		if (ShowCountDown && SecureStorage.Instance.DiamondsTutorialViewed)
		{
			if (!Number.gameObject.active)
			{
				Number.gameObject.SetActive(true);
			}
			if (CancelButton.gameObject.active)
			{
				CancelButton.gameObject.active = false;
			}
			if (CostIncreasesText.gameObject.active)
			{
				CostIncreasesText.gameObject.active = false;
			}
		}
		else
		{
			if (Number.gameObject.active)
			{
				Number.gameObject.SetActive(false);
			}
			if (!SecureStorage.Instance.DiamondsTutorialViewed)
			{
				if (CancelButton.gameObject.active)
				{
					CancelButton.gameObject.active = false;
				}
				m_messageBody.Text = Language.Get("S_DIAMONDS_TO_CONTINUE_TUTORIAL_INFO");
				if (!CostIncreasesText.gameObject.active)
				{
					CostIncreasesText.gameObject.active = true;
				}
			}
			else
			{
				if (!CancelButton.gameObject.active)
				{
					CancelButton.gameObject.active = true;
				}
				m_messageBody.transform.localPosition = new Vector3(0f, -20f, 0f);
				if (CostIncreasesText.gameObject.active)
				{
					CostIncreasesText.gameObject.active = false;
				}
			}
		}
		if (m_visibleContents.transform.localScale.x > 0.9f)
		{
			m_visibleContents.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
		}
		else
		{
			m_visibleContents.ScaleTo(Vector3.one, 0.5f, 0f, EaseType.spring);
		}
		if (AdditionalGemsRequiredForContinue <= 0 || !SecureStorage.Instance.DiamondsTutorialViewed)
		{
			if (!ContinueButton.active)
			{
				ContinueButton.SetActiveRecursively(true);
			}
			if (GetDiamondsButton.active)
			{
				GetDiamondsButton.SetActiveRecursively(false);
			}
			if (TotalGemsRequiredForContinue == 1)
			{
				if (SecureStorage.Instance.DiamondsTutorialViewed)
				{
					m_messageBody.Text = Language.Get(Localisation_EnoughDiamondsSolo);
				}
				DiamondCountValue.Text = "1";
			}
			else
			{
				m_messageBody.Text = string.Format(Language.Get(Localisation_EnoughDiamonds), TotalGemsRequiredForContinue);
				DiamondCountValue.Text = TotalGemsRequiredForContinue + string.Empty;
			}
		}
		else
		{
			if (!GetDiamondsButton)
			{
				GetDiamondsButton.SetActiveRecursively(true);
			}
			if (ContinueButton.active)
			{
				ContinueButton.SetActiveRecursively(false);
			}
			if (AdditionalGemsRequiredForContinue == 1)
			{
				m_messageBody.Text = Language.Get(Localisation_NotEnoughDiamondsSolo);
				DiamondCountValue.Text = "1";
			}
			else
			{
				m_messageBody.Text = string.Format(Language.Get(Localisation_NotEnoughDiamonds), AdditionalGemsRequiredForContinue);
				DiamondCountValue.Text = TotalGemsRequiredForContinue + string.Empty;
			}
		}
		yield return new WaitForSeconds(0.1f);
	}

	public void SetContinueNumberVal(int myValue)
	{
		Number.Text = myValue + string.Empty;
	}

	public void OnButtonPressed()
	{
		if (m_allowbuttons)
		{
			MonoBehaviour.print("CONTINUE BUTTON PRESSED " + Time.time);
			StartTheExit(true);
			GameController.Instance.ContinueFromDiamonds(TotalGemsRequiredForContinue);
			MenuSFX.Instance.Play2D("MenuConfirm");
			SecureStorage.Instance.DiamondsTutorialViewed = true;
		}
	}

	public void PurchaseButtonPressed()
	{
		if (m_allowbuttons)
		{
			MonoBehaviour.print("IN-GAME EXCHANGE FOR PURCHASING DIAMONDS INVOKED " + Time.time);
			ToGemStore = true;
			InitializeInGameStore(AdditionalGemsRequiredForContinue);
			MenuSFX.Instance.Play2D("MenuConfirm");
			StartTheExit(false);
		}
	}

	public void StartTheExit(bool DoDestroy)
	{
		m_allowbuttons = false;
		StartCoroutine(ExitDialog(DoDestroy));
	}

	private IEnumerator ExitDialog(bool DoDestroy)
	{
		yield return new WaitForSeconds(0.11f);
		float speed = 0.2f;
		m_visibleContents.ScaleTo(Vector3.zero, speed, 0f, EaseType.easeOutCubic);
		yield return new WaitForSeconds(speed);
		if (DoDestroy)
		{
			if (InGameStore != null)
			{
				Object.Destroy(InGameStore);
				InGameStore = null;
			}
			Object.Destroy(base.gameObject);
		}
	}

	private void InitializeInGameStore(int AdditionalRequired)
	{
		StartCoroutine(WaitThenInstantiateStore(AdditionalRequired));
	}

	private IEnumerator WaitThenInstantiateStore(int AdditionalGemsNeeded)
	{
		yield return new WaitForSeconds(0.25f);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			StartCoroutine(Display(false));
			EtceteraPlatformWrapper.ShowAlert("S_IAP_NO_NET_TITLE", "S_IAP_NO_NET_BODY", "S_IAP_OK");
			yield break;
		}
		MonoBehaviour.print("INSTANTIATE IN-GAME STORE PREFAB " + Time.time);
		InGameStore = Object.Instantiate(InGameStorePrefab, new Vector3(0f, 0f, -60f), base.transform.rotation) as GameObject;
		InGameStorePopup.Instance.SetAdditionalAmount(AdditionalGemsNeeded, false);
		InGameStorePopup.Instance.HideCancelButton();
		StartCoroutine(GemStoreController.Instance.PopulateProductList(false));
	}

	public void OnCancelPressed()
	{
		MonoBehaviour.print("DIAMONDS TO CONTINUE DIALOG CANCEL BUTTON PRESSED " + Time.time);
		MenuSFX.Instance.Play2D("MenuCancel");
		StartTheExit(true);
		GameController.Instance.CancelFromDiamonds();
	}

	public void UpdateDiamondCountOnGameHud()
	{
		GameController.Instance.GameHUD.m_diamondsText.Text = SecureStorage.Instance.GetGems().ToString();
	}

	public void PurchaseToContinue()
	{
		InGameStorePopup.Instance.ExitInGameStore();
		StartTheExit(true);
		GameController.Instance.ContinueFromDiamonds(TotalGemsRequiredForContinue);
	}
}
