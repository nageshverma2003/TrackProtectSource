using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Account
{
    public partial class ChangePasswordSuccess : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);
        }
    }
}
