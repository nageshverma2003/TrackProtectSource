using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TrackProtect.Logging;

namespace TrackProtect.Facebook
{
    /// <summary>
    /// Provides a service for Facebook posts
    /// </summary>
    public class PostService
    {
        private const string FACEBOOK_POST = "https://graph.facebook.com/{0}/feed?access_token={1}";

        private string _accessToken;
        private string _id;

        /// <summary>
        /// Constructor for PostService. Post to personal page.
        /// </summary>
        /// <param name="accessToken">Access token for authentication</param>
        public PostService(string accessToken)
        {
            _accessToken = accessToken;
            _id = "me";
        }

        /// <summary>
        /// Constructor for PostService. Post on given user account page.
        /// </summary>
        /// <param name="account">User account to post on</param>
        public PostService(Account account)
        {
            _accessToken = account.Access_Token;
            _id = account.Id;
        }

        public string Post(IFacebookFeed feed)
        {
            try
            {
                WebClient webClient = new WebClient();
                string request = String.Format("https://graph.facebook.com/{0}/feed?access_token={1}{2}", _id, _accessToken, feed.GetQueryString());
                string rawResult = webClient.UploadString(request, "POST", "");
                JObject result = JObject.Parse(rawResult);
                return (string)result["Id"];
            }
            catch (Exception ex)
            {
                Log.Instance.Write(LogLevel.Error, ex);
                throw;
            }
        }
    }
}