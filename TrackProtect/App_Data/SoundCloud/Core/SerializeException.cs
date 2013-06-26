using System;
using System.Runtime.Serialization;

namespace TrackProtect.SoundCloud.Core
{
  [Serializable]
  internal class SerializeException : FrameworkException
  {
    public SerializeException(Type objectType)
    {
      ObjectType = objectType;
    }

    public SerializeException(Type objectType, Exception innerException) :
      base(null, innerException)
    {
      ObjectType = objectType;
    }

    public Type ObjectType { get; private set; }

    public override string Message
    {
      get
      {
        return string.Format(
          "Serialize failed.{0}Type: {1}",
          Environment.NewLine,
          ObjectType
          );
      }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ObjectType", ObjectType);
    }
  }
}