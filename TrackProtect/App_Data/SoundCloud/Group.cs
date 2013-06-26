using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    [DataContract]
    public class Group : SoundCloudClient
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "created_at")] 
        private string creationDate;
        public DateTime CreationDate
        {
            get { return (DateTime.Parse(creationDate)); }
            set { creationDate = value.ToString(); }
        }

        [DataMember(Name = "permalink")]
        public string Permalink { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "short_description")]
        public string ShortDescription { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "artwork_url")]
        public string Artwork { get; set; }

        [DataMember(Name = "permalink_url")]
        public string PermalinkUrl { get; set; }

        [DataMember(Name = "creator")]
        public User User { get; set; }

        public List<User> GetUsers()
        {
            return SoundCloudApi.ApiAction<List<User>>(ApiCommand.GroupUsers, this.Id);
        }

        public List<User> GetModerators()
        {
            return SoundCloudApi.ApiAction<List<User>>(ApiCommand.GroupModerators, this.Id);
        }

        public List<User> GetMembers()
        {
            return SoundCloudApi.ApiAction<List<User>>(ApiCommand.GroupMembers, this.Id);
        }

        public List<User> GetContributors()
        {
            return SoundCloudApi.ApiAction<List<User>>(ApiCommand.GroupContributors, this.Id);
        }

        public List<Track> GetTracks()
        {
            return SoundCloudApi.ApiAction<List<Track>>(ApiCommand.GroupTracks, this.Id);
        }

        public static List<Group> GetAllGroups()
        {
            return SoundCloudApi.ApiAction<List<Group>>(ApiCommand.Groups);
        }

        public static Group GetGroup(int id)
        {
            return SoundCloudApi.ApiAction<Group>(ApiCommand.Group, id);
        }

        public static List<Group> Search(string term)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("q", term);
            return SoundCloudApi.ApiAction<List<Group>>(ApiCommand.Groups, parameters);
        }
    }
}