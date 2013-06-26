using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class FAQ : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "coupon";
            IncludePage(FAQInc, Resources.Resource.incFAQ);
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
    }
}