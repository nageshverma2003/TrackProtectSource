using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Admin
{
    public partial class AdminHome : System.Web.UI.Page
    {       
        protected void Page_Load(object sender, EventArgs e)
        {
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

        protected void Login_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                if (db.AdminLoginAuthentication(Email.Text.Trim(), Password.Text.Trim()))
                {
                    Session["AdminLogin"] = Email.Text.Trim();

                    Response.Redirect("ManagePages.aspx");                    
                }
                else
                {
                    CustomValidator CustomValidatorCtrl = new CustomValidator();

                    CustomValidatorCtrl.IsValid = false;

                    CustomValidatorCtrl.ValidationGroup = "LoginUserValidationGroup";

                    CustomValidatorCtrl.ErrorMessage = "Login Failed !";

                    this.Page.Form.Controls.Add(CustomValidatorCtrl);

                    Session.Remove("AdminLogin");
                }
            }
        }
    }
}