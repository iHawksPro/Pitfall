using System.Collections.Generic;
using UnityEngine;

public class PieceDescriptorBuffer : MonoBehaviour
{
	public static PieceDescriptorBuffer instance;

	private bool mIsGenerating;

	private List<BranchBuffer> mBranchBuffer;

	public static PieceDescriptorBuffer Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mIsGenerating = false;
		mBranchBuffer = new List<BranchBuffer>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (mBranchBuffer.Count == 0)
		{
			mIsGenerating = false;
			return;
		}
		LevelGenerator levelGenerator = LevelGenerator.Instance();
		BranchBuffer branchBuffer = mBranchBuffer[0];
		bool flag = true;
		PieceDescriptor next = branchBuffer.GetNext();
		if (next == branchBuffer.GetStart())
		{
			WorldConstructionController.Instance().StartBranchIndexing();
			switch (branchBuffer.GetBranch())
			{
			case WorldConstructionController.Branch.StraightOn:
			{
				PieceDescriptor pieceDescriptor3 = levelGenerator.GeneratePiece(next.GetNextTheme(false, false), next.GetNextGroup(), next, 0);
				next.SetStraightAheadPiece(pieceDescriptor3);
				pieceDescriptor3.transform.parent = next.transform.parent;
				next.AttachPiece(next.StraightAheadAnchor, pieceDescriptor3.EntryAnchor, pieceDescriptor3, 0);
				branchBuffer.SetNext(pieceDescriptor3);
				break;
			}
			case WorldConstructionController.Branch.Left:
			{
				PieceDescriptor pieceDescriptor2 = levelGenerator.GeneratePiece(next.GetNextTheme(true, false), next.GetNextGroup(), next, -1);
				next.SetBranchLeftPiece(pieceDescriptor2);
				pieceDescriptor2.transform.parent = next.transform.parent;
				next.AttachPiece(next.BranchLeftAnchor, pieceDescriptor2.EntryAnchor, pieceDescriptor2, -1);
				branchBuffer.SetNext(pieceDescriptor2);
				break;
			}
			case WorldConstructionController.Branch.Right:
			{
				PieceDescriptor pieceDescriptor = levelGenerator.GeneratePiece(next.GetNextTheme(false, true), next.GetNextGroup(), next, 1);
				next.SetBranchRightPiece(pieceDescriptor);
				pieceDescriptor.transform.parent = next.transform.parent;
				next.AttachPiece(next.BranchRightAnchor, pieceDescriptor.EntryAnchor, pieceDescriptor, 1);
				branchBuffer.SetNext(pieceDescriptor);
				break;
			}
			default:
				MonoBehaviour.print("ERROR! - PieceDescriptorBuffer branch initialisation failure. This could lead to a level generation catastrophe!");
				branchBuffer.SetNext(null);
				break;
			}
			return;
		}
		if (next != null && !WorldConstructionHelper.IsJunction(next.TypeId) && branchBuffer.GetGenerationDepthRemaining() > 0f)
		{
			if (next.StraightAheadPath != null)
			{
				PieceDescriptor straightAheadPiece = next.GetStraightAheadPiece();
				if (straightAheadPiece == null)
				{
					straightAheadPiece = levelGenerator.GeneratePiece(next.GetNextTheme(false, false), next.GetNextGroup(), next, 0);
					next.SetStraightAheadPiece(straightAheadPiece);
					straightAheadPiece.transform.parent = next.transform.parent;
					next.AttachPiece(next.StraightAheadAnchor, straightAheadPiece.EntryAnchor, straightAheadPiece, 0);
				}
			}
			next = next.GetStraightAheadPiece();
			branchBuffer.SetNext(next);
			if (next != null)
			{
				flag = false;
				if (!WorldConstructionHelper.IsMultiTilePiecePartial(next.TypeId))
				{
					branchBuffer.ReduceGenerationDepth(next.GetLength());
				}
			}
		}
		if (flag)
		{
			WorldConstructionController.Instance().StopBranchIndexing(branchBuffer.GetBranch());
			mBranchBuffer.RemoveAt(0);
		}
	}

	public void GenerateDiscardableBranchPath(PieceDescriptor piece, WorldConstructionController.Branch branchOrigin)
	{
		mIsGenerating = true;
		BranchBuffer item = new BranchBuffer(piece, branchOrigin);
		mBranchBuffer.Add(item);
	}

	public bool IsGenerating()
	{
		return mIsGenerating;
	}
}
