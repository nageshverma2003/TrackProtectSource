using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace TrackProtect.Member
{
    public partial class MemberHome : BasePage
    {
        private bool fieldsMissing = false;

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

            bool couponEntryState = true;
            UserInfo ui = null;
            ClientInfo ci = null;
            string userDocPath = null;
            using (Database db = new MySqlDatabase())
            {
                db.ResetUserWhmcsClientd(Util.UserId);

                ui = db.GetUser(Util.UserId);
                ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); //ci.GetFullName());
                //CreditsLiteral.Text     = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId)); 
                CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
                //ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);
                ProtectedLiteral.Text = Convert.ToString(protectedTracks);

                couponEntryState = (db.GetUserWhmcsClientId(Util.UserId) == 0);
                userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
            }

            int vcl = 0, ecl = 0;
            Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);
            //divManagedArtists.Visible = (vcl >= 100 || ecl >= 100);
            // managedArtist.Visible = (vcl >= 100 || ecl >= 100);
            fieldsMissing = false;
            decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
            ////Added by Nagesh to redirect user to complete profile page.
            //if (ui.IsActive == 0)
            //    Response.Redirect("~/FirstLogon.aspx?userId=" + ui.UserId);

            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(ci.GetFullName()))
                AddLine(sb, Resources.Resource.NoName);

            if (string.IsNullOrEmpty(ci.AddressLine1))
                AddLine(sb, Resources.Resource.NoAddress);

            if (string.IsNullOrEmpty(ci.City))
                AddLine(sb, Resources.Resource.NoResidence);

            if (string.IsNullOrEmpty(ci.OwnerKind))
                AddLine(sb, Resources.Resource.NoOwnerKind);

            if (string.IsNullOrEmpty(ci.TwitterId))
                AddLine(sb, Resources.Resource.NoTwitterId);

            if (string.IsNullOrEmpty(ci.FacebookId))
                AddLine(sb, Resources.Resource.NoFacebookId);

            if (string.IsNullOrEmpty(ci.SenaCode))
                AddLine(sb, Resources.Resource.NoSenaCode);

            if (string.IsNullOrEmpty(ci.IsrcCode))
                AddLine(sb, Resources.Resource.NoIsrcCode);

            string identDocName = string.Format("ID{0:D10}.cer", ui.UserId);
            string identDocPath = Path.Combine(userDocPath, identDocName);
            //if (!File.Exists(Server.MapPath(Request.ApplicationPath + identDocPath)))
            if (!File.Exists(identDocPath))
                AddLine(sb, Resources.Resource.NoCredentials);

            if (fieldsMissing)
                AddLine(sb, Resources.Resource.ClickToEdit);

            //MissingInfoPanel.Text = sb.ToString();
            //MissingInfoDiv.Visible = fieldsMissing;

            Session["percentComplete"] = percentComplete;

            CompletedLiteral.Text = string.Empty;
            if (percentComplete < 100)
                CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
            divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);

            //CouponCodeEdit.Enabled = couponEntryState;
            //CouponCodeGo.Enabled = couponEntryState;

            //------- Highlight the selected lang button ------- !

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

            ClientScript.RegisterStartupScript(this.GetType(), "HighLightMenu", "HighLightMenu('" + "Menu1" + "');", true);
        }

        private void AddLine(StringBuilder text, string lineToAdd)
        {
            if (text.Length > 0)
                text.Append("<br/>");
            text.Append(lineToAdd);
            fieldsMissing = true;
        }
    }
}