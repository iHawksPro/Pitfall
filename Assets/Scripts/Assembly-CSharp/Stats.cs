using System;
using UnityEngine;

public class Stats : ScriptableObject
{
	[Serializable]
	public class StatData
	{
		public string Identifier = string.Empty;

		public string Name = string.Empty;

		public string Suffix = string.Empty;
	}

	public StatData[] stats;
}
