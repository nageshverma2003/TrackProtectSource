using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace TrackProtect
{
    public partial class ShowProduct : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string culture = "nl-NL";
            if (Session["culture"] != null)
                culture = Session["culture"] as string;

            IncludePage(ShowProductInc, Resources.Resource.incShowProduct);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

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
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            long userid = Util.UserId;

            if (!IsPostBack)
            {
                DescriptionLiteral.Text = string.Empty;
                int pid = 0;
                if (Request.Params["pid"] != null)
                {
                    string tmp = Request.Params["pid"];
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        int iTmp;
                        if (int.TryParse(tmp, out iTmp))
                            pid = iTmp;
                    }
                }
                if (pid <= 0)
                {
                    DescriptionLiteral.Text = Resources.Resource.NoProductDescription;
                }
                else
                {
                    string[] _desc = new string[] { "starter", "medium", "pro", "bulk" };

                    DescriptionImage.ImageUrl = string.Format(Resources.Resource.imgVaultFmt, _desc[pid - 1]);
                    using (Database db = new MySqlDatabase())
                    {
                        ProductInfoList pil = db.GetProducts();
                        foreach (ProductInfo pi in pil)
                        {
                            if (pi.ProductId == pid)
                            {
                                BuyProductButton.Visible = true;
                                BuyProductButton.CommandName = pid.ToString();
                                /*
                                if (User.Identity.IsAuthenticated)
                                    BuyProductButton.Visible = true;
                                */

                                StringBuilder pricingInfo = new StringBuilder();
                                string iso2Country = "NL";
                                string isoCurrency = "EUR";
                                if (userid > -1)
                                {
                                    ClientInfo ci = db.GetClientInfo(userid);
                                    if (ci != null)
                                    {
                                        iso2Country = Util.GetCountryIso2(ci.Country);
                                        isoCurrency = Util.GetCurrencyIsoNameByCountryIso2(iso2Country);
                                    }
                                }
                                ProductPriceInfoList ppil = db.GetProductPrices(pi.ProductId, culture);
                                if (ppil.Count > 0)
                                {
                                    pricingInfo.Append("<table cellpadding='4'>");
                                    foreach (ProductPriceInfo ppi in ppil)
                                    {
                                        if (ppi.Price == 0m)
                                        {
                                            pricingInfo.AppendFormat(
                                                "<tr><td><span class='priceInfo'>{0}</span></td></tr>", Resources.Resource.RequestQuotation);
                                        }
                                        else
                                        {
                                            string curr = Util.GetCurrencySymbolByCountryIso2("NL");
                                            string currFmt = Util.GetCurrencyFormatByCountryIso2("NL");
                                            pricingInfo.AppendFormat("<tr><td><span class='priceInfo'>{0}</span></td><td><span class='priceInfo'>", Resources.Resource.Price);
                                            pricingInfo.AppendFormat(currFmt, curr, ppi.Price);
                                            pricingInfo.Append("</span></td></tr>");
                                        }
                                    }
                                    pricingInfo.Append("</table>");
                                }


                                TitleLiteral.Text = db.GetProductTitle(pi.ProductId, culture);

                                string desc = db.GetProductDescription(pi.ProductId, culture);
                                if (string.IsNullOrEmpty(desc))
                                    desc = pi.Description;

                                if (string.IsNullOrEmpty(desc))
                                    DescriptionLiteral.Text = "<p><h1>" + pi.Name + "</h1></p>" + pricingInfo.ToString();
                                else
                                    DescriptionLiteral.Text = desc + pricingInfo.ToString();
                            }
                        }
                    }
                }
            }
            else
            {
            }
        }

        protected void BuyProductButton_Command(object sender, CommandEventArgs e)
        {
            long userId = Util.UserId;
            if (!string.IsNullOrEmpty(e.CommandName))
            {
                int pid = Convert.ToInt32(e.CommandName);
                if (userId > 0)
                {
                    Response.Redirect(string.Format("~/Member/BuyProduct.aspx?pid={0}", pid), false);
                }
                else
                {
                    Response.Redirect(string.Format("~/Account/Register.aspx?pid={0}", pid), false);
                }
            }
        }
    }
}

