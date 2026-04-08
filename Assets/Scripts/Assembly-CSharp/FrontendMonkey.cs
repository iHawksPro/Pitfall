using System;
using System.Collections.Generic;
using UnityEngine;

public class FrontendMonkey : MonoBehaviour
{
	public enum MonkeyAnim
	{
		MA_TITLE_IDLE = 0,
		MA_VOLCANO_REACT = 1,
		MA_TITLE_TO_GAME = 2,
		MA_TITLE_IDLE_BREAK_0 = 3,
		MA_TITLE_IDLE_BREAK_1 = 4,
		MA_TITLE_IDLE_BREAK_2 = 5,
		MA_TITLE_IDLE_BREAK_3 = 6,
		MA_TITLE_TAP_0 = 7,
		MA_TITLE_TAP_1 = 8,
		MA_TITLE_TAP_2 = 9
	}

	public GameObject m_offScreenPos;

	public float m_idleBreakTime = 5f;

	[HideInInspector]
	public GameObject m_monkeyModel;

	private bool m_started;

	public SoundFXData m_screechSfx;

	public SoundFXData m_scaredSfx;

	public SoundFXData m_tapSfx;

	private string[] m_animNames;

	private void Start()
	{
		EnsureInitialized();
	}

	private static string[] GetAnimNames(Animation anim)
	{
		List<string> list = new List<string>();
		if (anim == null)
		{
			return list.ToArray();
		}
		foreach (AnimationState item in anim)
		{
			list.Add(item.name);
		}
		return list.ToArray();
	}

	private void PlayAnim(MonkeyAnim anim, QueueMode queueMode)
	{
		PlayAnim(anim, queueMode, false);
	}

	private void PlayAnim(MonkeyAnim anim, QueueMode queueMode, bool NoTransition)
	{
		Animation monkeyAnimation = GetMonkeyAnimation();
		string animName = GetAnimName(anim);
		if (monkeyAnimation != null && !string.IsNullOrEmpty(animName) && monkeyAnimation.GetClip(animName) != null)
		{
			if (NoTransition)
			{
				monkeyAnimation.Play(animName, PlayMode.StopAll);
			}
			else
			{
				monkeyAnimation.CrossFadeQueued(animName, 0.3f, queueMode);
			}
		}
	}

	public AnimationClip GetAnim(MonkeyAnim anim)
	{
		Animation monkeyAnimation = GetMonkeyAnimation();
		string animName = GetAnimName(anim);
		if (monkeyAnimation != null && !string.IsNullOrEmpty(animName))
		{
			return monkeyAnimation.GetClip(animName);
		}
		return null;
	}

	public float GetAnimLength(MonkeyAnim anim)
	{
		AnimationClip anim2 = GetAnim(anim);
		if (anim2 == null)
		{
			return 0f;
		}
		return anim2.length;
	}

	public float IpadToTitleLen()
	{
		EnsureInitialized();
		return GetAnimLength(MonkeyAnim.MA_VOLCANO_REACT);
	}

	public float IpadToTitle()
	{
		EnsureInitialized();
		if (RecoveredCompatibility.IsAndroidRuntime)
		{
			EnsureRecoveredTitleVisible();
			return 0f;
		}
		float animLength = GetAnimLength(MonkeyAnim.MA_VOLCANO_REACT);
		if (animLength <= 0f)
		{
			return 0f;
		}
		PlayAnim(MonkeyAnim.MA_VOLCANO_REACT, QueueMode.PlayNow, true);
		PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.CompleteOthers);
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
		return animLength;
	}

	public void RunOnScreen()
	{
		EnsureRecoveredTitleVisible();
		if (m_monkeyModel == null)
		{
			return;
		}
		if (RecoveredCompatibility.IsAndroidRuntime || GetMonkeyAnimation() == null || GetAnim(MonkeyAnim.MA_TITLE_IDLE) == null)
		{
			SnapToTitlePose();
			PlayTitleIdleFallback();
			CancelInvoke("IdleBreak");
			if (GetAnim(MonkeyAnim.MA_TITLE_IDLE) != null)
			{
				InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
			}
			return;
		}
		PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.PlayNow, true);
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
	}

	private void IdleBreak()
	{
		Animation monkeyAnimation = GetMonkeyAnimation();
		if (monkeyAnimation != null && m_animNames != null && m_animNames.Length > 0 && monkeyAnimation.IsPlaying(m_animNames[0]))
		{
			int num = 3;
			int num2 = 7;
			MonkeyAnim anim = (MonkeyAnim)UnityEngine.Random.Range(num, num + num2);
			PlayAnim(anim, QueueMode.PlayNow);
			PlayAnim(MonkeyAnim.MA_TITLE_IDLE, QueueMode.CompleteOthers);
		}
	}

	public void TitleToGame()
	{
		if (m_monkeyModel == null)
		{
			return;
		}
		PlayAnim(MonkeyAnim.MA_TITLE_TO_GAME, QueueMode.PlayNow);
		CancelInvoke("IdleBreak");
	}

	public void StopAllAnims()
	{
		CancelInvoke("IdleBreak");
		Animation monkeyAnimation = GetMonkeyAnimation();
		if (monkeyAnimation != null)
		{
			monkeyAnimation.Stop();
		}
	}

	private Animation GetMonkeyAnimation()
	{
		if (m_monkeyModel == null)
		{
			return null;
		}
		return m_monkeyModel.GetComponent<Animation>();
	}

	private string GetAnimName(MonkeyAnim anim)
	{
		if (m_animNames == null)
		{
			return null;
		}
		int num = (int)anim;
		if (num < 0 || num >= m_animNames.Length)
		{
			return null;
		}
		return m_animNames[num];
	}

	private void SnapToTitlePose()
	{
		if (m_monkeyModel == null)
		{
			return;
		}
		m_monkeyModel.transform.position = base.transform.position;
		m_monkeyModel.transform.rotation = base.transform.rotation;
	}

	private void PlayTitleIdleFallback()
	{
		Animation monkeyAnimation = GetMonkeyAnimation();
		AnimationClip anim = GetAnim(MonkeyAnim.MA_TITLE_IDLE);
		if (monkeyAnimation != null && anim != null)
		{
			monkeyAnimation.Play(anim.name, PlayMode.StopAll);
		}
	}

	public void EnsureRecoveredTitleVisible()
	{
		EnsureInitialized();
		if (m_monkeyModel == null)
		{
			TryInstantiateMonkeyModel();
		}
		if (m_monkeyModel == null)
		{
			return;
		}
		m_monkeyModel.SetActiveRecursively(true);
		EnableHierarchyRenderers(m_monkeyModel);
		SnapToTitlePose();
		PlayTitleIdleFallback();
	}

	private void EnsureInitialized()
	{
		if (m_started)
		{
			return;
		}
		m_started = true;
		TryInstantiateMonkeyModel();
	}

	private void TryInstantiateMonkeyModel()
	{
		bool flag = TBFUtils.Is256mbDevice();
		string path = ((!flag) ? "Frontend/Monkey_FrontEnd_Prefab" : "Frontend/Monkey_FrontEnd_Lite_Prefab");
		if (m_monkeyModel != null)
		{
			return;
		}
		try
		{
			GameObject gameObject = RecoveredResources.Load<GameObject>(path);
			if (gameObject == null)
			{
				Debug.LogWarning("Recovered monkey fallback: missing prefab " + path);
				return;
			}
			m_monkeyModel = (GameObject)UnityEngine.Object.Instantiate(gameObject);
			Transform transform = ((m_offScreenPos != null) ? m_offScreenPos.transform : base.transform);
			m_monkeyModel.transform.position = transform.position;
			m_monkeyModel.transform.rotation = transform.rotation;
			m_monkeyModel.transform.parent = base.transform;
			Animation monkeyAnimation = GetMonkeyAnimation();
			m_animNames = GetAnimNames(monkeyAnimation);
			if (monkeyAnimation == null)
			{
				Debug.LogWarning("Recovered monkey fallback: missing Animation component.");
				SnapToTitlePose();
			}
			else if (!flag)
			{
				FrontendMonkeyAnimEventHandler frontendMonkeyAnimEventHandler = m_monkeyModel.AddComponent<FrontendMonkeyAnimEventHandler>();
				frontendMonkeyAnimEventHandler.SetupAnimEvents(this);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Recovered monkey startup fallback: " + ex.Message);
		}
	}

	private static void EnableHierarchyRenderers(GameObject root)
	{
		Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
		Animation[] componentsInChildren2 = root.GetComponentsInChildren<Animation>(true);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].enabled = true;
		}
	}
}
