using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDescriptor : MonoBehaviour
{
	public List<PathRow> Rows = new List<PathRow>();

	public List<PathMaterialRow> Materials = new List<PathMaterialRow>();

	private bool mEntryPathTile;

	private bool mExitPathTile;

	private Transform mTransform;

	private bool mIsCachedWorldEntryPosL;

	private bool mIsCachedWorldEntryPosC;

	private bool mIsCachedWorldEntryPosR;

	private Vector3 mCachedWorldEntryPosL;

	private Vector3 mCachedWorldEntryPosC;

	private Vector3 mCachedWorldEntryPosR;

	private float mCachedClosestL;

	private float mCachedClosestC;

	private float mCachedClosestR;

	private static Vector3[] vecMat = new Vector3[4];

	private static float[] floatPointWeights = new float[4];

	public bool IsExitPath
	{
		get
		{
			return mExitPathTile;
		}
		set
		{
			mExitPathTile = value;
		}
	}

	private void Awake()
	{
		CacheTransform();
	}

	public float GetLength()
	{
		return 1f * (float)(Rows.Count - 1);
	}

	public Vector3 GetWorldPosition(float distance, float leftRight)
	{
		Vector3[] tilePoints = GetTilePoints(distance, leftRight);
		float[] tilePointWeights = GetTilePointWeights(distance, leftRight);
		return tilePoints[0] * tilePointWeights[0] + tilePoints[1] * tilePointWeights[1] + tilePoints[2] * tilePointWeights[2] + tilePoints[3] * tilePointWeights[3];
	}

	public Vector3 GetWorldPosEntryPathWithEntryLength(float leftRight)
	{
		if (mEntryPathTile)
		{
			if (leftRight < 0.1f && leftRight > -0.1f)
			{
				if (mIsCachedWorldEntryPosC)
				{
					return mCachedWorldEntryPosC;
				}
			}
			else if (leftRight > 0f)
			{
				if (mIsCachedWorldEntryPosR)
				{
					return mCachedWorldEntryPosR;
				}
			}
			else if (mIsCachedWorldEntryPosL)
			{
				return mCachedWorldEntryPosL;
			}
		}
		CacheEntryPathData();
		return GetWorldPosEntryPathWithEntryLength(leftRight);
	}

	public void CacheEntryPathData()
	{
		float length = GetLength();
		mCachedWorldEntryPosL = GetWorldPosition(length, -1f);
		mCachedWorldEntryPosC = GetWorldPosition(length, 0f);
		mCachedWorldEntryPosR = GetWorldPosition(length, 1f);
		mEntryPathTile = true;
		mIsCachedWorldEntryPosL = true;
		mIsCachedWorldEntryPosC = true;
		mIsCachedWorldEntryPosR = true;
	}

	public float GetClosestDistEntryExit(float leftRight)
	{
		if ((double)leftRight < -0.1)
		{
			return mCachedClosestL;
		}
		if ((double)leftRight > 0.1)
		{
			return mCachedClosestR;
		}
		return mCachedClosestC;
	}

	public void CacheClosestDist(float left, float centre, float right)
	{
		mCachedClosestL = left;
		mCachedClosestC = centre;
		mCachedClosestR = right;
	}

	public Vector3 GetWorldDirection(float distance, float leftRight)
	{
		int num = Mathf.FloorToInt(distance);
		float tileFraction = GetTileFraction(distance);
		CacheTransform();
		Matrix4x4 localToWorldMatrix = mTransform.localToWorldMatrix;
		int index = Mathf.Clamp(num, 0, Rows.Count - 1);
		int index2 = Mathf.Clamp(num + 1, 0, Rows.Count - 1);
		Vector3 normalized = (localToWorldMatrix.MultiplyPoint(Rows[index].Centre1) - localToWorldMatrix.MultiplyPoint(Rows[index].Centre0)).normalized;
		Vector3 normalized2 = (localToWorldMatrix.MultiplyPoint(Rows[index2].Centre1) - localToWorldMatrix.MultiplyPoint(Rows[index2].Centre0)).normalized;
		Vector3 normalized3 = (0.5f * localToWorldMatrix.MultiplyPoint(Rows[index2].Centre0) + 0.5f * localToWorldMatrix.MultiplyPoint(Rows[index2].Centre1) - (0.5f * localToWorldMatrix.MultiplyPoint(Rows[index].Centre0) + 0.5f * localToWorldMatrix.MultiplyPoint(Rows[index].Centre1))).normalized;
		Vector3 worldUp = GetWorldUp(distance, leftRight);
		Vector3 vector = Vector3.Cross(normalized, worldUp);
		Vector3 vector2 = Vector3.Cross(normalized2, worldUp);
		if (Vector3.Dot(normalized3, vector) < 0f)
		{
			vector = -vector;
		}
		if (Vector3.Dot(normalized3, vector2) < 0f)
		{
			vector2 = -vector2;
		}
		return (1f - tileFraction) * vector + tileFraction * vector2;
	}

	public Vector3 GetWorldUp(float distance, float leftRight)
	{
		Vector3[] tilePoints = GetTilePoints(distance, leftRight);
		float[] tilePointWeights = GetTilePointWeights(distance, leftRight);
		Vector3 normalized = (tilePoints[1] - tilePoints[0]).normalized;
		Vector3 normalized2 = (tilePoints[3] - tilePoints[2]).normalized;
		Vector3 normalized3 = (tilePoints[2] - tilePoints[0]).normalized;
		Vector3 normalized4 = (tilePoints[3] - tilePoints[1]).normalized;
		Vector3 vector = Vector3.Cross(normalized3, normalized);
		Vector3 vector2 = Vector3.Cross(normalized4, normalized);
		Vector3 vector3 = Vector3.Cross(normalized3, normalized2);
		Vector3 vector4 = Vector3.Cross(normalized4, normalized2);
		return tilePointWeights[0] * vector + tilePointWeights[1] * vector2 + tilePointWeights[2] * vector3 + tilePointWeights[3] * vector4;
	}

	public float GetClosestDistance(Vector3 position)
	{
		CacheTransform();
		Vector3 vector = mTransform.worldToLocalMatrix.MultiplyPoint(position);
		float num = float.PositiveInfinity;
		int num2 = 0;
		if (Rows.Count == 0)
		{
			return 0f;
		}
		int num3 = Rows.Count - 1;
		while (num3 >= 0)
		{
			Vector3 edgeCenter = GetEdgeCenter(Rows[num3].Centre0, Rows[num3].Centre1);
			float sqrMagnitude = (edgeCenter - vector).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num2 = num3;
				num = sqrMagnitude;
				num3--;
				continue;
			}
			return 1f * (float)num2;
		}
		return 1f * (float)num2;
	}

	public PathMaterial GetPathMaterial(float distance, float leftRight)
	{
		int tileRow = GetTileRow(distance);
		int leftRightIndex = GetLeftRightIndex(leftRight);
		if (tileRow >= Materials.Count)
		{
			return PathMaterial.None;
		}
		switch (leftRightIndex)
		{
		case 0:
			return Materials[tileRow].Left;
		case 1:
			return Materials[tileRow].Centre;
		case 2:
			return Materials[tileRow].Right;
		default:
			throw new Exception("GetPathMaterial: Left Right index out of bounds");
		}
	}

	public static Vector3 GetEdgeCenter(Vector3 a, Vector3 b)
	{
		return 0.5f * (a + b);
	}

	public static Vector3 GetTileCenter(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		return 0.25f * (a + b + c + d);
	}

	private int GetTileRow(float distance)
	{
		return Mathf.FloorToInt(distance);
	}

	private float GetTileFraction(float distance)
	{
		return distance - Mathf.Floor(distance);
	}

	private int GetLeftRightIndex(float leftRight)
	{
		float f = leftRight + 1.5f;
		return Mathf.Clamp(Mathf.FloorToInt(f), 0, 2);
	}

	private float GetLeftRightFraction(float leftRight)
	{
		float num = leftRight + 1.5f;
		return Mathf.Clamp01(num - Mathf.Clamp(Mathf.Floor(num), 0f, 2f));
	}

	public Vector3[] GetTilePoints(float distance, float leftRight)
	{
		int tileRow = GetTileRow(distance);
		int leftRightIndex = GetLeftRightIndex(leftRight);
		CacheTransform();
		Matrix4x4 localToWorldMatrix = mTransform.localToWorldMatrix;
		int index = Mathf.Clamp(tileRow, 0, Rows.Count - 1);
		int index2 = Mathf.Clamp(tileRow + 1, 0, Rows.Count - 1);
		switch (leftRightIndex)
		{
		case 0:
			vecMat[0] = localToWorldMatrix.MultiplyPoint(Rows[index].Left0);
			vecMat[1] = localToWorldMatrix.MultiplyPoint(Rows[index].Left1);
			vecMat[2] = localToWorldMatrix.MultiplyPoint(Rows[index2].Left0);
			vecMat[3] = localToWorldMatrix.MultiplyPoint(Rows[index2].Left1);
			return vecMat;
		case 1:
			vecMat[0] = localToWorldMatrix.MultiplyPoint(Rows[index].Centre0);
			vecMat[1] = localToWorldMatrix.MultiplyPoint(Rows[index].Centre1);
			vecMat[2] = localToWorldMatrix.MultiplyPoint(Rows[index2].Centre0);
			vecMat[3] = localToWorldMatrix.MultiplyPoint(Rows[index2].Centre1);
			return vecMat;
		case 2:
			vecMat[0] = localToWorldMatrix.MultiplyPoint(Rows[index].Right0);
			vecMat[1] = localToWorldMatrix.MultiplyPoint(Rows[index].Right1);
			vecMat[2] = localToWorldMatrix.MultiplyPoint(Rows[index2].Right0);
			vecMat[3] = localToWorldMatrix.MultiplyPoint(Rows[index2].Right1);
			return vecMat;
		default:
			throw new Exception("GetTilePoints: Left Right index out of bounds");
		}
	}

	private float[] GetTilePointWeights(float distance, float leftRight)
	{
		float tileFraction = GetTileFraction(distance);
		float leftRightFraction = GetLeftRightFraction(leftRight);
		floatPointWeights[0] = (1f - tileFraction) * (1f - leftRightFraction);
		floatPointWeights[1] = (1f - tileFraction) * leftRightFraction;
		floatPointWeights[2] = tileFraction * (1f - leftRightFraction);
		floatPointWeights[3] = tileFraction * leftRightFraction;
		return floatPointWeights;
	}

	private void GenerateDebugTileMeshes(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, string name, Tile tile)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = base.transform;
		Mesh mesh = new Mesh();
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		p1.y += 0.01f;
		p2.y += 0.01f;
		p3.y += 0.01f;
		p4.y += 0.01f;
		list.Add(p1);
		list.Add(p2);
		list.Add(p3);
		list.Add(p4);
		list2.Add(new Vector2(0f, 0f));
		list2.Add(new Vector2(1f, 0f));
		list2.Add(new Vector2(0f, 1f));
		list2.Add(new Vector2(1f, 1f));
		list3.Add(0);
		list3.Add(2);
		list3.Add(1);
		list3.Add(1);
		list3.Add(2);
		list3.Add(3);
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
		mesh.RecalculateNormals();
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>();
		gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
		if (tile.IsOfType(Tile.ResponseType.SwipeUp))
		{
			gameObject.GetComponent<Renderer>().material.color = Color.red;
		}
		else if (tile.IsOfType(Tile.ResponseType.SwipeDown))
		{
			gameObject.GetComponent<Renderer>().material.color = Color.blue;
		}
		else
		{
			gameObject.GetComponent<Renderer>().material.color = Color.green;
		}
	}

	private void CacheTransform()
	{
		if (mTransform == null)
		{
			mTransform = base.transform;
		}
	}
}
