using UnityEngine;

public class HeliCamSmooth : CameraBase
{
	protected Vector3 mPathCentre;

	public Vector3 Offset = new Vector3(-3f, 3f, 3f);

	public float MoveLag = 1f;

	public float TurnLag = 3f;

	public float LookAheadRange = 0.8f;

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
		float turnLag = TurnLag;
		float moveLag = MoveLag;
		Vector3 forward = base.TargetTransform.forward;
		forward.Normalize();
		Vector3 vector = Vector3.Cross(base.TargetTransform.up, forward);
		vector.Normalize();
		Vector3 zero = Vector3.zero;
		zero += Offset.x * forward;
		zero += Offset.y * base.TargetTransform.up;
		zero += Offset.z * vector;
		Vector3 vector2 = Vector3.Lerp(base.PreviousOffset, zero, 10f * moveLag * Time.deltaTime);
		MoveUpdate(base.TargetTransform.position + vector2, moveLag);
		Vector3 forward2 = base.TargetTransform.forward;
		forward2 *= LookAheadRange;
		forward2 += base.TargetTransform.position;
		forward2 += LookAheadOffset;
		LookUpdate(forward2, turnLag);
		base.LateUpdate();
		base.PreviousOffset = vector2;
	}
}
