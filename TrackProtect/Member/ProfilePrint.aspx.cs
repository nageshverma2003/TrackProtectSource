using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TrackProtect.Logging;
using System.IO;
using System.Configuration;

namespace TrackProtect.Member
{
    public partial class ProfilePrint : BasePage
    {
        #region Events -------!

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
            Session["bodyid"] = "profile";

            try
            {
                if (!IsPostBack)
                {

                    UploadFile1.Focus();
                    UserInformation();
                    FillAccountInformation();
                }

                //------- Highlight the selected lang button ------- !

                if (Convert.ToString(Session["culture"]).Contains("nl"))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);

                    ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);

                    ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
                    ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "ProfilePrint<Exception>");
            }
        }

        protected void Accept_Click(object sender, EventArgs e)
        {
            Page.Validate("FileUploadValidate");

            if (Page.IsValid)
            {
                long userid = Util.UserId;

                if (userid <= 0)
                {
                    Logger.Instance.Write(LogLevel.Warning, "No user-id known, out of context page access");
                    return;
                }

                string statusInfo1 = string.Empty;
                string statusInfo2 = string.Empty;
                using (Database db = new MySqlDatabase())
                {
                    string password = Session["access"] as string;

                    string uploadPath = string.Empty;

                    uploadPath = db.GetUserDocumentPath(userid, password);

                    uploadPath = uploadPath.Replace("\\", "/");

                    string doc1 = string.Empty, doc2 = string.Empty;
                    bool failed = false;
                    if (!failed && UploadFile1.HasFile)
                    {
                        doc1 = UploadFile1.FileName;
                        try
                        {
                            UploadFile1.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc1)));
                            statusInfo1 = string.Format("O{0}", Path.GetFileName(doc1));
                            //statusInfo1 = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc1));
                        }
                        catch (Exception)
                        {
                            statusInfo1 = string.Format("E{0}", Path.GetFileName(doc1));
                            //statusInfo1 = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc1));
                            failed = true;
                        }
                    }

                    //if (!failed && UploadFile2.HasFile)
                    //{
                    //    doc2 = UploadFile2.FileName;
                    //    try
                    //    {
                    //        UploadFile2.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc2)));
                    //        statusInfo2 = string.Format("O{0}", Path.GetFileName(doc2));
                    //        //statusInfo2 = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc2));
                    //    }
                    //    catch (Exception)
                    //    {
                    //        statusInfo2 = string.Format("E{0}", Path.GetFileName(doc2));
                    //        //statusInfo2 = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc2));
                    //        failed = true;
                    //    }
                    //}

                    string er1 = Uri.EscapeDataString(statusInfo1);
                    string er2 = Uri.EscapeDataString(statusInfo2);
                    string res = "OK";
                    if (!failed)
                    {
                        string f1 = string.Empty, f2 = string.Empty;
                        if (!string.IsNullOrEmpty(doc1))
                            f1 = Path.Combine(uploadPath, Path.GetFileName(doc1));
                        if (!string.IsNullOrEmpty(doc2))
                            f2 = Path.Combine(uploadPath, Path.GetFileName(doc2));
                        string cerfilename = CreateCertificate(userid, password, f1, f2, null);
                    }
                    else
                    {
                        res = "ERR";
                    }

                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + "The upload of your credentials was successful." + "');", true);
                    //Added by Nagesh
                    UserInfo ui = null;
                    ClientInfo ci = null;
                    string userDocPath = null;
                    ui = db.GetUser(Util.UserId);
                    ci = db.GetClientInfo(Util.UserId);
                    userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                    decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                    if (percentComplete < 100)
                    {
                        Response.Redirect("Profile.aspx");
                    }
                    else
                    {
                        Response.Redirect("Thankyou.aspx");
                    }
                    //Added by Nagesh
                }

                //Response.Redirect("Profile.aspx");
            }
        }

        protected void Reject_Click(object sender, EventArgs e)
        {
            Response.Redirect("Profile.aspx");
        }

        protected void RemoveSoundCloud_Submit(object sender, CommandEventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.SoundCloud);
                db.UpdateSoundCloudID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();
        }

        protected void RemoveFacebook_Submit(object sender, CommandEventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);
                db.UpdateFacebookID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();
        }

        protected void RemoveTwitter_Submit(object sender, CommandEventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.Twitter);
                db.UpdateTwitterID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();
        }

        #endregion


        #region Methods ------- !

        /// <summary>
        /// Fill the control with account information
        /// </summary>
        private void FillAccountInformation()
        {
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                Name.Text = ci.FirstName + " " + ci.LastName;
                StageName.Text = ci.stagename;
                CompanyName.Text = ci.CompanyName;
                if (ci.Gender == 'm' || ci.Gender == 'M')
                    Gender.Text = "Male";
                else if (ci.Gender == 'f' || ci.Gender == 'F')
                    Gender.Text = "Female";
                DOB.Text = Convert.ToString(ci.Birthdate).Split(' ')[0];
                Address.Text = ci.AddressLine1 + System.Environment.NewLine + ci.AddressLine2;
                Pincode.Text = ci.ZipCode;
                City.Text = ci.City;
                State.Text = ci.State;
                Country.Text = ci.Country;
                Number.Text = ci.Telephone;
                BumaNo.Text = ci.BumaCode;
                SenaNo.Text = ci.SenaCode;
                ISRC.Text = ci.IsrcCode;
                OwnerKind.Text = ci.OwnerKind;
                Email.Text = ui.Email;

                if (!string.IsNullOrEmpty(ci.TwitterId))
                {
                    lblTwitter.Text = "CONNECTED AS";
                    TwitterIdLabel.Text = ci.TwitterId;
                    TwitterIdLabel.Visible = true;
                    twitterdiv.Visible = true;
                    RemoveTwitter.Visible = true;
                    linkTwitter.Enabled = false;
                }
                else
                {
                    lblTwitter.Text = "CONNECT";
                    TwitterIdLabel.Visible = false;
                    RemoveTwitter.Visible = false;
                    twitterdiv.Visible = false;
                    linkTwitter.Enabled = true;
                }

                if (!string.IsNullOrEmpty(ci.FacebookId))
                {
                    lblFacebook.Text = "CONNECTED AS";
                    FacebookIdLabel.Text = ci.FacebookId;
                    FacebookIdLabel.Visible = true;
                    facebookdiv.Visible = true;
                    RemoveFacebook.Visible = true;
                    linkFacebook.Enabled = false;
                }
                else
                {
                    lblFacebook.Text = "CONNECT";
                    facebookdiv.Visible = false;
                    FacebookIdLabel.Visible = false;
                    RemoveFacebook.Visible = false;
                    linkFacebook.Enabled = true;
                }

                if (!string.IsNullOrEmpty(ci.SoundCloudId))
                {
                    lblsoundcloud.Text = "CONNECTED AS";
                    SoundCloudLabel.Text = ci.SoundCloudId;
                    SoundCloudLabel.Visible = true;
                    soundclouddiv.Visible = true;
                    RemoveSoundCloud.Visible = true;
                    linkSoundCloud.Enabled = false;
                }
                else
                {
                    lblsoundcloud.Text = "CONNECT";
                    soundclouddiv.Visible = false;
                    SoundCloudLabel.Visible = false;
                    RemoveSoundCloud.Visible = false;
                    linkSoundCloud.Enabled = true;
                }
            }
        }

        private void UserInformation()
        {
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);
                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                CreditsLiteral.Text = Convert.ToString(Util.GetUserCredits(Util.UserId));
                ProtectedLiteral.Text = Convert.ToString(protectedTracks);

                MemberSince.Text = Convert.ToString(ui.MemberSince);

                string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                Session["percentComplete"] = percentComplete;
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                divAccPerCompleted.Visible = ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }
        }

        private string CreateCertificate(long userid, string password, string doc1, string doc2, string doc3)
        {
            string ret = string.Empty;
            using (CertificateManager mgr = new CertificateManager(userid, password))
            {
                if (!string.IsNullOrEmpty(doc1))
                    mgr.AddDocument(doc1);
                if (!string.IsNullOrEmpty(doc2))
                    mgr.AddDocument(doc2);
                if (!string.IsNullOrEmpty(doc3))
                    mgr.AddDocument(doc3);
                mgr.CreateCertificate(string.Format("ID{0:D10}.cer", userid));
                ret = mgr.CertificateFilename;
            }
            return ret;
        }

        #endregion
    }
}