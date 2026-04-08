using UnityEngine;

public class FloatValueSlider : ValueSlider
{
	private float m_Value;

	private float m_Min;

	private float m_Max;

	public FloatValueSlider(string name, float xpos, ref float ypos, float startVal, float min, float max)
		: base(name, xpos, ref ypos)
	{
		m_Value = startVal;
		m_Min = min;
		m_Max = max;
	}

	public float OnGui()
	{
		GUI.Box(sliderBoxRect, m_Name);
		GUI.Label(valueLabelRect, m_Value.ToString("#.00"));
		m_Value = GUI.HorizontalSlider(sliderRect, m_Value, m_Min, m_Max);
		return m_Value;
	}

	public float GetValue()
	{
		return m_Value;
	}
}
