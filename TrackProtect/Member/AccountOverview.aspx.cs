using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Logging;

namespace TrackProtect.Member
{
    public partial class AccountOverview : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IncludePage(AccountOverviewInc, Resources.Resource.incMemberHome);
                IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

                using (Database db = new MySqlDatabase())
                {

                    UserInfo ui = db.GetUser(Util.UserId);
                    ClientInfo ci = db.GetClientInfo(Util.UserId);

                    DataSet ds = db.GetRegister(Util.UserId);
                    int protectedTracks = ds.Tables[0].Rows.Count;

                    LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                    LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); //ci.GetFullName());
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

                FillAccountInformation();

                int couponEntry = 0;
                if (Session["coupon.entry"] != null)
                    couponEntry = (int)Session["coupon.entry"];

                if (!IsPostBack)
                {
                    using (Database db = new MySqlDatabase())
                    {
                        int violationState = db.GetUserWhmcsClientId(Util.UserId);
                        if (violationState == 1)
                            couponEntry = 3;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "AccountOverView<Exception>");
            }
        }

        private void FillAccountInformation()
        {
            try
            {
                const string spanFormat = "<span class=\"accountData\">{0}</span>";
                using (Database db = new MySqlDatabase())
                {
                    UserInfo ui = db.GetUser(Util.UserId);
                    ClientInfo ci = db.GetClientInfo(Util.UserId);
                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    CountryInfo countryInfo = Util.GetCountryInfo(ci.Country);
                    if (countryInfo != null)

                        //cultureInfo = new CultureInfo(countryInfo.CultureName);
                        cultureInfo = new CultureInfo(cultureInfo.LCID);

                    string name = ci.GetFullName();
                    if (!string.IsNullOrEmpty(ci.AccountOwner))
                        name = ci.AccountOwner;

                    string subTypeName = "???";
                    string[] productNames = db.GetProductNames();
                    int subType = ui.SubscriptionType;
                    if (subType > 0)
                        --subType;
                    if (subType > -1 && subType < productNames.Length)
                        subTypeName = productNames[subType];

                    ButtonEditAccount.NavigateUrl = "~/Member/MemberEdit.aspx?mode=edit&pid=" + ui.SubscriptionType;

                    AccountNameLabel.Text = string.Format(spanFormat, ci.GetFullName());
                    string gender = string.Empty;
                    switch (ci.Gender)
                    {
                        case 'F':
                            gender = Resources.Resource.Female;
                            break;
                        case 'M':
                            gender = Resources.Resource.Male;
                            break;
                    }
                    Gender.Text = gender;
                    Birthdate.Text = ci.Birthdate.ToString("dd-MM-yyyy");
                    StringBuilder sb = new StringBuilder();
                    sb.Append(ci.AddressLine1);
                    if (!string.IsNullOrEmpty(ci.AddressLine2))
                    {
                        sb.Append("<br/>");
                        sb.Append(ci.AddressLine2);
                    }
                    sb.Append("<br/>");
                    sb.Append(ci.ZipCode);
                    sb.Append(" ");
                    sb.Append(ci.City);
                    sb.Append(", ");
                    sb.Append(ci.State);
                    sb.Append("<br/>");
                    sb.Append(ci.Country);
                    Address.Text = string.Format(spanFormat, sb.ToString());
                    Telephone.Text = string.Format(spanFormat, ci.Telephone);                    
                    MemberSinceLabel.Text = string.Format(spanFormat, ui.MemberSince.Date.ToString("D", cultureInfo));                   
                    EmailLabel.Text = string.Format(spanFormat, ui.Email);
                    IamLabel.Text = string.Format(spanFormat, ci.OwnerKind);
                    BumaCodeLabel.Text = string.Format(spanFormat, ci.BumaCode);
                    TwitterIdLabel.Text = string.Format(spanFormat, ci.TwitterId);
                    FacebookIdLabel.Text = string.Format(spanFormat, ci.FacebookId);
                    SoundCloudLabel.Text = string.Format(spanFormat, ci.SoundCloudId);
                    SoniallIdLabel.Text = string.Format(spanFormat, ci.SoniallId);
                    SenaCodeLabel.Text = string.Format(spanFormat, ci.SenaCode);
                    IsrcCodeLabel.Text = string.Format(spanFormat, ci.IsrcCode);
                    //CreditLiteral.Text = string.Format(spanFormat, ui.Credits);

                    string userDocPath = db.GetUserDocumentPath(ui.UserId, Session["access"] as string);
                    string identDocName = string.Format("ID{0:D10}.cer", ui.UserId);
                    string identDocPath = Path.Combine(userDocPath, identDocName);

                    // Assume the button will need to be visible, if not so we will discover afterwards
                    UploadCredentialsButton.Visible = true;
                    if (File.Exists(identDocPath))
                    {
                        IdentityCertificate.Text = Path.GetFileName(identDocPath);
                        DownloadIdent.Visible = true;
                        DownloadIdent.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", identDocPath);
                        UploadCredentialsButton.Visible = false;
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
                            DownloadIdent.NavigateUrl = string.Format("~/DownloadHandler.ashx?file='{0}'", identDocPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "AccountOverView<Exception> FillAccountInformation()");
            }
        }
    }
}