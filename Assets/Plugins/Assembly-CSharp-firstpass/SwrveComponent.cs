using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Swrve;
using UnityEngine;

public class SwrveComponent : MonoBehaviour
{
	protected string userId = string.Empty;

	protected string appVersion = string.Empty;

	protected string linkToken;

	protected static readonly string EventsSave = "Swrve_Events";

	protected static readonly string AbTestSave = "Swrve_AbTest";

	private StringBuilder eventBufferStringBuilder;

	private StringBuilder singleEventStringBuilder;

	protected string eventsUrl;

	protected string abTestUrl;

	protected string eventsPostString;

	protected byte[] eventsPostEncodedData;

	protected bool eventsConnecting;

	protected bool abTestConnecting;

	protected string linkAppLaunchUrl;

	protected string linkClickThruUrl;

	protected string linkIdentifieresUrl;

	protected static readonly string AbTestDefault = "[]";

	protected int abTestId;

	protected string abTestResult = AbTestDefault;

	public int gameId;

	public string apiKey = "your_api_key_here";

	public string eventsServer = "http://api.swrve.com/1/";

	public string abTestServer = "http://abtest.swrve.com/api/1/";

	public string linkServer = "https://link.swrve.com/1/";

	public int maxBufferChars = 20000;

	public bool forcePlayerPrefs;

	public string orientation = "both";

	public string language = "en-US";

	public int AbTestId
	{
		get
		{
			return abTestId;
		}
	}

	public string AbTestResult
	{
		get
		{
			return abTestResult;
		}
	}

	public event Action m_onBufferFullEvent;

	public event Action<string> m_onABTestsReady;

	private void EnsureBuffers()
	{
		if (eventBufferStringBuilder == null)
		{
			eventBufferStringBuilder = new StringBuilder(maxBufferChars);
		}
		if (singleEventStringBuilder == null)
		{
			singleEventStringBuilder = new StringBuilder();
		}
	}

	public void Swrve_AddSessionStart()
	{
		Swrve_AppendEventToBuffer(SessionStartEvent.FormatString, UnixTimeHelper.GetMilliseconds());
	}

	public void Swrve_AddSessionEnd()
	{
		Swrve_AppendEventToBuffer(SessionEndEvent.FormatString, UnixTimeHelper.GetMilliseconds());
	}

	public void Swrve_AddNamedEvent(string name, string jsonParameters)
	{
		Swrve_AppendEventToBuffer(NamedEvent.FormatString, UnixTimeHelper.GetMilliseconds(), name, jsonParameters);
	}

	public void Swrve_AddUserUpdateEvent(string jsonAttributes)
	{
		Swrve_AppendEventToBuffer(UserUpdateEvent.FormatString, UnixTimeHelper.GetMilliseconds(), jsonAttributes);
	}

	public void Swrve_AddPurchaseItemEvent(string item, string currency, int cost, int quantity)
	{
		Swrve_AppendEventToBuffer(PurchaseEvent.FormatString, UnixTimeHelper.GetMilliseconds(), item, currency, cost, quantity);
	}

	public void Swrve_AddBuyInEvent(string paymentProvider, string rewardCurrency, int rewardAmount, double localCost, string localCurrency)
	{
		Swrve_AppendEventToBuffer(BuyInEvent.FormatString, UnixTimeHelper.GetMilliseconds(), paymentProvider, rewardCurrency, localCurrency, rewardAmount, localCost);
	}

	public void Swrve_AddCurrencyGivenEvent(string givenCurrency, double amount)
	{
		Swrve_AppendEventToBuffer(CurrencyGivenEvent.FormatString, UnixTimeHelper.GetMilliseconds(), givenCurrency, amount);
	}

	public void Swrve_ClickThru(int gameId, string source)
	{
		StartCoroutine(Swrve_ClickThru_Coroutine(gameId, source));
	}

	private IEnumerator Swrve_ClickThru_Coroutine(int gameId, string source)
	{
		StringBuilder getRequest = new StringBuilder(linkClickThruUrl);
		getRequest.AppendFormat("?user={0}&api_key={1}&link_token={2}&destination={3}&source={4}", userId, apiKey, linkToken, gameId, source);
		using (WWW www = new WWW(getRequest.ToString()))
		{
			yield return www;
			if (UnityWwwHelper.DeduceWwwError(www, WwwExpectedResponse.NoBodyExpected) == WwwDeducedError.NetworkError)
			{
			}
		}
	}

	public void Swrve_PostEvents()
	{
		EnsureBuffers();
		if (eventsPostString == null || eventsPostString.Length == 0)
		{
			eventsPostString = eventBufferStringBuilder.ToString();
			eventBufferStringBuilder.Length = 0;
		}
		if (eventsPostString.Length > 0)
		{
			long seconds = UnixTimeHelper.GetSeconds();
			eventsPostEncodedData = PostBodyBuilder.Build(apiKey, gameId, userId, appVersion, seconds, eventsPostString);
		}
		if (eventsPostEncodedData != null)
		{
			if (!eventsConnecting)
			{
				eventsConnecting = true;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/json; charset=utf-8");
				dictionary.Add("Content-Length", eventsPostEncodedData.Length.ToString());
				Dictionary<string, string> d = dictionary;
				StartCoroutine(Swrve_PostEvents_Coroutine(new Hashtable(d)));
			}
			else
			{
				LogError("Failed to initiate events POST request");
			}
		}
		else
		{
			eventsPostString = null;
		}
	}

	public void Swrve_GetABTests(string userId, string version, string joinDate)
	{
		if (!abTestConnecting)
		{
			abTestConnecting = true;
			StringBuilder stringBuilder = new StringBuilder(abTestUrl);
			stringBuilder.AppendFormat("?user={0}&api_key={1}&app_verion={2}&joined={3}", userId, apiKey, version, joinDate);
			LogDebug("ABTEST url: " + stringBuilder.ToString());
			StartCoroutine(Swrve_GetAbTest_Coroutine(stringBuilder.ToString()));
		}
		else
		{
			LogError("Failed to initiate A/B test GET request");
		}
	}

	private string GetJoinDate()
	{
		if (PlayerPrefs.HasKey("JoinDate"))
		{
			string text = PlayerPrefs.GetString("JoinDate", string.Empty);
			if (text != string.Empty)
			{
				return text;
			}
		}
		string text2 = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
		PlayerPrefs.SetString("JoinDate", text2);
		return text2;
	}

	public void Swrve_SaveData(string path)
	{
		EnsureBuffers();
		Swrve_SaveString(path, AbTestSave, abTestResult);
		StringBuilder stringBuilder = new StringBuilder();
		if (eventsPostString != null && eventsPostString.Length > 0)
		{
			stringBuilder.Append(eventsPostString);
			if (eventBufferStringBuilder.Length > 0)
			{
				stringBuilder.Append(',');
			}
		}
		stringBuilder.Append(eventBufferStringBuilder.ToString());
		string saveData = stringBuilder.ToString();
		Swrve_SaveString(path, EventsSave, saveData);
	}

	public void Swrve_LoadData(string path)
	{
		EnsureBuffers();
		abTestId = 0;
		try
		{
			string s = Swrve_LoadString(path, AbTestSave);
			string decodedString;
			if (ResponseBodyTester.TestJsonStatic(Encoding.UTF8.GetBytes(s), out decodedString))
			{
				abTestResult = decodedString;
			}
			else
			{
				LogDebug("Failed to parse A/B test; using default");
			}
		}
		catch (Exception ex)
		{
			LogError(ex.ToString());
		}
		eventsPostString = Swrve_LoadString(path, EventsSave);
		eventBufferStringBuilder.Length = 0;
	}

	public void Init(int _swrveId, string _swrveKey, string _swrveUserId)
	{
		gameId = _swrveId;
		apiKey = _swrveKey;
		userId = _swrveUserId;
		EnsureBuffers();
		if (linkToken == null)
		{
			linkToken = GenerateLinkToken(userId);
		}
		eventsUrl = eventsServer + "batch";
		abTestUrl = abTestServer + "user_resources";
		linkAppLaunchUrl = linkServer + "app_launch";
		linkClickThruUrl = linkServer + "click_thru";
		linkIdentifieresUrl = linkServer + "send_identifiers";
	}

	private string GenerateLinkToken(string userId)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(userId);
		byte[] b = mD.ComputeHash(bytes);
		return new Guid(b).ToString();
	}

	public void OverrideURLs(string events, string abTest)
	{
		if (events != null)
		{
			eventsServer = events;
			eventsUrl = eventsServer + "batch";
		}
		if (abTest != null)
		{
			abTestServer = abTest;
			abTestUrl = abTestServer + "user_resources";
		}
	}

	private IEnumerator Swrve_PostEvents_Coroutine(Hashtable requestHeaders)
	{
		using (WWW www = new WWW(eventsUrl, eventsPostEncodedData, requestHeaders))
		{
			yield return www;
			if (UnityWwwHelper.DeduceWwwError(www, WwwExpectedResponse.NoBodyExpected) != WwwDeducedError.NetworkError)
			{
				eventsPostString = null;
			}
			eventsConnecting = false;
		}
	}

	private IEnumerator Swrve_GetAbTest_Coroutine(string getRequest)
	{
		using (WWW www = new WWW(getRequest))
		{
			yield return www;
			WwwDeducedError err = UnityWwwHelper.DeduceWwwError(www, WwwExpectedResponse.BodyExpected);
			if (err == WwwDeducedError.NoError)
			{
				string abTestCandidate;
				if (ResponseBodyTester.TestJsonStatic(www.bytes, out abTestCandidate))
				{
					abTestResult = abTestCandidate;
					Debug.Log("returned:" + abTestCandidate);
					abTestId++;
					if (this.m_onABTestsReady != null)
					{
						this.m_onABTestsReady(abTestResult);
					}
				}
				else
				{
					Debug.LogError("request.bytes (bad JSON): " + abTestCandidate);
				}
			}
			else
			{
				Debug.LogError("request error = " + err);
			}
			abTestConnecting = false;
		}
	}

	private void Swrve_AppendEventToBuffer(string fmt, params object[] subs)
	{
		EnsureBuffers();
		singleEventStringBuilder.AppendFormat(fmt, subs);
		if (eventBufferStringBuilder.Length + singleEventStringBuilder.Length <= maxBufferChars)
		{
			if (eventBufferStringBuilder.Length > 0)
			{
				eventBufferStringBuilder.Append(',');
			}
			eventBufferStringBuilder.Append(singleEventStringBuilder.ToString());
		}
		else if (this.m_onBufferFullEvent != null)
		{
			this.m_onBufferFullEvent();
			if (eventBufferStringBuilder.Length + singleEventStringBuilder.Length <= maxBufferChars)
			{
				Swrve_AppendEventToBuffer(fmt, subs);
			}
			else
			{
				Debug.LogWarning("Buffer overflow; discarding event");
			}
		}
		else
		{
			Debug.LogWarning("Buffer overflow; discarding event");
		}
		singleEventStringBuilder.Length = 0;
	}

	protected void Swrve_SaveString(string path, string saveName, string saveData)
	{
		bool flag = false;
		if (!forcePlayerPrefs)
		{
			try
			{
				string text = ((path.Length <= 0) ? string.Empty : "/");
				string text2 = path + text + saveName;
				using (FileStream stream = new FileStream(text2, FileMode.Create))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
					{
						streamWriter.Write(saveData);
					}
				}
				Debug.Log("Wrote file " + text2);
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
			}
		}
		if (!flag)
		{
			try
			{
				PlayerPrefs.SetString(saveName, saveData);
				Debug.Log("Set " + saveName + " in PlayerPrefs");
				flag = true;
			}
			catch (PlayerPrefsException ex2)
			{
				Debug.LogError(ex2.ToString());
			}
		}
		if (!flag)
		{
			Debug.Log(saveName + " not saved!");
		}
	}

	protected string Swrve_LoadString(string path, string saveName)
	{
		string text = null;
		if (!forcePlayerPrefs)
		{
			try
			{
				string text2 = ((path.Length <= 0) ? string.Empty : "/");
				string text3 = path + text2 + saveName;
				using (FileStream stream = new FileStream(text3, FileMode.Open))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						text = streamReader.ReadToEnd();
					}
				}
				Debug.Log("Read file " + text3);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}
		if (text == null)
		{
			try
			{
				if (PlayerPrefs.HasKey(saveName))
				{
					text = PlayerPrefs.GetString(saveName);
					Debug.Log("Got " + saveName + " from PlayerPrefs");
				}
			}
			catch (PlayerPrefsException ex2)
			{
				Debug.Log(ex2.ToString());
			}
		}
		return (text != null) ? text : string.Empty;
	}

	public static void LogDebug(string str)
	{
		Debug.Log(str);
	}

	public static void LogWarn(string str)
	{
		Debug.LogWarning(str);
	}

	public static void LogError(string str)
	{
		Debug.LogError(str);
	}
}
