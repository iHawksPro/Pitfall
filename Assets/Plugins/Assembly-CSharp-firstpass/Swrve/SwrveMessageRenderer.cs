using UnityEngine;

namespace Swrve
{
	public class SwrveMessageRenderer
	{
		protected static Color BUTTON_PRESSED_COLOR = new Color(0.5f, 0.5f, 0.5f);

		protected static Texture2D blankTexture;

		protected static Rect WholeScreen = default(Rect);

		public static SwrveMessageAnimator Animator = new SwrveMessageAnimator();

		public static Texture2D GetBlankTexture()
		{
			if (blankTexture == null)
			{
				blankTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				blankTexture.SetPixel(0, 0, Color.white);
				blankTexture.SetPixel(1, 0, Color.white);
				blankTexture.SetPixel(0, 1, Color.white);
				blankTexture.SetPixel(1, 1, Color.white);
				blankTexture.Apply(false, true);
			}
			return blankTexture;
		}

		public static void InitMessage(SwrveMessageFormat format)
		{
			Animator.InitMessage(format);
		}

		public static void AnimateMessage(SwrveMessageFormat format)
		{
			Animator.AnimateMessage(format);
		}

		public static void DrawMessage(SwrveMessageFormat format, int centerx, int centery)
		{
			AnimateMessage(format);
			if (format.BackgroundColor.HasValue && GetBlankTexture() != null)
			{
				Color value = format.BackgroundColor.Value;
				value.a = format.BackgroundAlpha;
				GUI.color = value;
				WholeScreen.width = Screen.width;
				WholeScreen.height = Screen.height;
				GUI.DrawTexture(WholeScreen, blankTexture, ScaleMode.StretchToFill, true, 10f);
				GUI.color = Color.white;
			}
			float num = format.Scale * format.AnimationScale;
			GUI.color = Color.white;
			foreach (SwrveImage image in format.Images)
			{
				Point centeredPosition = image.getCenteredPosition(num, num);
				centeredPosition.X += centerx;
				centeredPosition.Y += centery;
				image.Rect.x = centeredPosition.X;
				image.Rect.y = centeredPosition.Y;
				image.Rect.width = (float)image.Size.X * num;
				image.Rect.height = (float)image.Size.Y * num;
				if (image.Texture != null)
				{
					GUI.DrawTexture(image.Rect, image.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(image.Rect, image.File);
				}
			}
			foreach (SwrveButton button in format.Buttons)
			{
				float num2 = num * button.Scale;
				Point centeredPosition2 = button.getCenteredPosition(num2, num);
				centeredPosition2.X += centerx;
				centeredPosition2.Y += centery;
				button.Rect.x = centeredPosition2.X;
				button.Rect.y = centeredPosition2.Y;
				button.Rect.width = (float)button.Size.X * num2;
				button.Rect.height = (float)button.Size.Y * num2;
				GUI.color = ((!button.Pressed) ? Color.white : BUTTON_PRESSED_COLOR);
				if (button.Texture != null)
				{
					GUI.DrawTexture(button.Rect, button.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(button.Rect, button.Image);
				}
				GUI.color = Color.white;
			}
			if (Animator.IsMessageDismissed(format))
			{
				format.Dismissed = true;
				format.UnloadAssets();
			}
		}
	}
}
