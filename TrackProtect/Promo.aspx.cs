using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class Promo : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Session["bodyid"] = "coupon";
            IncludePage(PromoInc, Resources.Resource.incPromo);
            

            if (!IsPostBack)
            {
            }
        }
    }
}