using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TrackProtect
{
    public partial class Profile : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session != null)
            {
                // Store essential session information temporarily
                string culture = Session["culture"] as string ?? "nl-NL";

                Session["culture"] = null;

                // Restore the essential session information
                Session["culture"] = culture;
            }

            if (!Request.IsAuthenticated)
            {
                if (Request.Params["pid"] != null || Request.Params["tid"] != null)
                {
                    Response.Redirect(string.Format("~/Account/Login.aspx?tid={0}&pid={1}",
                                    Request.Params["tid"],
                                    Request.Params["pid"]), false);
                }
                else
                {
                    Response.Redirect("~/Default.aspx", false);
                }
            }

            ShowLoggedOnView();

            Literal name = HeadLoginView.FindControl("HeadLoginName") as Literal;
            if (name != null)
            {
                if (Session["userid"] != null)
                {
                    long userId = Util.UserId;
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(userId);
                        name.Text = string.Format(" {0}", ci.FirstName);// ci.GetFullName());
                    }
                }
            }
            if (!IsPostBack)
            {
                BasePage obj = new BasePage();
                obj.IncludePage(FooterLiteral, Resources.Resource.FooterSection);
            }
        }

        private void ShowLoggedOnView()
        {
            long userId = Util.UserId;
            int credits = Util.GetUserCredits(userId);
            UserInfo ui = null;
            ClientInfo ci = null;
            if (userId > 0)
            {
                ui = Util.GetUserInfo(userId);
                ci = Util.GetClientInfo(userId);
            }
            string culture = null;
            if (ci != null)
            {
                if (!string.IsNullOrEmpty(ci.Country) && !string.IsNullOrEmpty(ci.Language))
                {
                    string cultLang = Util.GetLanguageCodeByEnglishName(ci.Language);
                    string cultCtry = Util.GetCountryIso2(ci.Country);
                    culture = string.Format("{0}-{1}", cultLang, cultCtry);
                }
            }
            int vcl = 0, ecl = 0;
            Util.GetUserClearanceLevels(userId, out vcl, out ecl);

            //GoHome.NavigateUrl = "~/Member/MemberHome.aspx";           
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            Login Login1 = HeadLoginView.FindControl("Login1") as Login;
            if (Login1 == null)
                return;

            if (Login1.UserName.Contains("@"))
            {
                string username = Membership.GetUserNameByEmail(Login1.UserName);
                if (username != null)
                {
                    if (Membership.ValidateUser(username, Login1.Password))
                    {
                        Login1.UserName = username;
                        e.Authenticated = true;
                    }
                    else
                    {
                        e.Authenticated = false;
                    }
                }
            }
            else // Standard username & password login
            {
                e.Authenticated = Membership.ValidateUser(Login1.UserName, Login1.Password);
            }
        }

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }
    }
}