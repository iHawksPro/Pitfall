using UnityEngine;

public class BoolValueSlider : ValueSlider
{
	private bool m_Value;

	public BoolValueSlider(string name, float xpos, ref float ypos, bool startVal)
	{
		m_Value = startVal;
		sliderBoxRect = new Rect(xpos, ypos, 250f, 25f);
		sliderRect = new Rect(xpos, ypos, 25f, 25f);
		ypos += 30f;
		m_Name = name;
	}

	public bool OnGui()
	{
		GUI.Box(sliderBoxRect, m_Name);
		GUI.Label(valueLabelRect, string.Empty);
		m_Value = GUI.Toggle(sliderRect, m_Value, string.Empty);
		return m_Value;
	}

	public bool GetValue()
	{
		return m_Value;
	}
}
