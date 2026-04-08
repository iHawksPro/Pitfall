using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WindowsReleaseBuild
{
	private const string DefaultVersion = "1.0.3";

	private const string ExecutableName = "Pitfall-recovered.exe";

	private const string PackageName = "Pitfall-recovered-windows.zip";

	[MenuItem("Tools/Pitfall/Build Windows Release")]
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
		string outputDirectory = Path.Combine(projectPath, "Builds", "Windows");
		string playerDirectory = Path.Combine(outputDirectory, "Pitfall-recovered");
		string executablePath = Path.Combine(playerDirectory, ExecutableName);
		string packagePath = Path.Combine(outputDirectory, PackageName);
		string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
		if (scenes.Length == 0)
		{
			throw new InvalidOperationException("No enabled scenes were found for the Windows build.");
		}
		if (Directory.Exists(playerDirectory))
		{
			Directory.Delete(playerDirectory, true);
		}
		if (File.Exists(packagePath))
		{
			File.Delete(packagePath);
		}
		Directory.CreateDirectory(outputDirectory);
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
		PlayerSettings.bundleVersion = DefaultVersion;
		PlayerSettings.productName = "Pitfall Recovered";
		PlayerSettings.companyName = "iHawksPro";
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = scenes,
			locationPathName = executablePath,
			target = BuildTarget.StandaloneWindows64,
			options = BuildOptions.None
		};
		BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
		if (buildReport.summary.result != BuildResult.Succeeded)
		{
			throw new InvalidOperationException("Windows build failed. See the Unity log for details.");
		}
		ZipFile.CreateFromDirectory(playerDirectory, packagePath, System.IO.Compression.CompressionLevel.Optimal, true);
		Console.WriteLine("WINDOWS_BUILD_PATH=" + executablePath);
		Console.WriteLine("WINDOWS_PACKAGE_PATH=" + packagePath);
		Console.WriteLine("WINDOWS_PACKAGE_SIZE=" + new FileInfo(packagePath).Length);
	}
}
