using System.Text.RegularExpressions;

namespace Swrve
{
	public class BuyInEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"buy_in\",\n\t\"time\":{0},\n\t\"payment_provider\":\"{1}\",\n\t\"reward_currency\":\"{2}\",\n\t\"local_currency\":\"{3}\",\n\t\"reward_amount\":{4},\n\t\"local_cost\":{5}\n}}", "\\s", string.Empty);
	}
}
