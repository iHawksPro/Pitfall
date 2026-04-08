using System.Collections;

public class GameStateController : StateController
{
	protected override void OnStateActivate(string OldStateName)
	{
		StartCoroutine(DelayedStateActivate(OldStateName));
	}

	private IEnumerator DelayedStateActivate(string OldStateName)
	{
		while (GameController.Instance == null)
		{
			yield return null;
		}
		while (GameController.Instance.IsLoading)
		{
			yield return null;
		}
		base.OnStateActivate(OldStateName);
		UIMenuBackground.Instance.Hide();
	}
}
