using System;
using System.Collections.Generic;

[Serializable]
public class WorldLayout
{
	public List<WorldLayoutElement> SectionTypeList;

	public WorldLayout()
	{
		SectionTypeList = new List<WorldLayoutElement>();
	}

	public void Reset()
	{
		SectionTypeList.Clear();
	}

	public void Add(WorldConstructionHelper.PieceType type)
	{
		WorldLayoutElement item = new WorldLayoutElement(type);
		SectionTypeList.Add(item);
	}

	public void Add(WorldConstructionHelper.PieceType type, int length, bool adjustable)
	{
		WorldLayoutElement worldLayoutElement = new WorldLayoutElement(type, length, false);
		worldLayoutElement.MinimumLength = ((!adjustable) ? length : 0);
		SectionTypeList.Add(worldLayoutElement);
	}

	public void Add(WorldConstructionHelper.PieceType type, int length, int minimumLength)
	{
		WorldLayoutElement worldLayoutElement = new WorldLayoutElement(type, length, false);
		worldLayoutElement.MinimumLength = minimumLength;
		SectionTypeList.Add(worldLayoutElement);
	}

	public int GetSize()
	{
		return SectionTypeList.Count;
	}

	public WorldConstructionHelper.PieceType GetElementType(int index)
	{
		return SectionTypeList[index].GetElementType();
	}

	public int GetElementMinimumLength(int index)
	{
		return SectionTypeList[index].MinimumLength;
	}

	public int GetElementLength(int index)
	{
		return SectionTypeList[index].GetLength();
	}
}
