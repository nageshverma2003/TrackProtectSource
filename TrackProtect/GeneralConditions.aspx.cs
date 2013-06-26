using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace TrackProtect
{
	public partial class GeneralConditions : BasePage
	{
        protected void Page_Load(object sender, EventArgs e)
        {         
            IncludePage(ConditionsInc, Resources.Resource.incConditions);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
			}
		}
	}
}

