using UnityEngine;

public class StoreMarker : MonoBehaviour
{
	public SpriteText m_NameText;

	public SpriteText m_DistanceText;

	public void SetNameAndDist(string name, int dist)
	{
		m_NameText.Text = name;
		m_DistanceText.Text = string.Format("{0}m", dist);
	}
}
