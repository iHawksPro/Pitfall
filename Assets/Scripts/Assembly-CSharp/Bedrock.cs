using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Swrve;
using UnityEngine;

public class Bedrock : MonoBehaviour
{
	public enum BedrockTextEncoding
	{
		ASCII_127 = 0,
		ASCII_255 = 1,
		UTF7 = 2,
		UTF8 = 3
	}

	private struct brUnityInitSettings
	{
		public string _titleName;

		public string _appVersion;

		public bool _anonymousLogonEnabled;

		public string _swrveId;

		public string _swrveKey;

		public string _swrveAnalyticsUrl;

		public string _swrveABUrl;

		public string _flurryId;

		public brRemoteNotificationType _remoteNotificationTypes;

		public brLogLevel _logLevel;

		public brEnvironment _environment;
	}

	private enum brEnvironment
	{
		BR_ENV_DEVELOPMENT = 0,
		BR_ENV_CERTIFICATION = 1,
		BR_ENV_LIVE = 2
	}

	public enum brLogLevel
	{
		BR_NOTICE = 0,
		BR_WARNING = 1,
		BR_ERROR = 2
	}

	private enum brResult
	{
		BR_SUCCESS = 0,
		BR_LIBRARY_NOT_INITIALIZED = 1,
		BR_USER_IS_NOT_LOGGED_IN = 2,
		BR_INVALID_PARAMETER = 3,
		BR_FEATURE_DISABLED = 4,
		BR_BUFFER_TOO_SMALL = 5,
		BR_INTERNAL_ERROR = 6
	}

	public enum brTaskStatus
	{
		BR_TASK_INVALID_HANDLE = -1,
		BR_TASK_NOT_IN_USE = 0,
		BR_TASK_INIT = 1,
		BR_TASK_PENDING = 2,
		BR_TASK_SUCCESS = 3,
		BR_TASK_FAIL = 4
	}

	public enum brLobbyServerTier
	{
		BR_LOBBY_SERVER_COMMON = 0,
		BR_LOBBY_SERVER_FRANCHISE = 1,
		BR_LOBBY_SERVER_TITLE = 2
	}

	public enum brContentUnlockErrorCode
	{
		BR_SUCCESS = 0,
		BR_CONTENT_UNLOCK_UNKNOWN_ERROR = 1300,
		BR_CONTENT_UNLOCK_KEY_INVALID = 1301,
		BR_CONTENT_UNLOCK_KEY_ALREADY_USED_UP = 1302,
		BR_CONTENT_UNLOCK_SHARED_UNLOCK_LIMIT_REACHED = 1303,
		BR_CONTENT_UNLOCK_DIFFERENT_HARDWARE_ID = 1304,
		BR_CONTENT_UNLOCK_INVALID_CONTENT_OWNER = 1305
	}

	[Flags]
	public enum brRemoteNotificationType
	{
		BR_NOTIFICATION_TYPE_NONE = 0,
		BR_NOTIFICATION_TYPE_BADGE = 1,
		BR_NOTIFICATION_TYPE_SOUND = 2,
		BR_NOTIFICATION_TYPE_ALERT = 4,
		BR_NOTIFICATION_TYPE_NEWSSTAND_CONTENT_AVAILABILITY = 8
	}

	public enum _BrEventType
	{
		BR_LOG_ON_SUCCESS = 0,
		BR_LOG_OFF = 1,
		BR_LOG_ON_FAIL = 2,
		BR_PARAMETERS_AVAILABLE = 3,
		BR_USER_ABTEST_PARAMETERS_AVAILABLE = 4
	}

	public struct brKeyValuePair
	{
		public string key;

		public string val;
	}

	public struct KeyValueArray
	{
		public int size;

		public brKeyValuePair[] pairs;
	}

	public enum ECalendarUnit
	{
		BR_TIME_UNIT_NONE = 0,
		BR_TIME_UNIT_ERA = 1,
		BR_TIME_UNIT_YEAR = 2,
		BR_TIME_UNIT_MONTH = 4,
		BR_TIME_UNIT_DAY = 8,
		BR_TIME_UNIT_HOUR = 0x10,
		BR_TIME_UNIT_MINUTE = 0x20,
		BR_TIME_UNIT_SECOND = 0x40,
		BR_TIME_UNIT_WEEK = 0x80
	}

	public struct brLocalNotificationSettings
	{
		public int second;

		public int minute;

		public int hour;

		public int day;

		public int month;

		public int year;

		public int applicationIconBadgeNumber;

		public string alertBody;

		public string alertAction;

		public string alertLaunchImage;

		public string soundName;

		public ECalendarUnit repeatInterval;

		public bool hasAction;
	}

	public enum brLeaderboardWriteType
	{
		BR_STAT_WRITE_REPLACE = 0,
		BR_STAT_WRITE_ADD = 1,
		BR_STAT_WRITE_MAX = 2,
		BR_STAT_WRITE_MIN = 3,
		BR_STAT_WRITE_REPLACE_WHEN_RATING_INCREASE = 4,
		BR_STAT_WRITE_ADD_WHEN_RATING_INCREASE = 5,
		BR_STAT_WRITE_MAX_WHEN_RATING_INCREASE = 6,
		BR_STAT_WRITE_MIN_WHEN_RATING_INCREASE = 7
	}

	[Serializable]
	public struct brLeaderboardRow
	{
		public uint _leaderboardId;

		public ulong _userId;

		public brLeaderboardWriteType _writeType;

		public long _rating;

		public ulong _rank;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 65)]
		public byte[] _entityName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public int[] _integerFields;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public float[] _floatFields;

		public bool isValid()
		{
			return _userId != 0;
		}

		public string getEntityName()
		{
			return DecodeText(_entityName);
		}

		public override string ToString()
		{
			return string.Format("brLeaderboardRow: Board {0}, UserID {1}, Rating {2}, Rank {3}", _leaderboardId, _userId, _rating, _rank);
		}
	}

	[Serializable]
	public struct brLeaderboardRequestConfig
	{
		public uint _leaderboardId;

		public ulong _userId;

		public brLobbyServerTier _tier;

		public uint _maxResults;
	}

	private const string ImportLib = "Bedrock";

	public const int BR_MAX_USERNAME_LENGTH = 64;

	public const int BR_MAX_DEVICE_NAME_LENGTH = 64;

	public const int BR_LEADERBOARD_ROW_NUM_FIELDS = 5;

	public float _UpdateIntervalSeconds = 0.1f;

	[HideInInspector]
	public brRemoteNotificationType _remoteNotificationTypes = brRemoteNotificationType.BR_NOTIFICATION_TYPE_SOUND | brRemoteNotificationType.BR_NOTIFICATION_TYPE_ALERT;

	public string _flurryId;

	public brLogLevel _logLevel;

	public bool _useProductionServers;

	public bool _useDebugSwrveAPI;

	public bool _autoDownloadUserResources;

	public SWRVEConfig _iPhone;

	public SWRVEConfig _Android;

	public SWRVEConfig _KindleFire;

	public BedrockTextEncoding _textEncoding = BedrockTextEncoding.UTF8;

	public bool _bedrockInitializedExternally;

	private float _timeTillNextUpdate;

	private bool _hasConnected;

	private bool _swrveResourceDownloadCompleted;

	public static bool _UsePlayerPrefsOnUnsupportedPlatforms = true;

	private static bool _bedrockActive = false;

	private static Bedrock _instance = null;

	private static AndroidJavaObject BedrockSupport_plugin;

	private static bool _bedrockRuntimeUnavailable;

	public BedrockLeaderboardConfig Config;

	private static ISwrveButtonListener swrveMessageButtonListener = new PitfallButtonListener();

	private static IMessageListener swrveMessageListener = new SwrvePitfallMessageListener();

	private SWRVEConfig PlatformConfig
	{
		get
		{
			return _Android;
		}
	}

	public static Bedrock Instance
	{
		get
		{
			return _instance;
		}
	}

	public static event EventHandler<EventArgs> TitleParametersChanged;

	public static event EventHandler<EventArgs> UserResourcesChanged;

	[DllImport("Bedrock")]
	private static extern void initBedrock(ref brUnityInitSettings settings);

	[DllImport("Bedrock")]
	private static extern brResult brShutdown();

	[DllImport("Bedrock")]
	private static extern brResult brUpdate();

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsLogEvent(string name, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize, bool trackTime);

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsEndTimedEvent(string name);

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsSetCustomUserInformation([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("Bedrock")]
	private static extern brResult brAnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("Bedrock")]
	private static extern brResult brLogOn(string userName, string password);

	[DllImport("Bedrock")]
	private static extern brResult brLogOff(ulong userId);

	[DllImport("Bedrock")]
	private static extern ulong brGetOnlineId(string userName);

	[DllImport("Bedrock")]
	private static extern brResult brGetMessageOfTheDay(StringBuilder messageBuffer, ref uint messageBufferSize);

	[DllImport("Bedrock")]
	private static extern brResult brGetRemoteVariableAsString(string key, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("Bedrock")]
	private static extern brResult brGetRemoteVariableAsInt(string key, out int val);

	[DllImport("Bedrock")]
	private static extern brResult brGetRemoteVariableAsFloat(string key, out float val);

	[DllImport("Bedrock")]
	private static extern brResult brSetUserCacheVariableAsString(string key, string val);

	[DllImport("Bedrock")]
	private static extern brResult brSetUserCacheVariableAsInt(string key, int val);

	[DllImport("Bedrock")]
	private static extern brResult brSetUserCacheVariableAsFloat(string key, float val);

	[DllImport("Bedrock")]
	private static extern brResult brGetUserCacheVariableAsString(string key, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("Bedrock")]
	private static extern brResult brGetUserCacheVariableAsInt(string key, out int val);

	[DllImport("Bedrock")]
	private static extern brResult brGetUserCacheVariableAsFloat(string key, out float val);

	[DllImport("Bedrock")]
	private static extern brResult brDeleteUserCacheVariable(string key);

	[DllImport("Bedrock")]
	private static extern brResult brDeleteAllUserCacheVariables();

	[DllImport("Bedrock")]
	private static extern bool brHasUserCacheVariable(string key);

	[DllImport("Bedrock")]
	private static extern bool brGetConnected(ulong userId);

	[DllImport("Bedrock")]
	private static extern brLocalNotificationSettings brGetDefaultNotificationSettings();

	[DllImport("Bedrock")]
	private static extern void brScheduleLocalNotification(ref brLocalNotificationSettings settings, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] brKeyValuePair[] keyValueArray, int arraySize);

	[DllImport("Bedrock")]
	private static extern brResult brBeginAsyncRetrieveUserResources(ulong userId);

	[DllImport("Bedrock")]
	private static extern brResult brAcquireRemoteUserResources(string itemId, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 2)] ref brKeyValuePair[] pairs, ref int numPairs);

	[DllImport("Bedrock")]
	private static extern brResult brReleaseRemoteUserResources([MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] brKeyValuePair[] pairs, int numPairs);

	[DllImport("Bedrock")]
	private static extern brResult brGetRemoteUserResourcesAsXML(string itemId, byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("Bedrock")]
	private static extern ulong brGetDeviceAnonymousOnlineId();

	[DllImport("Bedrock")]
	private static extern brResult brGetSwrveId(byte[] byteBuffer, ref uint byteBufferSize);

	[DllImport("Bedrock")]
	private static extern brResult brDeviceAnonymousLogOn();

	[DllImport("Bedrock")]
	private static extern bool brGetLoggedOnAnonymously();

	[DllImport("Bedrock")]
	private static extern short brUnlockContent(brLobbyServerTier level, ulong userId, string licenseCode);

	[DllImport("Bedrock")]
	private static extern brResult brListUnlockedContent(brLobbyServerTier level, ulong userId, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 3)] uint[] contentKeyBuffer, uint contentKeyBufferSize);

	[DllImport("Bedrock")]
	private static extern short brQueryContentFromLicense(brLobbyServerTier level, ulong userId, string licenseCode);

	[DllImport("Bedrock")]
	private static extern brResult brReadContentKeyFromLicenseQuery(short taskHandle, ref uint contentKey);

	[DllImport("Bedrock")]
	private static extern void brStartTask(short taskHandle);

	[DllImport("Bedrock")]
	private static extern void brEndTask(ref short taskHandle);

	[DllImport("Bedrock")]
	private static extern brTaskStatus brGetTaskStatus(short taskHandle);

	[DllImport("Bedrock")]
	private static extern int brGetTaskErrorCode(short taskHandle);

	[DllImport("Bedrock")]
	private static extern short brWriteLeaderboardRow(brLobbyServerTier level, ulong userId, [MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 3)] brLeaderboardRow[] rows, int numRows);

	[DllImport("Bedrock")]
	private static extern short brReadLeaderboardByRank(brLeaderboardRequestConfig aConfig, ulong rank);

	[DllImport("Bedrock")]
	private static extern short brReadLeaderboardByPivot(brLeaderboardRequestConfig aConfig);

	[DllImport("Bedrock")]
	private static extern short brReadLeaderboardByRating(brLeaderboardRequestConfig aConfig, ulong rating);

	[DllImport("Bedrock")]
	private static extern brResult brGetLeaderboardValuesTBF([In][Out][MarshalAs(UnmanagedType.LPArray, SizeConst = 0, SizeParamIndex = 1)] brLeaderboardRow[] rows, uint maxResults);

	private static bool isSupportedPlatform()
	{
		return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android;
	}

	public static bool isBedrockActive()
	{
		return isSupportedPlatform() && _bedrockActive;
	}

	public static string DecodeText(byte[] rawBytes)
	{
		string text = string.Empty;
		if ((bool)_instance)
		{
			switch (_instance._textEncoding)
			{
			case BedrockTextEncoding.ASCII_127:
			{
				StringBuilder stringBuilder2 = new StringBuilder(rawBytes.Length);
				for (int j = 0; j < rawBytes.Length; j++)
				{
					if (rawBytes[j] < 128)
					{
						char value2 = (char)rawBytes[j];
						stringBuilder2.Append(value2);
					}
				}
				text = stringBuilder2.ToString();
				break;
			}
			case BedrockTextEncoding.ASCII_255:
			{
				StringBuilder stringBuilder = new StringBuilder(rawBytes.Length);
				for (int i = 0; i < rawBytes.Length; i++)
				{
					char value = (char)rawBytes[i];
					stringBuilder.Append(value);
				}
				text = stringBuilder.ToString();
				break;
			}
			case BedrockTextEncoding.UTF7:
				text = Encoding.UTF7.GetString(rawBytes);
				break;
			case BedrockTextEncoding.UTF8:
				text = Encoding.UTF8.GetString(rawBytes);
				break;
			}
		}
		else
		{
			text = Encoding.UTF8.GetString(rawBytes);
		}
		int num = text.IndexOf('\0');
		if (num >= 0)
		{
			text = text.Remove(num);
		}
		return text;
	}

	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			_bedrockActive = isSupportedPlatform();
			UnityEngine.Object.DontDestroyOnLoad(this);
			if (_bedrockActive)
			{
				TryInitializePlatformBridge();
			}
			if (!_instance._bedrockInitializedExternally)
			{
				initializeBedrock();
			}
			InitializeSwrveTalk();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void InitializeSwrveTalk()
	{
		SWRVEConfig platformConfig = PlatformConfig;
		int swrveId = int.Parse(platformConfig._sandboxSwrveId);
		string swrveKey = platformConfig._sandboxSwrveKey;
		string abTestServer = platformConfig._sandboxSwrveABUrl;
		string eventsServer = ((!_useDebugSwrveAPI) ? platformConfig._sandboxSwrveAnalyticsUrl : platformConfig._debugSwrveAnalyticsUrl);
		if (_useProductionServers)
		{
			swrveId = int.Parse(platformConfig._productionSwrveId);
			swrveKey = platformConfig._productionSwrveKey;
			eventsServer = ((!_useDebugSwrveAPI) ? platformConfig._productionSwrveAnalyticsUrl : platformConfig._debugSwrveAnalyticsUrl);
			abTestServer = platformConfig._productionSwrveABUrl;
		}
		if (SwrveTalkComponent.Instance != null)
		{
			SwrveTalkComponent.Instance.abTestServer = abTestServer;
			SwrveTalkComponent.Instance.eventsServer = eventsServer;
			string deviceLocaleLanguage = TBFUtils.DeviceLocaleLanguage;
			Debug.Log("** SWRVETALK ** Language = " + deviceLocaleLanguage);
			SwrveTalkComponent.Instance.language = deviceLocaleLanguage;
			SwrveTalkComponent.Instance.Init(swrveId, swrveKey, GetSwrveId().ToString());
		}
	}

	private static void SwrveTalkShowMessage(string eventName)
	{
		if (RecoveredCompatibility.DisableLegacyStartupPromos)
		{
			return;
		}
		if (SwrveTalkComponent.Instance == null)
		{
			return;
		}
		try
		{
			if (!isBedrockActive() && Application.isEditor)
			{
				SwrveTalkComponent.Instance.Swrve_AddNamedEvent(eventName, "null");
			}
			SwrveTalkComponent.Instance.StartCoroutine(SwrveTalkComponent.Instance.ShowMessageForEvent(eventName, swrveMessageButtonListener, swrveMessageListener));
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Skipping Swrve event '" + eventName + "' in compatibility mode: " + ex.Message);
		}
	}

	public static void StartUp()
	{
		if (_instance != null && !_bedrockActive && !_bedrockRuntimeUnavailable)
		{
			_instance._hasConnected = false;
			_bedrockActive = true;
			_instance.initializeBedrock();
		}
	}

	private static void DisableBedrockForCurrentSession(string reason)
	{
		_bedrockActive = false;
		_bedrockRuntimeUnavailable = true;
		Debug.LogWarning("Bedrock disabled for this session: " + reason);
	}

	private static void TryInitializePlatformBridge()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				BedrockSupport_plugin = new AndroidJavaObject("com.stovepipe.cp.bedrock.BedrockSupport", androidJavaObject);
				BedrockSupport_plugin.Call("InitialTasks");
			}
		}
		catch (Exception ex)
		{
			BedrockSupport_plugin = null;
			DisableBedrockForCurrentSession("Android bridge unavailable (" + ex.Message + ")");
		}
	}

	private void initializeBedrock()
	{
		if (!isBedrockActive())
		{
			return;
		}
		brUnityInitSettings settings = new brUnityInitSettings
		{
			_logLevel = _logLevel,
			_remoteNotificationTypes = _remoteNotificationTypes,
			_flurryId = _flurryId
		};
		_useProductionServers = true;
		SWRVEConfig platformConfig = PlatformConfig;
		settings._titleName = platformConfig._titleName;
		settings._appVersion = platformConfig._version;
		settings._anonymousLogonEnabled = platformConfig._anonymousLogin;
		if (_useProductionServers)
		{
			settings._environment = brEnvironment.BR_ENV_LIVE;
			settings._swrveId = platformConfig._productionSwrveId;
			settings._swrveKey = platformConfig._productionSwrveKey;
			if (_useDebugSwrveAPI)
			{
				settings._swrveAnalyticsUrl = platformConfig._debugSwrveAnalyticsUrl;
			}
			else
			{
				settings._swrveAnalyticsUrl = platformConfig._productionSwrveAnalyticsUrl;
			}
			settings._swrveABUrl = platformConfig._productionSwrveABUrl;
		}
		else
		{
			settings._environment = brEnvironment.BR_ENV_DEVELOPMENT;
			settings._swrveId = platformConfig._sandboxSwrveId;
			settings._swrveKey = platformConfig._sandboxSwrveKey;
			if (_useDebugSwrveAPI)
			{
				settings._swrveAnalyticsUrl = platformConfig._debugSwrveAnalyticsUrl;
			}
			else
			{
				settings._swrveAnalyticsUrl = platformConfig._sandboxSwrveAnalyticsUrl;
			}
			settings._swrveABUrl = platformConfig._sandboxSwrveABUrl;
		}
		TBFUtils.DebugLog("settings._titleName = " + settings._titleName);
		try
		{
			initBedrock(ref settings);
		}
		catch (Exception ex)
		{
			DisableBedrockForCurrentSession("native library unavailable (" + ex.Message + ")");
		}
	}

	private void Update()
	{
		if (isBedrockActive())
		{
			_timeTillNextUpdate -= Time.deltaTime;
			if (_timeTillNextUpdate <= 0f)
			{
				_timeTillNextUpdate = _UpdateIntervalSeconds;
				try
				{
					brUpdate();
				}
				catch (Exception ex)
				{
					DisableBedrockForCurrentSession("update failed (" + ex.Message + ")");
				}
			}
		}
	}

	public static void ForceUpdate()
	{
		if (isBedrockActive() && _instance != null)
		{
			_instance.Update();
		}
	}

	public static bool Shutdown()
	{
		brResult brResult2 = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive() && _instance != null)
		{
			brResult2 = brShutdown();
			_bedrockActive = false;
		}
		return brResult2 == brResult.BR_SUCCESS;
	}

	public static bool hasConnectedAfterStartup()
	{
		bool result = false;
		if (_instance != null)
		{
			result = _instance._hasConnected;
		}
		return result;
	}

	private void BedrockEventNotice(string message)
	{
		switch (message)
		{
		case "BR_LOG_ON_SUCCESS":
			_hasConnected = true;
			break;
		case "BR_PARAMETERS_AVAILABLE":
		{
			float result;
			if (_autoDownloadUserResources && !_swrveResourceDownloadCompleted && float.TryParse(PlatformConfig._version, out result))
			{
				float remoteVariableAsFloat = GetRemoteVariableAsFloat("latestAppVersion", result);
				if (result >= remoteVariableAsFloat)
				{
					AsyncRefreshUserResources(getAnonymousUserId());
				}
			}
			if (Bedrock.TitleParametersChanged != null)
			{
				Bedrock.TitleParametersChanged(this, new EventArgs());
			}
			break;
		}
		case "BR_USER_ABTEST_PARAMETERS_AVAILABLE":
			_swrveResourceDownloadCompleted = true;
			if (Bedrock.UserResourcesChanged != null)
			{
				Bedrock.UserResourcesChanged(this, new EventArgs());
			}
			break;
		}
	}

	public void RegisterForRemoteNotificationsFailed(string message)
	{
		TBFUtils.DebugLog("Received RegisterForRemoteNotificationsFailed: " + message);
	}

	public static string GetMessageOfTheDay(string defaultValue)
	{
		if (isBedrockActive())
		{
			uint num = 1024u;
			uint messageBufferSize = num;
			StringBuilder stringBuilder = new StringBuilder((int)messageBufferSize);
			brResult brResult2 = brGetMessageOfTheDay(stringBuilder, ref messageBufferSize);
			if (messageBufferSize > num)
			{
				stringBuilder = new StringBuilder((int)messageBufferSize);
				brResult2 = brGetMessageOfTheDay(stringBuilder, ref messageBufferSize);
			}
			if (brResult2 == brResult.BR_SUCCESS)
			{
				return stringBuilder.ToString();
			}
		}
		return defaultValue;
	}

	public static string GetRemoteVariableAsString(string key, string defaultValue)
	{
		if (isBedrockActive())
		{
			uint num = 1024u;
			uint byteBufferSize = num;
			byte[] array = new byte[byteBufferSize];
			brResult brResult2 = brGetRemoteVariableAsString(key, array, ref byteBufferSize);
			if (byteBufferSize > num)
			{
				array = new byte[byteBufferSize];
				brResult2 = brGetRemoteVariableAsString(key, array, ref byteBufferSize);
			}
			if (brResult2 == brResult.BR_SUCCESS)
			{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		return defaultValue;
	}

	public static int GetRemoteVariableAsInt(string key, int defaultValue)
	{
		if (isBedrockActive())
		{
			int val = defaultValue;
			if (brGetRemoteVariableAsInt(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		return defaultValue;
	}

	public static float GetRemoteVariableAsFloat(string key, float defaultValue)
	{
		if (isBedrockActive())
		{
			float val = defaultValue;
			if (brGetRemoteVariableAsFloat(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		return defaultValue;
	}

	public static bool UserVariableExists(string key)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brHasUserCacheVariable(key);
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			result = PlayerPrefs.HasKey(key);
		}
		return result;
	}

	public static bool DeleteUserVariable(string key)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brDeleteUserCacheVariable(key) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.DeleteKey(key);
			return true;
		}
		return result;
	}

	public static bool DeleteAllUserVariables()
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brDeleteAllUserCacheVariables() == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.DeleteAll();
			return true;
		}
		return result;
	}

	public static string GetUserVariableAsString(string key, string defaultValue)
	{
		if (isSupportedPlatform())
		{
			uint num = 1024u;
			uint byteBufferSize = num;
			byte[] array = new byte[byteBufferSize];
			brResult brResult2 = brGetUserCacheVariableAsString(key, array, ref byteBufferSize);
			if (byteBufferSize > num)
			{
				array = new byte[byteBufferSize];
				brResult2 = brGetUserCacheVariableAsString(key, array, ref byteBufferSize);
			}
			if (brResult2 == brResult.BR_SUCCESS)
			{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms && PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}
		return defaultValue;
	}

	public static int GetUserVariableAsInt(string key, int defaultValue)
	{
		if (isSupportedPlatform())
		{
			int val = defaultValue;
			if (brGetUserCacheVariableAsInt(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}
		return defaultValue;
	}

	public static float GetUserVariableAsFloat(string key, float defaultValue)
	{
		if (isSupportedPlatform())
		{
			float val = defaultValue;
			if (brGetUserCacheVariableAsFloat(key, out val) == brResult.BR_SUCCESS)
			{
				return val;
			}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}
		return defaultValue;
	}

	public static bool GetUserVariableAsBool(string key, bool defaultValue)
	{
		if (isSupportedPlatform())
		{
			string userVariableAsString = GetUserVariableAsString(key, "DEFAULT");
			if ("DEFAULT" != userVariableAsString)
			{
				bool result = defaultValue;
				if (bool.TryParse(userVariableAsString, out result))
				{
					return result;
				}
			}
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			string text = PlayerPrefs.GetString(key, "DEFAULT");
			if ("DEFAULT" != text)
			{
				bool result2 = defaultValue;
				if (bool.TryParse(text, out result2))
				{
					return result2;
				}
			}
		}
		return defaultValue;
	}

	public static bool SetUserVariableAsString(string key, string val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brSetUserCacheVariableAsString(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetString(key, val);
			PlayerPrefs.Save();
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsInt(string key, int val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brSetUserCacheVariableAsInt(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetInt(key, val);
			PlayerPrefs.Save();
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsFloat(string key, float val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brSetUserCacheVariableAsFloat(key, val) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetFloat(key, val);
			PlayerPrefs.Save();
			return true;
		}
		return result;
	}

	public static bool SetUserVariableAsBool(string key, bool val)
	{
		bool result = false;
		if (isSupportedPlatform())
		{
			result = brSetUserCacheVariableAsString(key, val.ToString()) == brResult.BR_SUCCESS;
		}
		else if (_UsePlayerPrefsOnUnsupportedPlatforms)
		{
			PlayerPrefs.SetString(key, val.ToString());
			PlayerPrefs.Save();
			return true;
		}
		return result;
	}

	public static bool AsyncRefreshUserResources(ulong userId)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brBeginAsyncRetrieveUserResources(userId) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool GetRemoteUserResources(string itemId, out Dictionary<string, string> resourceDictionary)
	{
		bool flag = false;
		resourceDictionary = null;
		if (isBedrockActive())
		{
			uint byteBufferSize = 2048u;
			byte[] array = new byte[byteBufferSize];
			brResult brResult2 = brGetRemoteUserResourcesAsXML(itemId, array, ref byteBufferSize);
			if (brResult2 == brResult.BR_BUFFER_TOO_SMALL)
			{
				array = new byte[byteBufferSize];
				brResult2 = brGetRemoteUserResourcesAsXML(itemId, array, ref byteBufferSize);
			}
			string text = DecodeText(array);
			if (brResult2 == brResult.BR_SUCCESS && !string.IsNullOrEmpty(text))
			{
				flag = true;
			}
			if (flag)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(text);
				XmlNodeList childNodes = xmlDocument.FirstChild.ChildNodes;
				resourceDictionary = new Dictionary<string, string>(childNodes.Count);
				foreach (XmlNode item in childNodes)
				{
					resourceDictionary.Add(item.Attributes["key"].Value, item.Attributes["value"].Value);
				}
			}
			if (TBFUtils.Is256mbDevice())
			{
				GC.Collect();
			}
		}
		return flag;
	}

	public static string GetFromResourceDictionaryAsString(Dictionary<string, string> resourceDictionary, string key, string defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			return resourceDictionary[key];
		}
		return defaultValue;
	}

	public static int GetFromResourceDictionaryAsInt(Dictionary<string, string> resourceDictionary, string key, int defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string s = resourceDictionary[key];
			int result = defaultValue;
			if (int.TryParse(s, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static float GetFromResourceDictionaryAsFloat(Dictionary<string, string> resourceDictionary, string key, float defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string s = resourceDictionary[key];
			float result = defaultValue;
			if (float.TryParse(s, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static bool GetFromResourceDictionaryAsBool(Dictionary<string, string> resourceDictionary, string key, bool defaultValue)
	{
		if (resourceDictionary.ContainsKey(key))
		{
			string value = resourceDictionary[key];
			bool result = defaultValue;
			if (bool.TryParse(value, out result))
			{
				return result;
			}
		}
		return defaultValue;
	}

	public static void ScheduleLocalNotification(string body, string action, int applicationIconBadgeNumber, DateTime notificationTime, KeyValueArray vals)
	{
		if (isBedrockActive())
		{
			brLocalNotificationSettings settings = brGetDefaultNotificationSettings();
			settings.alertBody = body;
			settings.hasAction = !string.IsNullOrEmpty(action);
			settings.alertAction = action;
			settings.applicationIconBadgeNumber = applicationIconBadgeNumber;
			settings.second = notificationTime.Second;
			settings.minute = notificationTime.Minute;
			settings.hour = notificationTime.Hour;
			settings.day = notificationTime.Day;
			settings.month = notificationTime.Month;
			settings.year = notificationTime.Year;
			brScheduleLocalNotification(ref settings, vals.pairs, vals.size);
		}
	}

	public static void ScheduleLocalNotification(string body, string action, int applicationIconBadgeNumber, TimeSpan timeFromNow)
	{
		KeyValueArray vals = default(KeyValueArray);
		vals.pairs = null;
		vals.size = 0;
		ScheduleLocalNotification(body, action, applicationIconBadgeNumber, timeFromNow, vals);
	}

	public static void ScheduleLocalNotification(string body, string action, int applicationIconBadgeNumber, TimeSpan timeFromNow, KeyValueArray vals)
	{
		ScheduleLocalNotification(body, action, applicationIconBadgeNumber, DateTime.Now.Add(timeFromNow), vals);
	}

	public static bool AnalyticsLogEvent(string name)
	{
		return AnalyticsLogEvent(name, false);
	}

	public static bool AnalyticsLogEvent(string name, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			return brAnalyticsLogEvent(name, null, 0, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key, string val, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key;
			brKeyValuePair2.val = val;
			brKeyValuePair[] keyValueArray = new brKeyValuePair[1] { brKeyValuePair2 };
			int arraySize = 1;
			return brAnalyticsLogEvent(name, keyValueArray, arraySize, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key1;
			brKeyValuePair2.val = val1;
			brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
			brKeyValuePair3.key = key2;
			brKeyValuePair3.val = val2;
			return brAnalyticsLogEvent(name, new brKeyValuePair[2] { brKeyValuePair2, brKeyValuePair3 }, 2, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, string key3, string val3, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key1;
			brKeyValuePair2.val = val1;
			brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
			brKeyValuePair3.key = key2;
			brKeyValuePair3.val = val2;
			brKeyValuePair brKeyValuePair4 = default(brKeyValuePair);
			brKeyValuePair4.key = key3;
			brKeyValuePair4.val = val3;
			return brAnalyticsLogEvent(name, new brKeyValuePair[3] { brKeyValuePair2, brKeyValuePair3, brKeyValuePair4 }, 3, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, string key3, string val3, string key4, string val4, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key1;
			brKeyValuePair2.val = val1;
			brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
			brKeyValuePair3.key = key2;
			brKeyValuePair3.val = val2;
			brKeyValuePair brKeyValuePair4 = default(brKeyValuePair);
			brKeyValuePair4.key = key3;
			brKeyValuePair4.val = val3;
			brKeyValuePair brKeyValuePair5 = default(brKeyValuePair);
			brKeyValuePair5.key = key4;
			brKeyValuePair5.val = val4;
			return brAnalyticsLogEvent(name, new brKeyValuePair[4] { brKeyValuePair2, brKeyValuePair3, brKeyValuePair4, brKeyValuePair5 }, 4, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, string key1, string val1, string key2, string val2, string key3, string val3, string key4, string val4, string key5, string val5, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key1;
			brKeyValuePair2.val = val1;
			brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
			brKeyValuePair3.key = key2;
			brKeyValuePair3.val = val2;
			brKeyValuePair brKeyValuePair4 = default(brKeyValuePair);
			brKeyValuePair4.key = key3;
			brKeyValuePair4.val = val3;
			brKeyValuePair brKeyValuePair5 = default(brKeyValuePair);
			brKeyValuePair5.key = key4;
			brKeyValuePair5.val = val4;
			brKeyValuePair brKeyValuePair6 = default(brKeyValuePair);
			brKeyValuePair6.key = key5;
			brKeyValuePair6.val = val5;
			return brAnalyticsLogEvent(name, new brKeyValuePair[5] { brKeyValuePair2, brKeyValuePair3, brKeyValuePair4, brKeyValuePair5, brKeyValuePair6 }, 5, trackTime) == brResult.BR_SUCCESS;
		}
		return false;
	}

	public static bool AnalyticsLogEvent(string name, KeyValueArray parameters, bool trackTime)
	{
		SwrveTalkShowMessage(name);
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogEvent(name, parameters.pairs, parameters.size, trackTime) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType)
	{
		return AnalyticsLogVirtualPurchase(itemID, itemCost, quantity, virtualCurrencyType, new KeyValueArray
		{
			size = 0,
			pairs = new brKeyValuePair[0]
		});
	}

	public static bool AnalyticsLogVirtualPurchase(string itemID, ulong itemCost, ulong quantity, string virtualCurrencyType, KeyValueArray parameters)
	{
		bool result = false;
		AnalyticsLogEvent("Purchasing.Currency.Virtual.Purchase");
		if (isBedrockActive())
		{
			result = brAnalyticsLogVirtualPurchase(itemID, itemCost, quantity, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType)
	{
		return AnalyticsLogRealPurchase(realCost, localCurrencyCode, paymentProvider, virtualCurrencyAmount, virtualCurrencyType, new KeyValueArray
		{
			size = 0,
			pairs = new brKeyValuePair[0]
		});
	}

	public static bool AnalyticsLogRealPurchase(ulong realCost, string localCurrencyCode, string paymentProvider, ulong virtualCurrencyAmount, string virtualCurrencyType, KeyValueArray parameters)
	{
		bool result = false;
		AnalyticsLogEvent("Purchasing.Currency.Real.Purchase");
		if (isBedrockActive())
		{
			result = brAnalyticsLogRealPurchase(realCost, localCurrencyCode, paymentProvider, virtualCurrencyAmount, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, string key1, string value1, string key2, string value2)
	{
		brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
		brKeyValuePair2.key = key1;
		brKeyValuePair2.val = value1;
		brKeyValuePair brKeyValuePair3 = default(brKeyValuePair);
		brKeyValuePair3.key = key2;
		brKeyValuePair3.val = value2;
		int num = 2;
		brKeyValuePair[] array = new brKeyValuePair[num];
		array[0] = brKeyValuePair2;
		array[1] = brKeyValuePair3;
		KeyValueArray parameters = default(KeyValueArray);
		parameters.pairs = array;
		parameters.size = num;
		return AnalyticsLogVirtualCurrencyAwarded(virtualCurrencyAmount, virtualCurrencyType, parameters);
	}

	public static bool AnalyticsLogVirtualCurrencyAwarded(ulong virtualCurrencyAmount, string virtualCurrencyType, KeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsLogVirtualCurrencyAwarded(virtualCurrencyAmount, virtualCurrencyType, parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsSetCustomUserInformation(string key, string val)
	{
		bool result = false;
		if (isBedrockActive())
		{
			brKeyValuePair brKeyValuePair2 = default(brKeyValuePair);
			brKeyValuePair2.key = key;
			brKeyValuePair2.val = val;
			brKeyValuePair[] keyValueArray = new brKeyValuePair[1] { brKeyValuePair2 };
			int arraySize = 1;
			result = brAnalyticsSetCustomUserInformation(keyValueArray, arraySize) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsSetCustomUserInformation(KeyValueArray parameters)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsSetCustomUserInformation(parameters.pairs, parameters.size) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool AnalyticsEndTimedEvent(string name)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brAnalyticsEndTimedEvent(name) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static short StartUnlockContent(brLobbyServerTier level, ulong userId, string licenseCode)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brUnlockContent(level, userId, licenseCode);
			brStartTask(num);
		}
		return num;
	}

	public static uint[] ListUnlockedContent(brLobbyServerTier level, ulong userId, uint maxContentKeys)
	{
		uint[] array = new uint[0];
		if (isBedrockActive())
		{
			uint[] array2 = new uint[maxContentKeys];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = uint.MaxValue;
			}
			if (brListUnlockedContent(level, userId, array2, maxContentKeys) == brResult.BR_SUCCESS)
			{
				int j;
				for (j = 0; array2[j] != uint.MaxValue; j++)
				{
				}
				array = new uint[j];
				for (int k = 0; k < j; k++)
				{
					array[k] = array2[k];
				}
			}
		}
		return array;
	}

	public static short StartAsyncGetContentKeyFromLicense(brLobbyServerTier level, ulong userId, string licenseCode)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brQueryContentFromLicense(level, userId, licenseCode);
			brStartTask(num);
		}
		return num;
	}

	public static bool FinishAsyncGetContentKeyFromLicense(short taskHandle, ref uint contentKey)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brReadContentKeyFromLicenseQuery(taskHandle, ref contentKey) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static brTaskStatus getTaskStatus(short taskHandle)
	{
		brTaskStatus result = brTaskStatus.BR_TASK_INVALID_HANDLE;
		if (isBedrockActive())
		{
			result = brGetTaskStatus(taskHandle);
		}
		return result;
	}

	public static void EndTask(ref short taskHandle)
	{
		if (isBedrockActive())
		{
			brEndTask(ref taskHandle);
		}
	}

	public static void ReleaseTaskHandle(ref short taskHandle)
	{
		if (isBedrockActive())
		{
			brEndTask(ref taskHandle);
		}
	}

	public static int getTaskErrorCode(short taskHandle)
	{
		int result = 0;
		if (isBedrockActive())
		{
			result = brGetTaskErrorCode(taskHandle);
		}
		return result;
	}

	public static bool StartLogOn(string username, string password)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brLogOn(username, password) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool StartLogOff(ulong userId)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brLogOff(userId) == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool isUserConnected(ulong userId)
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brGetConnected(userId);
		}
		return result;
	}

	public static ulong getUserId(string username)
	{
		ulong result = 0uL;
		if (isBedrockActive())
		{
			result = brGetOnlineId(username);
		}
		return result;
	}

	public static bool StartDeviceAnonymousLogOn()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brDeviceAnonymousLogOn() == brResult.BR_SUCCESS;
		}
		return result;
	}

	public static bool isDeviceAnonymouslyLoggedOn()
	{
		bool result = false;
		if (isBedrockActive())
		{
			result = brGetLoggedOnAnonymously();
		}
		return result;
	}

	public static ulong getAnonymousUserId()
	{
		ulong result = 0uL;
		if (isBedrockActive())
		{
			result = brGetDeviceAnonymousOnlineId();
		}
		return result;
	}

	public static string GetSwrveId()
	{
		if (isBedrockActive())
		{
			uint num = 128u;
			uint byteBufferSize = num;
			byte[] array = new byte[byteBufferSize];
			brResult brResult2 = brGetSwrveId(array, ref byteBufferSize);
			if (byteBufferSize > num)
			{
				array = new byte[byteBufferSize];
				brResult2 = brGetSwrveId(array, ref byteBufferSize);
			}
			if (brResult2 == brResult.BR_SUCCESS)
			{
				string text = DecodeText(array);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
		}
		return "unityEditor";
	}

	public static short StartWriteToLeaderboardRequest(brLobbyServerTier tier, ulong userId, brLeaderboardRow[] rows)
	{
		short num = -1;
		if (isBedrockActive())
		{
			num = brWriteLeaderboardRow(tier, userId, rows, rows.Length);
			if (num >= 0)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByPivot(brLobbyServerTier tier, uint leaderboardId, ulong userId, uint maxResults)
	{
		short num = -1;
		if (isBedrockActive())
		{
			brLeaderboardRequestConfig aConfig = default(brLeaderboardRequestConfig);
			aConfig._tier = tier;
			aConfig._userId = userId;
			aConfig._leaderboardId = leaderboardId;
			aConfig._maxResults = maxResults;
			num = brReadLeaderboardByPivot(aConfig);
			if (num >= 0)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByRank(brLobbyServerTier tier, uint leaderboardId, ulong userId, ulong rank, uint maxResults)
	{
		short num = -1;
		if (isBedrockActive())
		{
			brLeaderboardRequestConfig aConfig = default(brLeaderboardRequestConfig);
			aConfig._tier = tier;
			aConfig._userId = userId;
			aConfig._leaderboardId = leaderboardId;
			aConfig._maxResults = maxResults;
			num = brReadLeaderboardByRank(aConfig, rank);
			if (num >= 0)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static short StartReadLeaderboardByRating(brLobbyServerTier tier, uint leaderboardId, ulong userId, ulong rating, uint maxResults)
	{
		short num = -1;
		if (isBedrockActive())
		{
			brLeaderboardRequestConfig aConfig = default(brLeaderboardRequestConfig);
			aConfig._tier = tier;
			aConfig._userId = userId;
			aConfig._leaderboardId = leaderboardId;
			aConfig._maxResults = maxResults;
			num = brReadLeaderboardByRating(aConfig, rating);
			if (num >= 0)
			{
				brStartTask(num);
			}
		}
		return num;
	}

	public static bool GetLeaderboardResults(short taskHandle, brLeaderboardRow[] rows, uint maxResults, bool friendsOnly)
	{
		brResult brResult2 = brResult.BR_LIBRARY_NOT_INITIALIZED;
		if (isBedrockActive())
		{
			brResult2 = brGetLeaderboardValuesTBF(rows, maxResults);
		}
		return brResult2 == brResult.BR_SUCCESS;
	}
}
