using System.Collections.Generic;
using UnityEngine;

namespace Swrve
{
	public class UnityWwwHelper
	{
		public static WwwDeducedError DeduceWwwError(WWW request, WwwExpectedResponse expectedResponse)
		{
			if (request.responseHeaders.Count > 0)
			{
				string text = null;
				foreach (KeyValuePair<string, string> responseHeader in request.responseHeaders)
				{
					if (string.Compare(responseHeader.Key, "X-Swrve-Error", true) == 0)
					{
						text = responseHeader.Key;
						break;
					}
				}
				if (text != null)
				{
					Debug.Log("request.responseHeaders[\"X-Swrve-Error\"]: " + request.responseHeaders[text]);
					return WwwDeducedError.ApplicationErrorHeader;
				}
			}
			if (request.error != null)
			{
				Debug.Log("request.error: " + request.error);
				return WwwDeducedError.NetworkError;
			}
			bool flag = request.bytes != null && request.bytes.Length > 0;
			bool flag2 = WwwExpectedResponse.BodyExpected == expectedResponse;
			if ((flag2 && !flag) || (flag && !flag2))
			{
				Debug.Log("Bad request.bytes");
				return WwwDeducedError.ApplicationErrorBody;
			}
			return WwwDeducedError.NoError;
		}
	}
}
