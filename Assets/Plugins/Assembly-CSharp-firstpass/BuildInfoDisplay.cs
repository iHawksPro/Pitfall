using UnityEngine;

public class BuildInfoDisplay : MonoBehaviour
{
	public int X;

	public int Y;

	private void OnGUI()
	{
		GUI.Box(new Rect(X, Y, 120f, 70f), "Build Info");
		GUI.Label(new Rect(X + 10, Y + 20, 100f, 25f), "Date: " + TBFUtils.BuildDate);
		GUI.Label(new Rect(X + 10, Y + 35, 100f, 25f), "Time: " + TBFUtils.BuildTime);
	}
}
