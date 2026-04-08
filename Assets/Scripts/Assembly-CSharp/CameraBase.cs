using UnityEngine;

[RequireComponent(typeof(DebugWorldTracker))]
public abstract class CameraBase : MonoBehaviour
{
	public GameObject mTargetObj;

	public GameObject mLookAheadObj;

	public Transform mTargetTransform;

	private Camera mThisCamera;

	private bool mScreenShake;

	public float mScreenShakeAmount = 0.25f;

	private Vector3 mScreenShakeOffset;

	private float mScreenShakeDuration = 0.5f;

	private float mScreenShakeTime;

	private float mTempFerocity = -1f;

	private float mTempDuration = -1f;

	public float Fov = 55f;

	public float Roll;

	public Vector3 LookAheadOffset = new Vector3(0f, 0f, 0f);

	private Vector3 mPreviousOffset;

	public CameraTransitionData[] Transitions;

	public GameObject Target
	{
		get
		{
			return mTargetObj;
		}
		set
		{
			mTargetObj = value;
		}
	}

	public Transform TargetTransform
	{
		get
		{
			return mTargetTransform;
		}
		set
		{
			mTargetTransform = value;
		}
	}

	public GameObject LookAhead
	{
		get
		{
			return mLookAheadObj;
		}
		set
		{
			mLookAheadObj = value;
		}
	}

	public Vector3 PreviousOffset
	{
		get
		{
			return mPreviousOffset;
		}
		set
		{
			mPreviousOffset = value;
		}
	}

	public void StopScreenShake()
	{
		mScreenShakeOffset = Vector3.zero;
		mScreenShake = false;
		mTempDuration = -1f;
		mTempFerocity = -1f;
	}

	public virtual void Awake()
	{
		mScreenShakeOffset = Vector3.zero;
	}

	public CameraTransitionData GetTransitionData(CameraBase from)
	{
		CameraTransitionData[] transitions = Transitions;
		foreach (CameraTransitionData cameraTransitionData in transitions)
		{
			if (cameraTransitionData.CameraFrom == from)
			{
				return cameraTransitionData;
			}
		}
		return null;
	}

	public virtual void Reset()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
		if (!GameController.Instance.Paused() && mScreenShake)
		{
			float num = mScreenShakeAmount;
			float num2 = mScreenShakeDuration;
			Vector3 position = base.transform.position;
			if (mTempDuration != -1f && mTempFerocity != -1f)
			{
				num = mTempFerocity;
				num2 = mTempDuration;
			}
			mScreenShakeOffset = new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), 0f);
			position += mScreenShakeOffset;
			base.transform.position = position;
			mScreenShakeTime += Time.deltaTime;
			if (mScreenShakeTime > num2)
			{
				StopScreenShake();
			}
		}
	}

	public virtual void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "camera.png");
	}

	public void MoveUpdate(Vector3 toPos, float t)
	{
		base.gameObject.transform.position = toPos;
	}

	public void LookUpdate(Vector3 toAt, float t)
	{
		if (CameraManager.Instance.GetCurrentCamera() != this)
		{
			base.gameObject.transform.LookAt(toAt);
			return;
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
		base.gameObject.transform.LookAt(toAt);
		Vector3 eulerAngles2 = base.gameObject.transform.eulerAngles;
		base.gameObject.transform.eulerAngles = eulerAngles;
		t *= 10f * Time.deltaTime;
		zero2.x = Mathf.LerpAngle(eulerAngles.x, eulerAngles2.x, t);
		zero2.y = Mathf.LerpAngle(eulerAngles.y, eulerAngles2.y, t);
		zero2.z = Mathf.LerpAngle(eulerAngles.z, Roll, t);
		base.gameObject.transform.eulerAngles = zero2;
	}

	public void Shake()
	{
		mScreenShake = true;
		mScreenShakeTime = 0f;
	}

	public void Shake(float ferocity, float duration)
	{
		mScreenShake = true;
		mScreenShakeTime = 0f;
		mTempDuration = duration;
		mTempFerocity = ferocity;
	}
}
