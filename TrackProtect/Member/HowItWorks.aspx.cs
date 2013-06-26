using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class HowItWorks : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(HowItWorksInc, Resources.Resource.incHowItWorks);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);
                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                CreditsLiteral.Text = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId));
                ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            if (!IsPostBack)
            {
            }
        }
    }
}
