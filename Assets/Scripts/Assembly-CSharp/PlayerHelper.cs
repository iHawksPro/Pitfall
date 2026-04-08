using UnityEngine;

public static class PlayerHelper
{
	private static Transform mHandBone;

	public static Transform SearchHierarchyForBone(Transform current, string name)
	{
		if (current.name == name)
		{
			return current;
		}
		for (int i = 0; i < current.childCount; i++)
		{
			Transform transform = SearchHierarchyForBone(current.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static Transform GetHandBone(GameObject model)
	{
		if (mHandBone == null)
		{
			mHandBone = SearchHierarchyForBone(model.transform, "Bip001 L Hand");
		}
		return mHandBone;
	}
}
