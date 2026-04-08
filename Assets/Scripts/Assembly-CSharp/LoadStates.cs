using UnityEngine;

public class LoadStates : MonoBehaviour
{
	public string[] m_loadedStates;

	public StateRoot m_myState;

	public bool m_loadDefaults = true;

	private string[] statesToLoad
	{
		get
		{
			if (m_loadDefaults && m_loadedStates.Length == 0)
			{
				m_loadedStates = DefaultStates;
				m_loadDefaults = false;
			}
			return m_loadedStates;
		}
	}

	public static string[] DefaultStates
	{
		get
		{
			return new string[8] { "Title", "Game", "Results", "Pause", "BaseCamp", "FieldGuide", "Options", "Challenges" };
		}
	}

	private void OnStateActivate(string OldStateName)
	{
		string[] loadedStates = StateManager.Instance.LoadedStates;
		string[] array = loadedStates;
		foreach (string text in array)
		{
			if (!text.Equals(m_myState.stateName) && !IsStateReferenced(text))
			{
				StateManager.Instance.UnloadState(text);
			}
		}
		string[] array2 = statesToLoad;
		foreach (string stateName in array2)
		{
			if (!StateManager.Instance.Exists(stateName) && !StateManager.Instance.IsLoadingState(stateName))
			{
				StateManager.Instance.LoadFromSaveState(stateName);
			}
		}
	}

	private bool IsStateReferenced(string stateName)
	{
		string[] array = statesToLoad;
		foreach (string text in array)
		{
			if (text.Equals(stateName))
			{
				return true;
			}
		}
		return false;
	}
}
