using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using TrackProtect.Logging;

namespace TrackProtect
{
    public partial class ResetPassword : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "signup";

            //IncludePage(ResetPasswordInc, Resources.Resource.incResetPassword);
            //IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
            }

            Control ctrlDiv = this.Master.FindControl("logoutDiv");

            ctrlDiv.Visible = false;
        }

        protected void ResetPasswordCommand(object sender, CommandEventArgs e)
        {
            try
            {
                string password = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
                int totalRecords = 0;
                MembershipUserCollection users = Membership.FindUsersByEmail(Email.Text, 0, 10000000, out totalRecords);
                if (users != null)
                {
                    if (users.Count > 0)
                    {
                        foreach (MembershipUser user in users)
                        {
                            user.ChangePassword(user.ResetPassword(), password);
                            string body;
                            using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.tplPasswordReset)))
                            {
                                body = rdr.ReadToEnd();
                            }

                            body = body.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                            body = body.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                            body = body.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                            body = body.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                            body = body.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                            body = body.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                            body = body.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                            body = body.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                            body = body.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                            body = body.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                            body = body.Replace("{%userpass%}", password);
                            Util.SendEmail(new string[] { Email.Text }, "noreply@trackprotect.com", Resources.Resource.PasswordReset, body, null, 0);
                            ResultMessage.Text = Resources.Resource.PasswordIsReset;
                            Email.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "ResetPassword<Exception>");
            }
        }
    }
}