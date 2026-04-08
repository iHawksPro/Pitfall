using UnityEngine;

public class ButtonAnchor : MonoBehaviour
{
	public string m_gameObjectName;

	private void ResetTransform()
	{
		GameObject gameObject = GameObject.Find(m_gameObjectName);
		if (gameObject != null)
		{
			AutoSpriteControlBase componentInChildren = base.gameObject.GetComponentInChildren<AutoSpriteControlBase>();
			if (componentInChildren != null)
			{
				componentInChildren.transform.position = gameObject.transform.position;
				componentInChildren.transform.rotation = gameObject.transform.rotation;
			}
		}
		else
		{
			Debug.LogWarning("Can't find anchor object " + m_gameObjectName + " for button " + base.name);
		}
	}
}
