namespace Swrve
{
	public class SwrvePitfallMessageListener : IMessageListener
	{
		void IMessageListener.OnShow(SwrveMessageFormat format)
		{
			if (!format.Closing && PlayerController.Instance().IsRunning() && !GameController.Instance.Paused())
			{
				GameController.Instance.PauseGame();
			}
		}

		void IMessageListener.OnShowing(SwrveMessageFormat format)
		{
			if (!format.Closing && PlayerController.Instance().IsRunning() && !GameController.Instance.Paused())
			{
				GameController.Instance.PauseGame();
			}
		}

		void IMessageListener.OnDismiss(SwrveMessageFormat format)
		{
			if (PlayerController.Instance().IsRunning() && GameController.Instance.Paused())
			{
				GameController.Instance.ResumeGame();
				GameController.Instance.ShowHUD(-1f);
			}
		}
	}
}
