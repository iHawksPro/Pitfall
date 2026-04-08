using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThemeSetPieceController
{
	public List<ThemeSetPieceEntry> SceneFilenames;

	public float InitialDistanceBeforeSetPiecesAppear = 1500f;

	public bool DisableThemeSetPieces;

	private ThemeSetPieceEntry mCurrentEntry;

	private float mDistanceMarkerForNextSetPiece;

	private float mAddedLength;

	private bool mIsActive;

	public bool IsActive
	{
		get
		{
			return !DisableThemeSetPieces && mCurrentEntry != null && !mCurrentEntry.IsFinished && mIsActive;
		}
	}

	public void Update()
	{
		if (mIsActive && (mCurrentEntry == null || mCurrentEntry.IsFinished || DisableThemeSetPieces))
		{
			DoReset();
		}
	}

	public bool IsTimeForSetPiece(WorldConstructionHelper.Theme parentTheme, float distance)
	{
		bool result = false;
		if (mCurrentEntry != null && mCurrentEntry.Theme == parentTheme && !mIsActive && !DisableThemeSetPieces)
		{
			result = distance >= mDistanceMarkerForNextSetPiece;
		}
		return result;
	}

	public ThemeSetPieceEntry FindSetPieceForTheme(WorldConstructionHelper.Theme theme)
	{
		ThemeSetPieceEntry result = null;
		foreach (ThemeSetPieceEntry sceneFilename in SceneFilenames)
		{
			if (sceneFilename.Theme == theme)
			{
				result = sceneFilename;
				break;
			}
		}
		return result;
	}

	public void Activate()
	{
		mIsActive = !DisableThemeSetPieces;
	}

	public PieceDescriptor CreateNextPiece(PieceSet pieces, List<PieceDescriptor> validPieces, float desiredLength)
	{
		return mCurrentEntry.CreateNextPiece(pieces, validPieces, desiredLength);
	}

	public WorldConstructionHelper.Group GetFirstGroup()
	{
		return mCurrentEntry.FirstGroup();
	}

	public void Reset(float currentDistance)
	{
		if (currentDistance > InitialDistanceBeforeSetPiecesAppear)
		{
			mDistanceMarkerForNextSetPiece = currentDistance;
		}
		else
		{
			mDistanceMarkerForNextSetPiece = InitialDistanceBeforeSetPiecesAppear;
		}
		ThemeChange(LevelGenerator.instance.StartTheme);
		mIsActive = false;
	}

	public void ThemeChange(WorldConstructionHelper.Theme theme)
	{
		mCurrentEntry = null;
		ThemeSetPieceEntry themeSetPieceEntry = FindSetPieceForTheme(theme);
		if (themeSetPieceEntry != null)
		{
			SetNext(themeSetPieceEntry);
		}
	}

	private void SetNext(ThemeSetPieceEntry next)
	{
		mCurrentEntry = next;
		DoReset();
	}

	private void DoReset()
	{
		if (mCurrentEntry != null)
		{
			mDistanceMarkerForNextSetPiece += UnityEngine.Random.Range(mCurrentEntry.MinimumDistanceBetweenSetPieces, mCurrentEntry.MaximumDistanceBetweenSetPieces);
			mCurrentEntry.Reset();
		}
		mIsActive = false;
	}
}
