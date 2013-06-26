using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TrackProtect
{
	public class WebInterface
	{
		NameValueCollection _requestHeader = new NameValueCollection();
		NameValueCollection _requestData = new NameValueCollection();
		
		public NameValueCollection RequestHeader
		{
			get { return _requestHeader; }
		}
		
		public NameValueCollection RequestData
		{
			get { return _requestData; }
		}
		
		public WebInterface()
		{
		}
		
		public byte[] UploadValues(string uri, string method)
		{
			return UploadValues(new Uri(uri), method);
		}
		
		public byte[] UploadValues(string uri, string method, NameValueCollection data)
		{
			_requestData = data;
			return UploadValues(uri, method);
		}
		
		public byte[] UploadValues(Uri uri, string method, NameValueCollection data)
		{
			_requestData = data;
			return UploadValues(uri, method);
		}
		
		public byte[] UploadValues(Uri uri, string method)
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(uri.Host, uri.Port);
			
			StringBuilder sb = new StringBuilder();
			
			foreach (string key in _requestData.AllKeys)
			{
				if (sb.Length > 0)
					sb.Append('&');
				sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(_requestData[key]));
			}
			string content = sb.ToString();
			
			StringBuilder req = new StringBuilder();
			req.AppendFormat("{0} {1} HTTP/1.1\r\n", method, uri.AbsolutePath);
			req.AppendFormat("Host: {0}\r\n", uri.Host);
			req.AppendFormat("User-Agent: {0}\r\n", "HTTPTool/1.0");
			foreach (string key in _requestHeader.AllKeys)
				req.AppendFormat("{0}: {1}\r\n", key, _requestHeader[key]);
			if (!string.IsNullOrEmpty(content))
			{
				req.Append("Content-Type: application/x-www-form-urlencoded\r\n");
				req.AppendFormat("Content-Length: {0}\r\n", content.Length);
			}
			req.Append("\r\n");
			if (!string.IsNullOrEmpty(content))
				req.AppendFormat("{0}\r\n\r\n", content);
			
			byte[] raw = Encoding.ASCII.GetBytes(req.ToString());
			socket.Send(raw);
			
			string data = string.Empty;
			using (MemoryStream ms = new MemoryStream())
			{
				byte[] buf = new byte[1024];
				int nbytes = 0;
				do
				{
					nbytes = socket.Receive(buf, buf.Length, SocketFlags.None);
					ms.Write(buf, 0, nbytes);
				}
				while (nbytes > 0);
				
				data = Encoding.ASCII.GetString(ms.ToArray());
			}

			int pos = data.IndexOf("<?xml");
			if (pos > -1)
			{
			    // It contains XML so extract it
			    data = data.Substring(pos);
			}
			
			return Encoding.ASCII.GetBytes(data);
		}
		
		public static string MD5(string data)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider x = 
				new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
			bs = x.ComputeHash(bs);
			StringBuilder s = new StringBuilder();
			foreach (byte b in bs)
			   s.Append(b.ToString("x2").ToLower());

			return s.ToString();		
		}
	}
}

