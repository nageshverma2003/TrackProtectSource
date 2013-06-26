using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using TrackProtect.Logging;
using System.IO;
using System.Configuration;

namespace TrackProtect
{
    public partial class PayResult : BasePage
    {
        string email = string.Empty;
        string name = string.Empty;

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
            Session["bodyid"] = "user-home";

            //IncludePage(PayResultInc, Resources.Resource.incPayResult);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                email = ui.Email;
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                name = ci.FirstName;
                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
                ProtectedLiteral.Text = Convert.ToString(protectedTracks);
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            string res = Request.Params["res"] ?? "unknown";
            if (!string.IsNullOrEmpty(res))
            {
                switch (res.ToLower())
                {
                    case "success":
                        ProcessTransaction();
                        break;

                    case "error":
                        ProcessFailure();
                        break;

                    case "postback":
                        ProcessPostback();
                        break;

                    default:
                        break;
                }
            }

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


            CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
        }

        private void ProcessTransaction()
        {
            long userId = Util.UserId;
            long prodId = 0L;
            string country = "NL";
            string currency = "EUR";
            string culture = "nl-NL";
            decimal amount = 0m;
            if (Session["pid"] != null)
                prodId = Convert.ToInt64(Session["pid"]);
            if (Session["amt"] != null)
                amount = Convert.ToDecimal(Session["amt"]);
            if (Session["culture"] != null)
                culture = Session["culture"] as string;
            country = culture.Substring(3);
            currency = Util.GetCurrencyIsoNameByCountryIso2(country);

            Logger.Instance.Write(LogLevel.Info, "Process successful transaction: {0}", Request.RawUrl);

            string res = Request.Params["res"] ?? string.Empty;
            string status = Request.Params["Status"] ?? string.Empty;
            string statusCode = Request.Params["StatusCode"] ?? string.Empty;
            string merchant = Request.Params["Merchant"] ?? string.Empty;
            string orderId = Request.Params["OrderID"] ?? string.Empty;
            string paymentId = Request.Params["PaymentID"] ?? string.Empty;
            string reference = Request.Params["Reference"] ?? string.Empty;
            string transid = Request.Params["TransactionID"] ?? string.Empty;
            string paymentMethod = Request.Params["PaymentMethod"] ?? string.Empty;
            if (!string.IsNullOrEmpty(statusCode))
                statusCode = Uri.UnescapeDataString(statusCode);
            using (Database db = new MySqlDatabase())
            {
                StringBuilder sb = new StringBuilder();
                ProductInfo pi = db.GetProductById(prodId);
                switch (db.UpdateTransaction(orderId, res, status, statusCode, merchant, paymentId, reference,
                                 transid, paymentMethod, amount, pi, currency, country))
                {
                    case TransactionResult.Success:
                        {
                            long Transaction_id = 0;
                            long.TryParse(orderId, out Transaction_id);

                            db.UpdateUserCredits(userId, prodId, pi.Credits);
                            db.AddCreditHistory(userId, prodId, pi.Credits, Transaction_id);

                            sb.Append("<div>");
                            sb.AppendFormat("<p><strong>{0}</strong></p>", Resources.Resource.TransactionSuccessful);
                            sb.AppendFormat(Resources.Resource.HasCreditsNow, Util.GetUserCredits(userId));
                            sb.Append("</div>");
                        }
                        break;

                    case TransactionResult.NotFound:
                        {
                            sb.Append("<div>");
                            sb.AppendFormat("<p><strong>{0}</strong></p>", Resources.Resource.TransactionFailed);
                            sb.AppendFormat(Resources.Resource.HasCreditsNow, Util.GetUserCredits(userId));
                            sb.Append("</div>");
                        }
                        break;

                    case TransactionResult.AlreadyCompleted:
                        {
                            sb.Append("<div>");
                            sb.AppendFormat("<p><strong>{0}</strong></p>", Resources.Resource.TransactionAlreadyCompleted);
                            sb.AppendFormat(Resources.Resource.HasCreditsNow, Util.GetUserCredits(userId));
                            sb.Append("</div>");
                        }
                        break;
                }

                ResultLiteral.Text = sb.ToString();
            }

            sendMail(orderId, Resources.Resource.CreditPurchaseSuccessSubject, Resources.Resource.CreditPurchaseSuccessBody);
        }

        private void ProcessFailure()
        {
            Logger.Instance.Write(LogLevel.Info, "Process failed transaction: {0}", Request.RawUrl);
            long userId = Util.UserId;
            long prodId = 0L;
            string country = "NL";
            string currency = "EUR";
            string culture = "nl-NL";
            decimal amount = 0m;
            if (Session["pid"] != null)
                prodId = Convert.ToInt64(Session["pid"]);
            if (Session["amt"] != null)
                amount = Convert.ToDecimal(Session["amt"]);
            if (Session["culture"] != null)
                culture = Session["culture"] as string;
            if (string.IsNullOrEmpty(culture))
                culture = "nl-NL";
            if (culture.Length < 4)
            {
                switch (culture)
                {
                    case "nl":
                        culture += "-NL";
                        break;
                    case "nl-":
                        culture += "NL";
                        break;
                    case "NL":
                        culture = "nl-" + culture;
                        break;
                    case "-NL":
                        culture = "nl" + culture;
                        break;
                    case "en":
                        culture = "-US";
                        break;
                    case "en-":
                        culture += "US";
                        break;
                    case "US":
                        culture = "en-" + culture;
                        break;
                    case "-US":
                        culture = "en" + culture;
                        break;
                }
            }
            country = culture.Substring(3);
            currency = Util.GetCurrencyIsoNameByCountryIso2(country);

            string res = Request.Params["res"] ?? string.Empty;
            string status = Request.Params["Status"] ?? string.Empty;
            string statusCode = Request.Params["StatusCode"] ?? string.Empty;
            string merchant = Request.Params["Merchant"] ?? string.Empty;
            string orderId = Request.Params["OrderID"] ?? string.Empty;
            string paymentId = Request.Params["PaymentID"] ?? string.Empty;
            string reference = Request.Params["Reference"] ?? string.Empty;
            string transid = Request.Params["TransactionID"] ?? string.Empty;
            string paymentMethod = Request.Params["PaymentMethod"] ?? string.Empty;
            if (!string.IsNullOrEmpty(statusCode))
                statusCode = Uri.UnescapeDataString(statusCode);
            using (Database db = new MySqlDatabase())
            {
                ProductInfo pi = db.GetProductById(prodId);
                db.UpdateTransaction(orderId, res, status, statusCode, merchant, paymentId, reference,
                                 transid, paymentMethod, amount, pi, currency, country);

                StringBuilder sb = new StringBuilder();
                sb.Append("<div>");
                sb.AppendFormat("<p><strong>{0}</strong></p>", Resources.Resource.TransactionFailed);
                sb.AppendFormat("<p>{0}</p>", statusCode);
                sb.Append("</div>");
                ResultLiteral.Text = sb.ToString();
            }

            sendMail(orderId, Resources.Resource.CreditPurchaseFailureSubject, Resources.Resource.CreditPurchaseFailureBody);
        }

        private void ProcessPostback()
        {

        }

        private void sendMail(string orderId, string msgSubject, string msgbody)
        {
            try
            {
                StringBuilder body = new StringBuilder();

                using (TextReader rdr = new StreamReader(Server.MapPath(msgbody)))
                {
                    string text = rdr.ReadToEnd();

                    text = text.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                    text = text.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                    text = text.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                    text = text.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                    text = text.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                    text = text.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                    text = text.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                    text = text.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                    text = text.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                    text = text.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                    text = text.Replace("{%name%}", name);

                    body.Append(text);
                }

                Util.SendEmail(new string[] { email }, "noreply@trackprotect.com", orderId + " " + msgSubject, body.ToString(), null, 0);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "PayResult SEndMail()");
            }
        }
    }
}

