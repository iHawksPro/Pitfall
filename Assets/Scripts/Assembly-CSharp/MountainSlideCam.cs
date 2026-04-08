public class MountainSlideCam : GordonsCam
{
	public override void Awake()
	{
		base.Awake();
		mClampTargetHeight = false;
	}

	public override void ModOffsetsForAnimation(ref float modOffsX, ref float modOffsY, ref float lag)
	{
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}
}
