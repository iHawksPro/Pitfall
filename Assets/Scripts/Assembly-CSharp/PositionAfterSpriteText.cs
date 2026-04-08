using UnityEngine;

public class PositionAfterSpriteText : MonoBehaviour
{
	public SpriteText m_Parent;

	public float m_fXOffset;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)m_Parent)
		{
			Vector3 position = base.gameObject.transform.position;
			float num = m_Parent.GetWidth(m_Parent.Text) + m_Parent.GetWidth(" ");
			switch (m_Parent.Anchor)
			{
			case SpriteText.Anchor_Pos.Upper_Center:
			case SpriteText.Anchor_Pos.Middle_Center:
			case SpriteText.Anchor_Pos.Lower_Center:
				num *= 0.5f;
				break;
			case SpriteText.Anchor_Pos.Upper_Right:
			case SpriteText.Anchor_Pos.Middle_Right:
			case SpriteText.Anchor_Pos.Lower_Right:
				num = 0f - num;
				break;
			}
			position.x = m_Parent.transform.position.x + num + m_fXOffset;
			base.gameObject.transform.position = position;
		}
	}
}
