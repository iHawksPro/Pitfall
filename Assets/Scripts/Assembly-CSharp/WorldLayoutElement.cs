using System;
using UnityEngine;

[Serializable]
public class WorldLayoutElement
{
	public WorldConstructionHelper.PieceType Type;

	public int Length;

	public PieceDescriptor LastPiecePlaced;

	public int MinimumLength;

	public WorldLayoutElement(WorldConstructionHelper.PieceType type, int length, bool allowLengthScaling)
	{
		Type = type;
		MinimumLength = length;
		if (length == -1)
		{
			length = UnityEngine.Random.Range(5, 15);
		}
		if (allowLengthScaling && type == WorldConstructionHelper.PieceType.Straight)
		{
			float lengthScaling = WorldConstructionHelper.GetLengthScaling(length);
			length = (int)((float)length * lengthScaling);
		}
		Length = length;
	}

	public WorldLayoutElement(WorldConstructionHelper.PieceType type)
	{
		Type = type;
		Length = -1;
		MinimumLength = -1;
	}

	public WorldConstructionHelper.PieceType GetElementType()
	{
		return Type;
	}

	public int GetLength()
	{
		return Length;
	}

	public int GetMinimumLength()
	{
		return MinimumLength;
	}

	public void ReduceLength(int amount)
	{
		Length -= amount;
		if (Length < 0)
		{
			Length = 0;
		}
	}
}
