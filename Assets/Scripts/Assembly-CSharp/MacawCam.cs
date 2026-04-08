using UnityEngine;

public class MacawCam : CameraBase
{
	public Vector3 Offset = new Vector3(-2.25f, 3.25f, 0f);

	public float MoveLag = 0.5f;

	public float LookAheadRange = 1f;

	private Transform mPlayerAnimatedTransform;

	public override void Awake()
	{
		mPlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Footsteps");
	}

	public override void Start()
	{
		Reset();
	}

	public override void Update()
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
		mPlayerAnimatedTransform = PlayerHelper.SearchHierarchyForBone(PlayerController.Instance().PlayerModel.transform, "Bip001 Footsteps");
	}

	public override void LateUpdate()
	{
		if (mPlayerAnimatedTransform != null)
		{
			float moveLag = MoveLag;
			Transform targetTransform = base.TargetTransform;
			float num = mPlayerAnimatedTransform.position.y - targetTransform.position.y;
			float num2 = 0f;
			Vector3 vector = base.LookAhead.transform.position - targetTransform.position;
			vector.Normalize();
			Vector3 vector2 = Vector3.Cross(targetTransform.up, vector);
			vector2.Normalize();
			Vector3 zero = Vector3.zero;
			zero += (Offset.x + num2) * vector;
			zero += (Offset.y + num) * targetTransform.up;
			zero += Offset.z * vector2;
			float num3 = Vector2.Angle(new Vector2(base.PreviousOffset.x, base.PreviousOffset.z), new Vector2(zero.x, zero.z));
			Vector3 vector3 = Vector3.Lerp(base.PreviousOffset, zero, Mathf.Clamp01(Time.deltaTime * num3 * 10f));
			vector3.y = zero.y;
			MoveUpdate(targetTransform.position + vector3, moveLag);
			Vector3 toAt = base.LookAhead.transform.position - base.TargetTransform.position;
			toAt *= LookAheadRange;
			toAt += base.TargetTransform.position;
			toAt.y = targetTransform.position.y;
			toAt += LookAheadOffset;
			LookUpdate(toAt, moveLag);
			base.LateUpdate();
			base.PreviousOffset = vector3;
		}
	}
}
