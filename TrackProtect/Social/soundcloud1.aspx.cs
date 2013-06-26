using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;


namespace TrackProtect.Social
{
    public partial class soundcloud1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string retUrl = Request.Params["url"].Trim('#');

            string accessToken = string.Empty;

            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                string[] kvps = retUrl.Split('&');
                foreach (string kvp in kvps)
                {
                    string[] parts = kvp.Split('=');
                    if (parts.Length == 2)
                    {
                        string key = parts[0];
                        string val = parts[1];

                        if (val.EndsWith(",/social/soundcloud.aspx"))
                            val = val.Substring(0, val.Length - ",/social/soundcloud.aspx".Length);

                        if (string.Compare(key, "access_token", true) == 0)
                            accessToken = val;
                        db.UpdateSocialCredential(ci.ClientId, SocialConnector.SoundCloud, key, val);
                    }
                }
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                string userName = GetUserData(accessToken);

                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);

                    ClientInfo ci = db.GetClientInfo(ui.UserId);

                    db.UpdateSoundCloudId(ci.ClientId, userName);

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