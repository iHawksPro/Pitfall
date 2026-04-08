using UnityEngine;

public class BouncyArrow : MonoBehaviour
{
	private float m_OriginalDownArrowY;

	public float m_Amount = 30f;

	public float m_Speed = 10f;

	private void Awake()
	{
		m_OriginalDownArrowY = base.gameObject.transform.localPosition.y;
	}

	private void Start()
	{
	}

	private void Update()
	{
		float y = m_OriginalDownArrowY + 30f + Mathf.Cos(Time.realtimeSinceStartup * m_Speed) * m_Amount;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, y, base.transform.localPosition.z);
	}
}
