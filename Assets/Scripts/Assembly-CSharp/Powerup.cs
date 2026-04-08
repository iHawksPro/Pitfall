using UnityEngine;

public class Powerup : Pickup
{
	public enum PowerupType
	{
		CoinMagnet = 0,
		Invincible = 1,
		SpeedBoost = 2,
		Max = 3
	}

	private PowerupType mType;

	public Powerup(GameObject model, Vector3 position, PowerupType type, Transform parent)
	{
		if (type != PowerupType.SpeedBoost)
		{
			type = PowerupType.SpeedBoost;
		}
		mType = type;
		mObject = model;
		Vector3 localScale = new Vector3(0.5f, 0.5f, 0.5f);
		mObject.transform.localScale = localScale;
		Vector3 position2 = new Vector3(0f, -0.25f, 0f);
		position2 += position;
		mObject.transform.position = position2;
		mObject.transform.parent = parent;
	}

	protected override void Animate()
	{
		Vector3 axis = new Vector3(0f, 1f, 0f);
		mObject.transform.RotateAround(axis, Time.deltaTime * 5f);
	}

	public PowerupType GetPowerupType()
	{
		return mType;
	}
}
