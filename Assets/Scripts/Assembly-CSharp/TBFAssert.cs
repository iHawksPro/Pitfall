using System;
using UnityEngine;

public class TBFAssert : MonoBehaviour
{
	public SpriteText Title;

	public SpriteText BuildNumber;

	public SpriteText CurrentDateTime;

	public SpriteText Message;

	private static string mStackTrace;

	private static void SetStackTrace()
	{
		int num = 3;
		string[] array = Environment.StackTrace.Split('\n');
		for (int i = num; i < array.Length; i++)
		{
			mStackTrace += array[i];
		}
	}

	public static void DoAssert(bool check)
	{
		if (!check)
		{
			SetStackTrace();
			FireAssert("assert", "0", "an assert fired");
		}
	}

	public static void DoAssert(bool check, string title, string message)
	{
		if (!check)
		{
			SetStackTrace();
			FireAssert(title, "0", message);
		}
	}

	private static void FireAssert(string title, string buildId, string message)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object obj in array2)
		{
			GameObject gameObject = obj as GameObject;
			if ((bool)gameObject)
			{
				gameObject.active = false;
			}
		}
		UnityEngine.Object original = Resources.Load("Debug/Assert");
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original, new Vector3(0f, 0f, -1000f), new Quaternion(0f, 0f, 0f, 1f));
		TBFAssert component = gameObject2.GetComponent<TBFAssert>();
		component.Set(title, buildId, message);
		Time.timeScale = 0f;
	}

	public void Set(string title, string buildId, string message)
	{
		Title.Text = "ASSERT: " + title;
		BuildNumber.Text = "Build: " + buildId;
		CurrentDateTime.Text = DateTime.Now.ToString();
		Message.Text = message + "\n" + mStackTrace;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
