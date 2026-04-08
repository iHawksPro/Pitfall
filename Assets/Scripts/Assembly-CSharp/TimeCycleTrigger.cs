using UnityEngine;

public class TimeCycleTrigger : MonoBehaviour
{
	private bool m_Triggered;

	public TimeCycle.WeatherType Weather;

	public int Hour;

	public float TweenTime;

	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!m_Triggered)
		{
			TimeCycle.HelperSetTimeAndWeather(Hour, Weather, TweenTime);
			m_Triggered = true;
		}
	}

	private void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
			ExtraGizmos.DrawCollider(boxCollider);
		}
	}
}
