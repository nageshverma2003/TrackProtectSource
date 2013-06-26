using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;

namespace TrackProtect.Member
{
	public partial class MemberContact1 : BasePage
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(MemberContactInc, Resources.Resource.incMemberContact);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

			//if (Util.UserId == 1)
			//    Util.TestInvoice();

			using (Database db = new MySqlDatabase())
			{
				UserInfo ui = db.GetUser(Util.UserId);
				ClientInfo ci = db.GetClientInfo(Util.UserId);
				DataSet ds = db.GetRegister(Util.UserId);
				int protectedTracks = ds.Tables[0].Rows.Count;

				LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
				LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.GetFullName());
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

