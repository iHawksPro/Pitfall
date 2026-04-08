using System;

[Serializable]
public class GroupTransitionTimeOverride
{
	public WorldConstructionHelper.Theme Theme;

	public WorldConstructionHelper.Group Group;

	public float MinimumTimeInGroup;

	public float MaximumTimeInGroup;
}
