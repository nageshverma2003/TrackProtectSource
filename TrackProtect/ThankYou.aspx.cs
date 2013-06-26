using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class ThankYou : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session != null)
                {
                    // Store essential session information temporarily
                    string culture = Session["culture"] as string ?? "nl-NL";

                    Session.Clear();

                    // Restore the essential session information
                    Session["culture"] = culture;
                }

                if (Request.QueryString.Count > 0 && Convert.ToInt32(Request.QueryString["id"]) == 1)
                {
                    ThankyouMsg.Text = Resources.Resource.SignupFBAlreadyExist;
                }
                else if (Request.QueryString.Count > 0 && Convert.ToInt32(Request.QueryString["id"]) == 2)
                {
                    ThankyouMsg.Text = Resources.Resource.SignupFBError;
                }
                else if (Request.QueryString.Count > 0 && Convert.ToInt32(Request.QueryString["id"]) == 3)
                {
                    ThankyouMsg.Text = Resources.Resource.ContactMsg;
                }
                else if (Request.QueryString.Count > 0 && Convert.ToInt32(Request.QueryString["id"]) == 4)
                {
                    ThankyouMsg.Text = Resources.Resource.FBAuthenticateFail;
                }
                else
                {
                    IncludePage(ThankyouMsg, Resources.Resource.Thankyoumsg);
                }


                IncludePage(FooterLiteral, Resources.Resource.FooterSection);
            }

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageNL" + "');", true);
            }
        }

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }
    }
}