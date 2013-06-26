using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Account
{
    public partial class Login : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "user-home";

            //IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            LoginView lv = Master.FindControl("HeadLoginView") as LoginView;
            if (lv != null)
            {
                //lv.Visible = false;
            }

            LoginUser.DestinationPageUrl = "~/Member/MemberHome.aspx";
            string productId = string.Empty;
            string country = string.Empty;
            string price = string.Empty;
            string addParams = string.Empty;
            if (Request.Params["pid"] != null)
            {
                productId = Request.Params["pid"];
                if (Request.Params["country"] != null)
                    country = Request.Params["country"];
                if (Request.Params["price"] != null)
                    price = Request.Params["price"];

                if (!string.IsNullOrEmpty(productId))
                {
                    LogonMessage.Text = Resources.Resource.LogonRequiredForPurchase;

                    int prodid = 0;
                    if (!int.TryParse(productId, out prodid))
                        prodid = 0;

                    ProductInfo prodInfo = null;
                    using (Database db = new MySqlDatabase())
                    {
                        prodInfo = db.GetProductById(prodid);
                    }
                    if (Request.Params["tid"] == null)
                    {
                        addParams = string.Format("pid={0}&country={1}&price={2}", productId, country, price);
                        decimal decPrice = 0m;
                        if (!decimal.TryParse(price, out decPrice))
                            decPrice = 0;
                        if (decPrice > 0m)
                        {
                            if (System.String.Compare(prodInfo.Extra, "subscription", System.StringComparison.OrdinalIgnoreCase) != 0)
                                LoginUser.DestinationPageUrl = "~/Member/BuyProduct.aspx?" + addParams;
                            else
                                LoginUser.DestinationPageUrl = "~/Member/Subscription.aspx?" + addParams;
                        }
                        else
                        {
                            LoginUser.DestinationPageUrl = "~/Member/Quotation.aspx?" + addParams;
                        }
                    }
                    else
                    {
                        addParams = string.Format("tid={0}&pid={1}", Request.Params["tid"], Request.Params["pid"]);
                        LoginUser.DestinationPageUrl = "~/Member/BuyProduct.aspx?" + addParams;
                    }
                }
            }

            string guid = string.Empty;
            string type = string.Empty;
            string email = string.Empty;
            string emailRequesting = string.Empty;
            //messageDiv.Visible = false;
            if (Request.Params["id"] != null)
            {
                guid = Request.Params["id"];
                if (Request.Params["tp"] != null)
                    type = Request.Params["tp"];
                addParams = string.Format("id={0}&tp={1}", guid, type);
                using (Database db = new MySqlDatabase())
                {
                    db.ProcessConfirmation(guid, Convert.ToInt32(type), out email, out emailRequesting);
                }
                LogonMessage.Text = string.Format(Resources.Resource.LogonEmailUnknown, email);
                //messageDiv.Visible = true;
            }
            //RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]) + "&" + addParams;

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
            }

            Control ctrlDiv = this.Master.FindControl("logoutDiv");

            ctrlDiv.Visible = false;
        }

        protected void LoginUser_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if (LoginUser.UserName.Contains("@"))
            {
                string username = Membership.GetUserNameByEmail(LoginUser.UserName);
                if (username != null)
                {
                    if (Membership.ValidateUser(username, LoginUser.Password))
                    {
                        LoginUser.UserName = username;
                        e.Authenticated = true;
                    }
                    else
                    {
                        e.Authenticated = false;
                    }
                }
            }
            else // Standard username & password login
            {
                e.Authenticated = Membership.ValidateUser(LoginUser.UserName, LoginUser.Password);
            }
        }

        protected void LoginUser_LoggedIn(object sender, EventArgs e)
        {
            Response.Redirect(LoginUser.DestinationPageUrl);
        }
    }
}
