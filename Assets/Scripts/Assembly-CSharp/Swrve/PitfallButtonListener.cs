using System.Text.RegularExpressions;
using UnityEngine;

namespace Swrve
{
	public class PitfallButtonListener : ISwrveButtonListener
	{
		bool ISwrveButtonListener.OnAction(SwrveMessageFormat format, SwrveActionType type, string action, int game)
		{
			Debug.Log("swrve button action: " + action);
			if (type == SwrveActionType.Custom)
			{
				if (action.StartsWith("open_url#"))
				{
					string url = Regex.Split(action, "open_url#")[1];
					Application.OpenURL(url);
				}
				else if (action.Equals("open_store"))
				{
					if (BaseCampController.Instance != null)
					{
						BaseCampController.Instance.LaunchFreeDiamonds();
					}
				}
				else if (action.StartsWith("give#"))
				{
					string[] array = action.Split('#');
					int result = 1;
					int.TryParse(array[2], out result);
					string text = array[1];
					if (text.Equals("gems"))
					{
						SecureStorage.Instance.ChangeGems(result);
					}
					else if (text.Equals("coins"))
					{
						SecureStorage.Instance.ChangeCoins(result);
					}
					else
					{
						Debug.Log("swrve button change item count. Element: " + text + ", amount: " + result);
						SecureStorage.Instance.ChangeItemCount(text, result);
					}
				}
				else if (action.StartsWith("redirect_buy#"))
				{
					string[] array2 = action.Split('#');
					if (BaseCampController.Instance != null)
					{
						BaseCampProducts.ProductData productData = StoreProductManager.Instance.FindProduct(array2[1]);
						if (productData != null)
						{
							bool flag = StateManager.Instance.CurrentStateName.Equals("BaseCamp");
							BaseCampController.Instance.LaunchWithProductFocusImmediate(productData);
							if (flag)
							{
								BaseCampController.Instance.ForceActivate("Title");
							}
						}
					}
				}
				else if (action.StartsWith("discount_buy#"))
				{
					string[] array3 = action.Split('#');
					int result2 = 0;
					int.TryParse(array3[2], out result2);
					if (BaseCampController.Instance != null)
					{
						BaseCampProducts.ProductData productData2 = StoreProductManager.Instance.FindProduct(array3[1]);
						if (productData2 != null)
						{
							BaseCampProducts.ProductData productData3 = new BaseCampProducts.ProductData();
							productData2.CopyTo(productData3);
							productData3.Discount = result2;
							for (int i = 0; i < productData3.Levels.Length; i++)
							{
								productData3.Levels[i].CoinPrice = (int)((float)productData3.Levels[i].CoinPrice * ((float)(100 - productData3.Discount) / 100f));
								productData3.Levels[i].GemPrice = (int)((float)productData3.Levels[i].GemPrice * ((float)(100 - productData3.Discount) / 100f));
							}
							bool flag2 = StateManager.Instance.CurrentStateName.Equals("BaseCamp");
							BaseCampController.Instance.LaunchWithProductDiscount(productData3);
							if (flag2)
							{
								BaseCampController.Instance.ForceActivate("Title");
							}
						}
					}
				}
				else if (action.Equals("facebook_fan"))
				{
					DialogManager.Instance.OnFacebookYes();
				}
				else if (action.Equals("twitter_fan"))
				{
					DialogManager.Instance.TellTwitterFollowers();
				}
			}
			return true;
		}
	}
}
