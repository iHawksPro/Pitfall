using UnityEngine;

public class BaddieDeathCam : CameraBase
{
	public override void Awake()
	{
	}

	public override void Start()
	{
	}

	public override void Update()
	{
	}

	public void Trigger()
	{
		base.gameObject.ShakePosition(new Vector3(1f, 1f, 1f), 0.5f, 0f);
	}

	public override void LateUpdate()
	{
		if (base.TargetTransform != null)
		{
			LookUpdate(base.TargetTransform.position, 1f);
		}
		base.LateUpdate();
	}
}
