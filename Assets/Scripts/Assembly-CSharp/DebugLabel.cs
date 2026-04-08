using UnityEngine;

public class DebugLabel
{
	private Rect m_Rect;

	public DebugLabel(float xpos, ref float ypos)
	{
		m_Rect = new Rect(xpos, ypos, 250f, 25f);
		ypos += 30f;
	}

	public void OnGui(string str)
	{
		GUI.Label(m_Rect, str);
	}
}
