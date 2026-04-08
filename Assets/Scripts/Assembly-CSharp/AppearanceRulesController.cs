using System.Collections.Generic;
using UnityEngine;

public class AppearanceRulesController : MonoBehaviour
{
	public const int MaxRangeInMetres = 99999;

	public List<AppearanceRule> Rules;

	public int Crocodile;

	public int Scorpion;

	public int Snake;

	public int Meteor;

	public List<int> GetAvailableDifficultyIds(float forDistance)
	{
		List<int> list = new List<int>();
		AppearanceRule appearanceRule = null;
		foreach (AppearanceRule rule in Rules)
		{
			appearanceRule = rule;
			if (rule.StartDistance <= forDistance && forDistance <= rule.EndDistance)
			{
				break;
			}
		}
		foreach (AppearanceElement element in appearanceRule.Elements)
		{
			for (int i = 0; i < element.Chance; i++)
			{
				list.Add(element.DifficultyID);
			}
		}
		return list;
	}
}
