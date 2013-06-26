using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
	public partial class Acct : System.Web.UI.MasterPage
	{
		protected void Page_Load(object sender, EventArgs e)
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

			ShowLoggedOffView();
		}

		protected void SelectLanguage(object sender, CommandEventArgs e)
		{
			Session["culture"] = e.CommandArgument as string;
			Response.Redirect(Request.RawUrl, false);
		}

		private void NavigationMenuReinit()
		{
			MenuTable.Rows.Clear();
			TableRow row = new TableRow();
			row.ID = "MenuTableRow1";
			MenuTable.Rows.Add(row);
		}

		private void ShowLoggedOffView()
		{
			GoHome.NavigateUrl = "~/Default.aspx";

			NavigationMenuReinit();
			AddMenuItem(Resources.Resource.Home, "", "", "~/Default.aspx", "menuItem", true, true);
			AddMenuItem(Resources.Resource.HowItWorks, "", "", "~/HowItWorks.aspx", "menuItem", true, true);
			AddMenuItem(Resources.Resource.Products, "", "", "~/SelectProduct.aspx", "menuItem", true, true);
			AddMenuItem(Resources.Resource.AboutUs, "", "", "~/About.aspx", "menuItem", true, true);
			AddMenuItem(Resources.Resource.FAQ, "", "", "~/FAQ.aspx", "menuItem", true, true);
			AddMenuItem(Resources.Resource.Contact, "", "", "~/Contact.aspx", "menuItem", false, true);
		}

		private void AddMenuItem(string name, string value, string imageUrl, string navigateUrl, string cssClass, bool separator, bool enabled)
		{
			int row = MenuTable.Rows[0].Cells.Count + 1;
			TableCell cell = new TableCell();
			cell.CssClass = cssClass;

			Panel panel = new Panel();
			panel.ID = string.Format("div{0}", row);
			panel.CssClass = cssClass;
			panel.Style[HtmlTextWriterStyle.MarginLeft] = "24px";
			panel.Style[HtmlTextWriterStyle.MarginRight] = "24px";

			HyperLink hl = new HyperLink();
			hl.ID = string.Format("MenuItem{0}", row);
			if (!string.IsNullOrEmpty(cssClass))
				hl.CssClass = cssClass;
			if (!string.IsNullOrEmpty(imageUrl))
			{
				hl.ImageUrl = imageUrl;
			}
			else
			{
				if (!string.IsNullOrEmpty(name))
					hl.Text = name;
			}
			hl.NavigateUrl = navigateUrl;
			hl.Enabled = enabled;
			panel.Controls.Add(hl);

			cell.Controls.Add(panel);

			MenuTable.Rows[0].Cells.Add(cell);

			if (separator)
			{
				cell = new TableCell();
				cell.Width = Unit.Pixel(2);

				Image image = new Image();
				image.ID = string.Format("MenuSep{0}", row);
				image.ImageUrl = "~/Images/Nav_divider.gif";
				cell.Controls.Add(image);

				MenuTable.Rows[0].Cells.Add(cell);
			}
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
			else // Standard username & password login
			{
				e.Authenticated = Membership.ValidateUser(Login1.UserName, Login1.Password);
			}
		}
	}
}