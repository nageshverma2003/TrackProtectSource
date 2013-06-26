using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TrackProtect.Member
{
    public partial class Quotation : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["pid"] != null)
            {
                long productId;
                if (!long.TryParse(Request.Params["pid"], out productId))
                    productId = 0;
                Session["quotation.pid"] = productId;
            }

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
                string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }
        }

        protected void SendQuotation(object sender, CommandEventArgs e)
        {
            long productId = 0;
            int credits = 0;
            if (QuotationAmount.Text.Length > 0)
            {
                int tmp = 0;
                if (int.TryParse(QuotationAmount.Text, out tmp))
                    credits = tmp;
            }

            if (Session["quotation.pid"] != null)
                productId = (long)Session["quotation.pid"];

            if (credits > 0)
            {
                // Send e-mail with quotation request
                Config cfg = new Config();
                cfg.Load(Server.MapPath("~/Config/trackprotect.config"));

                string emailTo = cfg["email.sales"];
                StringBuilder body = new StringBuilder();

                // Get user/client information
                using (Database db = new MySqlDatabase())
                {
                    long userId = Util.UserId;
                    UserInfo ui = db.GetUser(userId);
                    ClientInfo ci = db.GetClientInfo(userId);
                    body.Append("<html><head></head><body><p><div style=\"background-color:#E6E6E6;\"><table><tr><td>");
                    body.Append("<img src=\"" + ConfigurationManager.AppSettings["EmailHeaderLogo"] + "\" alt=\"logo\"/></td><td><span>");
                    body.Append("<a style=\"color:#E4510A; margin-left:450px;\" href=\"" + ConfigurationManager.AppSettings["EmailmailToLink"] + "\" >" + ConfigurationManager.AppSettings["EmailmailToLink"] + "</a>");
                    body.Append("<br><a style=\"color:#E4510A; margin-left:450px;\" href=\"" + ConfigurationManager.AppSettings["SiteNavigationLink"] + "\">" + ConfigurationManager.AppSettings["SiteNavigationLink"] + "</a>");
                    body.Append("</span></td></tr></table></div><br><br>");

                    body.Append(ci.GetFullName());
                    body.AppendFormat("heeft een offerte aangevraagd voor {0} credits.\r\n", QuotationAmount.Text);
                    body.AppendFormat("e-mail         : {0}\r\n", ui.Email);
                    body.AppendFormat("e-mail receipt : {0}\r\n", ci.EmailReceipt);
                    body.AppendFormat("user-id        : {0}\r\n", ui.UserId);
                    body.AppendFormat("client-id      : {0}\r\n", ci.ClientId);

                    body.Append("<br><br><img src=\"" + ConfigurationManager.AppSettings["EmailFooterLogo"] + "\" alt=\"logo\"/>");
                    body.Append("<a href=\"" + ConfigurationManager.AppSettings["EmailFBlink"] + "\"><img src=\"" + ConfigurationManager.AppSettings["EmailFBLogo"] + "\" alt=\"facebook\"></img></a>");
                    body.Append("<a href=\"" + ConfigurationManager.AppSettings["EmailTwitterLink"] + "\"><img src=\"" + ConfigurationManager.AppSettings["EmailTwitterLogo"] + "\" alt=\"twitter\"></img></a>");
                    body.Append("<a href=\"" + ConfigurationManager.AppSettings["EmailSoundCloudLink"] + "\"><img src=\"" + ConfigurationManager.AppSettings["EmailSoundCloudLogo"] + "\" alt=\"soundcloud\"></img></a>");
                    body.Append("<br><br>Trackprotect is “a RHOS Initiative” – Robin Hood of Sound – A Sound Revolution.<br>");

                    if (productId == 0)
                        productId = 4;

                    db.CreateQuotation(ui.UserId, credits, productId, string.Format("Quotation for {0} credits", QuotationAmount.Text));
                }

                Util.SendEmail(new string[] { emailTo }, "noreply@trackprotect.com", Resources.Resource.Quotation, body.ToString(), new string[] { }, 0);
                Response.Redirect("~/Member/QuotationSuccess.aspx", false);
            }
            else
            {
                ErrorMessage.Text = Resources.Resource.InvalidAmount;
            }
        }
    }
}