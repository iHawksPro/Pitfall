using System;

[Serializable]
public class PlayerTweaks
{
	public float InitialSpeed = 12f;

	public float Acceleration = 10f;

	public float MaxSpeed = 50f;

	public float SpeedIncrease = 0.01f;

	public float RestartSpeedOffset = 3f;

	public float PlayerHorizontalMovementScale = 1f;

	public float ActionWarmDownTimeSwipeUp = 0.565f;

	public float ActionWarmDownTimeSwipeDown = 0.41f;

	public float ActionWarmDownTimeLongUp = 0.82f;

	public float CoinCollectionRangeDefault = 1f;

	public float CoinBikeCollectionMultiplier = 2f;

	public float CoinCollectionRangeBonus = 10f;

	public float CoinCollectionBonusTime = 10f;

	public float InvincibleBonusTime = 10f;

	public float SpeedBoostBonusMultiplier = 5f;

	public float MinimumSlideTime = 0.2f;

	public float TurnBufferLength = 20f;

	public float NoDodgeZoneLength = 20f;
}
