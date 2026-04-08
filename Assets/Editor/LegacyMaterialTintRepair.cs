using System;
using UnityEditor;
using UnityEngine;

public static class LegacyMaterialTintRepair
{
	private static readonly Color LegacyNeutralGrey = new Color(0.5882353f, 0.5882353f, 0.5882353f, 1f);
	private static readonly Color BrighterFrontEndGold = new Color(0.95f, 0.78f, 0.18f, 1f);
	private static readonly Color BrighterGold = new Color(0.72f, 0.58f, 0.1f, 1f);

	[MenuItem("Tools/Pitfall/Repair Legacy Material Tints")]
	public static void RunRepair()
	{
		int repairedCount = 0;
		string[] materialGuids = AssetDatabase.FindAssets("t:Material");
		foreach (string materialGuid in materialGuids)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(materialGuid);
			Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
			if (material == null || material.shader == null || !material.HasProperty("_Color"))
			{
				continue;
			}
			bool changed = false;
			Color currentColor = material.GetColor("_Color");
			if (material.shader.name == "Pitfall/CharWithPalette" && ApproximatelyNeutralGrey(currentColor))
			{
				material.SetColor("_Color", Color.white);
				changed = true;
			}
			else if (material.shader.name == "Sprite/Simple Texture (Unlit)" && material.mainTexture != null && ApproximatelyNeutralGrey(currentColor))
			{
				material.SetColor("_Color", Color.white);
				changed = true;
			}

			if (string.Equals(material.name, "FrontEndGold", StringComparison.OrdinalIgnoreCase))
			{
				Color adjustedColor = BrighterFrontEndGold;
				adjustedColor.a = currentColor.a;
				material.SetColor("_Color", adjustedColor);
				changed = true;
			}
			else if (string.Equals(material.name, "Gold", StringComparison.OrdinalIgnoreCase))
			{
				Color adjustedColor = BrighterGold;
				adjustedColor.a = currentColor.a;
				material.SetColor("_Color", adjustedColor);
				changed = true;
			}
			else if (string.Equals(material.name, "SmallActiLogo", StringComparison.OrdinalIgnoreCase) && ApproximatelyNeutralGrey(currentColor))
			{
				material.SetColor("_Color", Color.white);
				changed = true;
			}

			if (changed)
			{
				EditorUtility.SetDirty(material);
				repairedCount++;
			}
		}
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Debug.Log("LegacyMaterialTintRepair repaired " + repairedCount + " materials.");
	}

	private static bool ApproximatelyNeutralGrey(Color color)
	{
		return Mathf.Abs(color.r - LegacyNeutralGrey.r) < 0.015f && Mathf.Abs(color.g - LegacyNeutralGrey.g) < 0.015f && Mathf.Abs(color.b - LegacyNeutralGrey.b) < 0.015f && color.a > 0.95f;
	}
}
