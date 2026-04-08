using System.Collections.Generic;
using UnityEngine;

public class ResizeScale9ForText : MonoBehaviour
{
	public Scale9Grid m_Grid;

	public List<SpriteText> m_TextItems;

	private void Start()
	{
	}

	private void Update()
	{
		float num = 0f;
		for (int i = 0; i < m_TextItems.Count; i++)
		{
			num = Mathf.Max(TBFUtils.GetUITextScaling() * m_TextItems[i].GetScreenWidth(m_TextItems[i].Text + "                "), num);
		}
		m_Grid.size.x = num + (float)m_Grid.cornerSize;
		m_Grid.Resize();
	}
}
