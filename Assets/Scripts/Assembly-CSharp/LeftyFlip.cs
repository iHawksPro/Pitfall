using UnityEngine;

public class LeftyFlip : MonoBehaviour
{
	private EZScreenPlacement m_Placement;

	public HudMovement m_MovementScript;

	private float m_fOriginalH;

	private HudMovement.Direction m_OriginalDirection;

	private float m_OriginalTransitionPosX;

	private void Awake()
	{
		m_Placement = base.gameObject.GetComponent<EZScreenPlacement>();
		m_fOriginalH = m_Placement.screenPos.x;
		if ((bool)m_MovementScript)
		{
			m_OriginalDirection = m_MovementScript.m_TransitionDirection;
			m_OriginalTransitionPosX = m_MovementScript.transform.position.x;
		}
	}

	private void OnEnable()
	{
		if (SecureStorage.Instance.LeftyControls)
		{
			m_Placement.screenPos.x = 100f - m_fOriginalH;
			if ((bool)m_MovementScript)
			{
				if (m_OriginalDirection == HudMovement.Direction.FromLeft)
				{
					m_MovementScript.SetDirection(HudMovement.Direction.FromRight);
				}
				else if (m_OriginalDirection == HudMovement.Direction.FromRight)
				{
					m_MovementScript.SetDirection(HudMovement.Direction.FromLeft);
				}
			}
		}
		else
		{
			m_Placement.screenPos.x = m_fOriginalH;
		}
		m_Placement.PositionOnScreen();
	}
}
