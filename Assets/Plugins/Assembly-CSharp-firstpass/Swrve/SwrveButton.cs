using UnityEngine;

namespace Swrve
{
	public class SwrveButton : SwrveWidget
	{
		public string Image;

		public string Action;

		public SwrveActionType ActionType;

		public SwrveMessage Message;

		public int GameId;

		public Texture2D Texture;

		public Rect Rect;

		public bool Pressed;

		public float Scale = 1f;
	}
}
