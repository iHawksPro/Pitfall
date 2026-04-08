using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidReleaseBuild
{
	private const string DefaultApplicationId = "com.ihawkspro.pitfall.recovered";

	private const string DefaultVersion = "1.0.0";

	private const int DefaultVersionCode = 1;

	[MenuItem("Tools/Pitfall/Build Android APK")]
	public static void BuildMenu()
	{
		Build();
	}

	public static void BuildFromCommandLine()
	{
		Build();
	}

	private static void Build()
	{
		string projectPath = Directory.GetParent(Application.dataPath).FullName;
		string outputDirectory = Path.Combine(projectPath, "Builds", "Android");
		string outputPath = Path.Combine(outputDirectory, "Pitfall-recovered-arm64.apk");
		string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
		if (scenes.Length == 0)
		{
			throw new InvalidOperationException("No enabled scenes were found for the Android build.");
		}
		Directory.CreateDirectory(outputDirectory);
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		EditorUserBuildSettings.buildAppBundle = false;
		EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, DefaultApplicationId);
		PlayerSettings.bundleVersion = DefaultVersion;
		PlayerSettings.Android.bundleVersionCode = DefaultVersionCode;
		PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
		PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
		PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
		PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
		PlayerSettings.productName = "Pitfall Recovered";
		PlayerSettings.companyName = "iHawksPro";
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = outputPath,
			target = BuildTarget.Android,
			options = BuildOptions.None
		};
		BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
		if (buildReport.summary.result != BuildResult.Succeeded)
		{
			throw new InvalidOperationException("Android APK build failed. See the Unity log for details.");
		}
		Console.WriteLine("APK_PATH=" + outputPath);
		Console.WriteLine("APK_SIZE=" + new FileInfo(outputPath).Length);
	}
}
