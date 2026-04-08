using System.Collections.Generic;
using UnityEngine;

public class PositionAdjuster : MonoBehaviour
{
	public List<PosAdjust> m_Positions;

	public bool m_DoNotAspectCorrect;

	private Vector3 m_InitialPos;

	private void Awake()
	{
		m_InitialPos = base.transform.localPosition;
	}
}
