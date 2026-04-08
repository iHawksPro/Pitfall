using System.Collections.Generic;
using UnityEngine;

public class AuthoredLevelController : MonoBehaviour
{
	public static AuthoredLevelController instance;

	private AuthoredLevelLayoutController mGlobalWorld;

	private AuthoredLevelLayoutController mDebugMenuLevel;

	private List<AuthoredLevelLayoutController> mAuthoredLayouts;

	private AuthoredLevelLayoutController mActiveLayout;

	public static AuthoredLevelController Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mGlobalWorld = null;
		mAuthoredLayouts = new List<AuthoredLevelLayoutController>();
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
	}

	public void Reset()
	{
		mActiveLayout = null;
		if (mGlobalWorld != null)
		{
			mGlobalWorld.Reset();
		}
		foreach (AuthoredLevelLayoutController mAuthoredLayout in mAuthoredLayouts)
		{
			mAuthoredLayout.Reset();
		}
	}

	public PieceDescriptor CreateNextPiece(int parentSection, int parentElement, int parentBranch)
	{
		if (mGlobalWorld != null)
		{
			int testSection = mGlobalWorld.GetTestSection();
			int parentSection2 = ((testSection != -1) ? testSection : parentSection);
			return mGlobalWorld.CreateNextPiece(parentSection2, parentElement, parentBranch);
		}
		return null;
	}

	public PieceDescriptor CreateNextSuitablePiece(PieceDescriptor parentPiece, List<PieceDescriptor> modifiedPiecesList)
	{
		if (mActiveLayout == null && !SearchForActiveLayout(parentPiece, modifiedPiecesList))
		{
			return null;
		}
		int num = parentPiece.AuthoredSection;
		if (num == -1)
		{
			num = 0;
		}
		int authoredElement = parentPiece.AuthoredElement;
		if (mActiveLayout.IsNextPieceSelfLoop(num, authoredElement))
		{
			mActiveLayout.ResetAvailabilityForSpawn();
			mActiveLayout = null;
			return null;
		}
		return mActiveLayout.CreateNextPiece(num, authoredElement, 0);
	}

	public void RegisterLayoutController(AuthoredLevelLayoutController layoutController)
	{
		mAuthoredLayouts.Add(layoutController);
	}

	public void SetGlobalWorld(AuthoredLevelLayoutController layoutController)
	{
		if (mGlobalWorld != null && layoutController != null)
		{
			MonoBehaviour.print("WARNING - Multiple Test Worlds initialised. Only one will get used. Who knows which? Not me. *Shrugs*");
		}
		mGlobalWorld = layoutController;
	}

	public AuthoredLevelLayoutController GetGlobalWorld()
	{
		return mGlobalWorld;
	}

	public bool IsGlobalWorldActive()
	{
		return mGlobalWorld != null;
	}

	public void SetDebugMenuLevel(AuthoredLevelLayoutController layoutController)
	{
		mDebugMenuLevel = layoutController;
	}

	public AuthoredLevelLayoutController GetDebugMenuLevel()
	{
		return mDebugMenuLevel;
	}

	private bool SearchForActiveLayout(PieceDescriptor parentPiece, List<PieceDescriptor> modifiedPiecesList)
	{
		if (parentPiece == null)
		{
			return false;
		}
		foreach (AuthoredLevelLayoutController mAuthoredLayout in mAuthoredLayouts)
		{
			if (!mAuthoredLayout.AreAuthoredSectionsActive() || !mAuthoredLayout.IsAvailableForSpawn())
			{
				continue;
			}
			PlayerController playerController = PlayerController.Instance();
			float currentSpeed = playerController.GetCurrentSpeed();
			if (!(currentSpeed >= (float)mAuthoredLayout.SpeedMinimum) || !(currentSpeed <= (float)mAuthoredLayout.SpeedMaximum))
			{
				continue;
			}
			PieceDescriptor elementCached = mAuthoredLayout.SectionList[0].ElementList[0].GetElementCached();
			bool flag = false;
			foreach (PieceDescriptor modifiedPieces in modifiedPiecesList)
			{
				if (elementCached == modifiedPieces)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			mActiveLayout = mAuthoredLayout;
			return true;
		}
		return false;
	}
}
