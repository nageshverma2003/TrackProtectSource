using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using TrackProtect.Logging;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.Text;
using System.Drawing;

namespace TrackProtect.Member
{
    public partial class Profile : BasePage
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
                UserInformation();
                FillAccountInformation();


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
                Logger.Instance.Write(LogLevel.Error, ex, "Profile<Exception>");
            }
        }

        protected void RemoveSoundCloud_Submit(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.SoundCloud);
                db.UpdateSoundCloudID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();

            Session["RemoveBtnIsFired"] = true;
        }

        protected void RemoveFacebook_Submit(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.Facebook);
                db.UpdateFacebookID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();

            Session["RemoveBtnIsFired"] = true;
        }

        protected void RemoveTwitter_Submit(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.RemoveSocialCredential(ci.ClientId, SocialConnector.Twitter);
                db.UpdateTwitterID(ci.ClientId);
            }

            UserInformation();
            FillAccountInformation();

            Session["RemoveBtnIsFired"] = true;
        }

        protected void AccountOverview_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                string password = Session["access"] as string;
                string uploadPath = db.GetUserDocumentPath(Util.UserId, password);

                //GenerateAccountOverView(
                //    Server.MapPath(Path.Combine(uploadPath, "/Temporaryfiles/AccInfo_" + ui.UserId + "_" + ci.FirstName + "_" + ci.LastName + ".pdf")),
                //    HttpContext.Current.Server.MapPath(Resources.Resource.AccountOverViewTemplate),
                //    ci, ui);

                using (StreamReader reader = new StreamReader(Server.MapPath("~") + "/HtmlTemplates/accountoverview_template_en_form.html"))
                {
                    String htmlText = reader.ReadToEnd();

                    HtmlToPdf(Server.MapPath(Path.Combine(uploadPath, "/Temporaryfiles/AccInfo_" + ui.UserId + "_" + ci.FirstName + "_" + ci.LastName + ".pdf")),
                        htmlText,
                        ci, ui);
                }
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

                OwnerKind.Text = ci.OwnerKind;
                Email.Text = ui.Email;

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

        private void HtmlToPdf(string uploadPath, string htmlText, ClientInfo ci, UserInfo ui)
        {
            //HttpContext context = HttpContext.Current;
            StringReader reader = new StringReader(htmlText);

            //Create PDF document
            Document document = new Document(PageSize.A4);
            HTMLWorker parser = new HTMLWorker(document);

            string PDF_FileName = Server.MapPath("~") + "/Temporaryfiles/PDF_File.pdf";
            PdfWriter.GetInstance(document, new FileStream(PDF_FileName, FileMode.Create));
            document.Open();

            Bitmap m_Bitmap = new Bitmap(960, 1098);
            PointF point = new PointF(0, 0);
            SizeF maxSize = new System.Drawing.SizeF(960, 1098);
            HtmlRenderer.HtmlRender.Render(Graphics.FromImage(m_Bitmap), htmlText, point, maxSize);

            m_Bitmap.Save(Server.MapPath("~/Temporaryfiles/" + "Test.bmp"));

            try
            {
                parser.Parse(reader);
            }
            catch (Exception ex)
            {
                //Display parser errors in PDF.
                Paragraph paragraph = new Paragraph("Error!" + ex.Message);
                Chunk text = paragraph.Chunks[0] as Chunk;
                if (text != null)
                {
                    text.Font.Color = BaseColor.RED;
                }
                document.Add(paragraph);
            }
            finally
            {
                document.Close();
                DownLoadPdf(PDF_FileName);
            }
        }

        private void DownLoadPdf(string PDF_FileName)
        {
            WebClient client = new WebClient();
            Byte[] buffer = client.DownloadData(PDF_FileName);
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-length", buffer.Length.ToString());
            Response.BinaryWrite(buffer);
        }

        #endregion
    }
}