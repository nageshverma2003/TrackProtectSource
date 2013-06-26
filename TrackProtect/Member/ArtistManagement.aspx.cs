using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class ArtistManagement : BasePage
    {
        DataTable ManagedArtists
        {
            get { return Session["ManagedArtists"] as DataTable; }
            set { Session["ManagedArtists"] = value; }
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
            Session["bodyid"] = "relationships";
            // Ensure the user actually has clearance for this page, otherwise
            // revert to the default logged in page
            int vcl = 0, ecl = 0;
            Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);
            if (vcl < 100 && ecl < 100)
                Server.Transfer("~/Member/MemberHome.aspx");

            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);
                               
                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName);// ci.GetFullName());
                CreditsLiteral.Text =  Util.GetUserCredits(Util.UserId).ToString();
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
                TpIdButton.Height = TpIdText.Height;

                if (ManagedArtists == null)
                {
                    DataTable dt = new DataTable("artists");
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("userid", typeof(long));
                    ManagedArtists = dt;
                }
                FillManagedUsersTable();
            }
        }

        private void FillManagedUsersTable()
        {
            ManagedArtists.Clear();
            using (Database db = new MySqlDatabase())
            {
                UserInfo[] uis = db.GetManagedUsers(Util.UserId, 1);
                foreach (UserInfo ui in uis)
                {
                    ClientInfo ci = db.GetClientInfo(ui.UserId);
                    AddManagedArtistRow(ManagedArtists, ci.GetFullName(), ui.UserId);
                    ArtistsTable.DataSource = ManagedArtists;
                    ArtistsTable.DataBind();
                }
            }
        }

        private void AddManagedArtistRow(DataTable table, string name, long userid)
        {
            DataRow row = table.NewRow();
            row["name"] = name;
            row["userid"] = userid;
            table.Rows.Add(row);
        }

        protected void TpIdSearch(object sender, CommandEventArgs e)
        {
            RequestConfirmation(TpIdText.Text, 1);
        }

        private void RequestConfirmation(string email, int relationType)
        {
            string url = string.Empty;
            if (!Util.VerifyEmail(email))
            {
                url = "~/Member/InviteFailure.aspx?err=1";
            }
            else
            {
                switch (Confirmation.RequestConfirmation(
                    TpIdText.Text,
                    FirstName.Text,
                    LastName.Text,
                    Guid.NewGuid().ToString(),
                    Util.UserId,
                    relationType))
                {
                    case Confirmation.ConfirmationRequestResult.Success:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=0", TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.Exists:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=1", TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.Failed:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=2", TpIdText.Text);
                        break;

                    case Confirmation.ConfirmationRequestResult.AlreadyRequested:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=3", TpIdText.Text);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(url))
                Response.Redirect(url, false);
        }

        protected void ArtistsTable_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            // Retrieve the row that contains the button 
            // from the Rows collection.
            GridViewRow row = ArtistsTable.Rows[rowIndex];
            HiddenField hfd = null;
            string url = null;

            switch (e.CommandName)
            {
                case "AddTrack":
                    {
                        hfd = row.FindControl("HiddenFieldUserId") as HiddenField;
                        if (hfd != null)
                        {
                            url = string.Format("~/Member/RegisterDocManaged.aspx?id={0}", hfd.Value);
                        }
                    }
                    break;

                case "ViewTracks":
                    {
                        hfd = row.FindControl("HiddenFieldUserId") as HiddenField;
                        if (hfd != null)
                        {
                            url = string.Format("~/Member/ViewDocManaged.aspx?id={0}", hfd.Value);
                        }
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(url))
                Response.Redirect(url, false);
        }       
    }
}