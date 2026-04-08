using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicResultsContainerDisplay : MonoBehaviour
{
	private const float c_fSizeOfSprite = 100f;

	public UIStretchingPackedSprite m_Sprite;

	public GameObject m_Prefab;

	private List<GameObject> m_MyItems = new List<GameObject>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	protected void OnEnable()
	{
		if (m_Sprite == null)
		{
			return;
		}
		m_MyItems.Clear();
		if (GameController.Instance == null || GameController.Instance.m_TrialDataAtStart == null || GameController.Instance.m_CurrentTrialName == null)
		{
			return;
		}
		int numCollectables = GameController.Instance.m_TrialDataAtStart.m_srcData.m_numCollectables;
		TrialsDataManager.TrialState trialDataAtStart = GameController.Instance.m_TrialDataAtStart;
		TrialsDataManager.TrialState trialState = TrialsDataManager.Instance.FindTrialState(GameController.Instance.m_CurrentTrialName);
		if (trialState == null || trialDataAtStart == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < numCollectables; i++)
		{
			GameObject gameObject = Object.Instantiate(m_Prefab) as GameObject;
			gameObject.name = "Collectable" + i;
			Vector3 position = m_Sprite.m_Left.transform.position;
			position.x += (float)i * 100f + 100f;
			position.z -= 1f;
			gameObject.transform.position = position;
			RelicResultContainer component = gameObject.GetComponent<RelicResultContainer>();
			if (trialState.m_relicCollected[i])
			{
				component.m_NotFound.SetActiveRecursively(false);
				component.m_Found.SetActiveRecursively(true);
				if (!trialDataAtStart.m_relicCollected[i])
				{
					component.m_JustFound.SetActiveRecursively(true);
					num++;
					StartCoroutine(StampItem(component.m_Found, 0.9f * (float)num));
				}
				else
				{
					component.m_JustFound.SetActiveRecursively(false);
				}
			}
			else
			{
				component.m_Found.SetActiveRecursively(false);
				component.m_NotFound.SetActiveRecursively(true);
				component.m_JustFound.SetActiveRecursively(false);
			}
			gameObject.AddComponent<HudMovement>();
			m_MyItems.Add(gameObject);
		}
		Vector3 position2 = m_Sprite.m_Left.transform.position;
		position2.x += 100f * (float)numCollectables;
		m_Sprite.m_Right.transform.position = position2;
	}

	public IEnumerator StampItem(GameObject aObject, float fDelay)
	{
		while (!ResultsDisplay.Instance.ResultsFinished())
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForSeconds(fDelay);
		MenuSFX.Instance.Play2D("CheckPointPurchaseAvailable");
		aObject.PunchScale(new Vector3(0.75f, 0.75f, 0f), 0.5f, 0f);
	}

	protected void LateUpdate()
	{
		for (int i = 0; i < m_MyItems.Count; i++)
		{
			GameObject gameObject = m_MyItems[i];
			Vector3 position = m_Sprite.m_Left.transform.position;
			position.x += (float)i * 100f + 50f;
			position.z -= 1f;
			gameObject.transform.position = position;
		}
	}

	protected void OnDisable()
	{
		if (m_Sprite != null)
		{
			for (int i = 0; i < m_MyItems.Count; i++)
			{
				Object.Destroy(m_MyItems[i]);
			}
			m_MyItems.Clear();
		}
	}
}
