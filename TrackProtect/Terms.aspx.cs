using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace TrackProtect
{
	public partial class Terms : BasePage
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(TermsInc, Resources.Resource.incTerms);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
			}
		}
	}
}

