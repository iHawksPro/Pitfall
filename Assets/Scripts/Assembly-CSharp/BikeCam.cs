public class BikeCam : GordonsCam
{
	public override void ModOffsetsForAnimation(ref float modOffsX, ref float modOffsY, ref float lag)
	{
		if (mPac == null)
		{
			mPac = base.Target.GetComponentInChildren<PlayerAnimationController>();
		}
		if (mPac != null && mPac.IsSliding())
		{
			modOffsY = 0f - Offset.y / 2f;
		}
	}
}
