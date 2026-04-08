using System;
using UnityEditor;
using UnityEngine;

public static class LegacyAudioRepair
{
	private const string SessionKey = "Pitfall.LegacyAudioRepair.v2";

	[InitializeOnLoadMethod]
	private static void ScheduleRepair()
	{
		if (SessionState.GetBool(SessionKey, false))
		{
			return;
		}
		SessionState.SetBool(SessionKey, true);
		EditorApplication.delayCall += RepairLegacyAudio;
	}

	[MenuItem("Tools/Pitfall/Repair Legacy Audio Imports")]
	public static void RepairLegacyAudio()
	{
		int changedCount = 0;
		string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/AudioClip" });
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;
			if (importer == null)
			{
				continue;
			}
			AudioImporterSampleSettings settings = importer.defaultSampleSettings;
			bool changed = false;
			AudioClipLoadType desiredLoadType = AudioClipLoadType.DecompressOnLoad;
			if (settings.loadType != desiredLoadType)
			{
				settings.loadType = desiredLoadType;
				changed = true;
			}
			float targetQuality = (!IsCompressedFormat(path)) ? 1f : 0.85f;
			if (Math.Abs(settings.quality - targetQuality) > 0.001f)
			{
				settings.quality = targetQuality;
				changed = true;
			}
			if (settings.sampleRateSetting != AudioSampleRateSetting.PreserveSampleRate)
			{
				settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
				changed = true;
			}
			if (importer.loadInBackground)
			{
				importer.loadInBackground = false;
				changed = true;
			}
			if (!settings.preloadAudioData)
			{
				settings.preloadAudioData = true;
				changed = true;
			}
			if (!changed)
			{
				continue;
			}
			importer.defaultSampleSettings = settings;
			importer.SaveAndReimport();
			changedCount++;
		}
		if (changedCount > 0)
		{
			Debug.Log("LegacyAudioRepair updated " + changedCount + " audio clips for Unity 6 playback.");
		}
	}

	private static bool IsCompressedFormat(string path)
	{
		string lowerPath = path.ToLowerInvariant();
		return lowerPath.EndsWith(".mp3");
	}
}
