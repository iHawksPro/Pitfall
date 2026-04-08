using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFogSetting : MonoBehaviour
{
	[SerializeField]
	private bool enableFog = true;

	private bool previousFogState;

	private void OnPreRender()
	{
		previousFogState = RenderSettings.fog;
		RenderSettings.fog = enableFog;
	}

	private void OnPostRender()
	{
		RenderSettings.fog = previousFogState;
	}
}
