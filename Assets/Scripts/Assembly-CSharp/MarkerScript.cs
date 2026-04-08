using UnityEngine;

public class MarkerScript : MonoBehaviour
{
	public float DestroyPositionY;

	public float SpeedModifier = 2.1f;

	private float m_MapLen;

	public float m_Distance;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("DistanceMapPrefab");
		if ((bool)gameObject)
		{
			DestroyPositionY = gameObject.transform.position.y;
		}
	}

	public void SetUp(float dist, float maplen)
	{
		m_MapLen = maplen;
		m_Distance = dist;
	}

	private void Update()
	{
		PlayerController playerController = PlayerController.Instance();
		float num = playerController.Score().DistanceTravelled();
		if (playerController.IsDead() || GameController.Instance.Paused())
		{
			return;
		}
		if (num > m_Distance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Vector3 localPosition = base.gameObject.transform.localPosition;
		float num2 = 1f;
		if (GUIHelper.GetScreenHeight() == 480f)
		{
			num2 = 0.75f;
		}
		else if (GUIHelper.GetScreenHeight() == 320f)
		{
			num2 = 0.5f;
		}
		localPosition.y = num2 * (m_MapLen * (m_Distance - num)) / 4000f;
		localPosition.z = -10f;
		if (m_Distance - num > 4000f)
		{
			localPosition.y += 10000f;
		}
		base.gameObject.transform.localPosition = localPosition;
	}
}
