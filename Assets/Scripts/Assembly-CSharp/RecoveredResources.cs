using System;
using UnityEngine;

public static class RecoveredResources
{
	public static T Load<T>(string path) where T : UnityEngine.Object
	{
		string normalizedPath = Normalize(path);
		if (string.IsNullOrEmpty(normalizedPath))
		{
			return (T)null;
		}
		T val = Resources.Load<T>(normalizedPath);
		if (val != null)
		{
			return val;
		}
		string pathWithLowercaseDirectories = LowercaseDirectories(normalizedPath);
		if (!string.Equals(pathWithLowercaseDirectories, normalizedPath, StringComparison.Ordinal))
		{
			val = Resources.Load<T>(pathWithLowercaseDirectories);
			if (val != null)
			{
				return val;
			}
		}
		return (T)null;
	}

	private static string Normalize(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return string.Empty;
		}
		return path.Replace('\\', '/').Trim('/');
	}

	private static string LowercaseDirectories(string path)
	{
		string[] array = path.Split('/');
		for (int i = 0; i < array.Length - 1; i++)
		{
			array[i] = array[i].ToLowerInvariant();
		}
		return string.Join("/", array);
	}
}
