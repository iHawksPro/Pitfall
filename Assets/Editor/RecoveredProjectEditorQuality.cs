using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class RecoveredProjectEditorQuality
{
	static RecoveredProjectEditorQuality()
	{
		EditorApplication.delayCall += EnsureModernQuality;
	}

	private static void EnsureModernQuality()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			return;
		}
		string[] names = QualitySettings.names;
		if (names == null || names.Length == 0)
		{
			return;
		}
		int desiredQuality = names.Length - 1;
		if (QualitySettings.GetQualityLevel() != desiredQuality)
		{
			QualitySettings.SetQualityLevel(desiredQuality, applyExpensiveChanges: true);
		}
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
	}
}
