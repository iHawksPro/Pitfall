using System.Collections.Generic;
using UnityEngine;

public class SceneQueue : MonoBehaviour
{
	private bool m_isLoading;

	private List<string> m_scenesToLoad = new List<string>();

	private static SceneQueue m_instance;

	public static SceneQueue Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsLoading
	{
		get
		{
			return m_isLoading;
		}
	}

	private void Awake()
	{
		if (m_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		Object.DontDestroyOnLoad(this);
	}

	public void LoadScene(string Scene)
	{
		string text = SceneNameResolver.Resolve(Scene);
		if (m_scenesToLoad.Count == 0)
		{
			Application.LoadLevelAdditiveAsync(text);
			m_isLoading = true;
		}
		else
		{
			m_scenesToLoad.Add(text);
		}
	}

	public void SceneLoaded()
	{
		if (m_scenesToLoad.Count > 0)
		{
			Application.LoadLevelAdditiveAsync(m_scenesToLoad[0]);
			m_scenesToLoad.RemoveAt(0);
			m_isLoading = true;
		}
		else
		{
			m_isLoading = false;
		}
	}
}
