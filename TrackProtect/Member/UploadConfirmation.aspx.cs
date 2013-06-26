using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TrackProtect.Logging;
using System.Text;
using System.IO;
using System.Configuration;

namespace TrackProtect.Member
{
    public partial class UploadConfirmation : BasePage
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
            Session["bodyid"] = "coupon";
            UserInfo ui = null;
            ClientInfo ci = null;

            new BasePage();

            using (Database db = new MySqlDatabase())
            {
                ui = db.GetUser(Util.UserId);
                ci = db.GetClientInfo(Util.UserId);

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

                divTrackProtected.Visible = true;

                if (Convert.ToBoolean(Session["TrackProtected"]) == true)
                {
                    TrackProtected.Text = Resources.Resource.TrackProtectedSuccessNotify;

                    #region SoundCloud

                    divSoundCloudPost.Visible = true;

                    if (Convert.ToString(Session["SoundCloud.Response"]).Contains("Deactive"))
                        divSoundCloudPost.Visible = false;
                    else if (Convert.ToString(Session["SoundCloud.Response"]).Contains("Error"))
                        SoundCloudPost.Text = Resources.Resource.SoundCloudPostFailureNotify;
                    else if (!string.IsNullOrEmpty(Convert.ToString(Session["SoundCloud.Response"])))
                        SoundCloudPost.Text = Resources.Resource.SoundCloudPostSuccessNotify;

                    #endregion

                    #region Facebook My/User wall

                    if (Request.QueryString["type"] == "own")
                    {
                        divFacebookPost.Visible = true;

                        if (Convert.ToString(Session["Facebook.Response"]).Contains("Deactive"))
                            divFacebookPost.Visible = false;
                        else if (Convert.ToString(Session["Facebook.Response"]).Contains("Error"))
                            FacebookPost.Text = string.Format(Resources.Resource.FacebookPostOnOwnWallFailureNotify, Convert.ToString(Session["TPForUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["Facebook.Response"])))
                            FacebookPost.Text = string.Format(Resources.Resource.FacebookPostOnOwnWallSuccessNotify, Convert.ToString(Session["TPForUser"]));
                    }
                    else
                    {
                        divFacebookPost.Visible = true;

                        if (Convert.ToString(Session["Facebook.Response"]).Contains("Deactive"))
                            divFacebookPost.Visible = false;
                        else if (Convert.ToString(Session["Facebook.Response"]).Contains("Error"))
                            FacebookPost.Text = string.Format(Resources.Resource.FacebookPostOnUserWallFailureNotify, Convert.ToString(Session["ManagedUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["Facebook.Response"])))
                            FacebookPost.Text = string.Format(Resources.Resource.FacebookPostOnUserWallSuccessNotify, Convert.ToString(Session["ManagedUser"]));
                    }

                    #endregion

                    #region Facebook Genre

                    divFacebookGenreCommunityPost.Visible = true;

                    if (Convert.ToString(Session["FacebookGenre.Response"]).Contains("Deactive"))
                        divFacebookGenreCommunityPost.Visible = false;
                    else if (Convert.ToString(Session["FacebookGenre.Response"]).Contains("Error"))
                        FacebookGenreCommunityPost.Text = string.Format(Resources.Resource.FacebookGenreCommunityPostFailureNotify, Convert.ToString(Session["GenrePageName"]));
                    else if (!string.IsNullOrEmpty(Convert.ToString(Session["FacebookGenre.Response"])))
                        FacebookGenreCommunityPost.Text = string.Format(Resources.Resource.FacebookGenreCommunityPostSuccessNotify, Convert.ToString(Session["GenrePageName"]));

                    #endregion

                    #region Facebook SubGenre

                    divFacebookSubGenreCommunityPost.Visible = true;

                    if (Convert.ToString(Session["FacebookSubGenre.Response"]).Contains("Deactive"))
                        divFacebookSubGenreCommunityPost.Visible = false;
                    else if (Convert.ToString(Session["FacebookSubGenre.Response"]).Contains("Error"))
                        FacebookSubGenreCommunityPost.Text = string.Format(Resources.Resource.FacebookGenreCommunityPostFailureNotify, Convert.ToString(Session["SubGenrePageName"]));
                    else if (!string.IsNullOrEmpty(Convert.ToString(Session["FacebookSubGenre.Response"])))
                        FacebookSubGenreCommunityPost.Text = string.Format(Resources.Resource.FacebookGenreCommunityPostSuccessNotify, Convert.ToString(Session["SubGenrePageName"]));

                    #endregion

                    #region Facebook to Friends

                    if (Request.QueryString["type"] == "own")
                    {
                        divPostOnFriendWall.Visible = true;

                        if (Convert.ToString(Session["ShareToFriends"]).Contains("Deactive"))
                            divPostOnFriendWall.Visible = false;
                        else if (Convert.ToString(Session["ShareToFriends"]).Contains("Error"))
                            PostOnFriendWall.Text = string.Format(Resources.Resource.PostOnMyFriendWallFailureNotify, Convert.ToString(Session["TPForUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToFriends"])))
                            PostOnFriendWall.Text = string.Format(Resources.Resource.PostOnMyFriendWallSuccessNotify, Convert.ToString(Session["TPForUser"]));
                    }
                    else
                    {
                        divPostOnFriendWall.Visible = true;

                        if (Convert.ToString(Session["ShareToFriends"]).Contains("Deactive"))
                            divPostOnFriendWall.Visible = false;
                        else if (Convert.ToString(Session["ShareToFriends"]).Contains("Error"))
                            PostOnFriendWall.Text = string.Format(Resources.Resource.PostOnFriendWallFailureNotify, Convert.ToString(Session["ManagedUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToFriends"])))
                            PostOnFriendWall.Text = string.Format(Resources.Resource.PostOnFriendWallSuccessNotify, Convert.ToString(Session["ManagedUser"]));
                    }

                    #endregion

                    #region Facebook to Pages

                    if (Request.QueryString["type"] == "own")
                    {
                        divPostOnPages.Visible = true;

                        if (Convert.ToString(Session["ShareToPages"]).Contains("Deactive"))
                            divPostOnPages.Visible = false;
                        else if (Convert.ToString(Session["ShareToPages"]).Contains("Error"))
                            PostOnPages.Text = string.Format(Resources.Resource.PostOnMyPagesFailureNotify, Convert.ToString(Session["TPForUser"]), "");
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToPages"])))
                            PostOnPages.Text = string.Format(Resources.Resource.PostOnMyPagesSuccessNotify, Convert.ToString(Session["TPForUser"]), "");
                    }
                    else
                    {
                        divPostOnPages.Visible = true;

                        if (Convert.ToString(Session["ShareToPages"]).Contains("Deactive"))
                            divPostOnPages.Visible = false;
                        else if (Convert.ToString(Session["ShareToPages"]).Contains("Error"))
                            PostOnPages.Text = string.Format(Resources.Resource.PostOnPagesFailureNotify, Convert.ToString(Session["ManagedUser"]), "");
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToPages"])))
                            PostOnPages.Text = string.Format(Resources.Resource.PostOnPagesSuccessNotify, Convert.ToString(Session["ManagedUser"]), "");
                    }

                    #endregion

                    if (Request.QueryString["type"] != "own")
                    {
                        divPostToOwnWall.Visible = true;

                        if (Convert.ToString(Session["FacebookOwnPage.Response"]).Contains("Deactive"))
                            divPostOnPages.Visible = false;
                        else if (Convert.ToString(Session["FacebookOwnPage.Response"]).Contains("Error"))
                            PostToOwnWall.Text =
                              string.Format(Resources.Resource.FacebookPostOnOwnWallFailureNotify, Convert.ToString(Session["TPForUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["FacebookOwnPage.Response"])))
                            PostToOwnWall.Text =
                                string.Format(Resources.Resource.FacebookPostOnOwnWallSuccessNotify, Convert.ToString(Session["TPForUser"]));


                        divPostOnMyFriendWall.Visible = true;

                        if (Convert.ToString(Session["ShareToOwnFriends"]).Contains("Deactive"))
                            divPostOnMyFriendWall.Visible = false;
                        else if (Convert.ToString(Session["ShareToOwnFriends"]).Contains("Error"))
                            PostOnMyFriendWall.Text = string.Format(Resources.Resource.PostOnMyFriendWallFailureNotify, Convert.ToString(Session["TPForUser"]));
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToOwnFriends"])))
                            PostOnMyFriendWall.Text = string.Format(Resources.Resource.PostOnMyFriendWallSuccessNotify, Convert.ToString(Session["TPForUser"]));


                        divPostOnMyPages.Visible = true;

                        if (Convert.ToString(Session["ShareToOwnPages"]).Contains("Deactive"))
                            divPostOnMyPages.Visible = false;
                        else if (Convert.ToString(Session["ShareToOwnPages"]).Contains("Error"))
                            PostOnMyPages.Text = string.Format(Resources.Resource.PostOnMyPagesFailureNotify, Convert.ToString(Session["TPForUser"]), "");
                        else if (!string.IsNullOrEmpty(Convert.ToString(Session["ShareToOwnPages"])))
                            PostOnMyPages.Text = string.Format(Resources.Resource.PostOnMyPagesSuccessNotify, Convert.ToString(Session["TPForUser"]), "");
                    }

                    #region Twitter

                    divTwitterpost.Visible = true;

                    if (Convert.ToString(Session["Twitter.Response"]).Contains("Deactive"))
                        divTwitterpost.Visible = false;
                    else if (Convert.ToString(Session["Twitter.Response"]).Contains("Error"))
                        Twitterpost.Text = Resources.Resource.TwitterpostFailureNotify;
                    else if (!string.IsNullOrEmpty(Convert.ToString(Session["Twitter.Response"])))
                        Twitterpost.Text = Resources.Resource.TwitterpostSuccessNotify;

                    #endregion
                }
                else
                {
                    TrackProtected.Text = Resources.Resource.TrackProtectedFailureNotify;
                }
            }
        }

        protected void ViewTrack_Command(object sender, CommandEventArgs e)
        {
            Session.Remove("TrackProtected");
            Session.Remove("SoundCloud.Response");
            Session.Remove("Facebook.Response");
            Session.Remove("FacebookGenre.Response");
            Session.Remove("FacebookSubGenre.Response");
            Session.Remove("ShareToFriends");
            Session.Remove("ShareToPages");
            Session.Remove("FacebookOwnPage.Response");
            Session.Remove("ShareToOwnFriends");
            Session.Remove("ShareToOwnPages");
            Session.Remove("Twitter.Response");

            Session.Remove("TPForUser");

            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                Response.Redirect("ViewDocManaged.aspx?id=" + Request.QueryString["id"]);
            else
                Response.Redirect("MemberTracks.aspx");
        }

        protected void NavigateControlPanel_Command(object sender, CommandEventArgs e)
        {
            Session.Remove("TrackProtected");
            Session.Remove("SoundCloud.Response");
            Session.Remove("Facebook.Response");
            Session.Remove("FacebookGenre.Response");
            Session.Remove("FacebookSubGenre.Response");
            Session.Remove("ShareToFriends");
            Session.Remove("ShareToPages");
            Session.Remove("FacebookOwnPage.Response");
            Session.Remove("ShareToOwnFriends");
            Session.Remove("ShareToOwnPages");
            Session.Remove("Twitter.Response");

            Session.Remove("TPForUser");

            Response.Redirect("MemberHome.aspx");
        }
    }
}