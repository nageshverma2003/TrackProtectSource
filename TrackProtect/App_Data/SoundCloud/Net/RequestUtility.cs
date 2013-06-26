using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace TrackProtect.SoundCloud.Net
{
  public sealed class RequestUtility
  {
    internal static readonly Encoding Encoding = Encoding.UTF8;
    internal const string USERAGENT   = "Mozilla/4.0 (compatible; MSIE 9.0; Windows;)";
    internal const string ACCEPT      = "'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',";

    public static string GetResponseString(IRequestInfo requestInfo)
    {
      using (var dataStream = GetResponseStream(requestInfo))
      using (var reader = new StreamReader(dataStream, Encoding))
      {
        return reader.ReadToEnd();
      }
    }

    public static Stream GetResponseStream(IRequestInfo requestInfo)
    {
      if (requestInfo == null)
        throw new ArgumentNullException("requestInfo");

      var webRequest = WebRequest.Create(requestInfo.UriString) as HttpWebRequest;
      webRequest.UserAgent  = USERAGENT;
      webRequest.Accept     = ACCEPT;

      if (requestInfo.PostContent != null)
        webRequest.Method = WebRequestMethods.Http.Post;
      else
        webRequest.Method = WebRequestMethods.Http.Get;

      if (requestInfo.PostContent != null)
      {
        byte[] byteArray = requestInfo.PostContent;

        webRequest.ContentType = requestInfo.ContentType;
        webRequest.ContentLength = byteArray.Length;

        using (Stream dataStream = webRequest.GetRequestStream())
        {
          dataStream.Write(byteArray, 0, byteArray.Length);
          dataStream.Close();
        }
      }

      Stream memoryStream = new MemoryStream();

      try
      {
        using (WebResponse response = webRequest.GetResponse())
        using (Stream dataStream = response.GetResponseStream())
        {
          if (dataStream != null)
          {
            CopyStream(dataStream, memoryStream);
            dataStream.Close();
          }
          response.Close();
        }
      }
      catch (WebException ex)
      {
        Debug.WriteLine(ex.Message);
      }

      return memoryStream;
    }

    public static string GetOAuthRequestUriString(string oauth, string requestUriString)
    {
      requestUriString += (requestUriString.IndexOf("?") > -1 ? oauth : "?" + oauth);
      return requestUriString;
    }

    const int BUFFERLENGTH = 0x2000;
    static readonly byte[] Buffer = new byte[BUFFERLENGTH];

    static void CopyStream(Stream input, Stream output)
    {
      int len;
      while ((len = input.Read(Buffer, 0, BUFFERLENGTH)) > 0)
        output.Write(Buffer, 0, len);

      output.Position = 0;
    }
  }
}