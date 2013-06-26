using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Facebook;
using Facebook;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Data;

namespace TrackProtect.Member
{
    public partial class InviteFriends : System.Web.UI.Page
    {
        protected void Page_PreRender(Object o, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                bool isNotExpired = true;

                Facebook.AuthenticationService authService = new Facebook.AuthenticationService();

                Facebook.Me me;
                string accessToken = string.Empty;

                if (authService.TryAuthenticate(out me, out accessToken))
                {
                    isNotExpired = true;
                }
                else
                {
                    db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);
                    db.UpdateFacebookID(ci.ClientId);

                    isNotExpired = false;
                }

                if (!string.IsNullOrEmpty(ci.SoundCloudId))
                    SoundcloudItag.Attributes.Add("class", "soundcloud");
                else
                    SoundcloudItag.Attributes.Add("class", "soundcloud disabled");

                if (isNotExpired)
                    FacebookHeading.Attributes.Add("class", "social facebook");
                else
                    FacebookHeading.Attributes.Add("class", "social facebook disabled");

                if (!string.IsNullOrEmpty(ci.TwitterId))
                    TwitterHeading.Attributes.Add("class", "social twitter");
                else
                    TwitterHeading.Attributes.Add("class", "social twitter disabled");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "user-home";

            if (!IsPostBack)
            {
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

                    DataSet ds = db.GetRegister(Util.UserId);
                    int protectedTracks = ds.Tables[0].Rows.Count;

                    LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                    LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                    CreditsLiteral.Text = Util.GetUserCredits(Util.UserId).ToString();
                    ProtectedLiteral.Text = protectedTracks.ToString();
                    decimal percentComplete = 0m;
                    if (Session["percentComplete"] != null)
                        percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                    CompletedLiteral.Text = string.Empty;
                    if (percentComplete < 100)
                        CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                    divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
                }

                //using (Database db = new MySqlDatabase())
                //{
                //    ClientInfo ci = db.GetClientInfo(Util.UserId);
                //    string accessToken = db.GetSocialCredential(ci.ClientId, SocialConnector.Facebook, "accesstoken");

                //    FriendService fbFriendService = new FriendService(accessToken);

                //    //GetFacebookFriendList(accessToken);

                //    List<Friend> friendList = fbFriendService.Get();
                //}
            }


            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
            }
        }

        #region To get friend list ------- !

        public static IList<Friend> GetFacebookFriendList(string accessToken)
        {
            try
            {
                IList<Friend> faceBookUserList = new List<Friend>();

                FacebookClient client = new FacebookClient(accessToken);

                var friendListDataString = client.Get("/me/friends");

                MemoryStream ms =
                    new MemoryStream(Encoding.Default.GetBytes(Convert.ToString(friendListDataString).ToCharArray()));

                StreamReader sr = new StreamReader(ms);

                Dictionary<string, object> facebookJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());

                foreach (KeyValuePair<string, object> json in facebookJson)
                {
                    switch (json.Key.ToLower())
                    {
                        case "data":

                            List<object> dataList =
                                JsonConvert.DeserializeObject<List<object>>(json.Value.ToString());

                            int index = 0;

                            while (index != dataList.Count())
                            {
                                Dictionary<string, string> friendDictionary =
                                    JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(dataList[index]));

                                Friend fbUserObj = new Friend();

                                foreach (KeyValuePair<string, string> friend in friendDictionary)
                                {
                                    if (friend.Key == "name")
                                        fbUserObj.Name = friend.Value;
                                    else
                                    {
                                        fbUserObj.FacebookId = friend.Value;
                                        //fbUserObj.ProfileImg = string.Format("https://graph.facebook.com/{0}/picture", friend.Value);
                                    }
                                }

                                try
                                {
                                    faceBookUserList.Add(fbUserObj);
                                }
                                catch (Exception ex)
                                {
                                    //Utilities.ErrorLogger.LogException(ex, ex.Message);
                                }

                                index++;
                            }

                            break;

                        //case "paging":

                        //    break;
                    }
                }

                return faceBookUserList;
            }
            catch (Exception ex)
            {
                //Utilities.ErrorLogger.LogException(ex, ex.Message);

                return null;
            }
        }

        #endregion
    }
}