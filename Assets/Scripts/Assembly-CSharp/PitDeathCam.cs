public class PitDeathCam : CameraBase
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
