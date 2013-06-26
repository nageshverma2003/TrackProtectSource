using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using TrackProtect.SoundCloud.Json;
using TrackProtect.SoundCloud.Net;
using TrackProtect.UploadHelper;

namespace TrackProtect.SoundCloud.API.Console
{
    public class TracksClient
    {
        const string PATH = "https://api.soundcloud.com/tracks";
        readonly string _oauth;
        private string _token = string.Empty;

        public TracksClient(string token)
        {
            _token = token;
            _oauth = "oauth_token=" + token;
        }

        public TrackData GetTrack(long trackId)
        {
            string pattern = PATH + "/{0}.json";

            string requestUriString = string.Format(pattern, trackId);
            IRequestInfo requestInfo = new RequestInfoImp()
            {
                UriString = RequestUtility.GetOAuthRequestUriString(_oauth, requestUriString),
            };

            string json = RequestUtility.GetResponseString(requestInfo);
            return JsonHelper.Deserialize<TrackData>(json);
        }

        const string URL_SOUNDCLOUD_TRACKS = "https://api.soundcloud.com/tracks";
        public string UploadFile(string filename, string title, bool isPublic)
        {
            string ret = string.Empty;
            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(URL_SOUNDCLOUD_TRACKS) as HttpWebRequest;

            // Set some default headers
            request.Accept = "*/*";
            request.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
            request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8,ru;q=0.6");

            var files = new UploadFile[] {
            //new UploadFile(HttpContext.Current.Server.MapPath(filename), "track[asset_data]", "application/octet-stream")
            new UploadFile(filename, "track[asset_data]", "application/octet-stream")
          };

            var form = new NameValueCollection();
            form.Add("track[title]", title);
            form.Add("track[sharing]", isPublic ? "public" : "private");
            form.Add("oauth_token", _token);
            form.Add("format", "json");
            form.Add("Filename", Path.GetFileName(filename));
            form.Add("Upload", "Submit Query");

            try
            {
                using (var response = HttpUploadHelper.Upload(request, files, form))
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    ret = reader.ReadToEnd();
                }

                return ret;
            }
            catch
            {
                return "Error";
            }
        }
    }
}