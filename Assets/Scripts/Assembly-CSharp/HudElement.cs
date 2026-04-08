using System.Collections.Generic;
using UnityEngine;

public class HudElement : HudMovement
{
	public bool m_Rhs;

	public List<SubElement> m_SubElements = new List<SubElement>();

	private List<PositionAfterParent> m_PositionScript = new List<PositionAfterParent>();

	private List<FollowPosition> m_FollowPositionScript = new List<FollowPosition>();

	public override void Awake()
	{
		base.Awake();
		for (int i = 0; i < m_SubElements.Count; i++)
		{
			GameObject backGround = m_SubElements[i].m_BackGround;
			if (m_SubElements[i].m_Text != null)
			{
				SizeToText sizeToText = backGround.GetComponent<SizeToText>();
				if (sizeToText == null)
				{
					sizeToText = backGround.AddComponent<SizeToText>();
				}
				if (sizeToText != null)
				{
					sizeToText.m_Text = m_SubElements[i].m_Text;
				}
			}
		}
		for (int j = 0; j < m_SubElements.Count - 1; j++)
		{
			GameObject backGround2 = m_SubElements[j + 1].m_BackGround;
			PositionAfterParent positionAfterParent = backGround2.GetComponent<PositionAfterParent>();
			if (positionAfterParent == null)
			{
				positionAfterParent = backGround2.AddComponent<PositionAfterParent>();
			}
			if ((bool)positionAfterParent)
			{
				positionAfterParent.SetRHS(m_Rhs);
				positionAfterParent.SetZ(0f);
				positionAfterParent.SetRelativeTo(m_SubElements[j].m_BackGround.transform);
				m_PositionScript.Add(positionAfterParent);
			}
		}
	}

	private void LateUpdate()
	{
		foreach (PositionAfterParent item in m_PositionScript)
		{
			item.UpdatePos();
		}
		foreach (FollowPosition item2 in m_FollowPositionScript)
		{
			item2.UpdatePos();
		}
	}
}
