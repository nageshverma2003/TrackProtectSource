using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class Subscription : BasePage
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
            Session["bodyid"] = "user-home";

            if (!IsPostBack)
            {
                int ecl, vcl;
                Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);

                int tmp = 0;
                int pid = -1;
                decimal tmpDec = 0m;
                decimal unitprice = 0m;

                if (Request.Params["pid"] != null)
                {
                    if (int.TryParse(Request.Params["pid"], out tmp))
                        pid = tmp;
                    Session["subscription.productid"] = pid;
                    Session["pid"] = pid;
                }

                string desc = string.Empty;
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

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

                    string unitpriceStr = db.GetSetting("unitprice");
                    if (decimal.TryParse(unitpriceStr, out tmpDec))
                        unitprice = tmpDec;
                    Session["subscription.unitprice"] = unitprice;
                    Session["amt"] = unitprice;

                    CultureInfo culture = null;

                    if (Convert.ToString(Session["culture"]).Contains("nl"))
                    {
                        culture = new CultureInfo("nl-NL");
                    }
                    else if (Convert.ToString(Session["culture"]).Contains("en"))
                    {
                        culture = new CultureInfo("en-US");
                    }
                    else
                    {
                        culture = new CultureInfo("nl-NL");
                    }

                    desc = db.GetProductDescription(pid, culture);
                    if (vcl >= 100 || ecl >= 100)
                        desc = Resources.Resource.SubscriptionPurchase;
                }

                SubscriptionLiteral.Text = desc;
            }


            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
            }

            this.Page.Form.DefaultButton = SubscriptionSubmit.UniqueID;
        }

        protected void SubscriptionSubmit_Click(object sender, EventArgs e)
        {
            int ecl, vcl;
            Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);
            int qty = 0;
            if (int.TryParse(QuantityEdit.Text, out qty))
            {
                int prodid = (int)Session["subscription.productid"];
                decimal unitprice = (decimal)Session["subscription.unitprice"];
                decimal baseprice = 0m;
                string desc = string.Empty;
                ProductInfo pi;
                using (Database db = new MySqlDatabase())
                {
                    pi = db.GetProductById(prodid);
                    if (ecl < 100 || vcl < 100)
                    {
                        ProductPriceInfoList ppil = db.GetProductPrices(prodid, Request.Params["country"]);
                        desc = pi.Name;
                        baseprice = ppil[0].Price;
                    }
                }
                decimal totalprice = baseprice + (unitprice * qty);
                Session["amt"] = totalprice;
                if (pi.Extra.Length > 0)
                    pi.Extra += "\x01";
                pi.Extra += string.Format("{0:F2}", baseprice);

                List<ProductInfo> products = new List<ProductInfo>();

                if (baseprice > 0m)
                    products.Add(pi);

                string name = string.Format("{0} credits", qty);
                //desc = string.Format("{0} credits @ EUR {1:F2}", qty, unitprice);
                desc = string.Format("Managed Plan {0} credits", qty);
                pi = new ProductInfo(0, name, desc, qty, unitprice.ToString());
                if (baseprice == 0m)
                    pi.Extra = "subscription";
                products.Add(pi);

                PurchaseProduct(prodid, totalprice, desc, products.ToArray());
            }
        }
    }
}