using System.Text.RegularExpressions;

namespace Swrve
{
	public class NamedEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"event\",\n\t\"time\":{0},\n\t\"name\":\"{1}\",\n\t\"payload\":{2}\n}}", "\\s", string.Empty);
	}
}
