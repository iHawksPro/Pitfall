using System.Collections.Generic;
using UnityEngine;

public class DebugSpam : MonoBehaviour
{
	private class TimeStampMessage
	{
		public string mMessage;

		public float mTime;

		public TimeStampMessage(string messageText)
		{
			mMessage = messageText;
			mTime = Time.time;
		}
	}

	public static DebugSpam instance;

	public TextAnchor msgAnchorAt = TextAnchor.MiddleRight;

	public TextAnchor authoredAnchorAt = TextAnchor.LowerCenter;

	public int numberOfLines = 10;

	public int pixelOffset = 5;

	public float timeOut = 5f;

	private GameObject msgGuiObj;

	private LegacyGUIText msgGuiTxt;

	private GameObject setPieceGuiObj;

	private LegacyGUIText setPieceGuiTxt;

	private TextAnchor currentAnchorAt;

	private float currentPixelOffset;

	private List<TimeStampMessage> mMessageHistory;

	private string displayText;

	private string mLengthDebug;

	private void Awake()
	{
		instance = this;
		mMessageHistory = new List<TimeStampMessage>();
		msgGuiObj = new GameObject("Player Speed Spam");
		msgGuiObj.AddComponent<LegacyGUIText>();
		msgGuiObj.transform.position = Vector3.zero;
		msgGuiObj.transform.localScale = new Vector3(0f, 0f, 1f);
		msgGuiObj.name = "Debug Spam";
		msgGuiTxt = msgGuiObj.GetComponent<LegacyGUIText>();
		currentAnchorAt = msgAnchorAt;
		SetPosition(msgAnchorAt, msgGuiObj, msgGuiTxt, pixelOffset);
		setPieceGuiObj = new GameObject("Authored Spam");
		setPieceGuiObj.AddComponent<LegacyGUIText>();
		setPieceGuiObj.transform.position = Vector3.zero;
		setPieceGuiObj.transform.localScale = new Vector3(0f, 0f, 1f);
		setPieceGuiObj.name = "Debug Spam";
		setPieceGuiTxt = setPieceGuiObj.GetComponent<LegacyGUIText>();
		SetPosition(authoredAnchorAt, setPieceGuiObj, setPieceGuiTxt, pixelOffset);
		mLengthDebug = string.Empty;
	}

	private void Start()
	{
	}

	public static void Output(string output)
	{
		TimeStampMessage item = new TimeStampMessage(output + " (" + Time.frameCount + ")");
		instance.mMessageHistory.Insert(0, item);
		instance.UpdateText();
	}

	public static void OutputAuthored(bool authored)
	{
		instance.UpdateAuthoredText(authored);
	}

	private void Update()
	{
		if (currentAnchorAt != msgAnchorAt || currentPixelOffset != (float)pixelOffset)
		{
			currentAnchorAt = msgAnchorAt;
			currentPixelOffset = pixelOffset;
			SetPosition(msgAnchorAt, msgGuiObj, msgGuiTxt, pixelOffset);
		}
		while (mMessageHistory.Count > numberOfLines || (mMessageHistory.Count > 0 && Time.time - mMessageHistory[mMessageHistory.Count - 1].mTime > timeOut))
		{
			mMessageHistory.RemoveAt(mMessageHistory.Count - 1);
			UpdateText();
		}
	}

	private void UpdateText()
	{
		displayText = string.Empty;
		for (int i = 0; i < mMessageHistory.Count; i++)
		{
			if (i == 0)
			{
				displayText = mMessageHistory[i].mMessage;
			}
			else
			{
				displayText = mMessageHistory[i].mMessage + "\n" + displayText;
			}
		}
		msgGuiTxt.text = displayText;
	}

	private void UpdateAuthoredText(bool authored)
	{
		setPieceGuiTxt.text = ((!authored) ? "AUTO-GENERATED" : "AUTHORED") + " " + mLengthDebug;
	}

	public void OnDisable()
	{
		if (msgGuiObj != null)
		{
			Object.DestroyImmediate(msgGuiObj.gameObject);
		}
	}

	public static void SetLengthDebug(string lengthDebug)
	{
		instance.mLengthDebug = lengthDebug;
	}

	public static void SetPosition(TextAnchor anchorAt, GameObject guiObj, LegacyGUIText guiTxt, int pixelOffset)
	{
		switch (anchorAt)
		{
		case TextAnchor.UpperLeft:
			guiObj.transform.position = new Vector3(0f, 1f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Left;
			guiTxt.pixelOffset = new Vector2(pixelOffset, -pixelOffset);
			break;
		case TextAnchor.UpperCenter:
			guiObj.transform.position = new Vector3(0.5f, 1f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Center;
			guiTxt.pixelOffset = new Vector2(0f, -pixelOffset);
			break;
		case TextAnchor.UpperRight:
			guiObj.transform.position = new Vector3(1f, 1f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Right;
			guiTxt.pixelOffset = new Vector2(-pixelOffset, -pixelOffset);
			break;
		case TextAnchor.MiddleLeft:
			guiObj.transform.position = new Vector3(0f, 0.5f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Left;
			guiTxt.pixelOffset = new Vector2(pixelOffset, 0f);
			break;
		case TextAnchor.MiddleCenter:
			guiObj.transform.position = new Vector3(0.5f, 0.5f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Center;
			guiTxt.pixelOffset = new Vector2(0f, 0f);
			break;
		case TextAnchor.MiddleRight:
			guiObj.transform.position = new Vector3(1f, 0.5f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Right;
			guiTxt.pixelOffset = new Vector2(-pixelOffset, 0f);
			break;
		case TextAnchor.LowerLeft:
			guiObj.transform.position = new Vector3(0f, 0f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Left;
			guiTxt.pixelOffset = new Vector2(pixelOffset, pixelOffset);
			break;
		case TextAnchor.LowerCenter:
			guiObj.transform.position = new Vector3(0.5f, 0f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Center;
			guiTxt.pixelOffset = new Vector2(0f, pixelOffset);
			break;
		case TextAnchor.LowerRight:
			guiObj.transform.position = new Vector3(1f, 0f, 0f);
			guiTxt.anchor = anchorAt;
			guiTxt.alignment = TextAlignment.Right;
			guiTxt.pixelOffset = new Vector2(-pixelOffset, pixelOffset);
			break;
		}
	}
}
