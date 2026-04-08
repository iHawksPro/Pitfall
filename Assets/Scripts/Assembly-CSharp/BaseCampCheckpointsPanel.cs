using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCampCheckpointsPanel : StorePanel
{
	private const float distBetween = 506f;

	private const float offset = 350f;

	public GameObject m_MapSectionPrefab;

	public GameObject m_HarryIconPrefab;

	public GameObject m_HarryMarkerPrefab;

	public GameObject m_FriendMarkerPrefab;

	private List<CheckpointItem> m_CheckPoints = new List<CheckpointItem>();

	private List<GameObject> m_FriendMarkers = new List<GameObject>();

	private GameObject m_HarryMarker;

	private UIListItemContainer m_Container;

	private GameObject m_HarryIcon;

	private static BaseCampCheckpointsPanel m_instance;

	public static BaseCampCheckpointsPanel Instance
	{
		get
		{
			return m_instance;
		}
	}

	public void Awake()
	{
		if (m_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			m_instance = this;
		}
		PopulateProductList();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void LateUpdate()
	{
		if (!base.gameObject.active)
		{
			return;
		}
		m_HarryIcon.GetComponent<Renderer>().enabled = m_HarryIcon.transform.position.y < 430f;
		if (m_HarryMarker.transform.position.y < 430f)
		{
			m_HarryMarker.transform.localPosition = new Vector3(100f, m_HarryMarker.transform.localPosition.y, m_HarryMarker.transform.localPosition.z);
		}
		else
		{
			m_HarryMarker.transform.localPosition = new Vector3(-2000f, m_HarryMarker.transform.localPosition.y, m_HarryMarker.transform.localPosition.z);
		}
		foreach (GameObject friendMarker in m_FriendMarkers)
		{
			if (friendMarker.transform.position.y < 430f)
			{
				friendMarker.transform.localPosition = new Vector3(-250f, friendMarker.transform.localPosition.y, friendMarker.transform.localPosition.z);
			}
			else
			{
				friendMarker.transform.localPosition = new Vector3(-2000f, friendMarker.transform.localPosition.y, friendMarker.transform.localPosition.z);
			}
		}
	}

	protected override void OnEnable()
	{
		RefreshProductListIcons();
		int num = (int)PlayerController.Instance().Score().DistanceTravelled() / 2000;
		if (BaseCampController.Instance.TutorialActive())
		{
			num = 1;
			m_scrollList.dragThreshold = 1000f;
		}
		else
		{
			m_scrollList.dragThreshold = UIManager.instance.dragThreshold;
		}
		num--;
		num = Mathf.Clamp(num, 0, m_scrollList.Count - 1);
		m_scrollList.ScrollToItem(m_scrollList.Count - num - 1, 0f);
		if (m_HarryIcon == null)
		{
			m_HarryIcon = UnityEngine.Object.Instantiate(m_HarryIconPrefab) as GameObject;
			m_HarryIcon.transform.parent = m_scrollList.transform.Find("Mover");
		}
		if (m_HarryIcon != null)
		{
			int b = 100000;
			if (CheckPointController.Instance() != null)
			{
				b = CheckPointController.Instance().FinalCheckPointDistance();
			}
			int num2 = Mathf.Min(SecureStorage.Instance.FurthestDistanceTravelled, b);
			float y = ConvertMapDistanceToYPos(num2);
			m_HarryIcon.transform.localPosition = new Vector3(0f, y, m_HarryIcon.transform.position.z);
		}
		if (m_HarryMarker == null)
		{
			m_HarryMarker = UnityEngine.Object.Instantiate(m_HarryMarkerPrefab) as GameObject;
			m_HarryMarker.transform.parent = m_scrollList.transform.Find("Mover");
		}
		if (m_HarryMarker != null)
		{
			int b2 = 100000;
			if (CheckPointController.Instance() != null)
			{
				b2 = CheckPointController.Instance().FinalCheckPointDistance();
			}
			int num3 = Mathf.Min(SecureStorage.Instance.FurthestDistanceTravelled, b2);
			float y2 = ConvertMapDistanceToYPos(num3);
			m_HarryMarker.transform.localPosition = new Vector3(100f, y2, m_HarryMarker.transform.position.z);
			StoreMarker component = m_HarryMarker.GetComponent<StoreMarker>();
			component.SetNameAndDist(Language.Get("S_BEST"), SecureStorage.Instance.FurthestDistanceTravelled);
		}
		if (m_FriendMarkers.Count == 0)
		{
			int num4 = 16;
			for (int i = 0; i < num4; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_FriendMarkerPrefab) as GameObject;
				gameObject.transform.parent = m_scrollList.transform.Find("Mover");
				m_FriendMarkers.Add(gameObject);
			}
		}
		int num5 = 0;
		if (ScoreRetriever.Instance().RequestLock())
		{
			float num6 = -2000f;
			for (int j = 0; j < ScoreRetriever.Instance().GetMaxDist(); j += ScoreRetriever.Instance().GetChunkSize())
			{
				bool valid = false;
				ScoreRetriever.FriendDetails friendDetails = ScoreRetriever.Instance().RetrieveFriendAtDistance(j, out valid);
				if (valid && num5 < m_FriendMarkers.Count && (float)friendDetails.distance - num6 > 1000f)
				{
					GameObject gameObject2 = m_FriendMarkers[num5];
					gameObject2.SetActiveRecursively(true);
					StoreMarker component2 = gameObject2.GetComponent<StoreMarker>();
					component2.SetNameAndDist(friendDetails.name, friendDetails.distance);
					float y3 = ConvertMapDistanceToYPos(friendDetails.distance);
					gameObject2.transform.localPosition = new Vector3(-250f, y3, gameObject2.transform.localPosition.z);
					num6 = friendDetails.distance;
					num5++;
				}
			}
		}
		for (int k = num5; k < m_FriendMarkers.Count; k++)
		{
			GameObject gameObject3 = m_FriendMarkers[k];
			gameObject3.SetActiveRecursively(false);
			gameObject3.transform.localPosition = new Vector3(-2000f, -200000f, gameObject3.transform.localPosition.z);
			StoreMarker component3 = gameObject3.GetComponent<StoreMarker>();
			component3.SetNameAndDist(string.Empty, -1000);
		}
		ScoreRetriever.Instance().ReleaseLock();
		m_scrollList.AddInputScrollChangeDelegate(OnScrollChanged);
	}

	private float ConvertMapDistanceToYPos(float dist)
	{
		float y = m_CheckPoints[0].transform.localPosition.y;
		if (dist < 4000f)
		{
			dist = ((!(dist <= 1300f)) ? ((dist - 1300f) * (20f / 27f) + 2000f) : (dist * 1.5384616f));
		}
		y -= ((float)CheckPointController.Instance().FinalCheckPointDistance() - dist) / 2000f * 506f;
		return y + 350f;
	}

	protected override void OnDisable()
	{
		m_scrollList.RemoveInputScrollChangeDelegate(OnScrollChanged);
	}

	private void OnScrollChanged(float newScrollPos)
	{
		if (newScrollPos <= 0f)
		{
			SwrveEventsUI.StoreViewedAllPanelProducts();
		}
	}

	private void PopulateProductList()
	{
		m_scrollList.ClearList(true);
		m_CheckPoints.Clear();
		Dictionary<int, BaseCampProducts.ProductData> dictionary = new Dictionary<int, BaseCampProducts.ProductData>();
		string text = "checkpoint.";
		int length = text.Length;
		BaseCampProducts products = StoreProductManager.Instance.GetProducts(BaseCampProducts.StoreCategory.BCP_CHECKPOINT);
		BaseCampProducts.ProductData[] products2 = products.products;
		foreach (BaseCampProducts.ProductData productData in products2)
		{
			string value = productData.Identifier.Substring(length, productData.Identifier.Length - length);
			int key = Convert.ToInt32(value);
			dictionary.Add(key, productData);
		}
		for (int num = CheckPointController.Instance().FinalCheckPointDistance(); num >= 0; num -= 2000)
		{
			GameObject gameObject = new GameObject("MapContainer");
			gameObject.AddComponent<UIListItemContainer>();
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_MapSectionPrefab) as GameObject;
			gameObject2.name = "CheckpointIcon" + num;
			CheckpointItem component = gameObject2.GetComponent<CheckpointItem>();
			m_CheckPoints.Add(component);
			component.SetDistanceText(num);
			BaseCampProducts.ProductData value2 = null;
			if (dictionary.TryGetValue(num, out value2))
			{
				component.PopulateItem(value2);
			}
			gameObject2.transform.parent = gameObject.transform;
			m_scrollList.AddItem(gameObject);
		}
		RefreshProductListIcons();
	}

	public void RefreshProductListIcons()
	{
		int num = 0;
		for (int num2 = CheckPointController.Instance().FinalCheckPointDistance(); num2 >= 0; num2 -= 2000)
		{
			int num3 = num2;
			if (num2 == 2000)
			{
				num3 = CheckPointController.Instance().FirstCheckPointDistance();
			}
			CheckPointController.CHECKPOINT_TYPE checkPointTypeAt = CheckPointController.Instance().GetCheckPointTypeAt(num3);
			CheckpointItem checkpointItem = m_CheckPoints[num];
			bool isLast = num2 == CheckPointController.Instance().FinalCheckPointDistance();
			bool isFirst = num2 == 0;
			checkpointItem.SetIconFromType(checkPointTypeAt, isLast, isFirst);
			num++;
		}
	}
}
