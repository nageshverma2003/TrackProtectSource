using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    [DataContract]
    public class Comment : SoundCloudClient
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "track_id")]
        public int TrackId { get; set; }

        [DataMember(Name = "created_at")] 
        private string creationDate;
        public DateTime CreationDate
        {
            get { return (DateTime.Parse(creationDate)); }
            set { creationDate = value.ToString(); }
        }

        [DataMember(Name = "timestamp")]
        public int Timestamp { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "user_id")]
        public int UserId { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        public static Comment GetComment(int id)
        {
            return SoundCloudApi.ApiAction<Comment>(ApiCommand.Comment);
        }
    }
}