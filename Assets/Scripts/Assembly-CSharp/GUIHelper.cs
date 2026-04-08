using UnityEngine;

public static class GUIHelper
{
	private static float m_Height;

	private static float m_Adjust = 1f;

	private static float m_XAdjust = 1f;

	private static float m_InvAdjust = 1f;

	public static bool IsScaledUI()
	{
		return false;
	}

	public static float GetScreenWidth()
	{
		if (Screen.height > Screen.width)
		{
			return Screen.height;
		}
		return Screen.width;
	}

	public static float GetScreenHeight()
	{
		if (Screen.height > Screen.width)
		{
			return Screen.width;
		}
		return Screen.height;
	}

	public static void CheckForResChange()
	{
		float screenHeight = GetScreenHeight();
		if (m_Height != screenHeight)
		{
			m_Height = screenHeight;
			m_Adjust = screenHeight / 768f;
			m_InvAdjust = 768f / screenHeight;
			m_XAdjust = GetScreenWidth() / GetScreenHeight() / 1.3333f;
		}
	}

	public static float Adjustment()
	{
		CheckForResChange();
		return m_Adjust;
	}

	public static float InvAdjustment()
	{
		CheckForResChange();
		return m_InvAdjust;
	}

	public static float AdjustYForAspect(float initialY)
	{
		CheckForResChange();
		return initialY;
	}

	public static float AdjustXForAspect(float initialX, bool hasMovement)
	{
		CheckForResChange();
		if (!hasMovement)
		{
			return initialX;
		}
		return initialX * m_XAdjust;
	}
}
