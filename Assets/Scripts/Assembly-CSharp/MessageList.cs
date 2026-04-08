using System;
using UnityEngine;

public class MessageList : ScriptableObject
{
	[Serializable]
	public class MessageData
	{
		public string Title;

		public string Body;
	}

	public MessageData[] messages;
}
