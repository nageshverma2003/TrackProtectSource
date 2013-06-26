using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class UploadCredentialResult : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

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

            if (Request.Params["res"] != null)
            {
                if (Request.Params["res"].ToLower() == "ok")
                    result0.Text = Resources.Resource.UploadCredentialsSuccess;
                else if (Request.Params["res"].ToLower() == "err")
                    result0.Text = Resources.Resource.UploadCredentialsFailed;
            }

            string res1 = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["t1"]))
            {
                char errCode = Request.Params["t1"][0];
                string file = Request.Params["t1"].Substring(1);
                switch (errCode)
                {
                    case 'E':
                        res1 = string.Format(Resources.Resource.FileStoreFailed, file);
                        break;
                    case 'O':
                        res1 = string.Format(Resources.Resource.FileStoreSuccess, file);
                        break;
                }
            }

            string res2 = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["t2"]))
            {
                char errCode = Request.Params["t2"][0];
                string file = Request.Params["t2"].Substring(1);
                switch (errCode)
                {
                    case 'E':
                        res2 = string.Format(Resources.Resource.FileStoreFailed, file);
                        break;
                    case 'O':
                        res2 = string.Format(Resources.Resource.FileStoreSuccess, file);
                        break;
                }
            }
            result1.Text = res1;
            result2.Text = res2;
        }
    }
}