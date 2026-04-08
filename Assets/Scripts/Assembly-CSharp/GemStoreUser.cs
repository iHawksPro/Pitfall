using System;
using UnityEngine;

public class GemStoreUser : ScriptableObject
{
	[Serializable]
	public class GemStoreUserData
	{
		public string UserName = string.Empty;

		public string SegmentDescription = string.Empty;

		public string ProductSlot1Name;

		public string ProductSlot2Name;

		public string ProductSlot3Name;

		public string ProductSlot4Name;

		public string ProductSlot5Name;

		public GemStoreUserData()
		{
		}

		public GemStoreUserData(GemStoreUserData Other)
		{
			UserName = Other.UserName;
			SegmentDescription = Other.SegmentDescription;
			ProductSlot1Name = Other.ProductSlot1Name;
			ProductSlot2Name = Other.ProductSlot2Name;
			ProductSlot3Name = Other.ProductSlot3Name;
			ProductSlot4Name = Other.ProductSlot4Name;
			ProductSlot5Name = Other.ProductSlot5Name;
		}
	}

	public GemStoreUserData[] users;
}
