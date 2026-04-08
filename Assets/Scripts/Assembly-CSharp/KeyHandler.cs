using System.Collections.Generic;
using UnityEngine;

public class KeyHandler
{
	public enum ButtonInputType
	{
		BUTTON_PRESS = 1,
		BUTTON_HOLD = 2
	}

	private class Button
	{
		private string mName;

		private InputController.ButtonPressFunction mFunc;

		private ButtonInputType mType;

		private bool mDebounce;

		public Button(string name, InputController.ButtonPressFunction buttonFunction, ButtonInputType type)
		{
			mName = name;
			mFunc = buttonFunction;
			mType = type;
			mDebounce = false;
		}

		public void Reset()
		{
			mDebounce = true;
		}

		public void Update(PlayerYoke Yoke)
		{
			if (Input.GetButton(mName))
			{
				if (mType == ButtonInputType.BUTTON_HOLD || (mType == ButtonInputType.BUTTON_PRESS && !mDebounce))
				{
					mFunc(Yoke);
					mDebounce = true;
				}
			}
			else
			{
				mDebounce = false;
			}
		}
	}

	private List<Button> mButtons;

	public KeyHandler()
	{
		mButtons = new List<Button>();
	}

	public void Add(string name, InputController.ButtonPressFunction buttonFunction, ButtonInputType type)
	{
		mButtons.Add(new Button(name, buttonFunction, type));
	}

	public void Reset()
	{
		foreach (Button mButton in mButtons)
		{
			mButton.Reset();
		}
	}

	public void Update(PlayerYoke Yoke)
	{
		foreach (Button mButton in mButtons)
		{
			mButton.Update(Yoke);
		}
	}
}
