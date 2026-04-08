using UnityEngine;

public class RotatingElement : MonoBehaviour
{
	public float m_Speed = 30f;

	public bool m_RotateConstantly = true;

	private float m_RotateTime;

	private AutoSpriteBase m_Sprite;

	private float m_Rot;

	private void Awake()
	{
		m_Rot = 0f;
		m_RotateTime = 0f;
		m_Sprite = base.transform.GetComponent<AutoSpriteBase>();
	}

	private void Update()
	{
		bool flag = false;
		if (!m_RotateConstantly)
		{
			if (m_RotateTime > 0f)
			{
				m_RotateTime -= Time.deltaTime;
				flag = true;
				if (m_Sprite != null && m_Sprite.Color.a < 1f)
				{
					float a = Mathf.Min(1f, m_Sprite.Color.a + Time.deltaTime);
					m_Sprite.SetColor(new Color(m_Sprite.Color.r, m_Sprite.Color.g, m_Sprite.Color.b, a));
				}
			}
			else if (m_Sprite != null && m_Sprite.Color.a > 0f)
			{
				float a2 = Mathf.Max(0f, m_Sprite.Color.a - Time.deltaTime);
				m_Sprite.SetColor(new Color(m_Sprite.Color.r, m_Sprite.Color.g, m_Sprite.Color.b, a2));
				flag = true;
			}
		}
		if (m_RotateConstantly || flag)
		{
			m_Rot += Time.deltaTime * m_Speed;
			base.transform.eulerAngles = new Vector3(0f, 0f, m_Rot);
		}
	}

	public void StartRotating(float time)
	{
		m_RotateTime = time;
	}
}
