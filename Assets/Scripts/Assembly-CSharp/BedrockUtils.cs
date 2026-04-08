using System.Collections;
using UnityEngine;

public class BedrockUtils
{
	public const bool debugMode = false;

	public static string debugString;

	public static Bedrock.KeyValueArray Hash(params string[] args)
	{
		if (args.Length % 2 != 0)
		{
			Debug.LogError("Hash needs an even number of arguments");
			return default(Bedrock.KeyValueArray);
		}
		Bedrock.KeyValueArray result = default(Bedrock.KeyValueArray);
		result.size = args.Length / 2;
		result.pairs = new Bedrock.brKeyValuePair[result.size];
		for (int i = 0; i < args.Length - 1; i += 2)
		{
			result.pairs[i / 2].key = args[i];
			result.pairs[i / 2].val = args[i + 1];
		}
		return result;
	}

	public static Bedrock.KeyValueArray Hash(IDictionary dict)
	{
		Bedrock.KeyValueArray result = default(Bedrock.KeyValueArray);
		int count = dict.Keys.Count;
		result.size = count;
		result.pairs = new Bedrock.brKeyValuePair[result.size];
		int num = 0;
		foreach (string key in dict.Keys)
		{
			result.pairs[num].key = key;
			result.pairs[num].val = dict[key].ToString();
			num++;
		}
		return result;
	}
}
