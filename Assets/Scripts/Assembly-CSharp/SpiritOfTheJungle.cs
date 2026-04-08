using UnityEngine;

public class SpiritOfTheJungle
{
	private float mTimer;

	private float mCoolDown;

	public SpiritOfTheJungle()
	{
		Reset();
	}

	public int NumHeld()
	{
		return SecureStorage.Instance.GetItemCount("consumable.jungle");
	}

	public bool Use()
	{
		if (NumHeld() > 0 && !Active())
		{
			mTimer = UpgradeHelper.SpiritOfTheJungleDuration();
			SecureStorage.Instance.ChangeItemCount("consumable.jungle", -1);
			PlayerController.Instance().PotionToUse = PlayerController.Potion_Type.Spirit;
			PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.PlaySpiritEffect();
			return true;
		}
		return false;
	}

	public void Reset()
	{
		mTimer = 0f;
		mCoolDown = 0f;
	}

	public bool Active()
	{
		return mTimer > 0f;
	}

	public bool CoolDownActive()
	{
		return mCoolDown > 0f;
	}

	public void Update()
	{
		if (Active() && PlayerController.Instance().IsInSpeedBoostBonus())
		{
			Reset();
			PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.StopSpiritEffect();
			TBFUtils.DebugLog("Stop Spirit Effect");
		}
		else if (mTimer > 0f)
		{
			mTimer -= Time.deltaTime;
			if (mTimer <= 0f)
			{
				mTimer = 0f;
				mCoolDown = UpgradeHelper.SpiritOfTheJungleCoolDownDuration();
				PlayerController.Instance().GetPlayerAnimationController().PlayerFXController.StopSpiritEffect();
				TBFUtils.DebugLog("Stop Spirit Effect");
			}
		}
		else if (mCoolDown > 0f)
		{
			mCoolDown = Mathf.Max(0f, mCoolDown - Time.deltaTime);
		}
	}
}
