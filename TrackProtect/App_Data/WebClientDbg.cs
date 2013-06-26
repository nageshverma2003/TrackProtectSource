using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
#if NET_2_0
using System.Collections.Generic;
using System.Threading;
#endif

using TrackProtect.Logging;

namespace TrackProtect
{
	public delegate void DownloadDataCompletedEventHandler (object sender, DownloadDataCompletedEventArgs e);
	public delegate void DownloadStringCompletedEventHandler (object sender, DownloadStringCompletedEventArgs e);
	public delegate void OpenReadCompletedEventHandler (object sender, OpenReadCompletedEventArgs e);
	public delegate void OpenWriteCompletedEventHandler (object sender, OpenWriteCompletedEventArgs e);
	public delegate void UploadDataCompletedEventHandler (object sender, UploadDataCompletedEventArgs e);
	public delegate void UploadFileCompletedEventHandler (object sender, UploadFileCompletedEventArgs e);
	public delegate void UploadStringCompletedEventHandler (object sender, UploadStringCompletedEventArgs e);
	public delegate void UploadValuesCompletedEventHandler (object sender, UploadValuesCompletedEventArgs e);

	[ComVisible(true)]
	public
#if !NET_2_0
	sealed
#endif
	class WebClientDbg : Component
	{
		static readonly string	_urlEncodedType = "application/x-www-form-urlencoded";
		static byte[]			_hexBytes;
		ICredentials			_credentials;
		WebHeaderCollection		_requestHeaders;
		WebHeaderCollection		_responseHeaders;
		Uri 					_baseAddress;
		string					_baseString;
		NameValueCollection		_queryString;
		bool					_isBusy;
#if NET_2_0
		Encoding				_encoding = Encoding.Default;
		IWebProxy				_proxy;
#endif
		
		// Constructors
		static WebClientDbg()
		{
			_hexBytes = new byte [16];
			int index = 0;
			for (int i = '0'; i <= '9'; i++, index++)
				_hexBytes [index] = (byte) i;

			for (int i = 'A'; i <= 'F'; i++, index++)
				_hexBytes [index] = (byte) i;		
		}

		public WebClientDbg()
		{
		}

		// Properties
		public string BaseAddress
		{
			get 
			{
				if (_baseString == null)
				{
					if (_baseAddress == null)
						return string.Empty;
				}
				_baseString = _baseAddress.ToString();
				return _baseString;
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					_baseAddress = null;
				}
				else
				{
					_baseAddress = new Uri(value);
				}
			}
		}

		public ICredentials Credentials
		{
			get { return _credentials; }
			set { _credentials = value; }
		}

		public WebHeaderCollection RequestHeaders {
			get {
				if (_requestHeaders == null)
					_requestHeaders = new WebHeaderCollection ();

				return _requestHeaders;
			}
			set { _requestHeaders = value; }
		}
		
		public WebHeaderCollection QueryString
		{
			get
			{
				if (_queryString == null)
					_queryString = new NameValueCollection();
				return (WebHeaderCollection)_queryString;
			}
			set { _queryString = value; }
		}

		public WebHeaderCollection ResponseHeader
		{
			get { return _responseHeaders; }
		}

#if NET_2_0
		public Encoding Encoding
		{
			get { return _encoding; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				_encoding = value;
			}
		}

		public IWebProxy Proxy
		{
			get { return _proxy; }
			set { _proxy = value; }
		}
#endif

#if NET_2_0
		public bool IsBusy
		{
			get { return _isBusy || wait_handles != null && wait_handles.Count > 0; }
		}
#else
		bool IsBusy
		{
			get { return _isBusy; }
		}
#endif

		// Methods
		void CheckBusy()
		{
			if (IsBusy)
				throw new NotSupportedException("WebClientDbg does not support concurrent I/O operations");
		}

		void SetBusy()
		{
			lock (this)
			{
				CheckBusy();
				_isBusy = true;
			}
		}

		// DownloadData
		public byte[] DowloadData(string address)
		{
			return DownloadData(MakeUri(address));
		}

#if NET_2_0
		public
#endif
		byte[] DownloadData(Uri address)
		{
			try
			{
				SetBusy();
				return DownloadData(address);
			}
			finally
			{
				_isBusy = false;
			}
		}

		byte[] DownloadDataCore(Uri address)
		{
			WebRequest request = SetupRequest(address, "GET");
			WebResponse response = request.GetResponse();
			Stream st = ProcessResponse(response);
			return ReadAll(st, (int)response.ContentLength);
		}

		// DownloadFile
		public void DownloadFile(string address, string fileName)
		{
			DownloadFile(MakeUri(address), fileName);
		}

#if NET_2_0
		public
#endif
		void DownloadFile(Uri address, string fileName)
		{
			try
			{
				SetBusy();
				DownloadFileCore(address, fileName);
			}
			finally
			{
				_isBusy = false;
			}
		}

		void DownloadFileCore(Uri address, string fileName)
		{
			WebRequest request = SetupRequest(address);
			WebResponse response = request.GetResponse();
			Stream st = ProcessResponse(response);

			int cLength = (int)response.ContentLength;
			int length = (cLength <= -1 || cLength > 8192) ? 8192 : cLength;
			byte[] buffer = new byte[length];
			using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
			{
				int nread = 0;
				while ((nread = st.Read(buffer, 0, length)) != 0)
					fs.Write(buffer, 0, nread);

				fs.Close();
			}
		}

		// OpenRead
		public Stream OpenRead(string address)
		{
			return OpenRead(MakeUri(address));
		}

#if NET_2_0
		public
#endif
		Stream OpenRead(Uri address)
		{
			try
			{
				SetBusy();
				WebRequest request = SetupRequest(address);
				WebResponse response = request.GetResponse();
				return ProcessResponse(response);
			}
			finally
			{
				_isBusy = false;
			}
		}

		// OpenWrite
		public Stream OpenWrite(string address)
		{
			return OpenWrite(MakeUri(address));
		}

		public Stream OpenWrite(string address, string method)
		{
			return OpenWrite(MakeUri(address), method);
		}

#if NET_2_0
		public
#endif
		Stream OpenWrite(Uri address)
		{
			return OpenWrite(address, DetermineMethod(address));
		}

#if NET_2_0
		public
#endif
		Stream OpenWrite(Uri address, string method)
		{
			try
			{
				SetBusy();
				WebRequest request = SetupRequest(address, method);
				return request.GetRequestStream();
			}
			finally
			{
				_isBusy = false;
			}
		}

		private string DetermineMethod(Uri address)
		{
			if (address == null)
				throw new ArgumentNullException("address");
#if NET_2_0
			if (address.Scheme == Uri.UriSchemeFtp)
				return "RETR";
#endif
			return "POST";
		}

		// UploadData
		public byte[] UploadData(string address, byte[] data)
		{
			return UploadData(MakeUri(address), data);
		}

		public byte[] UploadData(string address, string method, byte[] data)
		{
			return UploadData(MakeUri(address), method, data);
		}

#if NET_2_0
		public
#endif
		byte[] UploadData(Uri address, byte[] data)
		{
			return UploadData(address, DetermineMethod(address), data);
		}

#if NET_2_0
		public
#endif
		byte[] UploadData(Uri address, string method, byte[] data)
		{
			try
			{
				SetBusy();
				return UploadDataCore(address, method, data);
			}
			finally
			{
				_isBusy = false;
			}
		}

		byte[] UploadDataCore(Uri address, string method, byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			int contentLength = data.Length;
			WebRequest request = SetupRequest(address, method, contentLength);
			using (Stream strm = request.GetRequestStream())
			{
				strm.Write(data, 0, contentLength);
			}
			WebResponse response = request.GetResponse();
			Stream st = ProcessResponse(response);
			return ReadAll(st, (int)response.ContentLength);
		}

		// UploadFile
		public byte[] UploadFile(string address, string fileName)
		{
			return UploadFile(MakeUri(address), fileName);
		}

#if NET_2_0
		public
#endif
		byte[] UploadFile(Uri address, string fileName)
		{
			return UploadFile(address, DetermineMethod(address), fileName);
		}

		public byte[] UploadFile(string address, string method, string fileName)
		{
			return UploadFile(MakeUri(address), method, fileName);
		}

#if NET_2_0
		public
#endif
		byte[] UploadFile(Uri address, string method, string fileName)
		{
			try
			{
				SetBusy();
				return UploadFileCore(address, method, fileName);
			}
			finally
			{
				_isBusy = false;
			}
		}
	
		byte[] UploadFileCore(Uri address, string method, string fileName)
		{
			string fileCType = RequestHeaders["Content-Type"];
			if (fileCType != null)
			{
				string lower = fileCType.ToLower();
				if (lower.StartsWith("multipart/"))
					throw new WebException("Content-Type cannot be set to a multipart" +
											" type for this request.");
			}
			else
			{
				fileCType = "application/octet-stream";
			}

			string boundary = "------------" + DateTime.Now.Ticks.ToString ("x");
			RequestHeaders["Content-Type"] = String.Format("multipart/form-data; boundary={0}", boundary);
			WebRequest request = SetupRequest(address, method);
			Stream reqStream = null;
			Stream fStream = null;
			byte[] resultBytes = null;
			
			try
			{
				fStream = File.OpenRead(fileName);
				reqStream = request.GetRequestStream();
				byte[] realBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
				reqStream.Write(realBoundary, 0, realBoundary.Length);
				string partHeaders = String.Format("Content-Disposition: form-data; " +
													"name=\"file\"; filename=\"{0}\"\r\n" +
													"Content-Type: {1}\r\n\r\n",
													Path.GetFileName(fileName), fileCType);
				byte[] partHeadersBytes = Encoding.UTF8.GetBytes(partHeaders);
				reqStream.Write(partHeadersBytes, 0, partHeadersBytes.Length);
				int nread;
				byte[] buffer = new byte[4096];
				while ((nread = fStream.Read(buffer, 0, 4096)) != 0)
					reqStream.Write(buffer, 0, nread);
				reqStream.WriteByte((byte)'\r');
				reqStream.WriteByte((byte)'\n');
				reqStream.Write(realBoundary, 0, realBoundary.Length);
				reqStream.Close();
				reqStream = null;
				WebResponse response = request.GetResponse();
				Stream st = ProcessResponse(response);
				resultBytes = ReadAll(st, (int)response.ContentLength);
			}
			catch (WebException ex)
			{
				Logger.Instance.Write(LogLevel.Error, ex, "UploadFileCore");
				throw;
			}
			catch (Exception ex)
			{
				throw new WebException("Error uploading file.", ex);
			}
			finally
			{
				if (fStream != null)
					fStream.Close();
				if (reqStream != null)
					reqStream.Close();
			}

			return resultBytes;
		}

		public byte[] UploadValues(string address, NameValueCollection data)
		{
			return UploadValues(MakeUri (address), data);
		}

#if NET_2_0
		public
#endif
		byte[] UploadValues(Uri address, NameValueCollection data)
		{
			return UploadValues(address, DetermineMethod(address), data);
		}

#if NET_2_0
		public
#endif
		byte[] UploadValues(Uri uri, string method, NameValueCollection data)
		{
			try
			{
				SetBusy();
				return UploadValuesCore(uri, method, data);
			}
			finally
			{
				_isBusy = false;
			}
		}

		byte[] UploadValuesCore(Uri uri, string method, NameValueCollection data)
		{
			new Trace("UploadValuesCore");
			
			if (data == null)
			{
				Logger.Instance.Write(LogLevel.Error, "'data' is null");
				throw new ArgumentNullException("data");	// MS throws a nullref
			}
			
			string cType = RequestHeaders["Content-Type"];
			if (cType != null && string.Compare(cType, _urlEncodedType, true) != 0)
			{
				Logger.Instance.Write (LogLevel.Error, "Content-Type header cannot be changed");
				throw new WebException("Content-Type header cannot be changed from its default " +
										"value for this request.");
			}
			
			RequestHeaders["Content-Type"] = _urlEncodedType;
			WebRequest request = SetupRequest(uri, method);
			Logger.Instance.Write (LogLevel.Debug, "About to get request stream");
			Stream rqStream = request.GetRequestStream();
			Logger.Instance.Write (LogLevel.Debug, "Setting up memory stream");
			MemoryStream tmpStream = new MemoryStream();
			Logger.Instance.Write (LogLevel.Debug, "Preparing request data");
			foreach (string key in data)
			{
				byte[] bytes = Encoding.ASCII.GetBytes(key);
				UrlEncodeAndWrite(tmpStream, bytes);
				tmpStream.WriteByte((byte)'=');
				bytes = Encoding.ASCII.GetBytes(data[key]);
				UrlEncodeAndWrite(tmpStream, bytes);
				tmpStream.WriteByte((byte)'&');
			}

			Logger.Instance.Write (LogLevel.Debug, "Completing request");
			int length = (int)tmpStream.Length;
			if (length > 0)
				tmpStream.SetLength(--length);	// remove trailing '&'
			tmpStream.WriteByte((byte)'\r');
			tmpStream.WriteByte((byte)'\n');

			byte[] buf = tmpStream.GetBuffer();
			Logger.Instance.Write (LogLevel.Debug, "Completed request, about to write");
			rqStream.Write(buf, 0, length + 2);
			rqStream.Close();
			tmpStream.Close();
			
			Logger.Instance.Write (LogLevel.Debug, "Entering request/response sequence");
			WebResponse response = request.GetResponse();
			Logger.Instance.Write (LogLevel.Debug, "Received response");
			Stream st = ProcessResponse(response);
			Logger.Instance.Write (LogLevel.Debug, "Processed response");
			byte[] rawResponse = ReadAll(st, (int)response.ContentLength);
			st.Close ();
			response.Close();
			return rawResponse;
		}

#if NET_2_0
		public string DownloadString(string address)
		{
			return _encoding.GetString(DownloadData(MakeUri(address)));
		}

		public string DownloadString(Uri address)
		{
			return _encoding.GetString(DownloadData(address));
		}

		public string UploadString(string address, string data)
		{
			byte[] resp = UploadData(address, _encoding.GetBytes(data));
			return _encoding.GetString(resp);
		}

		public string UploadString(string address, string method, string data)
		{
			byte[] resp = UploadData(address, method, _encoding.GetBytes(data));
			return _encoding.GetString(resp);
		}

		public string UploadString(Uri address, string data)
		{
			byte[] resp = UploadData(address, _encoding.GetBytes(data));
			return _encoding.GetString(resp);
		}

		public string UploadString(Uri address, string method, string data)
		{
			byte[] resp = UploadData(address, method, _encoding.GetBytes(data));
			return _encoding.GetString(resp);
		}

		public event DownloadDataCompletedEventHandler DownloadDataCompleted;
		public event AsyncCompletedEventHandler DownloadFileCompleted;
		public event DownloadProgressChangedEventHandler DownloadProgressChanged;
		public event DownloadStringCompletedEventHandler DownloadStringCompleted;
		public event OpenReadCompletedEventHandler OpenReadCompleted;
		public event OpenWriteCompletedEventHandler OpenWriteCompleted;
		public event UploadDataCompletedEventHandler UploadDataCompleted;
		public event UploadFileCompletedEventHandler UploadFileCompleted;
		public event UploadProgressChangedEventHandler UploadProgressChanged;
		public event UploadStringCompletedEventHandler UploadStringCompleted;
		public event UploadValuesCompletedEventHandler UploadValuesCompleted;
#endif

		Uri MakeUri(string path)
		{
			string query = null;
			if (_queryString != null && _queryString.Count != 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append('?');
				foreach (string key in _queryString)
					sb.AppendFormat("{0}={1}&", key, UrlEncode(_queryString[key]));
				if (sb.Length != 0)
				{
					sb.Length--;	// remove trailing '&'
					query = sb.ToString();
				}
			}

			if (_baseAddress == null && query == null)
			{
				try
				{
					return new Uri(path);
				}
				catch (System.UriFormatException)
				{
					if ((path[0] == Path.DirectorySeparatorChar) || 
						(path[1] == ':' && Char.ToLower(path[0]) > 'a' && Char.ToLower(path[0]) < 'z')) 
					{
						return new Uri("file://" + path);
					}
					else
					{
						return new Uri("file://" + Environment.CurrentDirectory + Path.DirectorySeparatorChar + path);
					}
				}
			}

			if (_baseAddress == null)
				return new Uri(path + query);

			if (query == null)
				return new Uri(_baseAddress, path);

			return new Uri(_baseAddress, path + query);
		}

		WebRequest SetupRequest(Uri uri)
		{
			new Trace("SetupRequest");
			Logger.Instance.Write (LogLevel.Debug, "About to create WebRequest, {0}", uri);
			WebRequest request = WebRequest.Create(uri);
#if NET_2_0
			if (Proxy != null)
				request.Proxy = Proxy;
#endif
			Logger.Instance.Write (LogLevel.Debug, "About to process request headers. _requestHeader: {0}", _requestHeaders == null ? "<null>" : _requestHeaders.ToString());
			if (_requestHeaders != null && _requestHeaders.Count != 0 && (request is HttpWebRequest))
			{
				Logger.Instance.Write (LogLevel.Debug, "Processing request headers");
				try
				{
					HttpWebRequest req = (HttpWebRequest)request;
					string expect		= _requestHeaders["Expect"];
					string contentType	= _requestHeaders["Content-Type"];
					string accept		= _requestHeaders["Accept"];
					string connection	= _requestHeaders["Connection"];
					string userAgent	= _requestHeaders["User-Agent"];
					string referer		= _requestHeaders["Referer"];
					_requestHeaders.Remove("Expect");
					_requestHeaders.Remove("Content-Type");
					_requestHeaders.Remove("Accept");
					_requestHeaders.Remove("Connection");
					_requestHeaders.Remove("Referer");
					_requestHeaders.Remove("User-Agent");
					request.Headers = _requestHeaders;

					if (!string.IsNullOrEmpty(expect))
						req.Expect = expect;
					if (!string.IsNullOrEmpty(accept))
						req.Accept = accept;
					if (!string.IsNullOrEmpty(contentType))
						req.ContentType = contentType;
					if (!string.IsNullOrEmpty(connection))
						req.Connection = connection;
					if (!string.IsNullOrEmpty(userAgent))
						req.UserAgent = userAgent;
					if (!string.IsNullOrEmpty(referer))
						req.Referer = referer;
				}
				catch (Exception ex)
				{
					Logger.Instance.Write (LogLevel.Error, ex, "SetupRequest");
				}

				Logger.Instance.Write (LogLevel.Debug, "Processed request headers");
			}

			_responseHeaders = null;
			return request;
		}

		WebRequest SetupRequest(Uri uri, string method)
		{
			WebRequest request = SetupRequest(uri);
			request.Method = method;
			return request;
		}

		WebRequest SetupRequest(Uri uri, string method, int contentLength)
		{
			WebRequest request = SetupRequest(uri, method);
			request.ContentLength = contentLength;
			return request;
		}
	
		Stream ProcessResponse(WebResponse response)
		{
			_responseHeaders = response.Headers;
			return response.GetResponseStream();
		}

		static byte[] ReadAll(Stream stream, int length)
		{
			new Trace("ReadAll");
			MemoryStream ms = null;
			bool nolength = (length == -1);
			int size = ((nolength) ? 8192 : length);
			if (nolength)
				ms = new MemoryStream();
			
			Logger.Instance.Write (LogLevel.Debug, "Reading {0} bytes from stream", size);
			int nread = 0;
			int offset = 0;
			byte[] buffer = new byte[size];
			while ((nread = stream.Read(buffer, offset, size)) != 0)
			{
				if (nolength)
				{
					ms.Write(buffer, 0, nread);
				}
				else
				{
					offset += nread;
					size -= nread;
				}
			}

			if (nolength)
				return ms.ToArray();

			return buffer;
		}

		string UrlEncode(string str)
		{
			StringBuilder result = new StringBuilder();
		
			int len = str.Length;
			for (int i = 0; i < len; i++)
			{
				char c = str[i];
				if (c == ' ')
				{
					result.Append('+');
				}
				else if ((c < '0' && c != '-' && c != '.') ||
						(c < 'A' && c > '9') ||
						(c > 'Z' && c < 'a' && c != '_') ||
						(c > 'z'))
				{
					result.Append('%');
					int idx = ((int)c) >> 4;
					result.Append((char)_hexBytes[idx]);
					idx = ((int)c) & 0x0F;
					result.Append((char)_hexBytes[idx]);
				}
				else
				{
					result.Append(c);
				}
			}

			return result.ToString();
		}

		static void UrlEncodeAndWrite(Stream stream, byte[] bytes)
		{
			if (bytes == null)
				return;

			int len = bytes.Length;
			if (len == 0)
				return;

			for (int i = 0; i < len; i++)
			{
				char c = (char) bytes [i];
				if (c == ' ')
				{
					stream.WriteByte ((byte) '+');
				}
				else if ((c < '0' && c != '-' && c != '.') ||
						(c < 'A' && c > '9') ||
						(c > 'Z' && c < 'a' && c != '_') ||
						(c > 'z')) 
				{
					stream.WriteByte ((byte) '%');
					int idx = ((int) c) >> 4;
					stream.WriteByte (_hexBytes[idx]);
					idx = ((int) c) & 0x0F;
					stream.WriteByte (_hexBytes[idx]);
				} 
				else 
				{
					stream.WriteByte ((byte) c);
				}
			}
		}

#if NET_2_0
		List<RegisteredWaitHandle> wait_handles;
		List<RegisteredWaitHandle> WaitHandles
		{
			get
			{
				if (wait_handles == null)
					wait_handles = new List<RegisteredWaitHandle>();
				return wait_handles;
			}
		}

		//[MonoTODO("Is it enough to just unregister wait handles from ThreadPool?")]
		public void CancelAsync()
		{
			if (wait_handles == null)
				return;
			lock (wait_handles)
			{
				foreach (RegisteredWaitHandle handle in wait_handles)
					handle.Unregister(null);
				wait_handles.Clear();
			}
		}

		public void DownloadDataAsync(Uri uri)
		{
			DownloadDataAsync(uri, null);
		}

		public void DownloadDataAsync(Uri uri, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object [] {uri, asyncState};
				WaitOrTimerCallback cb = delegate (object state, bool timedOut)
				{
					object[] args = (object[])state;
					byte[] data = timedOut ? null : DownloadData((Uri)args[0]);
					OnDownloadDataCompleted(new DownloadDataCompletedEventArgs(data, null, timedOut, args[1]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// DownloadFileAsync
		public void DownloadFileAsync(Uri uri, string method)
		{
			DownloadFileAsync(uri, method, null);
		}

		public void DownloadFileAsync(Uri uri, string method, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object[] { uri, method, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					if (!timedOut)
						DownloadFile((Uri)args[0], (string)args[1]);
					OnDownloadFileCompleted(new AsyncCompletedEventArgs(null, timedOut, args[2]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		public void DownloadStringAsync(Uri uri)
		{
			DownloadStringAsync(uri, null);
		}

		public void DownloadStringAsync(Uri uri, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object[] {uri, asyncState};
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					string data = timedOut ? null : DownloadString((Uri)args[0]);
					OnDownloadStringCompleted(new DownloadStringCompletedEventArgs(data, null, timedOut, args[1]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// OpenReadAsync
		public void OpenReadAsync(Uri uri)
		{
			OpenReadAsync(uri, null);
		}

		public void OpenReadAsync(Uri uri, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object[] { uri, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					Stream stream = timedOut ? null : OpenRead((Uri)args[0]);
					OnOpenReadCompleted(new OpenReadCompletedEventArgs(stream, null, timedOut, args[1]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// OpenWriteAsync
		public void OpenWriteAsync(Uri uri)
		{
			OpenWriteAsync(uri, null);
		}

		public void OpenWriteAsync(Uri uri, string method)
		{
			OpenWriteAsync(uri, method, null);
		}

		public void OpenWriteAsync(Uri uri, string method, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object[] { uri, method, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					Stream stream = timedOut ? null : OpenWrite((Uri)args[0], (string)args[1]);
					OnOpenWriteCompleted(new OpenWriteCompletedEventArgs(stream, null, timedOut, args[2]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// UploaddataAsync
		public void UploadDataAsync(Uri uri, byte[] data)
		{
			UploadDataAsync(uri, null, data);
		}

		public void UploadDataAsync(Uri uri, string method, byte[] data)
		{
			UploadDataAsync(uri, method, data, null);
		}

		public void UploadDataAsync(Uri uri, string method, byte[] data, object asyncState)
		{
			lock (this)
			{
				CheckBusy();
				object[] cbArgs = new object[] { uri, method, data, asyncState };
				WaitOrTimerCallback cb = delegate (object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					byte[] innerData = timedOut ? null : UploadData((Uri)args[0], (string)args[1], (byte[])args[2]);
					OnUploadDataCompleted(new UploadDataCompletedEventArgs(innerData, null, timedOut, args[3]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// UploadFileAsync
		public void UploadFileAsync(Uri uri, string file)
		{
			UploadFileAsync(uri, null, file);
		}

		public void UploadFileAsync(Uri uri, string method, string file)
		{
			UploadFileAsync(uri, method, file, null);
		}

		public void UploadFileAsync(Uri uri, string method, string file, object asyncState)
		{
			lock (this)
			{
				CheckBusy();

				object[] cbArgs = new object[] { uri, method, file, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					byte[] data = timedOut ? null : UploadFile((Uri)args[0], (string)args[1], (string)args[2]);
					OnUploadFileCompleted(new UploadFileCompletedEventArgs(data, null, timedOut, args[3]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// UploadStringAsync
		public void UploadStringAsync(Uri uri, string data)
		{
			UploadStringAsync(uri, null, data);
		}

		public void UploadStringAsync(Uri uri, string method, string data)
		{
			UploadStringAsync(uri, method, data, null);
		}

		public void UploadStringAsync(Uri uri, string method, string data, object asyncState)
		{
			lock (this)
			{
				CheckBusy();

				object[] cbArgs = new object[] { uri, method, data, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					string innerData = timedOut ? null : UploadString((Uri)args[0], (string)args[1], (string)args[2]);
					OnUploadStringCompleted(new UploadStringCompletedEventArgs(innerData, null, timedOut, args[3]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		// UploadValuesAsync
		public void UploadvaluesAsync(Uri uri, NameValueCollection values)
		{
			UploadValuesAsync(uri, null, values);
		}

		public void UploadValuesAsync(Uri uri, string method, NameValueCollection values)
		{
			UploadValuesAsync(uri, method, values, null);
		}

		public void UploadValuesAsync(Uri uri, string method, NameValueCollection values, object asyncState)
		{
			lock (this)
			{
				CheckBusy();

				object[] cbArgs = new object[] { uri, method, values, asyncState };
				WaitOrTimerCallback cb = delegate(object innerState, bool timedOut)
				{
					object[] args = (object[])innerState;
					byte[] data = timedOut ? null : UploadValues((Uri)args[0], (string)args[1], (NameValueCollection)args[2]);
					OnUploadValuesCompleted(new UploadValuesCompletedEventArgs(data, null, timedOut, args[3]));
				};
				AutoResetEvent ev = new AutoResetEvent(true);
				WaitHandles.Add(ThreadPool.RegisterWaitForSingleObject(ev, cb, cbArgs, -1, true));
			}
		}

		protected virtual void OnDownloadDataCompleted(DownloadDataCompletedEventArgs args)
		{
			if (DownloadDataCompleted != null)
				DownloadDataCompleted(this, args);
		}

		protected virtual void OnDownloadFileCompleted(AsyncCompletedEventArgs args)
		{
			if (DownloadFileCompleted != null)
				DownloadFileCompleted(this, args);
		}

		protected virtual void OnDownloadStringCompleted (
			DownloadStringCompletedEventArgs args)
		{
			if (DownloadStringCompleted != null)
				DownloadStringCompleted (this, args);
		}

		protected virtual void OnOpenReadCompleted (
			OpenReadCompletedEventArgs args)
		{
			if (OpenReadCompleted != null)
				OpenReadCompleted (this, args);
		}

		protected virtual void OnOpenWriteCompleted (
			OpenWriteCompletedEventArgs args)
		{
			if (OpenWriteCompleted != null)
				OpenWriteCompleted (this, args);
		}

		protected virtual void OnUploadDataCompleted (
			UploadDataCompletedEventArgs args)
		{
			if (UploadDataCompleted != null)
				UploadDataCompleted (this, args);
		}

		protected virtual void OnUploadFileCompleted (
			UploadFileCompletedEventArgs args)
		{
			if (UploadFileCompleted != null)
				UploadFileCompleted (this, args);
		}

		protected virtual void OnUploadStringCompleted (
			UploadStringCompletedEventArgs args)
		{
			if (UploadStringCompleted != null)
				UploadStringCompleted (this, args);
		}

		protected virtual void OnUploadValuesCompleted (
			UploadValuesCompletedEventArgs args)
		{
			if (UploadValuesCompleted != null)
				UploadValuesCompleted (this, args);
		}
#endif
	}

	public class DownloadDataCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DownloadDataCompletedEventArgs (byte [] result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		byte [] result;

		public byte [] Result {
			get { return result; }
		}
	}

	public class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DownloadStringCompletedEventArgs (string result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		string result;

		public string Result {
			get { return result; }
		}
	}

	public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenReadCompletedEventArgs (Stream result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		Stream result;

		public Stream Result {
			get { return result; }
		}
	}

	public class OpenWriteCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenWriteCompletedEventArgs (Stream result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		Stream result;

		public Stream Result {
			get { return result; }
		}
	}

	public class UploadDataCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadDataCompletedEventArgs (byte [] result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		byte [] result;

		public byte [] Result {
			get { return result; }
		}
	}	

	public class UploadFileCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadFileCompletedEventArgs (byte [] result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		byte [] result;

		public byte [] Result {
			get { return result; }
		}
	}

	public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadStringCompletedEventArgs (string result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		string result;

		public string Result {
			get { return result; }
		}
	}

	public class UploadValuesCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadValuesCompletedEventArgs (byte [] result,
			Exception error, bool cancelled, object userState)
			: base (error, cancelled, userState)
		{
			this.result = result;
		}

		byte [] result;

		public byte [] Result {
			get { return result; }
		}
	}
}