using UnityEngine;

public class ReplayButton : MonoBehaviour
{
	public AutoSpriteBase m_Icon;

	private void OnEnable()
	{
	}

	private bool IsMacawAvailable()
	{
		return CheckPointController.Instance().HasPlayerPassedValidCheckpoint();
	}

	public void Update()
	{
		if (!IsMacawAvailable() || !SecureStorage.Instance.HasMacaws)
		{
			float num = Mathf.Sin(Time.realtimeSinceStartup * 3f) * 0.5f + 0.5f;
			float a = 0.25f + 0.75f * num;
			m_Icon.SetColor(new Color(1f, 1f, 1f, a));
		}
		else
		{
			m_Icon.SetColor(new Color(1f, 1f, 1f, 1f));
		}
	}
}
