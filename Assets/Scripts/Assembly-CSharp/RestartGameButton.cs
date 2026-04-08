using UnityEngine;

public class RestartGameButton : MonoBehaviour
{
	public UIButton m_restartButton;

	public GameObject m_highlights;

	public GameObject m_counter;

	public SpriteText m_counterText;

	private MonoBehaviour m_scriptWithMethodToInvoke;

	private string m_methodToInvoke = string.Empty;

	public UIButton m_labelButton;

	private void Start()
	{
		m_scriptWithMethodToInvoke = m_restartButton.scriptWithMethodToInvoke;
		m_methodToInvoke = m_restartButton.methodToInvoke;
		m_restartButton.scriptWithMethodToInvoke = this;
		m_restartButton.methodToInvoke = "OnButtonPressed";
		m_restartButton.scriptWithMethodToInvokeWhenDisabled = this;
		m_restartButton.methodToInvokeWhenDisabled = "OnButtonPressedWhenDisabled";
	}

	private void OnEnable()
	{
		if (IsMacawAvailable())
		{
			m_highlights.SetActiveRecursively(true);
			m_restartButton.controlIsEnabled = true;
			m_counter.SetActiveRecursively(true);
			m_counterText.Text = GetMacawsHeld().ToString();
			m_labelButton.controlIsEnabled = true;
		}
		else
		{
			m_highlights.SetActiveRecursively(false);
			m_counter.SetActiveRecursively(false);
			m_restartButton.controlIsEnabled = false;
			m_labelButton.controlIsEnabled = false;
		}
	}

	private bool IsMacawAvailable()
	{
		bool result = false;
		CheckPointController checkPointController = CheckPointController.Instance();
		if (checkPointController != null)
		{
			float num = checkPointController.FindFurthestAvailableCheckpointForDistance(SecureStorage.Instance.FurthestDistanceTravelled);
			if (num > 0f)
			{
				result = GetMacawsHeld() > 0;
			}
		}
		return result;
	}

	private int GetMacawsHeld()
	{
		return SecureStorage.Instance.NumMacawsHeld;
	}

	private void OnButtonPressed()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		if (m_scriptWithMethodToInvoke != null)
		{
			m_scriptWithMethodToInvoke.Invoke(m_methodToInvoke, 0.25f);
		}
		CommonAnimations.AnimateButton(m_restartButton.gameObject);
	}

	private void OnButtonPressedWhenDisabled()
	{
		MenuSFX.Instance.Play2D("MenuConfirm");
		CommonAnimations.AnimateButton(m_restartButton.gameObject);
		DialogManager.Instance.LaunchNoMccawDialog();
	}
}
