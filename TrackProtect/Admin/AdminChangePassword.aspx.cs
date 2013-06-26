using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Admin
{
    public partial class AdminChangePassword : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            new BasePage();

            Control ctrlDiv = this.Master.FindControl("logoutDiv");

            ctrlDiv.Visible = false;

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
}