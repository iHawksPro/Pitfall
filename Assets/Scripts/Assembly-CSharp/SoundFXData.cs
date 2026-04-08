using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundFXData
{
	public enum SoundChoiceBehaviour
	{
		Sequential = 0,
		Random = 1,
		RandomNoRepeat = 2
	}

	public SoundChoiceBehaviour m_nextSoundChoice = SoundChoiceBehaviour.Random;

	public List<AudioClip> m_audioSourceData;

	public bool m_bypassEffects;

	public bool m_loop;

	public int m_priority = 128;

	public float m_volume = 1f;

	public float m_pitch = 1f;

	public int m_maxInstances = 1;

	public float m_timeBetweenDuplicatePlay = 0.1f;

	public float m_panLevel = 1f;

	public float m_spread;

	public float m_dopplerLevel;

	public float m_minDistance = 5f;

	public float m_maxDistance = 50f;

	public AudioRolloffMode m_rollOffMode = AudioRolloffMode.Linear;

	public float m_pan2D;

	public void Play(GameObject go)
	{
		SoundManager.Instance.Play(this, go);
	}

	public void Play2D()
	{
		SoundManager.Instance.Play2D(this);
	}
}
