using UnityEngine;

public class FollowCentre : MonoBehaviour
{
	public UIButton m_FollowButton;

	private void Awake()
	{
	}

	public void Start()
	{
	}

	public void Update()
	{
	}

	public void LateUpdate()
	{
		if ((bool)m_FollowButton)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = m_FollowButton.transform.localPosition.x + m_FollowButton.width * 0.5f;
			if (base.transform.localPosition != localPosition)
			{
				base.transform.localPosition = localPosition;
			}
		}
	}
}
