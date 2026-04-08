public class UpdateDistance : UpdateHUDValue
{
	public override int GetValueToAdd()
	{
		int result = 0;
		if (PlayerController.Instance() != null)
		{
			result = (int)PlayerController.Instance().Score().DistanceTravelled();
		}
		return result;
	}

	public override string GetUnits()
	{
		return "m";
	}
}
