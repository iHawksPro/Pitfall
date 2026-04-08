using UnityEngine;

public class StateRoot : MonoBehaviour
{
	public string stateName = "invalid";

	public bool canGoBack = true;

	public string backStateName = string.Empty;

	public bool storeStateinBackHistory = true;

	private void Awake()
	{
		SceneQueue.Instance.SceneLoaded();
		StateManager.Instance.StateLoaded(this);
		base.gameObject.name = "StateRoot-" + stateName;
	}
}
