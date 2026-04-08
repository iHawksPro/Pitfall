using System;
using UnityEngine;

public class GameObjectUtils : MonoBehaviour
{
	public static void HideObject(GameObject obj)
	{
		if (obj != null)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}

	public static void ShowObject(GameObject obj)
	{
		if (obj != null)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = true;
			}
		}
	}

	public static GameObject FindChildWithTag(GameObject obj, string tagToFind)
	{
		GameObject gameObject = null;
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject2 = obj.transform.GetChild(i).gameObject;
			gameObject = ((!gameObject2.CompareTag(tagToFind)) ? FindChildWithTag(gameObject2, tagToFind) : gameObject2);
			if (gameObject != null)
			{
				break;
			}
		}
		return gameObject;
	}

	public static GameObject FindChildWithName(GameObject obj, string nameToFind)
	{
		GameObject gameObject = null;
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject2 = obj.transform.GetChild(i).gameObject;
			gameObject = ((!gameObject2.name.Equals(nameToFind, StringComparison.CurrentCultureIgnoreCase)) ? FindChildWithName(gameObject2, nameToFind) : gameObject2);
			if (gameObject != null)
			{
				break;
			}
		}
		return gameObject;
	}
}
