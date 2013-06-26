using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
	public partial class ErrorPage : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			IncludePage(ProtectInc, Resources.Resource.incProtect);
			IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

			string title			= Request.Params["title"] ?? string.Empty;
			string message		= Request.Params["message"] ?? string.Empty;
			string returnurl	= Request.Params["returnurl"] ?? string.Empty;

			ErrorTitle.Text = Uri.UnescapeDataString(title);
			ErrorMessage.Text = Uri.UnescapeDataString(message);

			if (!string.IsNullOrEmpty(returnurl))
			{
				ReturnLink.NavigateUrl = Uri.UnescapeDataString(returnurl);
			}

			if (!IsPostBack)
			{
			}
		}
	}
}