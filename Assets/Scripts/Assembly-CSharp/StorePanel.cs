using System.Collections.Generic;
using UnityEngine;

public class StorePanel : MonoBehaviour
{
	public UIScrollList m_scrollList;

	public Dictionary<string, int> m_IDLookUp = new Dictionary<string, int>();

	public static Transform FindChildObjectByName(Transform current, string name)
	{
		if (current.name == name)
		{
			return current;
		}
		for (int i = 0; i < current.childCount; i++)
		{
			Transform transform = FindChildObjectByName(current.GetChild(i), name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public void ScrollToItem(BaseCampProducts.ProductData product)
	{
		int value = -1;
		if (m_IDLookUp.TryGetValue(product.Identifier, out value))
		{
			m_scrollList.ScrollToItem(value, 0.6f);
			IUIListObject item = m_scrollList.GetItem(value);
			Transform transform = FindChildObjectByName(item.transform, "Item");
			if ((bool)transform)
			{
				UIButton component = transform.GetComponent<UIButton>();
				component.scriptWithMethodToInvoke.Invoke(component.methodToInvoke, 0f);
			}
		}
	}

	public void AddScrollPosition(string id, int scrollPos)
	{
		if (!m_IDLookUp.ContainsKey(id))
		{
			m_IDLookUp.Add(id, scrollPos);
		}
		else
		{
			m_IDLookUp[id] = scrollPos;
		}
	}

	public float GetScrollPosition()
	{
		return m_scrollList.ScrollPosition;
	}

	private void OnScrollChanged(float newScrollPos)
	{
		if (newScrollPos >= 1f)
		{
			SwrveEventsUI.StoreViewedAllPanelProducts();
		}
	}

	protected virtual void OnEnable()
	{
		m_scrollList.AddInputScrollChangeDelegate(OnScrollChanged);
	}

	protected virtual void OnDisable()
	{
		m_scrollList.RemoveInputScrollChangeDelegate(OnScrollChanged);
	}
}
