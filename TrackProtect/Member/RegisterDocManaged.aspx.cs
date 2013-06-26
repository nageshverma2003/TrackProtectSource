using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using TrackProtect.Facebook;
using TrackProtect.Logging;
using Twitterizer;
using System.Configuration;
using Facebook;
using System.Dynamic;
using Newtonsoft.Json;

namespace TrackProtect.Member
{
    public partial class RegisterDocManaged : BasePage
    {
        #region Variables ------- !

        DataTable CoArtistsTable
        {
            get { return Session["CoArtistsTable"] as DataTable; }
            set { Session["CoArtistsTable"] = value; }
        }

        private ClientInfo _ManAccClientInfo = null;

        #endregion

        #region Page events ------- !

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
            Session["bodyid"] = "trackprotect";

            if (!IsPostBack)
            {
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);
                    _ManAccClientInfo = db.GetClientInfo(Util.UserId);

                    Session["TPForUser"] = _ManAccClientInfo.FirstName;


                    #region Fill Managed User Info

                    FillManagedArtistDropDown();

                    PopulateSocialMediaInfo(db);

                    #endregion

                    Genre1.DataSource = db.getGenreList();
                    Genre1.DataTextField = "Value";
                    Genre1.DataValueField = "Key";
                    Genre1.DataBind();
                    Genre1.Items.Insert(0, new ListItem("--Select--", null));

                    Genre2.DataSource = db.getGenreList();
                    Genre2.DataTextField = "Value";
                    Genre2.DataValueField = "Key";
                    Genre2.DataBind();
                    Genre2.Items.Insert(0, new ListItem("--Select--", null));

                    Genre3.DataSource = db.getGenreList();
                    Genre3.DataTextField = "Value";
                    Genre3.DataValueField = "Key";
                    Genre3.DataBind();
                    Genre3.Items.Insert(0, new ListItem("--Select--", null));

                    SubGenre1.DataSource = null;
                    SubGenre1.DataBind();
                    SubGenre1.Items.Insert(0, new ListItem("--Select--", null));

                    SubGenre2.DataSource = null;
                    SubGenre2.DataBind();
                    SubGenre2.Items.Insert(0, new ListItem("--Select--", null));

                    SubGenre3.DataSource = null;
                    SubGenre3.DataBind();
                    SubGenre3.Items.Insert(0, new ListItem("--Select--", null));

                    #region Control Panel information

                    DataSet ds = db.GetRegister(Util.UserId);
                    int protectedTracks = ds.Tables[0].Rows.Count;

                    LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                    LoggedOnUserName.Text = _ManAccClientInfo.FirstName;//_ManAccClientInfo.GetFullName();
                    CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
                    ProtectedLiteral.Text = Convert.ToString(protectedTracks);
                    decimal percentComplete = 0m;
                    if (Session["percentComplete"] != null)
                        percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                    CompletedLiteral.Text = string.Empty;
                    if (percentComplete < 100)
                        CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                    divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);

                    #endregion

                    #region Removed Code

                    //string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                    //string identDocName = string.Format("ID{0:D10}.cer", ui.UserId);
                    //string identDocPath = Path.Combine(userDocPath, identDocName);

                    //if (!File.Exists(Server.MapPath(identDocPath)))
                    //{
                    //    litDialogText.Text = Resources.Resource.InfoIncomplete;
                    //    Master.PrepareMessageBox("Message", Resources.Resource.InfoIncomplete);
                    //}

                    #endregion

                    ShareToMyWall.Text = string.Format(Resources.Resource.ShareToMyWall, _ManAccClientInfo.FirstName);
                    ShareToMyFriends.Text = string.Format(Resources.Resource.ShareToMyFriends, _ManAccClientInfo.FirstName);
                    ShareToMyPages.Text = string.Format(Resources.Resource.ShareToMyPages, _ManAccClientInfo.FirstName);
                }

                Session["CoArtistsTable"] = null;

                Session["ReturnUrl"] = Request.QueryString["ReturnUrl"];
                CoArtistsTable = CreateCoArtistsTable();

                long userId = Util.UserId;
                int credits = Util.GetUserCredits(userId);
                bool controlsEnabled = (credits > 0);
                FileUpload1.Enabled = controlsEnabled;
                FileUpload2.Enabled = controlsEnabled;
                FileUpload3.Enabled = controlsEnabled;
                RegisterDocumentButton.Enabled = controlsEnabled;

                FillCoArtistsDropDown();
            }

            //------- Highlight the selected lang button ------- !

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
            }

            ClientScript.RegisterStartupScript(this.GetType(), "HighLightMenu", "HighLightMenu('" + "Menu3" + "');", true);
        }

        #endregion

        #region Server Validations(Removed Code) ------- !

        //protected void checkFileSize_Validate(object sender, ServerValidateEventArgs args)
        //{
        //    //string data = args.Value;

        //    args.IsValid = false;

        //    double fileSize = FileUpload1.FileContent.Length;

        //    if (fileSize > 12582912)
        //    {
        //        args.IsValid = false;
        //    }
        //    else
        //    {
        //        args.IsValid = true;
        //    }
        //}

        //protected void SoundCloudMaxLength_ServerValidate(object sender, ServerValidateEventArgs args)
        //{
        //    args.IsValid = false;

        //    if (SoundCloudMsg.Text.Length > 140)
        //        args.IsValid = false;
        //    else
        //        args.IsValid = true;
        //}

        //protected void FacebookMaxLength_ServerValidate(object sender, ServerValidateEventArgs args)
        //{
        //    args.IsValid = false;

        //    if (FacebookMsg.Text.Length > 140)
        //        args.IsValid = false;
        //    else
        //        args.IsValid = true;
        //}

        //protected void TwitterMaxLength_ServerValidate(object sender, ServerValidateEventArgs args)
        //{
        //    args.IsValid = false;

        //    if (TwitterMsg.Text.Length > 140)
        //        args.IsValid = false;
        //    else
        //        args.IsValid = true;
        //}

        #endregion

        #region Methods ------- !

        private void PopulateSocialMediaInfo(Database db)
        {
            ClientInfo _ManagedUserInfo = db.GetClientInfo(Convert.ToInt64(Session["managed.userid"]));

            ViewState["isSoundCloudActive"] = FillSoundCloudInfo(_ManagedUserInfo);

            if (Convert.ToBoolean(ViewState["isSoundCloudActive"]) == true)
            {
                FillFacebookInfo(_ManagedUserInfo);

                FillTwitterInfo(_ManagedUserInfo);

                _ManAccClientInfo = db.GetClientInfo(Util.UserId);

                FillOwnFacebookInfo();

                if (string.IsNullOrEmpty(_ManAccClientInfo.FacebookId) && string.IsNullOrEmpty(_ManagedUserInfo.FacebookId))
                {
                    cbxSendToFacebook.Enabled = false;

                    FacebookMsg.Enabled = false;

                    cbxSendToFacebook.Visible = false;

                    ViewState["isFacebookActive"] = false;
                }
                else
                {
                    cbxSendToFacebook.Enabled = true;

                    FacebookMsg.Enabled = true;

                    cbxSendToFacebook.Visible = true;

                    ViewState["isFacebookActive"] = true;
                }
            }
            else
            {
                cbxSendToFacebook.Enabled = false;

                FacebookMsg.Enabled = false;

                cbxSendToFacebook.Visible = false;

                #region Deactivate Facebook

                Session["FBACC"] = Session["FBID"] = null;

                FillFBFriendList(null);

                FillFBPageList(null);

                cbxShareToUserWall.Enabled = false;

                cbxShareToUserWall.Visible = false;

                divShareToUserWall.Visible = false;

                #endregion

                #region Deactivate Facebook Friends

                divShareToFriends.Visible = false;

                cbxShareToFriends.Enabled = false;

                cbxShareToFriends.Visible = false;

                dlFriendList.Enabled = false;

                #endregion

                #region Deactivate Facebook Pages

                divShareToPages.Visible = false;

                cbxShareToPages.Enabled = false;

                cbxShareToPages.Visible = false;

                dlPageList.Enabled = false;

                #endregion

                #region Deactivate Own Facebook

                Session["FBOwnACC"] = Session["FBOwnID"] = null;

                cbxShareToMyWall.Enabled = false;

                cbxShareToMyWall.Visible = false;

                divShareToMyWall.Visible = false;

                #endregion

                #region Deactivate Own Facebook Friends

                divShareToOwnFriends.Visible = false;

                cbxShareToOwnFriends.Enabled = false;

                cbxShareToOwnFriends.Visible = false;

                dlOwnFriendList.Enabled = false;

                #endregion

                #region Deactivate  Own Facebook Pages

                divShareToOwnPages.Visible = false;

                cbxShareToOwnPages.Enabled = false;

                cbxShareToOwnPages.Visible = false;

                dlOwnPageList.Enabled = false;

                #endregion

                #region Deactivate Twitter

                Session["AccessToken"] = Session["AccessTokenSecret"] = null;

                cbxSendToTwitter.Enabled = false;

                TwitterMsg.Enabled = false;

                cbxSendToTwitter.Visible = false;

                #endregion
            }

            ShareToUserWall.Text = string.Format(Resources.Resource.ShareToUserWall, _ManagedUserInfo.FirstName);

            ShareToFriends.Text = string.Format(Resources.Resource.ShareToFriends, _ManagedUserInfo.FirstName);

            ShareToPages.Text = string.Format(Resources.Resource.ShareToPages, _ManagedUserInfo.FirstName);
        }

        private void FillManagedArtistDropDown()
        {
            ddlManagedArtist.Items.Clear();
            ddlManagedArtist.Items.Add(new ListItem("---", "0"));

            using (Database db = new MySqlDatabase())
            {
                UserInfo[] uis = db.GetManagedUsers(Util.UserId, 1);

                foreach (UserInfo ui in uis)
                {
                    ClientInfo ci = db.GetClientInfo(ui.UserId);
                    ddlManagedArtist.Items.Add(new ListItem(ci.GetFullName(), ui.UserId.ToString()));
                }

                if (!string.IsNullOrEmpty(Request["id"]))
                {
                    Session["managed.userid"] = Convert.ToInt64(Request["id"]);

                    IsrcHandle.Text = GetIsrcCode(Convert.ToInt64(Session["managed.userid"]));

                    ddlManagedArtist.SelectedValue = Request.QueryString["id"].ToString();

                    ClientInfo _ManagedUserInfo = db.GetClientInfo(Convert.ToInt64(Request.QueryString["id"]));

                    string accessToken = db.GetSocialCredential(_ManagedUserInfo.ClientId, SocialConnector.Facebook, "accesstoken");

                    string expiresRaw = db.GetSocialCredential(_ManagedUserInfo.ClientId, SocialConnector.Facebook, "accesstokenexpires");

                    bool isNotExpired = true;

                    Facebook.AuthenticationService authService = new Facebook.AuthenticationService();

                    if (authService.TryAuthenticateAdminFBCred(expiresRaw, accessToken))
                    {
                        isNotExpired = true;
                    }
                    else
                    {
                        db.RemoveSocialCredential(_ManagedUserInfo.ClientId, SocialConnector.Facebook);
                        db.UpdateFacebookID(_ManagedUserInfo.ClientId);

                        isNotExpired = false;
                    }



                }
            }
        }

        private void FillCoArtistsDropDown()
        {
            CoArtistDropDown.Items.Clear();

            CoArtistDropDown.Items.Add(new ListItem("---", "0"));
            using (Database db = new MySqlDatabase())
            {
                UserInfo[] uis = db.GetManagedUsers(Util.UserId, 0);
                foreach (UserInfo ui in uis)
                {
                    ClientInfo ci = db.GetClientInfo(ui.UserId);
                    CoArtistDropDown.Items.Add(new ListItem(ci.GetFullName(), ui.UserId.ToString()));
                }
            }
        }

        #region Social Networking Section ------- !

        private bool FillOwnFacebookInfo()
        {
            if (!string.IsNullOrEmpty(_ManAccClientInfo.FacebookId))
            {
                using (Database db = new MySqlDatabase())
                {
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

                    AuthenticationService authServices = new AuthenticationService();

                    Me me;

                    string accessToken = string.Empty;

                    if (authServices.TryAuthenticate(out me, out accessToken))
                    {
                        Session["FBOwnACC"] = accessToken;

                        try
                        {
                            Session["FBOwnID"] = me.Id;
                        }
                        catch { }


                        FillOwnFBFriendList(Convert.ToString(Session["FBOwnACC"]));

                        FillOwnFBPageList(Convert.ToString(Session["FBOwnACC"]));

                        cbxShareToMyWall.Enabled = true;

                        cbxShareToMyWall.Visible = true;

                        divShareToMyWall.Visible = true;

                        return true;
                    }
                    else
                    {
                        Session["FBOwnACC"] = Session["FBOwnID"] = null;

                        db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);

                        db.UpdateFacebookID(ci.ClientId);

                        FillOwnFBFriendList(null);

                        FillOwnFBPageList(null);

                        cbxShareToMyWall.Enabled = false;

                        cbxShareToMyWall.Visible = false;

                        divShareToMyWall.Visible = false;

                        return false;
                    }
                }
            }

            Session["FBOwnACC"] = Session["FBOwnID"] = null;

            FillOwnFBFriendList(null);

            FillOwnFBPageList(null);

            cbxShareToMyWall.Enabled = false;

            cbxShareToMyWall.Visible = false;

            divShareToMyWall.Visible = false;

            return false;
        }

        private void FillOwnFBFriendList(string accessToken)
        {
            dlOwnFriendList.DataSource = null;
            dlOwnFriendList.DataBind();

            if (!string.IsNullOrEmpty(accessToken))
            {
                IList<Friend> FriendList = GetFacebookFriendList(accessToken);

                if (FriendList != null)
                {
                    if (FriendList.Count() > 0)
                    {
                        divShareToOwnFriends.Visible = true;

                        cbxShareToOwnFriends.Enabled = true;

                        cbxShareToOwnFriends.Visible = true;

                        dlOwnFriendList.Enabled = true;

                        dlOwnFriendList.DataSource = FriendList;
                        dlOwnFriendList.DataBind();

                        return;
                    }
                }
            }

            divShareToOwnFriends.Visible = false;

            cbxShareToOwnFriends.Enabled = false;

            cbxShareToOwnFriends.Visible = false;

            dlOwnFriendList.Enabled = false;
        }

        private void FillOwnFBPageList(string accessToken)
        {
            dlOwnPageList.DataSource = null;
            dlOwnPageList.DataBind();

            if (!string.IsNullOrEmpty(accessToken))
            {
                IList<TrackProtect.Facebook.Page> PageList = GetFacebookPageList(accessToken);

                if (PageList != null)
                {
                    if (PageList.Count() > 0)
                    {
                        divShareToOwnPages.Visible = true;

                        cbxShareToOwnPages.Enabled = true;

                        cbxShareToOwnPages.Visible = true;

                        dlOwnPageList.Enabled = true;

                        dlOwnPageList.DataSource = PageList;
                        dlOwnPageList.DataBind();

                        return;
                    }
                }
            }

            divShareToOwnPages.Visible = false;

            cbxShareToOwnPages.Enabled = false;

            cbxShareToOwnPages.Visible = false;

            dlOwnPageList.Enabled = false;
        }



        private bool FillSoundCloudInfo(ClientInfo ManagedUserInfo)
        {
            if (ManagedUserInfo != null)
                if (!string.IsNullOrEmpty(ManagedUserInfo.SoundCloudId))
                {
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(Convert.ToInt64(Session["managed.userid"]));

                        Session["soundCloudAccessToken"] = db.GetSocialCredential(ci.ClientId, SocialConnector.SoundCloud, "access_token");
                    }

                    cbxSendToSoundCloud.Enabled = true;
                    SoundCloudMsg.Enabled = true;
                    cbxSendToSoundCloud.Visible = true;

                    return true;
                }

            Session["soundCloudAccessToken"] = null;

            cbxSendToSoundCloud.Enabled = false;
            cbxSendToSoundCloud.Visible = false;
            SoundCloudMsg.Enabled = false;

            return false;
        }

        private bool FillFacebookInfo(ClientInfo ManagedUserInfo)
        {
            if (ManagedUserInfo != null)
                if (!string.IsNullOrEmpty(ManagedUserInfo.FacebookId))
                {
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(Convert.ToInt64(Session["managed.userid"]));

                        Session["FBACC"] = db.GetSocialCredential(ci.ClientId, SocialConnector.Facebook, "accesstoken");

                        string expiresRaw = db.GetSocialCredential(ci.ClientId, SocialConnector.Facebook, "accesstokenexpires");

                        Facebook.AuthenticationService authServices = new Facebook.AuthenticationService();

                        if (authServices.TryAuthenticateAdminFBCred(expiresRaw, Convert.ToString(Session["FBACC"])))
                        {
                            Me me = authServices.GetMe(Convert.ToString(Session["FBACC"]));

                            try
                            {
                                Session["FBID"] = me.Id;
                            }
                            catch { }

                            FillFBFriendList(Convert.ToString(Session["FBACC"]));

                            FillFBPageList(Convert.ToString(Session["FBACC"]));

                            cbxShareToUserWall.Enabled = true;

                            cbxShareToUserWall.Visible = true;

                            divShareToUserWall.Visible = true;

                            return true;
                        }
                        else
                        {
                            Session["FBACC"] = Session["FBID"] = null;

                            db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);

                            db.UpdateFacebookID(ci.ClientId);

                            FillFBFriendList(null);

                            FillFBPageList(null);

                            cbxShareToUserWall.Enabled = false;

                            cbxShareToUserWall.Visible = false;

                            divShareToUserWall.Visible = false;

                            return false;
                        }
                    }
                }

            Session["FBACC"] = Session["FBID"] = null;

            FillFBFriendList(null);

            FillFBPageList(null);

            cbxShareToUserWall.Enabled = false;

            cbxShareToUserWall.Visible = false;

            divShareToUserWall.Visible = false;

            return false;
        }

        private void FillFBFriendList(string accessToken)
        {
            dlFriendList.DataSource = null;
            dlFriendList.DataBind();

            if (!string.IsNullOrEmpty(accessToken))
            {
                IList<Friend> FriendList = GetFacebookFriendList(accessToken);

                if (FriendList != null)
                {
                    if (FriendList.Count() > 0)
                    {
                        divShareToFriends.Visible = true;

                        cbxShareToFriends.Enabled = true;

                        cbxShareToFriends.Visible = true;

                        dlFriendList.Enabled = true;

                        dlFriendList.DataSource = FriendList;
                        dlFriendList.DataBind();

                        return;
                    }
                }
            }

            divShareToFriends.Visible = false;

            cbxShareToFriends.Enabled = false;

            cbxShareToFriends.Visible = false;

            dlFriendList.Enabled = false;
        }

        private void FillFBPageList(string accessToken)
        {
            dlPageList.DataSource = null;
            dlPageList.DataBind();

            if (!string.IsNullOrEmpty(accessToken))
            {
                IList<TrackProtect.Facebook.Page> PageList = GetFacebookPageList(accessToken);

                if (PageList != null)
                {
                    if (PageList.Count() > 0)
                    {
                        divShareToPages.Visible = true;

                        cbxShareToPages.Enabled = true;

                        cbxShareToPages.Visible = true;

                        dlPageList.Enabled = true;

                        dlPageList.DataSource = PageList;
                        dlPageList.DataBind();

                        return;
                    }
                }
            }

            divShareToPages.Visible = false;

            cbxShareToPages.Enabled = false;

            cbxShareToPages.Visible = false;

            dlPageList.Enabled = false;
        }



        IList<string> getFriendIDs()
        {
            IList<string> friendId = new List<string>();

            foreach (DataListItem li in dlFriendList.Items)
            {
                CheckBox cb = li.FindControl("cbxfriendID") as CheckBox;

                if (cb != null)
                {
                    if (cb.Checked)
                    {
                        HiddenField hf = li.FindControl("hfFriendId") as HiddenField;
                        friendId.Add(hf.Value);
                    }
                }
            }

            return friendId;
        }

        Dictionary<string, string> getPageIDs()
        {
            Dictionary<string, string> PageIDs = new Dictionary<string, string>();

            foreach (DataListItem li in dlPageList.Items)
            {
                CheckBox cb = li.FindControl("cbxPageID") as CheckBox;

                if (cb != null)
                {
                    if (cb.Checked)
                    {
                        HiddenField hfPageID = li.FindControl("hfPageId") as HiddenField;
                        HiddenField hfAccessToken = li.FindControl("hfPageAccessToken") as HiddenField;
                        PageIDs.Add(hfPageID.Value, hfAccessToken.Value);
                    }
                }
            }

            return PageIDs;
        }

        private IList<Friend> GetFacebookFriendList(string accessToken)
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
                                        fbUserObj.ProfileImg = string.Format("https://graph.facebook.com/{0}/picture", friend.Value);
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
                    }
                }

                return faceBookUserList;
            }
            catch
            {
                return null;
            }
        }

        private IList<TrackProtect.Facebook.Page> GetFacebookPageList(string accessToken)
        {
            try
            {
                IList<TrackProtect.Facebook.Page> faceBookPageList = new List<TrackProtect.Facebook.Page>();

                FacebookClient client = new FacebookClient(accessToken);

                var PageListDataString = client.Get("/me/accounts");

                MemoryStream ms =
                    new MemoryStream(Encoding.Default.GetBytes(Convert.ToString(PageListDataString).ToCharArray()));

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
                                Dictionary<string, object> pageDictionary =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(dataList[index]));

                                TrackProtect.Facebook.Page fbPageObj = new TrackProtect.Facebook.Page();

                                foreach (KeyValuePair<string, object> page in pageDictionary)
                                {
                                    switch (page.Key)
                                    {
                                        case "name":
                                            fbPageObj.PageName = Convert.ToString(page.Value);
                                            break;
                                        case "id":
                                            fbPageObj.PageID = Convert.ToString(page.Value);
                                            break;
                                        case "access_token":
                                            fbPageObj.AccessToken = Convert.ToString(page.Value);
                                            break;
                                    }

                                    //if (page.Key == "name")
                                    //    fbPageObj.PageName = Convert.ToString(page.Value);
                                    //else
                                    //{
                                    //    fbPageObj.PageID = page.Value;
                                    //    fbPageObj.PagePicture = string.Format("https://graph.facebook.com/{0}/picture", page.Value);
                                    //}
                                }

                                try
                                {
                                    faceBookPageList.Add(fbPageObj);
                                }
                                catch (Exception ex)
                                {
                                    //Utilities.ErrorLogger.LogException(ex, ex.Message);
                                }

                                index++;
                            }

                            break;
                    }
                }

                return faceBookPageList;
            }
            catch
            {
                return null;
            }
        }




        IList<string> getOwnFriendIDs()
        {
            IList<string> friendId = new List<string>();

            foreach (DataListItem li in dlOwnFriendList.Items)
            {
                CheckBox cb = li.FindControl("cbxfriendID") as CheckBox;

                if (cb != null)
                {
                    if (cb.Checked)
                    {
                        HiddenField hf = li.FindControl("hfFriendId") as HiddenField;
                        friendId.Add(hf.Value);
                    }
                }
            }

            return friendId;
        }

        Dictionary<string, string> getOwnPageIDs()
        {
            Dictionary<string, string> PageIDs = new Dictionary<string, string>();

            foreach (DataListItem li in dlOwnPageList.Items)
            {
                CheckBox cb = li.FindControl("cbxPageID") as CheckBox;

                if (cb != null)
                {
                    if (cb.Checked)
                    {
                        HiddenField hfPageID = li.FindControl("hfPageId") as HiddenField;
                        HiddenField hfAccessToken = li.FindControl("hfPageAccessToken") as HiddenField;
                        PageIDs.Add(hfPageID.Value, hfAccessToken.Value);
                    }
                }
            }

            return PageIDs;
        }

        private IList<Friend> GetOwnFacebookFriendList(string accessToken)
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
                                        fbUserObj.ProfileImg = string.Format("https://graph.facebook.com/{0}/picture", friend.Value);
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
                    }
                }

                return faceBookUserList;
            }
            catch
            {
                return null;
            }
        }

        private IList<TrackProtect.Facebook.Page> GetOwnFacebookPageList(string accessToken)
        {
            try
            {
                IList<TrackProtect.Facebook.Page> faceBookPageList = new List<TrackProtect.Facebook.Page>();

                FacebookClient client = new FacebookClient(accessToken);

                var PageListDataString = client.Get("/me/accounts");

                MemoryStream ms =
                    new MemoryStream(Encoding.Default.GetBytes(Convert.ToString(PageListDataString).ToCharArray()));

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
                                Dictionary<string, object> pageDictionary =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(dataList[index]));

                                TrackProtect.Facebook.Page fbPageObj = new TrackProtect.Facebook.Page();

                                foreach (KeyValuePair<string, object> page in pageDictionary)
                                {
                                    switch (page.Key)
                                    {
                                        case "name":
                                            fbPageObj.PageName = Convert.ToString(page.Value);
                                            break;
                                        case "id":
                                            fbPageObj.PageID = Convert.ToString(page.Value);
                                            break;
                                        case "access_token":
                                            fbPageObj.AccessToken = Convert.ToString(page.Value);
                                            break;
                                    }
                                }

                                try
                                {
                                    faceBookPageList.Add(fbPageObj);
                                }
                                catch (Exception ex)
                                {
                                    //Utilities.ErrorLogger.LogException(ex, ex.Message);
                                }

                                index++;
                            }

                            break;
                    }
                }

                return faceBookPageList;
            }
            catch
            {
                return null;
            }
        }


        private string getPageAccessToken(string pageId)
        {
            string accessToken = string.Empty;
            FacebookClient fbcl = new FacebookClient();
            dynamic result = fbcl.Get("/" + pageId + "?fields=access_token");
            accessToken = result.access_token;
            return accessToken;
        }


        private string postToFacebook(string messageToPost, string link, string accessToken, string id)
        {
            try
            {
                FacebookClient client = new FacebookClient();
                client.AccessToken = accessToken;

                dynamic parameters = new ExpandoObject();
                parameters.access_token = accessToken;
                parameters.link = link;
                parameters.message = messageToPost;

                dynamic result = null;

                result = client.Post("/" + id.Trim() + "/feed", parameters);

                return Convert.ToString(result.id);
            }
            catch
            {
                return "Error";
            }
        }


        private void FillTwitterInfo(ClientInfo ManagedUserInfo)
        {
            if (ManagedUserInfo != null)
                if (!string.IsNullOrEmpty(ManagedUserInfo.TwitterId))
                {
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(Convert.ToInt64(Session["managed.userid"]));

                        Session["AccessToken"] = db.GetSocialCredential(ManagedUserInfo.ClientId, SocialConnector.Twitter, "oauthtoken");

                        Session["AccessTokenSecret"] = db.GetSocialCredential(ManagedUserInfo.ClientId, SocialConnector.Twitter, "oauthverifier");
                    }

                    cbxSendToTwitter.Enabled = true;

                    TwitterMsg.Enabled = true;

                    cbxSendToTwitter.Visible = true;

                    return;
                }

            Session["AccessToken"] = Session["AccessTokenSecret"] = null;

            cbxSendToTwitter.Enabled = false;

            TwitterMsg.Enabled = false;

            cbxSendToTwitter.Visible = false;
        }

        private string tweetToTwitter(string shortUrl, string stream_url)
        {
            OAuthTokens accesstoken = new OAuthTokens()
            {
                AccessToken = Convert.ToString(Session["AccessToken"]),

                AccessTokenSecret = Convert.ToString(Session["AccessTokenSecret"]),

                ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],

                ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
            };

            String messageToPost = string.Empty;

            if (!string.IsNullOrEmpty(TwitterMsg.Text.Trim()))
                messageToPost += TwitterMsg.Text.Trim() + System.Environment.NewLine;

            messageToPost += BitlyApi.GetShortUrl(stream_url) + System.Environment.NewLine;

            messageToPost += "Trackname:" + TrackNameText.Text.Trim() + System.Environment.NewLine;

            messageToPost += "Trackprotect ID:" + shortUrl;

            try
            {
                StatusUpdateOptions so = new StatusUpdateOptions();

                so.APIBaseAddress = "http://api.twitter.com/1.1/";

                TwitterResponse<TwitterStatus> status = TwitterStatus.Update(accesstoken, messageToPost, so);

                if (status.Result == RequestResult.Success)
                    return status.ToString();
                else
                    return "Error";
            }
            catch
            {
                return "Error";
            }
        }

        #endregion

        private void CustomValidationShow(string resource)
        {
            CustomValidator CustomValidatorCtrl = new CustomValidator();

            CustomValidatorCtrl.IsValid = false;

            CustomValidatorCtrl.ValidationGroup = "TrackProtectValidation";

            CustomValidatorCtrl.ErrorMessage = resource;

            this.Page.Form.Controls.Add(CustomValidatorCtrl);
        }

        private string CreateCertificate
           (long userid, string doc1, string doc2, string doc3, string isrcCode, string trackName, CoopArtistList coopArtists)
        {
            ClientInfo ci = null;
            using (Database db = new MySqlDatabase())
            {
                ci = db.GetClientInfo(Util.UserId);
            }
            string ret = string.Empty;
            using (CertificateManager mgr = new CertificateManager(userid, string.Empty))
            {
                mgr.AddTrackName(trackName);
                if (!string.IsNullOrEmpty(doc1))
                    mgr.AddDocument(doc1);
                if (!string.IsNullOrEmpty(doc2))
                    mgr.AddDocument(doc2);
                if (!string.IsNullOrEmpty(doc3))
                    mgr.AddDocument(doc3);
                if (!string.IsNullOrEmpty(isrcCode))
                    mgr.AddIsrcCode(isrcCode);
                foreach (CoopArtist coop in coopArtists)
                    mgr.AddCoopArtist(coop.Artist, coop.Role);
                if (ci != null)
                    mgr.Agent = ci.GetFullName();
                mgr.CreateCertificate(string.Format("{0}.cer", DateTime.UtcNow.ToString("yyyyMMddHHmmss")));
                ret = mgr.CertificateFilename;
            }
            return ret;
        }

        private long RegisterDoc(long registerId, string documentname)
        {
            long documentid = 0;
            using (Database db = new MySqlDatabase())
            {
                string documenthash = Convert.ToBase64String(CertificateManager.CalculateHash(documentname));
                documentid = db.RegisterDocument(registerId, documentname, documenthash);
            }
            return documentid;
        }

        private long RegisterCoArtist(long registerId, string text, string value, string role)
        {
            long coartistId = 0;
            using (Database db = new MySqlDatabase())
            {
                long clientId = Convert.ToInt64(value);
                coartistId = db.RegisterCoArtist(registerId, clientId, role);
            }
            return coartistId;
        }

        private static void DeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception)
            {

            }
        }

        private void AddCoArtistRow(DataTable coArtistsTable, string name, string role, long id)
        {
            DataRow row = coArtistsTable.NewRow();

            row["name"] = name;
            row["role"] = role;
            row["clientid"] = id;

            coArtistsTable.Rows.Add(row);
        }

        private void DeleteCoArtistRow(DataTable coArtistsTable, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= coArtistsTable.Rows.Count)
                return;

            coArtistsTable.Rows.RemoveAt(rowIndex);
            DataView dataView = new DataView(CoArtistsTable);
            CoArtistsList.DataSource = dataView;
            CoArtistsList.DataBind();
        }

        private DataTable CreateCoArtistsTable()
        {
            DataTable table = new DataTable("artists");

            table.Columns.Add("name", typeof(string));
            table.Columns.Add("role", typeof(string));
            table.Columns.Add("clientid", typeof(long));

            return table;
        }

        private static string GetIsrcCode(long userId)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(userId);
                string isrcCode = string.Format("ISRC {0}-{1}-",
                                                ci.IsrcCode,
                                                DateTime.Now.ToString("yy"));
                return isrcCode;
            }
        }

        /// <summary>
        ///  Initializing with default values to sicail credentials response.
        /// </summary>
        private void InitializeWithDefaultValues()
        {
            Session["SoundCloud.Response"] = "Deactive";
            Session["Facebook.Response"] = "Deactive";
            Session["Twitter.Response"] = "Deactive";
            Session["FacebookGenre.Response"] = "Deactive";
            Session["FacebookSubGenre.Response"] = "Deactive";
            Session["ShareToFriends"] = "Deactive";
            Session["ShareToPages"] = "Deactive";

            Session["FacebookOwnPage.Response"] = "Deactive";
            Session["ShareToOwnFriends"] = "Deactive";
            Session["ShareToOwnPages"] = "Deactive";
        }

        #endregion

        #region Events

        protected void ddlManagedArtist_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["managed.userid"] = ((DropDownList)sender).SelectedValue;

            if (Convert.ToInt32(Session["managed.userid"]) > 0)
                using (Database db = new MySqlDatabase())
                {
                    ClientInfo _ManagedUserInfo = db.GetClientInfo(Convert.ToInt64(Session["managed.userid"]));

                    PopulateSocialMediaInfo(db);

                    Session["ManagedUser"] = _ManagedUserInfo.FirstName;
                }
        }

        protected void SubmitButton_Command(object sender, CommandEventArgs e)
        {
            InitializeWithDefaultValues();

            Page.Validate("TrackProtectValidation");

            bool atleastOneTaggingSelected = false;

            if (ViewState["Genre1"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["Genre2"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["Genre3"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SubGenre1"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SubGenre2"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SubGenre3"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SoundTag1"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SoundTag2"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;
            else if (ViewState["SoundTag3"] != null && atleastOneTaggingSelected == false)
                atleastOneTaggingSelected = true;

            if (!FileUpload1.HasFile)
            {
                CustomValidationShow(Resources.Resource.SelectMp3File);
            }
            else if (FileUpload1.FileContent.Length > 12582912)
            {
                CustomValidationShow(Resources.Resource.FileSizeExceed);
            }
            else if (atleastOneTaggingSelected == false)
            {
                CustomValidationShow(Resources.Resource.SelectOneTag);
            }
            else if (Session["soundCloudAccessToken"] == null && (cbxSendToFacebook.Checked == true || cbxSendToTwitter.Checked == true))
            {
                CustomValidationShow(Resources.Resource.SoundCloudCredRequired);
            }
            else if (cbxSendToSoundCloud.Checked == false && (cbxSendToFacebook.Checked == true || cbxSendToTwitter.Checked == true))
            {
                CustomValidationShow(Resources.Resource.SelectSoundcloud);
            }


            if (Page.IsValid)
            {
                bool failed = false;
                long managedUserId = 0;
                if (Session["managed.userid"] != null)
                    managedUserId = Convert.ToInt64(Session["managed.userid"]);

                if (!FileUpload1.HasFile)
                {
                    // No file for the first option, need one to proceed or need a check to indicate
                    // only songtext and sheet music will be registered.
                    Response.Redirect(string.Format("~/Member/ErrorPage.aspx?title={0}&message={1}&returnurl={2}",
                        Resources.Resource.FirstFileRequiredTitle,
                        Resources.Resource.FirstFileRequiredMessage,
                        Uri.EscapeDataString(Request.RawUrl)), false);
                }

                long registerId = 0;

                string trackName = TrackNameText.Text;
                string doc1 = string.Empty, doc2 = string.Empty, doc3 = string.Empty;
                string uploadPath = string.Empty;
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);

                    uploadPath = db.GetUserDocumentPath(managedUserId);


                    string isrcCode = string.Empty;
                    if (!string.IsNullOrEmpty(IsrcHandle.Text) && !string.IsNullOrEmpty(IsrcPostfix.Text))
                        isrcCode = IsrcHandle.Text + "-" + IsrcPostfix.Text;

                    if (!failed && FileUpload1.HasFile)
                    {
                        doc1 = FileUpload1.FileName;
                        try
                        {
                            FileUpload1.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc1)));
                            StatusInfo1.Text = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc1));
                        }
                        catch (Exception)
                        {
                            StatusInfo1.Text = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc1));
                            failed = true;
                        }
                    }
                    if (!failed && FileUpload2.HasFile)
                    {
                        doc2 = FileUpload2.FileName;
                        try
                        {
                            FileUpload2.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc2)));
                            StatusInfo2.Text = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc2));
                        }
                        catch (Exception)
                        {
                            StatusInfo2.Text = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc2));
                            failed = true;
                        }
                    }
                    if (!failed && FileUpload3.HasFile)
                    {
                        doc3 = FileUpload3.FileName;
                        try
                        {
                            FileUpload3.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc3)));
                            StatusInfo3.Text = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc3));
                        }
                        catch (Exception)
                        {
                            StatusInfo3.Text = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc3));
                            failed = true;
                        }
                    }
                    if (!failed)
                    {
                        // Continue registration
                        string f1 = string.Empty, f2 = string.Empty, f3 = string.Empty;
                        if (!string.IsNullOrEmpty(doc1))
                            f1 = Path.Combine(uploadPath, Path.GetFileName(doc1));
                        if (!string.IsNullOrEmpty(doc2))
                            f2 = Path.Combine(uploadPath, Path.GetFileName(doc2));
                        if (!string.IsNullOrEmpty(doc3))
                            f3 = Path.Combine(uploadPath, Path.GetFileName(doc3));

                        // Get the cooperating artists to add to the certificate
                        CoopArtistList coopArtists = new CoopArtistList();
                        foreach (DataRow row in CoArtistsTable.Rows)
                        {
                            string name = Convert.ToString(row[0]);
                            string role = Convert.ToString(row[1]);
                            coopArtists.Add(new CoopArtist(name, role));
                        }

                        string certfilename = CreateCertificate(managedUserId, f1, f2, f3, isrcCode, trackName, coopArtists);
                        //long registerId = db.CreateRegistry(managedUserId, certfilename, f1);
                        registerId = db.CreateRegistry(Util.UserId, managedUserId, certfilename, f1, trackName.Trim(), isrcCode,
                            (Genre1.SelectedItem.Text == "--Select--" ? "" : Genre1.SelectedItem.Text),
                            (Genre2.SelectedItem.Text == "--Select--" ? "" : Genre2.SelectedItem.Text),
                            (Genre3.SelectedItem.Text == "--Select--" ? "" : Genre3.SelectedItem.Text),
                            (SubGenre1.SelectedItem.Text == "--Select--" ? "" : SubGenre1.SelectedItem.Text),
                            (SubGenre2.SelectedItem.Text == "--Select--" ? "" : SubGenre2.SelectedItem.Text),
                            (SubGenre3.SelectedItem.Text == "--Select--" ? "" : SubGenre3.SelectedItem.Text),
                            (Tag1.SelectedItem.Text == "--Select--" ? "" : Tag1.SelectedItem.Text),
                            (Tag2.SelectedItem.Text == "--Select--" ? "" : Tag2.SelectedItem.Text),
                            (Tag3.SelectedItem.Text == "--Select--" ? "" : Tag3.SelectedItem.Text), StageNameText.Text.Trim());

                        if (registerId > 0)
                        {
                            if (!failed && !string.IsNullOrEmpty(f1))
                                failed = (RegisterDoc(registerId, f1) == 0);

                            if (!failed && !string.IsNullOrEmpty(f2))
                                failed = (RegisterDoc(registerId, f2) == 0);

                            if (!failed && !string.IsNullOrEmpty(f3))
                                failed = (RegisterDoc(registerId, f3) == 0);
                            if (!failed && CoArtistsTable.Rows.Count > 0)
                            {
                                foreach (DataRow row in CoArtistsTable.Rows)
                                {
                                    string name = Convert.ToString(row[0]);
                                    string role = Convert.ToString(row[1]);
                                    long clientId = Convert.ToInt64(row[2]);
                                    string id = clientId.ToString();
                                    RegisterCoArtist(registerId, name, id, role);
                                }
                            }
                            if (!failed)
                            {
                                StatusInfo4.Text = Resources.Resource.RegistrationSuccessful;
                                db.DecrementCredits(Util.UserId);
                                db.AddCreditHistory(Util.UserId, 0L, 1, 0);

                                string certdocfile = Path.ChangeExtension(certfilename, ".pdf");
                                Util.SendRegistration(Convert.ToInt64(Session["managed.userid"]), uploadPath, trackName, certfilename, certdocfile);
                            }
                            else
                            {
                                StatusInfo4.Text = Resources.Resource.RegistrationFailed;
                            }
                        }
                        else
                        {
                            // Registration failed
                            StatusInfo4.Text = Resources.Resource.RegistrationFailed;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(doc1))
                            DeleteFile(Path.Combine(uploadPath, Path.GetFileName(doc1)));
                        if (!string.IsNullOrEmpty(doc2))
                            DeleteFile(Path.Combine(uploadPath, Path.GetFileName(doc2)));
                        if (!string.IsNullOrEmpty(doc3))
                            DeleteFile(Path.Combine(uploadPath, Path.GetFileName(doc3)));
                    }
                }

                if (failed == false)
                {
                    Session["TrackProtected"] = true;

                    if (cbxSendToSoundCloud.Checked == false)
                        Session["SoundCloud.Response"] = "Deactive";
                    else if (cbxSendToSoundCloud.Checked && !File.Exists(Path.Combine(uploadPath, doc1)))
                        Session["SoundCloud.Response"] = "Error";
                    else if (cbxSendToSoundCloud.Checked && File.Exists(Path.Combine(uploadPath, doc1)))
                        Session["SoundCloud.Response"] = Publish.SoundCloudPublish(Convert.ToString(Session["soundCloudAccessToken"]), Path.Combine(uploadPath, doc1), trackName + System.Environment.NewLine + SoundCloudMsg.Text.Trim());


                    if (Session["SoundCloud.Response"] != null)
                    {
                        if (!Convert.ToString(Session["SoundCloud.Response"]).Contains("Deactive") && !Convert.ToString(Session["SoundCloud.Response"]).Contains("Error"))
                        {
                            string shortUrl =
                                BitlyApi.GetShortUrl(ConfigurationManager.AppSettings["SiteNavigationLink"] + "/idlookup.aspx?TrackID="
                                + EncryptionClass.Encrypt(Convert.ToString(registerId)));

                            string stream_url = string.Empty;

                            try
                            {
                                var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(Session["SoundCloud.Response"]));

                                stream_url = Convert.ToString(obj["permalink_url"]) + "/" + Convert.ToString(obj["secret_token"]);
                            }
                            catch { }

                            #region Facebook

                            if (cbxSendToFacebook.Checked == false)
                                Session["Facebook.Response"] = "Deactive";
                            else if (cbxSendToFacebook.Checked)
                            {
                                String messageToPost = string.Empty;
                                messageToPost += FacebookMsg.Text.Trim() + System.Environment.NewLine + System.Environment.NewLine;
                                messageToPost += "Trackname : " + TrackNameText.Text.Trim() + System.Environment.NewLine;
                                messageToPost += "Trackprotect ID : " + shortUrl + System.Environment.NewLine;
                                //messageToPost += "Protected by: " + ConfigurationManager.AppSettings["SiteNavigationLink"] + System.Environment.NewLine;                                                            

                                if (cbxSendToGenreCommunityPage.Checked == false)
                                    Session["FacebookGenre.Response"] = "Deactive";
                                else if (cbxSendToGenreCommunityPage.Checked)
                                    Session["FacebookGenre.Response"] = postToFacebook(messageToPost, stream_url, Convert.ToString(ViewState["GenrePageToken"]), Convert.ToString(ViewState["GenrePageID"]));

                                if (cbxSendToSubGenreCommunityPage.Checked == false)
                                    Session["FacebookSubGenre.Response"] = "Deactive";
                                else if (cbxSendToSubGenreCommunityPage.Checked)
                                    Session["FacebookSubGenre.Response"] = postToFacebook(messageToPost, stream_url, Convert.ToString(ViewState["SubGenrePageToken"]), Convert.ToString(ViewState["SubGenrePageID"]));


                                if (cbxShareToUserWall.Checked == false)
                                    Session["Facebook.Response"] = "Deactive";
                                else if (cbxShareToUserWall.Checked)
                                    Session["Facebook.Response"] = postToFacebook(messageToPost, stream_url, Convert.ToString(Session["FBACC"]), Convert.ToString(Session["FBID"]));


                                if (cbxShareToFriends.Checked == false)
                                    Session["ShareToFriends"] = "Deactive";
                                else if (cbxShareToFriends.Checked)
                                {
                                    IList<string> friendList = getFriendIDs();

                                    foreach (var friendId in friendList)
                                        Session["ShareToFriends"] =
                                            postToFacebook(messageToPost, stream_url, Convert.ToString(Session["FBACC"]), friendId);
                                }

                                if (cbxShareToPages.Checked == false)
                                    Session["ShareToPages"] = "Deactive";
                                else if (cbxShareToPages.Checked)
                                {
                                    Dictionary<string, string> pageList = getPageIDs();

                                    foreach (var pageId in pageList)
                                        Session["ShareToPages"] = postToFacebook(messageToPost, stream_url, pageId.Value, pageId.Key);
                                }

                                if (cbxShareToMyWall.Checked == false)
                                    Session["FacebookOwnPage.Response"] = "Deactive";
                                else if (cbxShareToMyWall.Checked)
                                    Session["FacebookOwnPage.Response"] = postToFacebook(messageToPost, stream_url, Convert.ToString(Session["FBOwnACC"]), Convert.ToString(Session["FBOwnID"]));

                                if (cbxShareToOwnFriends.Checked == false)
                                    Session["ShareToOwnFriends"] = "Deactive";
                                else if (cbxShareToOwnFriends.Checked)
                                {
                                    IList<string> friendList = getOwnFriendIDs();

                                    foreach (var friendId in friendList)
                                        Session["ShareToOwnFriends"] = postToFacebook(messageToPost, stream_url, Convert.ToString(Session["FBOwnACC"]), friendId);
                                }

                                if (cbxShareToOwnPages.Checked == false)
                                    Session["ShareToOwnPages"] = "Deactive";
                                else if (cbxShareToOwnPages.Checked)
                                {
                                    Dictionary<string, string> pageList = getOwnPageIDs();

                                    foreach (var pageId in pageList)
                                        Session["ShareToOwnPages"] = postToFacebook(messageToPost, stream_url, pageId.Value, pageId.Key);
                                }
                            }

                            #endregion

                            #region Twitter

                            if (cbxSendToTwitter.Checked == false)
                                Session["Twitter.Response"] = "Deactive";
                            else if (cbxSendToTwitter.Checked)
                                Session["Twitter.Response"] = tweetToTwitter(shortUrl, stream_url);

                            #endregion
                        }
                    }
                }
                else
                    Session["TrackProtected"] = false;

                string continueUrl = "~/Member/UploadConfirmation.aspx?id=" + Convert.ToInt64(Session["managed.userid"]);

                if (HttpContext.Current.Session["ReturnUrl"] != null)
                    continueUrl = HttpContext.Current.Session["ReturnUrl"] as string;

                Response.Redirect(continueUrl, false);
            }

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void AddCoArtist(object sender, CommandEventArgs e)
        {
            bool found = false;
            if (CoArtistDropDown.SelectedIndex < 0)
                return;

            using (Database db = new MySqlDatabase())
            {
                long userId = Convert.ToInt64(CoArtistDropDown.SelectedValue);
                if (userId > 0)
                {
                    UserInfo ui = db.GetUser(userId);
                    ClientInfo ci = db.GetClientInfo(userId);

                    Session["user.userid"] = userId;
                    AddCoArtistRow(CoArtistsTable, ci.GetFullName(), CoArtistRole.Text, ci.ClientId);

                    DataView dataView = new DataView(CoArtistsTable);
                    CoArtistsList.DataSource = dataView;
                    CoArtistsList.DataBind();

                    found = true;
                }
            }
            if (!found)
                ErrorMessage.Text = Resources.Resource.ClientNotFound;
        }

        protected void CoArtistsList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName.ToLower() == "delete")
            {
            }
        }

        protected void CoArtistsList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DeleteCoArtistRow(CoArtistsTable, e.RowIndex);
        }

        protected void CoArtistsList_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
        }

        protected void CoArtistsList_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            DeleteCoArtistRow(CoArtistsTable, e.ItemIndex);
        }

        protected void SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            if (ddl == null)
                return;
            if (ddl.SelectedIndex > -1)
            {
                ListItem item = ddl.Items[ddl.SelectedIndex];
                long userIdManaged = Convert.ToInt64(item.Value);
                IsrcHandle.Text = GetIsrcCode(userIdManaged);
            }
        }

        #region Genre/SubGenre events ------- !

        protected void Genre1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
            {
                ViewState["Genre1"] = ((DropDownList)sender).SelectedItem.Value;

                using (Database db = new MySqlDatabase())
                {
                    SubGenre1.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["Genre1"]));
                    SubGenre1.DataTextField = "Value";
                    SubGenre1.DataValueField = "Key";
                    SubGenre1.DataBind();
                    SubGenre1.Items.Insert(0, new ListItem("--Select--", null));

                    int fbPageId = 0;

                    try
                    {
                        fbPageId = db.getAdminFBPageIdBYGenreIdandType(Convert.ToInt32(ViewState["Genre1"]), "genre");
                    }
                    catch { }

                    if (fbPageId != 0 && Convert.ToBoolean(ViewState["isSoundCloudActive"]) == true && Convert.ToBoolean(ViewState["isFacebookActive"]) == true)
                    {
                        IDictionary<string, string> pageCred = db.getAdminFBPageCredByPageID(fbPageId);

                        Session["GenrePageName"] = ViewState["GenrePageName"] = pageCred["PageName"];
                        ViewState["GenrePageID"] = pageCred["PageID"];
                        ViewState["GenrePageToken"] = pageCred["PageToken"];

                        cbxSendToGenreCommunityPage.Visible = true;
                        cbxGenreCross.Visible = true;
                        CommunityGenrePageLabel.Visible = true;
                        CommunityGenrePageLabel.Text = Convert.ToString(ViewState["GenrePageName"]);
                    }
                    else
                    {
                        ViewState["GenrePageName"] = null;
                        ViewState["GenrePageID"] = null;
                        ViewState["GenrePageToken"] = null;

                        cbxSendToGenreCommunityPage.Visible = false;
                        cbxGenreCross.Visible = false;
                        CommunityGenrePageLabel.Visible = false;
                        CommunityGenrePageLabel.Text = string.Empty;
                    }

                    ViewState["SubGenre1"] = null;

                    cbxSendToSubGenreCommunityPage.Visible = false;
                    cbxSubGenreCross.Visible = false;
                    CommunitySubGenrePageLabel.Visible = false;
                    CommunitySubGenrePageLabel.Text = string.Empty;
                }
            }
            else
            {
                ViewState["Genre1"] = null;

                cbxSendToGenreCommunityPage.Visible = false;
                cbxGenreCross.Visible = false;
                CommunityGenrePageLabel.Visible = false;
                CommunityGenrePageLabel.Text = string.Empty;


                ViewState["SubGenre1"] = null;

                cbxSendToSubGenreCommunityPage.Visible = false;
                cbxSubGenreCross.Visible = false;
                CommunitySubGenrePageLabel.Visible = false;
                CommunitySubGenrePageLabel.Text = string.Empty;
            }

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void Genre2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
            {
                ViewState["Genre2"] = ((DropDownList)sender).SelectedItem.Value;

                using (Database db = new MySqlDatabase())
                {
                    SubGenre2.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["Genre2"]));
                    SubGenre2.DataTextField = "Value";
                    SubGenre2.DataValueField = "Key";
                    SubGenre2.DataBind();
                    SubGenre2.Items.Insert(0, new ListItem("--Select--", null));
                }
            }
            else
            {
                ViewState["Genre2"] = null;
            }

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void Genre3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
            {
                ViewState["Genre3"] = ((DropDownList)sender).SelectedItem.Value;

                using (Database db = new MySqlDatabase())
                {
                    SubGenre3.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["Genre3"]));
                    SubGenre3.DataTextField = "Value";
                    SubGenre3.DataValueField = "Key";
                    SubGenre3.DataBind();
                    SubGenre3.Items.Insert(0, new ListItem("--Select--", null));
                }
            }
            else
            {
                ViewState["Genre3"] = null;
            }

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void SubGenre1_Selectionchanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
            {
                ViewState["SubGenre1"] = ((DropDownList)sender).SelectedItem.Value;

                using (Database db = new MySqlDatabase())
                {
                    int fbPageId = 0;

                    try
                    {
                        fbPageId = db.getAdminFBPageIdBYGenreIdandType(Convert.ToInt32(ViewState["SubGenre1"]), "subgenre");
                    }
                    catch { }

                    if (fbPageId != 0 && Convert.ToBoolean(ViewState["isSoundCloudActive"]) == true && Convert.ToBoolean(ViewState["isFacebookActive"]) == true)
                    {
                        IDictionary<string, string> pageCred = db.getAdminFBPageCredByPageID(fbPageId);

                        Session["SubGenrePageName"] = ViewState["SubGenrePageName"] = pageCred["PageName"];
                        ViewState["SubGenrePageID"] = pageCred["PageID"];
                        ViewState["SubGenrePageToken"] = pageCred["PageToken"];

                        cbxSendToSubGenreCommunityPage.Visible = true;
                        cbxSubGenreCross.Visible = true;
                        CommunitySubGenrePageLabel.Visible = true;
                        CommunitySubGenrePageLabel.Text = Convert.ToString(ViewState["SubGenrePageName"]);
                    }
                    else
                    {
                        ViewState["SubGenrePageName"] = null;
                        ViewState["SubGenrePageID"] = null;
                        ViewState["SubGenrePageToken"] = null;

                        cbxSendToSubGenreCommunityPage.Visible = false;
                        cbxSubGenreCross.Visible = false;
                        CommunitySubGenrePageLabel.Visible = false;
                        CommunitySubGenrePageLabel.Text = string.Empty;
                    }
                }
            }
            else
            {
                ViewState["SubGenre1"] = null;

                cbxSendToSubGenreCommunityPage.Visible = false;
                cbxSubGenreCross.Visible = false;
                CommunitySubGenrePageLabel.Visible = false;
                CommunitySubGenrePageLabel.Text = string.Empty;
            }

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void SubGenre2_Selectionchanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
                ViewState["SubGenre2"] = ((DropDownList)sender).SelectedItem.Value;
            else
                ViewState["SubGenre2"] = null;

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void SubGenre3_Selectionchanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
                ViewState["SubGenre3"] = ((DropDownList)sender).SelectedItem.Value;
            else
                ViewState["SubGenre3"] = null;

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void Tag1_SelectionChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
                ViewState["SoundTag1"] = ((DropDownList)sender).SelectedItem.Value;
            else
                ViewState["SoundTag1"] = null;

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void Tag2_SelectionChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
                ViewState["SoundTag2"] = ((DropDownList)sender).SelectedItem.Value;
            else
                ViewState["SoundTag2"] = null;

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        protected void Tag3_SelectionChanged(object sender, EventArgs e)
        {
            if (!((DropDownList)sender).SelectedItem.Value.Contains("Select"))
                ViewState["SoundTag3"] = ((DropDownList)sender).SelectedItem.Value;
            else
                ViewState["SoundTag3"] = null;

            if (cbxSendToFacebook.Checked)
                divFBSharing.Attributes.Add("style", "display:block");
            else
                divFBSharing.Attributes.Add("style", "display:none");
        }

        #endregion

        #endregion
    }
}