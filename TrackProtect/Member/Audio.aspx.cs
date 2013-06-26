using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class Audio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Path"] != null)
                QTPlayer1.MOVFile = Convert.ToString(Session["Path"]); // "http://localhost:4508/trackprotect_repos/repository/Genda Phool.mp3";
            else
                QTPlayer1.Dispose();
        }
    }
}