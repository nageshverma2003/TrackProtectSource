using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace TrackProtect
{
    public partial class Logon : System.Web.UI.MasterPage
    {
        public string bodyId = string.Empty;

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
            if (Request.Params["id"] == null)
            {
                if (HttpContext.Current != null &&
                    HttpContext.Current.User != null &&
                    HttpContext.Current.User.Identity.IsAuthenticated == true &&
                    HttpContext.Current.Session["userid"] == null)
                {
                    // We got here in a strange way, user is unknown (Session cleared) but
                    // not properly signed out. We try to sign out again and redirect back
                    // here.
                    FormsAuthentication.SignOut();
                    Response.Redirect("~/Default.aspx", false);
                }
            }          

            BasePage obj = new BasePage();
            obj.IncludePage(FooterLiteral, Resources.Resource.FooterSection);
        }

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }

        //protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        //{
        //    Login Login1 = HeadLoginView1.FindControl("Login1") as Login;
        //    if (Login1 == null)
        //        return;

        //    if (Login1.UserName.Contains("@"))
        //    {
        //        string username = Membership.GetUserNameByEmail(Login1.UserName);
        //        if (username != null)
        //        {
        //            if (Membership.ValidateUser(username, Login1.Password))
        //            {
        //                Login1.UserName = username;
        //                e.Authenticated = true;
        //            }
        //            else
        //            {
        //                e.Authenticated = false;
        //            }
        //        }
        //    }
        //    else 
        //    {
        //        e.Authenticated = Membership.ValidateUser(Login1.UserName, Login1.Password);
        //    }
        //}
    }
}