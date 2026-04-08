using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Swrve;
using UnityEngine;

public class SwrveTalkComponent : SwrveComponent
{
	public static string SDK_VERSION = "2.0";

	public static double CAMPAIGN_VERSION = 1.0;

	protected static readonly string CampaignsSave = "Swrve_Campaigns";

	protected static readonly string CampaignsSettingsSave = "Swrve_CampaignsData";

	private static float DEFAULT_DPI = 160f;

	protected string campaignsUrl;

	protected string campaignsResult;

	protected bool campaignsConnecting;

	protected List<SwrveCampaign> campaigns = new List<SwrveCampaign>();

	protected Hashtable campaignSettings = new Hashtable();

	protected IDictionary<string, string> gameStoreLinks = new Dictionary<string, string>();

	protected SwrveMessageFormat currentMessage;

	protected SwrveMessageFormat currentDisplayingMessage;

	protected bool processingClicks;

	protected string cdn = "http://swrve-content.s3.amazonaws.com/messaging/message_image/";

	protected string swrvePath;

	protected static int DEFAULT_DELAY_FIRST_MESSAGE;

	protected static long DEFAULT_MAX_SHOWS = 10L;

	protected static int DEFAULT_MIN_DELAY = 30;

	public int deviceWidth;

	public int deviceHeight;

	protected DateTime showMessagesAfter;

	protected long messagesLeftToShow;

	protected int minDelayBetweenMessage;

	public string contentServer = "http://content.swrve.com/api/1/";

	public int forcedDeviceWidth;

	public int forcedDeviceHeight;

	private static SwrveTalkComponent instance;

	public static SwrveTalkComponent Instance
	{
		get
		{
			if (!instance)
			{
				instance = (SwrveTalkComponent)UnityEngine.Object.FindObjectOfType(typeof(SwrveTalkComponent));
				if (!instance)
				{
					Debug.LogError("There needs to be one active SwrveTalkComponent script on a GameObject in your scene.");
				}
			}
			return instance;
		}
	}

	public new void Init(int _swrveId, string _swrveKey, string _swrveUserId)
	{
		try
		{
			base.Init(_swrveId, _swrveKey, _swrveUserId);
			swrvePath = Application.temporaryCachePath;
			if (swrvePath == null || swrvePath.Length == 0)
			{
				swrvePath = Application.persistentDataPath;
				Debug.Log("Swrve path (tried again): " + swrvePath);
			}
			Swrve_LoadData(swrvePath);
			deviceWidth = ((forcedDeviceWidth == 0 || !Application.isEditor) ? Screen.width : forcedDeviceWidth);
			deviceHeight = ((forcedDeviceHeight == 0 || !Application.isEditor) ? Screen.height : forcedDeviceHeight);
			Swrve_SendDeviceInfo();
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError("Error while initializing " + ex);
		}
	}

	private void Swrve_SendDeviceInfo()
	{
		string deviceName = SystemInfo.deviceName;
		string operatingSystem = SystemInfo.operatingSystem;
		float num = ((Screen.dpi != 0f) ? Screen.dpi : DEFAULT_DPI);
		StringBuilder stringBuilder = new StringBuilder("{");
		stringBuilder.AppendFormat("\"swrve.device_name\":\"{0}\", \"swrve.os\":\"{1}\", \"swrve.device_width\":{2}, \"swrve.device_height\":{3}, \"swrve.device_dpi\":{4}, \"swrve.language\":\"{5}\", \"swrve.sdk_version\":\"{6}\"", deviceName, operatingSystem, deviceWidth, deviceHeight, num, language, SDK_VERSION).Append("}");
		Swrve_AddUserUpdateEvent(stringBuilder.ToString());
		Swrve_PostEvents();
	}

	public void ButtonWasPressedByUser(SwrveButton button)
	{
		try
		{
			SwrveComponent.LogDebug(string.Concat("Button ", button.ActionType, ": ", button.Action, " game id: ", button.GameId));
			if (button.ActionType != SwrveActionType.Dismiss)
			{
				string text = "Swrve.Messages.Message-" + button.Message.Id + ".click";
				Debug.Log("Sending click event: " + text);
				Swrve_AddNamedEvent(text, "null");
				Swrve_PostEvents();
			}
			if (button.ActionType == SwrveActionType.Install)
			{
				Debug.Log("Sending click_thru link event");
				string source = "Swrve.Message-" + button.Message.Id;
				Swrve_ClickThru(button.GameId, source);
			}
			else if (button.ActionType == SwrveActionType.Dismiss && button.Message.Campaign != null)
			{
				button.Message.Campaign.DecrementDismissalsRemaining();
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Error while processing button click " + ex);
		}
	}

	public void MessageWasShownToUser(SwrveMessage message)
	{
		try
		{
			DateTime now = DateTime.Now;
			showMessagesAfter = now + TimeSpan.FromSeconds(minDelayBetweenMessage);
			messagesLeftToShow--;
			SwrveCampaign campaign = message.Campaign;
			if (campaign != null)
			{
				if (!campaign.RandomOrder)
				{
					int num = (campaign.Next = (campaign.Next + 1) % campaign.Messages.Count);
					Swrve_SaveCampaignData(swrvePath, campaign);
					Debug.Log("Round Robin: Next message in campaign " + campaign.Id + " is " + num);
				}
				else
				{
					Debug.Log("Next message in campaign is random");
				}
			}
			string text = "Swrve.Messages.Message-" + message.Id + ".impression";
			Debug.Log("Sending view event: " + text);
			Swrve_AddNamedEvent(text, "null");
			Swrve_PostEvents();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error while processing message impression " + ex);
		}
	}

	public bool IsMessageDispaying()
	{
		return currentMessage != null;
	}

	private void OnGUI()
	{
		if (currentDisplayingMessage == null)
		{
			return;
		}
		UIManager.instance.blockInput = true;
		if (currentDisplayingMessage.MessageListener != null)
		{
			currentDisplayingMessage.MessageListener.OnShowing(currentDisplayingMessage);
		}
		SwrveMessageRenderer.DrawMessage(currentMessage, Screen.width / 2 + currentMessage.Position.X, Screen.height / 2 + currentMessage.Position.Y);
		if (processingClicks)
		{
			return;
		}
		processingClicks = true;
		if (Input.GetMouseButtonDown(0) && !currentMessage.Closing)
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.y = (float)Screen.height - mousePosition.y;
			foreach (SwrveButton button in currentMessage.Buttons)
			{
				if (button.Rect.Contains(mousePosition))
				{
					button.Pressed = true;
				}
			}
		}
		if (Input.GetMouseButtonUp(0) && !currentMessage.Closing)
		{
			bool flag = false;
			foreach (SwrveButton button2 in currentMessage.Buttons)
			{
				Vector3 mousePosition2 = Input.mousePosition;
				mousePosition2.y = (float)Screen.height - mousePosition2.y;
				if (button2.Rect.Contains(mousePosition2) && button2.Pressed)
				{
					SwrveComponent.LogDebug("Clicked button " + button2.ActionType);
					bool flag2 = true;
					if (currentMessage.ButtonListener != null)
					{
						flag2 = currentMessage.ButtonListener.OnAction(currentMessage, button2.ActionType, button2.Action, gameId);
					}
					if (flag2)
					{
						flag = true;
						if (button2.ActionType == SwrveActionType.Install)
						{
							string key = button2.GameId.ToString();
							if (gameStoreLinks.ContainsKey(key))
							{
								Application.OpenURL(gameStoreLinks[key]);
							}
						}
						ButtonWasPressedByUser(button2);
					}
				}
				button2.Pressed = false;
			}
			if (flag)
			{
				currentMessage.Dismiss();
			}
		}
		if (currentMessage.Dismissed)
		{
			currentMessage = null;
			currentDisplayingMessage = null;
			UIManager.instance.blockInput = false;
		}
		processingClicks = false;
	}

	private void Start()
	{
	}

	private IEnumerator CheckCampaignAssets(SwrveCampaign campaign)
	{
		bool allAssetsLoaded = true;
		SwrveComponent.LogDebug("Loading campaign " + campaign.Id + " assets");
		foreach (SwrveMessage message in campaign.Messages)
		{
			SwrveComponent.LogDebug("Loading message with " + message.Formats.Count + " formats");
			foreach (SwrveMessageFormat format in message.Formats)
			{
				SwrveComponent.LogDebug("Loading format");
				foreach (SwrveImage image in format.Images)
				{
					if (image.File == null || !(image.File != string.Empty))
					{
						continue;
					}
					SwrveComponent.LogDebug("Loading image " + image.File);
					List<Texture2D> resultTexture = new List<Texture2D>();
					if (!CheckAsset(image.File))
					{
						SwrveComponent.LogDebug("Image not found, loading from server");
						yield return StartCoroutine(DownloadAsset(image.File, cdn + image.File, resultTexture));
						if (resultTexture.Count == 0)
						{
							allAssetsLoaded = false;
						}
					}
				}
				foreach (SwrveButton button in format.Buttons)
				{
					if (button.Image == null || !(button.Image != string.Empty))
					{
						continue;
					}
					SwrveComponent.LogDebug("Loading button image " + button.Image);
					List<Texture2D> resultTexture2 = new List<Texture2D>();
					if (!CheckAsset(button.Image))
					{
						SwrveComponent.LogDebug("Button image not found, loading from server");
						yield return StartCoroutine(DownloadAsset(button.Image, cdn + button.Image, resultTexture2));
						if (resultTexture2.Count == 0)
						{
							allAssetsLoaded = false;
						}
					}
				}
			}
		}
		GC.Collect();
		if (allAssetsLoaded)
		{
			if (campaignSettings != null && campaignSettings.ContainsKey("Next" + campaign.Id))
			{
				int next = (int)(double)campaignSettings["Next" + campaign.Id];
				campaign.Next = next;
			}
			SwrveComponent.LogDebug("Finished loading campaign " + campaign.Id + " assets");
			campaigns.Add(campaign);
		}
		else
		{
			SwrveComponent.LogError("Could not load all the campaign " + campaign.Id + " assets");
		}
	}

	public IEnumerator ShowMessageForEvent(string eventName, ISwrveButtonListener buttonListener, IMessageListener messageListener)
	{
		if (currentMessage != null)
		{
			yield break;
		}
		SwrveMessage message = null;
		DateTime now = DateTime.Now;
		SwrveComponent.LogDebug("Trying to get message for: " + eventName);
		if (campaigns != null && campaigns.Count > 0)
		{
			if (IsTooSoonToShowMessageAt(now))
			{
				SwrveComponent.LogDebug("Not showing messages: too soon after after launch or last message. Next show at " + showMessagesAfter.ToString());
				yield break;
			}
			if (HasShowTooManyMessagesAlready())
			{
				SwrveComponent.LogDebug("Not showing messages: too many messages shown");
				yield break;
			}
			campaigns.Shuffle();
			IEnumerator<SwrveCampaign> itCampaign = campaigns.GetEnumerator();
			while (itCampaign.MoveNext() && message == null)
			{
				SwrveCampaign campaign = itCampaign.Current;
				message = campaign.GetMessageForEvent(eventName);
			}
		}
		if (message == null)
		{
			SwrveComponent.LogDebug("No message found for: " + eventName);
			yield break;
		}
		SwrveMessageFormat selectedFormat = message.Formats[0];
		List<bool> result = new List<bool>();
		yield return StartCoroutine(PreloadFormatAssets(selectedFormat, result));
		if (result.Count != 0)
		{
			ShowMessageFormat(selectedFormat, buttonListener, messageListener);
		}
	}

	private IEnumerator PreloadFormatAssets(SwrveMessageFormat format, List<bool> res)
	{
		SwrveComponent.LogDebug("Loading format");
		bool allLoaded = true;
		foreach (SwrveImage image in format.Images)
		{
			if (image.Texture == null && image.File != null && image.File != string.Empty)
			{
				SwrveComponent.LogDebug("Preloading image " + image.File);
				List<Texture2D> result = new List<Texture2D>();
				yield return StartCoroutine(LoadAsset(image.File, result));
				if (result.Count > 0)
				{
					image.Texture = result[0];
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		foreach (SwrveButton button in format.Buttons)
		{
			if (button.Texture == null && button.Image != null && button.Image != string.Empty)
			{
				SwrveComponent.LogDebug("Preloading button image " + button.Image);
				List<Texture2D> result2 = new List<Texture2D>();
				yield return StartCoroutine(LoadAsset(button.Image, result2));
				if (result2.Count > 0)
				{
					button.Texture = result2[0];
				}
				else
				{
					allLoaded = false;
				}
			}
		}
		if (allLoaded)
		{
			res.Add(true);
		}
	}

	public void DismissMessage()
	{
		try
		{
			if (currentMessage != null)
			{
				currentMessage.Dismiss();
			}
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError("Error while dismissing a message " + ex);
		}
	}

	protected bool HasShowTooManyMessagesAlready()
	{
		return messagesLeftToShow <= 0;
	}

	protected bool IsTooSoonToShowMessageAt(DateTime now)
	{
		return now < showMessagesAfter;
	}

	private SwrveMessageFormat ShowMessageFormat(SwrveMessageFormat format, ISwrveButtonListener buttonListener, IMessageListener messageListener)
	{
		currentMessage = format;
		format.MessageListener = messageListener;
		format.ButtonListener = buttonListener;
		format.Delay = format.Message.Delay;
		if (format.Message.Delay <= 0.0)
		{
			PopUpMessage();
		}
		else
		{
			StartCoroutine(DelayShowMessage_Coroutine(format.Message.Delay));
		}
		return format;
	}

	private void PopUpMessage()
	{
		currentDisplayingMessage = currentMessage;
		SwrveMessageRenderer.InitMessage(currentDisplayingMessage);
		MessageWasShownToUser(currentDisplayingMessage.Message);
	}

	private IEnumerator DelayShowMessage_Coroutine(double delay)
	{
		SwrveComponent.LogDebug("Wait for " + delay + " milliseconds for message");
		yield return new WaitForSeconds((float)(delay / 1000.0));
		PopUpMessage();
	}

	private IEnumerator LoadAsset(string fileName, List<Texture2D> textures)
	{
		if (File.Exists(swrvePath + "/" + fileName))
		{
			WWW www = new WWW("file://" + swrvePath + "/" + fileName);
			yield return www;
			if (www != null && www.error == null)
			{
				Texture2D texture = new Texture2D(4, 4);
				www.LoadImageIntoTexture(texture);
				textures.Add(texture);
			}
		}
	}

	private bool CheckAsset(string fileName)
	{
		if (File.Exists(swrvePath + "/" + fileName))
		{
			return true;
		}
		return false;
	}

	private IEnumerator DownloadAsset(string fileName, string url, List<Texture2D> textures)
	{
		SwrveComponent.LogDebug("Downloading assset: " + url);
		WWW www = new WWW(url);
		yield return www;
		WwwDeducedError err = UnityWwwHelper.DeduceWwwError(www, WwwExpectedResponse.BodyExpected);
		if (www != null && err == WwwDeducedError.NoError && www.isDone)
		{
			Texture2D loadedTexture = www.texture;
			if (loadedTexture != null)
			{
				SwrveComponent.LogDebug("Saving to " + swrvePath + "/" + fileName);
				byte[] bytes = loadedTexture.EncodeToPNG();
				FileStream file = File.Open(swrvePath + "/" + fileName, FileMode.Create);
				BinaryWriter binary = new BinaryWriter(file);
				binary.Write(bytes);
				file.Close();
				bytes = null;
				textures.Add(loadedTexture);
			}
		}
	}

	private void ProcessCampaigns(Hashtable root)
	{
		double num = (double)root["version"];
		if (num != CAMPAIGN_VERSION)
		{
			return;
		}
		cdn = root["cdn_root"] as string;
		Hashtable hashtable = root["game_data"] as Hashtable;
		foreach (string key in hashtable.Keys)
		{
			if (gameStoreLinks.ContainsKey(key))
			{
				gameStoreLinks.Remove(key);
			}
			gameStoreLinks.Add(new KeyValuePair<string, string>(key, ((Hashtable)hashtable[key])["app_store_url"] as string));
		}
		Hashtable hashtable2 = root["rules"] as Hashtable;
		int num2 = ((!hashtable2.ContainsKey("delay_first_message")) ? DEFAULT_DELAY_FIRST_MESSAGE : ((int)(double)hashtable2["delay_first_message"]));
		long num3 = ((!hashtable2.ContainsKey("max_messages_per_session")) ? DEFAULT_MAX_SHOWS : ((long)(double)hashtable2["max_messages_per_session"]));
		int num4 = ((!hashtable2.ContainsKey("min_delay_between_messages")) ? DEFAULT_MIN_DELAY : ((int)(double)hashtable2["min_delay_between_messages"]));
		DateTime now = DateTime.Now;
		minDelayBetweenMessage = num4;
		messagesLeftToShow = num3;
		showMessagesAfter = now + TimeSpan.FromSeconds(num2);
		SwrveComponent.LogDebug("Game rules OK: Delay Seconds: " + num2 + " Max shows: " + num3);
		SwrveComponent.LogDebug("Time is " + now.ToString() + " show messages after " + showMessagesAfter.ToString());
		ArrayList arrayList = root["campaigns"] as ArrayList;
		int i = 0;
		for (int count = arrayList.Count; i < count; i++)
		{
			Hashtable campaignData = arrayList[i] as Hashtable;
			SwrveCampaign swrveCampaign = SwrveCampaign.LoadFromJSON(campaignData, deviceWidth, deviceHeight);
			if (swrveCampaign.Messages.Count > 0)
			{
				SwrveComponent.LogDebug("Started loading assets of campaign");
				StartCoroutine(CheckCampaignAssets(swrveCampaign));
			}
		}
	}

	public void Swrve_GetCampaigns()
	{
		try
		{
			if (!campaignsConnecting)
			{
				campaignsConnecting = true;
				campaigns.Clear();
				StringBuilder stringBuilder = new StringBuilder(campaignsUrl);
				float num = ((Screen.dpi != 0f) ? Screen.dpi : DEFAULT_DPI);
				stringBuilder.AppendFormat("?version=2&user={0}&api_key={1}&app_verion={2}&link_token={3}&device_width={4}&device_height={5}&device_dpi={6}&language={7}&orientation={8}", userId, apiKey, appVersion, linkToken, deviceWidth, deviceHeight, num, language, orientation);
				SwrveComponent.LogDebug("CAMPAIGNS url: " + stringBuilder.ToString());
				StartCoroutine(Swrve_GetCampaigns_Coroutine(stringBuilder.ToString()));
			}
			else
			{
				SwrveComponent.LogError("Error while trying to get campaign data");
			}
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError("Error while trying to get campaign data " + ex);
		}
	}

	public void RefreshCampaigns()
	{
		Swrve_GetCampaigns();
	}

	private Hashtable parseCampaignData(string str)
	{
		return str.hashtableFromJson();
	}

	private IEnumerator Swrve_GetCampaigns_Coroutine(string getRequest)
	{
		using (WWW www = new WWW(getRequest))
		{
			yield return www;
			SwrveComponent.LogDebug("Loading campaigns...");
			WwwDeducedError err = UnityWwwHelper.DeduceWwwError(www, WwwExpectedResponse.BodyExpected);
			if (err == WwwDeducedError.NoError)
			{
				string campaignsCandidate;
				if (ResponseBodyTester.TestJsonStatic(www.bytes, out campaignsCandidate))
				{
					campaignsResult = campaignsCandidate;
					SwrveComponent.LogDebug("Processing campaigns..." + campaignsResult);
				}
				else
				{
					SwrveComponent.LogDebug("Bad campaign JSON: " + campaignsCandidate);
				}
			}
			else
			{
				SwrveComponent.LogError("Campaigns request error: " + err);
			}
			if (campaignsResult != null && campaignsResult.Length != 0)
			{
				Hashtable campaignsData = parseCampaignData(campaignsResult);
				ProcessCampaigns(campaignsData);
				Swrve_SaveCampaignsCache(swrvePath);
			}
			campaignsConnecting = false;
		}
	}

	public void Swrve_SaveCampaignsCache(string path)
	{
		try
		{
			Swrve_SaveString(path, CampaignsSave, campaignsResult);
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError("Error while saving campaigns to the cache " + ex);
		}
	}

	public void Swrve_SaveCampaignData(string path, SwrveCampaign campaign)
	{
		try
		{
			campaignSettings["Next" + campaign.Id] = campaign.Next;
			string saveData = MiniJSON.jsonEncode(campaignSettings);
			Swrve_SaveString(path, CampaignsSettingsSave, saveData);
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError("Error while trying to save campaign settings " + ex);
		}
	}

	public new void Swrve_LoadData(string path)
	{
		base.Swrve_LoadData(path);
		try
		{
			string s = Swrve_LoadString(path, CampaignsSave);
			string decodedString;
			if (ResponseBodyTester.TestJsonStatic(Encoding.UTF8.GetBytes(s), out decodedString))
			{
				campaignsResult = decodedString;
			}
			else
			{
				SwrveComponent.LogDebug("Failed to parse campaigns cache; using default");
			}
		}
		catch (Exception ex)
		{
			SwrveComponent.LogError(ex.ToString());
		}
		try
		{
			string s2 = Swrve_LoadString(path, CampaignsSettingsSave);
			string decodedString2;
			if (ResponseBodyTester.TestJsonStatic(Encoding.UTF8.GetBytes(s2), out decodedString2))
			{
				campaignSettings = decodedString2.hashtableFromJson();
			}
		}
		catch (Exception ex2)
		{
			SwrveComponent.LogError(ex2.ToString());
		}
	}
}
