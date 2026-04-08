using UnityEngine;

public class UpgradeTable
{
	private float[] mTable;

	public UpgradeTable(float[] table)
	{
		mTable = table;
	}

	public float Lookup(int index)
	{
		index = Mathf.Clamp(index, 0, mTable.Length - 1);
		return mTable[index];
	}
}
