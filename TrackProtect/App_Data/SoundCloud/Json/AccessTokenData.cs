using Newtonsoft.Json;

namespace TrackProtect.SoundCloud.Json
{
  [JsonObject]
  internal class AccessTokenData
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; internal set; }

    [JsonProperty("scope")]
    public string Scope { get; internal set; }
  }
}