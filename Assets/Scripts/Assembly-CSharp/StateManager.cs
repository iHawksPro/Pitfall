using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
	private Dictionary<string, StateRoot> m_states = new Dictionary<string, StateRoot>();

	private List<string> m_loadingStates = new List<string>();

	private StateRoot m_currentState;

	private string m_nextState = string.Empty;

	private string m_lastStateName = string.Empty;

	private string m_backStateName = string.Empty;

	private bool m_isLocked;

	public bool m_autoActivate = true;

	private static StateManager m_instance;

	public static StateManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	public StateRoot CurrentState
	{
		get
		{
			return m_currentState;
		}
	}

	public string CurrentStateName
	{
		get
		{
			return m_currentState.stateName;
		}
	}

	public int nStates
	{
		get
		{
			return m_states.Count;
		}
	}

	public string[] LoadedStates
	{
		get
		{
			Dictionary<string, StateRoot>.KeyCollection keys = m_states.Keys;
			string[] array = new string[keys.Count];
			keys.CopyTo(array, 0);
			return array;
		}
	}

	public static event Action<string> stateDeactivated;

	public static event Action<string> stateActivated;

	private void Awake()
	{
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		m_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public bool IsLoading()
	{
		return m_loadingStates.Count > 0;
	}

	public void Start()
	{
		StartCoroutine(SwitchToNextState());
	}

	public bool Exists(string Name)
	{
		return m_states.ContainsKey(Name);
	}

	public bool CanUpdateFriends()
	{
		if (CurrentState.stateName == "Game" || CurrentState.stateName == "BaseCamp" || CurrentState.stateName == "Pause")
		{
			return false;
		}
		return true;
	}

	public void StateLoaded(StateRoot State)
	{
		m_loadingStates.Remove(State.stateName);
		if (m_states.ContainsKey(State.stateName))
		{
			Debug.LogError("State " + State.stateName + " already exists");
			return;
		}
		m_states[State.stateName] = State;
		State.BroadcastMessage("OnStateLoaded", SendMessageOptions.DontRequireReceiver);
		if (m_autoActivate && m_currentState == null)
		{
			m_currentState = State;
			m_currentState.gameObject.BroadcastMessage("OnStateActivate", string.Empty, SendMessageOptions.DontRequireReceiver);
		}
	}

	private IEnumerator SetCurrentStateCoroutine(StateRoot newState)
	{
		if (m_currentState != newState && !m_isLocked)
		{
			if (m_currentState != null)
			{
				string stateName = ((!(newState != null)) ? string.Empty : newState.stateName);
				m_currentState.gameObject.BroadcastMessage("OnStateDeactivate", stateName, SendMessageOptions.DontRequireReceiver);
				OnStateDeactivated(stateName);
			}
			string oldStateName = (m_lastStateName = ((!(m_currentState != null)) ? string.Empty : m_currentState.stateName));
			if (m_currentState.storeStateinBackHistory)
			{
				m_backStateName = m_lastStateName;
			}
			float animateOutTime = 0.5f;
			StateController currentController = m_currentState.gameObject.GetComponent<StateController>();
			if (currentController != null)
			{
				animateOutTime = currentController.animateOutTime;
			}
			if (animateOutTime != 0f)
			{
				yield return new WaitForSeconds(animateOutTime);
			}
			m_currentState = newState;
			if (m_currentState != null)
			{
				m_currentState.gameObject.BroadcastMessage("OnStateActivate", oldStateName, SendMessageOptions.DontRequireReceiver);
				OnStateActivated(m_currentState.stateName);
			}
		}
	}

	private IEnumerator SwitchToNextState()
	{
		while (true)
		{
			if (m_nextState.Length > 0 && Exists(m_nextState))
			{
				SetCurrentState(m_nextState);
				m_nextState = string.Empty;
			}
			yield return 0;
		}
	}

	public StateRoot GetState(string stateName)
	{
		return m_states[stateName];
	}

	public void SetCurrentState(string StateName)
	{
		StateRoot stateRoot = m_states[StateName];
		if (stateRoot != null)
		{
			StartCoroutine("SetCurrentStateCoroutine", stateRoot);
		}
	}

	private void DoUnloadAndDestroyState(StateRoot State)
	{
		State.gameObject.BroadcastMessage("OnStateUnloaded", SendMessageOptions.DontRequireReceiver);
		UnityEngine.Object.Destroy(State.gameObject);
	}

	public void UnloadState(StateRoot State)
	{
		if (State != m_currentState)
		{
			DoUnloadAndDestroyState(State);
			m_states.Remove(State.stateName);
		}
	}

	public void UnloadState(string StateName)
	{
		StateRoot stateRoot = m_states[StateName];
		if (stateRoot != null)
		{
			UnloadState(stateRoot);
		}
	}

	public void UnloadAll()
	{
		foreach (StateRoot value in m_states.Values)
		{
			DoUnloadAndDestroyState(value);
		}
		m_states.Clear();
	}

	public bool IsLoadingState(string StateName)
	{
		return m_loadingStates.Contains(StateName);
	}

	public void LoadFromSaveState(string StateName)
	{
		m_loadingStates.Add(StateName);
		SceneQueue.Instance.LoadScene(StateName);
	}

	public void LoadAndActivateState(string StateName)
	{
		UIManager.instance.blockInput = true;
		TBFUtils.DebugLog("Turning OFF input because of state change to " + StateName);
		if (m_nextState.Length > 0)
		{
			TBFUtils.DebugLog(m_nextState + " is already waiting to be loaded");
		}
		if (!Exists(StateName) && !m_loadingStates.Contains(StateName))
		{
			LoadFromSaveState(StateName);
		}
		m_nextState = StateName;
	}

	public bool ResultsFromGame()
	{
		if (m_currentState.stateName == "Results" && m_lastStateName == "Game")
		{
			return true;
		}
		return false;
	}

	public void LoadAndActivatePreviousState()
	{
		if (!m_currentState.canGoBack)
		{
			return;
		}
		if (m_currentState.backStateName == string.Empty)
		{
			if (m_backStateName != string.Empty)
			{
				LoadAndActivateState(m_backStateName);
				if (m_backStateName.Equals("Title"))
				{
					Bedrock.AnalyticsLogEvent("UI.Title.StoreButtonBackTouched");
					DailyDoubleController.Instance.BottomTicker.transform.localPosition = DailyDoubleController.Instance.BottomTickerOrigPos;
					DailyDoubleController.Instance.BottomTicker.gameObject.SetActiveRecursively(false);
				}
			}
		}
		else
		{
			LoadAndActivateState(m_currentState.backStateName);
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && DialogManager.Instance != null && !DialogManager.DialogActive && m_currentState != null)
		{
			if (m_currentState.stateName == "Game")
			{
				GameController.Instance.GameHUD.OnHudPause();
			}
			else if (m_currentState.stateName == "Pause")
			{
				m_currentState.gameObject.BroadcastMessage("OnResumePressed", SendMessageOptions.DontRequireReceiver);
			}
			else if (m_currentState.stateName != "Title")
			{
				m_currentState.gameObject.BroadcastMessage("OnBackPressed", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnStateDeactivated(string StateName)
	{
		if (StateManager.stateDeactivated != null)
		{
			StateManager.stateDeactivated(StateName);
		}
	}

	private void OnStateActivated(string NewStateName)
	{
		if (StateManager.stateActivated != null)
		{
			StateManager.stateActivated(NewStateName);
		}
	}
}
