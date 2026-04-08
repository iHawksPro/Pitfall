using System;
using System.Collections;
using System.Collections.Generic;

namespace Swrve
{
	public class SwrveCampaign
	{
		private static Random rnd = new Random();

		public int Id;

		public List<SwrveMessage> Messages;

		public HashSet<string> Triggers;

		public int Next;

		public bool DismissalsEnabled;

		public int Dismissals;

		public bool RandomOrder;

		public DateTime StartDate;

		public DateTime EndDate;

		public bool DeviceTimezone;

		public SwrveCampaign()
		{
			Messages = new List<SwrveMessage>();
			Triggers = new HashSet<string>();
		}

		public SwrveMessage GetMessageForEvent(string triggerEvent)
		{
			DateTime dateTime = ((!DeviceTimezone) ? DateTime.UtcNow : DateTime.Now);
			int count = Messages.Count;
			if (count == 0)
			{
				SwrveComponent.LogDebug("No messages in campaign " + Id);
				return null;
			}
			if (DismissalsEnabled && Dismissals <= 0)
			{
				SwrveComponent.LogDebug("Campaign " + Id + " has been dismissed too many times already");
				return null;
			}
			if (StartDate > dateTime)
			{
				SwrveComponent.LogDebug("Campaign " + Id + " has not started yet");
				return null;
			}
			if (EndDate < dateTime)
			{
				SwrveComponent.LogDebug("Campaign" + Id + " has finished");
				return null;
			}
			if (!Triggers.Contains(triggerEvent))
			{
				SwrveComponent.LogDebug("There is no trigger in " + Id + " that matches " + triggerEvent);
				return null;
			}
			SwrveComponent.LogDebug(triggerEvent + " matches a trigger in " + Id);
			return GetNextMessage(count);
		}

		protected SwrveMessage GetNextMessage(int messagesCount)
		{
			int num = Next;
			if (RandomOrder)
			{
				num = rnd.Next(messagesCount);
			}
			if (num < Messages.Count)
			{
				return Messages[num];
			}
			return null;
		}

		public void AddMessage(SwrveMessage message)
		{
			Messages.Add(message);
		}

		public static SwrveCampaign LoadFromJSON(Hashtable campaignData, int deviceWidth, int deviceHeight)
		{
			SwrveCampaign swrveCampaign = new SwrveCampaign();
			swrveCampaign.Id = (int)(double)campaignData["id"];
			AssignCampaignTriggers(swrveCampaign, campaignData);
			AssignCampaignRules(swrveCampaign, campaignData);
			AssignCampaignDates(swrveCampaign, campaignData);
			ArrayList arrayList = campaignData["messages"] as ArrayList;
			int i = 0;
			for (int count = arrayList.Count; i < count; i++)
			{
				Hashtable messageData = arrayList[i] as Hashtable;
				SwrveMessage swrveMessage = SwrveMessage.LoadFromJSON(swrveCampaign, messageData, deviceWidth, deviceHeight);
				if (swrveMessage.Formats.Count > 0)
				{
					swrveCampaign.AddMessage(swrveMessage);
				}
			}
			AssignPitfallRules(swrveCampaign, campaignData);
			return swrveCampaign;
		}

		private static void AssignCampaignTriggers(SwrveCampaign campaign, Hashtable campaignData)
		{
			ArrayList arrayList = campaignData["triggers"] as ArrayList;
			int i = 0;
			for (int count = arrayList.Count; i < count; i++)
			{
				string item = arrayList[i] as string;
				campaign.Triggers.Add(item);
			}
		}

		private static void AssignCampaignRules(SwrveCampaign campaign, Hashtable campaignData)
		{
			Hashtable hashtable = campaignData["rules"] as Hashtable;
			campaign.RandomOrder = (hashtable["display_order"] as string).Equals("random");
			if (hashtable.ContainsKey("dismiss_after_views"))
			{
				int num = (int)(double)hashtable["dismiss_after_views"];
				if (num > 0)
				{
					campaign.DismissalsEnabled = true;
					campaign.Dismissals = num;
				}
			}
		}

		private static void AssignCampaignDates(SwrveCampaign campaign, Hashtable campaignData)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			campaign.StartDate = dateTime.AddMilliseconds((long)(double)campaignData["start_date"]);
			campaign.EndDate = dateTime.AddMilliseconds((long)(double)campaignData["end_date"]);
		}

		private static void AssignPitfallRules(SwrveCampaign campaign, Hashtable campaignData)
		{
			foreach (SwrveMessage message in campaign.Messages)
			{
				if (message.Name.Contains("#DEVICE_TMZ#"))
				{
					campaign.DeviceTimezone = true;
				}
			}
		}

		public void DecrementDismissalsRemaining()
		{
			Dismissals--;
		}
	}
}
