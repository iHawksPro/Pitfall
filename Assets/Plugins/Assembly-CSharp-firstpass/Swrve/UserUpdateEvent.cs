using System.Text.RegularExpressions;

namespace Swrve
{
	public class UserUpdateEvent
	{
		public static readonly string FormatString = Regex.Replace("\n{{\n\t\"type\":\"user\",\n\t\"time\":{0},\n\t\"attributes\":{1}\n}}", "\\s", string.Empty);
	}
}
