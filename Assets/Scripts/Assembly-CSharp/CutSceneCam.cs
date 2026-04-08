public class CutSceneCam : CameraBase
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
		if (base.TargetTransform != null)
		{
			base.transform.LookAt(base.TargetTransform.position);
		}
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
