using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect
{
    public partial class Message : System.Web.UI.Page
    {
        /// <summary>
        /// The event handler for the Page Load event
        /// </summary>
        /// <param name="sender">The page that cause the Load event</param>
        /// <param name="e">The parameters associated with this Load event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = Convert.ToString(Session["MESSAGE"]);
            InsertScripts();
        }

        /// <summary>
        /// Inserts javascript scripts into the page before client side execution
        /// </summary>
        private void InsertScripts()
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("popup"))
                Page.ClientScript.RegisterClientScriptInclude("popup", "Scripts/popup.js");
        }
    }
}