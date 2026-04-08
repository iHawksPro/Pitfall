using System.Collections.Generic;
using UnityEngine;

public class HudTileFeed : MonoBehaviour
{
	public float updateInterval = 0.5f;

	public int MaxTileHistory = 10;

	private float mTimeLeft;

	private string mFormat;

	private List<string> mHistory;

	private void Start()
	{
		if (!GetComponent<LegacyGUIText>())
		{
			Debug.Log("HudTileFeed needs a LegacyGUIText component!");
			base.enabled = false;
			return;
		}
		mFormat = GetComponent<LegacyGUIText>().text;
		mTimeLeft = updateInterval;
		GetComponent<LegacyGUIText>().material.color = Color.blue;
		Vector2 pixelOffset = GetComponent<LegacyGUIText>().pixelOffset;
		pixelOffset.x = (float)Screen.width / 3f;
		GetComponent<LegacyGUIText>().pixelOffset = pixelOffset;
		mHistory = new List<string>();
	}

	private void Update()
	{
		mTimeLeft -= Time.deltaTime;
		if (!((double)mTimeLeft <= 0.0))
		{
			return;
		}
		string text = "Unknown";
		string text2 = "Unknown";
		string text3 = "Unknown";
		if ((bool)LevelGenerator.Instance())
		{
			PieceDescriptor currentPiece = LevelGenerator.Instance().GetCurrentPiece();
			if (currentPiece != null)
			{
				text = currentPiece.Theme.ToString();
				text2 = currentPiece.Group.ToString();
				text3 = currentPiece.name;
				if (mHistory.Count == 0 || mHistory[mHistory.Count - 1] != text3)
				{
					mHistory.Add(text3);
					if (mHistory.Count > MaxTileHistory)
					{
						mHistory.RemoveAt(0);
					}
					string text4 = text + mFormat + text2 + mFormat + mFormat;
					for (int num = mHistory.Count - 1; num >= 0; num--)
					{
						text4 = text4 + mHistory[num] + mFormat;
					}
					GetComponent<LegacyGUIText>().text = text4;
				}
			}
		}
		mTimeLeft = updateInterval;
	}
}
