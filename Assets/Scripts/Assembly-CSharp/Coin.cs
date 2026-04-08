using UnityEngine;

public class Coin : Pickup
{
	public enum CoinType
	{
		ValueLow = 0,
		ValueMedium = 1,
		ValueHigh = 2,
		Max = 3
	}

	private CoinType mCoinType;

	private float mSpinRate;

	private Vector3 mRotationAxis;

	private int mRunIndex;

	private int mRunElementIndex;

	private static float mPhaseCounter;

	public Coin(GameObject model, GameObject pickupPFX, Color colour, Vector3 position, Transform parent, CoinType type, int runIndex, int runElementIndex)
	{
		mObject = model;
		Vector3 position2 = new Vector3(0f, 0f, 0f);
		Vector3 localScale = new Vector3(100f, 100f, 100f);
		position2 += position;
		mObject.transform.position = position2;
		mObject.transform.localScale = localScale;
		mCoinType = type;
		mObject.transform.parent = parent;
		mRunIndex = runIndex;
		mRunElementIndex = runElementIndex;
		mSpinRate = 8f;
		mRotationAxis = new Vector3(0f, 1f, 0f);
		mObject.transform.RotateAround(mRotationAxis, 0.4f * mPhaseCounter);
		mPhaseCounter += 1f;
		mPickupPFX = pickupPFX;
		mPickupPFXColor = colour;
	}

	protected override void Animate()
	{
		mObject.transform.RotateAround(mRotationAxis, Time.deltaTime * mSpinRate);
	}

	public CoinType GetCoinType()
	{
		return mCoinType;
	}

	public int GetRunIndex()
	{
		return mRunIndex;
	}

	public int GetRunElementIndex()
	{
		return mRunElementIndex;
	}
}
