using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    public class JsonSerializer
    {
        public static string Serialize<T>(T obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T));
            serializer.WriteObject(stream, obj);
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static T Deserialize<T>(string json)
        {
            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof (T));
            return (T) deserializer.ReadObject(stream);
        }
    }
}