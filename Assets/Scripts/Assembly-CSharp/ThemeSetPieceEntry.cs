using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThemeSetPieceEntry
{
	public WorldConstructionHelper.Theme Theme;

	public string AuthoredSetPieceScene;

	public float MinimumDistanceBetweenSetPieces = 200f;

	public float MaximumDistanceBetweenSetPieces = 500f;

	public bool AdjustStraightLengthForPlayerSpeed;

	private PieceSet mPieceSet;

	private List<int> mIndexList;

	private float mAddedLength;

	private int mNextSectionForTheme;

	private int mLastElementForTheme = -1;

	private bool mFinished;

	public bool IsFinished
	{
		get
		{
			return mFinished;
		}
	}

	public void SetPieceSet(PieceSet pieces)
	{
		mPieceSet = pieces;
		if (mPieceSet != null && mPieceSet.AuthoredSetPieces != null)
		{
			foreach (AuthoredLevelLayoutSection section in mPieceSet.AuthoredSetPieces.SectionList)
			{
				section.OnLoad(mPieceSet);
				if (section.ElementList.Count > 0 && !mPieceSet.AuthoredSetPieces.TestWorld)
				{
					PieceDescriptor elementCached = section.ElementList[0].GetElementCached();
					if (elementCached.TypeId == WorldConstructionHelper.PieceType.ThemeTransition && elementCached.ThemeTransitionEntry != Theme)
					{
						section.ElementList.RemoveAt(0);
					}
				}
				section.DifficultyGroups.Clear();
				foreach (AuthoredLevelLayoutElement element in section.ElementList)
				{
					PieceDescriptor elementCached2 = element.GetElementCached();
					foreach (int difficultyGroup in elementCached2.DifficultyGroups)
					{
						if (!section.DifficultyGroups.Contains(difficultyGroup))
						{
							section.DifficultyGroups.Add(difficultyGroup);
						}
					}
				}
			}
		}
		if (mIndexList == null || mIndexList.Count == 0)
		{
			mIndexList = new List<int>();
			Shuffle(0, mPieceSet.AuthoredSetPieces.SectionList.Count);
		}
		mAddedLength = 0f;
		mLastElementForTheme = -1;
		mFinished = false;
	}

	public void ClearPieceSet()
	{
		mPieceSet = null;
	}

	public WorldConstructionHelper.Group FirstGroup()
	{
		return FirstGroup(mIndexList[mNextSectionForTheme]);
	}

	public WorldConstructionHelper.Group FirstGroup(int section)
	{
		WorldConstructionHelper.Group result = WorldConstructionHelper.Group.Invalid;
		if (mPieceSet != null && mPieceSet.AuthoredSetPieces != null && mPieceSet.AuthoredSetPieces.SectionList.Count > section && mPieceSet.AuthoredSetPieces.SectionList[section].ElementList.Count > 0)
		{
			result = mPieceSet.AuthoredSetPieces.SectionList[section].ElementList[0].Group;
		}
		return result;
	}

	public PieceDescriptor CreateNextPiece(PieceSet pieces, List<PieceDescriptor> validPieces, float desiredLength)
	{
		PieceDescriptor pieceDescriptor = null;
		if (mNextSectionForTheme >= mIndexList.Count)
		{
			Debug.Log("Set Piece has become active in error!");
		}
		int num = mIndexList[mNextSectionForTheme];
		if (!mFinished && mPieceSet != null && mPieceSet.AuthoredSetPieces != null && mPieceSet.AuthoredSetPieces.SectionList[num] != null)
		{
			if (AdjustStraightLengthForPlayerSpeed)
			{
				int num2 = mLastElementForTheme + 1;
				PieceDescriptor piece = mPieceSet.AuthoredSetPieces.GetPiece(num, num2);
				if (mAddedLength <= 0f)
				{
					if (piece != null && (WorldConstructionHelper.IsHazard(piece.TypeId) || WorldConstructionHelper.IsMultiTilePieceExit(piece.TypeId)))
					{
						float num3 = 0f;
						PieceDescriptor pieceDescriptor2 = null;
						int numberOfElements = mPieceSet.AuthoredSetPieces.GetNumberOfElements(num);
						for (int i = num2 + 1; i < numberOfElements; i++)
						{
							PieceDescriptor piece2 = mPieceSet.AuthoredSetPieces.GetPiece(num, i);
							if (WorldConstructionHelper.IsHazard(piece2.TypeId) || WorldConstructionHelper.IsMultiTilePieceEntry(piece2.TypeId))
							{
								pieceDescriptor2 = piece2;
								break;
							}
							if (piece2.TypeId == WorldConstructionHelper.PieceType.Straight)
							{
								num3 += piece2.GetCachedLength();
							}
						}
						float num4 = 0f;
						float num5 = 0f;
						if (piece != null)
						{
							num4 = piece.GetCachedLength() - (float)piece.HazardRowEnd;
						}
						if (pieceDescriptor2 != null)
						{
							num5 = pieceDescriptor2.HazardRowStart;
						}
						int num6 = (int)(desiredLength - num4 - num5 - num3);
						if (num6 > 0)
						{
							mAddedLength = num6;
						}
					}
				}
				else if (mAddedLength > 0f)
				{
					WorldConstructionHelper.Group grp = ((piece.Group != WorldConstructionHelper.Group.Transition) ? piece.Group : piece.GroupTransitionEntry);
					pieceDescriptor = mPieceSet.AuthoredSetPieces.CreateExtraStraightPiece(pieces, Theme, grp, mAddedLength);
					mAddedLength -= pieceDescriptor.GetLength();
					if (mAddedLength < 0f)
					{
						mAddedLength = 0f;
					}
				}
			}
			if (pieceDescriptor == null)
			{
				pieceDescriptor = mPieceSet.AuthoredSetPieces.CreateNextPiece(num, mLastElementForTheme, 0);
				mLastElementForTheme++;
				if (mLastElementForTheme >= mPieceSet.AuthoredSetPieces.SectionList[num].ElementList.Count - 1)
				{
					mFinished = true;
					mLastElementForTheme = -1;
					if (mNextSectionForTheme + 1 >= mIndexList.Count)
					{
						Shuffle(num, mPieceSet.AuthoredSetPieces.SectionList.Count);
					}
					else
					{
						mNextSectionForTheme++;
					}
				}
			}
		}
		else
		{
			Debug.Log("WARNING, Not returning a piece from Authored Set Piece!");
		}
		return pieceDescriptor;
	}

	public void Reset()
	{
		mLastElementForTheme = -1;
		mFinished = false;
	}

	private void Shuffle(int lastIndex, int count)
	{
		mIndexList.Clear();
		mNextSectionForTheme = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		int num = lastIndex + 1;
		num = UnityEngine.Random.Range(num, num + count - 1);
		num %= count;
		do
		{
			mIndexList.Add(num);
			list.Remove(num);
			if (list.Count > 0)
			{
				num = UnityEngine.Random.Range(0, list.Count - 1);
				num = list[num];
			}
		}
		while (list.Count > 0);
	}
}
