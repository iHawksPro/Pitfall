using UnityEngine;

public class DisableMacawButton : MonoBehaviour
{
	public GameObject m_Ray;

	public GameObject m_RayBack;

	public GameObject m_SparkleOverlay;

	public GameObject m_NumHeld;

	public SpriteText m_NumHeldText;

	private float m_Rot;

	private float m_Rot2;

	private bool m_NumVisible;

	private bool m_ButtonActive;

	private void OnEnable()
	{
		int numMacawsHeld = SecureStorage.Instance.NumMacawsHeld;
		UIButton component = base.transform.GetComponent<UIButton>();
		if (!IsMacawAvailable() || !SecureStorage.Instance.HasMacaws)
		{
			m_ButtonActive = false;
			m_NumVisible = false;
		}
		else
		{
			m_ButtonActive = true;
			m_NumVisible = numMacawsHeld > 0;
		}
		UIButton componentInChildren = base.gameObject.GetComponentInChildren<UIButton>();
		componentInChildren.gameObject.SetActiveRecursively(true);
		componentInChildren.controlIsEnabled = m_ButtonActive;
		m_NumHeldText.Text = numMacawsHeld.ToString();
		m_Rot = 0f;
		m_Rot2 = 0f;
	}

	private bool IsMacawAvailable()
	{
		return CheckPointController.Instance().HasPlayerPassedValidCheckpoint();
	}

	public void Update()
	{
		if (m_ButtonActive)
		{
			m_Rot += Time.deltaTime * 30f;
			m_Rot2 += Time.deltaTime * 20f;
			m_Ray.transform.localEulerAngles = new Vector3(0f, 0f, m_Rot);
			m_RayBack.transform.localEulerAngles = new Vector3(0f, 0f, 0f - m_Rot2);
		}
		else
		{
			m_Ray.SetActiveRecursively(false);
			m_RayBack.SetActiveRecursively(false);
			m_SparkleOverlay.SetActiveRecursively(false);
		}
	}

	public void LateUpdate()
	{
		if (m_NumVisible != m_NumHeld.active)
		{
			m_NumHeld.SetActiveRecursively(m_NumVisible);
		}
	}
}
