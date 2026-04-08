using UnityEngine;

public class ButtonLightRay : MonoBehaviour
{
	public float m_rotateSpeed = 20f;

	private Vector3 m_rotation;

	private void Start()
	{
		m_rotation = new Vector3(0f, 0f, 0f);
	}

	private void Update()
	{
		m_rotation.z += m_rotateSpeed * Time.deltaTime;
		base.transform.localEulerAngles = m_rotation;
	}
}
