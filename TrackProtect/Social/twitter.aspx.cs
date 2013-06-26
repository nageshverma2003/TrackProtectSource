using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Social;

namespace TrackProtect.Social
{
    public partial class twitter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // social/twitter.aspx
                // ?oauth_token=dHPqFAhPluzvt7pQSr602qTRLhXDO49PotEsqvkEcQ
                // &oauth_verifier=tNtpcSGCObsf7t7DNYiKuoCa5aVH18H4x4Up75fY
                //using (Database db = new MySqlDatabase())
                //{
                //    ClientInfo ci = db.GetClientInfo(Util.UserId);
                //    db.UpdateSocialCredential(
                //        ci.ClientId,
                //        SocialConnector.Twitter,
                //        "requesttoken.value",
                //        Request.Params["oauth_token"] as string);
                //    db.UpdateSocialCredential(ci.ClientId,
                //        SocialConnector.Twitter,
                //        "requesttoken.secret",
                //        "");
                //    db.UpdateSocialCredential(
                //        ci.ClientId,
                //        SocialConnector.Twitter,
                //        "oauthverifier",
                //        Request.Params["oauth_verifier"] as string);
                //}
            }
        }
    }
}