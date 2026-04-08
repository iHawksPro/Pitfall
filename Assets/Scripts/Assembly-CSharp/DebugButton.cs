using UnityEngine;

public class DebugButton
{
	private bool m_Value;

	private string m_Name;

	private Rect m_ButtonRect;

	public DebugButton(string name, float xpos, ref float ypos)
	{
		m_ButtonRect = new Rect(xpos, ypos, 250f, 25f);
		ypos += 30f;
		m_Name = name;
	}

	public bool OnGui()
	{
		m_Value = GUI.Button(m_ButtonRect, m_Name);
		return m_Value;
	}
}
