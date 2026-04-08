using System;

[Serializable]
public class PlayerThemeSettings
{
	public bool m_ReturnToCentre;

	public bool m_AllowPitJump;

	public bool m_AllowNormalJump;

	public bool m_CanSideStep;

	public bool m_HasTurnAnimations;

	public bool m_AllowExtraJump;

	public float m_SpeedMultiplier;

	public float m_MinimumDodgeTime;

	public float m_TurnRate;

	public bool m_HasTransition;

	public float m_TiltSpeed;

	public float m_TiltSmoothing;

	public float m_DodgeTiltSpeed;

	public float m_DodgeTiltTime;

	public float m_TiltSideStepTolerance;

	public float m_CentreTolerance;

	public float m_TiltScale;

	public int m_SampleSize;

	public float m_slideTiltSpeedModifier;

	public bool ReturnToCentre
	{
		get
		{
			return m_ReturnToCentre;
		}
		set
		{
			m_ReturnToCentre = value;
		}
	}

	public bool AllowPitJump
	{
		get
		{
			return m_AllowPitJump;
		}
	}

	public bool AllowNormalJump
	{
		get
		{
			return m_AllowNormalJump;
		}
	}

	public bool CanSideStep
	{
		get
		{
			return m_CanSideStep;
		}
	}

	public bool HasTurnAnimations
	{
		get
		{
			return m_HasTurnAnimations;
		}
	}

	public bool AllowExtraJump
	{
		get
		{
			return m_AllowExtraJump;
		}
	}

	public float SpeedMultiplier
	{
		get
		{
			return m_SpeedMultiplier;
		}
		set
		{
			m_SpeedMultiplier = value;
		}
	}

	public float MinimumDodgeTime
	{
		get
		{
			return m_MinimumDodgeTime;
		}
	}

	public float TurnRate
	{
		get
		{
			return m_TurnRate;
		}
		set
		{
			m_TurnRate = value;
		}
	}

	public bool HasTransition
	{
		get
		{
			return m_HasTransition;
		}
	}

	public float TiltSpeed
	{
		get
		{
			return m_TiltSpeed;
		}
		set
		{
			m_TiltSpeed = value;
		}
	}

	public float TiltSmoothing
	{
		get
		{
			return m_TiltSmoothing;
		}
		set
		{
			m_TiltSmoothing = value;
		}
	}

	public float DodgeTiltSpeed
	{
		get
		{
			return m_DodgeTiltSpeed;
		}
		set
		{
			m_DodgeTiltSpeed = value;
		}
	}

	public float DodgeTiltTime
	{
		get
		{
			return m_DodgeTiltTime;
		}
		set
		{
			m_DodgeTiltTime = value;
		}
	}

	public float TiltSideStepTolerance
	{
		get
		{
			return m_TiltSideStepTolerance;
		}
		set
		{
			m_TiltSideStepTolerance = value;
		}
	}

	public float CentreTolerance
	{
		get
		{
			return m_CentreTolerance;
		}
		set
		{
			m_CentreTolerance = value;
		}
	}

	public float TiltScale
	{
		get
		{
			return m_TiltScale;
		}
		set
		{
			m_TiltScale = value;
		}
	}

	public int SampleSize
	{
		get
		{
			return m_SampleSize;
		}
		set
		{
			m_SampleSize = value;
		}
	}

	public float SlideTiltSpeedModifier
	{
		get
		{
			return m_slideTiltSpeedModifier;
		}
		set
		{
			m_slideTiltSpeedModifier = value;
		}
	}
}
