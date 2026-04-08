using System.Collections;
using UnityEngine;

public class SplashIntroController : StateController
{
	public GameObject m_introAnim;

	public Camera m_introCamera;

	public float m_timeUntilSkip = 8f;

	private bool m_introDone;

	public SpriteText m_skipText;

	private void HideIntroImmediately()
	{
		UIManager.instance.blockInput = true;
		if (m_skipText != null)
		{
			m_skipText.Hide(true);
		}
		if (m_introAnim != null)
		{
			Animation animation = m_introAnim.GetComponent<Animation>();
			if (animation != null)
			{
				animation.Stop();
			}
		}
		HideState();
	}

	public override void Awake()
	{
		LoadingScreen.Instance.Show();
	}

	protected override void OnStateActivate(string OldStateName)
	{
		UIManager.instance.blockInput = true;
		ShowState();
	}

	protected override void ShowState()
	{
		base.ShowState();
		m_skipText.Hide(true);
		m_introDone = false;
		float length = m_introAnim.GetComponent<Animation>().clip.length;
		StartCoroutine(WaitForIntroEnd(length));
		m_introAnim.GetComponent<Animation>().Play();
		LoadingScreen.Instance.Hide();
	}

	private IEnumerator WaitForIntroEnd(float introLength)
	{
		yield return new WaitForSeconds(m_timeUntilSkip);
		UIManager.instance.blockInput = false;
		m_skipText.Hide(false);
		float remainingTime = introLength - m_timeUntilSkip;
		yield return new WaitForSeconds(remainingTime);
		IntroDone();
	}

	private void IntroDone()
	{
		if (!m_introDone)
		{
			HideIntroImmediately();
			UIMenuBackground.Instance.SwitchCameraFocus("Title");
			UIMenuBackground.Instance.Show();
			UIMenuBackground.Instance.PlayPostSplashAnim();
			StateManager.Instance.LoadAndActivateState("Title");
			m_introDone = true;
		}
	}

	private void TapToSkip()
	{
		if (!m_introDone)
		{
			StopCoroutine("WaitForIntroEnd");
		}
		IntroDone();
	}

	protected override IEnumerator AnimateStateOut()
	{
		HideIntroImmediately();
		yield break;
	}
}
