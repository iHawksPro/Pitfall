public class UpdateXPDescription : UpdateHUDValue
{
	private new void OnEnable()
	{
		m_ValueText = base.transform.GetComponent<SpriteText>();
		UpdateDescription();
	}

	public void UpdateDescription()
	{
		if (PlayerController.Instance() != null)
		{
			m_ValueText.Text = PlayerController.Instance().Score().GetXPDescription();
		}
	}

	private void Update()
	{
	}
}
