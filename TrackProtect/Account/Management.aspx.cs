using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace TrackProtect.Account
{
    public partial class Management : BasePage
    {
        DataTable Managers
        {
            get { return Session["ManagerTable"] as DataTable; }
            set { Session["ManagerTable"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure the user actually has clearance for this page, otherwise
            // revert to the default logged in page
            int vcl = 0, ecl = 0;
            Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);
            if (vcl < 1000 && ecl < 1000)
                Server.Transfer("~/Member/MemberHome.aspx");
        
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            if (!IsPostBack)
            {
                UpdateManagerTable();
            }
        }

        private void UpdateManagerTable()
        {
            Managers = GetManagers();
            ManagersTable.DataSource = Managers;
            ManagersTable.DataBind();
        }

        private DataTable GetManagers()
        {
            DataTable table = new DataTable("managers");
            table.Columns.Add("name", typeof (string));
            table.Columns.Add("vcl", typeof (int));
            table.Columns.Add("ecl", typeof (int));

            using (Database db = new MySqlDatabase())
            {
                DataTable dt = db.GetManagers();
                foreach (DataRow row in dt.Rows)
                {
                    long userId = (long) row["user_id"];
                    ClientInfo ci = db.GetClientInfo(userId);
                    int vcl = 0, ecl = 0;
                    Util.GetUserClearanceLevels(userId, out vcl, out ecl);

                    DataRow newRow = table.NewRow();
                    newRow["name"] = ci.GetFullName();
                    newRow["vcl"] = vcl;
                    newRow["ecl"] = ecl;
                    table.Rows.Add(newRow);
                }
            }
            return table;
        }

        protected void TpIdSearch(object sender, CommandEventArgs e)
        {
        	string email = TpIdText.Text;
            using (Database db = new MySqlDatabase())
            {
                long userId = db.GetUserIdByEmail(email);
                UserInfo ui = db.GetUser(userId);
                ClientInfo ci = db.GetClientInfo(userId);

                Session["mgmt.userid"] = userId;
                ManagerNameLabel.Text = ci.GetFullName();
            }
        }

        protected void AcceptUser(object sender, CommandEventArgs e)
        {
            if (Session["mgmt.userid"] == null)
                return;

			int mgrVcl = 0, mgrEcl = 0;
			Util.GetUserClearanceLevels(Util.UserId, out mgrVcl, out mgrEcl);

			int vcl = 0, ecl = 0, tmp = 0;
            if (int.TryParse(VclText.Text, out tmp))
                vcl = tmp;
            if (int.TryParse(EclText.Text, out tmp))
                ecl = tmp;
            
			// Rights entered may not be higher then the rights of the manager
			if (vcl <= mgrVcl && ecl <= mgrEcl)
			{
				long userIdToManage = (long)Session["mgmt.userid"];
				using (Database db = new MySqlDatabase())
				{
					db.RegisterUserRights(userIdToManage, vcl, ecl);
					Session.Remove("mgmt.userid");
					ManagerNameLabel.Text = string.Empty;
				}
				UpdateManagerTable();
			}
			else
			{
				ResultMessage.Text = Resources.Resource.UserRightsTooHigh;
			}
        }
    }
}