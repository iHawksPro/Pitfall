using System.Text.RegularExpressions;

namespace Swrve
{
	public class SessionEndEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"session_end\",\n\t\"time\":{0}\n}}", "\\s", string.Empty);
	}
}
