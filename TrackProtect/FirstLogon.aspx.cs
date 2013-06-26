using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data;
using System.Text;
using System.IO;
using System.Configuration;

namespace TrackProtect
{
    public partial class Explanation : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "firstlogin"; //"user-home";

            if (!IsPostBack)
            {
                if (isProfileCompleted())
                {
                    Response.Redirect("Member/MemberHome.aspx");
                }
                else
                {
                    bool flagIsActive = false;
                    //Get the value of isActive
                    if (Convert.ToInt32(Session["isActive"]) > 0)
                        flagIsActive = true;


                    //End getting value isActive
                    if (!string.IsNullOrEmpty(Request.QueryString["userId"]) && !string.IsNullOrEmpty(Request.QueryString["key"]) && !string.IsNullOrEmpty(Request.QueryString["mode"]))
                    {
                        if (Convert.ToInt32(Request.QueryString["mode"].Trim()) == 1)
                            Session["SignUpMode"] = "Facebook logon";
                        else if (Convert.ToInt32(Request.QueryString["mode"].Trim()) == 2)
                            Session["SignUpMode"] = "Manual Sign Up";
                        else
                            Session["SignUpMode"] = "Sign Up";


                        if (!flagIsActive)
                        {
                            SendMail(Convert.ToInt64(Request.QueryString["userId"]));
                            updateUserStatus(Convert.ToInt64(Request.QueryString["userId"]));
                        }

                        AuthenticateUser();
                    }
                    else
                    {
                        Session["SignUpMode"] = Resources.Resource.FirstLogonHeader; //"Sign Up";
                        if (!flagIsActive)
                        {
                            SendMail(Util.UserId);
                            updateUserStatus(Util.UserId);
                        }
                        using (Database db = new MySqlDatabase())
                        {
                            ClientInfo ci = db.GetClientInfo(Util.UserId);
                            Session["UserName"] = ci.GetFullName();
                        }
                    }

                    LogonMode.Text = Convert.ToString(Session["SignUpMode"]);
                    //FirstNameLiteral.Text = Convert.ToString(Session["UserName"]);
                    Dictionary<string, string> param1 = new Dictionary<string, string>();
                    TrackProtect.ParamsDictionary param = new ParamsDictionary();
                    //Replacing value of first Name from inc file.
                    param.Add("VarFirstName", Convert.ToString(Session["UserName"]));
                    IncludePage(ltrFirstLogon, Resources.Resource.FirstLogon, param);

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
        }

        private void SendMail(long userId)
        {
            string SignUpBody = string.Empty;

            string SignUpSubject = Resources.Resource.ManualSighUpEmailSubject;

            if (Convert.ToString(Session["SignUpMode"]).Contains("Facebook logon"))
                SignUpBody = Resources.Resource.FBAccountCreation;
            else
                SignUpBody = Resources.Resource.ManualSighUpEmailBody;


            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(userId);
                ClientInfo ci = db.GetClientInfo(ui.UserId);

                StringBuilder body = new StringBuilder();
                using (TextReader rdr = new StreamReader(Server.MapPath(SignUpBody)))
                {
                    string fname = ci.FirstName;
                    string text = rdr.ReadToEnd();

                    Session.Remove("register.pwd");
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

                    text = text.Replace("{%firstname%}", ci.FirstName);
                    text = text.Replace("{%email%}", ui.Email);
                    text = text.Replace("{%password%}", Convert.ToString(ViewState["pwd"]));

                    //string link = "<a href=\"http://test.trackprotect.com/FirstLogon.aspx?userId=\"" + Util.UserId + "&email=" + ui.Email + "&password=" + pwd + "\"> Click Here </a>";


                    string loginlink = ConfigurationManager.AppSettings["SiteNavigationLink"];
                    text = text.Replace("{%loginlink%}", loginlink);

                    string memberlink = ConfigurationManager.AppSettings["SiteNavigationLink"] + "/Member/MemberHome.aspx";
                    text = text.Replace("{%memberhomelink%}", memberlink);

                    body.Append(text);
                }

                Util.SendEmail(new string[] { ui.Email }, "noreply@trackprotect.com", SignUpSubject, body.ToString(), null,0);
            }
        }

        protected void AuthenticateUser()
        {
            string userID = Request.QueryString["userId"];

            string key = Request.QueryString["key"].Replace(" ", "+");
            string pwd = EncryptionClass.Decrypt(key);
            string email = string.Empty;

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Convert.ToInt64(userID));
                email = ui.Email;

                ClientInfo ci = db.GetClientInfo(Convert.ToInt64(userID));

                Session["UserName"] = ci.GetFullName();
            }

            string username = Membership.GetUserNameByEmail(email);

            FormsAuthentication.SetAuthCookie(username, false);

            FormsAuthenticationTicket ticket1 =
               new FormsAuthenticationTicket(
                    1,                                   // version
                    username,   // get username  from the form
                    DateTime.Now,                        // issue time is now
                    DateTime.Now.AddMinutes(10),         // expires in 10 minutes
                    false,      // cookie is not persistent
                    ""                              // role assignment is stored
                // in userData
                    );
            HttpCookie cookie1 = new HttpCookie(
              FormsAuthentication.FormsCookieName,
              FormsAuthentication.Encrypt(ticket1));
            Response.Cookies.Add(cookie1);

            Membership.ValidateUser(username, pwd);

            // 4. Do the redirect. 
            String returnUrl1;

            // the login is successful
            returnUrl1 = "FirstLogon.aspx";
            Response.Redirect(returnUrl1);
        }

        private void updateUserStatus(long userId)
        {
            try
            {
                using (Database db = new MySqlDatabase())
                {
                    db.UpdateUserStatus(userId);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private bool isProfileCompleted()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["userId"]))
            {
                Util.UserId = Convert.ToInt64(Request.QueryString["userId"]);
            }

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);
                DataSet ds = db.GetRegister(Util.UserId);

                string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                Session["percentComplete"] = percentComplete;

                Session["isActive"] = ui.IsActive;
                if (percentComplete < 100)
                    return false;
                else
                    return true;
            }
        }
    }
}