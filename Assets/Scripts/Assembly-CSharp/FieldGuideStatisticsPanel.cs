using UnityEngine;

public class FieldGuideStatisticsPanel : StorePanel
{
	public GameObject m_scrollItemPrefab;

	public GameObject m_scrollItemPrefab2;

	public Stats m_StatsData;

	private void Awake()
	{
	}

	public void RePopulate()
	{
		PopulateProductList();
	}

	private void PopulateProductList()
	{
		int num = 0;
		m_scrollList.ClearList(true);
		GameObject gameObject = null;
		Stats.StatData[] stats = m_StatsData.stats;
		foreach (Stats.StatData data in stats)
		{
			gameObject = new GameObject("ProductContainer");
			gameObject.AddComponent<UIListItemContainer>();
			GameObject gameObject2 = null;
			if (num % 2 == 0)
			{
				gameObject2 = Object.Instantiate(m_scrollItemPrefab) as GameObject;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 35f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
			}
			else
			{
				gameObject2 = Object.Instantiate(m_scrollItemPrefab2) as GameObject;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x - 35f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
			}
			StatBar component = gameObject2.GetComponent<StatBar>();
			component.PopulateItem(data);
			gameObject2.transform.parent = gameObject.transform;
			m_scrollList.AddItem(gameObject);
			num++;
		}
	}

	private void OnScrollChanged(float newScrollPos)
	{
		if (newScrollPos >= 1f)
		{
			SwrveEventsUI.FieldViewedAllPanel();
		}
	}

	protected override void OnEnable()
	{
		m_scrollList.AddInputScrollChangeDelegate(OnScrollChanged);
	}

	protected override void OnDisable()
	{
		m_scrollList.RemoveInputScrollChangeDelegate(OnScrollChanged);
	}
}
