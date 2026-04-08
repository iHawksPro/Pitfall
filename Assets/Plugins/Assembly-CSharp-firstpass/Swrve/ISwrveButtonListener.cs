namespace Swrve
{
	public interface ISwrveButtonListener
	{
		bool OnAction(SwrveMessageFormat format, SwrveActionType type, string action, int game);
	}
}
