using UnityEngine;

public class GordonsCam : CameraBase
{
	public Vector3 Offset = new Vector3(-0.25f, 1.4f, 0.5f);

	public float MoveLag = 1f;

	public float LookAheadRange = 1f;

	private bool mHasCornered;

	private Vector3 mLookAheadCache;

	protected Transform mPlayerAnimatedTransform;

	protected PlayerAnimationController mPac;

	protected Vector3 mPathCentre;

	protected bool mClampTargetHeight;

	public bool UsePlayersUpForRoll;

	private Vector3 mOffsetMods = new Vector3(0f, 0f, 0f);

	public override void Awake()
	{
		mHasCornered = false;
		mClampTargetHeight = true;
		mPlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
		mPac = base.Target.GetComponentInChildren<PlayerAnimationController>();
	}

	public override void Reset()
	{
		Vector3 zero = Vector3.zero;
		zero += Offset.x * base.TargetTransform.forward;
		zero += Offset.y * base.TargetTransform.up;
		zero += Offset.z * base.TargetTransform.right;
		base.transform.position = base.TargetTransform.position + zero;
		base.transform.LookAt(base.LookAhead.transform.position);
		base.PreviousOffset = zero;
		mPlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Pelvis");
		base.Reset();
	}

	public virtual void ModOffsetsForAnimation(ref float modOffsX, ref float modOffsY, ref float lag)
	{
		if (mPac == null)
		{
			mPac = base.Target.GetComponentInChildren<PlayerAnimationController>();
		}
		if (mPac != null)
		{
			if (mPac.IsSliding())
			{
				modOffsY = 0f - Offset.y / 1.7f;
				modOffsX = 1f;
			}
			if (mPac.IsJumping())
			{
				modOffsY = 0.8f * (mPlayerAnimatedTransform.position.y - mPathCentre.y);
			}
			if (mPac.IsInRopeSwing())
			{
				modOffsY = 0.8f * (mPlayerAnimatedTransform.position.y - mPathCentre.y);
				modOffsX = -2f;
			}
		}
	}

	public override void LateUpdate()
	{
		Transform targetTransform = base.TargetTransform;
		mPathCentre = targetTransform.position;
		float num = 0f;
		if ((bool)LevelGenerator.Instance())
		{
			if (LevelGenerator.Instance().GetCurrentPiece() != null)
			{
				mPathCentre = LevelGenerator.Instance().GetPathCentreAtCurrentPoint();
				num = LevelGenerator.Instance().GetPathWidthAtCurrentPoint();
			}
			else
			{
				mPathCentre = targetTransform.position;
				num = 0f;
			}
		}
		mPathCentre.y = targetTransform.position.y;
		float lag = MoveLag;
		Vector3 vector = base.LookAhead.transform.position - mPathCentre;
		float magnitude = vector.magnitude;
		float leftRight = PlayerController.Instance().GetLeftRight();
		float modOffsY = 0f;
		float modOffsX = 0f;
		ModOffsetsForAnimation(ref modOffsX, ref modOffsY, ref lag);
		bool flag = false;
		if ((double)Vector3.Dot(targetTransform.forward, base.LookAhead.transform.forward) < 0.6)
		{
			flag = true;
			mHasCornered = true;
		}
		Vector3 vector2 = targetTransform.forward;
		Vector3 vector3 = targetTransform.right;
		if (!flag)
		{
			vector2 = vector;
			vector2.Normalize();
			vector3 = Vector3.Cross(targetTransform.up, vector2);
			vector3.Normalize();
		}
		Vector3 zero = Vector3.zero;
		zero += (Offset.x + modOffsX) * vector2;
		zero += (Offset.y + modOffsY) * targetTransform.up;
		zero += Offset.z * num * (leftRight * 0.33f) * vector3;
		Vector3 vector4 = Vector3.Lerp(base.PreviousOffset, zero, 10f * lag * Time.deltaTime);
		MoveUpdate(mPathCentre + vector4, lag);
		if (UsePlayersUpForRoll)
		{
			Roll = base.Target.transform.localEulerAngles.z;
		}
		Vector3 vector5 = vector;
		if (!mHasCornered)
		{
			mLookAheadCache = vector5;
		}
		vector5 = (mLookAheadCache = ((!flag) ? vector : (targetTransform.forward * LevelGenerator.Instance().PlayerLookAheadDistance)));
		vector5 *= LookAheadRange;
		vector5 += mPathCentre;
		vector5 += targetTransform.right * (num * (leftRight / 3f));
		if (mClampTargetHeight)
		{
			vector5.y = mPathCentre.y;
			if (modOffsY > 0f)
			{
				vector5.y += modOffsY;
			}
		}
		vector5 += LookAheadOffset;
		LookUpdate(vector5, lag);
		base.LateUpdate();
		base.PreviousOffset = vector4;
	}
}
