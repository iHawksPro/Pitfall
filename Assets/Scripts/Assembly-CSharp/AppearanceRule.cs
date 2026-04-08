using System;
using System.Collections.Generic;

[Serializable]
public class AppearanceRule
{
	public float StartDistance;

	public float EndDistance;

	public List<AppearanceElement> Elements;

	public string DebugComment;
}
