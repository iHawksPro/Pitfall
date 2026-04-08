using UnityEngine;

public class TimeCycle : MonoBehaviour
{
	public enum WeatherType
	{
		SUNNY = 0,
		RAIN = 1
	}

	public TimeCycleDay[] m_Days = new TimeCycleDay[2]
	{
		new TimeCycleDay("Sunny"),
		new TimeCycleDay("Rain")
	};

	private float m_CurrentTime;

	private WeatherType m_CurrentWeather;

	private float m_TargetTime;

	private WeatherType m_TargetWeather;

	private float m_TweenDuration;

	private float m_CurrentTweenTime;

	private float m_TweenHoursDiff;

	private float m_TweenHourStart;

	public RainMaker m_RainMaker;

	private void Start()
	{
		m_CurrentTime = 0f;
		m_CurrentWeather = WeatherType.SUNNY;
		m_TargetTime = 0f;
		m_TargetWeather = m_CurrentWeather;
	}

	public static void HelperSetTimeAndWeather(int hour, WeatherType weather, float tweenTime)
	{
		GameObject gameObject = GameObject.Find("TimeCycleController");
		if (gameObject != null)
		{
			TimeCycle timeCycle = gameObject.GetComponent("TimeCycle") as TimeCycle;
			if (timeCycle != null)
			{
				timeCycle.SetTimeAndWeather(hour, weather, tweenTime);
			}
		}
	}

	public void SetTimeAndWeather(int hour, WeatherType weather, float tweenTime)
	{
		if (hour < 0)
		{
			hour = 0;
		}
		if (hour >= 8)
		{
			hour = 7;
		}
		m_TargetTime = hour;
		m_TargetWeather = weather;
		m_CurrentTweenTime = 0f;
		m_TweenDuration = tweenTime;
		m_TweenHoursDiff = m_TargetTime - m_CurrentTime;
		if (m_TweenHoursDiff < 0f)
		{
			m_TweenHoursDiff += 8f;
		}
		m_TweenHourStart = m_CurrentTime;
	}

	public TimeCycleHour GetWeatherTweenData(TimeCycleHour start, TimeCycleHour end, float delta)
	{
		TimeCycleHour timeCycleHour = new TimeCycleHour();
		timeCycleHour.m_BackgroundColour = Color.Lerp(start.m_BackgroundColour, end.m_BackgroundColour, delta);
		timeCycleHour.m_AmbientColour = Color.Lerp(start.m_AmbientColour, end.m_AmbientColour, delta);
		timeCycleHour.m_SunColour = Color.Lerp(start.m_SunColour, end.m_SunColour, delta);
		timeCycleHour.m_SunIntensity = Mathf.Lerp(start.m_SunIntensity, end.m_SunIntensity, delta);
		timeCycleHour.m_SunHeading = Mathf.LerpAngle(start.m_SunHeading, end.m_SunHeading, delta);
		timeCycleHour.m_SunPitch = Mathf.LerpAngle(start.m_SunPitch, end.m_SunPitch, delta);
		return timeCycleHour;
	}

	public void ApplySetting(TimeCycleHour hour)
	{
		base.GetComponent<Light>().color = hour.m_SunColour;
		base.GetComponent<Light>().intensity = hour.m_SunIntensity;
		base.GetComponent<Light>().transform.eulerAngles = new Vector3(hour.m_SunPitch, hour.m_SunHeading, 0f);
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
		if (gameObject != null)
		{
			Camera camera = gameObject.GetComponent(typeof(Camera)) as Camera;
			camera.backgroundColor = hour.m_BackgroundColour;
		}
		RenderSettings.ambientLight = hour.m_AmbientColour;
	}

	public void GetHours(WeatherType weatherFrom, WeatherType weatherTo, float time, out TimeCycleHour start, out TimeCycleHour end)
	{
		int num = Mathf.FloorToInt(time);
		if (num < 0)
		{
			num += 8;
		}
		int num2 = Mathf.CeilToInt(time);
		if (num2 >= 8)
		{
			num2 -= 8;
		}
		start = m_Days[(int)weatherFrom].m_Hours[num];
		end = m_Days[(int)weatherTo].m_Hours[num2];
	}

	private void Update()
	{
		if (m_TweenDuration > 0f)
		{
			m_CurrentTweenTime += Time.deltaTime;
			if (m_CurrentTweenTime >= m_TweenDuration)
			{
				m_TweenDuration = 0f;
			}
			else
			{
				float num = m_CurrentTweenTime / m_TweenDuration;
				float currentTime = m_CurrentTime;
				m_CurrentTime = m_TweenHourStart + m_TweenHoursDiff * num;
				if (m_CurrentTime >= 8f)
				{
					m_CurrentTime -= 8f;
				}
				if (Mathf.FloorToInt(currentTime) != Mathf.FloorToInt(m_CurrentTime))
				{
					m_CurrentWeather = m_TargetWeather;
				}
				if (m_CurrentWeather != m_TargetWeather)
				{
					if (m_TargetWeather == WeatherType.RAIN)
					{
						m_RainMaker.Rain(num);
					}
					else if (m_TargetWeather != WeatherType.RAIN)
					{
						m_RainMaker.Rain(1f - num);
					}
				}
			}
		}
		if (m_TweenDuration == 0f)
		{
			m_CurrentWeather = m_TargetWeather;
			m_CurrentTime = m_TargetTime;
			m_TweenDuration = -1f;
			if (m_CurrentWeather == WeatherType.RAIN)
			{
				m_RainMaker.Rain(1f);
			}
			else
			{
				m_RainMaker.Rain(0f);
			}
		}
		TimeCycleHour start;
		TimeCycleHour end;
		GetHours(m_CurrentWeather, m_TargetWeather, m_CurrentTime, out start, out end);
		TimeCycleHour weatherTweenData = GetWeatherTweenData(start, end, m_CurrentTime - (float)Mathf.FloorToInt(m_CurrentTime));
		ApplySetting(weatherTweenData);
	}
}
