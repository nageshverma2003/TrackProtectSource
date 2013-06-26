using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class About : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(AboutInc, Resources.Resource.incAbout);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
 			}
        }
    }
}
