using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
	public partial class ErrorDialog : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string title = string.Empty;
			string message = string.Empty;

			if (Request.Params["title"] != null)
				title = Request.Params["title"];
			if (Request.Params["message"] != null)
				message = Request.Params["message"];
			ErrorTitle.Text = Uri.UnescapeDataString(title);
			ErrorMessage.Text = Uri.UnescapeDataString(message);
		}
	}
}