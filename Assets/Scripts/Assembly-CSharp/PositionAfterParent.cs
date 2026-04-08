using UnityEngine;

public class PositionAfterParent : MonoBehaviour
{
	private AutoSpriteBase m_ParentButton;

	private AutoSpriteBase m_MyButton;

	public Transform m_RelativeObject;

	private Vector3 m_Start;

	private Vector3 m_cachedLocalPos = new Vector3(-1000f, -1000f, -1000f);

	private Vector2 m_Offset;

	private bool m_Rhs;

	public void SetOffset(float x, float y)
	{
		m_Offset.x = x;
		m_Offset.y = y;
	}

	public void SetZ(float z)
	{
		m_Start.z = z;
	}

	public void SetRHS(bool val)
	{
		m_Rhs = val;
	}

	public void SetRelativeTo(Transform trans)
	{
		m_RelativeObject = trans;
	}

	private void Awake()
	{
		m_Offset = new Vector2(0f, 0f);
		m_Rhs = false;
	}

	private void Start()
	{
		m_ParentButton = m_RelativeObject.GetComponent<AutoSpriteBase>();
		m_MyButton = base.gameObject.GetComponent<AutoSpriteBase>();
		m_Start = base.transform.localPosition;
	}

	public void UpdatePos()
	{
		if ((bool)m_ParentButton)
		{
			Vector3 localPosition = base.transform.localPosition;
			if (m_Rhs)
			{
				float num = m_MyButton.PixelSize.x * base.transform.localScale.x * TBFUtils.GetInverseUIScaling();
				localPosition.x = m_Offset.x - num;
				localPosition.x += m_ParentButton.transform.localPosition.x;
				localPosition.x += 0.5f;
				localPosition.x += 2.5f;
			}
			else
			{
				float num2 = m_ParentButton.PixelSize.x * m_RelativeObject.localScale.x * TBFUtils.GetInverseUIScaling();
				localPosition.x = num2 + m_Offset.x;
				localPosition.x += m_ParentButton.transform.localPosition.x;
				localPosition.x -= 0.5f;
				localPosition.x -= 2.5f;
			}
			localPosition.y = m_Start.y + m_Offset.y;
			localPosition.z = m_Start.z;
			if (localPosition != m_cachedLocalPos)
			{
				base.transform.localPosition = localPosition;
				m_cachedLocalPos = localPosition;
			}
		}
	}
}
