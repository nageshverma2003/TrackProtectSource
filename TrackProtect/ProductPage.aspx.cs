using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class ProductPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["culture"] == null)
                {
                    string culture = Request.UserLanguages[0];
                    Session["culture"] = culture;
                    Culture = culture;
                    UICulture = culture;

                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                }
            }

            if (!User.Identity.IsAuthenticated)
            {
                ClearUserSession();
            }
            else
            {
                if (Session["userid"] != null &&
                    Session["useruid"] != null &&
                    Session["access"] != null &&
                    Session["subscriptiontype"] != null)
                {
                    Response.Redirect("~/Member/MemberHome.aspx", false);
                }
            }

            IncludePage(DefaultInc, Resources.Resource.incDefault);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

            if (!IsPostBack)
            {
                Config cfg = new Config();
                cfg.Load(Server.MapPath("~/Config/trackprotect.config"));

                using (Database db = new MySqlDatabase())
                {
                    RegisterStarterButton.CommandName = "-1";
                    RegisterMediumButton.CommandName = "-1";
                    RegisterProButton.CommandName = "-1";
                    RegisterEnterpriseButton.CommandName = "-1";

                    ProductInfoList pil = db.GetProducts();
                    foreach (ProductInfo pi in pil)
                    {
                        switch (pi.ProductId)
                        {
                            case 1: RegisterStarterButton.CommandName = pi.ProductId.ToString(); break;
                            case 2: RegisterMediumButton.CommandName = pi.ProductId.ToString(); break;
                            case 3: RegisterProButton.CommandName = pi.ProductId.ToString(); break;
                            case 4: RegisterEnterpriseButton.CommandName = pi.ProductId.ToString(); break;
                        }
                    }
                }
            }
            else { }
        }

        private void ClearUserSession()
        {
            if (Session["userid"] != null)
                Session.Remove("userid");
            if (Session["useruid"] != null)
                Session.Remove("useruid");
            if (Session["access"] != null)
                Session.Remove("access");
            if (Session["subscriptiontype"] != null)
                Session.Remove("subscriptiontype");
        }

        public void CommandButton_Click(Object sender, CommandEventArgs e)
        {
            HttpContext.Current.Session.Remove("subscriptiontype");
            int subscriptiontype = -1;
            if (!string.IsNullOrEmpty(e.CommandName))
            {
                int tmp = 0;
                if (int.TryParse(e.CommandName, out tmp))
                    subscriptiontype = tmp;
            }
            HttpContext.Current.Session["subscriptiontype"] = subscriptiontype;
            Response.Redirect(string.Format("~/Member/ShowProduct.aspx?pid={0}", HttpContext.Current.Session["subscriptiontype"]), false);
        }
    }
}