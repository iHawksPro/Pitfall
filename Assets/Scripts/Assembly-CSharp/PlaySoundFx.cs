using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlaySoundFx : MonoBehaviour
{
	public SFXBank SoundBank;

	public string PlayFunction;

	public bool PlayAs3D = true;

	public bool PlayOnStart = true;

	public bool DestroyAfterPlay = true;

	private List<SoundFXData> m_cleanupSFX;

	private void Start()
	{
		m_cleanupSFX = new List<SoundFXData>();
		if (PlayOnStart)
		{
			Play();
		}
	}

	public static SoundFXData PlaySfxHelper(GameObject go, SFXBank bank, string function, bool playAs3d)
	{
		SoundFXData result = null;
		if (bank == null || function == null || function == string.Empty)
		{
			Debug.LogWarning("playing an empty sfx? - " + go.name);
		}
		else
		{
			PropertyInfo property = bank.GetType().GetProperty("Instance");
			if (property != null)
			{
				SFXBank sFXBank = property.GetValue(null, null) as SFXBank;
				SoundFXData sFXDataFromName = sFXBank.GetSFXDataFromName(function);
				if (playAs3d)
				{
					sFXDataFromName.Play(go);
					result = sFXDataFromName;
				}
				else
				{
					sFXDataFromName.Play2D();
				}
			}
		}
		return result;
	}

	public void Play(GameObject source, string sfxName)
	{
		SoundFXData soundFXData = PlaySfxHelper(source, SoundBank, sfxName, PlayAs3D);
		if (DestroyAfterPlay)
		{
			Object.Destroy(this);
		}
		else if (soundFXData != null)
		{
			m_cleanupSFX.Add(soundFXData);
		}
	}

	public void Play()
	{
		PieceDescriptor component = base.gameObject.GetComponent<PieceDescriptor>();
		if (component != null)
		{
			Play(component.EntryAnchor, PlayFunction);
		}
		else
		{
			Play(base.gameObject, PlayFunction);
		}
	}

	private void OnDestroy()
	{
		foreach (SoundFXData item in m_cleanupSFX)
		{
			SoundManager.Instance.Stop(item, base.gameObject);
		}
	}
}
