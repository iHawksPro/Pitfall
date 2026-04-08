using UnityEngine;

public class SlidingButtonBar : MonoBehaviour
{
	public AutoSpriteControlBase[] m_items;

	public UIStateToggleBtn m_toggleButton;

	public float m_slideTime = 0.3f;

	public string SlideInSfx;

	public string SlideOutSfx;

	private bool m_expanded;

	private Vector3 m_slidingStartPosition;

	private Vector3 m_slidingEndPosition;

	private Vector3 m_vCachedTabSize;

	private void Start()
	{
		m_toggleButton.scriptWithMethodToInvoke = this;
		m_toggleButton.methodToInvoke = "ToggleExpand";
		m_slidingStartPosition = new Vector3(base.gameObject.transform.localPosition.x, base.gameObject.transform.localPosition.y, base.gameObject.transform.localPosition.z);
		float num = CalculateSlideWidth();
		m_slidingEndPosition = new Vector3(m_slidingStartPosition.x - num, m_slidingStartPosition.y, m_slidingStartPosition.z);
		m_expanded = false;
		EnableItems(false);
		BoxCollider component = m_toggleButton.gameObject.GetComponent<BoxCollider>();
		m_vCachedTabSize = component.size;
		m_vCachedTabSize.x *= 1.5f;
		m_vCachedTabSize.y *= 1.3f;
		component.size = m_vCachedTabSize;
	}

	private void OnEnable()
	{
		float num = CalculateSlideWidth();
		m_slidingEndPosition = new Vector3(m_slidingStartPosition.x - num, m_slidingStartPosition.y, m_slidingStartPosition.z);
	}

	private void ToggleExpand()
	{
		m_expanded = !m_expanded;
		SetExpand(m_expanded);
	}

	public void ForceExpansion(bool bExpanded)
	{
		m_expanded = bExpanded;
		SetExpand(m_expanded);
		m_toggleButton.SetToggleState(bExpanded ? 1 : 0);
	}

	private void SetExpand(bool expand)
	{
		if (expand)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("islocal", true, "position", m_slidingEndPosition, "time", m_slideTime, "oncomplete", "OnCompleteSlideOut", "oncompletetarget", base.gameObject));
			if (SlideOutSfx != string.Empty)
			{
				MenuSFX.Instance.Play2D(SlideOutSfx);
			}
		}
		else
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("islocal", true, "position", m_slidingStartPosition, "time", m_slideTime, "oncomplete", "OnCompleteSlideOut", "oncompletetarget", base.gameObject));
			if (SlideInSfx != string.Empty)
			{
				MenuSFX.Instance.Play2D(SlideInSfx);
			}
		}
	}

	private void EnableItems(bool enabled)
	{
		AutoSpriteControlBase[] items = m_items;
		foreach (AutoSpriteControlBase autoSpriteControlBase in items)
		{
			autoSpriteControlBase.controlIsEnabled = enabled;
		}
		if (m_vCachedTabSize.x > 0f)
		{
			BoxCollider component = m_toggleButton.gameObject.GetComponent<BoxCollider>();
			component.size = m_vCachedTabSize;
		}
	}

	private void RemoveNullItems()
	{
		int num = 0;
		for (int i = 0; i < m_items.Length; i++)
		{
			if (m_items[i] == null)
			{
				num++;
			}
		}
		if (num <= 0)
		{
			return;
		}
		int num2 = m_items.Length - num;
		AutoSpriteControlBase[] array = new AutoSpriteControlBase[num2];
		int num3 = 0;
		for (int j = 0; j < m_items.Length; j++)
		{
			if (m_items[j] != null)
			{
				array[num3] = m_items[j];
				array[num3].transform.parent.localPosition = new Vector3(-260f - 190f * (float)num3, 0f, -1f);
				num3++;
			}
		}
		m_items = array;
	}

	private float CalculateSlideWidth()
	{
		RemoveNullItems();
		float result = 0f;
		if (m_items.Length > 0)
		{
			Vector3 position = m_toggleButton.transform.position;
			float num = (m_items[m_items.Length - 1].transform.position.x - position.x) / (float)m_items.Length;
			result = num * (float)m_items.Length + num / 2f;
		}
		if ((bool)StateManager.Instance && StateManager.Instance.CurrentStateName == "Results" && GameController.Instance.IsPlayingTrialsMode && GameController.Instance.Player.IsDead() && m_items.Length > 0)
		{
			Vector3 position2 = m_toggleButton.transform.position;
			float num2 = m_items[0].transform.position.x - position2.x;
			result = num2 + num2 / 4f;
		}
		return result;
	}

	private void OnCompleteSlideOut()
	{
		EnableItems(true);
	}

	private void OnCompleteSlideIn()
	{
		EnableItems(false);
	}

	private void GoOffScreen()
	{
		SetExpand(false);
		m_expanded = false;
		m_toggleButton.SetToggleState(0);
	}
}
