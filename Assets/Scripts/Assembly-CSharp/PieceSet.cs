using System.Collections.Generic;
using UnityEngine;

public class PieceSet : MonoBehaviour
{
	private class Bucket
	{
		public List<PieceDescriptor> Pieces;

		public Bucket()
		{
			Pieces = new List<PieceDescriptor>();
		}
	}

	public string SetName = "invalid";

	public List<PieceDescriptor> Pieces;

	public AuthoredLevelLayoutController AuthoredSetPieces;

	private Bucket[,] mTwosByThemeAndGroup;

	private Bucket[,] mSixesByThemeAndGroup;

	private Bucket[,] mTwelvesByThemeAndGroup;

	private Bucket[,] mOtherStraightsByThemeAndGroup;

	private Bucket[,] mLeftBendsByThemeAndGroup;

	private Bucket[,] mRightBendsByThemeAndGroup;

	private Bucket[,] mPiecesByThemeAndGroup;

	private Bucket[,] mHazardsByThemeAndGroup;

	private Bucket[,,] mPiecesByThemeGroupAndType;

	private PieceDescriptor[,] mPossibleThemeTransitions;

	private PieceDescriptor[] mCheckpoints;

	private PieceDescriptor[] mPickups;

	private void Awake()
	{
		base.gameObject.name = "PieceSet-" + SetName;
	}

	private void Update()
	{
		foreach (PieceDescriptor piece in Pieces)
		{
			piece.OnLoad();
		}
		base.gameObject.SetActiveRecursively(false);
	}

	public List<PieceDescriptor> GetPiecesFor(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp)
	{
		List<PieceDescriptor> result = null;
		if (mPiecesByThemeAndGroup != null && mPiecesByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mPiecesByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		return result;
	}

	public List<PieceDescriptor> GetHazardsFor(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp)
	{
		List<PieceDescriptor> result = null;
		if (mHazardsByThemeAndGroup != null && mHazardsByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mHazardsByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		return result;
	}

	public List<PieceDescriptor> GetPiecesFor(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp, WorldConstructionHelper.PieceType type)
	{
		List<PieceDescriptor> result = null;
		if (mPiecesByThemeGroupAndType != null && mPiecesByThemeGroupAndType[(int)theme, (int)grp, (int)type] != null)
		{
			result = mPiecesByThemeGroupAndType[(int)theme, (int)grp, (int)type].Pieces;
		}
		return result;
	}

	public List<PieceDescriptor> GetStraightsAt(float length, WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp)
	{
		List<PieceDescriptor> result = null;
		if (length >= 24f && mOtherStraightsByThemeAndGroup != null && mOtherStraightsByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mOtherStraightsByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		else if (length >= 11f && mTwelvesByThemeAndGroup != null && mTwelvesByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mTwelvesByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		else if (length >= 5f && mSixesByThemeAndGroup != null && mSixesByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mSixesByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		else if (mTwosByThemeAndGroup != null && mTwosByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mTwosByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		return result;
	}

	public List<PieceDescriptor> GetLeftBendsAt(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp)
	{
		List<PieceDescriptor> result = null;
		if (mLeftBendsByThemeAndGroup != null && mLeftBendsByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mLeftBendsByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		return result;
	}

	public List<PieceDescriptor> GetRightBendsAt(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp)
	{
		List<PieceDescriptor> result = null;
		if (mRightBendsByThemeAndGroup != null && mRightBendsByThemeAndGroup[(int)theme, (int)grp] != null)
		{
			result = mRightBendsByThemeAndGroup[(int)theme, (int)grp].Pieces;
		}
		return result;
	}

	public PieceDescriptor GetThemeTransition(WorldConstructionHelper.Theme themeFrom, WorldConstructionHelper.Theme themeTo)
	{
		PieceDescriptor result = null;
		if (mPossibleThemeTransitions != null)
		{
			result = mPossibleThemeTransitions[(int)themeFrom, (int)themeTo];
		}
		return result;
	}

	public PieceDescriptor GetCheckpointFor(WorldConstructionHelper.Theme theme)
	{
		PieceDescriptor result = null;
		if (mCheckpoints != null)
		{
			result = mCheckpoints[(int)theme];
		}
		return result;
	}

	public PieceDescriptor GetPickupFor(WorldConstructionHelper.Theme theme)
	{
		PieceDescriptor result = null;
		if (mPickups != null)
		{
			result = mPickups[(int)theme];
		}
		return result;
	}

	public bool IsThemeTransitionPossible(WorldConstructionHelper.Theme themeFrom, WorldConstructionHelper.Theme themeTo)
	{
		bool result = false;
		if (mPossibleThemeTransitions != null)
		{
			result = mPossibleThemeTransitions[(int)themeFrom, (int)themeTo] != null;
		}
		return result;
	}

	private static int ComparePieceByNameHash(PieceDescriptor a, PieceDescriptor b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return 1;
		}
		if (b == null)
		{
			return -1;
		}
		if (a.m_iBaseNameHash > b.m_iBaseNameHash)
		{
			return -1;
		}
		return 1;
	}

	public void Refresh()
	{
		mTwosByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mSixesByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mTwelvesByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mOtherStraightsByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mLeftBendsByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mRightBendsByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mPiecesByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mHazardsByThemeAndGroup = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize];
		mPiecesByThemeGroupAndType = new Bucket[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Group_ListSize, WorldConstructionHelper.Type_ListSize];
		mPossibleThemeTransitions = new PieceDescriptor[WorldConstructionHelper.Theme_ListSize, WorldConstructionHelper.Theme_ListSize];
		mCheckpoints = new PieceDescriptor[WorldConstructionHelper.Theme_ListSize];
		mPickups = new PieceDescriptor[WorldConstructionHelper.Theme_ListSize];
		Pieces.Sort(ComparePieceByNameHash);
		foreach (PieceDescriptor piece in Pieces)
		{
			WorldConstructionHelper.Group obj = piece.Group;
			if (obj == WorldConstructionHelper.Group.Transition)
			{
				obj = piece.GroupTransitionEntry;
			}
			if (piece.TypeId == WorldConstructionHelper.PieceType.Straight && piece.Group != WorldConstructionHelper.Group.Transition)
			{
				float cachedLength = piece.GetCachedLength();
				if (cachedLength == 2f)
				{
					if (mTwosByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mTwosByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mTwosByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
				else if (cachedLength == 6f)
				{
					if (mSixesByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mSixesByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mSixesByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
				else if (cachedLength == 12f && piece.Bend == 0)
				{
					if (mTwelvesByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mTwelvesByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mTwelvesByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
				else if (piece.Bend == 0)
				{
					if (mOtherStraightsByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mOtherStraightsByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mOtherStraightsByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
				else if (piece.Bend < 0)
				{
					if (mLeftBendsByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mLeftBendsByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mLeftBendsByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
				else
				{
					if (mRightBendsByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
					{
						mRightBendsByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
					}
					mRightBendsByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
				}
			}
			if (mPiecesByThemeAndGroup[(int)piece.Theme, (int)obj] == null)
			{
				mPiecesByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
				mHazardsByThemeAndGroup[(int)piece.Theme, (int)obj] = new Bucket();
			}
			mPiecesByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
			if (WorldConstructionHelper.IsHazard(piece.TypeId))
			{
				mHazardsByThemeAndGroup[(int)piece.Theme, (int)obj].Pieces.Add(piece);
			}
			if (mPiecesByThemeGroupAndType[(int)piece.Theme, (int)obj, (int)piece.TypeId] == null)
			{
				mPiecesByThemeGroupAndType[(int)piece.Theme, (int)obj, (int)piece.TypeId] = new Bucket();
			}
			mPiecesByThemeGroupAndType[(int)piece.Theme, (int)obj, (int)piece.TypeId].Pieces.Add(piece);
			if (piece.TypeId == WorldConstructionHelper.PieceType.ThemeTransition)
			{
				mPossibleThemeTransitions[(int)piece.ThemeTransitionEntry, (int)piece.Theme] = piece;
			}
			if (piece.TypeId == WorldConstructionHelper.PieceType.Checkpoint)
			{
				mCheckpoints[(int)piece.Theme] = piece;
			}
			if (piece.TypeId == WorldConstructionHelper.PieceType.Pickup)
			{
				mPickups[(int)piece.Theme] = piece;
			}
		}
	}
}
