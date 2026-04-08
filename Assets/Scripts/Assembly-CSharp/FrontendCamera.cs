using System;
using System.Collections;
using UnityEngine;

public class FrontendCamera : MonoBehaviour
{
	public Camera m_camera;

	public GameObject m_introAnimCameraRoot;

	public Transform m_introAnimCameraTransform;

	private float m_maxCamShakeAngle = 10f;

	private float m_camShakeAmount;

	private Transform m_trackObj;

	private Vector3 m_trackOffset = new Vector3(0f, 0f, 0f);

	public void PlayIntroAnim(Action onFinished)
	{
		float length = m_introAnimCameraRoot.GetComponent<Animation>().GetClip("In").length;
		m_introAnimCameraRoot.GetComponent<Animation>().Play("In");
		StartCoroutine(OnIntroAnimFinished(length, onFinished));
	}

	private IEnumerator OnIntroAnimFinished(float animLength, Action onFinished)
	{
		while (animLength > 0f)
		{
			m_camera.transform.position = m_introAnimCameraTransform.position;
			m_camera.transform.rotation = m_introAnimCameraTransform.rotation;
			animLength -= Time.deltaTime;
			yield return null;
		}
		if (onFinished != null)
		{
			onFinished();
		}
		PlayIdle();
	}

	public void PlayIdle()
	{
		ResetCamera();
		m_introAnimCameraRoot.GetComponent<Animation>().Play("Idle");
	}

	public float TitleToGameAnimLength()
	{
		return m_introAnimCameraRoot.GetComponent<Animation>().GetClip("Out").length;
	}

	public void TitleToGame()
	{
		float animLength = TitleToGameAnimLength();
		m_introAnimCameraRoot.GetComponent<Animation>().Play("Out");
		StartCoroutine(TitleToGameCameraTrack(animLength, null));
	}

	private IEnumerator TitleToGameCameraTrack(float animLength, Action onFinished)
	{
		while (animLength > 0f)
		{
			m_camera.transform.position = m_introAnimCameraTransform.position;
			m_camera.transform.rotation = m_introAnimCameraTransform.rotation;
			animLength -= Time.deltaTime;
			yield return null;
		}
		if (onFinished != null)
		{
			onFinished();
		}
	}

	public void SetFocus(UIMenuCameraParameters targetObj, float transitionTime)
	{
		m_trackObj = null;
		if (transitionTime != 0f)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("position", targetObj.transform, "time", transitionTime, "easetype", iTween.EaseType.easeInOutCubic));
			m_camera.fieldOfView = targetObj.m_fov;
		}
		else
		{
			base.gameObject.transform.position = targetObj.transform.position;
			base.gameObject.transform.rotation = targetObj.transform.rotation;
		}
	}

	public void ResetCamera()
	{
		m_camera.transform.localPosition = Vector3.zero;
		m_camera.transform.localRotation = Quaternion.identity;
		m_camera.transform.localScale = Vector3.one;
		m_camShakeAmount = 0f;
	}

	public void SetCameraShake(float Amount)
	{
		m_camShakeAmount = Amount;
	}

	public void TrackObject(Transform trackObj)
	{
		m_trackOffset = base.gameObject.transform.position - trackObj.position;
		m_trackObj = trackObj;
	}

	private void LateUpdate()
	{
		if (m_camShakeAmount > 0f)
		{
			float x = UnityEngine.Random.Range(0f - m_maxCamShakeAngle, m_maxCamShakeAngle) * m_camShakeAmount;
			float y = UnityEngine.Random.Range(0f - m_maxCamShakeAngle, m_maxCamShakeAngle) * m_camShakeAmount;
			m_camera.transform.localEulerAngles = new Vector3(x, y, 0f);
		}
		if (m_trackObj != null)
		{
			Vector3 position = m_trackObj.position;
			position += m_trackOffset;
			base.gameObject.transform.position = position;
		}
	}
}
