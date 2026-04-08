public class BranchBuffer
{
	private PieceDescriptor mStart;

	private PieceDescriptor mNext;

	private WorldConstructionController.Branch mBranch;

	private float mGenerationDepthRemaining;

	public BranchBuffer(PieceDescriptor start, WorldConstructionController.Branch branch)
	{
		mStart = (mNext = start);
		mBranch = branch;
		if (mBranch == WorldConstructionController.Branch.StraightOn)
		{
			mGenerationDepthRemaining = 40f;
		}
		else
		{
			mGenerationDepthRemaining = 10f;
		}
	}

	public PieceDescriptor GetStart()
	{
		return mStart;
	}

	public PieceDescriptor GetNext()
	{
		return mNext;
	}

	public void SetNext(PieceDescriptor piece)
	{
		mNext = piece;
	}

	public WorldConstructionController.Branch GetBranch()
	{
		return mBranch;
	}

	public float GetGenerationDepthRemaining()
	{
		return mGenerationDepthRemaining;
	}

	public void ReduceGenerationDepth(float depth)
	{
		mGenerationDepthRemaining -= depth;
	}
}
