using System.Text.RegularExpressions;

namespace Swrve
{
	public class CurrencyGivenEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"currency_given\",\n\t\"time\":{0},\n\t\"given_currency\":\"{1}\",\n\t\"given_amount\":{2}\n}}", "\\s", string.Empty);
	}
}
