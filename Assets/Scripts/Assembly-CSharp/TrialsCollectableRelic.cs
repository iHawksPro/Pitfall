using UnityEngine;

public class TrialsCollectableRelic : MonoBehaviour
{
	public string m_trial;

	public int m_index;

	public GameObject m_relicPrefab;

	public GameObject m_collectPFX;

	private GameObject m_relicModel;

	public bool HasBeenCollected()
	{
		return TrialsDataManager.Instance.HasCollectedRelic(m_trial, m_index);
	}

	public void SpawnRelic()
	{
		m_relicModel = (GameObject)Object.Instantiate(m_relicPrefab);
		m_relicModel.transform.parent = base.gameObject.transform;
		m_relicModel.transform.localPosition = Vector3.zero;
	}

	private void Update()
	{
		if (!(m_relicModel == null) && !(GameController.Instance == null) && GameController.Instance.IsPlayingTrialsMode)
		{
			PlayerController playerController = PlayerController.Instance();
			if (playerController != null && Collect(playerController, playerController.GetCoinCollectionRange()))
			{
				OnCollected();
			}
		}
	}

	public bool Collect(PlayerController player, float collectionDistance)
	{
		bool result = false;
		float sqrMagnitude = (base.transform.position - PickupController.Instance().PlayerAnimatedTransform.position).sqrMagnitude;
		if (sqrMagnitude < collectionDistance * collectionDistance)
		{
			result = true;
		}
		return result;
	}

	public void OnCollected()
	{
		TrialsDataManager.Instance.CollectRelic(m_trial, m_index);
		if (m_collectPFX != null)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(m_collectPFX, base.transform.position, Quaternion.identity);
		}
		MenuSFX.Instance.Play2D("RelicGet");
		if (m_relicModel != null)
		{
			Object.Destroy(m_relicModel);
			m_relicModel = null;
		}
		Object.Destroy(base.gameObject);
	}
}
