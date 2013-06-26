using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace TrackProtect
{
	public partial class Contact : BasePage
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            new BasePage();

            IncludePage(ContactInc, Resources.Resource.incContact);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
			}
		}
	}
}

