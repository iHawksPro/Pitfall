using System.Collections.Generic;
using UnityEngine;

public class Distinctionbar : MonoBehaviour
{
	public List<GameObject> Distinctions = new List<GameObject>();

	public List<GameObject> DistinctionBlanks = new List<GameObject>();

	private void Start()
	{
	}

	private void LateUpdate()
	{
		for (int i = 0; i < 4; i++)
		{
			if (SecureStorage.Instance.GetXPDistinction() > i)
			{
				Distinctions[i].SetActiveRecursively(true);
				DistinctionBlanks[i].SetActiveRecursively(false);
			}
			else
			{
				Distinctions[i].SetActiveRecursively(false);
				DistinctionBlanks[i].SetActiveRecursively(true);
			}
		}
	}

	private void Update()
	{
	}
}
