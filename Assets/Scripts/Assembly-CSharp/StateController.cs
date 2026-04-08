using System.Collections;
using UnityEngine;

public class StateController : MonoBehaviour
{
	public const float DefaultAnimateInTime = 0.5f;

	public const float DefaultAnimateOutTime = 0.5f;

	protected bool m_isActive;

	protected string m_lastState = string.Empty;

	public float animateInTime = 0.5f;

	public float animateOutTime = 0.5f;

	public virtual void Awake()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(false);
		}
	}

	protected virtual void OnStateLoaded()
	{
		if (StateManager.Instance.nStates > 1)
		{
			HideState();
		}
	}

	protected virtual void OnStateActivate(string OldStateName)
	{
		m_isActive = true;
		m_lastState = OldStateName;
		StartCoroutine(AnimateStateInInternal());
	}

	protected virtual void OnStateDeactivate(string NewStateName)
	{
		m_isActive = false;
		StartCoroutine(AnimateStateOutInternal());
	}

	protected virtual void OnStateBack()
	{
		StateManager.Instance.LoadAndActivateState(m_lastState);
	}

	protected virtual void HideState()
	{
		base.transform.position = new Vector3(0f, 10000f, 0f);
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(false);
		}
	}

	protected virtual void ShowState()
	{
		base.transform.position = Vector3.zero;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(true);
		}
	}

	private IEnumerator AnimateStateInInternal()
	{
		yield return StartCoroutine(AnimateStateIn());
		UIManager.instance.blockInput = false;
	}

	private IEnumerator AnimateStateOutInternal()
	{
		UIManager.instance.blockInput = true;
		yield return StartCoroutine(AnimateStateOut());
	}

	protected virtual IEnumerator AnimateStateIn()
	{
		HideState();
		while (StateManager.Instance.IsLoading())
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.1f);
		ShowState();
	}

	protected virtual IEnumerator AnimateStateOut()
	{
		base.gameObject.BroadcastMessage("GoOffScreen", SendMessageOptions.DontRequireReceiver);
		yield return new WaitForSeconds(0.2f);
		MenuSFX.Instance.Play2D("MenuBoxSwoosh");
		yield return new WaitForSeconds(0.3f);
		HideState();
	}

	public virtual void OnBackPressed()
	{
		if (BuyDialog.Instance != null)
		{
			BuyDialog.Instance.OnCancelPressed();
			return;
		}
		MenuSFX.Instance.Play2D("MenuCancel");
		StateManager.Instance.LoadAndActivatePreviousState();
	}
}
