using UnityEngine;

public class GUISystem : MonoBehaviour
{
	private static GUISystem m_instance;

	public Camera m_guiCamera;

	public UIManager m_uiManager;

	public static GUISystem Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = Object.FindObjectOfType(typeof(GUISystem)) as GUISystem;
			}
			if (m_instance == null)
			{
				Debug.LogError("Unable to locate the GUI System. Please ensure there is one in the scene");
			}
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance != null)
		{
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		m_instance = this;
		if (m_guiCamera == null)
		{
			m_guiCamera = base.gameObject.GetComponentInChildren<Camera>();
		}
		if (m_uiManager == null)
		{
			m_uiManager = base.gameObject.GetComponentInChildren<UIManager>();
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}
}
