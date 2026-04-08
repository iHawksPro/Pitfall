using UnityEngine;

public class HudFps : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private void Start()
	{
		if (!GetComponent<LegacyGUIText>())
		{
			Debug.Log("UtilityFramesPerSecond needs a LegacyGUIText component!");
			base.enabled = false;
			return;
		}
		timeleft = updateInterval;
		Vector2 pixelOffset = GetComponent<LegacyGUIText>().pixelOffset;
		pixelOffset.y = (float)Screen.height / 2f;
		GetComponent<LegacyGUIText>().pixelOffset = pixelOffset;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			string text = string.Format("{0:F2} FPS", num);
			GetComponent<LegacyGUIText>().text = text;
			GetComponent<LegacyGUIText>().material.color = Color.green;
			if (num < 30f)
			{
				GetComponent<LegacyGUIText>().material.color = Color.yellow;
			}
			else if (num < 10f)
			{
				GetComponent<LegacyGUIText>().material.color = Color.red;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
