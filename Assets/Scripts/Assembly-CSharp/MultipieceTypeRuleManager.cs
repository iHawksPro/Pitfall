using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultipieceTypeRuleManager
{
	public List<MultipieceTypeRule> Rules;

	public int DefaultMinimumLoopSections = 5;

	public int DefaultMaximumLoopSections = 10;

	public int FindLoopIterationsFor(WorldConstructionHelper.Theme theme, WorldConstructionHelper.Group grp, WorldConstructionHelper.PieceType type)
	{
		PlayerController playerController = PlayerController.Instance();
		float num = 6f;
		float num2 = num;
		if (WorldConstructionHelper.IsRopeSwing(type))
		{
			num2 = num * 1.5f;
		}
		int val = (int)(playerController.GetCurrentSpeed() / (num * 3f));
		int val2 = (int)(num2 / num);
		int value = Math.Max(val2, val);
		int min = DefaultMinimumLoopSections;
		int max = DefaultMaximumLoopSections;
		foreach (MultipieceTypeRule rule in Rules)
		{
			if (rule.Theme == theme && (rule.Group == WorldConstructionHelper.Group.Invalid || rule.Group == grp) && (rule.Type == WorldConstructionHelper.PieceType.Exclude || rule.Type == type))
			{
				min = rule.MinimumLoopSections;
				max = rule.MaximumLoopSections;
				break;
			}
		}
		return Mathf.Clamp(value, min, max);
	}
}
