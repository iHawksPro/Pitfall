using UnityEngine;

public class JaguarCam : GordonsCam
{
	private float mTimer;

	private float mCurrentRoll;

	private float mCentrePoint;

	public float mBobbingSpeed = 0.0018f;

	public float mRollAmount = 1f;

	private Vector3 mOriginalLookAheadOffset;

	public override void Awake()
	{
		base.Awake();
		mOriginalLookAheadOffset = LookAheadOffset;
		mTimer = 0f;
		mCentrePoint = Offset.y;
	}

	public override void ModOffsetsForAnimation(ref float modOffsX, ref float modOffsY, ref float lag)
	{
		if (mPac == null)
		{
			mPac = base.Target.GetComponentInChildren<PlayerAnimationController>();
		}
		if (mPac != null && mPac.IsJumping())
		{
			modOffsY = 0.5f * (mPlayerAnimatedTransform.position.y - mPathCentre.y);
			modOffsX = -2f;
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		if (!PlayerController.Instance().IsRunning())
		{
			return;
		}
		if (!PlayerController.Instance().IsJumping())
		{
			Vector3 lookAheadOffset = LookAheadOffset;
			float num = Mathf.Sin(mTimer);
			mTimer += mBobbingSpeed;
			if ((double)mTimer > 3.4 || mTimer < 0f)
			{
				mBobbingSpeed *= -1f;
				mRollAmount *= -1f;
			}
			if (num != 0f)
			{
				float num2 = num;
				lookAheadOffset.y = mOriginalLookAheadOffset.y + mCentrePoint + num2;
			}
			LookAheadOffset = lookAheadOffset;
			Roll = mRollAmount;
		}
		else
		{
			mTimer = 0f;
		}
	}
}
