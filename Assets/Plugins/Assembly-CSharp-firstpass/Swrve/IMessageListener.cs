namespace Swrve
{
	public interface IMessageListener
	{
		void OnShow(SwrveMessageFormat format);

		void OnShowing(SwrveMessageFormat format);

		void OnDismiss(SwrveMessageFormat format);
	}
}
