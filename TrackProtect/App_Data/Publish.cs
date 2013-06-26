using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

//using TrackProtect.App_Data;
using TrackProtect.SoundCloud.API.Console;
using TrackProtect.UploadHelper;

//using Facebook;
//using Facebook.Reflection;
//using Facebook.Web;
using TrackProtect.Facebook;

using TrackProtect.SoundCloud;
using TrackProtect.SoundCloud.Core;
using TrackProtect.SoundCloud.Framework;
using TrackProtect.SoundCloud.Json;
using TrackProtect.SoundCloud.Net;

using TrackProtect.Logging;
using System.Threading.Tasks;

namespace TrackProtect
{
    public class Publish
    {
        //private static IOAuth1ServiceProvider<ITwitter> twitterProvider = null;

        //public static bool TwitterAuth(HttpContext context, out OAuthToken accessToken, out string oauthVerifier, out TwitterProfile me)
        //{
        //    //TrackProtect.Twitter.AuthenticationService authentication = new TrackProtect.Twitter.AuthenticationService();
        //    accessToken = null;
        //    oauthVerifier = null;
        //    me = null;
        //    //OAuthToken requestToken;
        //    //if (!authentication.TryAuthentication(out requestToken, out oauthVerifier))
        //    //{
        //    //    authentication.Authenticate(context);
        //    //    return false;
        //    //}
        //    //authentication.Authorize(context, out me, out accessToken, out oauthVerifier);
        //    return true;
        //}

        //public static bool TwitterAuthorize(out TwitterProfile profile)
        //{
        //    profile = null;
        //    string key = GetConfiguration("twitter.app_id");
        //    string secret = GetConfiguration("twitter.app_secret");

        //    string twitterTarget = "/social/twitter.aspx";
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(HttpContext.Current.Request.Url.Scheme);
        //    sb.Append("://");
        //    sb.Append(HttpContext.Current.Request.Url.Authority);
        //    sb.Append(twitterTarget);

        //    twitterTarget = sb.ToString();
        //    twitterProvider = new TwitterServiceProvider(key, secret);
        //    NameValueCollection par = new NameValueCollection();
        //    par.Add("force_login", "true");
        //    OAuthToken requestToken = twitterProvider.OAuthOperations.FetchRequestTokenAsync(twitterTarget, par).Result;
        //    using (Database db = new MySqlDatabase())
        //    {
        //        ClientInfo ci = db.GetClientInfo(Util.UserId);

        //        db.UpdateSocialCredential(ci.ClientId, SocialConnector.Twitter, "requesttoken.value", requestToken.Value);
        //        db.UpdateSocialCredential(ci.ClientId, SocialConnector.Twitter, "requesttoken.secret", requestToken.Secret);
        //    }
        //    //OAuthToken accessToken = twitterProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, new NameValueCollection());
        //    //ITwitter twitter = twitterProvider.GetApi(accessToken.Value, accessToken.Secret);
        //    //TwitterProfile prof = twitter.UserOperations.GetUserProfileAsync().Result;
            
        //    return true;
        //}

        //public static void TwitterPublish(string text)
        //{
        //    OAuthToken requestToken = TwitterGetRequestToken();
        //    if (requestToken == null)
        //    {
        //        //TODO: TwitterAuthorize();
        //        requestToken = TwitterGetRequestToken();
        //    }

        //    if (requestToken == null)
        //        return; // Can't authorize, can't tweet

        //    string key = GetConfiguration("twitter.app_id");
        //    string secret = GetConfiguration("twitter.app_secret");
        //    twitterProvider = new TwitterServiceProvider(key, secret);

        //    HttpContext.Current.Session["TwitterRequestToken"] = requestToken;
        //    HttpContext.Current.Session["TwitterTweet"] = text;

        //    HttpContext.Current.Response.Redirect(twitterProvider.OAuthOperations.BuildAuthenticateUrl(requestToken.Value, null));
        //}

        //public static void TwitterCallback()
        //{
        //    string oauth_verifier;
        //    using (Database db = new MySqlDatabase())
        //    {
        //        ClientInfo ci = db.GetClientInfo(Util.UserId);
        //        oauth_verifier = db.GetSocialCredential(ci.ClientId, SocialConnector.Twitter, "oauth_verifier");
        //    }
        //    if (string.IsNullOrEmpty(oauth_verifier))
        //        return;

        //    string key = GetConfiguration("twitter.app_id");
        //    string secret = GetConfiguration("twitter.app_secret");
        //    twitterProvider = new TwitterServiceProvider(key, secret);

        //    OAuthToken requestToken = HttpContext.Current.Session["TwitterRequestToken"] as OAuthToken;
        //    AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(requestToken, oauth_verifier);
        //    OAuthToken token = twitterProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null).Result;

        //    HttpContext.Current.Session["TwitterAccessToken"] = token;

        //    ITwitter twitterClient = twitterProvider.GetApi(token.Value, token.Secret);
        //    //TwitterProfile profile = twitterClient.UserOperations.GetUserProfileAsync().Result;
        //    string tweet = HttpContext.Current.Session["TwitterTweet"] as string;

        //    twitterClient.TimelineOperations.UpdateStatusAsync(tweet);
        //}


        //public static void TwitterComplete()
        //{
        //}

        //private static OAuthToken TwitterGetRequestToken()
        //{
        //    string tokenValue = null, tokenSecret = null;
        //    using (Database db = new MySqlDatabase())
        //    {
        //        ClientInfo ci = db.GetClientInfo(Util.UserId);

        //        tokenValue = db.GetSocialCredential(ci.ClientId, SocialConnector.Twitter, "requesttoken.value");
        //        tokenSecret = db.GetSocialCredential(ci.ClientId, SocialConnector.Twitter, "requesttoken.secret");
        //    }

        //    if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(tokenSecret))
        //        return null;

        //    return new OAuthToken(tokenValue, tokenSecret);
        //}

        /// <summary>
        /// Authenticate with Facebook. When not logged in or never set app authorisation the page will be redirected to Facebook.
        /// </summary>
        /// <param name="context">Http Context</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="me">The user's information</param>
        /// <returns>True if succeed, false if not</returns>
        public static bool FacebookAuth(HttpContext context, out string accessToken, out Me me)
        {
            AuthenticationService authentication = new AuthenticationService();
            me = null;
            accessToken = "";

            if (!authentication.TryAuthenticate(out me, out accessToken))
            {
                authentication.Authenticate(context, out me, out accessToken);
                if (me == null)
                {
                    Logger.Instance.Write(LogLevel.Warning, "Cannot authenticate with facebook", new object[] { accessToken });
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Post a message to the Facebook wall
        /// </summary>
        /// <param name="account">Wall account to post to</param>
        /// <param name="message">Message title to post</param>
        /// <param name="description">Message description to post</param>
        /// <param name="caption">Message caption to post</param>
        public void Facebook(
            Facebook.Account account, 
            string message, 
            string description,
            string caption)
        {
            PostFeed feed = new PostFeed();
            feed.Message = message; // "I just released a new track!";
            feed.Description = description;
            feed.Link = "http://www.trackprotect.com";
            feed.Caption = caption; // "This track is protected by Trackprotect.com";

            Facebook.PostService postService = new PostService(account);
            postService.Post(feed);
        }

        #region Old Facebook code

        //public static void FacebookPublish(string key, string secret, string text)
        //{
        //    string accessToken = facebookGetAccessToken(key, secret);
        //    facebookPost(key, accessToken, "Hello from TrackProtect");
        //}

        //public static void FacebookCallback(string oauth_verifier)
        //{
        //}

        //public static void FacebookComplete()
        //{
        //}

        //private static string facebookGetAccessToken(string key, string secret)
        //{
        //    WebClient client = new WebClient();
        //    var uri = string.Concat(
        //        "https://graph.facebook.com/oauth/access_token?", 
        //        "client_id=", key, "&",
        //        "client_secret=", secret, "&",
        //        "grant_type=", "client_credentials"
        //    );

        //    var response = client.DownloadString(uri);
        //    if (string.IsNullOrEmpty(response))
        //        return null;

        //    return response.Split('=')[1];
        //}

        //private static string facebookPost(string key, string accessToken, string text)
        //{
        //        var fb = new FacebookClient(accessToken);
        //    var result = fb.Post(key + "/feed", new
        //    {
        //        message = text
        //    });
        //    string ret = result.ToString();
        //    return ret;
        //}

        #endregion

        public static string _soundcloudFilename = string.Empty;
        public static string _soundcloudTitle = string.Empty;

        public static string SoundCloudPublish(string accessToken, string filename, string title)
        {
            string soundcloudTarget = "/SoundCloud/SoundCloudCallback.aspx";
            StringBuilder sb = new StringBuilder();
            sb.Append(HttpContext.Current.Request.Url.Scheme);
            sb.Append("://");
            sb.Append(HttpContext.Current.Request.Url.Authority);
            sb.Append(soundcloudTarget);

            soundcloudTarget = sb.ToString();
            
            _soundcloudFilename = filename;
            _soundcloudTitle = title;
            /******************************************************************\
            Manager mgr = new Manager();
            OAuthResponse requestToken = 
                mgr.AcquireRequestToken("https://soundcloud.com/connect", "POST");
            var url = "http://soundcloud.com/oauth/authorize?oauth_token=" + 
                mgr.Token;
            HttpContext.Current.Response.Redirect(url);
            \******************************************************************/
            string redirectUri = soundcloudTarget;

            SoundCloudClient client = new SoundCloudClient(string.Empty, string.Empty, redirectUri);
            //string accessToken = client.GetAccessToken();
            TracksClient tracks = new TracksClient(accessToken);
            return tracks.UploadFile(_soundcloudFilename, _soundcloudTitle, true);
        }

        public static void SoundCloudCallback(string oauth_verifier)
        {
        }

        public static void SoundCloudComplete()
        {
      
        }

        private static string GetConfiguration(string key)
        {
            string res = null;
            using (Database db = new MySqlDatabase())
            {
                res = db.GetSetting(key);
            }
            return res;
        }
    }
}