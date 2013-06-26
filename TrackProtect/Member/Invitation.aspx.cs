using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace TrackProtect.Member
{
    public partial class Invitation : BasePage
    {
        int _vcl = 0;
        int _ecl = 0;

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
            Session["bodyid"] = "relationships";
            Session["loggedinUserEmail"] = string.Empty;
            Util.GetUserClearanceLevels(Util.UserId, out _vcl, out _ecl);
            if (_vcl < 100 && _ecl < 100)
            {
                divManaccChk.Visible = false;
            }
            else
            {
                divManaccChk.Visible = true;
            }
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;
                Session["loggedinUserEmail"] = ui.Email;
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


            //------- Highlight the selected lang button ------- !

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);

                ddl_Language.SelectedValue = "nl";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);

                ddl_Language.SelectedValue = "en";
            }
        }

        protected void TpIdSearch(object sender, CommandEventArgs e)
        {
            if (cbxManaccRelation.Checked)
                RequestConfirmation(TpIdText.Text, 1, ddl_Language.SelectedValue);
            else
                RequestConfirmation(TpIdText.Text, 0, ddl_Language.SelectedValue);
        }


        #region Page Functions
        private void RequestConfirmation(string email, int relationType, string language)
        {
            string url = string.Empty;

            if (Convert.ToString(Session["loggedinUserEmail"]).Trim() == email.Trim())
            {
                Response.Redirect("~/Member/InviteFailure.aspx?err=4");
            }
            if (!Util.VerifyEmail(email))
            {
                url = "~/Member/InviteFailure.aspx?err=1";
            }
            else
            {
                switch (Confirmation.RequestConfirmation(
                    email,
                    FirstName.Text,
                    LastName.Text,
                    Guid.NewGuid().ToString(),
                    Util.UserId,
                    relationType,
                    language
                    ))
                {
                    case Confirmation.ConfirmationRequestResult.Success:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=0", email, TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.Exists:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=1", email, TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.Failed:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=2", email, TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.AlreadyRequested:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=3", email, TpIdText.Text);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(url))
                Response.Redirect(url, false);
        }
        #endregion
    }
}