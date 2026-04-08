using UnityEngine;

[DisallowMultipleComponent]
public class ParticleEmitter : MonoBehaviour
{
	private ParticleSystem mParticleSystem;

	public bool emit
	{
		get
		{
			EnsureParticleSystem();
			if (mParticleSystem == null)
			{
				return false;
			}
			return mParticleSystem.emission.enabled;
		}
		set
		{
			EnsureParticleSystem();
			if (mParticleSystem == null)
			{
				return;
			}
			ParticleSystem.EmissionModule emission = mParticleSystem.emission;
			emission.enabled = value;
			if (value)
			{
				if (!mParticleSystem.isPlaying)
				{
					mParticleSystem.Play();
				}
			}
			else if (mParticleSystem.isPlaying)
			{
				mParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	private void Awake()
	{
		EnsureParticleSystem();
	}

	private void EnsureParticleSystem()
	{
		if (mParticleSystem == null)
		{
			mParticleSystem = GetComponent<ParticleSystem>();
		}
	}
}
