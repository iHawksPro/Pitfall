using UnityEngine;

public class RainMaker : MonoBehaviour
{
	public GameObject TrackThis;

	public ParticleEmitter HeavyRainEmitter;

	public ParticleEmitter MediumRainEmitter;

	public ParticleEmitter LightRainEmitter;

	private float m_RainAmount;

	private void Start()
	{
		m_RainAmount = 0f;
	}

	public void Rain(float amount)
	{
		m_RainAmount = amount;
	}

	private void Update()
	{
		Vector3 position = TrackThis.transform.position;
		position.y += 6f;
		position.z += 8f;
		base.transform.position = position;
		if (m_RainAmount > 0f)
		{
			HeavyRainEmitter.emit = m_RainAmount > 0.9f;
			MediumRainEmitter.emit = m_RainAmount > 0.4f;
			LightRainEmitter.emit = true;
		}
		else
		{
			HeavyRainEmitter.emit = false;
			MediumRainEmitter.emit = false;
			LightRainEmitter.emit = false;
		}
	}
}
