using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    [DataContract]
    public class Connection : SoundCloudClient
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; set; }

        [DataMember(Name = "created_at")] 
        private string creationDate;
        public DateTime CreationDate
        {
            get { return (DateTime.Parse(creationDate)); }
            set { creationDate = value.ToString(); }
        }

        [DataMember(Name = "service")]
        public string Service { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "post_favorite")]
        public bool PostFavorite { get; set; }

        [DataMember(Name = "post_publish")]
        public bool PostPublish { get; set; }

        [DataMember(Name = "uri")]
        public Uri Uri { get; set; }

        public static List<Connection> MyConnection()
        {
            return SoundCloudApi.ApiAction<List<Connection>>(ApiCommand.MeConnection);
        }
    }
}