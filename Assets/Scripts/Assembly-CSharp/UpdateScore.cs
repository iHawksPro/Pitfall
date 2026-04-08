public class UpdateScore : UpdateHUDValue
{
	public override int GetValueToAdd()
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = PlayerController.Instance().Score().TotalScoreXPAdjusted();
		}
		return result;
	}
}
