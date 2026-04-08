using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JsonCheckerTool;

namespace Swrve
{
	public class PostBodyBuilder
	{
		private const int ApiVersion = 2;

		private static readonly string HexDigits = "0123456789abcdef";

		private static readonly string Format = Regex.Replace("\n{{\n\t\"user\":\"{0}\",\n\t\"version\":{1},\n\t\"app_version\":\"{2}\",\n\t\"session_token\":\"{3}\",\n\t\"data\":[{4}]\n}}", "\\s", string.Empty);

		public static byte[] Build(string apiKey, int gameId, string userId, string appVersion, long time, string events)
		{
			string text = CreateSessionToken(apiKey, gameId, userId, time);
			string s = string.Format(Format, userId, 2, appVersion, text, events);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			JsonChecker jsonChecker = new JsonChecker(20);
			try
			{
				for (int i = 0; i < bytes.Length; i++)
				{
					jsonChecker.Check(bytes[i]);
				}
				jsonChecker.FinalCheck();
				return bytes;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static string CreateSessionToken(string apiKey, int gameId, string userId, long time)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			stringBuilder.AppendFormat("{0}={1}={2}=", gameId, userId, time);
			using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
			{
				StringBuilder stringBuilder2 = new StringBuilder(userId);
				stringBuilder2.Append(time);
				stringBuilder2.Append(apiKey);
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder2.ToString());
				AppendHexBytes(stringBuilder, mD5CryptoServiceProvider.ComputeHash(bytes));
			}
			return stringBuilder.ToString();
		}

		private static void AppendHexBytes(StringBuilder sb, byte[] bytes)
		{
			sb.EnsureCapacity(sb.Length + 2 * bytes.Length);
			for (int i = 0; i < bytes.Length; i++)
			{
				int index = bytes[i] >> 4;
				int index2 = bytes[i] & 0xF;
				sb.Append(HexDigits[index]);
				sb.Append(HexDigits[index2]);
			}
		}
	}
}
