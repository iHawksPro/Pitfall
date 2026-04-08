using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupTransitionTimeManager
{
	public float DefaultMinimumTimeInGroup = 5f;

	public float DefaultMaximumTimeInGroup = 6f;

	public List<GroupTransitionTimeOverride> OverrideList = new List<GroupTransitionTimeOverride>();

	public float CurrentTimeInGroup;

	public void ResetTime(WorldConstructionHelper.Theme nextTheme, WorldConstructionHelper.Group nextGroup)
	{
		float min = DefaultMinimumTimeInGroup;
		float max = DefaultMaximumTimeInGroup;
		foreach (GroupTransitionTimeOverride @override in OverrideList)
		{
			if (@override.Theme == nextTheme && @override.Group == nextGroup)
			{
				min = @override.MinimumTimeInGroup;
				max = @override.MaximumTimeInGroup;
				break;
			}
		}
		CurrentTimeInGroup = UnityEngine.Random.Range(min, max);
	}

	public void ResetToDefaultTime()
	{
		CurrentTimeInGroup = UnityEngine.Random.Range(DefaultMinimumTimeInGroup, DefaultMaximumTimeInGroup);
	}
}
