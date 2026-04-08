using UnityEngine;

public class Meteor
{
	public enum TARGET
	{
		PATH = 0,
		OFF_PATH = 1,
		TO_NODE = 2,
		THROUGH_NODE = 3
	}

	public enum METEOR_TYPE
	{
		SMALL = 0,
		LARGE = 1,
		HUGE = 2,
		DISINTEGRATING = 3
	}

	private Vector3 mStartPosition;

	private Vector3 mEndPosition;

	private Vector3 mNormalisedVelocity;

	private Vector3 mVelocity;

	private float mTimeToImpact;

	private float mCurrentFlightDelta;

	private bool mCanKillPlayer;

	private float mCurrentParticleDelta;

	private float mTimeToStraighten = 1.5f;

	private GameObject mModel;

	private float mLife;

	private METEOR_TYPE mType;

	private bool mDoneFirstCollision;

	private bool mHasInjuredPlayer;

	private GameObject mImpactPrefab;

	private bool mDestroyOnImpact;

	private AnimationCurve mSpeedCurve;

	private PlayerAnimationController mPac;

	private float mTotalFlightTime;

	private bool mHasAvoided;

	private TARGET mTarget;

	public Meteor(GameObject model, METEOR_TYPE type, GameObject impactPrefab, AnimationCurve speedCurve)
	{
		mModel = model;
		mType = type;
		mImpactPrefab = impactPrefab;
		mSpeedCurve = speedCurve;
		mModel.SetActiveRecursively(false);
	}

	public METEOR_TYPE GetMeteorType()
	{
		return mType;
	}

	public GameObject GetImpactPrefab()
	{
		return mImpactPrefab;
	}

	private void Start()
	{
	}

	public void SetUpMeteor(Vector3 startPosition, Vector3 endPosition, float timeToImpact, float life, bool canKillPlayer, GameObject playerObj, TARGET target, bool destroyOnImpact, GameObject impactPrefab)
	{
		mStartPosition = startPosition;
		mEndPosition = endPosition;
		mVelocity = mEndPosition - mStartPosition;
		mNormalisedVelocity = mVelocity.normalized;
		mTimeToImpact = timeToImpact;
		mCurrentFlightDelta = 0f;
		mLife = life;
		mDoneFirstCollision = false;
		mCanKillPlayer = canKillPlayer;
		mTarget = target;
		mCurrentParticleDelta = 0f;
		mDestroyOnImpact = destroyOnImpact;
		mImpactPrefab = impactPrefab;
		mPac = PlayerController.instance.GetPlayerAnimationController();
		mModel.SetActiveRecursively(true);
		mModel.transform.up = -mNormalisedVelocity;
		mModel.transform.position = mStartPosition;
		mTotalFlightTime = 0f;
		mHasAvoided = false;
		mModel.transform.parent = LevelGenerator.Instance().transform;
	}

	public void SetUpMeteor(Vector3 startPosition, Vector3 endPosition, float timeToImpact, float life, bool canKillPlayer, GameObject playerObj, TARGET target, bool destroyOnImpact)
	{
		SetUpMeteor(startPosition, endPosition, timeToImpact, life, canKillPlayer, playerObj, target, destroyOnImpact, mImpactPrefab);
	}

	public void Update()
	{
		if (mTarget == TARGET.PATH || mTarget == TARGET.TO_NODE)
		{
			UpdateToTarget();
		}
		else if (mTarget == TARGET.OFF_PATH || mTarget == TARGET.THROUGH_NODE)
		{
			UpdateThroughTarget();
		}
		if (!mModel)
		{
			return;
		}
		PlayerController playerController = PlayerController.Instance();
		if (mPac != null && !mPac.IsJumping() && mCanKillPlayer)
		{
			float sqrMagnitude = (playerController.transform.position - mModel.transform.position).sqrMagnitude;
			float num = 0.04f;
			if (mType == METEOR_TYPE.HUGE)
			{
				num = 0.06f;
			}
			float num2 = playerController.GetCurrentSpeed() * num;
			if (sqrMagnitude < num2 && !playerController.IsInvincible() && !playerController.IsImmuneFromDamage())
			{
				playerController.Kill(PieceDescriptor.KillType.Meteor);
				mCanKillPlayer = false;
			}
			if (!mHasAvoided && ((playerController.IsInSpeedBoostBonus() && playerController.SpeedBoostAffectsSpeed()) || playerController.DebugInvincible || playerController.IsImmuneFromDamage()) && mType == METEOR_TYPE.HUGE)
			{
				float num3 = 20f;
				if (sqrMagnitude < num3)
				{
					playerController.Swipe(PlayerController.SwipeBuffer.Up);
					mHasAvoided = true;
				}
			}
		}
		if (mTotalFlightTime > mTimeToImpact + mLife && (!playerController.IsDead() || mDestroyOnImpact))
		{
			RemoveFromPlay();
		}
	}

	private void UpdateToTarget()
	{
		if (!mModel)
		{
			return;
		}
		mCurrentFlightDelta += Time.deltaTime;
		mTotalFlightTime += Time.deltaTime;
		float num = mSpeedCurve.Evaluate(Mathf.Clamp01(mCurrentFlightDelta / mTimeToImpact));
		mModel.transform.position = mStartPosition + mVelocity * num;
		if (mDoneFirstCollision)
		{
			mCurrentParticleDelta += Time.deltaTime;
			float num2 = Mathf.Clamp01(mCurrentParticleDelta / mTimeToStraighten);
			Vector3 up = Vector3.up + -mNormalisedVelocity * (1f - num2);
			mModel.transform.up = up;
		}
		if (mCanKillPlayer && ((mType == METEOR_TYPE.DISINTEGRATING && num >= 0.75f) || num >= 1f) && !mDoneFirstCollision)
		{
			if ((mType == METEOR_TYPE.LARGE || mType == METEOR_TYPE.HUGE) && !GameController.Instance.Paused())
			{
				CameraManager.Instance.ShakeCurrentCamera();
			}
			Object.Instantiate(mImpactPrefab, mModel.transform.position, Quaternion.identity);
			mDoneFirstCollision = true;
		}
		if (mDoneFirstCollision && mDestroyOnImpact)
		{
			RemoveFromPlay();
		}
	}

	private void UpdateThroughTarget()
	{
		if ((bool)mModel)
		{
			mCurrentFlightDelta += Time.deltaTime;
			mTotalFlightTime += Time.deltaTime;
			float num = mSpeedCurve.Evaluate(Mathf.Clamp01(mCurrentFlightDelta / mTimeToImpact));
			if (num < 1f)
			{
				mModel.transform.position = mStartPosition + mVelocity * num;
				return;
			}
			mStartPosition += mVelocity * num;
			mCurrentFlightDelta = 0f;
		}
	}

	public void SetTarget(TARGET target)
	{
		mTarget = target;
		if (mTarget == TARGET.PATH)
		{
			mCanKillPlayer = true;
		}
		else if (mTarget == TARGET.OFF_PATH)
		{
			mCanKillPlayer = false;
		}
	}

	public void Destroy()
	{
		Object.Destroy(mModel);
		mModel = null;
	}

	public bool IsValid()
	{
		return mModel != null;
	}

	public GameObject GetModel()
	{
		if (IsValid())
		{
			return mModel;
		}
		return null;
	}

	public void RemoveFromPlay()
	{
		if (IsValid())
		{
			mModel.SetActiveRecursively(false);
			mLife = 0f;
			mCanKillPlayer = false;
		}
	}

	public bool Active()
	{
		return mLife > 0f;
	}
}
