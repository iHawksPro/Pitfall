using UnityEngine;

public class TiltHandler
{
	private const int MaxSampleSize = 20;

	private float[] mTiltArray;

	public TiltHandler()
	{
		mTiltArray = new float[20];
		Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < 20; i++)
		{
			mTiltArray[i] = 0f;
		}
	}

	protected float GetHorizontalTilt()
	{
		return Input.acceleration.x;
	}

	public void Update(PlayerYoke Yoke)
	{
		PlayerThemeSettings playerThemeSettings = PlayerController.Instance().CurrentTheme();
		int sampleSize = playerThemeSettings.SampleSize;
		float centreTolerance = playerThemeSettings.CentreTolerance;
		float tiltScale = playerThemeSettings.TiltScale;
		float num = (Yoke.RawTiltAmount = GetHorizontalTilt());
		num = ((num < 0f - centreTolerance) ? (num + centreTolerance) : ((!(num > centreTolerance)) ? 0f : (num - centreTolerance)));
		num = Mathf.Clamp(num * tiltScale, -1f, 1f);
		for (int i = 0; i < sampleSize - 1; i++)
		{
			mTiltArray[i] = mTiltArray[i + 1];
		}
		mTiltArray[sampleSize - 1] = num;
		float num2 = 0f;
		for (int j = 0; j < sampleSize; j++)
		{
			num2 += mTiltArray[j];
		}
		Yoke.Tilt = true;
		Yoke.TiltAmount = num2 / (float)sampleSize;
	}
}
