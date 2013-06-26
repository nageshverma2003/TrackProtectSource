using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace TrackProtect
{
	public partial class SelectProduct : BasePage
	{
	    readonly string[] _desc = new string[] { "starter", "medium", "pro", "bulk" };
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(SelectProductInc, Resources.Resource.incSelectProduct);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

			long userId = Util.UserId;

            divSignUp.Visible = (Session["userid"] == null);

			if (!IsPostBack)
			{
				using (Database db = new MySqlDatabase())
				{
					ClientInfo ci = null;
					if (userId > -1)
						ci = db.GetClientInfo(userId);
					
					string currency = "EUR";
					string countryIso2	= "NL";
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
					ProductInfoList pil = db.GetProducts();
					int i = 0;
					foreach (ProductInfo prod in pil)
					{
						ProductPriceInfoList ppil = db.GetProductPrices(prod.ProductId, culture);
						TableRow row = new TableRow();
						row.VerticalAlign = VerticalAlign.Top;
						
						TableCell cell = new TableCell();
						if (i < 4)
						{
							Image img = new Image();
							img.ImageUrl = string.Format(Resources.Resource.imgVaultFmt, _desc[i]);
							cell.Controls.Add(img);
							++i;
						}
						else
						{
							cell.Text = "&nbsp;";
						}
						row.Cells.Add(cell);

					    string desc = db.GetProductTitle(prod.ProductId, culture);
						cell = new TableCell();
						Literal lit = new Literal();
						lit.Text = "<div style=\"margin-left:8px;margin-right:8px;\">" + desc + "</div>";
						cell.Controls.Add(lit);
						row.Cells.Add(cell);
						
						cell = new TableCell();
						cell.Width = Unit.Pixel(50);
						cell.Font.Bold = true;
						cell.HorizontalAlign = HorizontalAlign.Center;
                        if (ppil[0].Price > 0m)
                        {
                            cell.Text = string.Format(
                                currencyFmt,
                                Util.GetCurrencySymbolByCountryIso2("NL"),
                                ppil[0].Price);
                        }
                        else
                        {
                            cell.Text = Resources.Resource.Quotation;
                        }
					    row.Cells.Add(cell);
						
						cell = new TableCell();
                        HyperLink hl = new HyperLink();
                        hl.CssClass = "linkBuy";
                        if (string.IsNullOrEmpty(countryIso2))
                            countryIso2 = "NL";
                        if (ppil[0].Price > 0m)
                        {
                            // Normal products, just process them
                            hl.NavigateUrl = string.Format(Session["userid"] != null ? 
                                "~/Member/BuyProduct.aspx?pid={0}&country={1}&price={2}" : 
                                "~/Account/Login.aspx?pid={0}&country={1}&price={2}", 
                                prod.ProductId, countryIso2, ppil[0].Price);
                            hl.ImageUrl = Resources.Resource.imgBuyCredits;
                        }
                        else
                        {
                            // Special products, check the description field to
                            // find out more about the product
                            switch (prod.Extra.ToLower())
                            {
                            case "subscription":
                                hl.NavigateUrl = string.Format(Session["userid"] != null ?
                                    "~/Member/Subscription.aspx?pid={0}&country={1}&price={2}" :
                                    "~/Account/Login.aspx?pid={0}&country={1}&price={2}&sub=1"
                                    );                                
                                break;

                            default:
                                hl.NavigateUrl = string.Format(Session["userid"] != null ?
                                    "~/Member/Quotation.aspx?pid={0}&country={1}&price={2}" :
                                    "~/Account/Login.aspx?pid={0}&country={1}&price={2}&sub=0", 
                                    prod.ProductId, countryIso2, 0);
                                hl.ImageUrl = Resources.Resource.imgQuotation;
                                break;
                            }
                        }
                        cell.Controls.Add(hl);
					    row.Cells.Add (cell);
						
						ProductTable.Rows.Add (row);
					}
				}
			}
			else
			{
			}
        }
	}
}

