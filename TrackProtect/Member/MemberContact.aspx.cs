using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Configuration;
using TrackProtect.Logging;

namespace TrackProtect.Member
{
    public partial class MemberContact : BasePage
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
            new BasePage();

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                //CreditsLiteral.Text     = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId)); 
                CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
                //ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);
                ProtectedLiteral.Text = Convert.ToString(protectedTracks);
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            IncludePage(ContactLiteral, Resources.Resource.MemberContact);

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

            ClientScript.RegisterStartupScript(this.GetType(), "HighLightMenu", "HighLightMenu('" + "Menu5" + "');", true);
        }

        protected void Send_Click(object sender, EventArgs e)
        {
            Regex re = new Regex(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$");

            if (string.IsNullOrEmpty(Name.Text))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter your name." + "');", true);
            if (string.IsNullOrEmpty(Email.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter your email address." + "');", true);
            else if (string.IsNullOrEmpty(Message.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter some text in message." + "');", true);
            else if (!re.IsMatch(Email.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Invalid email address." + "');", true);
            else
            {
                StringBuilder body = new StringBuilder();

                using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.SupportEmailTpl)))
                {
                    string text = rdr.ReadToEnd();

                    text = text.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                    text = text.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                    text = text.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                    text = text.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                    text = text.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                    text = text.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                    text = text.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                    text = text.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                    text = text.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                    text = text.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                    text = text.Replace("{%name%}", Name.Text.Trim());
                    text = text.Replace("{%email%}", Email.Text.Trim());
                    text = text.Replace("{%message%}", Message.Text.Trim());

                    body.Append(text);
                }

                bool mailSent = true;

                try
                {
                    Util.SendEmail(new string[] { ConfigurationManager.AppSettings["SupportEmail"] }, Email.Text.Trim(), "Support Message", body.ToString(), null, 0);
                }
                catch
                {
                    mailSent = false;
                }

                body.Clear();

                if (mailSent)
                {
                    using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.InquiryAutoReplyBody)))
                    {
                        string text = rdr.ReadToEnd();

                        text = text.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                        text = text.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                        text = text.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                        text = text.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                        text = text.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                        text = text.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                        text = text.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                        text = text.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                        text = text.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                        text = text.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                        text = text.Replace("{%name%}", Name.Text.Trim());

                        body.Append(text);
                    }

                    try
                    {
                        Util.SendEmail(new string[] { Email.Text.Trim() }, "noreply@trackprotect.com", Resources.Resource.InquiryAutoReplySubject, body.ToString(), null, 0);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "MemberContact Send_Click");
                    }
                }

                Response.Redirect("ThankYou.aspx?id=msg");

                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "You message has been sent to our support team." + "');", true);
            }
        }
    }
}