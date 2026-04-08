using UnityEngine;

public class AnimDebug : MonoBehaviour
{
	public int XPos = 20;

	public int YPos = 40;

	private Rect boxRect;

	private Rect sliderBoxRect;

	private Rect timeLabelRect;

	private Rect sliderRect;

	private Rect stopRect;

	private Rect stepRect;

	private Rect recRect;

	private AnimationState stateRef;

	private string debugText;

	private bool paused;

	private bool step;

	private float timeScale = 1f;

	private float gameTime;

	private bool animDump;

	private void Start()
	{
		boxRect = new Rect(XPos, YPos, 360f, 120f);
		sliderBoxRect = new Rect(XPos, YPos + 128, 360f, 24f);
		timeLabelRect = new Rect(XPos + 4, YPos + 130, 50f, 30f);
		sliderRect = new Rect(XPos + 34, YPos + 135, 120f, 15f);
		stopRect = new Rect(XPos + 185, YPos + 133, 24f, 15f);
		stepRect = new Rect(XPos + 210, YPos + 133, 24f, 15f);
		recRect = new Rect(XPos + 160, YPos + 133, 24f, 15f);
	}

	private void OnGUI()
	{
		debugText = string.Empty;
		Animation component = GetComponent<Animation>();
		foreach (AnimationState item in component)
		{
			if (component.IsPlaying(item.name))
			{
				debugText = debugText + GetTextDesc(item) + "\n";
			}
		}
		GUI.Box(boxRect, debugText);
		GUI.Box(sliderBoxRect, string.Empty);
		GUI.Label(timeLabelRect, string.Empty + timeScale.ToString("#.00"));
		timeScale = GUI.HorizontalSlider(sliderRect, timeScale, 0f, 1f);
		if (animDump)
		{
			if (GUI.Button(recRect, "-"))
			{
				animDump = false;
			}
		}
		else if (GUI.Button(recRect, "d"))
		{
			animDump = true;
		}
		if (paused)
		{
			if (GUI.Button(stopRect, "-"))
			{
				paused = false;
			}
		}
		else if (GUI.Button(stopRect, "[]"))
		{
			paused = true;
		}
		if (GUI.Button(stepRect, ">") && paused)
		{
			gameTime = Time.time;
			step = true;
		}
		if (step)
		{
			Time.timeScale = timeScale;
			if (Time.time > gameTime)
			{
				step = false;
			}
		}
		else if (paused)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = timeScale;
		}
		if (animDump && !paused)
		{
			Debug.Log("----------------------------------");
			Debug.Log(debugText);
		}
	}

	private string GetTextDesc(AnimationState st)
	{
		float length = st.length;
		float time = st.time;
		string text = st.name;
		WrapMode wrapMode = st.wrapMode;
		float weight = st.weight;
		int layer = st.layer;
		return string.Format("{0}: {1} {2:0.00f}/{3:0.00f} {4:0.0f} ({5})", text, wrapMode, time, length, weight, layer);
	}
}
