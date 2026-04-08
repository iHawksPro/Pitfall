using System;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

public class SynchronisedClock : MonoBehaviour
{
	public static SynchronisedClock Instance;

	public DateTime ServerTime { get; private set; }

	private void Awake()
	{
		Instance = this;
		StartCoroutine("GetServerTime");
	}

	private IEnumerator GetServerTime()
	{
		WaitForSeconds wfs = new WaitForSeconds(60f);
		while (true)
		{
			WWW www = new WWW("http://nist.time.gov/actualtime.cgi");
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				Debug.Log("SynchronisedClock: Fetched the server time");
				XmlTextReader xtr = new XmlTextReader(new StringReader(www.text));
				while (xtr.Read())
				{
					if (xtr.Name == "timestamp")
					{
						long utcTimestamp = 0L;
						if (long.TryParse(xtr.GetAttribute("time"), out utcTimestamp))
						{
							Debug.Log("SynchronisedClock: Now using the fetched server time");
							ServerTime = ConvertUTCTimestampInSecondsToDateTime(utcTimestamp / 1000000);
						}
						else
						{
							Debug.LogWarning("SynchronisedClock: Failed to parse the fetched server time, defaulting to system time");
							ServerTime = DateTime.UtcNow;
						}
					}
				}
			}
			else
			{
				Debug.LogWarning("SynchronisedClock: Failed to reach the server time, defaulting to system time. Error: " + www.error);
				ServerTime = DateTime.UtcNow;
			}
			Debug.Log("SynchronisedClock: Server time: " + ServerTime);
			yield return wfs;
		}
	}

	public static DateTime ConvertUTCTimestampInSecondsToDateTime(long utcTimestampInSeconds)
	{
		return new DateTime(1970, 1, 1).AddSeconds(utcTimestampInSeconds);
	}
}
