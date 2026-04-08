using System;
using UnityEngine;

public class Sparkle : MonoBehaviour
{
	private SimpleSprite m_Sprite;

	public float m_Speed = 1f;

	public float m_MaxAlpha = 1f;

	public float m_Radius = 80f;

	private void Start()
	{
		m_Sprite = base.transform.GetComponent<SimpleSprite>();
		RandomPos();
	}

	private void Update()
	{
		m_Sprite.SetColor(new Color(1f, 1f, 1f, m_MaxAlpha * Mathf.Sin(Time.realtimeSinceStartup * m_Speed)));
		if (m_Sprite.Color.a < 0.02f)
		{
			RandomPos();
		}
	}

	private void RandomPos()
	{
		float f = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
		base.transform.localPosition = new Vector3(Mathf.Cos(f) * m_Radius, Mathf.Sin(f) * m_Radius, base.transform.localPosition.z);
	}
}
