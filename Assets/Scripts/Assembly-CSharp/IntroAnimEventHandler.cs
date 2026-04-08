using Bresenhams;
using UnityEngine;

public class IntroAnimEventHandler : MonoBehaviour
{
	private const int nRopePixels = 100;

	public GameObject m_ropeTop;

	public GameObject m_ropeEnd;

	public GameObject m_ropePixelPrefab;

	public SoundFXData m_ropeSwing;

	public SoundFXData m_jump;

	public SoundFXData m_damage;

	private GameObject[] m_ropePixels = new GameObject[100];

	private bool m_ropeEnabled;

	private float m_ropePixelSize;

	private int m_currentRopePixel;

	private void Start()
	{
		InitRopePixels();
	}

	private void LateUpdate()
	{
		if (m_ropeEnabled)
		{
			DrawRopePixels();
		}
	}

	public void RopeOn()
	{
		TBFUtils.DebugLog("Rope On");
		m_ropeEnabled = true;
		DisableRopePixels();
		SoundManager.Instance.Play2D(m_ropeSwing);
	}

	public void RopeOff()
	{
		TBFUtils.DebugLog("Rope Off");
		m_ropeEnabled = false;
		DisableRopePixels();
	}

	public void Jump()
	{
		SoundManager.Instance.Play2D(m_jump);
	}

	public void Damage()
	{
	}

	private void InitRopePixels()
	{
		for (int i = 0; i < 100; i++)
		{
			m_ropePixels[i] = (GameObject)Object.Instantiate(m_ropePixelPrefab);
			m_ropePixels[i].name = "RopePixel_" + i;
			m_ropePixels[i].transform.position = Vector3.zero;
			m_ropePixels[i].transform.parent = base.transform;
		}
		m_ropePixelSize = m_ropePixels[0].transform.localScale.x;
		m_ropeEnabled = false;
	}

	private void DisableRopePixels()
	{
		for (int i = 0; i < 100; i++)
		{
			m_ropePixels[i].transform.position = Vector3.zero;
			m_ropePixels[i].GetComponent<Renderer>().enabled = false;
		}
	}

	private void WorldToPixel(Vector3 worldPos, out int pixelX, out int pixelY)
	{
		Vector3 vector = worldPos - m_ropeTop.transform.position;
		pixelX = Mathf.RoundToInt(vector.x / m_ropePixelSize);
		pixelY = Mathf.RoundToInt(vector.y / m_ropePixelSize);
	}

	private Vector3 PixelToWorld(int x, int y)
	{
		Vector3 position = m_ropeTop.transform.position;
		return new Vector3
		{
			x = position.x + (float)x * m_ropePixelSize,
			y = position.y + (float)y * m_ropePixelSize,
			z = position.z
		};
	}

	private bool RopePlot(int xPos, int yPos)
	{
		Vector3 position = PixelToWorld(xPos, yPos);
		m_ropePixels[m_currentRopePixel].transform.position = position;
		m_ropePixels[m_currentRopePixel].GetComponent<Renderer>().enabled = true;
		m_currentRopePixel++;
		return m_currentRopePixel < 99;
	}

	private void DrawRopePixels()
	{
		int pixelX = 0;
		int pixelY = 0;
		int pixelX2 = 0;
		int pixelY2 = 0;
		WorldToPixel(m_ropeTop.transform.position, out pixelX, out pixelY);
		WorldToPixel(m_ropeEnd.transform.position, out pixelX2, out pixelY2);
		DisableRopePixels();
		m_currentRopePixel = 0;
		Algorithms.Line(pixelX, pixelY, pixelX2, pixelY2, RopePlot);
	}
}
