using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// SoundCloud application.
    /// </summary>
    [DataContract]
    public class SoundCloudApplication
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        #region Links

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "permalink_url")]
        public string PermalinkUrl { get; set; }

        #endregion Links
    }
}