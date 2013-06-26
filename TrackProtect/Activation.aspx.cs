using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class Activation : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(ActivateInc, Resources.Resource.incActivate);
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement);

            if (!IsPostBack)
            {
                if (Request.Params["uid"] != null)
                {
                    string uid = Request.Params["uid"];

                    using (Database db = new MySqlDatabase())
                    {
                        // Check the UID against the database
                        long userId = db.GetUserIdByUid(uid);
                        db.ActivateUser(userId);
                    }
                }
            }
        }
    }
}