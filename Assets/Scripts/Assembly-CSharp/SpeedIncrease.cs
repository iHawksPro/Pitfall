using System.Collections.Generic;
using UnityEngine;

public class SpeedIncrease
{
	private static float PowerupSpeedBoostDelta = 0.5f;

	private static int SpeedSampleRange = 2000;

	private PlayerController mPlayer;

	private Material mLineMaterial;

	private List<float> mSpeedSample;

	private List<float> mSpeedBoostSample;

	private float mInitialSpeed;

	private float mPowerupSpeedBoostCurrent;

	private float mPowerupSpeedBoostTimeRemaining;

	private int mPowerupSpeedBoostStage;

	private bool mIsActive;

	public SpeedIncrease(PlayerController player)
	{
		mPlayer = player;
		mSpeedSample = new List<float>();
		mSpeedBoostSample = new List<float>();
		mInitialSpeed = 0f;
	}

	public void Reset()
	{
		mSpeedSample.Clear();
		mSpeedBoostSample.Clear();
		if (PlayerController.Instance() != null && PlayerController.Instance().GetPlayerAnimationController() != null)
		{
			PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.StopHasteEffect();
		}
		mIsActive = false;
	}

	public int NumHeld()
	{
		return SecureStorage.Instance.GetItemCount("consumable.speedincrease");
	}

	public bool Use()
	{
		if (NumHeld() > 0 && !mIsActive)
		{
			mIsActive = true;
			mInitialSpeed = mPlayer.GetMaxSpeed();
			mPowerupSpeedBoostCurrent = mInitialSpeed;
			mPowerupSpeedBoostStage = 0;
			mSpeedSample.Clear();
			mSpeedBoostSample.Clear();
			SecureStorage.Instance.ChangeItemCount("consumable.speedincrease", -1);
			PlayerController.Instance().PotionToUse = PlayerController.Potion_Type.SpeedBoost;
			PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.PlayHasteEffect();
			return true;
		}
		return false;
	}

	public void Update()
	{
		if (!mIsActive)
		{
			return;
		}
		float maxSpeed = mPlayer.GetMaxSpeed();
		float num = maxSpeed + maxSpeed * PowerupSpeedBoostDelta;
		switch (mPowerupSpeedBoostStage)
		{
		case 0:
			mPowerupSpeedBoostCurrent = Mathf.Min(num, mPowerupSpeedBoostCurrent + 3f * Time.deltaTime);
			if (mPowerupSpeedBoostCurrent == num)
			{
				mPowerupSpeedBoostStage = 1;
				mPowerupSpeedBoostTimeRemaining = UpgradeHelper.HasteTonicDuration();
			}
			break;
		case 1:
			mPowerupSpeedBoostCurrent = num;
			mPowerupSpeedBoostTimeRemaining -= Time.deltaTime;
			if (mPowerupSpeedBoostTimeRemaining <= 0f)
			{
				mPowerupSpeedBoostStage = 2;
			}
			break;
		case 2:
			mPowerupSpeedBoostCurrent = Mathf.Max(maxSpeed, mPowerupSpeedBoostCurrent - 3f * Time.deltaTime);
			if (mPowerupSpeedBoostCurrent == maxSpeed)
			{
				mIsActive = false;
				PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.StopHasteEffect();
				TBFUtils.DebugLog("Stop Haste Effect");
			}
			break;
		}
	}

	public void DrawSpeedGraph()
	{
		if (!mLineMaterial)
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			mLineMaterial = new Material(shader);
			mLineMaterial.hideFlags = HideFlags.HideAndDontSave;
			mLineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
		GL.PushMatrix();
		mLineMaterial.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Color(Color.white);
		GL.Begin(1);
		float num = (float)Screen.width * 0.2f;
		float num2 = (float)Screen.height * 0.2f;
		float num3 = num;
		float num4 = num2;
		float num5 = (float)Screen.width - num;
		float num6 = (float)Screen.height - num2;
		GL.Vertex3(num3, num4, 0f);
		GL.Vertex3(num5, num4, 0f);
		GL.Vertex3(num5, num4, 0f);
		GL.Vertex3(num5, num6, 0f);
		if (mSpeedSample.Count >= SpeedSampleRange)
		{
			mSpeedSample.Clear();
			mSpeedBoostSample.Clear();
		}
		mSpeedSample.Add(mPlayer.GetMaxSpeed());
		if (mIsActive)
		{
			mSpeedBoostSample.Add(mPowerupSpeedBoostCurrent);
		}
		float initialSpeed = mPlayer.Settings.InitialSpeed;
		float num7 = mPlayer.Settings.MaxSpeed - initialSpeed;
		for (int i = 0; i < mSpeedSample.Count; i++)
		{
			float num8 = num3 + (num5 - num3) / (float)SpeedSampleRange * (float)i;
			float num9 = num4 + (num6 - num4) / num7 * (mSpeedSample[i] - initialSpeed);
			GL.Color(Color.blue);
			GL.Vertex3(num8, num9, 0f);
			GL.Vertex3(num8 + 1f, num9 + 1f, 0f);
			if (mIsActive)
			{
				float num10 = mSpeedSample[i] + mSpeedSample[i] * PowerupSpeedBoostDelta;
				float num11 = num4 + (num6 - num4) / num7 * (num10 - initialSpeed);
				GL.Color(Color.red);
				GL.Vertex3(num8, num11, 0f);
				GL.Vertex3(num8 + 1f, num11 + 1f, 0f);
				float num12 = num4 + (num6 - num4) / num7 * (mSpeedBoostSample[i] - initialSpeed);
				GL.Color(Color.yellow);
				GL.Vertex3(num8, num12, 0f);
				GL.Vertex3(num8 + 1f, num12 + 1f, 0f);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	public float GetCurrentSpeed()
	{
		return mPowerupSpeedBoostCurrent;
	}

	public bool IsActive()
	{
		return mIsActive;
	}
}
