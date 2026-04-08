using UnityEngine;

public class PoisonHUDController : MonoBehaviour
{
	public enum PoisonHUDState
	{
		Off = 0,
		FadeOn = 1,
		On = 2,
		FadeOff = 3
	}

	private PoisonHUDState mState;

	public GameObject mHUDgfxRU_Parent;

	public GameObject mHUDgfxRD_Child;

	public GameObject mHUDgfxLD_Child;

	public GameObject mHUDgfxLU_Child;

	public float FadeScale;

	private Color m_Blend = default(Color);

	private float m_fTimeDuration;

	private static PoisonHUDController mInstance;

	public static PoisonHUDController Instance()
	{
		return mInstance;
	}

	public void Awake()
	{
		mInstance = this;
		mState = PoisonHUDState.Off;
		if (TBFUtils.GetAspectRatio() > 1.5f)
		{
			base.transform.localScale = new Vector3(0.86f, 0.5f, 0.75f);
		}
		m_Blend = mHUDgfxRU_Parent.GetComponent<Renderer>().material.color;
		m_Blend.a = 0f;
		mHUDgfxRU_Parent.SetActiveRecursively(false);
	}

	public void Reset()
	{
		mState = PoisonHUDState.Off;
		mHUDgfxRU_Parent.SetActiveRecursively(false);
		m_Blend.a = 0f;
	}

	public void Update()
	{
		switch (mState)
		{
		case PoisonHUDState.Off:
			break;
		case PoisonHUDState.FadeOn:
			m_Blend.a += Time.deltaTime * FadeScale;
			if (m_Blend.a > 1f)
			{
				m_Blend.a = 1f;
				mState = PoisonHUDState.On;
				m_fTimeDuration = 0f;
			}
			SetColour();
			break;
		case PoisonHUDState.FadeOff:
			m_Blend.a -= Time.deltaTime * FadeScale;
			if (m_Blend.a < 0f)
			{
				m_Blend.a = 0f;
				mState = PoisonHUDState.Off;
				mHUDgfxRU_Parent.SetActiveRecursively(false);
			}
			SetColour();
			break;
		case PoisonHUDState.On:
			if (PlayerController.Instance().IsDead())
			{
				Hide();
				break;
			}
			m_fTimeDuration += Time.deltaTime * FadeScale;
			m_Blend.a = Mathf.Abs(Mathf.Cos(m_fTimeDuration * 2.5f));
			SetColour();
			break;
		}
	}

	public void Show()
	{
		if (mState == PoisonHUDState.Off)
		{
			mState = PoisonHUDState.FadeOn;
			mHUDgfxRU_Parent.SetActiveRecursively(true);
		}
	}

	public void Hide()
	{
		if (mState != PoisonHUDState.Off || mState != PoisonHUDState.FadeOff)
		{
			mState = PoisonHUDState.FadeOff;
		}
	}

	public void HideImmediately()
	{
		if (mState != PoisonHUDState.Off || mState != PoisonHUDState.FadeOff)
		{
			m_Blend.a = 0f;
			SetColour();
			mState = PoisonHUDState.Off;
			mHUDgfxRU_Parent.SetActiveRecursively(false);
		}
	}

	private void SetColour()
	{
		mHUDgfxRU_Parent.GetComponent<Renderer>().material.color = m_Blend;
		mHUDgfxRD_Child.GetComponent<Renderer>().material.color = m_Blend;
		mHUDgfxLD_Child.GetComponent<Renderer>().material.color = m_Blend;
		mHUDgfxLU_Child.GetComponent<Renderer>().material.color = m_Blend;
	}
}
