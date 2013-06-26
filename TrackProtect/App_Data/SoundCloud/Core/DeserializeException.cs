using System;
using System.Runtime.Serialization;

namespace TrackProtect.SoundCloud.Core
{
  [Serializable]
  internal class DeserializeException : FrameworkException
  {
    public DeserializeException(Type objectType, string objectString)
    {
      ObjectType = objectType;
      ObjectString = objectString;
    }

    public DeserializeException(Type objectType, string objectString, Exception innerException) :
      base(null, innerException)
    {
      ObjectType = objectType;
      ObjectString = objectString;
    }

    public Type ObjectType { get; private set; }
    public string ObjectString { get; private set; }

    public override string Message
    {
      get
      {
        return string.Format(
          "Deserialize failed.{0}Type: {1}{0}String: {2}",
          Environment.NewLine,
          ObjectType,
          ObjectString);
      }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ObjectType", ObjectType);
      info.AddValue("ObjectString", ObjectString);
    }
  }
}