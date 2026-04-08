using System;

[Serializable]
public class MultipieceTypeRule
{
	public WorldConstructionHelper.Theme Theme;

	public WorldConstructionHelper.Group Group;

	public WorldConstructionHelper.PieceType Type;

	public int MinimumLoopSections;

	public int MaximumLoopSections;
}
