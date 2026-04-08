using UnityEngine;

public class FieldGuidePatchesPanel : StorePanel
{
	public GameObject m_scrollItemPrefab;

	public Achievements m_achievementsData;

	private void Awake()
	{
		PopulateList();
	}

	public void RePopulate()
	{
		PopulateList();
	}

	private void PopulateList()
	{
		int num = 0;
		m_scrollList.ClearList(true);
		int num2 = 0;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		float num3 = 434f;
		num3 = 362f;
		AchievementManager.Instance.SanityCheckUberAchievement();
		for (int i = 2; i < 3; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Achievements.AchievementData[] achievements = m_achievementsData.achievements;
				foreach (Achievements.AchievementData achievementData in achievements)
				{
					int num4 = 0;
					if (!AchievementManager.Instance.IsOneShot(achievementData.Identifier))
					{
						num4 = 1;
					}
					if ((!(achievementData.Identifier == "accum.complete") || Application.platform == RuntimePlatform.IPhonePlayer) && (!(achievementData.Identifier == "accum.completedroid") || Application.platform != RuntimePlatform.IPhonePlayer) && !AchievementManager.Instance.IsDistinction(achievementData.Identifier) && j == num4 && (i == achievementData.Version || (i <= 2 && achievementData.Version <= i)))
					{
						if (num2 == 0)
						{
							gameObject = new GameObject("ProductContainer");
							gameObject.AddComponent<UIListItemContainer>();
						}
						gameObject2 = Object.Instantiate(m_scrollItemPrefab) as GameObject;
						PatchItem component = gameObject2.GetComponent<PatchItem>();
						if (component == null)
						{
							Debug.LogError("Couldn't get product item");
						}
						component.PopulateItem(achievementData);
						gameObject2.transform.localPosition = new Vector3((float)(num2 - 1) * num3, 0f, 0f);
						gameObject2.transform.parent = gameObject.transform;
						if (num == 10 || num == 11 || num == 12)
						{
							gameObject2.transform.localPosition = new Vector3(gameObject2.transform.localPosition.x + num3 * 0.5f, gameObject2.transform.localPosition.y, gameObject2.transform.localPosition.z);
						}
						if (num == 13)
						{
							gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
						}
						num2++;
						if (num2 == 3 || (num == 10 && num2 == 2) || (num == 11 && num2 == 2) || (num == 12 && num2 == 2))
						{
							m_scrollList.AddItem(gameObject);
							num2 = 0;
							num++;
						}
					}
				}
			}
		}
		if (num2 != 0)
		{
			m_scrollList.AddItem(gameObject);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
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
