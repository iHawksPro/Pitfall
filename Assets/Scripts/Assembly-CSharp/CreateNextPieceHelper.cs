using System.Collections.Generic;
using UnityEngine;

public class CreateNextPieceHelper
{
	private static List<PieceDescriptor> mValidPiecesWithinLength = new List<PieceDescriptor>();

	public static WorldConstructionHelper.Group FindGroupForTransitionEntry(WorldConstructionHelper.Theme fromTheme, WorldConstructionHelper.Theme toTheme)
	{
		WorldConstructionHelper.Group result = WorldConstructionHelper.Group.A;
		if (fromTheme == WorldConstructionHelper.Theme.Minecart && toTheme == WorldConstructionHelper.Theme.Jungle)
		{
			result = WorldConstructionHelper.Group.B;
		}
		return result;
	}

	public static WorldConstructionHelper.Group FindGroupForTransitionExit(WorldConstructionHelper.Theme fromTheme, WorldConstructionHelper.Theme toTheme)
	{
		WorldConstructionHelper.Group result = WorldConstructionHelper.Group.A;
		if (fromTheme == WorldConstructionHelper.Theme.Jungle && toTheme == WorldConstructionHelper.Theme.Minecart)
		{
			result = WorldConstructionHelper.Group.B;
		}
		else if (fromTheme == WorldConstructionHelper.Theme.Jungle && toTheme == WorldConstructionHelper.Theme.Bike)
		{
			result = WorldConstructionHelper.Group.B;
		}
		return result;
	}

	public static bool CanTransitionToGroup(List<PieceDescriptor> validPieces, WorldConstructionHelper.Group grp)
	{
		foreach (PieceDescriptor validPiece in validPieces)
		{
			if (validPiece.Group == WorldConstructionHelper.Group.Transition && validPiece.GroupTransitionExit == grp)
			{
				return true;
			}
		}
		return false;
	}

	public static PieceDescriptor FindTransitionToGroup(List<PieceDescriptor> validPieces, WorldConstructionHelper.Group grp)
	{
		PieceDescriptor result = null;
		foreach (PieceDescriptor validPiece in validPieces)
		{
			if (validPiece.Group == WorldConstructionHelper.Group.Transition && validPiece.GroupTransitionExit == grp)
			{
				result = validPiece;
			}
		}
		return result;
	}

	public static PieceDescriptor FindPieceOfType(List<PieceDescriptor> validPieces, WorldConstructionHelper.PieceType type)
	{
		PieceDescriptor result = null;
		foreach (PieceDescriptor validPiece in validPieces)
		{
			if (validPiece.TypeId == type)
			{
				result = validPiece;
				break;
			}
		}
		return result;
	}

	public static List<PieceDescriptor> FindGroupTransitions(List<PieceDescriptor> validPieces)
	{
		List<PieceDescriptor> list = new List<PieceDescriptor>();
		foreach (PieceDescriptor validPiece in validPieces)
		{
			if (validPiece.Group == WorldConstructionHelper.Group.Transition)
			{
				list.Add(validPiece);
			}
		}
		return list;
	}

	public static List<WorldConstructionHelper.Group> FindGroupsThatCanTransitionToGroup(PieceSet allPieces, WorldConstructionHelper.Group grp)
	{
		List<WorldConstructionHelper.Group> list = new List<WorldConstructionHelper.Group>();
		foreach (PieceDescriptor piece in allPieces.Pieces)
		{
			if (piece.Group == WorldConstructionHelper.Group.Transition && piece.GroupTransitionExit == grp)
			{
				list.Add(piece.GroupTransitionEntry);
			}
		}
		return list;
	}

	public static PieceDescriptor GetStraightAtRequiredLength(PieceSet pieces, WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp, float requiredLength, bool leftAllowed, bool rightAllowed, GameType currentGameType)
	{
		PieceDescriptor pieceDescriptor = null;
		mValidPiecesWithinLength.Clear();
		List<PieceDescriptor> straightsAt = pieces.GetStraightsAt(requiredLength, theme, grp);
		mValidPiecesWithinLength.AddRange(straightsAt);
		if (requiredLength > 12f)
		{
			if (leftAllowed)
			{
				List<PieceDescriptor> leftBendsAt = pieces.GetLeftBendsAt(theme, grp);
				if (leftBendsAt != null)
				{
					mValidPiecesWithinLength.AddRange(leftBendsAt);
				}
			}
			if (rightAllowed)
			{
				List<PieceDescriptor> rightBendsAt = pieces.GetRightBendsAt(theme, grp);
				if (rightBendsAt != null)
				{
					mValidPiecesWithinLength.AddRange(rightBendsAt);
				}
			}
		}
		if (mValidPiecesWithinLength.Count > 0)
		{
			pieceDescriptor = null;
			while (pieceDescriptor == null)
			{
				pieceDescriptor = mValidPiecesWithinLength[Random.Range(0, mValidPiecesWithinLength.Count)];
				if (pieceDescriptor.IsExcludedForGameType(currentGameType))
				{
					Debug.Log("Discarding " + pieceDescriptor.name + " for game type");
					pieceDescriptor = null;
				}
			}
		}
		return pieceDescriptor;
	}
}
