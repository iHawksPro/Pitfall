using UnityEngine;

public class HarrySprite : MonoBehaviour
{
	public SpriteRoot[] m_frames;

	private int m_currentFrame = -1;

	private void Start()
	{
		m_currentFrame = -1;
		SetFrame(0);
	}

	public int GetFrame()
	{
		return m_currentFrame;
	}

	public void SetFrame(int NewFrame)
	{
		for (int i = 0; i < m_frames.Length; i++)
		{
			if (m_frames[i] != null)
			{
				m_frames[i].GetComponent<Renderer>().enabled = false;
			}
		}
		if (NewFrame >= 0 && NewFrame < m_frames.Length && m_frames[NewFrame] != null)
		{
			m_frames[NewFrame].GetComponent<Renderer>().enabled = true;
		}
		m_currentFrame = NewFrame;
	}

	public int GetFrameCount()
	{
		return m_frames.Length;
	}

	private void Update()
	{
	}
}
