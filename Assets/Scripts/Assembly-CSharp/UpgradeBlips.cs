using System.Collections.Generic;
using UnityEngine;

public class UpgradeBlips : MonoBehaviour
{
	public List<GameObject> m_Blips = new List<GameObject>();

	private int m_Level;

	public void SetLevel(int level)
	{
		m_Level = level;
	}

	private void Start()
	{
		m_Level = 0;
	}

	private void Update()
	{
		for (int i = 0; i < m_Blips.Count; i++)
		{
			m_Blips[i].active = i == m_Level;
		}
	}
}
