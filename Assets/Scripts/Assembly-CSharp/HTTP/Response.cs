using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace HTTP
{
	public class Response
	{
		public int status = 200;

		public string message = "OK";

		public byte[] bytes;

		private List<byte[]> chunks;

		private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

		public string Text
		{
			get
			{
				if (bytes == null)
				{
					return string.Empty;
				}
				return Encoding.UTF8.GetString(bytes);
			}
		}

		public string Asset
		{
			get
			{
				throw new NotSupportedException("This can't be done, yet.");
			}
		}

		private void AddHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Add(value);
		}

		public List<string> GetHeaders(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			return headers[name];
		}

		public string GetHeader(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				return string.Empty;
			}
			return headers[name][headers[name].Count - 1];
		}

		private string ReadLine(Stream stream)
		{
			List<byte> list = new List<byte>();
			while (true)
			{
				byte b = (byte)stream.ReadByte();
				if (b == Request.EOL[1])
				{
					break;
				}
				list.Add(b);
			}
			return Encoding.ASCII.GetString(list.ToArray()).Trim();
		}

		private string[] ReadKeyValue(Stream stream)
		{
			string text = ReadLine(stream);
			if (text == string.Empty)
			{
				return null;
			}
			int num = text.IndexOf(':');
			if (num == -1)
			{
				return null;
			}
			return new string[2]
			{
				text.Substring(0, num).Trim(),
				text.Substring(num + 1).Trim()
			};
		}

		public byte[] TakeChunk()
		{
			byte[] result = null;
			lock (chunks)
			{
				if (chunks.Count > 0)
				{
					result = chunks[0];
					chunks.RemoveAt(0);
					return result;
				}
				return result;
			}
		}

		public void ReadFromStream(Stream inputStream)
		{
			string[] array = ReadLine(inputStream).Split(' ');
			MemoryStream memoryStream = new MemoryStream();
			if (!int.TryParse(array[1], out status))
			{
				throw new HTTPException("Bad Status Code");
			}
			message = string.Join(" ", array, 2, array.Length - 2);
			headers.Clear();
			while (true)
			{
				string[] array2 = ReadKeyValue(inputStream);
				if (array2 == null)
				{
					break;
				}
				AddHeader(array2[0], array2[1]);
			}
			if (GetHeader("transfer-encoding") == "chunked")
			{
				chunks = new List<byte[]>();
				while (true)
				{
					string text = ReadLine(inputStream);
					if (text == "0")
					{
						break;
					}
					int num = int.Parse(text, NumberStyles.AllowHexSpecifier);
					for (int i = 0; i < num; i++)
					{
						memoryStream.WriteByte((byte)inputStream.ReadByte());
					}
					lock (chunks)
					{
						if (GetHeader("content-encoding").Contains("gzip"))
						{
							chunks.Add(UnZip(memoryStream));
						}
						else
						{
							chunks.Add(memoryStream.ToArray());
						}
					}
					memoryStream.SetLength(0L);
					inputStream.ReadByte();
					inputStream.ReadByte();
				}
				lock (chunks)
				{
					chunks.Add(new byte[0]);
				}
				while (true)
				{
					string[] array3 = ReadKeyValue(inputStream);
					if (array3 == null)
					{
						break;
					}
					AddHeader(array3[0], array3[1]);
				}
				List<byte> list = new List<byte>();
				foreach (byte[] chunk in chunks)
				{
					list.AddRange(chunk);
				}
				bytes = list.ToArray();
			}
			else
			{
				int num2 = 0;
				try
				{
					num2 = int.Parse(GetHeader("content-length"));
				}
				catch
				{
					num2 = 0;
				}
				for (int j = 0; j < num2; j++)
				{
					memoryStream.WriteByte((byte)inputStream.ReadByte());
				}
				if (GetHeader("content-encoding").Contains("gzip"))
				{
					bytes = UnZip(memoryStream);
				}
				else
				{
					bytes = memoryStream.ToArray();
				}
			}
		}

		private byte[] UnZip(MemoryStream output)
		{
			Debug.Log("HTTP ERROR: Unable to handle GZip encoding");
			return output.ToArray();
		}
	}
}
