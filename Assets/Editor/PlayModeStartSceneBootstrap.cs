using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeStartSceneBootstrap
{
	private const string BootScenePath = "Assets/Scenes/States/Boot.unity";

	static PlayModeStartSceneBootstrap()
	{
		EditorApplication.delayCall += EnsurePlayModeStartScene;
	}

	private static void EnsurePlayModeStartScene()
	{
		SceneAsset bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
		if (bootScene == null)
		{
			Debug.LogWarning("Could not find Boot scene at " + BootScenePath);
			return;
		}
		if (EditorSceneManager.playModeStartScene != bootScene)
		{
			EditorSceneManager.playModeStartScene = bootScene;
		}
	}
}
