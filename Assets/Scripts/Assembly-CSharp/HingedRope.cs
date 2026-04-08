using UnityEngine;

public class HingedRope : MonoBehaviour
{
	public enum RopeState
	{
		WaitForNode = 0,
		GotNode = 1,
		PlayerAttached = 2,
		PlayerLetGo = 3
	}

	private RopeState mRopeState;

	private float mRootRotXRad;

	private Transform mRopeRootObject;

	private Transform mRopeModelObject;

	public void SetState(RopeState newState)
	{
		if (newState != RopeState.PlayerLetGo || mRopeState == RopeState.PlayerAttached)
		{
			mRopeState = newState;
		}
	}

	public bool HasValidNode(PieceDescriptor playerPiece)
	{
		Transform ropeNode = null;
		if (LevelGenerator.Instance().GetRopeNode(out ropeNode) && ropeNode.transform.parent == playerPiece.transform)
		{
			return true;
		}
		return false;
	}

	private void Awake()
	{
		mRopeRootObject = base.transform.GetChild(0);
		mRopeModelObject = mRopeRootObject.GetChild(0);
	}

	private void Start()
	{
		SetState(RopeState.WaitForNode);
	}

	public void LateUpdate()
	{
		switch (mRopeState)
		{
		case RopeState.WaitForNode:
			UpdateRopeState_WaitForNode();
			break;
		case RopeState.GotNode:
			UpdateRopeState_GotNode();
			break;
		case RopeState.PlayerAttached:
			UpdateRopeState_PlayerAttached();
			break;
		case RopeState.PlayerLetGo:
			UpdateRopeState_PlayerLetGo();
			break;
		}
	}

	private void UpdateRopeState_WaitForNode()
	{
		Transform ropeNode;
		if (LevelGenerator.Instance().GetRopeNode(out ropeNode))
		{
			base.transform.position = ropeNode.transform.position;
			base.transform.rotation = ropeNode.transform.rotation;
			mRopeRootObject.localEulerAngles = new Vector3(0f, 0f, 0f);
			mRootRotXRad = 0f;
			mRopeModelObject.localPosition = new Vector3(0f, 0f, 0f);
			SetState(RopeState.GotNode);
		}
	}

	private void UpdateRopeState_GotNode()
	{
		PlayerController playerController = PlayerController.Instance();
		Transform ropeNode;
		if (LevelGenerator.Instance().GetRopeNode(out ropeNode) && !playerController.IsDead() && !playerController.DebugInvincible)
		{
			GameObject playerModel = LevelGenerator.Instance().Player.PlayerModel;
			Vector3 position = playerModel.transform.position;
			Vector3 rhs = -ropeNode.transform.forward;
			Vector3 normalized = (base.transform.position - position).normalized;
			float num = Vector3.Dot(normalized, rhs);
			if (num < 0f)
			{
				SetState(RopeState.PlayerAttached);
			}
		}
	}

	private void UpdateRopeState_PlayerAttached()
	{
		GameObject playerModel = LevelGenerator.Instance().Player.PlayerModel;
		Vector3 position = playerModel.transform.position;
		Transform handBone = PlayerHelper.GetHandBone(playerModel);
		if ((bool)handBone)
		{
			position = handBone.position;
		}
		base.transform.position -= base.transform.forward * PlayerController.Instance().GetDistanceMoved() * 0.7f;
		Vector3 up = Vector3.up;
		Vector3 vector = base.transform.position - position;
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		float f = Vector3.Dot(vector, up);
		float num = 3f;
		float num2 = 0f;
		mRootRotXRad = Mathf.Acos(f);
		num2 = magnitude - num;
		mRopeRootObject.localEulerAngles = new Vector3(mRootRotXRad * 57.29578f, 0f, 0f);
		Vector3 localPosition = new Vector3(0f, 0f - num2, 0f);
		mRopeModelObject.localPosition = localPosition;
	}

	private void UpdateRopeState_PlayerLetGo()
	{
		GameObject playerModel = LevelGenerator.Instance().Player.PlayerModel;
		if ((base.transform.position - playerModel.transform.position).sqrMagnitude > 25f)
		{
			LevelGenerator.Instance().ClearRopeNode();
			SetState(RopeState.WaitForNode);
		}
	}
}
