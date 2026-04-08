using System;
using System.Text;
using JsonCheckerTool;

namespace Swrve
{
	public class ResponseBodyTester
	{
		public static bool TestJsonStatic(byte[] bodyBytes, out string decodedString)
		{
			try
			{
				decodedString = Encoding.UTF8.GetString(bodyBytes);
			}
			catch (Exception)
			{
				decodedString = string.Empty;
			}
			if (decodedString.Length > 0)
			{
				JsonChecker jsonChecker = new JsonChecker(20);
				try
				{
					for (int i = 0; i < decodedString.Length; i++)
					{
						jsonChecker.Check(decodedString[i]);
					}
					jsonChecker.FinalCheck();
					return true;
				}
				catch (Exception)
				{
				}
			}
			return false;
		}
	}
}
