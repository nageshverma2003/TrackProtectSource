using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using TrackProtect.SoundCloud.Framework;

namespace TrackProtect.Social
{
    public partial class soundcloud : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string scope = Request.QueryString["scope"];

            string access_token = Request.QueryString["access_token"];

            if (!string.IsNullOrEmpty(access_token))
            {
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);

                    ClientInfo ci = db.GetClientInfo(ui.UserId);

                    db.UpdateSocialCredential(ci.ClientId, SocialConnector.SoundCloud, "access_token", access_token);

                    db.UpdateSocialCredential(ci.ClientId, SocialConnector.SoundCloud, "scope", scope);

                    db.UpdateSoundCloudId(ci.ClientId, GetUserData(access_token));
                }
            }
        }

        protected string GetUserData(string accessToken)
        {
            IDictionary<String, String> extraData = new Dictionary<String, String>();

            var webRequest = (HttpWebRequest)WebRequest.Create("https://api.soundcloud.com/me.json?oauth_token=" + accessToken);
            webRequest.Method = "GET";
            string response = "";
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
            }

            var json = JObject.Parse(response);
            //string id = (string)json["id"];
            string username = (string)json["username"];
            //string permalinkUrl = (string)json["permalink_url"];

            //   extraData = new Dictionary<String, String>
            //{
            //    {"SCAccessToken", accessToken},
            //    {"username", username}, 
            //    {"permalinkUrl", permalinkUrl}, 
            //    {"id", id}                                           
            //};

            return username;
        }
    }
}