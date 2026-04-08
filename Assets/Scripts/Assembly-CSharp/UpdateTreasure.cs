public class UpdateTreasure : UpdateHUDValue
{
	public override void CustomSetup()
	{
		m_CountdownFormat = "{0}+{1}";
	}

	public override int GetValueToAdd()
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = PlayerController.Instance().Score().CoinsCollected();
		}
		return result;
	}

	public override int GetCurrentValue()
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = SecureStorage.Instance.GetCoins() - PlayerController.Instance().Score().CoinsCollected();
		}
		return result;
	}
}
