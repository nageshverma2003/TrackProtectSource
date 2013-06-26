using TrackProtect.SoundCloud.Json;
using TrackProtect.SoundCloud.Net;

namespace TrackProtect.SoundCloud.API.Console
{
  public class MeClient
  {
    const string PATH = "https://api.soundcloud.com/me";

    readonly string _oauth;

    public MeClient(string token)
    {
      _oauth = "oauth_token=" + token;
    }

    public MeData GetMe()
    {
      const string pattern = PATH + ".json";
      string requestUriString = pattern;
      IRequestInfo requestInfo = new RequestInfoImp()
      {
        UriString = RequestUtility.GetOAuthRequestUriString(_oauth, requestUriString)
      };

      string json = RequestUtility.GetResponseString(requestInfo);
      var result = JsonHelper.Deserialize<MeData>(json);
      return result;
    }
  }
}
