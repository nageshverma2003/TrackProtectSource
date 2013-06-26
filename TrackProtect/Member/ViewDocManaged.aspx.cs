using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Net;

namespace TrackProtect.Member
{
    public partial class ViewDocManaged : BasePage
    {
        /// <summary>
        /// WebMethod - Get the PDF path from javascript and assign to session variable
        /// </summary>
        /// <param name="strpath"></param> 
        [System.Web.Services.WebMethod]
        public static string SetDownloadPath(string strpath)
        {
            ViewDocManaged obj = new ViewDocManaged();
            obj.Session["strDwnPath"] = strpath;
            return strpath;
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
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName);// ci.GetFullName());
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

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request["id"]))
                {
                    long managedUserId = Convert.ToInt64(Request["id"]);
                    Session["managed.userid"] = managedUserId;
                    FillRegistrationGrid(Util.UserId, managedUserId);
                }
            }

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
        }

        private void FillRegistrationGrid(long managerUserId, long managedUserId)
        {
            if (managerUserId == 0 || managedUserId == 0)
                return;

            int credits = Util.GetUserCredits(Util.UserId);

            using (Database db = new MySqlDatabase())
            {
                DataSet registry = db.GetRegisterWithManager(managerUserId, managedUserId);
                DataView dataView = new DataView(registry.Tables["Table"]);
                //RegistrationGrid.DataSource = dataView;
                //RegistrationGrid.DataBind();

                dlMyTracks.DataSource = dataView;
                dlMyTracks.DataBind();
            }
        }

        //public void RegistrationGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        DataRowView dataItem = e.Row.DataItem as DataRowView;
        //        string txtCer = dataItem[1] as string;
        //        string txtDoc = Path.ChangeExtension(txtCer, ".pdf");
        //        long managedUserId = (long)Session["managed.userid"];
        //        string navUrlCer = GetUserFilepath(managedUserId, txtCer);
        //        string navUrlDoc = GetUserFilepath(managedUserId, txtDoc);


        //        /*
        //        ImageButton btn = e.Row.Cells[4].FindControl("downloadButton") as ImageButton;
        //        btn.CommandArgument = navUrl;
        //        */

        //        HyperLink hl = e.Row.Cells[3].FindControl("downloadButton") as HyperLink;
        //        if (!string.IsNullOrEmpty(navUrlCer) && File.Exists(Server.MapPath(navUrlCer)))
        //        {
        //            hl.Text = Path.GetFileNameWithoutExtension(txtCer);
        //            hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(navUrlCer));
        //        }
        //        else
        //        {
        //            hl.Text = Resources.Resource.NoFile;
        //            hl.NavigateUrl = string.Empty;
        //        }

        //        hl = e.Row.Cells[4].FindControl("downloadDocument") as HyperLink;
        //        if (!string.IsNullOrEmpty(navUrlDoc) && File.Exists(Server.MapPath(navUrlDoc)))
        //        {
        //            hl.Text = Path.GetFileNameWithoutExtension(txtDoc);
        //            hl.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(navUrlDoc));
        //        }
        //        else
        //        {
        //            hl.Text = Resources.Resource.NoFile;
        //            hl.NavigateUrl = string.Empty;
        //        }
        //    }
        //}

        public string GetUserFilepath(string filename, long userId)
        {
            //long userid = Util.UserId;
            string repositoryPath = null;
            using (Database db = new MySqlDatabase())
            {
                repositoryPath = db.GetUserDocumentPath(userId);
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
            string navUrlCer = GetUserFilepath(txtCer, Convert.ToInt64(Request["id"])).Replace("\\", "/");
            string navUrlDoc = GetUserFilepath(txtDoc, Convert.ToInt64(Request["id"])).Replace("\\", "/");
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

            Session["Path1"] = soundPath(((System.Data.DataRowView)(e.Item.DataItem)).Row.ItemArray[0].ToString());

            //HiddenField hf = e.Item.FindControl("regIDHF") as HiddenField;

            //hl = e.Item.FindControl("PlayMusic") as HyperLink;

            //hl.NavigateUrl = "javascript:AudioWindow('" + Convert.ToInt32(hf.Value) + "')";

            //hl.NavigateUrl = "javascript:AudioWindow()";
        }

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