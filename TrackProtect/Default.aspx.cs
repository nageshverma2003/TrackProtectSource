using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Configuration;


namespace TrackProtect
{
    public partial class Default : BasePage
    {
        #region Variables ------- !

        readonly string[] _desc = new string[] { "starter", "medium", "pro", "bulk" };

        #endregion


        #region events ------- !

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session != null)
            {
                // Store essential session information temporarily
                string culture = Session["culture"] as string ?? "nl-NL";
                // Restore the essential session information
                Session.Clear();
                Session.RemoveAll();

                Session["culture"] = culture;
            }

            if (!IsPostBack)
            {
                long userId = Util.UserId;
                Session["bodyid"] = "user-home";
                #region Generating product information and binding their price ------- !

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
                    string price = string.Empty;
                    string link = string.Empty;
                    int i = 0;
                    foreach (ProductInfo prod in pil)
                    {
                        //Added by Nagesh

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

                        if (ppil[0].Price > 0m)
                        {
                            // Normal products, just process them
                            //link = string.Format(Session["userid"] != null ?
                            //    "/Member/BuyProduct.aspx?pid={0}&country={1}&price={2}" :
                            //    "/Account/Login.aspx?pid={0}&country={1}&price={2}",
                            //    prod.ProductId, countryIso2, ppil[0].Price);

                            link = string.Format(Session["userid"] != null ?
                                "/Member/BuyProduct.aspx?pid={0}&country={1}&price={2}" :
                                "/Account/Login.aspx?pid={0}&country={1}&price={2}",
                                prod.ProductId, "NL", ppil[0].Price);

                        }
                        else
                        {
                            // Special products, check the description field to
                            // find out more about the product
                            switch (prod.Extra.ToLower())
                            {
                                case "subscription":
                                    link = string.Format(Session["userid"] != null ?
                                        "/Member/Subscription.aspx?pid={0}&country={1}&price={2}" :
                                        "/Account/Login.aspx?pid={0}&country={1}&price={2}&sub=1"
                                        );
                                    break;

                                default:
                                    link = string.Format(Session["userid"] != null ?
                                        "/Member/Quotation.aspx?pid={0}&country={1}&price={2}" :
                                        "/Account/Login.aspx?pid={0}&country={1}&price={2}&sub=0",
                                        prod.ProductId, countryIso2, 0);

                                    break;
                            }
                        }
                        //End Here
                        string desc = db.GetProduct_Desc_Price(prod.ProductId, culture);

                        Product _product = new Product();
                        _product.ProductPlan = prod.ProductPlan;
                        _product.Credits = Convert.ToString(prod.Credits);
                        _product.ProductDesc = desc.Split('#')[0];
                        _product.ProductPrice = desc.Split('#')[1];
                        _product.ProductId = prod.ProductId;
                        listProductInformation.Add(_product);
                        string planCss = "plans";
                        if (i == 3)
                        {
                            planCss = "plans managed";
                        }
                        ltrProducts.Text = ltrProducts.Text + "<li> <div class='" + planCss + "'><div class='icon-img'><i class='icon-logo'></i></div><h2 class='plan-title'> " + _product.ProductPlan + "<span class='number'>" + _product.Credits + "</span></h2><p class='description'>" + _product.ProductDesc + "</p><div class='row'><div class='small-6 columns'> <h2 class='price'>" + price + "</h2></div> <div class='small-6 columns'><a href=" + link + " class='button'>" + Resources.Resource.BuyNow + "</a></div></div><p class='footnote'> " + _product.ProductPrice + "</p> </div></li>";
                        i++;
                    }
                }

                #endregion

                #region Setting en/nl contents ------- !

                //ProductList.DataSource = listProductInformation;
                //ProductList.DataBind();

                if (Session["culture"] == null)
                {
                    string _culture = "nl-NL";

                    string lang = "en";
                    if (Request.UserLanguages != null)
                        lang = Request.UserLanguages[0] ?? "en";

                    lang = lang.Split(';')[0].Trim();
                    switch (lang)
                    {
                        case "en":
                            _culture = "en-US";
                            break;
                        case "nl":
                            _culture = "nl-NL";
                            break;
                    }

                    Session["culture"] = _culture;
                    Culture = _culture;
                    UICulture = _culture;

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(_culture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(_culture);
                }

                IncludePage(InfoGraphicLiteral, Resources.Resource.InfoGraphicSection);
                IncludePage(UpsLiteral, Resources.Resource.UpsSection);
                IncludePage(AboutLiteral, Resources.Resource.AboutSection);
                IncludePage(IntroLiteral, Resources.Resource.IntroSection);
                IncludePage(FooterLiteral, Resources.Resource.FooterSection);
                IncludePage(FAQLiteral, Resources.Resource.FAQTop10);
                IncludePage(newsLiteral, Resources.Resource.NewsLiteral);

                #endregion
                FormsAuthentication.SignOut();
            }

            //------- Highlight the selected lang button ------- !

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "btnNLSmall" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "btnENSmall" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "btnENSmall" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "btnNLSmall" + "');", true);
            }
        }

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            Login Login1 = HeadLoginView.FindControl("Login1") as Login;
            if (Login1 == null)
                return;

            if (Login1.UserName.Contains("@"))
            {
                string username = Membership.GetUserNameByEmail(Login1.UserName);
                if (username != null)
                {
                    if (Membership.ValidateUser(username, Login1.Password))
                    {
                        Login1.UserName = username;
                        e.Authenticated = true;
                    }
                    else
                    {
                        e.Authenticated = false;
                    }
                }
            }
            else
            {
                e.Authenticated = Membership.ValidateUser(Login1.UserName, Login1.Password);
            }
        }

        protected void Send_Click(object sender, EventArgs e)
        {
            Regex re = new Regex(@"^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$");

            if (string.IsNullOrEmpty(ContactText.Text))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter your name." + "');", true);
            if (string.IsNullOrEmpty(EmailText.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter your email address." + "');", true);
            else if (string.IsNullOrEmpty(MsgText.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Enter some text in message." + "');", true);
            else if (!re.IsMatch(EmailText.Text.Trim()))
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "Invalid email address." + "');", true);
            else
            {
                StringBuilder body = new StringBuilder();

                using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.SupportEmailTpl)))
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

                    text = text.Replace("{%name%}", ContactText.Text.Trim());
                    text = text.Replace("{%email%}", EmailText.Text.Trim());
                    text = text.Replace("{%message%}", MsgText.Text.Trim());

                    body.Append(text);
                }

                bool mailSent = true;

                try
                {
                    Util.SendEmail(new string[] { ConfigurationManager.AppSettings["SupportEmail"] }, EmailText.Text.Trim(), "Support Message", body.ToString(), null, 0);
                }
                catch
                {
                    mailSent = false;
                }

                body.Clear();

                if (mailSent)
                {
                    using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.InquiryAutoReplyBody)))
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

                        text = text.Replace("{%name%}", ContactText.Text.Trim());

                        body.Append(text);
                    }

                    try
                    {
                        Util.SendEmail
                            (new string[] { EmailText.Text.Trim() }, "noreply@trackprotect.com", Resources.Resource.InquiryAutoReplySubject, body.ToString(), null, 0);
                    }
                    catch
                    {

                    }
                }

                Response.Redirect("ThankYou.aspx?id=3");

                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "You message has been sent to our support team." + "');", true);
            }
        }

        #endregion


        #region Methods ------- !

        private void setCulture()
        {
            if (Session != null)
            {
                // Store essential session information temporarily
                string culture = Session["culture"] as string ?? "nl-NL";
                string captchaGuid = Session["captcha.guid"] as string;
                string captchaCode = null;
                if (!string.IsNullOrEmpty(captchaGuid))
                    captchaCode = Session[captchaGuid] as string;

                Session.Clear();
                FormsAuthentication.SignOut();

                // Restore the essential session information
                Session["culture"] = culture;
                if (!string.IsNullOrEmpty(captchaGuid))
                {
                    Session["captcha.guid"] = captchaGuid;
                    if (!string.IsNullOrEmpty(captchaCode))
                        Session[captchaGuid] = captchaCode;
                }
            }
        }

        #endregion
    }
}