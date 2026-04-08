using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SFXBank : SingletonMonoBehaviour
{
	private Dictionary<string, SoundFXData> mSfxDictionary;

	private void BuildDictionary()
	{
		if (mSfxDictionary != null)
		{
			return;
		}
		mSfxDictionary = new Dictionary<string, SoundFXData>();
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(SoundFXData))
			{
				mSfxDictionary.Add(fieldInfo.Name, fieldInfo.GetValue(this) as SoundFXData);
			}
		}
	}

	public bool IsAnySoundPlaying()
	{
		if (mSfxDictionary == null)
		{
			BuildDictionary();
		}
		bool result = false;
		foreach (string key in mSfxDictionary.Keys)
		{
			SoundFXData fxData = mSfxDictionary[key];
			if (SoundManager.Instance.IsPlaying(fxData))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public SoundFXData GetSFXDataFromName(string name)
	{
		if (mSfxDictionary == null)
		{
			BuildDictionary();
		}
		SoundFXData value = null;
		if (!mSfxDictionary.TryGetValue(name, out value))
		{
			Debug.Log(string.Format("Bank does not contain sfx: '{0}'", name));
		}
		return value;
	}

	public virtual void Play2D(string Name)
	{
		SoundFXData sFXDataFromName = GetSFXDataFromName(Name);
		if (sFXDataFromName != null)
		{
			SoundManager.Instance.Play2D(sFXDataFromName);
		}
	}

	public virtual void Play2D(string Name, float Delay)
	{
		SoundFXData sFXDataFromName = GetSFXDataFromName(Name);
		if (sFXDataFromName != null)
		{
			SoundManager.Instance.Play2D(sFXDataFromName, 1f, Delay);
		}
	}

	public virtual void Stop2D(string Name)
	{
		SoundFXData sFXDataFromName = GetSFXDataFromName(Name);
		if (sFXDataFromName != null)
		{
			SoundManager.Instance.Stop2D(sFXDataFromName);
		}
	}

	public virtual void Play(string Name, GameObject go)
	{
		SoundFXData sFXDataFromName = GetSFXDataFromName(Name);
		if (sFXDataFromName != null)
		{
			SoundManager.Instance.Play(sFXDataFromName, go);
		}
	}

	public virtual void Stop(string Name, GameObject go)
	{
		SoundFXData sFXDataFromName = GetSFXDataFromName(Name);
		if (sFXDataFromName != null)
		{
			SoundManager.Instance.Stop(sFXDataFromName, go);
		}
	}
}
