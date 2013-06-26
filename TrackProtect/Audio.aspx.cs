using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TrackProtect
{
    public partial class Audio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //audioFilePath = Convert.ToString(Session["sound"]);

            //lblSound.Text = "Sound : " + Convert.ToString(Session["soundName"]);

            string MusicFile = string.Empty;

            using (Database db = new MySqlDatabase())
            {
                MusicFile = db.GetMusicPathByRegID(Convert.ToInt32(Request.QueryString["play"]));
                //MusicFile = db.GetMusicPathByRegID(Convert.ToInt32(Session["songId"]));

                if (MusicFile.Contains("/trackprotect_repos/repository"))
                {
                    QTPlayer1.MOVFile = ConfigurationManager.AppSettings["SiteNavigationLink"] + MusicFile.Substring(MusicFile.IndexOf("/trackprotect_repos/repository")).Replace("\\", "/");
                }
                else
                {
                    QTPlayer1.MOVFile = ConfigurationManager.AppSettings["SiteNavigationLink"] + "/trackprotect_repos/repository" + MusicFile.Replace("\\", "/");
                }
            }
        }
    }
}