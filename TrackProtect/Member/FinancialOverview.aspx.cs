using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace TrackProtect.Member
{
    public partial class FinancialOverview : BasePage
    {
        private string _culture = string.Empty;
        private string _userDocumentPath = string.Empty;

        [System.Web.Services.WebMethod]
        public static string SetDownloadPath(string strpath)
        {
            FinancialOverview obj = new FinancialOverview();
            obj.Session["strDwnPath"] = strpath;
            return strpath;
        }

        protected void Page_PreRender(Object o, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                bool isNotExpired = true;

                Facebook.AuthenticationService authService = new Facebook.AuthenticationService();

                Facebook.Me me;
                string accessToken = string.Empty;

                if (authService.TryAuthenticate(out me, out accessToken))
                {
                    isNotExpired = true;
                }
                else
                {
                    db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);
                    db.UpdateFacebookID(ci.ClientId);

                    isNotExpired = false;
                }

                if (!string.IsNullOrEmpty(ci.SoundCloudId))
                    SoundcloudItag.Attributes.Add("class", "soundcloud");
                else
                    SoundcloudItag.Attributes.Add("class", "soundcloud disabled");

                if (isNotExpired)
                    FacebookHeading.Attributes.Add("class", "social facebook");
                else
                    FacebookHeading.Attributes.Add("class", "social facebook disabled");

                if (!string.IsNullOrEmpty(ci.TwitterId))
                    TwitterHeading.Attributes.Add("class", "social twitter");
                else
                    TwitterHeading.Attributes.Add("class", "social twitter disabled");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "credits";
            IncludePage(FinancialEditInc, Resources.Resource.incFinancialOverview);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName);// ci.GetFullName());
                CreditsLiteral.Text = Util.GetUserCredits(Util.UserId).ToString();
                ProtectedLiteral.Text = protectedTracks.ToString();
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            if (!IsPostBack)
            {
                if (Session["culture"] != null)
                    _culture = Session["culture"] as string;
                long userId = Util.UserId;
                string passwd = Session["access"] as string;
                List<CreditHistory> creditHistory = null;
                using (Database db = new MySqlDatabase())
                {
                    _userDocumentPath = db.GetUserDocumentPath(userId, passwd);
                    creditHistory = db.GetCreditHistory(userId);
                }

                dlMyTracks.DataSource = creditHistory;
                dlMyTracks.DataBind();
                //Transactions transactions = Util.GetTransactions(Util.UserId);
                AddHeaders(FinancialOverviewTable);
                //foreach (Transaction transaction in transactions)
                //{
                //    AddTransaction(FinancialOverviewTable, transaction);
                //}
                foreach (CreditHistory ch in creditHistory)
                {
                    AddCreditHistory(FinancialOverviewTable, ch);
                }
                AddFooters(FinancialOverviewTable);
            }

            //------- Highlight the selected lang button ------- !

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

        private void AddHeaders(Table table)
        {
            TableRow row = new TableRow();
            AddCell(row, Unit.Pixel(100), string.Format("<div style=\"margin-left: 4px;\"><b>{0}</b></div>", Resources.Resource.Date));
            AddCell(row, Unit.Pixel(180), string.Format("<b>{0}</b>", Resources.Resource.Description));
            AddCell(row, Unit.Pixel(100), string.Format("<b>{0}</b>", Resources.Resource.ExpirationDate));
            AddCell(row, Unit.Pixel(100), string.Format("<b>{0}</b>", Resources.Resource.Credits));
            AddCell(row, Unit.Pixel(100), string.Format("<b>{0}</b>", Resources.Resource.Invoice));
            table.Rows.Add(row);

            row = new TableRow();
            row.Height = Unit.Pixel(2);
            AddLine(row);
            table.Rows.Add(row);
        }

        private void AddFooters(Table table)
        {
            TableRow row = new TableRow();
            row.Height = Unit.Pixel(2);
            AddLine(row);
            table.Rows.Add(row);
        }

        private void AddCell(TableRow row, string data)
        {
            AddCell(row, Unit.Empty, data);
        }

        private void AddCell(TableRow row, Unit width, string data)
        {
            TableCell cell = new TableCell();
            if (!width.IsEmpty)
                cell.Width = width;
            cell.Text = data;
            row.Cells.Add(cell);
        }

        private void AddLine(TableRow row)
        {
            TableCell cell = new TableCell();
            cell.ColumnSpan = 6;
            Image img = new Image();
            img.ImageUrl = "~/Images/hor_sep.png";
            cell.Controls.Add(img);
            row.Cells.Add(cell);
        }

        private void AddCell(TableRow row, string format, params object[] args)
        {
            AddCell(row, string.Format(format, args));
        }

        private void AddCell(TableRow row, Unit width, string format, params object[] args)
        {
            AddCell(row, width, string.Format(format, args));
        }

        private void AddTransaction(Table table, Transaction transaction)
        {
            CultureInfo ci = new CultureInfo("nl-NL");
            if (!string.IsNullOrEmpty(_culture))
                ci = new CultureInfo(_culture);
            TableRow row = new TableRow();
            AddCell(row, string.Format("<div style=\"margin-left: 4px;\">{0}</div>", transaction.Date.ToString("d", ci)));
            AddCell(row, transaction.Description);

            AddCell(row, transaction.Amount.ToString("C", ci));
            AddCell(row, transaction.PaymentMethod);
            string invoiceName = string.Empty;
            if (transaction.Status == "OK")
                invoiceName = string.Format("INV{0}.pdf", transaction.PaymentId);
            //AddCell(row, invoiceName);

            string navUrl = Path.Combine(_userDocumentPath, invoiceName);
            HyperLink hl = new HyperLink();
            if (!string.IsNullOrEmpty(navUrl) && File.Exists(navUrl))
            {
                hl.Text = invoiceName;
                hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", navUrl);
            }
            else
            {
                hl.Text = Resources.Resource.NoFile;
                hl.NavigateUrl = string.Empty;
            }
            TableCell cell = new TableCell();
            cell.Width = new Unit("32px");
            cell.Controls.Add(hl);
            row.Cells.Add(cell);

            table.Rows.Add(row);
        }

        private void AddCreditHistory(Table table, CreditHistory creditHistory)
        {
            CultureInfo ci = new CultureInfo("nl-NL");
            if (!string.IsNullOrEmpty(_culture))
                ci = new CultureInfo(_culture);
            TableRow row = new TableRow();
            AddCell(row, string.Format("<div style=\"margin-left: 4px;\">{0}</div>", creditHistory.PurchaseDate.ToString("d", ci)));
            AddCell(row, creditHistory.Description);
            AddCell(row, creditHistory.PurchaseDate.AddYears(1).ToString("d", ci));
            AddCell(row, creditHistory.Credits.ToString(ci));
            //AddCell(row, creditHistory.InvoiceFile);
            string navUrl = Path.Combine(_userDocumentPath, creditHistory.InvoiceFile ?? string.Empty);
            HyperLink hl = new HyperLink();
            if (!string.IsNullOrEmpty(navUrl) && File.Exists(navUrl))
            {
                hl.Text = creditHistory.InvoiceFile;
                hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", navUrl);
            }
            else
            {
                hl.Text = Resources.Resource.NoFile;
                hl.Enabled = false;
                hl.NavigateUrl = string.Empty;
            }
            TableCell cell = new TableCell();
            cell.Width = new Unit("32px");
            cell.Controls.Add(hl);
            row.Cells.Add(cell);

            table.Rows.Add(row);

        }

        protected void dlMyTracks_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            HiddenField hdnInvoice = e.Item.FindControl("hdnInvoice") as HiddenField;
            HyperLink hlInvoice = e.Item.FindControl("hlInvoice") as HyperLink;
            //string navUrl = Server.MapPath(Path.Combine(_userDocumentPath, hdnInvoice.Value ?? string.Empty));
            string navUrl = Path.Combine(_userDocumentPath, hdnInvoice.Value ?? string.Empty).Replace("\\", "/");

            if (!string.IsNullOrEmpty(hdnInvoice.Value))
            {
                //hlInvoice.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", navUrl);
                hlInvoice.Attributes.Add("onclick", "InitializeRequest('" + navUrl + "');");
            }
            else
            {
                hlInvoice.Enabled = false;
            }
        }
    }
}