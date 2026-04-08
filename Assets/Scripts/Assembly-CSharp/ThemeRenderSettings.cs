using System;
using UnityEngine;

[Serializable]
public class ThemeRenderSettings
{
	public WorldConstructionHelper.Theme m_theme;

	public Color m_ambientColour;

	public bool m_fogEnabled;

	public Color m_fogColour;

	public float m_fogStart;

	public float m_fogEnd;

	public ThemeRenderSettings(WorldConstructionHelper.Theme theme, Color ambientColor, bool fogEnabled, Color fogColor, float fogStart, float fogEnd)
	{
		m_theme = theme;
		m_ambientColour = ambientColor;
		m_fogEnabled = fogEnabled;
		m_fogColour = fogColor;
		m_fogStart = fogStart;
		m_fogEnd = fogEnd;
	}
}
