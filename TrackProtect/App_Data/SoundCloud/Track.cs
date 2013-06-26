﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// SoundCloud track.
    /// </summary>
    [DataContract]
    public class Track : SoundCloudClient
    {
        #region Properties

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "created_at")]
        private string creationDate;

        public DateTime CreationDate { get { return (DateTime.Parse(creationDate)); } set { creationDate = value.ToString(); } }

        [DataMember(Name = "user_id")]
        public int UserId { get; set; }

        [DataMember(Name = "duration")]
        public int Duration { get; set; }

        [DataMember(Name = "commentable")]
        public bool Commentable { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "sharing")]
        public string Sharing { get; set; }

        [DataMember(Name = "tag_list")]
        public string TagList { get; set; }

        [DataMember(Name = "permalink")]
        public string Permalink { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "streamable")]
        public bool Streamable { get; set; }

        [DataMember(Name = "downloadable")]
        public bool Downloadable { get; set; }

        [DataMember(Name = "genre")]
        public string Genre { get; set; }

        [DataMember(Name = "release")]
        public string Release { get; set; }

        [DataMember(Name = "purchase_url")]
        public string PurchaseUrl { get; set; }

        [DataMember(Name = "label_id")]
        public string LabelId { get; set; }

        [DataMember(Name = "label_name")]
        public string LabelName { get; set; }

        [DataMember(Name = "isrc")]
        public string Isrc { get; set; }

        [DataMember(Name = "video_url")]
        public string VideoUrl { get; set; }

        [DataMember(Name = "track_type")]
        public string TrackType { get; set; }

        [DataMember(Name = "key_signature")]
        public string KeySignature { get; set; }

        [DataMember(Name = "bpm")]
        public string Bpm { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "release_year")]
        public string ReleaseYear { get; set; }

        [DataMember(Name = "release_month")]
        public string ReleaseMonth { get; set; }

        [DataMember(Name = "release_day")]
        public string ReleaseDay { get; set; }

        [DataMember(Name = "original_format")]
        public string OriginalFormat { get; set; }

        [DataMember(Name = "license")]
        public string License { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "permalink_url")]
        public string PermalinkUrl { get; set; }

        [DataMember(Name = "artwork_url")]
        public string Artwork { get; set; }

        [DataMember(Name = "waveform_url")]
        public string WaveForm { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "stream_url")]
        public string StreamUrl { get; set; }

        [DataMember(Name = "download_url")]
        public string DownloadUrl { get; set; }

        [DataMember(Name = "downloads_remaining")]
        public int DownloadsRemaining { get; set; }

        [DataMember(Name = "secret_token")]
        public string SecretToken { get; set; }

        [DataMember(Name = "secret_uri")]
        public string SecretUri { get; set; }

        [DataMember(Name = "user_playback_count")]
        public int UserPlaybackCount { get; set; }

        [DataMember(Name = "user_favorite")]
        public bool UserFavorite { get; set; }

        [DataMember(Name = "playback_count")]
        public int PlaybackCount { get; set; }

        [DataMember(Name = "download_count")]
        public int DownloadCount { get; set; }

        [DataMember(Name = "favoritings_count")]
        public int FavoritingsCount { get; set; }

        [DataMember(Name = "comment_count")]
        public int CommentsCount { get; set; }

        [DataMember(Name = "attachments_uri")]
        public string AttachmentUri { get; set; }

        #endregion Properties

        #region Shared Methods

        /// <summary>
        /// Returns a collection of tracks uploaded by logged-in user.
        /// </summary>
        public static List<Track> MyTacks()
        {
            return SoundCloudApi.ApiAction<List<Track>>(ApiCommand.MeTracks);
        }

        /// <summary>
        /// Returns a collection of tracks.
        /// </summary>
        public static List<Track> GetTracks()
        {
            return SoundCloudApi.ApiAction<List<Track>>(ApiCommand.Tracks);
        }

        /// <summary>
        /// Returns a track by track id.
        /// </summary>
        /// 
        /// <param name="id">Track id.</param>
        public static Track GetTrack(int id)
        {
            return SoundCloudApi.ApiAction<Track>(ApiCommand.Track, id);
        }

        /// <summary>
        /// Returns a collection of tracks after filtering.
        /// </summary>
        /// 
        /// <param name="term"></param>
        /// <param name="tags"></param>
        /// <param name="filter"></param>
        /// <param name="license"></param>
        /// <param name="order"></param>
        /// <param name="bpmFrom"></param>
        /// <param name="bpmTo"></param>
        /// <param name="durationFrom"></param>
        /// <param name="durationTo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ids"></param>
        /// <param name="genres"></param>
        /// <param name="types"></param>
        public static List<Track> Search(string term, string[] tags, Filter filter, string license, string order, int? bpmFrom, int? bpmTo, int? durationFrom, int? durationTo, DateTime from, DateTime to, int[] ids, string[] genres, string[] types)
        {
            Dictionary<string, object> filters = new Dictionary<string, object>();

            if (term != null)
            {
                filters.Add("q", term);
            }
            if (tags != null && tags.Length > 0)
            {
                if (tags.Length == 1)
                    filters.Add("tags", tags[0]);
                else
                {
                    string t = string.Empty;

                    foreach (string tag in tags)
                    {
                        t += string.Concat(tag, ",");
                    }

                    filters.Add("tags", t);
                }
            }
            if (filter != null)
            {
                filters.Add("filter", filter.ToString().ToLower());
            }
            if (license != null)
            {
                filters.Add("license", license);
            }
            if (order != null)
            {
                filters.Add("order", order);
            }
            if (bpmFrom != null && bpmTo != null)
            {
                filters.Add("bpm[from]", bpmFrom);
                filters.Add("bpm[to]", bpmTo);
            }
            if (durationFrom != null && durationTo != null)
            {
                filters.Add("duration[from]", durationFrom);
                filters.Add("duration[to]", durationTo);
            }
            if (from != null && to != null)
            {
                filters.Add("created_at[from]", from.ToString("yyyy/MM/dd hh:mm:ss"));
                filters.Add("created_at[to]", to.ToString("yyyy/MM/dd hh:mm:ss"));
            }
            if (ids != null && ids.Length > 0)
            {
                if (ids.Length == 1)
                    filters.Add("ids", ids[0]);
                else
                {
                    string i = string.Empty;

                    foreach (int id in ids)
                    {
                        i += string.Concat(id, ",");
                    }
                }
            }
            if (genres != null && genres.Length > 0)
            {
                if (genres.Length == 1)
                    filters.Add("genres", genres[0]);
                else
                {
                    string g = string.Empty;

                    foreach (string genre in genres)
                    {
                        g += string.Concat(genre, ",");
                    }
                }
            }
            if (types != null && types.Length > 0)
            {
                if (types.Length == 1)
                    filters.Add("types", types[0]);
                else
                {
                    string t = string.Empty;

                    foreach (string type in types)
                    {
                        t += string.Concat(type, ",");
                    }
                }
            }

            return SoundCloudApi.ApiAction<List<Track>>(ApiCommand.Tracks, filters);
        }

        #endregion

        /// <summary>
        /// Upload a track to sound cloud.
        /// </summary>
        /// 
        /// <remarks>
        /// To upload a track, you have to specify the following properties :
        ///  * Uri : the path of the file on your computer.
        ///  * Title : title of the track.
        ///  * Description : a brief description of the track.
        ///  * Sharing : public or private.
        /// </remarks>
        public void Add()
        {
            if (this.Uri != null && this.Title != null && this.Description != null && this.Sharing != null)
            {
                if (File.Exists(Path.GetFullPath(this.Uri)))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();

                    WebClient client = new WebClient();

                    parameters.Add("track[asset_data]", Path.GetFullPath(this.Uri));
                    parameters.Add("track[title]", this.Title);
                    parameters.Add("track[description]", this.Description);
                    parameters.Add("track[sharing]", this.Sharing);

                    SoundCloudApi.ApiAction<Track>(ApiCommand.Tracks, HttpMethod.Post, parameters);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Deletes a given track.
        /// </summary>
        /// 
        /// <remarks>To delete a track, you have to be the owner or you have permission to delete it.</remarks>
        public void Delete()
        {
            SoundCloudApi.ApiAction<Track>(ApiCommand.Track, HttpMethod.Delete, this.Id);
        }

        /// <summary>
        /// Returns comments of a track by track id.
        /// </summary>
        /// 
        /// <param name="id">Track id.</param>
        public List<Comment> GetComments()
        {
            return SoundCloudApi.ApiAction<List<Comment>>(ApiCommand.TrackComments, this.Id);
        }

        /// <summary>
        /// Returns all users with permission for a track by track id.
        /// </summary>
        /// 
        /// <param name="id">Track id.</param>
        public List<User> GetPermissions()
        {
            return SoundCloudApi.ApiAction<List<User>>(ApiCommand.TrackPermissions, this.Id);
        }

        /// <summary>
        /// Adds the given track to the logged-in user's list of favorites.
        /// </summary>
        /// 
        /// <param name="id">Track id.</param>
        public void AddToFavorites()
        {
            SoundCloudApi.ApiAction<Track>(ApiCommand.MeFavoritesTrack, HttpMethod.Put, this.Id);
        }

        /// <summary>
        /// Deletes the given track from the logged-in user's list of favorites.
        /// </summary>
        /// 
        /// <param name="id">Track id.</param>
        public void RemoveFromFavorites()
        {
            SoundCloudApi.ApiAction<Track>(ApiCommand.MeFavoritesTrack, HttpMethod.Delete, this.Id);
        }

        /// <summary>
        /// Share a track to a social network.
        /// </summary>
        /// 
        /// <param name="connection">Registered social profile on sound cloud.</param>
        public void Share(Connection connection)
        {
            Share(connection, null);
        }

        /// <summary>
        /// Share a track to a social network.
        /// </summary>
        /// 
        /// <param name="connection">Registered social profile on sound cloud.</param>
        /// <param name="sharingNote">String that will be used as status message. This string might be truncated by SoundCloud.</param>
        public void Share(Connection connection, string sharingNote)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("connections[][id]", connection.Id);

            if (sharingNote != null) parameters.Add("sharing_note", sharingNote);

            SoundCloudApi.ApiAction<Track>(ApiCommand.TrackShare, HttpMethod.Post, parameters, this.Id);
        }
    }