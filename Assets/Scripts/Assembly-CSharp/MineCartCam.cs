using UnityEngine;

public class MineCartCam : GordonsCam
{
	public float mShakeAmount = 0.02f;

	private Vector3 mShakeOffs;

	public override void Awake()
	{
		base.Awake();
		mShakeOffs = Vector3.zero;
	}

	public override void ModOffsetsForAnimation(ref float modOffsX, ref float modOffsY, ref float lag)
	{
		if (mPac == null)
		{
			mPac = base.Target.GetComponentInChildren<PlayerAnimationController>();
		}
		if (mPac != null)
		{
			if (mPac.IsSliding())
			{
				modOffsY = 0f - Offset.y / 2f;
				modOffsX = 1f;
				lag *= 0.75f;
			}
			if (mPac.IsJumping())
			{
				modOffsY = 0.5f * (mPlayerAnimatedTransform.position.y - mPathCentre.y);
				modOffsX = -0.5f;
			}
			if (mPac.IsInRopeSwing())
			{
				modOffsY = 1.1f * (mPlayerAnimatedTransform.position.y - mPathCentre.y);
				modOffsX = -1.5f;
			}
		}
	}

	public override void LateUpdate()
	{
		if (!GameController.Instance.Paused())
		{
			base.transform.position -= mShakeOffs;
			float num = mShakeAmount;
			if (mPac != null && mPac.IsJumping())
			{
				num = 0f;
			}
			base.LateUpdate();
			Vector3 position = base.transform.position;
			mShakeOffs = new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), 0f);
			position += mShakeOffs;
			base.transform.position = position;
		}
	}
}
