using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Media;
using QuickTimePlayer;

namespace TrackProtect.Member
{
    public partial class MemberTracks : BasePage
    {
        /// <summary>
        /// WebMethod - Get the PDF path from javascript and assign to session variable
        /// </summary>
        /// <param name="strpath"></param> 
        [System.Web.Services.WebMethod]
        public static string SetDownloadPath(string strpath)
        {
            MemberTracks obj = new MemberTracks();
            obj.Session["strDwnPath"] = strpath;
            return strpath;
        }

        //[System.Web.Services.WebMethod]
        //public static void PlayMusicMethod(string musicPath)
        //{          
        //    musicPath = "http://localhost:4508/trackprotect_repos/repository/Genda Phool.mp3";
        //    MemberTracks obj = new MemberTracks();

        //    if (obj.Session["Path"] != null)
        //    {
        //        if (musicPath != Convert.ToString(obj.Session["Path"]))
        //            obj.Session["Path"] = musicPath;//musicPath;
        //        else
        //            obj.Session["Path"] = "";
        //    }
        //    else
        //    {
        //        obj.Session["Path"] = musicPath;
        //    }
        //}

        [System.Web.Services.WebMethod]
        public static void SetMusicFilePath(string musicFile)
        {
            HttpContext.Current.Session["MusicFilePath"] = musicFile;
        }

        protected void Page_PreRender(Object o, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                bool isNotExpired = true;

                Facebook.AuthenticationService authService = new Facebook.AuthenticationService();

                Facebook.Me me;
                string accessToken = string.Empty;

                if (authService.TryAuthenticate(out me, out accessToken))
                {
                    isNotExpired = true;
                }
                else
                {
                    db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);
                    db.UpdateFacebookID(ci.ClientId);

                    isNotExpired = false;
                }

                if (!string.IsNullOrEmpty(ci.SoundCloudId))
                    SoundcloudItag.Attributes.Add("class", "soundcloud");
                else
                    SoundcloudItag.Attributes.Add("class", "soundcloud disabled");

                if (isNotExpired)
                    FacebookHeading.Attributes.Add("class", "social facebook");
                else
                    FacebookHeading.Attributes.Add("class", "social facebook disabled");

                if (!string.IsNullOrEmpty(ci.TwitterId))
                    TwitterHeading.Attributes.Add("class", "social twitter");
                else
                    TwitterHeading.Attributes.Add("class", "social twitter disabled");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "tracks";
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                CreditsLiteral.Text = Util.GetUserCredits(Util.UserId).ToString();
                ProtectedLiteral.Text = protectedTracks.ToString();
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }

            FillRegistrationGrid();

            //------- Highlight the selected lang button ------- !

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript
                    (this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript
                    (this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
            }

            ClientScript.RegisterStartupScript
                (this.GetType(), "HighLightMenu", "HighLightMenu('" + "Menu2" + "');", true);
        }

        private void FillRegistrationGrid()
        {
            if (HttpContext.Current.Session["userid"] == null)
                return;

            int credits = Util.GetUserCredits(Util.UserId);

            long userid = Util.UserId;
            using (Database db = new MySqlDatabase())
            {
                DataSet registry = db.GetRegister(userid);

                if (registry.Tables["Table"].Rows.Count > 0)
                {
                    NotifyTpProtectTrackDiv.Visible = false;
                    DataView dataView = new DataView(registry.Tables["Table"]);
                    // RegistrationGrid.DataSource = dataView;
                    //RegistrationGrid.DataBind();
                    dlMyTracks.DataSource = dataView;
                    dlMyTracks.DataBind();
                }
                else
                {
                    NotifyTpProtectTrackDiv.Visible = true;
                    dlMyTracks.Visible = false;
                }
            }
        }

        //public void RegistrationGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        DataRowView dataItem = e.Row.DataItem as DataRowView;
        //        string txtCer = dataItem[1] as string;
        //        string txtDoc = Path.ChangeExtension(txtCer, ".pdf");
        //        string navUrlCer = GetUserFilepath(txtCer);
        //        string navUrlDoc = GetUserFilepath(txtDoc);
        //        /*
        //        ImageButton btn = e.Row.Cells[4].FindControl("downloadButton") as ImageButton;
        //        btn.CommandArgument = navUrl;
        //        */

        //        HyperLink hl = e.Row.Cells[0].FindControl("downloadButton") as HyperLink;
        //        if (!string.IsNullOrEmpty(navUrlCer) && File.Exists(navUrlCer))
        //        {
        //            //hl.Text = Path.GetFileNameWithoutExtension(txtCer);
        //            hl.ImageUrl = "~/Images/certificates-icon.png";
        //            hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", navUrlCer);
        //        }
        //        else
        //        {
        //            hl.Text = Resources.Resource.NoFile;
        //            hl.NavigateUrl = string.Empty;
        //        }

        //        hl = e.Row.Cells[0].FindControl("downloadDocument") as HyperLink;
        //        if (!string.IsNullOrEmpty(navUrlDoc) && File.Exists(navUrlDoc))
        //        {
        //            //hl.Text = Path.GetFileNameWithoutExtension(txtDoc);
        //            hl.ImageUrl = "~/Images/pdf-icon.png";
        //            hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", navUrlDoc);
        //        }
        //        else
        //        {
        //            hl.Text = Resources.Resource.NoFile;
        //            hl.NavigateUrl = string.Empty;
        //        }
        //    }
        //}

        public string GetUserFilepath(string filename)
        {
            long userid = Util.UserId;
            string pwd = string.Empty;
            if (HttpContext.Current.Session["access"] != null)
                pwd = HttpContext.Current.Session["access"] as string;

            string repositoryPath = null;
            using (Database db = new MySqlDatabase())
            {
                repositoryPath = db.GetUserDocumentPath(userid, pwd);
            }
            string ret = Path.Combine(repositoryPath, filename);
            if (File.Exists(ret))
                return ret;

            return string.Empty;
        }

        public void DownloadButton_Click(Object sender, CommandEventArgs e)
        {
            string filepath = e.CommandArgument as string;
            string filename = Path.GetFileName(filepath);
            string url = ConfigurationManager.AppSettings["SiteNavigationLink"] + filename;
            WebClient client = new System.Net.WebClient();
            client.DownloadFile(url, filename);
        }

        public string Cutdesc(string s)
        {
            if (s.Length > 16)
            {
                s = s.Substring(0, 16);
                s = s + "..";
            }
            return s;
        }

        protected void dlMyTracks_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            string txtCer = ((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[1].ToString();
            string txtDoc = Path.ChangeExtension(txtCer, ".pdf");
            string navUrlCer = GetUserFilepath(txtCer).Replace("\\", "/");
            string navUrlDoc = GetUserFilepath(txtDoc).Replace("\\", "/");

            HyperLink hl = e.Item.FindControl("downloadButton") as HyperLink;

            try
            {
                if (!string.IsNullOrEmpty(navUrlCer))
                    if (File.Exists(navUrlCer))
                    {
                        //hl.Text = Path.GetFileNameWithoutExtension(txtCer);
                        // hl.ImageUrl = "~/Images/certificates-icon.png";
                        //hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(navUrlCer));
                        hl.Attributes.Add("onclick", "InitializeRequest('" + navUrlCer + "');");
                    }
                    else
                    {
                        //hl.Text = Resources.Resource.NoFile;
                        hl.NavigateUrl = string.Empty;
                    }
            }
            catch { }

            hl = e.Item.FindControl("downloadDocument") as HyperLink;

            try
            {
                if (!string.IsNullOrEmpty(navUrlDoc))
                    if (File.Exists(navUrlDoc))
                    {
                        //hl.Text = Path.GetFileNameWithoutExtension(txtDoc);
                        //  hl.ImageUrl = "~/Images/pdf-icon.png";
                        //hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(navUrlDoc));
                        hl.Attributes.Add("onclick", "InitializeRequest('" + navUrlDoc + "');");
                    }
                    else
                    {
                        //hl.Text = Resources.Resource.NoFile;
                        hl.NavigateUrl = string.Empty;
                    }
            }
            catch { }

            hl = e.Item.FindControl("MusicLink") as HyperLink;

            try
            {
                HiddenField hf = e.Item.FindControl("regIDHF") as HiddenField;
                using (Database db = new MySqlDatabase())
                    hl.Attributes.Add("onclick", "AudioWindow('" + Convert.ToInt32(hf.Value) + "');"); ;
                //hl.Attributes.Add("onclick", "PlayMusic('" + db.GetMusicPathByRegID(Convert.ToInt32(hf.Value)).Replace(@"\", "/") + "');"); ;
            }
            catch { }

            //Session["Path1"] = soundPath(((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[0].ToString());



            //hl = e.Item.FindControl("PlayMusic") as HyperLink;

            //hl.NavigateUrl = "javascript:AudioWindow('" + Convert.ToInt32(hf.Value) + "')";

            //hl.NavigateUrl = "javascript:AudioWindow()";
        }

        //event for the control from inside the datalist
        protected void dlMyTracks_ItemCommand(object source, DataListCommandEventArgs e)
        {
            Session["songId"] = Convert.ToInt32(e.CommandArgument);

            ClientScript.RegisterStartupScript(this.GetType(), "AudioWindow", "javascript:AudioWindow();", true);
        }

        public string soundPath(string id)
        {
            string MusicFile = string.Empty;

            using (Database db = new MySqlDatabase())
            {
                MusicFile = db.GetMusicPathByRegID(Convert.ToInt32(id));
            }

            return ConfigurationManager.AppSettings["SiteNavigationLink"] + MusicFile.Replace("\\", "/");
        }
    }
}