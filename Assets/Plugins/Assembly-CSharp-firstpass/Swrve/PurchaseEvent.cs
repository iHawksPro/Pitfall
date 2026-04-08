using System.Text.RegularExpressions;

namespace Swrve
{
	public class PurchaseEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"purchase\",\n\t\"time\":{0},\n\t\"item\":\"{1}\",\n\t\"currency\":\"{2}\",\n\t\"cost\":{3},\n\t\"quantity\":{4}\n}}", "\\s", string.Empty);
	}
}
