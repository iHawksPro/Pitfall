using System;
using System.Collections.Generic;

[Serializable]
public class AuthoredLevelLayoutSection
{
	public string Name;

	public List<AuthoredLevelLayoutElement> ElementList = new List<AuthoredLevelLayoutElement>();

	public List<int> DifficultyGroups = new List<int>();

	public int StraightAheadSection = -1;

	public int BranchLeftSection = -1;

	public int BranchRightSection = -1;

	public bool TutorialSection;

	public bool OnLoad(PieceSet pieceSet)
	{
		bool flag = true;
		foreach (AuthoredLevelLayoutElement element in ElementList)
		{
			flag &= element.OnLoad(pieceSet);
		}
		return flag;
	}
}
