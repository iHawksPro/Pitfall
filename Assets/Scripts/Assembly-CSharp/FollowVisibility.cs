using UnityEngine;

public class FollowVisibility : MonoBehaviour
{
	public GameObject m_FollowObject;

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
		if ((bool)m_FollowObject)
		{
			base.gameObject.SetActiveRecursively(m_FollowObject.active);
			base.gameObject.active = true;
		}
	}
}
