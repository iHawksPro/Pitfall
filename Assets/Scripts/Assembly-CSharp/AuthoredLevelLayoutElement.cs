using System;

[Serializable]
public class AuthoredLevelLayoutElement
{
	public enum Placement
	{
		None = 0,
		Crocodile = 1,
		Scorpion = 2,
		Snake = 3,
		Powerup = 4,
		Coin_L = 5,
		Coin_C = 6,
		Coin_R = 7
	}

	public string Element;

	public int ElementHash;

	public int ListIndex;

	public Placement Spawn;

	public WorldConstructionHelper.Theme Theme;

	public WorldConstructionHelper.Group Group;

	public int relicIndex = -1;

	private PieceDescriptor mElementCached;

	public bool OnLoad(PieceSet pieceSet)
	{
		int iElemHash = Element.GetHashCode();
		PieceDescriptor pieceDescriptor = pieceSet.Pieces.Find((PieceDescriptor i) => i.m_iBaseNameHash == iElemHash);
		if (pieceDescriptor != null && mElementCached == null)
		{
			mElementCached = pieceDescriptor;
		}
		return pieceDescriptor != null;
	}

	public PieceDescriptor GetElementCached()
	{
		return mElementCached;
	}
}
