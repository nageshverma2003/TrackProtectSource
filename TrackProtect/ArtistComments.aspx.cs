using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
	public partial class ArtistComments : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			IncludePage(ArtistCommentsInc, Resources.Resource.incArtistComments);
			IncludePage(ProtectInc, Resources.Resource.incProtect);
			IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

			if (!IsPostBack)
			{
			}
		}
	}
}