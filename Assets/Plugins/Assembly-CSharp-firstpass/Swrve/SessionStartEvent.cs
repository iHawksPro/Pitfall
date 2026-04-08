using System.Text.RegularExpressions;

namespace Swrve
{
	public class SessionStartEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"session_start\",\n\t\"time\":{0}\n}}", "\\s", string.Empty);
	}
}
