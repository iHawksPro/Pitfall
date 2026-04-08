using System.Collections;
using UnityEngine;

public class FrontendHarry : MonoBehaviour
{
	public HarryCostume[] m_costumes;

	public HarryIpad m_iPad;

	public GameObject m_offScreenPos;

	public GameObject m_titlePos;

	public float m_idleBreakTime = 6f;

	public Material m_superLockedMaterial;

	private HarryCostume m_currentCostume;

	private bool m_started;

	private int m_lastTitleIdle = -1;

	private GameObject m_currentModel;

	private Animation CurrentAnimation
	{
		get
		{
			if (m_currentModel == null)
			{
				return null;
			}
			return m_currentModel.GetComponent<Animation>();
		}
	}

	private void Start()
	{
		if (!m_started)
		{
			Costume currentCostumeType = SecureStorage.Instance.GetCurrentCostumeType();
			SetCostume(currentCostumeType);
			GameObject gameObject = (GameObject)Object.Instantiate(m_iPad.m_iPadPrefab);
			gameObject.transform.position = m_offScreenPos.transform.position;
			gameObject.transform.rotation = m_offScreenPos.transform.rotation;
			gameObject.transform.parent = base.transform;
			m_iPad.m_model = gameObject;
			m_started = true;
		}
	}

	private void WipeAnimations(GameObject go)
	{
		if (!(go.GetComponent<Animation>() == null))
		{
			WrapMode wrapMode = go.GetComponent<Animation>().wrapMode;
			bool playAutomatically = go.GetComponent<Animation>().playAutomatically;
			bool animatePhysics = go.GetComponent<Animation>().animatePhysics;
			AnimationCullingType cullingType = go.GetComponent<Animation>().cullingType;
			Object.DestroyImmediate(go.GetComponent<Animation>());
			Animation animation = go.AddComponent<Animation>();
			animation.wrapMode = wrapMode;
			animation.playAutomatically = playAutomatically;
			animation.animatePhysics = animatePhysics;
			animation.cullingType = cullingType;
		}
	}

	private void SetCostume(Costume CostumeType)
	{
		if (m_currentModel != null)
		{
			WipeAnimations(m_currentModel);
			Object.DestroyImmediate(m_currentModel);
			m_currentModel = null;
			if (TBFUtils.Is256mbDevice())
			{
				Resources.UnloadUnusedAssets();
			}
		}
		HarryCostume harryCostume = FindCostume(CostumeType);
		if (!TryInstantiateCostume(harryCostume))
		{
			harryCostume = FindCostume(Costume.None);
			if (!TryInstantiateCostume(harryCostume))
			{
				for (int i = 0; i < m_costumes.Length; i++)
				{
					if (TryInstantiateCostume(m_costumes[i]))
					{
						break;
					}
				}
			}
		}
		if (m_currentModel == null)
		{
			Debug.LogWarning("Recovered Harry fallback: unable to load any front-end costume.");
		}
	}

	private void PlayAnim(string animName, QueueMode queueMode)
	{
		PlayAnim(animName, queueMode, false);
	}

	private void PlayAnim(string animName, QueueMode queueMode, bool NoTransition)
	{
		Animation currentAnimation = CurrentAnimation;
		if (currentAnimation == null || currentAnimation.GetClip(animName) == null)
		{
			return;
		}
		if (NoTransition)
		{
			currentAnimation.Play(animName, PlayMode.StopAll);
		}
		else
		{
			currentAnimation.CrossFadeQueued(animName, 0.3f, queueMode);
		}
	}

	public float IpadToTitleLen()
	{
		if (!m_started)
		{
			Start();
		}
		if (m_currentModel == null || CurrentAnimation == null || CurrentAnimation.GetClip("IpadToTitle") == null)
		{
			return 0f;
		}
		return CurrentAnimation.GetClip("IpadToTitle").length;
	}

	public float IpadToTitle()
	{
		if (!m_started)
		{
			Start();
		}
		if (RecoveredCompatibility.IsAndroidRuntime)
		{
			RunOnScreen();
			return 0f;
		}
		if (m_currentModel == null || CurrentAnimation == null || CurrentAnimation.GetClip("IpadToTitle") == null)
		{
			return 0f;
		}
		float length = CurrentAnimation.GetClip("IpadToTitle").length;
		PlayAnim("IpadToTitle", QueueMode.PlayNow, true);
		PlayAnim("TitleIdle", QueueMode.CompleteOthers);
		float length2 = m_iPad.m_model.GetComponent<Animation>().clip.length;
		m_iPad.m_model.SetActiveRecursively(true);
		m_iPad.m_model.GetComponent<Animation>().Play();
		StartCoroutine(HideIpad(length2));
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
		return length;
	}

	private IEnumerator HideIpad(float animLength)
	{
		yield return new WaitForSeconds(animLength);
		m_iPad.m_model.SetActiveRecursively(false);
	}

	public void RunOnScreen()
	{
		if (!m_started)
		{
			Start();
		}
		if (m_currentModel == null)
		{
			return;
		}
		Animation currentAnimation = CurrentAnimation;
		if (RecoveredCompatibility.IsAndroidRuntime || currentAnimation == null || currentAnimation.GetClip("OffScreenToTitle") == null)
		{
			SnapToTitlePose();
			PlayTitleIdleFallback();
			CancelInvoke("IdleBreak");
			if (currentAnimation != null && currentAnimation.GetClip("TitleIdle") != null)
			{
				InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
			}
			return;
		}
		PlayAnim("OffScreenToTitle", QueueMode.PlayNow, true);
		PlayAnim("TitleIdle", QueueMode.CompleteOthers);
		InvokeRepeating("IdleBreak", m_idleBreakTime, m_idleBreakTime);
	}

	private void IdleBreak()
	{
		Animation currentAnimation = CurrentAnimation;
		if (currentAnimation != null && currentAnimation.IsPlaying("TitleIdle"))
		{
			int num;
			do
			{
				num = Random.Range(0, 4);
			}
			while (num == m_lastTitleIdle);
			m_lastTitleIdle = num;
			string animName = "TitleIdleBreak_" + num;
			PlayAnim(animName, QueueMode.PlayNow);
			PlayAnim("TitleIdle", QueueMode.CompleteOthers);
		}
	}

	public void ResultsToShop()
	{
		if (m_currentModel == null)
		{
			return;
		}
		PlayAnim("TitleToShop", QueueMode.PlayNow, true);
		PlayAnim("ShopIdle", QueueMode.CompleteOthers);
	}

	public void TitleToShop()
	{
		if (m_currentModel == null)
		{
			return;
		}
		PlayAnim("TitleToShop", QueueMode.PlayNow);
		PlayAnim("ShopIdle", QueueMode.CompleteOthers);
	}

	public void ShopToTitle()
	{
		if (m_currentModel == null)
		{
			return;
		}
		PlayAnim("ShopToTitle", QueueMode.PlayNow);
		PlayAnim("TitleIdle", QueueMode.CompleteOthers);
	}

	public void ChangeCostume(Costume newCostume)
	{
		if (m_currentCostume != null && newCostume != m_currentCostume.m_costume)
		{
			StartCoroutine(ChangeCostumeCoroutine(newCostume));
		}
	}

	private IEnumerator ChangeCostumeCoroutine(Costume newCostume)
	{
		PlayAnim("ShopExitRight", QueueMode.PlayNow);
		yield return new WaitForSeconds(0.5f);
		SetCostume(newCostume);
		if (m_currentCostume != null && m_currentCostume.m_shopAppearSfx != null)
		{
			m_currentCostume.m_shopAppearSfx.Play2D();
		}
		PlayAnim("ShopEnterRight", QueueMode.PlayNow);
		PlayAnim("ShopIdle", QueueMode.CompleteOthers);
	}

	public void TitleToGame()
	{
		if (m_currentModel == null)
		{
			return;
		}
		PlayAnim("TitleToGame", QueueMode.PlayNow);
		CancelInvoke("IdleBreak");
	}

	public Transform GetHarryRootBone()
	{
		return PlayerHelper.SearchHierarchyForBone(m_currentModel.transform, "Bip001 Pelvis");
	}

	public float TitleToGameAnimLength()
	{
		if (m_currentModel == null || CurrentAnimation == null || CurrentAnimation.GetClip("TitleToGame") == null)
		{
			return 0f;
		}
		return CurrentAnimation.GetClip("TitleToGame").length;
	}

	public void StopAllAnims()
	{
		CancelInvoke("IdleBreak");
		if (CurrentAnimation != null)
		{
			CurrentAnimation.Stop();
		}
	}

	private HarryCostume FindCostume(Costume costumeType)
	{
		for (int i = 0; i < m_costumes.Length; i++)
		{
			if (m_costumes[i].m_costume == costumeType)
			{
				return m_costumes[i];
			}
		}
		return null;
	}

	private bool TryInstantiateCostume(HarryCostume costume)
	{
		if (costume == null || string.IsNullOrEmpty(costume.m_costumeResource))
		{
			return false;
		}
		GameObject gameObject = RecoveredResources.Load<GameObject>(costume.m_costumeResource);
		if (gameObject == null)
		{
			return false;
		}
		m_currentModel = (GameObject)Object.Instantiate(gameObject);
		Transform transform = ((m_offScreenPos != null) ? m_offScreenPos.transform : base.transform);
		m_currentModel.transform.position = transform.position;
		m_currentModel.transform.rotation = transform.rotation;
		m_currentModel.transform.parent = base.transform;
		if (costume.m_costume == Costume.Super && !TrialsDataManager.Instance.HaveCollectedAllRelics)
		{
			SkinnedMeshRenderer componentInChildren = m_currentModel.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.material = m_superLockedMaterial;
			}
		}
		m_currentCostume = costume;
		return true;
	}

	private void SnapToTitlePose()
	{
		if (m_currentModel == null)
		{
			return;
		}
		Transform transform = ((m_titlePos != null) ? m_titlePos.transform : base.transform);
		m_currentModel.transform.position = transform.position;
		m_currentModel.transform.rotation = transform.rotation;
	}

	private void PlayTitleIdleFallback()
	{
		Animation currentAnimation = CurrentAnimation;
		if (currentAnimation != null && currentAnimation.GetClip("TitleIdle") != null)
		{
			currentAnimation.Play("TitleIdle", PlayMode.StopAll);
		}
	}
}
