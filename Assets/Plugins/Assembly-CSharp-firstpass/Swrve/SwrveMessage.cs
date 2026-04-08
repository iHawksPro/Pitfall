using System.Collections;
using System.Collections.Generic;

namespace Swrve
{
	public class SwrveMessage
	{
		public int Id;

		public string Name;

		public SwrveCampaign Campaign;

		public List<SwrveMessageFormat> Formats;

		public double Delay;

		public bool Animated = true;

		public SwrveMessage(SwrveCampaign campaign)
		{
			Campaign = campaign;
			Formats = new List<SwrveMessageFormat>();
		}

		public static SwrveMessage LoadFromJSON(SwrveCampaign campaign, Hashtable messageData, int deviceWidth, int deviceHeight)
		{
			SwrveMessage swrveMessage = new SwrveMessage(campaign);
			swrveMessage.Id = (int)(double)messageData["id"];
			swrveMessage.Name = messageData["name"] as string;
			Hashtable hashtable = messageData["template"] as Hashtable;
			ArrayList arrayList = hashtable["formats"] as ArrayList;
			int i = 0;
			for (int count = arrayList.Count; i < count; i++)
			{
				Hashtable messageFormatData = arrayList[i] as Hashtable;
				SwrveMessageFormat item = SwrveMessageFormat.LoadFromJSON(swrveMessage, messageFormatData);
				swrveMessage.Formats.Add(item);
			}
			PitfallRulesProcessing(campaign, swrveMessage, messageData);
			return swrveMessage;
		}

		private static void PitfallRulesProcessing(SwrveCampaign campaign, SwrveMessage message, Hashtable messageData)
		{
			if (message.Name.StartsWith("DELAY#"))
			{
				string[] array = message.Name.Split('#');
				double result = 0.0;
				double.TryParse(array[1], out result);
				message.Delay = result;
			}
			if (message.Name.Contains("#NO_ANIM#"))
			{
				message.Animated = false;
			}
		}
	}
}
