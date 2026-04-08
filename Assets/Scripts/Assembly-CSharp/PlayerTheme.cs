using System;

[Serializable]
public class PlayerTheme
{
	public enum ThemeType
	{
		Run = 0,
		Bike = 1,
		Cart = 2,
		Slope = 3,
		Jaguar = 4,
		None = 5
	}

	public PlayerThemeSettings m_Run;

	public PlayerThemeSettings m_Bike;

	public PlayerThemeSettings m_Cart;

	public PlayerThemeSettings m_Slope;

	public PlayerThemeSettings m_Jaguar;

	private ThemeType m_ThemeType;

	public PlayerTheme()
	{
		m_ThemeType = ThemeType.None;
	}

	public ThemeType Current()
	{
		return m_ThemeType;
	}

	public void SetThemeType(ThemeType type)
	{
		m_ThemeType = type;
	}

	public PlayerThemeSettings CurrentTheme()
	{
		return GetTheme(m_ThemeType);
	}

	public PlayerThemeSettings GetTheme(ThemeType themeType)
	{
		switch (themeType)
		{
		case ThemeType.Run:
			return m_Run;
		case ThemeType.Cart:
			return m_Cart;
		case ThemeType.Bike:
			return m_Bike;
		case ThemeType.Slope:
			return m_Slope;
		case ThemeType.Jaguar:
			return m_Jaguar;
		default:
			return m_Run;
		}
	}
}
