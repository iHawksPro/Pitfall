using UnityEngine;

public class ScreenshotUtility : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
