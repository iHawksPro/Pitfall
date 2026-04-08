using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TBF/UIStretchingPackedSprite")]
[RequireComponent(typeof(PackedSprite))]
public class UIStretchingPackedSprite : MonoBehaviour
{
	public enum eAdjustType
	{
		eLEFT_TO_RIGHT = 0,
		eRIGHT_TO_LEFT = 1,
		eCENTRE_AROUND_TEXT = 2
	}

	private const float c_fTextWrapOffset = -10f;

	public PackedSprite m_Left;

	public PackedSprite m_Middle;

	public PackedSprite m_Right;

	public GameObject m_LinkedScaleObject;

	public bool UseExtraSpaces = true;

	public SpriteText m_Text;

	private float m_TextOriginalOffset;

	public UIButton m_UIButton;

	public UIPanelTab m_UIPanelTab;

	public float fLeftUV = 0.45f;

	public float fRightUV = 0.55f;

	public bool m_bUseAbsoluteAdjustment;

	public float m_fExtraPadding;

	private float m_fOriginalWidth;

	private float m_fOriginalHeight;

	private float m_fAbsoluteAdjustment;

	public eAdjustType ePositionAdjustment;

	public List<GameObject> m_CascadeList;

	public void Awake()
	{
		m_fAbsoluteAdjustment = m_Right.transform.localPosition.x - m_Left.transform.localPosition.x;
		if ((bool)m_Text)
		{
			switch (ePositionAdjustment)
			{
			case eAdjustType.eRIGHT_TO_LEFT:
				m_TextOriginalOffset = m_Text.transform.position.x - m_Left.transform.position.x;
				break;
			case eAdjustType.eLEFT_TO_RIGHT:
				m_TextOriginalOffset = m_Text.transform.position.x - base.transform.position.x;
				break;
			}
		}
	}

	private Vector3 GetLinkedScaleObjectEffectiveScale()
	{
		Vector3 result = new Vector3(1f, 1f, 1f);
		if (m_LinkedScaleObject != null)
		{
			Transform parent = m_LinkedScaleObject.transform;
			while ((bool)parent)
			{
				result.x *= parent.localScale.x;
				result.y *= parent.localScale.y;
				result.z *= parent.localScale.z;
				parent = parent.parent;
			}
		}
		return result;
	}

	public void Start()
	{
		PackedSprite component = GetComponent<PackedSprite>();
		component.Hide(true);
		m_fOriginalHeight = component.height;
		m_fOriginalWidth = component.width;
		component.SetSize(0f, 0f);
	}

	public void Rebuild()
	{
		if (m_CascadeList != null && m_CascadeList.Count > 0)
		{
			m_TextOriginalOffset = 0f;
			foreach (GameObject cascade in m_CascadeList)
			{
				SpriteText component = cascade.GetComponent<SpriteText>();
				if ((bool)component)
				{
					if (UseExtraSpaces)
					{
						m_TextOriginalOffset += component.GetWidth(component.Text) + component.GetWidth("  ");
					}
					else
					{
						m_TextOriginalOffset += component.GetWidth(component.Text);
					}
					continue;
				}
				AutoSpriteBase component2 = cascade.GetComponent<AutoSpriteBase>();
				if ((bool)component2)
				{
					m_TextOriginalOffset += component2.width;
				}
			}
			m_TextOriginalOffset += m_fExtraPadding;
		}
		MeshRenderer component3 = GetComponent<MeshRenderer>();
		if (component3 != null)
		{
			component3.enabled = false;
		}
		PackedSprite component4 = GetComponent<PackedSprite>();
		float fOriginalWidth = m_fOriginalWidth;
		float fOriginalHeight = m_fOriginalHeight;
		Rect uVs = component4.GetUVs();
		float to = uVs.x + uVs.width;
		float num = Mathf.Lerp(uVs.x, to, fLeftUV);
		float num2 = Mathf.Lerp(uVs.x, to, fRightUV);
		Rect uVs2 = component4.GetUVs();
		uVs2.width = uVs.width * fLeftUV;
		if (m_Left != null)
		{
			m_Left.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
			m_Left.SetUVs(uVs2);
			m_Left.SetSize(fOriginalWidth * fLeftUV, fOriginalHeight);
			if (ePositionAdjustment == eAdjustType.eRIGHT_TO_LEFT)
			{
				if (m_CascadeList != null && m_CascadeList.Count > 0)
				{
					m_Left.transform.position = new Vector3(m_Right.transform.position.x - m_TextOriginalOffset - 10f, m_Right.transform.position.y, m_Right.transform.position.z);
				}
				else if (m_Text == null)
				{
					m_Left.transform.position = new Vector3(m_Left.transform.position.x, m_Right.transform.position.y, m_Right.transform.position.z);
				}
				else
				{
					m_Left.transform.position = new Vector3(m_Right.transform.position.x - m_Text.GetWidth(m_Text.Text) - m_Text.GetWidth(" ") - m_TextOriginalOffset, m_Right.transform.position.y, m_Right.transform.position.z);
				}
			}
			if (ePositionAdjustment == eAdjustType.eCENTRE_AROUND_TEXT)
			{
				if (m_CascadeList != null && m_CascadeList.Count > 0)
				{
					m_Left.transform.position = new Vector3(m_Text.transform.position.x - m_TextOriginalOffset * 0.5f, m_Text.transform.position.y + -10f, m_Text.transform.position.z + 1f);
				}
				else
				{
					m_Left.transform.position = new Vector3(m_Text.transform.position.x - 0.5f * m_Text.GetWidth(m_Text.Text) - m_Text.GetWidth(" "), m_Text.transform.position.y + -10f, m_Text.transform.position.z + 1f);
				}
			}
			if ((bool)m_LinkedScaleObject)
			{
				m_Left.transform.localScale = GetLinkedScaleObjectEffectiveScale();
			}
		}
		Rect uVs3 = component4.GetUVs();
		uVs3.x = Mathf.Lerp(uVs.x, to, fRightUV);
		uVs3.width = uVs.width * (1f - fRightUV);
		if (m_Right != null)
		{
			m_Right.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
			m_Right.SetUVs(uVs3);
			m_Right.SetSize(fOriginalWidth * (1f - fRightUV), fOriginalHeight);
			if (ePositionAdjustment == eAdjustType.eLEFT_TO_RIGHT)
			{
				if (m_CascadeList != null && m_CascadeList.Count > 0)
				{
					m_Right.transform.position = new Vector3(m_Left.transform.position.x + m_TextOriginalOffset, m_Left.transform.position.y, m_Left.transform.position.z);
				}
				else if (m_Text == null)
				{
					if (!m_bUseAbsoluteAdjustment)
					{
						m_Right.transform.position = new Vector3(m_Right.transform.position.x, m_Left.transform.position.y, m_Left.transform.position.z);
					}
					else
					{
						m_Right.transform.position = new Vector3(Mathf.Ceil(m_Left.transform.position.x + m_fAbsoluteAdjustment * GetLinkedScaleObjectEffectiveScale().x), m_Left.transform.position.y, m_Left.transform.position.z);
					}
				}
				else
				{
					m_Right.transform.position = new Vector3(Mathf.Ceil(m_Left.transform.position.x + m_Text.GetWidth(m_Text.Text) + m_Text.GetWidth(" ") + m_TextOriginalOffset), m_Left.transform.position.y, m_Left.transform.position.z);
				}
			}
			if (ePositionAdjustment == eAdjustType.eCENTRE_AROUND_TEXT)
			{
				if (m_CascadeList != null && m_CascadeList.Count > 0)
				{
					m_Right.transform.position = new Vector3(m_Text.transform.position.x + m_TextOriginalOffset * 0.5f, m_Text.transform.position.y + -10f, m_Text.transform.position.z + 1f);
				}
				else
				{
					m_Right.transform.position = new Vector3(m_Text.transform.position.x + 0.5f * m_Text.GetWidth(m_Text.Text) + m_Text.GetWidth(" "), m_Text.transform.position.y + -10f, m_Text.transform.position.z + 1f);
				}
			}
			if ((bool)m_LinkedScaleObject)
			{
				m_Right.transform.localScale = GetLinkedScaleObjectEffectiveScale();
			}
		}
		Rect uVs4 = component4.GetUVs();
		uVs4.x = Mathf.Lerp(uVs.x, to, fLeftUV);
		uVs4.width = uVs.width * (fRightUV - fLeftUV);
		if (m_Middle != null)
		{
			m_Middle.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
			m_Middle.SetUVs(uVs4);
			m_Middle.SetSize(1f, fOriginalHeight);
			float f = Mathf.Ceil(m_Right.transform.position.x - m_Left.transform.position.x);
			if ((bool)m_LinkedScaleObject)
			{
				Vector3 linkedScaleObjectEffectiveScale = GetLinkedScaleObjectEffectiveScale();
				Vector3 localScale = m_Middle.transform.localScale;
				if (linkedScaleObjectEffectiveScale.x != 0f)
				{
					m_Middle.transform.localScale = new Vector3(5f + Mathf.Ceil(f) / linkedScaleObjectEffectiveScale.x, linkedScaleObjectEffectiveScale.y, linkedScaleObjectEffectiveScale.z);
				}
				else
				{
					m_Middle.transform.localScale = new Vector3(5f, linkedScaleObjectEffectiveScale.y, linkedScaleObjectEffectiveScale.z);
				}
				m_Middle.transform.position = new Vector3(m_Left.transform.position.x - 2.5f, m_Left.transform.position.y, m_Left.transform.position.z);
			}
			else
			{
				m_Middle.transform.localScale = new Vector3(Mathf.Ceil(f), m_Middle.transform.localScale.y, m_Middle.transform.localScale.z);
				m_Middle.transform.position = new Vector3(m_Left.transform.position.x, m_Left.transform.position.y, m_Left.transform.position.z);
			}
		}
		if (m_UIButton != null)
		{
			Vector3 position = m_Left.transform.position;
			position.x -= m_Left.width * 0.75f;
			m_UIButton.transform.position = position;
			m_UIButton.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
			m_UIButton.SetSize(m_Middle.transform.localScale.x + m_Left.width * 1.5f, fOriginalHeight);
		}
		else if (m_UIPanelTab != null)
		{
			Vector3 position2 = m_Left.transform.position;
			position2.x -= m_Left.width * 0.75f;
			m_UIPanelTab.transform.position = position2;
			m_UIPanelTab.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
			m_UIPanelTab.SetSize(m_Middle.transform.localScale.x + m_Left.width * 1.5f, fOriginalHeight);
		}
	}

	public void Update()
	{
	}

	public void OnEnable()
	{
		Rebuild();
	}

	public void LateUpdate()
	{
		Rebuild();
	}
}
