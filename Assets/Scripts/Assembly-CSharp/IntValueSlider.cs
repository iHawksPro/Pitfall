using UnityEngine;

public class IntValueSlider : ValueSlider
{
	private int m_Value;

	private int m_Min;

	private int m_Max;

	public IntValueSlider(string name, float xpos, ref float ypos, int startVal, int min, int max)
		: base(name, xpos, ref ypos)
	{
		m_Value = startVal;
		m_Min = min;
		m_Max = max;
	}

	public int OnGui()
	{
		GUI.Box(sliderBoxRect, m_Name);
		GUI.Label(valueLabelRect, m_Value.ToString("#.00"));
		m_Value = (int)GUI.HorizontalSlider(sliderRect, m_Value, m_Min, m_Max);
		return m_Value;
	}

	public int GetValue()
	{
		return m_Value;
	}
}
