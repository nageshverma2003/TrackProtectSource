using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace TrackProtect.Member
{
    public partial class ThankYou : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "user-home";

            if (Request.QueryString.Count > 0 && Request.QueryString["id"].Contains("msg"))
            {
                ThankYouMsg.Text = Resources.Resource.ContactMsg;
            }
            else
            {
                ThankYouMsg.Text = Resources.Resource.ProfileCompleteion;
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

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }
    }
}