using System;

[Serializable]
public class PieceTriggerPoint
{
	public enum BranchId
	{
		Left = -1,
		Ahead = 0,
		Right = 1
	}

	public enum LaneSelect
	{
		All = 0,
		Left = 1,
		Centre = 2,
		Right = 3,
		LeftAndCentre = 4,
		RightAndCentre = 5
	}

	public float Distance;

	public float Time;

	public LaneSelect Lane;

	public BranchId Branch;

	public bool IgnoreBranchSwitch;

	public string MethodToInvoke = string.Empty;

	public string Param1 = string.Empty;

	public string Param2 = string.Empty;

	public bool Fired { get; set; }

	public float GetLeftBound()
	{
		switch (Lane)
		{
		case LaneSelect.All:
		case LaneSelect.Left:
		case LaneSelect.LeftAndCentre:
			return -1.5f;
		case LaneSelect.Centre:
		case LaneSelect.RightAndCentre:
			return -0.5f;
		case LaneSelect.Right:
			return 0.5f;
		default:
			throw new Exception("Bad lane specifier on trigger");
		}
	}

	public float GetRightBound()
	{
		switch (Lane)
		{
		case LaneSelect.All:
		case LaneSelect.Right:
		case LaneSelect.RightAndCentre:
			return 1.5f;
		case LaneSelect.Centre:
		case LaneSelect.LeftAndCentre:
			return 0.5f;
		case LaneSelect.Left:
			return -0.5f;
		default:
			throw new Exception("Bad lane specifier on trigger");
		}
	}
}
