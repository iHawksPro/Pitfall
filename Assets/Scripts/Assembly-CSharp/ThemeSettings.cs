using System;

[Serializable]
public class ThemeSettings
{
	public WorldConstructionHelper.Theme m_theme;

	public string m_scene;

	public ThemeSettings(WorldConstructionHelper.Theme theme, string sceneName)
	{
		m_theme = theme;
		m_scene = sceneName;
	}
}
