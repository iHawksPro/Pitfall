using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneRenderAudit
{
	[MenuItem("Tools/Pitfall/Audit Title Scene Renderers")]
	public static void AuditTitleScene()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/States/Title.unity", OpenSceneMode.Single);
		Debug.Log("=== SceneRenderAudit: Title.unity ===");
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None).OrderBy(r => r.name))
		{
			Material material = renderer.sharedMaterial;
			if (material == null)
			{
				continue;
			}
			Mesh mesh = GetMesh(renderer);
			string materialPath = AssetDatabase.GetAssetPath(material);
			string meshPath = AssetDatabase.GetAssetPath(mesh);
			string shaderName = material.shader != null ? material.shader.name : "<null>";
			Debug.Log(string.Format("Renderer '{0}' shader='{1}' material='{2}' mesh='{3}'", renderer.name, shaderName, materialPath, meshPath));
			if (mesh == null)
			{
				continue;
			}
			Vector2[] uv = mesh.uv;
			Vector2[] uv2 = mesh.uv2;
			Color[] colors = mesh.colors;
			Debug.Log(string.Format("  mesh='{0}' verts={1} uv={2} uv2={3} colors={4}", mesh.name, mesh.vertexCount, uv.Length, uv2.Length, colors.Length));
			if (uv.Length > 0)
			{
				Debug.Log("  uv range=" + DescribeRange(uv));
				Debug.Log("  uv samples=" + string.Join(" | ", uv.Take(4).Select(v => v.ToString("F3")).ToArray()));
			}
			if (uv2.Length > 0)
			{
				Debug.Log("  uv2 range=" + DescribeRange(uv2));
				Debug.Log("  uv2 samples=" + string.Join(" | ", uv2.Take(4).Select(v => v.ToString("F3")).ToArray()));
			}
			if (colors.Length > 0)
			{
				Debug.Log("  color range=" + DescribeColorRange(colors));
				Debug.Log("  color samples=" + string.Join(" | ", colors.Take(4).Select(c => c.ToString()).ToArray()));
			}
		}
	}

	private static Mesh GetMesh(Renderer renderer)
	{
		MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			return meshFilter.sharedMesh;
		}
		SkinnedMeshRenderer skinned = renderer as SkinnedMeshRenderer;
		if (skinned != null)
		{
			return skinned.sharedMesh;
		}
		return null;
	}

	private static string DescribeRange(Vector2[] values)
	{
		float minX = values.Min(v => v.x);
		float maxX = values.Max(v => v.x);
		float minY = values.Min(v => v.y);
		float maxY = values.Max(v => v.y);
		return string.Format("x[{0:F3},{1:F3}] y[{2:F3},{3:F3}]", minX, maxX, minY, maxY);
	}

	private static string DescribeColorRange(Color[] values)
	{
		float minR = values.Min(v => v.r);
		float maxR = values.Max(v => v.r);
		float minG = values.Min(v => v.g);
		float maxG = values.Max(v => v.g);
		float minB = values.Min(v => v.b);
		float maxB = values.Max(v => v.b);
		float minA = values.Min(v => v.a);
		float maxA = values.Max(v => v.a);
		return string.Format("r[{0:F3},{1:F3}] g[{2:F3},{3:F3}] b[{4:F3},{5:F3}] a[{6:F3},{7:F3}]", minR, maxR, minG, maxG, minB, maxB, minA, maxA);
	}
}
