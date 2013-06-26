using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class Confirm : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["requestingUserinfo"] != null)
            {
                string id = Request.QueryString["id"];
                int _relationType = Convert.ToInt32(Request.QueryString["tp"]);
                string userName = Request.QueryString["requestingUserinfo"];
                string url = string.Format("/Account/Register.aspx?tp={0}&id={1}&requestingUserinfo={2}", _relationType, id, userName);
                Response.Redirect(url);
            }

            Session["bodyid"] = "user-home";

            //IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            int relationType = 0;
            if (!string.IsNullOrEmpty(Request.Params["tp"]))
            {
                if (!int.TryParse(Request.Params["tp"], out relationType))
                    relationType = 0;
            }

            if (Request.Params["id"] != null)
            {
                ConfirmationResult res = Confirmation.ProcessConfirmation(Request.Params["id"], relationType);
                switch (res)
                {
                    case ConfirmationResult.Success:
                        ResultLabel.Text = Resources.Resource.Confirmed;
                        break;

                    case ConfirmationResult.UserUnknown:
                        Response.Redirect(string.Format("~/Account/Register.aspx?id={0}&tp={1}", Request.Params["id"], relationType), false);
                        break;

                    case ConfirmationResult.ConfirmationFailed:
                        ResultLabel.Text = Resources.Resource.Rejected;
                        break;
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

            Control ctrlDiv = this.Master.FindControl("logoutDiv");

            ctrlDiv.Visible = false;
        }
    }
}