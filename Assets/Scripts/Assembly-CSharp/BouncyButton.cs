using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class BouncyButton : MonoBehaviour
{
	public GameObject m_bouncingObject;

	public float m_pulseTime;

	public Vector3 m_pulseAmount = new Vector3(1.1f, 1.1f, 1f);

	public string m_sfxToPlay;

	private UIButton m_button;

	private MonoBehaviour m_scriptWithMethodToInvoke;

	private string m_methodToInvoke = string.Empty;

	private bool m_bWorking;

	private void Awake()
	{
		m_button = base.gameObject.GetComponentInChildren<UIButton>();
		if (m_button != null)
		{
			ReplaceButtonScript();
		}
		if (m_bouncingObject == null)
		{
			m_bouncingObject = base.gameObject;
		}
		m_bWorking = false;
	}

	private void Start()
	{
		if (m_pulseTime > 0f)
		{
			m_bouncingObject.ScaleTo(m_pulseAmount, m_pulseTime / 2f, 0f, EaseType.easeInOutCubic, LoopType.pingPong);
		}
	}

	private void ReplaceButtonScript()
	{
		m_scriptWithMethodToInvoke = m_button.scriptWithMethodToInvoke;
		m_methodToInvoke = m_button.methodToInvoke;
		m_button.scriptWithMethodToInvoke = this;
		m_button.methodToInvoke = "OnButtonPress";
	}

	private IEnumerator BounceButtonAndCallMethod()
	{
		if (m_sfxToPlay != null && m_sfxToPlay.Length > 0)
		{
			MenuSFX.Instance.Play2D(m_sfxToPlay);
		}
		CommonAnimations.AnimateButton(m_bouncingObject);
		float pauseEndTime = Time.realtimeSinceStartup + 0.25f;
		while (Time.realtimeSinceStartup < pauseEndTime)
		{
			yield return 0;
		}
		if (m_scriptWithMethodToInvoke != null)
		{
			m_scriptWithMethodToInvoke.Invoke(m_methodToInvoke, m_button.delay);
		}
		m_bWorking = false;
	}

	private void OnButtonPress()
	{
		if (!m_bWorking)
		{
			m_bWorking = true;
			StartCoroutine("BounceButtonAndCallMethod");
		}
	}
}
