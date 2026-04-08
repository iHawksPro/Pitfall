using System;
using System.Collections.Generic;
using Swrve;
using UnityEngine;

public class SwrveMessageAnimator
{
	private static float tstart;

	private static float t;

	private static float dismissDelay;

	private static Dictionary<SwrveButton, float> buttonsWooble;

	public void InitMessage(SwrveMessageFormat format)
	{
		tstart = Time.time;
		t = 0f;
		buttonsWooble = new Dictionary<SwrveButton, float>();
		format.AnimationScale = 0f;
		format.BackgroundAlpha = 0f;
		format.Init(new Point(0, 0), null);
		dismissDelay = 0f;
	}

	public void AnimateMessage(SwrveMessageFormat format)
	{
		t = (Time.time - tstart) * 20f;
		if (format.Message.Animated)
		{
			format.Buttons.ForEach(AnimateButton);
		}
		if (format.Closing)
		{
			if (dismissDelay == 0f)
			{
				dismissDelay = 1f;
				return;
			}
			dismissDelay += 0.1f;
			if (dismissDelay > 4f)
			{
				format.AnimationScale -= format.AnimationScale / 10f;
				format.BackgroundAlpha -= format.BackgroundAlpha / 10f;
			}
		}
		else
		{
			format.AnimationScale -= (format.AnimationScale - 1f) / 10f;
			format.BackgroundAlpha -= (format.BackgroundAlpha - 0.5f) / 10f;
		}
	}

	public void AnimateButton(SwrveButton button)
	{
		button.Scale = 1f;
		if (button.ActionType != SwrveActionType.Dismiss)
		{
			button.Scale = 0.9f + (float)Math.Abs(Math.Sin(t / 5f)) * 0.1f;
		}
		if (button.Pressed && Input.GetMouseButtonUp(0))
		{
			buttonsWooble[button] = 0.2f;
		}
		if (buttonsWooble.ContainsKey(button))
		{
			float num = buttonsWooble[button];
			if (num > 0f)
			{
				num -= 0.005f;
			}
			buttonsWooble[button] = num;
			if (num < 0f)
			{
				num = 0f;
				buttonsWooble.Remove(button);
			}
			button.Scale += (float)Math.Abs(Math.Sin(t)) * num;
		}
	}

	public bool IsMessageDismissed(SwrveMessageFormat format)
	{
		return format.Closing && format.AnimationScale < 0.1f;
	}
}
