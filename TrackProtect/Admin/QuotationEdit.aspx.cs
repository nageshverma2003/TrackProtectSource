using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TrackProtect.Admin
{
    public partial class QuotationEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            UpdateUserStatusPanel();

            if (!IsPostBack)
            {
                if (Request.Params["credits"] != null)
                {
                    int credits = Convert.ToInt32(Request.Params["credits"]);
                    Credits.Text = credits.ToString();
                }
            }
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

        protected void StoreQuotation(object sender, CommandEventArgs e)
        {
            long transactionId = 0, userId = 0;
            if (Request.Params["tid"] != null)
                transactionId = Convert.ToInt64(Request.Params["tid"]);
            if (Request.Params["uid"] != null)
                userId = Convert.ToInt64(Request.Params["uid"]);

            decimal amount = 0m;
            if (!decimal.TryParse(Amount.Text, out amount))
                amount = 0m;

            if (amount > 0m)
            {
                Config config = new Config();
                config.Load(HttpContext.Current.Server.MapPath("~/Config/trackprotect.config"));
                using (Database db = new MySqlDatabase())
                {
                    string from = config["email.sales"];
                    if (string.IsNullOrEmpty(from))
                        from = "sales@trackprotect.com";

                    UserInfo ui = db.GetUser(userId);
                    ClientInfo ci = db.GetClientInfo(userId);

                    db.UpdateQuotation(transactionId, amount);
                    Transaction transaction = db.GetQuotation(transactionId);
                    string body = string.Empty;
                    string paymentLink = string.Empty;
                    paymentLink = string.Format(Resources.Resource.QuoteReplyLink, transactionId, transaction.ProductId);
                    using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.emlQuoteReplyBody)))
                    {
                        string fname = ci.FirstName;
                        body = rdr.ReadToEnd();
                        body = body.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                        body = body.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                        body = body.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                        body = body.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                        body = body.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                        body = body.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                        body = body.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                        body = body.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                        body = body.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                        body = body.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                        body = body.Replace("{%salutation%}", ci.GetFullName());
                        body = body.Replace("{%username%}", ui.Email);
                        body = body.Replace("{%firstname%}", ci.FirstName);
                        body = body.Replace("{%lastname%}", ci.LastName);
                        body = body.Replace("{%addressline1%}", ci.AddressLine1);
                        body = body.Replace("{%addressline2%}", ci.AddressLine2);
                        body = body.Replace("{%zipcode%}", ci.ZipCode);
                        body = body.Replace("{%city%}", ci.City);
                        body = body.Replace("{%state%}", ci.State);
                        body = body.Replace("{%country%}", ci.Country);
                        body = body.Replace("{%telephone%}", ci.Telephone);
                        body = body.Replace("{%cellular%}", ci.Cellular);
                        body = body.Replace("{%email%}", ui.Email);
                        body = body.Replace("{%ownerkind%}", ci.OwnerKind);
                        body = body.Replace("{%twitterid%}", ci.TwitterId);
                        body = body.Replace("{%facebookid%}", ci.FacebookId);
                        body = body.Replace("{%senacode%}", ci.SenaCode);
                        body = body.Replace("{%isrccode%}", ci.IsrcCode);
                        body = body.Replace("{%link%}", paymentLink);
                    }

                    Util.SendEmail(new string[] { ui.Email }, from, Resources.Resource.emlQuoteReplySubject, body, null,0);
                }
            }
        }
    }
}