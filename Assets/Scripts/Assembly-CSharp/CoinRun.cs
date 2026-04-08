public class CoinRun
{
	public int CoinRunIndex;

	public int CoinRunCount;

	public PieceDescriptor Piece;

	public int CurrentRunDepthRemaining;

	public float DistanceIntoPath;

	public int YCurve;

	public int YCurveStart;

	public int YCurveEnd;

	public float YCurveStartDistanceIntoPiece;

	public bool IsForcedLeft;

	public bool IsForcedCentre;

	public bool IsForcedRight;

	public PieceDescriptor LastStreamedPiece;

	public int ExitBranch;

	public int runBranchDirChoice;

	public int jumpDuckDirChoice;
}
