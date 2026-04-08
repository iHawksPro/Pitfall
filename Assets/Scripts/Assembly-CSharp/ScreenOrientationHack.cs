using System.Collections;
using UnityEngine;

public class ScreenOrientationHack : MonoBehaviour
{
	private IEnumerator Start()
	{
		switch (Input.deviceOrientation)
		{
		case DeviceOrientation.Portrait:
		case DeviceOrientation.PortraitUpsideDown:
		case DeviceOrientation.FaceUp:
		case DeviceOrientation.FaceDown:
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			yield return new WaitForSeconds(0.5f);
			Screen.orientation = ScreenOrientation.AutoRotation;
			break;
		}
		yield return null;
	}
}
