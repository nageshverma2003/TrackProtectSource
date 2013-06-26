using TrackProtect.SoundCloud.Json;
using TrackProtect.SoundCloud.Net;

namespace TrackProtect.SoundCloud.API.Console
{
  public class UsersClient
  {
    const string PATH = "https://api.soundcloud.com/users";
    readonly string _oauth;

    public UsersClient(string token)
    {
      _oauth = "oauth_token=" + token;
    }

    public UserData GetUser(long userId)
    {
      const string pattern = PATH + "/{0}.json";
      string requestUriString = string.Format(pattern, userId);
      IRequestInfo requestInfo = new RequestInfoImp()
      {
        UriString = RequestUtility.GetOAuthRequestUriString(_oauth, requestUriString)
      };

      string json = RequestUtility.GetResponseString(requestInfo);
      var result = JsonHelper.Deserialize<UserData>(json);
      return result;
    }

    public TrackData[] GetTracks(long userId)
    {
      const string pattern = PATH + "/{0}/tracks.json";
      string requestUriString = string.Format(pattern, userId);
      IRequestInfo requestInfo = new RequestInfoImp()
      {
        UriString = RequestUtility.GetOAuthRequestUriString(_oauth, requestUriString)
      };

      string json = RequestUtility.GetResponseString(requestInfo);
      var result = JsonHelper.Deserialize<TrackData[]>(json);
      return result;
    }
  }
}