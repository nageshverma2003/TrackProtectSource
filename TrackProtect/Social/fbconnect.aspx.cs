using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Facebook;
using TrackProtect.Logging;

namespace TrackProtect.Social
{
    public partial class fbconnect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthenticationService authentication = new AuthenticationService();
            Me me;
            string accessToken = "";

            authentication.Authenticate(Context, out me, out accessToken);            

            if (me == null)
            {
                Logger.Instance.Write(LogLevel.Warning, "Cannot authenticate with facebook", new object[] { accessToken });
            }
        }
    }
}