using System;
using Newtonsoft.Json;
using TrackProtect.SoundCloud.Core;

namespace TrackProtect.SoundCloud.Json
{
  public sealed class JsonHelper
  {
    public static T Deserialize<T>(string text)
    {
      T resultObject;

      try
      {
        resultObject = JsonConvert.DeserializeObject<T>(text);
      }
      catch (Exception ex)
      {
        throw new DeserializeException(typeof(T), text, ex);
      }

      return resultObject;
    }

    public static string Serialize<T>(T obj)
    {
      string text;

      try
      {
        text = JsonConvert.SerializeObject(obj);
      }
      catch (Exception ex)
      {
        throw new SerializeException(typeof(T), ex);
      }

      return text;
    }
  }
}