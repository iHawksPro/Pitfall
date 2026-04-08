using UnityEngine;

public class Stamp : MonoBehaviour
{
	private int myNPStatus = -1;

	public GameObject m_NinjaStamp;

	public GameObject m_PirateStamp;

	private void LateUpdate()
	{
		if (m_NinjaStamp.active)
		{
			m_NinjaStamp.SetActiveRecursively(false);
		}
		if (m_PirateStamp.active)
		{
			m_PirateStamp.SetActiveRecursively(false);
		}
	}
}
