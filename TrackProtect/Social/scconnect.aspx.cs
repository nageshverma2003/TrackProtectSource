using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.SoundCloud;
using TrackProtect.SoundCloud.Framework;

namespace TrackProtect.Social
{
    public partial class scconnect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SoundCloudClient sc = new SoundCloudClient
                ("b7cd21ffd93dc601e76f180705b84429", "8425feac820870f86617c8607608c30e", "http://test.trackprotect.com/Social/scconnect.aspx", false);

            if (string.IsNullOrEmpty(Request.QueryString["code"]))
            {      
                Response.Redirect(sc.GetAuthorizeUrl());
            }
            else
            {
                sc.GetAccessToken(Request.QueryString["code"]);
            }
        }
    }
}