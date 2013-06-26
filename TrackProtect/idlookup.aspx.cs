using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Configuration;
using TrackProtect.Logging;

namespace TrackProtect
{
    public partial class idlookup : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            new BasePage();

            Session["bodyid"] = "coupon";

            Control ctrlDiv = this.Master.FindControl("logoutDiv");

            ctrlDiv.Visible = false;

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["TrackID"]))
                {
                    using (Database db = new MySqlDatabase())
                    {
                        IDictionary<string, string> trackInfo = new Dictionary<string, string>();

                        try
                        {
                            string decryptString = EncryptionClass.Decrypt(Request.QueryString["TrackID"]);
                            if (!string.IsNullOrEmpty(decryptString))
                            {
                                if (Convert.ToInt32(decryptString) != 0)
                                {
                                    trackInfo = db.getTrackInformationByID(Convert.ToInt32(decryptString));

                                    StageName.Text = trackInfo["StageName"];

                                    TrackName.Text = trackInfo["TrackName"];

                                    ISRCCode.Text = trackInfo["isrcCode"];

                                    string tags = string.Empty;

                                    tags = tags + (string.IsNullOrEmpty(trackInfo["genre1"]) ? string.Empty : trackInfo["genre1"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["genre2"]) ? string.Empty : trackInfo["genre2"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["genre3"]) ? string.Empty : trackInfo["genre3"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["subgenre1"]) ? string.Empty : trackInfo["subgenre1"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["subgenre2"]) ? string.Empty : trackInfo["subgenre2"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["subgenre3"]) ? string.Empty : trackInfo["subgenre3"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["tag1"]) ? string.Empty : trackInfo["tag1"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["tag2"]) ? string.Empty : trackInfo["tag2"] + " ");
                                    tags = tags + (string.IsNullOrEmpty(trackInfo["tag3"]) ? string.Empty : trackInfo["tag3"] + " ");

                                    AddedTags.Text = tags;
                                }
                                else
                                {
                                    StageName.Text = TrackName.Text = ISRCCode.Text = AddedTags.Text = Resources.Resource.NoTrackInfoFound;

                                    ClientScript.RegisterStartupScript
                                        (this.GetType(), "alert", "alert('" + Resources.Resource.TrackInfoCannotShown + "');", true);
                                }
                            }
                            else
                            {
                                StageName.Text = TrackName.Text = ISRCCode.Text = AddedTags.Text = Resources.Resource.NoTrackInfoFound;

                                ClientScript.RegisterStartupScript
                                    (this.GetType(), "alert", "alert('" + Resources.Resource.TrackInfoCannotShown + "');", true);
                            }
                        }
                        catch
                        {
                            StageName.Text = TrackName.Text = ISRCCode.Text = AddedTags.Text = Resources.Resource.NoTrackInfoFound;

                            ClientScript.RegisterStartupScript
                                (this.GetType(), "alert", "alert('" + Resources.Resource.TrackInfoCannotShown + "');", true);
                        }
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + Resources.Resource.NoTrackInfoFound + "');", true);
                }
            }

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
        }

        protected void Send_Click(object sender, EventArgs e)
        {
            Regex re = new Regex(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$");

            if (string.IsNullOrEmpty(Subject.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter subject for this mail." + "');", true);
            else if (string.IsNullOrEmpty(Name.Text))
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
                    Util.SendEmail(new string[] { ConfigurationManager.AppSettings["SupportEmail"] }, Email.Text.Trim(), Subject.Text.Trim(), body.ToString(), null,0);
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
                        Util.SendEmail(new string[] { Email.Text.Trim() }, "noreply@trackprotect.com", Resources.Resource.InquiryAutoReplySubject, body.ToString(), null,0);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "MemberContact Send_Click");
                    }
                }

                Response.Redirect("ThankYou.aspx?id=msg");
            }
        }
    }
}