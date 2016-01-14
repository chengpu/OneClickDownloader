using System;
using System.Text;
using System.IO;
using System.Net;


namespace OneClickDownloader
{
	public class HttpHelper
	{
		public static byte[] GetData(string url)
		{
			try
			{
				HttpWebRequest httpRequest = WebRequest.CreateHttp(url);
				HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
				using (Stream stream = httpResponse.GetResponseStream())
				{
					byte[] buffer = new byte[httpResponse.ContentLength];
					int offset = 0;
					while ((buffer.Length - offset) > 0)
					{
						int n = stream.Read(buffer, offset, buffer.Length - offset);
						if (n <= 0)
						{
							return null;
						}
						offset += n;
					}
					return buffer;
				}
			}
			catch
			{
				return null;
			}
		}
	}
}

