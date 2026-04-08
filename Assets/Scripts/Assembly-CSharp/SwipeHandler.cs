using System.Collections.Generic;
using UnityEngine;

public class SwipeHandler
{
	private int mNumInputFrames;

	private Vector2 mStartInputPosition;

	private Vector2 mLastDelta;

	private bool mInputSent;

	private bool mDebounce;

	private int mNumTaps;

	private float mLastTapTime;

	private Vector2 mLastInputPos;

	private List<Vector2> mDebugInputHistory = new List<Vector2>();

	private float mDebugLastAngleForVerticalSwipe;

	private Vector2 mDebugDelta = new Vector2(0f, 0f);

	private int m_holdTouchId;

	public SwipeHandler()
	{
		Reset();
	}

	public Vector2 LastInputPosition()
	{
		return mLastInputPos;
	}

	public void SetHoldTouchId(int touchId)
	{
		m_holdTouchId = touchId;
	}

	private void ProcessInput(Vector2 inputPos, PlayerYoke Yoke, InputController.SwipeBias bias, float tapBias)
	{
		PlayerYoke.SetAnyInput(Yoke);
		if (inputPos.y > 0.9f * (float)Screen.height && inputPos.x > 0.25f * (float)Screen.width && inputPos.x < 0.75f * (float)Screen.width)
		{
			PlayerYoke.SetDebug(Yoke);
		}
		mLastInputPos = inputPos;
		float num = InputTweaks.AngleForVerticalSwipe;
		switch (bias)
		{
		case InputController.SwipeBias.Horizontal:
			num = 90f - num;
			break;
		case InputController.SwipeBias.None:
			num = 45f;
			break;
		}
		mDebugLastAngleForVerticalSwipe = num;
		mDebugInputHistory.Add(inputPos);
		if (++mNumInputFrames == 1)
		{
			mStartInputPosition = inputPos;
		}
		if (mNumInputFrames <= 1 || mInputSent)
		{
			return;
		}
		Vector2 vector = (mLastDelta = inputPos - mStartInputPosition);
		float num2 = InputTweaks.TouchSize * tapBias;
		if (vector.sqrMagnitude > num2 * num2)
		{
			mDebugDelta = vector;
			Vector2 lhs = vector;
			lhs.Normalize();
			float num3 = 57.29578f * Mathf.Acos(Vector2.Dot(lhs, Vector2.up));
			if (Vector2.Dot(lhs, Vector2.right) < 0f)
			{
				num3 = 360f - num3;
			}
			if (num3 < num || num3 > 360f - num)
			{
				mInputSent = true;
				PlayerYoke.SetJump(Yoke);
			}
			else if (num3 > num && num3 < 180f - num)
			{
				mInputSent = true;
				PlayerYoke.SetTurnRight(Yoke);
			}
			else if (num3 > 180f - num && num3 < 180f + num)
			{
				mInputSent = true;
				PlayerYoke.SetSlide(Yoke);
			}
			else if (num3 > 180f + num && num3 < 360f - num)
			{
				mInputSent = true;
				PlayerYoke.SetTurnLeft(Yoke);
			}
		}
	}

	public void Reset()
	{
		mNumInputFrames = 0;
		mInputSent = false;
		mDebugLastAngleForVerticalSwipe = 0f;
		mDebounce = true;
		mNumTaps = 0;
		mLastTapTime = 0f;
		mLastInputPos = new Vector2(0f, 0f);
		m_holdTouchId = -1;
	}

	public void Update(PlayerYoke Yoke, InputController.SwipeBias bias, float tapBias, bool enableMouseInput)
	{
		bool flag = false;
		if (mDebounce)
		{
			if (Input.touchCount == 0 && (!enableMouseInput || !Input.GetMouseButton(0)))
			{
				mDebounce = false;
			}
		}
		else if (Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.fingerId != m_holdTouchId)
				{
					flag = true;
					ProcessInput(new Vector2(touch.position.x, touch.position.y), Yoke, bias, tapBias);
				}
			}
		}
		else if (enableMouseInput && Input.GetMouseButton(0))
		{
			flag = true;
			ProcessInput(new Vector2(Input.mousePosition.x, Input.mousePosition.y), Yoke, bias, tapBias);
		}
		if (flag)
		{
			return;
		}
		if (mNumInputFrames > 1)
		{
			float num = InputTweaks.TouchSize * tapBias;
			float num2 = Screen.height;
			float num3 = (float)Screen.height * 0.14f;
			if (mLastDelta.sqrMagnitude < num * num && mLastInputPos.y < num2 && mLastInputPos.y > num3)
			{
				PlayerYoke.SetTap(Yoke);
				if (mNumTaps == 1)
				{
					float num4 = Time.time - mLastTapTime;
					if (num4 < 0.3f)
					{
						PlayerYoke.SetDoubleTap(Yoke);
					}
					mNumTaps = 0;
				}
				else
				{
					mNumTaps++;
					mLastTapTime = Time.time;
				}
			}
		}
		mInputSent = false;
		mNumInputFrames = 0;
		mDebugInputHistory.Clear();
	}

	public void DebugRender()
	{
	}
}
