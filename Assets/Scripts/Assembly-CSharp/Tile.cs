using System;
using System.Collections.Generic;

[Serializable]
public class Tile
{
	public enum ResponseType
	{
		Tap = 0,
		SwipeUp = 1,
		SwipeDown = 2,
		SwipeLeft = 3,
		SwipeRight = 4,
		Empty = 5,
		SwipeLeftPrepare = 6,
		SwipeRightPrepare = 7,
		EndOfPath = 8,
		RopeSwing = 9,
		Kill = 10,
		KillLeft = 11,
		KillRight = 12,
		BlockLeft = 13,
		BlockRight = 14,
		Total = 15
	}

	public List<ResponseType> Types = new List<ResponseType>();

	public Tile(string typeString)
	{
		string[] array = typeString.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.ToLowerInvariant();
			if (text2.Equals("up"))
			{
				SetType(ResponseType.SwipeUp);
			}
			else if (text2.Equals("down"))
			{
				SetType(ResponseType.SwipeDown);
			}
			else if (text2.Equals("left"))
			{
				SetType(ResponseType.SwipeLeft);
			}
			else if (text2.Equals("right"))
			{
				SetType(ResponseType.SwipeRight);
			}
			else if (text2.Equals("intoLeft"))
			{
				SetType(ResponseType.SwipeLeftPrepare);
			}
			else if (text2.Equals("intoRight"))
			{
				SetType(ResponseType.SwipeRightPrepare);
			}
			else if (text2.Equals("rope"))
			{
				SetType(ResponseType.RopeSwing);
			}
			else if (text2.Equals("kill"))
			{
				SetType(ResponseType.Kill);
			}
			else if (text2.Equals("kill_left"))
			{
				SetType(ResponseType.KillLeft);
			}
			else if (text2.Equals("kill_right"))
			{
				SetType(ResponseType.KillRight);
			}
			else if (text2.Equals("block_left"))
			{
				SetType(ResponseType.BlockLeft);
			}
			else if (text2.Equals("block_right"))
			{
				SetType(ResponseType.BlockRight);
			}
		}
		if (Types.Count == 0)
		{
			SetType(ResponseType.Empty);
		}
	}

	public void SetType(ResponseType type)
	{
		Types.Add(type);
	}

	public bool IsOfType(ResponseType type)
	{
		for (int i = 0; i < Types.Count; i++)
		{
			if (Types[i] == type)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsRequiringSwipe()
	{
		return IsOfType(ResponseType.SwipeUp) || IsOfType(ResponseType.SwipeDown) || IsOfType(ResponseType.SwipeLeft) || IsOfType(ResponseType.SwipeRight);
	}
}
