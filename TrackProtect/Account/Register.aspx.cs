using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Globalization;
using System.IO;
using System.Text;
//using Infragistics.WebUI.WebSchedule;
using System.Configuration;

namespace TrackProtect.Account
{
    public partial class Register : BasePage
    {
        #region Variables ------- !

        //const string REGISTER_CONTINUATION_PAGE = "~/Member/ProfileInfo.aspx";
        const string REGISTER_CONTINUATION_PAGE = "~/ThankYou.aspx";

        bool _cancelRegistration;

        #endregion


        #region Events ------- !

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["bodyid"] = "user-home";

            if (Session != null)
            {
                // Store essential session information temporarily
                string culture = Session["culture"] as string ?? "nl-NL";

                Session.Clear();
                FormsAuthentication.SignOut();

                // Restore the essential session information
                Session["culture"] = culture;
            }

            if (Request.IsAuthenticated)
            {
                if (Request.Params["pid"] != null || Request.Params["tid"] != null)
                {
                    Response.Redirect(string.Format("~/Account/Login.aspx?tid={0}&pid={1}",
                                    Request.Params["tid"],
                                    Request.Params["pid"]), false);
                }
                else
                {
                    Response.Redirect("~/Default.aspx", false);
                }
            }

            Literal name = HeadLoginView.FindControl("HeadLoginName") as Literal;
            if (name != null)
            {
                if (Session["userid"] != null)
                {
                    long userId = Util.UserId;
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(userId);
                        name.Text = string.Format(" {0}", ci.GetFullName());
                    }
                }
            }

            SetStringForWizardLabel("PasswordLengthLabel", string.Format(Resources.Resource.NewPasswordReq, Membership.MinRequiredPasswordLength));

            RegisterUser.ContinueDestinationPageUrl = REGISTER_CONTINUATION_PAGE;

            if (!string.IsNullOrEmpty(Request.Params["pid"]))
            {
                Session["register.pid"] = Request.Params["pid"];
                Session["register.country"] = Request.Params["country"];
                Session["register.price"] = Request.Params["price"];
            }

            if (Session["register.pid"] != null)
            {
                string destinationPage = "BuyProduct";
                decimal price = 0m;
                if (!decimal.TryParse(Session["register.price"] as string, out price))
                    price = 0m;
                if (price == 0m)
                {
                    destinationPage = "Quotation";
                }
                else
                {
                    int prodid;
                    if (!int.TryParse(Request.Params["pid"], out prodid))
                        prodid = 0;
                    if (prodid > 0)
                    {
                        using (Database db = new MySqlDatabase())
                        {
                            ProductInfo prodInfo = db.GetProductById(prodid);
                            if (System.String.Compare(prodInfo.Extra, "subscription", System.StringComparison.OrdinalIgnoreCase) == 0)
                                destinationPage = "Subscription";
                        }
                    }
                }
                RegisterUser.ContinueDestinationPageUrl = string.Format("~/Member/{0}.aspx?pid={1}&country={2}&price={3}",
                    destinationPage,
                    Session["register.pid"],
                    Session["register.country"],
                    Session["register.price"]);
            }

            if (!string.IsNullOrEmpty(Request.Params["id"]))
            {
                Session["register.confirm.id"] = Request.Params["id"];
                Session["register.confirm.tp"] = Request.Params["tp"];
                RegisterUser.ContinueDestinationPageUrl = REGISTER_CONTINUATION_PAGE; // string.Format("~/Member/Confirm.aspx?id={0}&tp={1}", Request.Params["id"], Request.Params["tp"]);
            }

            if (!IsPostBack)
            {
                int pid = 0;
                if (Request.Params["pid"] != null)
                {
                    string tmp = Request.Params["pid"];
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        int iTmp;
                        if (int.TryParse(tmp, out iTmp))
                            pid = iTmp;
                    }
                }

                if (Request.Params["mode"] != null)
                {
                    string tmp = Request.Params["mode"];
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        if (tmp.ToLower() == "edit")
                        {
                            TextBox userName = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("UserName") as TextBox;
                            if (userName != null)
                                userName.ReadOnly = true;
                        }
                    }
                }

                if (Request.Params["res"] != null)
                {
                    string res = Request.Params["res"];
                    if (!string.IsNullOrEmpty(res))
                    {
                        string errorMsg = Resources.Resource.SecurityCodeIncorrect;
                        if (Session["errmsg"] != null)
                            errorMsg = Session["errmsg"] as string;
                        Literal ErrorMessage = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("ErrorMessage") as Literal;
                        if (ErrorMessage != null)
                            ErrorMessage.Text = string.Format(Resources.Resource.ErrorMessageFmt, errorMsg);
                    }
                }

                if (Request.QueryString["ErrorId"] == "alreadyexist")
                {
                    Response.Redirect("~/ThankYou.aspx?id=1");
                }
                else if (Request.QueryString["ErrorId"] == "error")
                {
                    Response.Redirect("~/ThankYou.aspx?id=2");
                }
                else if (Request.QueryString["ErrorId"] == "cancel")
                {
                    Response.Redirect("~/ThankYou.aspx?id=4");
                }
                else if (Request.QueryString["ErrorId"] == "success")
                {
                    string email = Request.QueryString["email"];
                    string firstName = Request.QueryString["first_name"];
                    string lastName = Request.QueryString["last_name"];
                    RegisterUser.UserName = Guid.NewGuid().ToString();

                    ViewState["pwd"] = GeneratePassword();

                    using (Database db = new MySqlDatabase())
                    {//Added by Nagesh to remove duplicate emails, Also removed email send function from db.registeruser, email should be sent from here only 
                        Util.UserId = db.RegisterUser(RegisterUser.UserName, "/", email, "", Convert.ToString(ViewState["pwd"]), null, null, 0);
                    }

                    RegisterClientInfoUsingFBCredentials(firstName, lastName, email, Convert.ToString(ViewState["pwd"]));
                    Response.Redirect("~/ThankYou.aspx");

                }
                else if (Request.QueryString["id"] != null && Request.QueryString["tp"] != null && Request.QueryString["requestingUserinfo"] != null)
                {
                    string id = Request.QueryString["id"];
                    string relationType = Request.QueryString["tp"];
                    string fullName = string.Empty;

                    try
                    {
                        fullName = EncryptionClass.Decrypt(Request.QueryString["requestingUserinfo"]);
                    }
                    catch
                    {
                    }

                    if (!string.IsNullOrEmpty(fullName))
                    {
                        TextBox FirstName = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("FirstName") as TextBox;
                        FirstName.Text = fullName.Split(' ')[0];

                        TextBox LastName = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("LastName") as TextBox;
                        LastName.Text = fullName.Split(' ')[1];
                    }

                    string requestedEmail = string.Empty;

                    using (Database db = new MySqlDatabase())
                    {
                        db.getEmailByUniqueId(id, out requestedEmail);
                    }

                    string UserEmail = requestedEmail;

                    TextBox Email = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("Email") as TextBox;
                    Email.Text = UserEmail;
                }
            }


            IncludePage(FooterLiteral, Resources.Resource.FooterSection);

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "LanguageNL" + "');", true);
            }
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            if (_cancelRegistration)
            {
                Membership.DeleteUser(RegisterUser.UserName);
                Response.Redirect("~/Account/Register.aspx?res=failed", false);
                return;
            }

            FormsAuthentication.SetAuthCookie(RegisterUser.UserName, false /* createPersistentCookie */);

            RegisterClientInfo();
            string continueUrl = RegisterUser.ContinueDestinationPageUrl;
            if (String.IsNullOrEmpty(continueUrl))
            {
                continueUrl = REGISTER_CONTINUATION_PAGE;
            }

            Response.Redirect(continueUrl, false);
        }

        protected void RegisterUser_CreatingUser(object sender, LoginCancelEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterUser.UserName))
                RegisterUser.UserName = Guid.NewGuid().ToString();

            TextBox password = (TextBox)RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("Password");
            if (!string.IsNullOrEmpty(password.Text))
                ViewState["pwd"] = Session["register.pwd"] = password.Text;

            _cancelRegistration = e.Cancel;
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            System.Web.UI.WebControls.Login _Login = HeadLoginView.FindControl("Login1") as System.Web.UI.WebControls.Login;
            if (_Login == null)
                return;

            if (_Login.UserName.Contains("@"))
            {
                string username = Membership.GetUserNameByEmail(_Login.UserName);
                if (username != null)
                {
                    if (Membership.ValidateUser(username, _Login.Password))
                    {
                        _Login.UserName = username;
                        e.Authenticated = true;
                    }
                    else
                    {
                        e.Authenticated = false;
                    }
                }
            }
            else
            {
                e.Authenticated = Membership.ValidateUser(_Login.UserName, _Login.Password);
            }
        }

        protected void SelectLanguage(object sender, CommandEventArgs e)
        {
            Session["culture"] = e.CommandArgument as string;
            Response.Redirect(Request.RawUrl, false);
        }

        #endregion


        #region Methods ------- !

        private bool ValidateUserCode(string userEnteredCode)
        {
            string guid = Session["captcha.guid"] as string;
            if (guid == null)
                return false;

            if (Session[guid].ToString().Equals(userEnteredCode))
            {
                return true;
            }
            else
            {
                // clear the session and generate a new code 
                Session[guid] = null;

                return false;
            }
        }

        string GetStringFromWizardTextBox(string name)
        {
            TextBox textBox = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as TextBox;
            if (textBox == null)
                return string.Empty;
            return textBox.Text;
        }

        char GetCharFromWizardRadioButtonList(string name)
        {
            RadioButtonList list = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as RadioButtonList;
            if (list == null)
                return 'U';

            if (list.SelectedItem == null)
                return 'U';

            return list.SelectedItem.Value[0];
        }

        string GetValueFromWizardWebDateChooser(string name)
        {
            string ret = string.Empty;
            //WebDateChooser wdc = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as WebDateChooser;
            //ret = wdc.Text;

            return ret;
        }

        void SetCharForWizardRadioButtonList(string name, char value)
        {
            RadioButtonList list = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as RadioButtonList;

            if (list == null)
                return;

            foreach (ListItem item in list.Items)
            {
                if (value == item.Value[0])
                {
                    item.Selected = true;
                    return;
                }
            }
        }

        void SetStringForWizardTextBox(string name, string val)
        {
            TextBox textBox = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as TextBox;
            if (textBox == null)
                return;
            textBox.Text = val;
        }

        void SetStringForWizardLabel(string name, string val)
        {
            Label label = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as Label;
            if (label == null)
                return;
            label.Text = val;
        }

        string GetStringFromWizardDropDown(string name)
        {
            DropDownList ddl = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as DropDownList;
            if (ddl == null)
                return string.Empty;
            int idx = ddl.SelectedIndex;
            if (idx == -1)
            {
                // Check whether there's a text
                return ddl.Text;
                //return string.Empty;
            }

            return ddl.Items[idx].Text;
        }

        void SetStringForWizardDropDown(string name, string val)
        {
            DropDownList ddl = RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl(name) as DropDownList;
            if (ddl == null)
                return;
            ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByText(val));
        }

        void RegisterClientInfo()
        {
            long userid = Util.UserId;
            if (userid == 0)
            {
                // Oops, something went wrong, report it and bail out
                return;
            }

            CultureInfo culture = new CultureInfo("nl-NL");
            if (Session["culture"] != null)
                culture = Session["culture"] as CultureInfo;
            using (Database db = new MySqlDatabase())
            {
                string lastname = GetStringFromWizardTextBox("LastName");
                string firstname = GetStringFromWizardTextBox("FirstName");
                string addressline1 = string.Empty; //GetStringFromWizardTextBox("AddressLine1");
                string addressline2 = string.Empty; //GetStringFromWizardTextBox("AddressLine2");
                string zipcode = string.Empty; //GetStringFromWizardTextBox("Zipcode");
                string state = string.Empty; //GetStringFromWizardTextBox("State");
                string city = string.Empty; //GetStringFromWizardTextBox("City");
                string country = string.Empty;
                string language = string.Empty; //GetStringFromWizardDropDown("Language");
                if (string.IsNullOrEmpty(language))
                {
                    language = LanguageIndex.Value;
                }
                language = string.Empty;
                string telephone = string.Empty; //GetStringFromWizardTextBox("Telephone");
                string cellular = string.Empty; //GetStringFromWizardTextBox("Cellular");
                string companyname = string.Empty;
                string accountowner = string.Empty;
                string bumacode = string.Empty;
                string senacode = string.Empty; //GetStringFromWizardTextBox("SenaCode");
                string isrccode = string.Empty; //GetStringFromWizardTextBox("IsrcCode");
                string twitterid = string.Empty; //GetStringFromWizardTextBox("TwitterID");
                string facebookid = string.Empty; //GetStringFromWizardTextBox("FacebookID");
                string soundcloudid = string.Empty;
                string soniallid = string.Empty; //GetStringFromWizardTextBox("SoniallID");
                string ownerkind = string.Empty;
                string creditcardnr = string.Empty;
                string creditcardcvv = string.Empty;
                string emailforreceipt = GetStringFromWizardTextBox("Email");
                string referer = string.Empty; // GetStringFromWizardTextBox("Referer");
                //string activationCode = GetStringFromWizardTextBox("ActivationCode");
                char gender = '\0';
                //string bday = GetStringFromWizardTextBox("Birthday");
                string bday = string.Empty; //GetValueFromWizardWebDateChooser("Birthday");
                string stagename = string.Empty;
                DateTime birthday = new DateTime(1, 1, 1, 0, 0, 0);
                //if (!DateTime.TryParse(bday, culture, DateTimeStyles.AssumeLocal, out birthday))
                //    birthday = new DateTime(1, 1, 1, 0, 0, 0);
                long clientid = db.RegisterClientInfo(
                    lastname,
                    firstname,
                    addressline1,
                    addressline2,
                    zipcode,
                    state,
                    city,
                    country,
                    language,
                    telephone,
                    cellular,
                    companyname,
                    userid,
                    accountowner,
                    bumacode,
                    senacode,
                    isrccode,
                    twitterid,
                    facebookid,
                    soundcloudid,
                    soniallid,
                    ownerkind,
                    creditcardnr,
                    creditcardcvv,
                    emailforreceipt,
                    referer,
                    gender,
                    birthday,
                    stagename);
                if (clientid == 0)
                {
                    Literal ErrorMessage =
                        RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("ErrorMessage") as Literal;
                    if (ErrorMessage != null)
                        ErrorMessage.Text = Resources.Resource.CouldntRegister;
                }
                else
                {
                    UserInfo ui = db.GetUser(userid);
                    ClientInfo ci = db.GetClientInfo(userid);

                    // When a user comes from an confirmation and used a different mail adres which was given in confirmation
                    // then the confirmation mail must be updated to keep the link. -Lambert 2012-12-1
                    Guid confirmationid;
                    if (!string.IsNullOrEmpty(Request.Params["id"]) && Guid.TryParse(Request.Params["id"], out confirmationid))
                    {
                        int tp;
                        if (!int.TryParse(Request.Params["tp"], out tp))
                            tp = 0;

                        db.UpdateConfirmation(confirmationid, ui.Email);
                        ConfirmationResult res = Confirmation.ProcessConfirmation(Request.Params["id"], tp);
                        switch (res)
                        {
                            case ConfirmationResult.Success:
                                break;

                            case ConfirmationResult.UserUnknown:
                                break;

                            case ConfirmationResult.ConfirmationFailed:
                                //ResultLabel.Text = Resources.Resource.Rejected;
                                break;
                        }
                    }

                    // Ok, all is set and registered, now send an e-mail to the newly registered user
                    StringBuilder body = new StringBuilder();
                    using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.SignUpAuthenticationBody)))
                    {
                        string fname = ci.FirstName;
                        string text = rdr.ReadToEnd();

                        Session.Remove("register.pwd");
                        text = text.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                        text = text.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                        text = text.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                        text = text.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                        text = text.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                        text = text.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                        text = text.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                        text = text.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                        text = text.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                        text = text.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                        text = text.Replace("{%firstname%}", ci.FirstName);
                        text = text.Replace("{%email%}", ui.Email);
                        text = text.Replace("{%password%}", Convert.ToString(ViewState["pwd"]));

                        //string link = "<a href=\"http://test.trackprotect.com/FirstLogon.aspx?userId=\"" + Util.UserId + "&email=" + ui.Email + "&password=" + pwd + "\"> Click Here </a>";


                        string key = EncryptionClass.Encrypt(Convert.ToString(ViewState["pwd"])).Replace(" ", "%20");

                        string clickherelink = "<a href=\"" + ConfigurationManager.AppSettings["SignUpLink"] + Util.UserId + "&key=" + key + "&mode=2" + "\"> Click Here </a>";
                        text = text.Replace("{%clickherelink%}", clickherelink);

                        string detailedlink = ConfigurationManager.AppSettings["SignUpLink"] + Util.UserId + "&key=" + key + "&mode=2";
                        text = text.Replace("{%detailedlink%}", detailedlink);

                        body.Append(text);
                    }

                    Util.SendEmail(new string[] { ui.Email }, "noreply@trackprotect.com", Resources.Resource.SignUpEmailSubject, body.ToString(), null, 0);
                }
            }
        }

        void RegisterClientInfoUsingFBCredentials(string firstName, string lastName, string email, string pwd)
        {
            long userid = Util.UserId;
            if (userid == 0)
            {
                // Oops, something went wrong, report it and bail out
                return;
            }

            CultureInfo culture = new CultureInfo("nl-NL");
            if (Session["culture"] != null)
                culture = Session["culture"] as CultureInfo;
            using (Database db = new MySqlDatabase())
            {
                string lastname = lastName;
                string firstname = firstName;
                string addressline1 = string.Empty; //GetStringFromWizardTextBox("AddressLine1");
                string addressline2 = string.Empty; //GetStringFromWizardTextBox("AddressLine2");
                string zipcode = string.Empty; //GetStringFromWizardTextBox("Zipcode");
                string state = string.Empty; //GetStringFromWizardTextBox("State");
                string city = string.Empty; //GetStringFromWizardTextBox("City");
                string country = string.Empty;
                string language = string.Empty; //GetStringFromWizardDropDown("Language");
                if (string.IsNullOrEmpty(language))
                {
                    language = LanguageIndex.Value;
                }
                language = string.Empty;
                string telephone = string.Empty; //GetStringFromWizardTextBox("Telephone");
                string cellular = string.Empty; //GetStringFromWizardTextBox("Cellular");
                string companyname = string.Empty;
                string accountowner = string.Empty;
                string bumacode = string.Empty;
                string senacode = string.Empty; //GetStringFromWizardTextBox("SenaCode");
                string isrccode = string.Empty; //GetStringFromWizardTextBox("IsrcCode");
                string twitterid = string.Empty; //GetStringFromWizardTextBox("TwitterID");
                string facebookid = string.Empty; //GetStringFromWizardTextBox("FacebookID");
                string soundcloudid = string.Empty;
                string soniallid = string.Empty; //GetStringFromWizardTextBox("SoniallID");
                string ownerkind = string.Empty;
                string creditcardnr = string.Empty;
                string creditcardcvv = string.Empty;
                string emailforreceipt = email;
                string referer = string.Empty; // GetStringFromWizardTextBox("Referer");
                //string activationCode = GetStringFromWizardTextBox("ActivationCode");
                char gender = '\0';
                //string bday = GetStringFromWizardTextBox("Birthday");
                string bday = string.Empty; //GetValueFromWizardWebDateChooser("Birthday");
                string stagename = string.Empty;
                DateTime birthday = new DateTime(1, 1, 1, 0, 0, 0);
                //if (!DateTime.TryParse(bday, culture, DateTimeStyles.AssumeLocal, out birthday))
                //    birthday = new DateTime(1, 1, 1, 0, 0, 0);
                long clientid = db.RegisterClientInfo(
                    lastname,
                    firstname,
                    addressline1,
                    addressline2,
                    zipcode,
                    state,
                    city,
                    country,
                    language,
                    telephone,
                    cellular,
                    companyname,
                    userid,
                    accountowner,
                    bumacode,
                    senacode,
                    isrccode,
                    twitterid,
                    facebookid,
                    soundcloudid,
                    soniallid,
                    ownerkind,
                    creditcardnr,
                    creditcardcvv,
                    emailforreceipt,
                    referer,
                    gender,
                    birthday,
                    stagename);
                if (clientid == 0)
                {
                    Literal ErrorMessage =
                        RegisterUser.CreateUserStep.ContentTemplateContainer.FindControl("ErrorMessage") as Literal;
                    if (ErrorMessage != null)
                        ErrorMessage.Text = Resources.Resource.CouldntRegister;
                }
                else
                {
                    UserInfo ui = db.GetUser(userid);
                    ClientInfo ci = db.GetClientInfo(userid);

                    // When a user comes from an confirmation and used a different mail adres which was given in confirmation
                    // then the confirmation mail must be updated to keep the link. -Lambert 2012-12-1
                    Guid confirmationid;
                    if (!string.IsNullOrEmpty(Request.Params["id"]) && Guid.TryParse(Request.Params["id"], out confirmationid))
                    {
                        int tp;
                        if (!int.TryParse(Request.Params["tp"], out tp))
                            tp = 0;

                        db.UpdateConfirmation(confirmationid, ui.Email);
                        ConfirmationResult res = Confirmation.ProcessConfirmation(Request.Params["id"], tp);
                        switch (res)
                        {
                            case ConfirmationResult.Success:
                                break;

                            case ConfirmationResult.UserUnknown:
                                break;

                            case ConfirmationResult.ConfirmationFailed:
                                //ResultLabel.Text = Resources.Resource.Rejected;
                                break;
                        }
                    }

                    // Ok, all is set and registered, now send an e-mail to the newly registered user
                    StringBuilder body = new StringBuilder();
                    using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.SignUpAuthenticationBody)))
                    {
                        string fname = ci.FirstName;
                        string text = rdr.ReadToEnd();

                        Session.Remove("register.pwd");

                        text = text.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                        text = text.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                        text = text.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                        text = text.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                        text = text.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                        text = text.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                        text = text.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                        text = text.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                        text = text.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                        text = text.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                        text = text.Replace("{%firstname%}", ci.FirstName);
                        text = text.Replace("{%email%}", ui.Email);
                        text = text.Replace("{%password%}", Convert.ToString(ViewState["pwd"]));

                        //string link = "<a href=\"http://test.trackprotect.com/FirstLogon.aspx?userId=\"" + Util.UserId + "&email=" + ui.Email + "&password=" + pwd + "\"> Click Here </a>";


                        string key = EncryptionClass.Encrypt(pwd).Replace(" ", "%20");

                        string clickherelink = "<a href=\"" + ConfigurationManager.AppSettings["SignUpLink"] + Util.UserId + "&key=" + key + "&mode=1" + "\"> Click Here </a>";
                        text = text.Replace("{%clickherelink%}", clickherelink);

                        string detailedlink = ConfigurationManager.AppSettings["SignUpLink"] + Util.UserId + "&key=" + key + "&mode=1";
                        text = text.Replace("{%detailedlink%}", detailedlink);

                        body.Append(text);
                    }

                    Util.SendEmail(new string[] { ui.Email }, "noreply@trackprotect.com", Resources.Resource.SignUpEmailSubject, body.ToString(), null, 0);
                }
            }
        }

        void TriggerCountryOnChange(string elementId, string hiddenfieldId, int index)
        {
            const string script = @"
				<script language='javascript'>
					fillLanguageList({0}, {1}, {2});
                    storeSelection({0}, {1});
				</script>";
            string scriptToExecute = string.Format(script, elementId, hiddenfieldId, index);
            if (!ClientScript.IsStartupScriptRegistered("onchangetrigger"))
                ClientScript.RegisterStartupScript(this.GetType(), "onchangetrigger", scriptToExecute);
        }

        /// <summary>
        /// this method is used to create random password in case of forgot password
        /// </summary>
        /// <returns></returns>
        string GeneratePassword()
        {
            string strPwdchar = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //string strPwdchar = "abcdefghijklmnopqrstuvwxyz0123456789#+@&$ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string strPwd = "";
            Random rnd = new Random();
            for (int i = 0; i <= 7; i++)
            {
                int iRandom = rnd.Next(0, strPwdchar.Length - 1);
                strPwd += strPwdchar.Substring(iRandom, 1);
            }
            return strPwd;
        }

        #endregion
    }
}