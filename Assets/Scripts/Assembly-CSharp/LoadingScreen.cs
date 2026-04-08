using System.Collections;
using UnityEngine;

public class LoadingScreen : SingletonMonoBehaviour
{
	private const string m_messageKey = "S_LOADING";

	private Renderer[] m_renderers;

	private bool m_isVisible = true;

	public SpriteText m_messageText;

	public static LoadingScreen Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<LoadingScreen>();
		}
	}

	public bool IsVisible
	{
		get
		{
			return m_isVisible;
		}
	}

	protected override void AwakeOnce()
	{
		if (Instance == this)
		{
			Object.DontDestroyOnLoad(this);
			m_renderers = base.gameObject.GetComponentsInChildren<Renderer>();
			Hide();
		}
	}

	public void Show()
	{
		if (!m_isVisible)
		{
			m_isVisible = true;
			SetVisibility();
			base.transform.position = Vector3.zero;
			StartCoroutine("DoAnimation");
		}
	}

	public void Hide()
	{
		if (m_isVisible)
		{
			base.transform.position = new Vector3(0f, -10000f, 0f);
			m_isVisible = false;
			SetVisibility();
			StopCoroutine("DoAnimation");
		}
	}

	private void SetVisibility()
	{
		Renderer[] renderers = m_renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = m_isVisible;
		}
	}

	private IEnumerator DoAnimation()
	{
		int nFrames = 0;
		while (true)
		{
			yield return new WaitForSeconds(0.25f);
			int nDots = nFrames % 4;
			string Message = Language.Get("S_LOADING");
			for (int iDot = 0; iDot < nDots; iDot++)
			{
				Message += ".";
			}
			m_messageText.Text = Message;
			nFrames++;
		}
	}
}
