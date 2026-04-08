public class FrontendCam : CameraBase
{
	public void SetFocus(UIMenuCameraParameters targetObj, float transitionTime)
	{
		if (transitionTime != 0f)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("position", targetObj.transform, "time", transitionTime, "easetype", iTween.EaseType.easeInOutCubic));
			Fov = targetObj.m_fov;
		}
		else
		{
			base.gameObject.transform.position = targetObj.transform.position;
			base.gameObject.transform.rotation = targetObj.transform.rotation;
		}
	}
}
