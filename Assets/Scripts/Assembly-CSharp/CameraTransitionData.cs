using System;

[Serializable]
public class CameraTransitionData
{
	public CameraBase CameraFrom;

	public TweenFunctions.TweenType TweenType;

	public float Duration;

	public static CameraTransitionData JumpCut
	{
		get
		{
			CameraTransitionData cameraTransitionData = new CameraTransitionData();
			cameraTransitionData.CameraFrom = null;
			cameraTransitionData.TweenType = TweenFunctions.TweenType.linear;
			cameraTransitionData.Duration = 0f;
			return cameraTransitionData;
		}
	}

	public CameraTransitionData()
	{
	}

	public CameraTransitionData(CameraBase from, TweenFunctions.TweenType type, float duration)
	{
		CameraFrom = from;
		TweenType = type;
		Duration = duration;
	}
}
