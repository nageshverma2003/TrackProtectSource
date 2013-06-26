using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class HowItWorks : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(HowItWorksInc, Resources.Resource.incHowItWorks);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
			}
        }
    }
}
