using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundFX
{
	public SoundFXData m_sfxData;

	private int m_lastSoundIndex = -1;

	private LinkedList<AudioSource> m_playingSources = new LinkedList<AudioSource>();

	private HashSet<AudioSource> m_pausedSounds = new HashSet<AudioSource>();

	private float m_lastPlayCall = float.NegativeInfinity;

	private bool m_isPaused;

	public bool IsPlaying
	{
		get
		{
			DestroyFinishedAudioSources();
			return m_playingSources.Count != 0;
		}
	}

	private AudioClip ChooseSfxDataToPlay()
	{
		int num = 0;
		int count = m_sfxData.m_audioSourceData.Count;
		switch (m_sfxData.m_nextSoundChoice)
		{
		case SoundFXData.SoundChoiceBehaviour.Random:
			num = UnityEngine.Random.Range(0, count);
			break;
		case SoundFXData.SoundChoiceBehaviour.RandomNoRepeat:
			if (count <= 1)
			{
				break;
			}
			if (m_lastSoundIndex == -1)
			{
				num = UnityEngine.Random.Range(0, count);
				break;
			}
			do
			{
				num = UnityEngine.Random.Range(0, count);
			}
			while (num == m_lastSoundIndex);
			break;
		case SoundFXData.SoundChoiceBehaviour.Sequential:
			num = (m_lastSoundIndex + 1) % count;
			break;
		}
		AudioClip audioClip = null;
		if (num < count)
		{
			audioClip = m_sfxData.m_audioSourceData[num];
			if (audioClip != null)
			{
				m_lastSoundIndex = num;
			}
		}
		return audioClip;
	}

	public GameObject Play(GameObject source, bool createSpeaker, float pitchOverride)
	{
		return Play(source, createSpeaker, pitchOverride, false);
	}

	public GameObject Play(GameObject source, bool createSpeaker, float pitchOverride, bool force2D)
	{
		if (m_sfxData.m_audioSourceData.Count == 0)
		{
			return null;
		}
		DestroyFinishedAudioSources();
		if (m_playingSources.Count > m_sfxData.m_maxInstances)
		{
			return null;
		}
		if (m_sfxData.m_timeBetweenDuplicatePlay > 0f)
		{
			if (m_lastPlayCall + m_sfxData.m_timeBetweenDuplicatePlay > Time.time)
			{
				return null;
			}
			m_lastPlayCall = Time.time;
		}
		AudioSource audioSource = GetAudioSource(source, createSpeaker);
		if (audioSource == null)
		{
			return null;
		}
		AudioClip audioClip = ChooseSfxDataToPlay();
		InitAudioSource(audioSource, audioClip, force2D);
		if (audioSource.clip == null)
		{
			return null;
		}
		if (pitchOverride != 1f)
		{
			ApplyPitch(audioSource, pitchOverride);
		}
		float length = audioSource.clip.length;
		audioSource.Play();
		GameObject gameObject = ((!createSpeaker) ? source : audioSource.gameObject);
		if (!audioSource.loop)
		{
			UnityEngine.Object obj = ((!createSpeaker) ? ((UnityEngine.Object)audioSource) : ((UnityEngine.Object)gameObject));
			UnityEngine.Object.Destroy(obj, length);
		}
		if (createSpeaker)
		{
			gameObject.name = string.Format("Speaker - {0} - {1}", audioClip.name, source.name);
		}
		return gameObject;
	}

	public void Stop(GameObject source)
	{
		foreach (AudioSource playingSource in m_playingSources)
		{
			if (playingSource != null && WasAudioSourceStartedForGameObject(playingSource, source))
			{
				m_pausedSounds.Remove(playingSource);
				playingSource.Stop();
			}
		}
		DestroyFinishedAudioSources();
	}

	public void StopAll()
	{
		foreach (AudioSource playingSource in m_playingSources)
		{
			if (playingSource != null)
			{
				playingSource.Stop();
			}
		}
		m_pausedSounds.Clear();
		m_isPaused = false;
		DestroyFinishedAudioSources();
	}

	private bool WasAudioSourceStartedForGameObject(AudioSource audioSrc, GameObject gameObj)
	{
		if (audioSrc.gameObject == gameObj)
		{
			return true;
		}
		CopyPosition component = audioSrc.gameObject.GetComponent<CopyPosition>();
		if (component != null && component.SrcObject == gameObj)
		{
			return true;
		}
		return false;
	}

	private void InitAudioSource(AudioSource audioSrc, AudioClip clip, bool force2D)
	{
		audioSrc.clip = clip;
		audioSrc.mute = false;
		audioSrc.bypassEffects = m_sfxData.m_bypassEffects;
		audioSrc.loop = m_sfxData.m_loop;
		audioSrc.playOnAwake = false;
		audioSrc.priority = m_sfxData.m_priority;
		audioSrc.volume = m_sfxData.m_volume;
		audioSrc.spatialBlend = ((!force2D) ? m_sfxData.m_panLevel : 0f);
		audioSrc.spread = ((!force2D) ? m_sfxData.m_spread : 0f);
		audioSrc.dopplerLevel = ((!force2D) ? m_sfxData.m_dopplerLevel : 0f);
		audioSrc.minDistance = ((!force2D) ? m_sfxData.m_minDistance : 1f);
		audioSrc.maxDistance = ((!force2D) ? m_sfxData.m_maxDistance : 1f);
		audioSrc.rolloffMode = ((!force2D) ? m_sfxData.m_rollOffMode : AudioRolloffMode.Logarithmic);
		audioSrc.panStereo = m_sfxData.m_pan2D;
		ApplyPitch(audioSrc, m_sfxData.m_pitch);
	}

	private void ApplyPitch(AudioSource audioSrc, float requestedPitch)
	{
		if (audioSrc == null)
		{
			return;
		}
		if (requestedPitch >= 0f || audioSrc.clip == null)
		{
			audioSrc.pitch = requestedPitch;
			return;
		}
		if (audioSrc.clip.loadType == AudioClipLoadType.DecompressOnLoad)
		{
			audioSrc.pitch = requestedPitch;
			return;
		}
		audioSrc.pitch = Mathf.Abs(requestedPitch);
	}

	private void DestroyFinishedAudioSources()
	{
		LinkedListNode<AudioSource> linkedListNode = m_playingSources.First;
		while (linkedListNode != null)
		{
			LinkedListNode<AudioSource> next = linkedListNode.Next;
			AudioSource value = linkedListNode.Value;
			if (value == null || (!value.isPlaying && !m_isPaused))
			{
				if (value != null)
				{
					if (value.loop && value.gameObject != null && value.gameObject.name.StartsWith("Speaker - "))
					{
						UnityEngine.Object.Destroy(value.gameObject);
					}
					else
					{
						UnityEngine.Object.Destroy(value);
					}
				}
				m_playingSources.Remove(linkedListNode);
			}
			linkedListNode = next;
		}
	}

	private AudioSource GetAudioSource(GameObject source, bool createSpeaker)
	{
		DestroyFinishedAudioSources();
		GameObject gameObject = source;
		if (createSpeaker)
		{
			GameObject gameObject2 = new GameObject("Speaker");
			CopyPosition copyPosition = gameObject2.AddComponent<CopyPosition>();
			copyPosition.SrcObject = source;
			gameObject2.transform.position = source.transform.position;
			gameObject = gameObject2;
		}
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		m_playingSources.AddLast(audioSource);
		return audioSource;
	}

	public float GetVolume(GameObject source)
	{
		foreach (AudioSource playingSource in m_playingSources)
		{
			if (playingSource != null && WasAudioSourceStartedForGameObject(playingSource, source))
			{
				return playingSource.volume;
			}
		}
		return 0f;
	}

	public void SetVolume(GameObject source, float volume)
	{
		foreach (AudioSource playingSource in m_playingSources)
		{
			if (playingSource != null && WasAudioSourceStartedForGameObject(playingSource, source))
			{
				playingSource.volume = volume;
			}
		}
	}

	public void Pause()
	{
		foreach (AudioSource playingSource in m_playingSources)
		{
			if (playingSource != null)
			{
				playingSource.Pause();
				m_pausedSounds.Add(playingSource);
			}
		}
		m_isPaused = true;
		DestroyFinishedAudioSources();
	}

	public void UnPause()
	{
		foreach (AudioSource pausedSound in m_pausedSounds)
		{
			if (pausedSound != null)
			{
				pausedSound.Play();
			}
		}
		m_pausedSounds.Clear();
		m_isPaused = false;
	}
}
