using System;
using UnityEngine;

public class ExtraGizmos
{
	public static void DrawArrow(Transform transform, float length)
	{
		DrawArrow(transform.localToWorldMatrix, length);
	}

	public static void DrawArrow(Matrix4x4 matrix, float length)
	{
		float num = 0.1f;
		Vector3 vector = matrix.GetColumn(0);
		Vector3 vector2 = matrix.GetColumn(2);
		Vector3 vector3 = matrix.GetColumn(3);
		Vector3 to = new Vector3(vector3.x + length * vector2.x, vector3.y + length * vector2.y, vector3.z + length * vector2.z);
		Vector3 vector4 = new Vector3(to.x - num * vector2.x + num * vector.x, to.y - num * vector2.y + num * vector.y, to.z - num * vector2.z + num * vector.z);
		Vector3 vector5 = new Vector3(to.x - num * vector2.x - num * vector.x, to.y - num * vector2.y - num * vector.y, to.z - num * vector2.z - num * vector.z);
		Gizmos.DrawLine(vector3, to);
		Gizmos.DrawLine(vector4, to);
		Gizmos.DrawLine(vector5, to);
	}

	public static void DrawEdge(Transform transform)
	{
		DrawEdge(transform.localToWorldMatrix);
	}

	public static void DrawEdge(Matrix4x4 matrix)
	{
		Vector3 vector = matrix.GetColumn(0);
		Vector3 vector2 = matrix.GetColumn(3);
		Gizmos.DrawLine(vector2 - 1f * vector, vector2 + 1f * vector);
		DrawArrow(matrix, 0.5f);
	}

	public static void DrawCollider(Collider collider)
	{
		if (!(collider == null))
		{
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = collider.transform.localToWorldMatrix;
			Type type = collider.GetType();
			if (type == typeof(BoxCollider))
			{
				BoxCollider boxCollider = collider as BoxCollider;
				Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			}
			else if (type == typeof(SphereCollider))
			{
				SphereCollider sphereCollider = collider as SphereCollider;
				Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
			}
			Gizmos.matrix = matrix;
		}
	}

	public static void DrawSheet(Matrix4x4 start, float length, float width)
	{
		Vector3 vector = start.GetColumn(3) + 0.5f * length * start.GetColumn(2);
		start.SetColumn(3, vector);
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = start;
		Gizmos.DrawCube(Vector3.zero, new Vector3(width, 0.01f, length));
		Gizmos.matrix = matrix;
	}
}
