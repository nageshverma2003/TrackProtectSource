using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using TrackProtect.Logging;

namespace TrackProtect.Member
{
    /// <summary>
    /// Allows editing member's personal information
    /// </summary>
    public partial class MemberEdit : BasePage
    {
        private int _countrySelectedIndex = -1;

        /// <summary>
        /// Handler for the Page_Load event of this page
        /// </summary>
        /// <param name="sender">the sender of this event</param>
        /// <param name="e">the arguments associated with this event</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IncludePage(MemberHomeInc, Resources.Resource.incMemberHome);
                IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);
                    ClientInfo ci = db.GetClientInfo(Util.UserId);
                    DataSet ds = db.GetRegister(Util.UserId);
                    int protectedTracks = ds.Tables[0].Rows.Count;

                    LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                    LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.GetFullName());
                    CreditsLiteral.Text = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId));
                    ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);
                    string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                    decimal percentComplete = DetermineCompletion(userDocPath, ui, ci);
                    Session["percentComplete"] = percentComplete;
                    CompletedLiteral.Text = string.Empty;
                    if (percentComplete < 100)
                        CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                    ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
                }

                if (!IsPostBack)
                {
                    FillAccountInformation();
                    Language.Attributes["onchange"] = string.Format("storeSelection({0}, {1});", Language.ClientID,
                                                                    LanguageIndex.ClientID);
                    TriggerCountryOnChange(Language.ClientID, LanguageIndex.ClientID, _countrySelectedIndex);

                    //Birthday.MaxDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "MemberEdit<Exception>");
            }
        }

        /// <summary>
        /// Handler for the OnInit event of the page
        /// </summary>
        /// <param name="e">Arguments associated with this event</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            FillCountryDropDown();
        }

        /// <summary>
        /// Handler for the Render event of the page
        /// </summary>
        /// <param name="writer">The writer used to render on</param>
        protected override void Render(HtmlTextWriter writer)
        {
            LanguageInfo[] lis = Util.LanguageInfo;
            foreach (LanguageInfo li in lis)
                Page.ClientScript.RegisterForEventValidation(Language.UniqueID, li.GlobalLanguageName);
            base.Render(writer);
        }

        /// <summary>
        /// Fill the Country drop down box with country information
        /// </summary>
        void FillCountryDropDown()
        {
            if (Country != null)
            {
                Country.Attributes["onchange"] = string.Format("fillLanguageListEdit({0}, {1}, this.value);", Language.ClientID, LanguageIndex.ClientID);
                Country.Items.Clear();
                string[] countries = Util.GetCountries();
                int idx = 0;
                foreach (string country in countries)
                {
                    Country.Items.Add(new ListItem(country, idx.ToString(CultureInfo.InvariantCulture)));
                    ++idx;
                }

                idx = Country.Items.IndexOf(Country.Items.FindByText("Netherlands"));
                if (idx == -1)
                {
                    if (Session["country.index"] != null)
                        idx = (int)Session["country.index"];
                }
                _countrySelectedIndex = Country.SelectedIndex = idx;
            }
        }

        /// <summary>
        /// Register a script to trigger filling of the language drop down when
        /// a country has been selected in the country drop down.
        /// </summary>
        /// <param name="elementId">Id of the element to change</param>
        /// <param name="hiddenfieldId">the hidden field that contains the current selection</param>
        /// <param name="index">index of the selected item</param>
        void TriggerCountryOnChange(string elementId, string hiddenfieldId, int index)
        {
            const string script = @"
				<script language='javascript'>
					fillLanguageListEdit({0}, {1}, {2});
				</script>";
            string scriptToExecute = string.Format(script, elementId, hiddenfieldId, index);
            if (!ClientScript.IsStartupScriptRegistered("onchangetrigger"))
                ClientScript.RegisterStartupScript(GetType(), "onchangetrigger", scriptToExecute);
        }

        /// <summary>
        /// Fill the control with account information
        /// </summary>
        private void FillAccountInformation()
        {
            try
            {
                using (Database db = new MySqlDatabase())
                {
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

                    _countrySelectedIndex = -1;
                    LanguageIndex.Value = ci.Language;
                    TriggerCountryOnChange(Language.ClientID, LanguageIndex.ClientID, _countrySelectedIndex);
                    LastName.Text = ci.LastName;
                    FirstName.Text = ci.FirstName;
                    AddressLine1.Text = ci.AddressLine1;
                    AddressLine2.Text = ci.AddressLine2;
                    Zipcode.Text = ci.ZipCode;
                    City.Text = ci.City;
                    State.Text = ci.State;
                    Country.SelectedIndex = Country.Items.IndexOf(Country.Items.FindByText(ci.Country));
                    Language.SelectedIndex = _countrySelectedIndex;
                    Telephone.Text = ci.Telephone;
                    //Cellular.Text			= ci.Cellular;
                    AccountOwner.Text = ci.AccountOwner;
                    BumaID.Text = ci.BumaCode;
                    SenaCode.Text = ci.SenaCode;
                    IsrcCode.Text = ci.IsrcCode;
                    TwitterID.Text = ci.TwitterId;
                    FacebookID.Text = ci.FacebookId;
                    SoundCloudID.Text = ci.SoundCloudId;
                    //SoniallID.Text			= ci.SoniallId;
                    OwnerKind.SelectedIndex = OwnerKind.Items.IndexOf(OwnerKind.Items.FindByText(ci.OwnerKind));
                    //EmailForReceipt.Text	= ci.EmailReceipt;
                    foreach (ListItem item in Gender.Items)
                    {
                        if (ci.Gender == item.Value[0])
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                    //Birthday.Value = ci.Birthdate;

                    ViewState["StageName"] = ci.stagename;
                    ViewState["CompanyName"] = ci.CompanyName;
                    ViewState["EmailReceipt"] = ci.EmailReceipt;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Handler for the ModifyUserButton click event
        /// </summary>
        /// <param name="sender">the control that sent this event</param>
        /// <param name="e">the arguments associated with the click</param>
        public void ModifyUserButtonClick(Object sender, ImageClickEventArgs e)
        {
            try
            {
                string lastName = LastName.Text;
                string firstName = FirstName.Text;
                string addressLine1 = AddressLine1.Text;
                string addressLine2 = AddressLine2.Text;
                string zipcode = Zipcode.Text;
                string city = City.Text;
                string state = State.Text;
                string country = string.Empty;
                if (Country.SelectedIndex > -1)
                    country = Country.Items[Country.SelectedIndex].Text;
                string language = string.Empty;
                language = Language.SelectedIndex > -1 ? Language.Items[Language.SelectedIndex].Text : LanguageIndex.Value;

                string telephone = Telephone.Text;
                string cellular = string.Empty; // Cellular.Text;
                string accountOwner = AccountOwner.Text;
                string bumaCode = BumaID.Text;
                string senaCode = SenaCode.Text;
                string isrcCode = IsrcCode.Text;
                string twitterId = TwitterID.Text;
                string facebookId = FacebookID.Text;
                string soundcloudId = SoundCloudID.Text;
                string soniallId = SoniallID.Text;
                string ownerKind = string.Empty;
                //string emailReceipt	= EmailForReceipt.Text;
                //string referer		= Referer.Text;
                string referer = string.Empty;
                char gender = Convert.ToChar(Gender.SelectedItem.Value);
                DateTime birthday = Convert.ToDateTime("Birthday.Text");
                //if (!DateTime.TryParse(Birthday.Text, culture, DateTimeStyles.AssumeLocal, out birthday))
                //    birthday = new DateTime(1, 1, 1, 0, 0, 0);
                if (OwnerKind.SelectedIndex > -1)
                    ownerKind = OwnerKind.Items[OwnerKind.SelectedIndex].Text;
                using (Database db = new MySqlDatabase())
                {
                    ClientInfo ci = db.GetClientInfo(Util.UserId);
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
                                          Convert.ToString(ViewState["CompanyName"]),
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
                                          Convert.ToString(ViewState["EmailReceipt"]),
                                          referer,
                                          gender,
                                          birthday,
                                          Convert.ToString(ViewState["StageName"]));

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
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "MemberEdit<Exception> on ModifyUserButtonClick");
            }

            Response.Redirect("~/Member/AccountOverview.aspx", false);
        }
    }
}