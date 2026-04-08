using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HTTP
{
	public class Request
	{
		public string method = "GET";

		public string protocol = "HTTP/1.1";

		public byte[] bytes;

		public Uri uri;

		public static byte[] EOL = new byte[2] { 13, 10 };

		public Response response;

		public bool isDone;

		public int maximumRetryCount = 8;

		public bool acceptGzip;

		public bool useCache;

		public Exception exception;

		public RequestState state;

		private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

		private static Dictionary<string, string> etags = new Dictionary<string, string>();

		public string Text
		{
			set
			{
				bytes = Encoding.UTF8.GetBytes(value);
			}
		}

		public Request(string method, string uri)
		{
			this.method = method;
			this.uri = new Uri(uri);
		}

		public Request(string method, string uri, bool useCache)
		{
			this.method = method;
			this.uri = new Uri(uri);
			this.useCache = useCache;
		}

		public Request(string method, string uri, byte[] bytes)
		{
			this.method = method;
			this.uri = new Uri(uri);
			this.bytes = bytes;
		}

		public void AddHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Add(value);
		}

		public string GetHeader(string name)
		{
			name = name.ToLower().Trim();
			if (!headers.ContainsKey(name))
			{
				return string.Empty;
			}
			return headers[name][0];
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

		public void SetHeader(string name, string value)
		{
			name = name.ToLower().Trim();
			value = value.Trim();
			if (!headers.ContainsKey(name))
			{
				headers[name] = new List<string>();
			}
			headers[name].Clear();
			headers[name].Add(value);
		}

		public void Send()
		{
			isDone = false;
			state = RequestState.Waiting;
			if (acceptGzip)
			{
				SetHeader("Accept-Encoding", "gzip");
			}
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					int num = 0;
					while (++num < maximumRetryCount)
					{
						if (useCache)
						{
							string value = string.Empty;
							if (etags.TryGetValue(uri.AbsoluteUri, out value))
							{
								SetHeader("If-None-Match", value);
							}
						}
						SetHeader("Host", uri.Host);
						TcpClient tcpClient = new TcpClient();
						tcpClient.Connect(uri.Host, uri.Port);
						using (NetworkStream networkStream = tcpClient.GetStream())
						{
							Stream stream = networkStream;
							if (uri.Scheme.ToLower() == "https")
							{
								stream = new SslStream(networkStream, false, ValidateServerCertificate);
								try
								{
									SslStream sslStream = stream as SslStream;
									sslStream.AuthenticateAsClient(uri.Host);
								}
								catch (Exception ex)
								{
									Debug.LogError("Exception: " + ex.Message);
									return;
								}
							}
							WriteToStream(stream);
							response = new Response();
							state = RequestState.Reading;
							response.ReadFromStream(stream);
						}
						tcpClient.Close();
						int status = response.status;
						if (status == 301 || status == 302 || status == 307)
						{
							uri = new Uri(response.GetHeader("Location"));
						}
						else
						{
							num = maximumRetryCount;
						}
					}
					if (useCache)
					{
						string header = response.GetHeader("etag");
						if (header.Length > 0)
						{
							etags[uri.AbsoluteUri] = header;
						}
					}
				}
				catch (Exception ex2)
				{
					Console.WriteLine("Unhandled Exception, aborting request.");
					Console.WriteLine(ex2);
					Debug.Log("Unhanded Exception, aborting");
					Debug.Log(ex2.ToString());
					exception = ex2;
					response = null;
				}
				state = RequestState.Done;
				isDone = true;
			});
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			Debug.LogWarning("SSL Cert Error:" + sslPolicyErrors);
			return true;
		}

		private void WriteToStream(Stream outputStream)
		{
			BinaryWriter binaryWriter = new BinaryWriter(outputStream);
			binaryWriter.Write(Encoding.ASCII.GetBytes(method.ToUpper() + " " + uri.PathAndQuery + " " + protocol));
			binaryWriter.Write(EOL);
			foreach (string key in headers.Keys)
			{
				foreach (string item in headers[key])
				{
					binaryWriter.Write(Encoding.ASCII.GetBytes(key));
					binaryWriter.Write(':');
					binaryWriter.Write(Encoding.ASCII.GetBytes(item));
					binaryWriter.Write(EOL);
				}
			}
			if (bytes != null && bytes.Length > 0)
			{
				if (GetHeader("Content-Length") == string.Empty)
				{
					binaryWriter.Write(Encoding.ASCII.GetBytes("content-length: " + bytes.Length));
					binaryWriter.Write(EOL);
					binaryWriter.Write(EOL);
				}
				binaryWriter.Write(bytes);
			}
			else
			{
				binaryWriter.Write(EOL);
			}
		}
	}
}
