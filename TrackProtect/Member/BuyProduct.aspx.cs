using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using TrackProtect.Logging;
using ICEPAY;

namespace TrackProtect
{
    public partial class BuyProduct : BasePage
    {
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
            Logger logger = Logger.Instance;

            IncludePage(BuyProductInc, Resources.Resource.incBuyProduct);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);


            string activeModule = string.Empty;

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
                string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }


            if (!IsPostBack)
            {
                long prodid = -1;
                long transid = -1;
                ParamsDictionary parms = new ParamsDictionary();
                string desc = "???";
                if (Request.Params["pid"] != null /* && Request.Params["tid"] == null */)
                {
                    prodid = Convert.ToInt64(Request.Params["pid"]);
                    if (prodid > -1)
                    {
                        using (Database db = new MySqlDatabase())
                        {
                            ProductInfo pi = db.GetProductById(prodid);
                            ProductPriceInfoList ppil = db.GetProductPrices(prodid);
                            decimal price = 0m;
                            foreach (ProductPriceInfo ppi in ppil)
                            {
                                if (ppi.IsoCurrency == "EUR")
                                {
                                    price = ppi.Price;
                                    break;
                                }
                            }

                            desc = pi.Name;
                            parms.Add("{%product%}", desc);
                            parms.Add("{%credits%}", pi.Credits.ToString());
                            parms.Add("{%price%}", string.Format("{0:C}", price));
                        }

                        string _priceInEuro = parms["{%price%}"];

                        if (_priceInEuro.Contains("$"))
                        {
                            parms.Remove("{%price%}");
                            _priceInEuro = _priceInEuro.Replace("$", "€").Replace(".", ",");
                            parms.Add("{%price%}", _priceInEuro);
                        }
                    }
                }

                if (Request.Params["tid"] != null /* && Request.Params["pid"] != null */)
                {
                    transid = Convert.ToInt64(Request.Params["tid"]);
                    if (transid > -1)
                    {
                        using (Database db = new MySqlDatabase())
                        {
                            Transaction transaction = db.GetQuotation(transid);
                            string statuscode = transaction.StatusCode;
                            string[] parts = statuscode.Split('(', ':', ')');
                            int credits = 0;
                            if (parts.Length >= 3)
                                credits = Convert.ToInt32(parts[2]);
                            desc = string.Format(Resources.Resource.BulkPurchase, credits, transaction.Amount);
                            parms.Add("{%product%}", desc);
                            parms.Add("{%credits%}", credits.ToString());
                        }
                    }
                }

                IncludePage(ProductInc, Resources.Resource.incBuyProductText, parms);

                //ProductLiteral.Text = string.Format(Resources.Resource.Purchase1, desc);
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
        }

        protected void SubmitButton_Command(object sender, CommandEventArgs e)
        {
            long prodid = -1;
            if (Request.Params["pid"] != null)
                prodid = Convert.ToInt64(Request.Params["pid"]);

            if (prodid > -1)
            {
                long userid = Util.UserId;

                string desc = string.Empty;
                decimal amount = 0m;
                //bool taxed = false;
                using (Database db = new MySqlDatabase())
                {
                    ProductInfo pi = db.GetProductById(prodid);
                    ProductPriceInfoList ppil = db.GetProductPrices(prodid, Request.Params["country"]);
                    desc = pi.Name;
                    amount = ppil[0].Price;
                    //taxed = true;
                }
                Session["pid"] = prodid;
                Session["amt"] = amount;

                PurchaseProduct(prodid, amount, desc);
            }
        }
    }
}

