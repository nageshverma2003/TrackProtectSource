using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Twitter
{
  public partial class TwitterCallback : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           // Publish.TwitterCallback();
        }
    }
  }
}