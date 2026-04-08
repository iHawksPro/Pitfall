using UnityEngine;

public class HeliCam : CameraBase
{
	protected Vector3 mPathCentre;

	public Vector3 Offset = new Vector3(-3f, 3f, 3f);

	public float MoveLag = 1f;

	public float TurnLag = 3f;

	public float LookAheadRange = 0.8f;

	public Transform SmoothedTargetTransform;

	public float SmoothingAmount = 1f;

	private float mSmoothingChangeRate;

	public override void Awake()
	{
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
	}

	public override void LateUpdate()
	{
		SmoothingAmount = Mathf.Clamp01(SmoothingAmount + mSmoothingChangeRate * Time.deltaTime);
		Transform transform = ((!(SmoothedTargetTransform == null)) ? SmoothedTargetTransform : base.TargetTransform);
		float turnLag = TurnLag;
		float moveLag = MoveLag;
		Vector3 vector = Vector3.Lerp(base.TargetTransform.position, SmoothedTargetTransform.position, SmoothingAmount);
		Vector3 vector2 = Vector3.Lerp(base.TargetTransform.up, SmoothedTargetTransform.up, SmoothingAmount);
		Vector3 vector3 = base.LookAhead.transform.position - vector;
		vector3.Normalize();
		Vector3 vector4 = Vector3.Cross(vector2, vector3);
		vector4.Normalize();
		Vector3 zero = Vector3.zero;
		zero += Offset.x * vector3;
		zero += Offset.y * vector2;
		zero += Offset.z * vector4;
		Vector3 vector5 = Vector3.Lerp(base.PreviousOffset, zero, 10f * moveLag * Time.deltaTime);
		if (LevelGenerator.instance != null)
		{
			if (LevelGenerator.Instance().GetCurrentPiece() != null)
			{
				switch (LevelGenerator.Instance().GetCurrentPiece().Theme)
				{
				case WorldConstructionHelper.Theme.Mountain:
					mPathCentre = LevelGenerator.instance.GetPathCentreAt(0f);
					break;
				case WorldConstructionHelper.Theme.Jungle:
				case WorldConstructionHelper.Theme.Bike:
				{
					float leftRight = Mathf.Clamp(PlayerController.Instance().GetLeftRight(), -0.75f, 0.75f);
					mPathCentre = LevelGenerator.instance.GetPathCentreAt(leftRight);
					break;
				}
				default:
					mPathCentre = LevelGenerator.instance.GetPathCentreAtCurrentPoint();
					break;
				}
			}
		}
		else
		{
			mPathCentre = vector;
		}
		base.gameObject.transform.position = mPathCentre + vector5;
		Vector3 toAt = base.LookAhead.transform.position - vector;
		toAt *= LookAheadRange;
		toAt += vector;
		toAt += LookAheadOffset;
		LookUpdate(toAt, turnLag);
		base.LateUpdate();
		base.PreviousOffset = vector5;
	}

	public void GoSilky()
	{
		mSmoothingChangeRate = 1f;
	}

	public void GoHessian()
	{
		mSmoothingChangeRate = -1f;
	}
}
