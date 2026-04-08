using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public Camera MainCamera;

	public TextMesh DebugText;

	public CameraBase ShoulderCam;

	public CameraBase HeliLeftCam;

	public CameraBase HeliRightCam;

	public CameraBase MountainSlideCam;

	public CameraBase MineCartCam;

	public CameraBase BikeCam;

	public CameraBase CineCam1;

	public CameraBase BikeCineCam1;

	public CameraBase SideCam;

	public CameraBase GordonsCam;

	public CameraBase CineCam2;

	public CameraBase CheckpointCam;

	public CameraBase BikeHeliCamLeft;

	public CameraBase MacawCam;

	public CameraBase JaguarCam;

	public CameraBase DeathShakeCam;

	public CameraBase PitDeathCam;

	public CameraBase CollisionDeathCam;

	public CameraBase BaddieDeathCam;

	public CameraBase BikeDeathCam;

	public CameraBase MineCartDeathCam;

	public CameraBase FrontendCam;

	public CameraBase DefaultCam;

	public TweenFunctions.TweenType DefaultTransitionType = TweenFunctions.TweenType.easeInOutCubic;

	public float DefaultTransitionTime = 0.5f;

	private float mBlend;

	private float mBlendAmt;

	private float mQueuedBlend;

	private float mQueuedBlendAmt;

	private Quaternion tempRotation;

	private Vector3 tempPosition;

	private float tempFOV;

	private CameraTransitionData mTransitionData;

	private CameraTransitionData mQueuedTransitionData;

	public static CameraManager mInstance;

	private CameraBase mCameraMode;

	private CameraBase mCameraModeRestore;

	private CameraBase mCameraModeResurrect;

	private CameraBase mNextCamera;

	private CameraBase mQueuedCamera;

	private bool mRestoreCamFromPowerup;

	private bool mCurrentlyUsingPowerupCam;

	public float MinCameraTransTime = 1f;

	public static CameraManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	public bool CurrentlyUsingPowerupCam
	{
		get
		{
			return mCurrentlyUsingPowerupCam;
		}
	}

	public CameraBase GetRestoreCamera()
	{
		return mCameraModeRestore;
	}

	public CameraBase GetNextCamera()
	{
		return mNextCamera;
	}

	public CameraBase GetQueuedCamera()
	{
		return mQueuedCamera;
	}

	private void Awake()
	{
		if (DefaultCam == null)
		{
			DefaultCam = GordonsCam;
		}
		mInstance = this;
		mCameraMode = null;
		mNextCamera = DefaultCam;
		mRestoreCamFromPowerup = false;
	}

	private void Start()
	{
		SwitchToCameraMode(DefaultCam, CameraTransitionData.JumpCut);
	}

	public void ResetCameras()
	{
		SetGameCamsActive(true);
		GordonsCam.Reset();
		ShoulderCam.Reset();
		HeliLeftCam.Reset();
		HeliRightCam.Reset();
		MountainSlideCam.Reset();
		MineCartCam.Reset();
		BikeCam.Reset();
		CineCam1.Reset();
		BikeCineCam1.Reset();
		SideCam.Reset();
		GordonsCam.Reset();
		CineCam2.Reset();
		CheckpointCam.Reset();
		BikeHeliCamLeft.Reset();
		MacawCam.Reset();
		JaguarCam.Reset();
		DeathShakeCam.Reset();
		PitDeathCam.Reset();
		CollisionDeathCam.Reset();
		BaddieDeathCam.Reset();
		BikeDeathCam.Reset();
		MineCartDeathCam.Reset();
		FrontendCam.Reset();
		DefaultCam.Reset();
		mCameraMode = DefaultCam;
		mCameraModeRestore = null;
		mCameraModeResurrect = null;
		mNextCamera = null;
		mQueuedCamera = null;
	}

	public void ResetCameraTutorial()
	{
		SetGameCamsActive(true);
		GordonsCam.Reset();
		ShoulderCam.Reset();
		HeliLeftCam.Reset();
		HeliRightCam.Reset();
		MountainSlideCam.Reset();
		MineCartCam.Reset();
		BikeCam.Reset();
		CineCam1.Reset();
		BikeCineCam1.Reset();
		SideCam.Reset();
		GordonsCam.Reset();
		CineCam2.Reset();
		CheckpointCam.Reset();
		BikeHeliCamLeft.Reset();
		MacawCam.Reset();
		JaguarCam.Reset();
		DeathShakeCam.Reset();
		PitDeathCam.Reset();
		CollisionDeathCam.Reset();
		BaddieDeathCam.Reset();
		BikeDeathCam.Reset();
		MineCartDeathCam.Reset();
		FrontendCam.Reset();
		DefaultCam.Reset();
		mCameraMode = mCameraModeResurrect;
		mNextCamera = null;
		mQueuedCamera = null;
		mCameraModeRestore = null;
	}

	private bool IsDeathCam(CameraBase thisCamera)
	{
		return thisCamera == DeathShakeCam || thisCamera == PitDeathCam || thisCamera == CollisionDeathCam || thisCamera == BaddieDeathCam || thisCamera == BikeDeathCam || thisCamera == MineCartDeathCam;
	}

	public bool IsValidRessurectionCamera(CameraBase thisCamera)
	{
		return !IsDeathCam(thisCamera) && !IsPowerupCamera(thisCamera);
	}

	private void DebugSwitchCamera(KeyCode key, CameraBase cam)
	{
		if (Input.GetKeyDown(key))
		{
			if (IsPowerupCamera(mCameraMode))
			{
				mCameraMode = cam;
			}
			CameraTransitionData camTrans = new CameraTransitionData(cam, TweenFunctions.TweenType.easeInOutCubic, 1f);
			SwitchToCameraMode(cam, camTrans);
		}
	}

	private void Update()
	{
	}

	private void RePosCam(CameraBase cam, Vector3 offset)
	{
		if (!(cam == null))
		{
			Vector3 position = cam.transform.position;
			position -= offset;
			cam.transform.position = position;
		}
	}

	public void RepositionCameraWithWorldRecentre(Vector3 offset)
	{
		Vector3 position = MainCamera.transform.position;
		position -= offset;
		MainCamera.transform.position = position;
		RePosCam(HeliLeftCam, offset);
		RePosCam(HeliRightCam, offset);
		RePosCam(MountainSlideCam, offset);
		RePosCam(MineCartCam, offset);
		RePosCam(BikeCam, offset);
		RePosCam(CineCam1, offset);
		RePosCam(BikeCineCam1, offset);
		RePosCam(SideCam, offset);
		RePosCam(GordonsCam, offset);
		RePosCam(MacawCam, offset);
		RePosCam(JaguarCam, offset);
		RePosCam(DeathShakeCam, offset);
	}

	private bool IsPowerupCamera(CameraBase camMode)
	{
		return camMode == JaguarCam;
	}

	public void RestoreCameraFromPowerUp()
	{
		mRestoreCamFromPowerup = true;
		SwitchToCameraMode(mCameraModeRestore);
	}

	public void SetResurrectionCamera(CameraBase cameraModeResurrect)
	{
		if (cameraModeResurrect != null)
		{
			mCameraModeResurrect = cameraModeResurrect;
		}
	}

	private float easeInOutCubic(float start, float end, float value)
	{
		value /= 0.5f;
		end -= start;
		if (value < 1f)
		{
			return end / 2f * value * value * value + start;
		}
		value -= 2f;
		return end / 2f * (value * value * value + 2f) + start;
	}

	public void UpdateFromSingleSource(CameraBase cam)
	{
		MainCamera.gameObject.transform.position = cam.transform.position;
		MainCamera.gameObject.transform.rotation = cam.transform.rotation;
		MainCamera.fieldOfView = cam.Fov;
	}

	public void LateUpdate()
	{
		if (mNextCamera != null)
		{
			if (mCameraMode == null)
			{
				mCameraMode = mNextCamera;
				mNextCamera = mQueuedCamera;
				mQueuedCamera = null;
				mTransitionData = mQueuedTransitionData;
				mQueuedTransitionData = null;
			}
			if (!(mNextCamera != null))
			{
				return;
			}
			if ((bool)mQueuedCamera && (mQueuedTransitionData.CameraFrom == null || mQueuedTransitionData.Duration == 0f))
			{
				mTransitionData = mQueuedTransitionData;
				mNextCamera = mQueuedCamera;
				mQueuedCamera = null;
				mQueuedTransitionData = null;
				return;
			}
			if (mTransitionData.CameraFrom == null || mTransitionData.Duration == 0f)
			{
				mBlend = 1f;
				mCameraMode = mNextCamera;
				mNextCamera = null;
				UpdateFromSingleSource(mCameraMode);
				return;
			}
			if (mQueuedCamera != null && mQueuedTransitionData != null)
			{
				mQueuedBlend = TweenFunctions.tween(mQueuedTransitionData.TweenType, 0f, 1f, mQueuedBlendAmt);
				mQueuedBlendAmt += Time.deltaTime * (1f / mQueuedTransitionData.Duration);
				mQueuedBlendAmt = Mathf.Clamp01(mQueuedBlendAmt);
				mQueuedBlend = Mathf.Clamp01(mQueuedBlend);
			}
			else
			{
				mBlend = TweenFunctions.tween(mTransitionData.TweenType, 0f, 1f, mBlendAmt);
				mBlendAmt += Time.deltaTime * (1f / mTransitionData.Duration);
				mBlendAmt = Mathf.Clamp01(mBlendAmt);
				mBlend = Mathf.Clamp01(mBlend);
				MainCamera.gameObject.transform.position = Vector3.Slerp(mCameraMode.transform.position, mNextCamera.transform.position, mBlend);
				MainCamera.gameObject.transform.rotation = Quaternion.Slerp(mCameraMode.transform.rotation, mNextCamera.transform.rotation, mBlend);
				MainCamera.fieldOfView = Mathf.Lerp(mCameraMode.Fov, mNextCamera.Fov, mBlend);
				tempPosition = MainCamera.gameObject.transform.position;
				tempRotation = MainCamera.gameObject.transform.rotation;
				tempFOV = MainCamera.fieldOfView;
			}
			if (mQueuedCamera != null && mQueuedTransitionData != null)
			{
				MainCamera.gameObject.transform.position = Vector3.Slerp(tempPosition, mQueuedCamera.transform.position, mQueuedBlend);
				MainCamera.gameObject.transform.rotation = Quaternion.Slerp(tempRotation, mQueuedCamera.transform.rotation, mQueuedBlend);
				MainCamera.fieldOfView = Mathf.Lerp(tempFOV, mQueuedCamera.Fov, mQueuedBlend);
				tempPosition = Vector3.Slerp(mCameraMode.transform.position, mNextCamera.transform.position, mBlend);
				tempRotation = Quaternion.Slerp(mCameraMode.transform.rotation, mNextCamera.transform.rotation, mBlend);
				tempFOV = Mathf.Lerp(mCameraMode.Fov, mNextCamera.Fov, mBlend);
			}
			if (mBlend == 1f)
			{
				mCameraMode = mNextCamera;
				mNextCamera = mQueuedCamera;
				mTransitionData = mQueuedTransitionData;
				mQueuedCamera = null;
				mQueuedTransitionData = null;
				tempPosition = Vector3.zero;
				tempRotation = Quaternion.identity;
				tempFOV = 0f;
				if (mNextCamera != null)
				{
					mBlend = 0f;
					mBlendAmt = 0f;
				}
			}
			if (mQueuedBlend == 1f)
			{
				mCameraMode = mQueuedCamera;
				mNextCamera = null;
				mTransitionData = null;
				mQueuedCamera = null;
				mQueuedTransitionData = null;
				mBlend = 0f;
				mBlendAmt = 0f;
				mQueuedBlend = 0f;
				mQueuedBlendAmt = 0f;
				tempPosition = Vector3.zero;
				tempRotation = Quaternion.identity;
				tempFOV = 0f;
			}
		}
		else if ((bool)mCameraMode)
		{
			UpdateFromSingleSource(mCameraMode);
		}
	}

	public CameraBase GetCurrentCamera()
	{
		return mCameraMode;
	}

	public void SwitchToCameraMode(CameraBase nextMode)
	{
		if (mCameraMode != null)
		{
			CameraBase cameraBase = mNextCamera ?? mCameraMode;
			CameraTransitionData transitionData = nextMode.GetTransitionData(cameraBase);
			if (transitionData != null)
			{
				float duration = ModifyDuration(transitionData.Duration);
				transitionData.Duration = duration;
				SwitchToCameraMode(nextMode, transitionData);
				return;
			}
		}
		CameraTransitionData camTrans = new CameraTransitionData(mCameraMode, DefaultTransitionType, DefaultTransitionTime);
		SwitchToCameraMode(nextMode, camTrans);
	}

	private float ModifyDuration(float currentDuration)
	{
		if (currentDuration > MinCameraTransTime)
		{
			PlayerController playerController = PlayerController.Instance();
			float currentSpeed = playerController.GetCurrentSpeed();
			float num = 18f;
			float maxSpeed = playerController.Settings.MaxSpeed;
			float a = 1f - (currentSpeed - num) / (maxSpeed - num);
			a = Mathf.Min(a, 1f);
			currentDuration *= a;
			if (currentDuration < MinCameraTransTime)
			{
				currentDuration = MinCameraTransTime;
			}
		}
		return currentDuration;
	}

	private void SetGameCamsActive(bool bActive)
	{
		GordonsCam.gameObject.active = bActive;
		ShoulderCam.gameObject.active = bActive;
		HeliLeftCam.gameObject.active = bActive;
		HeliRightCam.gameObject.active = bActive;
		MountainSlideCam.gameObject.active = bActive;
		MineCartCam.gameObject.active = bActive;
		BikeCam.gameObject.active = bActive;
		CineCam1.gameObject.active = bActive;
		BikeCineCam1.gameObject.active = bActive;
		SideCam.gameObject.active = bActive;
		CineCam2.gameObject.active = bActive;
		CheckpointCam.gameObject.active = bActive;
		BikeHeliCamLeft.gameObject.active = bActive;
		MacawCam.gameObject.active = bActive;
		JaguarCam.gameObject.active = bActive;
		DeathShakeCam.gameObject.active = bActive;
		PitDeathCam.gameObject.active = bActive;
		CollisionDeathCam.gameObject.active = bActive;
		BaddieDeathCam.gameObject.active = bActive;
		BikeDeathCam.gameObject.active = bActive;
		MineCartDeathCam.gameObject.active = bActive;
	}

	public void SwitchToCameraMode(CameraBase nextMode, CameraTransitionData camTrans)
	{
		TBFAssert.DoAssert(nextMode != null, "null camera", string.Empty);
		if (IsValidRessurectionCamera(nextMode))
		{
			SetResurrectionCamera(nextMode);
		}
		if (nextMode == FrontendCam)
		{
			SetGameCamsActive(false);
		}
		if (mRestoreCamFromPowerup)
		{
			mNextCamera = null;
			nextMode = mCameraModeRestore;
			mCameraModeRestore = null;
			mCurrentlyUsingPowerupCam = false;
		}
		else
		{
			PieceDescriptor currentPiece = LevelGenerator.Instance().GetCurrentPiece();
			if (currentPiece != null && currentPiece.Theme == WorldConstructionHelper.Theme.Mountain)
			{
				mCurrentlyUsingPowerupCam = false;
			}
			else if (IsPowerupCamera(mCameraMode) && !mCurrentlyUsingPowerupCam)
			{
				mNextCamera = null;
				mQueuedCamera = mCameraModeRestore;
				mQueuedTransitionData = camTrans;
				mCameraModeRestore = null;
				mCurrentlyUsingPowerupCam = false;
			}
			else if (IsPowerupCamera(nextMode) && !IsPowerupCamera(mCameraMode))
			{
				if (mNextCamera != null)
				{
					mCameraModeRestore = mNextCamera;
				}
				else
				{
					mCameraModeRestore = mCameraMode;
				}
				mCurrentlyUsingPowerupCam = true;
			}
			else
			{
				if (IsPowerupCamera(mCameraMode) && !IsPowerupCamera(nextMode))
				{
					mCameraModeRestore = nextMode;
					mCurrentlyUsingPowerupCam = true;
					return;
				}
				if ((IsPowerupCamera(mNextCamera) || IsPowerupCamera(mQueuedCamera)) && !IsPowerupCamera(nextMode))
				{
					mCameraModeRestore = nextMode;
					mCurrentlyUsingPowerupCam = true;
					return;
				}
			}
		}
		mRestoreCamFromPowerup = false;
		if (mNextCamera == null)
		{
			mBlend = 0f;
			mBlendAmt = 0f;
			mNextCamera = nextMode;
			mTransitionData = camTrans;
		}
		else
		{
			mQueuedCamera = nextMode;
			mQueuedTransitionData = camTrans;
		}
	}

	public void RestoreFromResurrection()
	{
		if (mCameraModeResurrect != null)
		{
			SwitchToCameraMode(mCameraModeResurrect, new CameraTransitionData(mCameraModeResurrect, TweenFunctions.TweenType.spring, 0f));
		}
	}

	public void SwitchToDeathCamera(PieceDescriptor.KillType type, PlayerTheme.ThemeType ThemeType)
	{
		switch (ThemeType)
		{
		case PlayerTheme.ThemeType.Run:
			switch (type)
			{
			case PieceDescriptor.KillType.Pit:
			{
				if (mCameraMode != null)
				{
					PitDeathCam.transform.position = mCameraMode.transform.position;
				}
				PitDeathCam component7 = PitDeathCam.GetComponent<PitDeathCam>();
				if (component7 != null)
				{
					component7.Trigger();
				}
				SwitchToCameraMode(PitDeathCam);
				break;
			}
			case PieceDescriptor.KillType.Wall:
			{
				if (mCameraMode != null)
				{
					CollisionDeathCam.transform.position = mCameraMode.transform.position;
				}
				CollisionDeathCam component6 = CollisionDeathCam.GetComponent<CollisionDeathCam>();
				if (component6 != null)
				{
					component6.Trigger();
				}
				SwitchToCameraMode(CollisionDeathCam, CameraTransitionData.JumpCut);
				break;
			}
			case PieceDescriptor.KillType.Poison:
			{
				if (mCameraMode != null)
				{
					BaddieDeathCam.transform.position = mCameraMode.transform.position;
				}
				BaddieDeathCam component5 = BaddieDeathCam.GetComponent<BaddieDeathCam>();
				if (component5 != null)
				{
					component5.Trigger();
				}
				SwitchToCameraMode(BaddieDeathCam, CameraTransitionData.JumpCut);
				break;
			}
			default:
			{
				if (mCameraMode != null)
				{
					DeathShakeCam.transform.position = mCameraMode.transform.position;
				}
				DeathShakeCam component4 = DeathShakeCam.GetComponent<DeathShakeCam>();
				if (component4 != null)
				{
					component4.Trigger();
				}
				SwitchToCameraMode(DeathShakeCam, CameraTransitionData.JumpCut);
				break;
			}
			}
			break;
		case PlayerTheme.ThemeType.Bike:
		{
			if (mCameraMode != null)
			{
				BikeDeathCam.transform.position = mCameraMode.transform.position;
			}
			BikeDeathCam component3 = BikeDeathCam.GetComponent<BikeDeathCam>();
			if (component3 != null)
			{
				component3.Trigger();
			}
			SwitchToCameraMode(BikeDeathCam, CameraTransitionData.JumpCut);
			break;
		}
		case PlayerTheme.ThemeType.Cart:
		{
			if (mCameraMode != null)
			{
				MineCartDeathCam.transform.position = mCameraMode.transform.position;
			}
			MineCartDeathCam component2 = MineCartDeathCam.GetComponent<MineCartDeathCam>();
			if (component2 != null)
			{
				component2.Trigger();
			}
			SwitchToCameraMode(MineCartDeathCam, CameraTransitionData.JumpCut);
			break;
		}
		default:
		{
			if (mCameraMode != null)
			{
				DeathShakeCam.transform.position = mCameraMode.transform.position;
			}
			DeathShakeCam component = DeathShakeCam.GetComponent<DeathShakeCam>();
			if (component != null)
			{
				component.Trigger();
			}
			SwitchToCameraMode(DeathShakeCam, CameraTransitionData.JumpCut);
			break;
		}
		}
	}

	public void ShakeCurrentCamera()
	{
		if (mCameraMode != null)
		{
			mCameraMode.Shake();
		}
	}

	public void ShakeCurrentCamera(float ferocity, float duration)
	{
		if (mCameraMode != null)
		{
			mCameraMode.Shake(ferocity, duration);
		}
	}

	public void StopCameraShake()
	{
		mCameraMode.StopScreenShake();
	}
}
