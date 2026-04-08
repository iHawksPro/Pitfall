using UnityEngine;

public class FollowPosition : MonoBehaviour
{
	public GameObject m_FollowButton;

	private Vector2 m_Offset;

	private Vector3 m_cachedLocalPos = new Vector3(-1000f, -1000f, -1000f);

	public void Setup(GameObject objectToFollow, float x, float y)
	{
		m_FollowButton = objectToFollow;
		m_Offset.x = x;
		m_Offset.y = y;
	}

	private void Awake()
	{
		m_Offset = new Vector2(0f, 0f);
	}

	public void UpdatePos()
	{
		if ((bool)m_FollowButton)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = m_FollowButton.transform.localPosition.x + m_Offset.x;
			localPosition.y = m_FollowButton.transform.localPosition.y + m_Offset.y;
			if (m_cachedLocalPos != localPosition)
			{
				base.transform.localPosition = localPosition;
				m_cachedLocalPos = localPosition;
			}
		}
	}
}
