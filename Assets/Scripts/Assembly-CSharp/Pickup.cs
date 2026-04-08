using UnityEngine;

public class Pickup
{
	protected GameObject mObject;

	protected GameObject mPickupPFX;

	protected Color mPickupPFXColor = new Color(1f, 1f, 1f, 1f);

	private bool mCollected;

	private float mInitialScaleAxis;

	private float mTimeToLive;

	private float mTimeToRelease;

	public Pickup()
	{
		mCollected = false;
		mInitialScaleAxis = (mTimeToLive = 0f);
		mTimeToRelease = 20f;
	}

	public void Update(PlayerController player)
	{
		if (!IsValid())
		{
			return;
		}
		if (mCollected)
		{
			Vector3 vector = player.transform.position + player.transform.up * 150f + player.transform.forward * 500f - player.transform.right * 150f;
			Vector3 position = mObject.transform.position;
			Vector3 vector2 = vector - position;
			mTimeToLive -= Time.deltaTime;
			if (mTimeToLive > 0f)
			{
				vector2.Normalize();
				mObject.transform.Translate(vector2 * Time.deltaTime * player.GetCurrentSpeed() * 2f, Space.World);
			}
			else
			{
				mObject.SetActiveRecursively(false);
			}
		}
		else
		{
			Animate();
			mTimeToRelease -= Time.deltaTime;
			if (mTimeToRelease < 0f)
			{
				mObject.SetActiveRecursively(false);
			}
		}
	}

	public bool IsValid()
	{
		if (mObject == null)
		{
			return false;
		}
		if (!mObject.active)
		{
			return false;
		}
		return true;
	}

	public bool Collect(PlayerController player, float collectionDistance)
	{
		if (mCollected)
		{
			return false;
		}
		float sqrMagnitude = (mObject.transform.position - PickupController.Instance().PlayerAnimatedTransform.position).sqrMagnitude;
		if (sqrMagnitude < collectionDistance * collectionDistance)
		{
			mCollected = true;
			mTimeToLive = 0f;
			if (mPickupPFX != null)
			{
				GameObject gameObject = Object.Instantiate(mPickupPFX, mObject.transform.position, Quaternion.identity) as GameObject;
				gameObject.GetComponent<Renderer>().material.color = mPickupPFXColor;
			}
			return true;
		}
		return false;
	}

	protected virtual void Animate()
	{
		if (mInitialScaleAxis == 0f)
		{
			mInitialScaleAxis = mObject.transform.localScale.x;
		}
		float num = mInitialScaleAxis * 0.2f;
		float num2 = mInitialScaleAxis - num * 0.5f + Mathf.Sin(Time.time * 5f) * num;
		Vector3 localScale = new Vector3(num2, num2, num2);
		mObject.transform.localScale = localScale;
	}
}
