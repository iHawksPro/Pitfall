using UnityEngine;

public class AnimationSoundPlayer : MonoBehaviour
{
	public SFXBank m_sfxBank;

	public string[] m_sfxNames;

	public bool m_playAs3D;

	private int GetSfxIndex(string SfxName)
	{
		int result = -1;
		if (m_sfxNames != null)
		{
			int num = int.Parse(SfxName.Substring(SfxName.Length - 2)) - 1;
			if (num >= 0 && num < m_sfxNames.Length)
			{
				result = num;
			}
		}
		return result;
	}

	private void KeyOn(string SfxName)
	{
		int sfxIndex = GetSfxIndex(SfxName);
		if (sfxIndex == -1)
		{
			return;
		}
		SoundFXData sFXDataFromName = m_sfxBank.GetSFXDataFromName(m_sfxNames[sfxIndex]);
		if (sFXDataFromName != null)
		{
			if (m_playAs3D)
			{
				m_sfxBank.Play(m_sfxNames[sfxIndex], base.gameObject);
			}
			else
			{
				m_sfxBank.Play2D(m_sfxNames[sfxIndex]);
			}
		}
	}

	private void KeyOff(string SfxName)
	{
		int sfxIndex = GetSfxIndex(SfxName);
		if (sfxIndex == -1)
		{
			return;
		}
		SoundFXData sFXDataFromName = m_sfxBank.GetSFXDataFromName(m_sfxNames[sfxIndex]);
		if (sFXDataFromName != null)
		{
			if (m_playAs3D)
			{
				m_sfxBank.Stop(m_sfxNames[sfxIndex], base.gameObject);
			}
			else
			{
				m_sfxBank.Stop2D(m_sfxNames[sfxIndex]);
			}
		}
	}
}
