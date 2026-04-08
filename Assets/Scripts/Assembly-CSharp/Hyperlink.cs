using UnityEngine;

public class Hyperlink : MonoBehaviour
{
	public SpriteText Label;

	public void OnLinkPressed()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			EtceteraPlatformWrapper.ShowAlert(string.Empty, Language.Get("S_NET_UNAVAILABLE_BODY_DROID"), Language.Get("S_OK"));
		}
		else
		{
			EtceteraPlatformWrapper.ShowWebPage(Label.text);
		}
	}
}
