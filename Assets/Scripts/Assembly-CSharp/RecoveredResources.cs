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
		val = FindByName<T>(normalizedPath);
		if (val != null)
		{
			return val;
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

	private static T FindByName<T>(string path) where T : UnityEngine.Object
	{
		int num = path.LastIndexOf('/');
		string text = ((num < 0) ? string.Empty : path.Substring(0, num));
		string value = ((num < 0) ? path : path.Substring(num + 1));
		if (string.IsNullOrEmpty(value))
		{
			return (T)null;
		}
		T val = FindByNameInDirectory<T>(text, value);
		if (val != null)
		{
			return val;
		}
		string lowercaseDirectories = LowercaseDirectories(text);
		if (!string.Equals(lowercaseDirectories, text, StringComparison.Ordinal))
		{
			val = FindByNameInDirectory<T>(lowercaseDirectories, value);
			if (val != null)
			{
				return val;
			}
		}
		return (T)null;
	}

	private static T FindByNameInDirectory<T>(string directory, string assetName) where T : UnityEngine.Object
	{
		T[] array = Resources.LoadAll<T>(directory);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && string.Equals(array[i].name, assetName, StringComparison.OrdinalIgnoreCase))
			{
				return array[i];
			}
		}
		return (T)null;
	}
}
