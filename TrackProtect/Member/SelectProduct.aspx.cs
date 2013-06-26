using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections.Generic;

namespace TrackProtect.Member
{
    public partial class SelectProduct : BasePage
    {
        readonly string[] _desc = new string[] { "starter", "medium", "pro", "bulk" };

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
            Session["bodyid"] = "tracks";
            IncludePage(SelectProductInc, Resources.Resource.incSelectProduct);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            long userId = Util.UserId;

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
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

            getProductList(userId);

            SetProductItemLinks();

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
        }

        private void getProductList(long userId)
        {
            int ecl = 0;
            int vcl = 0;

            Util.GetUserClearanceLevels(userId, out ecl, out vcl);

            List<Product> listProductInformation = new List<Product>();
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = null;
                if (userId > -1)
                    ci = db.GetClientInfo(userId);

                string currency = "EUR";
                string countryIso2 = "NL";
                string currencyFmt = "{0} {1:N2}";
                if (ci != null)
                {
                    countryIso2 = Util.GetCountryIso2(ci.Country);
                    currency = Util.GetCurrencyIsoNameByCountryIso2("NL");
                    currencyFmt = Util.GetCurrencyFormatByCountryIso2("NL");
                }

                string culture = "en-US";
                if (Session["culture"] != null)
                    culture = Session["culture"] as string;

                if (culture.Length == 2)
                {
                    switch (culture)
                    {
                        case "nl":
                            culture += "-NL";
                            break;
                        case "en":
                            culture += "US";
                            break;
                        case "NL":
                            culture = "nl-" + culture;
                            break;
                        case "US":
                            culture = "en" + culture;
                            break;
                    }
                }

                ProductInfoList pil = db._GetProducts();

                foreach (ProductInfo prod in pil)
                {
                    String price = string.Empty;
                    ProductPriceInfoList ppil = db.GetProductPrices(prod.ProductId, culture);

                    if (ppil[0].Price > 0m)
                    {
                        price = string.Format(
                            currencyFmt,
                            Util.GetCurrencySymbolByCountryIso2("NL"),
                            ppil[0].Price);
                    }
                    else
                    {
                        price = Resources.Resource.Quotation;
                    }

                    string desc = db.GetProduct_Desc_Price(prod.ProductId, culture);

                    Product _product = new Product();

                    if (ecl >= 100 && vcl >= 100 && prod.ProductId == 4)
                        _product.ProductPlan = Resources.Resource.ManagedPlan;
                    else
                        _product.ProductPlan = prod.ProductPlan;


                    if (prod.Credits <= 0)
                        _product.Credits = string.Empty;
                    else
                        _product.Credits = Convert.ToString(prod.Credits);

                    _product.ProductDesc = desc.Split('#')[0];
                    _product.ProductPrice = desc.Split('#')[1];
                    _product.ProductId = prod.ProductId;
                    _product.Extra = prod.Extra;

                    if (ecl >= 100 && vcl >= 100 && prod.ProductId == 4)
                        _product.Price = string.Empty;
                    else
                        _product.Price = price;


                    listProductInformation.Add(_product);
                }

                if (ecl >= 100 && vcl >= 100)
                {
                    List<Product> productInfoList = new List<Product>();

                    List<Product> unmanagedProductList = new List<Product>();

                    List<Product> managedProductList = new List<Product>();

                    int count = 0;

                    foreach (Product product in listProductInformation)
                    {
                        if (count < 3)
                            unmanagedProductList.Add(product);
                        else
                            managedProductList.Add(product);

                        count++;
                    }

                    foreach (var _managedProduct in managedProductList)
                    {
                        productInfoList.Add(_managedProduct);
                    }

                    foreach (var _unmanagedProduct in unmanagedProductList)
                    {
                        productInfoList.Add(_unmanagedProduct);
                    }

                    ProductList.DataSource = productInfoList;
                }
                else
                {
                    ProductList.DataSource = listProductInformation;
                }

                ProductList.DataBind();
            }
        }

        private void SetProductItemLinks()
        {
            foreach (DataListItem li in ProductList.Items)
            {
                Label hdnProductID = (Label)li.FindControl("hdnProductID");
                Label hdnProductExtra = (Label)li.FindControl("hdnProductExtra");


                //Setting Productlist link
                string culture = "en-US";
                if (Session["culture"] != null)
                    culture = Session["culture"] as string;
                HyperLink HLBuyProduct = (HyperLink)li.FindControl("HLBuyProduct");

                ClientInfo ci = null;
                long userId = Util.UserId;

                using (Database db = new MySqlDatabase())
                {
                    if (userId > -1)
                        ci = db.GetClientInfo(userId);

                    string currency = "EUR";
                    string countryIso2 = "NL";
                    string currencyFmt = "{0} {1:N2}";
                    if (ci != null)
                    {
                        countryIso2 = Util.GetCountryIso2(ci.Country);
                        currency = Util.GetCurrencyIsoNameByCountryIso2("NL");
                        currencyFmt = Util.GetCurrencyFormatByCountryIso2("NL");
                    }

                    ProductPriceInfoList ppil = db.GetProductPrices(Convert.ToInt32(hdnProductID.Text.Trim()), culture);


                    if (ppil[0].Price > 0m)
                    {
                        string price = Convert.ToString(ppil[0].Price);

                        if (price.Contains("."))
                        {
                            price = price.Replace(".", ",");
                        }

                        // Normal products, just process them
                        if (hdnProductExtra.Text.Trim().ToLower() != "subscription")
                        {
                            //HLBuyProduct.NavigateUrl = string.Format(Session["userid"] != null ?
                            //    "~/Member/BuyProduct.aspx?pid={0}&country={1}&price={2}" :
                            //    "~/Account/Login.aspx?pid={0}&country={1}&price={2}",
                            //    Convert.ToInt32(hdnProductID.Text.Trim()), countryIso2, ppil[0].Price);

                            HLBuyProduct.NavigateUrl = string.Format(Session["userid"] != null ?
                                "~/Member/BuyProduct.aspx?pid={0}&country={1}&price={2}" :
                                "~/Account/Login.aspx?pid={0}&country={1}&price={2}",
                                Convert.ToInt32(hdnProductID.Text.Trim()), "NL", price);
                        }
                        else
                        {
                            //HLBuyProduct.NavigateUrl = string.Format(Session["userid"] != null
                            //                                                             ? "~/Member/Subscription.aspx?pid={0}&country={1}&price={2}"
                            //                                                             : "~/Account/Login.aspx?pid={0}&country={1}&price={2}",
                            //                                                        Convert.ToInt32(hdnProductID.Text.Trim()), countryIso2, ppil[0].Price);

                            HLBuyProduct.NavigateUrl = string.Format(Session["userid"] != null
                                                                                        ? "~/Member/Subscription.aspx?pid={0}&country={1}&price={2}"
                                                                                        : "~/Account/Login.aspx?pid={0}&country={1}&price={2}",
                                                                                   Convert.ToInt32(hdnProductID.Text.Trim()), "NL", price);
                        }
                    }
                    else
                    {
                        HLBuyProduct.NavigateUrl = string.Format(Session["userid"] != null ?
                            "~/Member/Subscription.aspx?pid={0}&country={1}&price={2}" :
                            "~/Account/Login.aspx?pid={0}&country={1}&price={2}",
                           Convert.ToInt32(hdnProductID.Text.Trim()), countryIso2, 0);

                    }
                }
            }
        }
    }
}

