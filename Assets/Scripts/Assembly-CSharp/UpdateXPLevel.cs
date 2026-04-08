using UnityEngine;

public class UpdateXPLevel : MonoBehaviour
{
	private bool m_Update;

	private void OnEnable()
	{
		m_Update = true;
	}

	public void Update()
	{
		if (m_Update)
		{
			SpriteText component = base.transform.GetComponent<SpriteText>();
			if (component != null && component.gameObject.active && PlayerController.Instance() != null)
			{
				component.Text = PlayerController.Instance().Score().CurrentXP()
					.ToString();
				m_Update = false;
			}
		}
	}
}
