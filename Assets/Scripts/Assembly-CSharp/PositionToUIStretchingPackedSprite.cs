using UnityEngine;

[AddComponentMenu("TBF/PositiontoUISPS")]
public class PositionToUIStretchingPackedSprite : MonoBehaviour
{
	public UIStretchingPackedSprite m_PositionToObject;

	public UIStretchingPackedSprite m_AdjustForObject;

	public bool bKeepY;

	public void LateUpdate()
	{
		if ((bool)m_PositionToObject)
		{
			float num = 0.5f * m_PositionToObject.m_Middle.transform.localScale.x;
			Vector3 position = m_PositionToObject.m_Left.transform.position;
			position.x += num;
			if ((bool)m_AdjustForObject)
			{
				float num2 = 0.5f * m_AdjustForObject.m_Middle.transform.localScale.x;
				position.x += num2 - num;
			}
			if (bKeepY)
			{
				base.transform.position = new Vector3(position.x, base.transform.position.y, base.transform.position.z);
			}
			else
			{
				base.transform.position = new Vector3(position.x, position.y, base.transform.position.z);
			}
		}
	}
}
