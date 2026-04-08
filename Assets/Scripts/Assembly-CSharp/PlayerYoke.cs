using UnityEngine;

public class PlayerYoke : MonoBehaviour
{
	public bool Jump { get; set; }

	public bool Slide { get; set; }

	public bool TurnLeft { get; set; }

	public bool TurnRight { get; set; }

	public bool AnyInput { get; set; }

	public bool TiltLeft { get; set; }

	public bool TiltRight { get; set; }

	public bool Tilt { get; set; }

	public float TiltAmount { get; set; }

	public float RawTiltAmount { get; set; }

	public bool Restart { get; set; }

	public bool Tap { get; set; }

	public bool DoubleTap { get; set; }

	public bool Debug { get; set; }

	private void Start()
	{
		Reset();
	}

	public void Reset()
	{
		Jump = false;
		Slide = false;
		Debug = false;
		TurnLeft = false;
		TurnRight = false;
		AnyInput = false;
		TiltLeft = false;
		TiltRight = false;
		Tilt = false;
		TiltAmount = 0f;
		Restart = false;
		Tap = false;
		DoubleTap = false;
	}

	private void Update()
	{
	}

	public static void SetJump(PlayerYoke instance)
	{
		instance.Jump = true;
	}

	public static void SetSlide(PlayerYoke instance)
	{
		instance.Slide = true;
	}

	public static void SetTurnLeft(PlayerYoke instance)
	{
		instance.TurnLeft = true;
	}

	public static void SetTurnRight(PlayerYoke instance)
	{
		instance.TurnRight = true;
	}

	public static void SetAnyInput(PlayerYoke instance)
	{
		instance.AnyInput = true;
	}

	public static void SetTiltLeft(PlayerYoke instance)
	{
		instance.TiltLeft = true;
	}

	public static void SetTiltRight(PlayerYoke instance)
	{
		instance.TiltRight = true;
	}

	public static void SetTap(PlayerYoke instance)
	{
		instance.Tap = true;
	}

	public static void SetDoubleTap(PlayerYoke instance)
	{
		instance.DoubleTap = true;
	}

	public static void SetDebug(PlayerYoke instance)
	{
		instance.Debug = true;
	}
}
