using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Logging;
using System.Data;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;

namespace TrackProtect.Member
{
    public partial class ProfileReg : BasePage
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
                    BumaNo.Focus();
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
                Logger.Instance.Write(LogLevel.Error, ex, "ProfileReg<Exception>");
            }
        }

        protected void Accept_Click(object sender, EventArgs e)
        {
            ModifyClientInfo();
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

        protected void AccountOverview_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                string password = Session["access"] as string;
                string uploadPath = db.GetUserDocumentPath(Util.UserId, password);

                GenerateAccountOverView(
                    Server.MapPath(Path.Combine(uploadPath, "/Temporaryfiles/AccInfo_" + ui.UserId + "_" + ci.FirstName + "_" + ci.LastName + ".pdf")),
                    HttpContext.Current.Server.MapPath(Resources.Resource.AccountOverViewTemplate),
                    ci, ui);
            }
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

                string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                string identDocName = string.Format("ID{0:D10}.cer", ui.UserId);
                string identDocPath = Path.Combine(userDocPath, identDocName);

                // Assume the button will need to be visible, if not so we will discover afterwards                
                if (File.Exists(identDocPath))
                {
                    IdentityCertificate.Text = Path.GetFileName(identDocPath);
                    DownloadIdent.Visible = true;
                    //DownloadIdent.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(identDocPath));
                    DownloadIdent.Attributes.Add("onclick", "InitializeRequest('" + identDocPath.Replace("\\", "/") + "');");
                }
                else
                {
                    // credential document has been marked as 'old' due to changed information
                    // of the client and no new credential file has been uploaded
                    identDocName = string.Format("ID{0:D10}.0.cer", ui.UserId);
                    identDocPath = Path.Combine(userDocPath, identDocName);
                    if (File.Exists(identDocPath))
                    {
                        IdentityCertificate.Text = Path.GetFileName(identDocPath);
                        DownloadIdent.Visible = true;
                        //DownloadIdent.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", Server.MapPath(identDocPath));
                        DownloadIdent.Attributes.Add("onclick", "InitializeRequest('" + identDocPath.Replace("\\", "/") + "');");
                    }
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

        private void ModifyClientInfo()
        {
            try
            {
                using (Database db = new MySqlDatabase())
                {
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

                    string lastName = ci.LastName;
                    string firstName = ci.FirstName;
                    string addressLine1 = ci.AddressLine1;
                    string addressLine2 = ci.AddressLine2;
                    string zipcode = ci.ZipCode;
                    string city = ci.City;
                    string state = ci.State;
                    string country = ci.Country;
                    string language = ci.Language;
                    string telephone = ci.Telephone;
                    string cellular = ci.Cellular; // Cellular.Text;
                    string accountOwner = ci.AccountOwner;
                    string bumaCode = BumaNo.Text.Trim();
                    string senaCode = SenaNo.Text.Trim();
                    string isrcCode = ISRC.Text.Trim();
                    string twitterId = ci.TwitterId.Trim();
                    string facebookId = ci.FacebookId.Trim();
                    string soundcloudId = ci.SoundCloudId.Trim();
                    string soniallId = ci.SoniallId.Trim();
                    string ownerKind = ci.OwnerKind;
                    string referer = ci.Referer;
                    char gender = ci.Gender;
                    string stagename = ci.stagename;
                    string companyname = ci.CompanyName;
                    DateTime birthday = Convert.ToDateTime(ci.Birthdate);

                    if (ci != null)
                        referer = ci.Referer;
                    db.RegisterClientInfo(lastName,
                                          firstName,
                                          addressLine1,
                                          addressLine2,
                                          zipcode,
                                          state,
                                          city,
                                          country,
                                          language,
                                          telephone,
                                          cellular,
                                          companyname,
                                          Util.UserId,
                                          accountOwner,
                                          bumaCode,
                                          senaCode,
                                          isrcCode,
                                          twitterId,
                                          facebookId,
                                          soundcloudId,
                                          soniallId,
                                          ownerKind,
                                          string.Empty,
                                          string.Empty,
                                          string.Empty,
                                          referer,
                                          gender,
                                          birthday,
                                          stagename);

                    // Remove the user's identification certificate so a new one
                    // can be registered.
                    UserInfo ui = db.GetUser(Util.UserId);
                    string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                    string identDocName = string.Format("ID{0:D10}.cer", ui.UserId);
                    string identDocPath = Path.Combine(userDocPath, identDocName);

                    if (File.Exists(identDocPath))
                    {
                        string filePattern = string.Format("ID{0:D10}.*.cer", ui.UserId);
                        string[] files = Directory.GetFiles(userDocPath, filePattern);
                        int highIndex = -1;
                        foreach (string file in files)
                        {
                            string filename = Path.GetFileName(file);
                            if (filename != null)
                            {
                                string[] parts = filename.Split('.');
                                if (parts.Length == 3)
                                {
                                    int index;
                                    if (int.TryParse(parts[1], out index))
                                    {
                                        if (index > highIndex)
                                            highIndex = index;
                                    }
                                }
                            }
                        }
                        if (highIndex > -1)
                        {
                            ++highIndex;
                            while (highIndex > 0)
                            {
                                string srcFilename = Path.Combine(userDocPath,
                                                                  string.Format("ID{0:D10}.{1}.cer",
                                                                                ui.UserId, highIndex - 1));
                                string tgtFilename = Path.Combine(userDocPath,
                                                                  string.Format("ID{0:D10}.{1}.cer",
                                                                                ui.UserId, highIndex));
                                File.Move(srcFilename, tgtFilename);
                                --highIndex;
                            }
                        }

                        string newDocName = string.Format("ID{0:D10}.0.cer", ui.UserId);
                        string newDocPath = Path.Combine(userDocPath, newDocName);
                        if (File.Exists(newDocPath))
                            File.Delete(newDocPath);
                        string newPdfPath = Path.ChangeExtension(newDocPath, ".pdf");
                        if (File.Exists(newPdfPath))
                            File.Delete(newPdfPath);
                        string identPdfPath = Path.ChangeExtension(identDocPath, ".pdf");

                        File.Move(identDocPath, newDocPath);
                        if (File.Exists(identPdfPath))
                            File.Move(identPdfPath, newPdfPath);

                        //File.Delete(identDocPath);

                        //Added by Nagesh

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
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "MemberEdit<Exception> on ModifyUserButtonClick");
            }

            Response.Redirect("~/Member/ProfilePrint.aspx", false);
        }

        public void GenerateAccountOverView(string targetFilename, string templateFilename, ClientInfo ci, UserInfo ui)
        {
            PdfReader reader = new PdfReader(templateFilename);
            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(targetFilename, FileMode.Create)))
            {
                AcroFields form = stamper.AcroFields;
                var fieldKeys = form.Fields.Keys;
                foreach (string fieldKey in fieldKeys)
                {
                    if (fieldKey.Contains("name"))
                        form.SetField(fieldKey, ci.FirstName + " " + ci.LastName);
                    if (fieldKey.Contains("gender"))
                    {
                        if (ci.Gender == 'm' || ci.Gender == 'M')
                            form.SetField(fieldKey, "Male");
                        else if (ci.Gender == 'f' || ci.Gender == 'F')
                            form.SetField(fieldKey, "Female");
                    }
                    if (fieldKey.Contains("bumastemra"))
                        form.SetField(fieldKey, ci.BumaCode);
                    if (fieldKey.Contains("senareg"))
                        form.SetField(fieldKey, ci.SenaCode);
                    if (fieldKey.Contains("isrchandle"))
                        form.SetField(fieldKey, ci.IsrcCode);
                    if (fieldKey.Contains("birthdate"))
                        form.SetField(fieldKey, Convert.ToString(ci.Birthdate));
                    if (fieldKey.Contains("country"))
                        form.SetField(fieldKey, ci.Country);
                    if (fieldKey.Contains("phonenumber"))
                        form.SetField(fieldKey, ci.Telephone);
                    if (fieldKey.Contains("emailaddress"))
                        form.SetField(fieldKey, ui.Email);
                    if (fieldKey.Contains("role"))
                        form.SetField(fieldKey, ci.OwnerKind);
                }

                stamper.FormFlattening = true;
            }

            try
            {
                System.IO.FileInfo FileName = new System.IO.FileInfo(targetFilename);
                FileStream myFile = new FileStream(targetFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader _BinaryReader = new BinaryReader(myFile);


                try
                {
                    long startBytes = 0;

                    Response.Clear();
                    Response.Buffer = false;
                    Response.AddHeader("Accept-Ranges", "bytes");
                    Response.ContentType = "application/octet-stream";
                    string fileName = "AccInfo_" + ui.UserId + "_" + ci.FirstName + "_" + ci.LastName + ".pdf";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    Response.AddHeader("Content-Length", (FileName.Length - startBytes).ToString());
                    Response.AddHeader("Connection", "Keep-Alive");
                    Response.ContentEncoding = Encoding.UTF8;

                    //Send data
                    _BinaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    //Dividing the data in 1024 bytes package
                    int maxCount = (int)Math.Ceiling((FileName.Length - startBytes + 0.0) / 1024);

                    //Download in block of 1024 bytes
                    int i;
                    for (i = 0; i < maxCount && Response.IsClientConnected; i++)
                    {
                        Response.BinaryWrite(_BinaryReader.ReadBytes(1024));
                        Response.Flush();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Response.End();
                    _BinaryReader.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileaccessWarning", "alert('File not found.');", true);
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileaccessWarning", "alert('Please provide access permissions to the file path.');", true);
            }
            catch (Exception ex)
            {
                string str = ex.Message.Replace("'", "\"").Replace("\r\n", string.Empty);

                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileerrorWarning", "alert('" + str + "');", true);
            }

            try
            {
                File.Delete(targetFilename);
            }
            catch { }

        }

        #endregion
    }
}