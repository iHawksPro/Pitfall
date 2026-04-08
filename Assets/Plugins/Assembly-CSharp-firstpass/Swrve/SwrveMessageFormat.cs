using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swrve
{
	public class SwrveMessageFormat
	{
		public List<SwrveButton> Buttons;

		public List<SwrveImage> Images;

		public string Name;

		public string Language;

		public Point Size = new Point(0, 0);

		public SwrveMessage Message;

		public float Scale = 1f;

		public double Delay;

		public bool Closing;

		public bool Dismissed;

		public ISwrveButtonListener ButtonListener;

		public IMessageListener MessageListener;

		public Point Position;

		public Point TargetPosition;

		public Color? BackgroundColor = Color.black;

		public float BackgroundAlpha;

		public float AnimationScale = 1f;

		public SwrveMessageFormat(SwrveMessage message)
		{
			Message = message;
			Buttons = new List<SwrveButton>();
			Images = new List<SwrveImage>();
		}

		public static SwrveMessageFormat LoadFromJSON(SwrveMessage message, Hashtable messageFormatData)
		{
			SwrveMessageFormat swrveMessageFormat = new SwrveMessageFormat(message);
			swrveMessageFormat.Name = messageFormatData["name"] as string;
			swrveMessageFormat.Language = messageFormatData["language"] as string;
			if (messageFormatData.ContainsKey("scale"))
			{
				object obj = messageFormatData["scale"];
				if (obj is double)
				{
					swrveMessageFormat.Scale = (float)(double)obj;
				}
				else if (obj is float)
				{
					swrveMessageFormat.Scale = (float)obj;
				}
			}
			Hashtable hashtable = messageFormatData["size"] as Hashtable;
			swrveMessageFormat.Size.X = (int)(double)(hashtable["w"] as Hashtable)["value"];
			swrveMessageFormat.Size.Y = (int)(double)(hashtable["h"] as Hashtable)["value"];
			ArrayList arrayList = messageFormatData["buttons"] as ArrayList;
			int i = 0;
			for (int count = arrayList.Count; i < count; i++)
			{
				SwrveButton item = LoadButtonFromJSON(message, arrayList[i] as Hashtable);
				swrveMessageFormat.Buttons.Add(item);
			}
			ArrayList arrayList2 = messageFormatData["images"] as ArrayList;
			int j = 0;
			for (int count2 = arrayList2.Count; j < count2; j++)
			{
				SwrveImage item2 = LoadImageFromJSON(message, arrayList2[j] as Hashtable);
				swrveMessageFormat.Images.Add(item2);
			}
			return swrveMessageFormat;
		}

		private static SwrveButton LoadButtonFromJSON(SwrveMessage message, Hashtable buttonData)
		{
			SwrveButton swrveButton = new SwrveButton();
			swrveButton.Position.X = (int)(double)(buttonData["x"] as Hashtable)["value"];
			swrveButton.Position.Y = (int)(double)(buttonData["y"] as Hashtable)["value"];
			swrveButton.Size.X = (int)(double)(buttonData["w"] as Hashtable)["value"];
			swrveButton.Size.Y = (int)(double)(buttonData["h"] as Hashtable)["value"];
			swrveButton.Image = (string)(buttonData["image_up"] as Hashtable)["value"];
			swrveButton.Message = message;
			string text = (string)(buttonData["type"] as Hashtable)["value"];
			SwrveActionType actionType = SwrveActionType.Dismiss;
			if (text.ToLower().Equals("install"))
			{
				actionType = SwrveActionType.Install;
			}
			else if (text.ToLower().Equals("custom"))
			{
				actionType = SwrveActionType.Custom;
			}
			swrveButton.ActionType = actionType;
			swrveButton.Action = (string)(buttonData["action"] as Hashtable)["value"];
			if (swrveButton.ActionType == SwrveActionType.Install)
			{
				string text2 = (string)(buttonData["game_id"] as Hashtable)["value"];
				if (text2 != null && !text2.Equals(string.Empty))
				{
					swrveButton.GameId = int.Parse(text2);
				}
			}
			return swrveButton;
		}

		private static SwrveImage LoadImageFromJSON(SwrveMessage message, Hashtable imageData)
		{
			SwrveImage swrveImage = new SwrveImage();
			swrveImage.Position.X = (int)(double)(imageData["x"] as Hashtable)["value"];
			swrveImage.Position.Y = (int)(double)(imageData["y"] as Hashtable)["value"];
			swrveImage.Size.X = (int)(double)(imageData["w"] as Hashtable)["value"];
			swrveImage.Size.Y = (int)(double)(imageData["h"] as Hashtable)["value"];
			swrveImage.File = (string)(imageData["image"] as Hashtable)["value"];
			return swrveImage;
		}

		public void Dismiss()
		{
			if (!Closing)
			{
				Closing = true;
				ButtonListener = null;
				if (MessageListener != null)
				{
					MessageListener.OnDismiss(this);
				}
			}
		}

		public void Init(Point startPoint, Point endPoint)
		{
			Closing = false;
			Dismissed = false;
			Position = startPoint;
			TargetPosition = endPoint;
			if (MessageListener != null)
			{
				MessageListener.OnShow(this);
			}
		}

		public bool IsReady()
		{
			return Delay <= 0.0;
		}

		public void UnloadAssets()
		{
			foreach (SwrveImage image in Images)
			{
				Object.Destroy(image.Texture);
				image.Texture = null;
			}
			foreach (SwrveButton button in Buttons)
			{
				Object.Destroy(button.Texture);
				button.Texture = null;
			}
		}
	}
}
