using UnityEngine;

public class FXAutoDestroyScript : MonoBehaviour
{
	private ParticleSystem thisParticleSystem;

	public float Duration = 0.05f;

	private void Start()
	{
		thisParticleSystem = base.gameObject.GetComponent<ParticleSystem>();
		Object.Destroy(base.gameObject, Duration);
	}
}
