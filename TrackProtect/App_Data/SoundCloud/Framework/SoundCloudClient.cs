using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using TrackProtect.SoundCloud.Core;
using TrackProtect.SoundCloud.Json;

namespace TrackProtect.SoundCloud.Framework
{
  public class SoundCloudClient
  {
    const int APIVERSION = 1;

    const string AUTHORIZE    = "authorize";
    const string ACCESSTOKEN  = "access_token";
    const string DEVELOPMENT  = "development";
    const string PRODUCTION   = "production";

    internal static readonly Encoding Encoding = Encoding.UTF8;

    static readonly Dictionary<string, string> Paths = new Dictionary<string, string>
    {
      { AUTHORIZE,    "connect"       },
      { ACCESSTOKEN,  "oauth2/token"  }
    };

    static readonly Dictionary<string, string> Domains = new Dictionary<string, string>
    {
      { DEVELOPMENT,  "sandbox-soundcloud.com"  },
      { PRODUCTION,   "soundcloud.com"          }
    };

    readonly string _clientId;
    readonly string _clientSecret;
    readonly string _redirectUri;
    readonly bool _development;

    string _accessToken;

    public SoundCloudClient(string clientId, string clientSecret, string redirectUri = null, bool development = false)
    {
      _clientId     = clientId;
      _clientSecret = clientSecret;
      _redirectUri  = redirectUri;
      _development  = development;
    }

    public string AccessToken
    {
      get { return _accessToken; }
      set { _accessToken = value; }
    }

    public string GetAuthorizeUrl(Dictionary<string, string> pars = null)
    {
      if (pars == null)
        pars = new Dictionary<string, string>();

      var defaultParams = new Dictionary<string, string>
      {
        { "client_id",      _clientId     },
        { "redirect_uri",   _redirectUri  },
        { "response_type",  "code"        }
      };
      pars = Merge(defaultParams, pars);
      return BuildUrl(Paths[AUTHORIZE], pars, false);
    }

    public string GetAccessTokenUrl(Dictionary<string, string> pars = null)
    {
      return BuildUrl(Paths[ACCESSTOKEN], pars, false);
    }

    public string GetAccessToken(string code = "", Dictionary<string, string> postData = null)
    {
      if (postData == null)
        postData = new Dictionary<string, string>();

      var defaultPostData = new Dictionary<string, string>
      {
        { "code",           code                  },
        { "client_id",      _clientId             },
        { "client_secret",  _clientSecret         },
        { "redirect_uri",   _redirectUri          },
        { "grant_type",     "authorization_code"  }
      };

      postData = Merge(defaultPostData, postData);
      return GetAccessTokenInternal(postData);
    }

    private static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> target)
    {
      var array = new Dictionary<TKey, TValue>();
      foreach (var s in source)
        array.Add(s.Key, s.Value);
      foreach (var t in target)
        array.Add(t.Key, t.Value);
      return array;
    }

    private static string HttpBuildQuery(Dictionary<string, string> pars)
    {
      var list = pars.Select(param => param.Key + "=" + param.Value);
      return string.Join("&", list);
    }

    private static string Request(string path, string method="GET", Dictionary<string, string> options=null)
    {
      if (options == null)
        options = new Dictionary<string, string>();

      var urlBuilder = new StringBuilder(path);
      if (options.Count > 0)
      {
        urlBuilder.Append(path.IndexOf('?') > -1 ? "" : "?");
        urlBuilder.Append(HttpBuildQuery(options));
      }

      string url = urlBuilder.ToString();
      var webRequest = WebRequest.Create(url) as HttpWebRequest;
      webRequest.Method = method;

      string resultString;
      using (var webResponse = webRequest.GetResponse())
      {
        using (var stream = webResponse.GetResponseStream())
        using (var reader = new StreamReader(stream, Encoding))
        {
          resultString = reader.ReadToEnd();
        }
      }

      return resultString;
    }

    private string BuildUrl(string path, Dictionary<string, string> pars = null, bool includeVersion = true)
    {
      if (pars == null)
        pars = new Dictionary<string, string>();

      if (string.IsNullOrEmpty(_accessToken))
      {
        pars["consumer_key"] = _clientId;
      }

      StringBuilder urlBuilder;
      if (Regex.IsMatch(path, "^https?://"))
      {
        urlBuilder = new StringBuilder(path);
      }
      else
      {
        urlBuilder = new StringBuilder("https://");
        urlBuilder.Append(!Regex.IsMatch(path, "connect") ? "api." : "");
        urlBuilder.Append((_development)
          ? Domains[DEVELOPMENT]
          : Domains[PRODUCTION]);
        urlBuilder.Append("/");
        urlBuilder.Append(includeVersion ? "v" + APIVERSION + "/" : "");
        urlBuilder.Append(path);
      }

      urlBuilder.Append((pars.Count > 0) ? "?" + HttpBuildQuery(pars) : "");
      return urlBuilder.ToString();
    }

    private string GetAccessTokenInternal(Dictionary<string, string> postData)
    {
      string url = GetAccessTokenUrl(postData);
      string resultString = Request(url, "POST", null);
      var response = JsonHelper.Deserialize<AccessTokenData>(resultString);
      _accessToken = response.AccessToken;
      return _accessToken;
    }
  }
}