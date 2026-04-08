using UnityEngine;

public class ValueSlider
{
	protected Rect sliderBoxRect;

	protected Rect valueLabelRect;

	protected Rect sliderRect;

	protected string m_Name;

	public ValueSlider()
	{
	}

	public ValueSlider(string name, float xpos, ref float ypos)
	{
		sliderBoxRect = new Rect(xpos, ypos, 250f, 50f);
		valueLabelRect = new Rect(xpos + 4f, ypos + 22f, 50f, 30f);
		sliderRect = new Rect(xpos + 34f, ypos + 27f, 200f, 15f);
		ypos += 55f;
		m_Name = name;
	}
}
