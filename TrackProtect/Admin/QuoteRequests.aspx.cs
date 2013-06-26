using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Admin
{
	public partial class QuoteRequests : BasePage
	{
		private DataTable QuotationsList
		{
			get { return Session["QuotationsList"] as DataTable; }
			set { Session["QuotationsList"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (QuotationsList == null)
			{
				QuotationsList = new DataTable("quotations");
				QuotationsList.Columns.Add("date", typeof(DateTime));
				QuotationsList.Columns.Add("email", typeof(string));
				QuotationsList.Columns.Add("amount", typeof(Decimal));
				QuotationsList.Columns.Add("credits", typeof(Int32));
				QuotationsList.Columns.Add("transaction_id", typeof(ulong));
				QuotationsList.Columns.Add("user_id", typeof(ulong));
			}

			UpdateUserStatusPanel();

			if (!IsPostBack)
			{
				FillQuotationsList();
			}
			else
			{
				
			}
		}

		private void FillQuotationsList()
		{
			QuotationsList.Rows.Clear();
			using (Database db = new MySqlDatabase())
			{
				DataTable temp = db.GetAllOpenQuotations();
				foreach (DataRow row in temp.Rows)
				{
					DataRow newRow = QuotationsList.NewRow();

					newRow["date"] = row["date"];
					newRow["email"] = row["email"];
					newRow["amount"] = row["amount"];
					string statuscode = row["statuscode"] as string;
					int credits = 0;
					if (statuscode.StartsWith("RFQ"))
					{
						int pos = statuscode.IndexOf('(');
						if (pos > -1)
							statuscode = statuscode.Substring(pos);
						pos = statuscode.IndexOf(')');
						if (pos > -1)
							statuscode = statuscode.Substring(0, pos);
						string[] parts = statuscode.Split(':');
						if (parts.Length == 2)
						{
							if (!int.TryParse(parts[1], out credits))
								credits = 0;
						}
					}
					newRow["credits"] = credits;
					newRow["transaction_id"] = row["transaction_id"];
					newRow["user_id"] = row["user_id"];

					QuotationsList.Rows.Add(newRow);
				}
			}


			DataView dataView = new DataView(QuotationsList);
			quotationsList.DataSource = dataView;
			quotationsList.DataBind();
		}

		private void UpdateUserStatusPanel()
		{
			UserInfo ui = null;
			ClientInfo ci = null;
			string userDocPath = null;
			using (Database db = new MySqlDatabase())
			{
				db.ResetUserWhmcsClientd(Util.UserId);

				ui = db.GetUser(Util.UserId);
				ci = db.GetClientInfo(Util.UserId);
				DataSet ds = db.GetRegister(Util.UserId);
				int protectedTracks = ds.Tables[0].Rows.Count;

				LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
				LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.GetFullName());
				CreditsLiteral.Text = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId));
				ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);

				userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
			}

			int vcl = 0, ecl = 0;
			Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);

			decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
			CompletedLiteral.Text = string.Empty;
			if (percentComplete < 100)
				CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
			ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
		}

		protected void quotationsList_ItemEditing(object sender, ListViewEditEventArgs e)
		{
			int rowIndex = e.NewEditIndex;
			DataRow row = QuotationsList.Rows[rowIndex];
			string url = string.Format("~/Admin/QuotationEdit.aspx?tid={0}&uid={1}&credits={2}",
				row["transaction_id"],
				row["user_id"],
				row["credits"]);
			Response.Redirect(url, false);
		}
	}
}