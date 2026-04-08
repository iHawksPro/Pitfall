using UnityEngine;

public class LevelUtils
{
	public static void ModifyTransformToAlignPoints(Transform t, Transform a, Transform b)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetRow(0, a.right);
		identity.SetRow(1, a.up);
		identity.SetRow(2, a.forward);
		Matrix4x4 identity2 = Matrix4x4.identity;
		identity2.SetColumn(0, b.right);
		identity2.SetColumn(1, b.up);
		identity2.SetColumn(2, b.forward);
		Matrix4x4 matrix4x = identity * identity2;
		t.right = matrix4x.GetRow(0);
		t.up = matrix4x.GetRow(1);
		t.forward = matrix4x.GetRow(2);
		t.Translate(-b.position, Space.World);
		t.Translate(a.position, Space.World);
	}
}
