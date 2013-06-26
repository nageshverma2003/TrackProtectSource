using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.IO;
using System.Configuration;
using TrackProtect.Logging;
using System.Web.UI.HtmlControls;

namespace TrackProtect.Member
{
    public partial class ManageRelations : BasePage
    {
        int relType = 0;

        DataTable ManagedRelations
        {
            get { return Session["ManagedRelations"] as DataTable; }
            set { Session["ManagedRelations"] = value; }
        }

        DataTable Invitations
        {
            get { return Session["Invitations"] as DataTable; }
            set { Session["Invitations"] = value; }
        }

        DataTable ManagedInvitations
        {
            get { return Session["ManageInvitations"] as DataTable; }
            set { Session["ManageInvitations"] = value; }
        }

        DataTable ManagedArtists
        {
            get { return Session["ManagedArtists"] as DataTable; }
            set { Session["ManagedArtists"] = value; }
        }

        int _vcl = 0;
        int _ecl = 0;

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
            ResultLabel.Text = "";
            Session["bodyid"] = "relationships";
            //Session["loggedinUserEmail"] = string.Empty;
            //IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            Util.GetUserClearanceLevels(Util.UserId, out _vcl, out _ecl);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;
                //Session["loggedinUserEmail"] = ui.Email;
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
                //TpIdButton.Height = TpIdText.Height;

                if (ManagedArtists == null)
                {
                    DataTable dt = new DataTable("artists");
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("userid", typeof(long));
                    ManagedArtists = dt;
                }

                if (ManagedRelations == null)
                {
                    DataTable dt = new DataTable("artists");
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("userid", typeof(long));
                    dt.Columns.Add("reltype", typeof(int));
                    dt.Columns.Add("manage", typeof(bool));
                    ManagedRelations = dt;
                }
                FillManagedRelationsTable();
                FillInvitationsTable();
                FillManagedUsersTable();
            }

            //------- Highlight the selected lang button ------- !

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_HeadLoginView_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_HeadLoginView_LanguageNL" + "');", true);
            }

        }

        private void FillInvitationsTable()
        {
            StringBuilder sb = new StringBuilder();

            using (Database db = new MySqlDatabase())
            {
                DataTable dt = db.GetInvitations(Util.UserId);

                DataTable pendingInvitationTable = dt.Clone();

                DataTable pendingManageInvitationTable = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["relationtype"]) == 0)
                        pendingInvitationTable.ImportRow(row);
                    else
                        pendingManageInvitationTable.ImportRow(row);
                }

                if (Invitations == null)
                {
                    DataTable tempDt = new DataTable("confirmations");
                    tempDt.Columns.Add("name", typeof(string));
                    tempDt.Columns.Add("email", typeof(string));
                    tempDt.Columns.Add("confirmation_id", typeof(long));
                    tempDt.Columns.Add("relation_type", typeof(int));
                    Invitations = tempDt;
                }
                Invitations.Clear();
                if (pendingInvitationTable.Rows.Count > 0)
                {
                    pendingInvitationsHeader.Visible = (dt.Rows.Count > 0);

                    foreach (DataRow row in pendingInvitationTable.Rows)
                    {
                        long confId = Convert.ToInt64(row["confirmation_id"]);
                        long userId = Convert.ToInt64(row["requesting_user_id"]);
                        relType = Convert.ToInt32(row["relationtype"]);

                        UserInfo ui = db.GetUser(userId);
                        ClientInfo ci = db.GetClientInfo(userId);
                        AddInvitationsRow(Invitations, ci.GetFullName(), ui.Email, confId, relType);
                    }
                    Invitations.DefaultView.Sort = "name ASC";
                    pendingInvitations.DataSource = Invitations;
                    pendingInvitations.DataBind();
                }
                else
                {
                    pendingInvitations.DataSource = null;
                    pendingInvitations.DataBind();
                }

                if (ManagedInvitations == null)
                {
                    DataTable tempDt = new DataTable("confirmations");
                    tempDt.Columns.Add("name", typeof(string));
                    tempDt.Columns.Add("email", typeof(string));
                    tempDt.Columns.Add("confirmation_id", typeof(long));
                    tempDt.Columns.Add("relation_type", typeof(int));
                    ManagedInvitations = tempDt;
                }
                ManagedInvitations.Clear();
                if (pendingManageInvitationTable.Rows.Count > 0)
                {
                    pendingManageInvitationsHeader.Visible = (dt.Rows.Count > 0);

                    foreach (DataRow row in pendingManageInvitationTable.Rows)
                    {
                        long confId = Convert.ToInt64(row["confirmation_id"]);
                        long userId = Convert.ToInt64(row["requesting_user_id"]);
                        relType = Convert.ToInt32(row["relationtype"]);

                        UserInfo ui = db.GetUser(userId);
                        ClientInfo ci = db.GetClientInfo(userId);
                        AddInvitationsRow(ManagedInvitations, ci.GetFullName(), ui.Email, confId, relType);
                    }
                    ManagedInvitations.DefaultView.Sort = "name ASC";
                    pendingManageInvitations.DataSource = ManagedInvitations;
                    pendingManageInvitations.DataBind();
                }
                else
                {
                    pendingManageInvitations.DataSource = null;
                    pendingManageInvitations.DataBind();
                }
            }
        }

        private void AddInvitationsRow(DataTable table, string name, string email, long confId, int reltype)
        {
            DataRow row = table.NewRow();
            row["name"] = name;
            row["email"] = email;
            row["confirmation_id"] = confId;
            row["relation_type"] = reltype;
            table.Rows.Add(row);
        }

        private void FillManagedRelationsTable()
        {
            StringBuilder sb = new StringBuilder();
            ManagedRelations.Clear();
            using (Database db = new MySqlDatabase())
            {
                UserInfo[] uis = db.GetManagedUsers(Util.UserId, 0);
                foreach (UserInfo ui in uis)
                {
                    ClientInfo ci = db.GetClientInfo(ui.UserId);
                    int relType = db.DetermineRelationType(Util.UserId, ui.UserId);
                    AddManagedRelationRow(sb, ManagedRelations, ci.GetFullName(), ui.UserId, relType);
                }
                ManagedRelations.DefaultView.Sort = "name ASC";

                ArtistsTable.DataSource = ManagedRelations;
                ArtistsTable.DataBind();

                dlMyRelations.DataSource = ManagedRelations;
                dlMyRelations.DataBind();
                if (ManagedRelations.Rows.Count > 0)
                {
                    lblMSg.Visible = false;
                }
                else
                {
                    lblMSg.Visible = true;
                }
            }
        }

        private void AddManagedRelationRow(StringBuilder sb, DataTable table, string name, long userid, int relType)
        {
            DataRow row = table.NewRow();
            row["name"] = name;
            row["userid"] = userid;
            row["reltype"] = relType;
            row["manage"] = (_vcl >= 100 && relType == 0);
            table.Rows.Add(row);
        }

        private void RequestConfirmation(string email, int relationType, string FirstName, string LastName)
        {
            string url = string.Empty;

            if (Convert.ToString(Session["loggedinUserEmail"]).Trim() == email.Trim())
            {
                Response.Redirect("~/Member/InviteFailure.aspx?err=4");
            }
            if (!Util.VerifyEmail(email))
            {
                url = "~/Member/InviteFailure.aspx?err=1";
            }
            else
            {
                switch (Confirmation.RequestConfirmation(
                    email,
                    FirstName,
                    LastName,
                    Guid.NewGuid().ToString(),
                    Util.UserId,
                    relationType))
                {
                    case Confirmation.ConfirmationRequestResult.Success:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=0", email, email);
                        break;

                    case Confirmation.ConfirmationRequestResult.Exists:
                        url = string.Format("~/Member/InviteSuccess.aspx?email={0}&mode=1", email, email);
                        break;

                    case Confirmation.ConfirmationRequestResult.Failed:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=2", email, email);
                        break;

                    case Confirmation.ConfirmationRequestResult.AlreadyRequested:
                        url = string.Format("~/Member/InviteFailure.aspx?email={0}&err=3", email, email);
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
            HiddenField hfd = row.FindControl("HiddenFieldUserId") as HiddenField;

            switch (e.CommandName)
            {
                case "DeleteUser":
                    if (hfd != null)
                    {
                        long targetId = 0;
                        if (!long.TryParse(hfd.Value, out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                db.DeleteRelation(sourceId, targetId, 1);
                            }
                        }
                    }
                    break;

                case "RelateUser":
                    if (hfd != null)
                    {
                        long targetId = 0;
                        if (!long.TryParse(hfd.Value, out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                UserInfo targetUi = db.GetUser(targetId);
                                ClientInfo targetCI = db.GetClientInfo(targetId);

                                RequestConfirmation(targetUi.Email, 1, targetCI.FirstName, targetCI.LastName);
                            }
                        }
                    }
                    break;
            }
            FillManagedRelationsTable();

        }

        protected void ArtistsTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            string username = string.Empty;
            foreach (DataControlFieldCell cell in e.Row.Cells)
            {
                // check all cells in one row
                foreach (Control control in cell.Controls)
                {
                    if (control is HiddenField)
                    {
                        HiddenField hfd = control as HiddenField;
                        long targetId = 0;
                        if (!long.TryParse(hfd.Value, out targetId))
                            targetId = 0;

                        using (Database db = new MySqlDatabase())
                        {
                            ClientInfo ci = db.GetClientInfo(targetId);
                            username = ci.GetFullName();
                        }
                    }

                    // Must use LinkButton here instead of ImageButton
                    // if you are having Links (not images) as the command button.
                    ImageButton button = control as ImageButton;
                    if (button != null && button.CommandName == "DeleteUser")
                    {
                        // Add delete confirmation
                        if (string.IsNullOrEmpty(username))
                            username = "anonymous";
                        string question = string.Format(Resources.Resource.ConfirmDelete, username);
                        button.OnClientClick = string.Format("if (!confirm('{0}')) return;", question);
                        DataRow row = (e.Row.DataItem as DataRowView).Row;
                        bool manage = (bool)row["manage"];
                        int reltype = (int)row["reltype"];

                        // Negate 'manage' here first since it indicates whether a user
                        // CAN be managed, not whether it IS managed.
                        button.ToolTip = (!manage) ? Resources.Resource.DeleteManagedRelation : Resources.Resource.DeleteNormalRelation;
                    }
                    else if (button != null && button.CommandName == "RelateUser")
                    {
                        DataRow row = (e.Row.DataItem as DataRowView).Row;
                        bool manage = (bool)row["manage"];
                        int reltype = (int)row["reltype"];

                        // Add relation
                        button.Visible = (_ecl >= 100 && manage && reltype == 0);
                    }
                }
            }
        }

        protected void pendingInvitations_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Accept")
                {
                    int rowIndex = e.Item.DataItemIndex;
                    DataRow row = Invitations.Rows[rowIndex];

                    // Process the confirmation related to this item
                    using (Database db = new MySqlDatabase())
                    {
                        int relType = Convert.ToInt32(row["relation_type"]);
                        string uid = db.GetConfirmationUid(Convert.ToInt64(row["confirmation_id"]));
                        switch (Confirmation.ProcessConfirmation(uid, relType))
                        {
                            case ConfirmationResult.Success:
                                ResultLabel.Text = Resources.Resource.Confirmed;
                                break;

                            case ConfirmationResult.ConfirmationFailed:
                                ResultLabel.Text = Resources.Resource.Rejected;
                                break;
                        }
                    }
                }
                if (e.CommandName == "Decline")
                {
                    int rowIndex = e.Item.DataItemIndex;
                    DataRow row = Invitations.Rows[rowIndex];

                    // Process the confirmation related to this item
                    using (Database db = new MySqlDatabase())
                    {
                        db.DeleteInvitation(Convert.ToInt64(row["confirmation_id"]));
                    }
                }

                FillInvitationsTable();
                FillManagedRelationsTable();
                FillManagedUsersTable();
            }
            catch { }
        }

        protected void pendingManageInvitations_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Accept")
                {
                    int rowIndex = e.Item.DataItemIndex;
                    DataRow row = ManagedInvitations.Rows[rowIndex];

                    // Process the confirmation related to this item
                    using (Database db = new MySqlDatabase())
                    {
                        int relType = Convert.ToInt32(row["relation_type"]);
                        string uid = db.GetConfirmationUid(Convert.ToInt64(row["confirmation_id"]));
                        switch (Confirmation.ProcessConfirmation(uid, relType))
                        {
                            case ConfirmationResult.Success:
                                ResultLabel.Text = Resources.Resource.Confirmed;
                                break;

                            case ConfirmationResult.ConfirmationFailed:
                                ResultLabel.Text = Resources.Resource.Rejected;
                                break;
                        }
                    }
                }
                if (e.CommandName == "Decline")
                {
                    int rowIndex = e.Item.DataItemIndex;
                    DataRow row = ManagedInvitations.Rows[rowIndex];

                    // Process the confirmation related to this item
                    using (Database db = new MySqlDatabase())
                    {
                        db.DeleteInvitation(Convert.ToInt64(row["confirmation_id"]));
                    }
                }
                FillInvitationsTable();
                FillManagedRelationsTable();
                FillManagedUsersTable();
            }
            catch { }
        }

        protected void OnLayoutCreated(object sender, EventArgs e)
        {
            (pendingInvitations.FindControl("InvitationType") as Localize).Text = Resources.Resource.InvitationForRelation;
        }

        protected void ManageOnLayoutCreated(object sender, EventArgs e)
        {
            (pendingManageInvitations.FindControl("ManageInvitationType") as Localize).Text = Resources.Resource.InvitationForManagedArtist;
        }

        #region Managed USers Artist Managemet page code
        private void FillManagedUsersTable()
        {
            int vcl = 0, ecl = 0;
            Util.GetUserClearanceLevels(Util.UserId, out vcl, out ecl);
            if (vcl < 100 && ecl < 100)
            {
                divManagedArtists.Visible = false;
                //divManaccChk.Visible = false;
            }
            else
            {
                divManagedArtists.Visible = true;
                // divManaccChk.Visible = true;
                ManagedArtists.Clear();
                using (Database db = new MySqlDatabase())
                {
                    UserInfo[] uis = db.GetManagedUsers(Util.UserId, 1);
                    if (uis.Length > 0)
                    {
                        foreach (UserInfo ui in uis)
                        {
                            ClientInfo ci = db.GetClientInfo(ui.UserId);
                            AddManagedArtistRow(ManagedArtists, ci.GetFullName(), ui.UserId);
                            dlMyManagedRelations.DataSource = ManagedArtists;
                            dlMyManagedRelations.DataBind();
                            if (ManagedArtists.Rows.Count > 0)
                                lblMSgMANACC.Visible = false;
                            else
                                lblMSgMANACC.Visible = true;
                        }
                    }
                    else
                    {
                        dlMyManagedRelations.DataSource = ManagedArtists;
                        dlMyManagedRelations.DataBind();
                        lblMSgMANACC.Visible = true;
                    }

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


        #endregion
        //For Deletion Relations
        protected void dlMyRelations_ItemCommand(object source, DataListCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            // Retrieve the row that contains the button 
            // from the Rows collection.

            switch (e.CommandName)
            {
                case "DeleteUser":
                    if (rowIndex > 0)
                    {
                        long targetId = 0;
                        if (!long.TryParse(rowIndex.ToString(), out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                db.DeleteRelation(sourceId, targetId, 1);
                            }
                        }
                    }
                    break;

                case "RelateUser":
                    if (rowIndex > 0)
                    {
                        long targetId = 0;
                        if (!long.TryParse(rowIndex.ToString(), out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                UserInfo targetUi = db.GetUser(targetId);
                                ClientInfo targetCI = db.GetClientInfo(targetId);

                                RequestConfirmation(targetUi.Email, 1, targetCI.FirstName, targetCI.LastName);
                            }
                        }
                    }
                    break;
                case "RevokeUser":
                    if (rowIndex > 0)
                    {

                        long targetId = 0;
                        if (!long.TryParse(rowIndex.ToString(), out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                db.DeleteRelation(sourceId, targetId, 2);
                            }
                        }
                    }
                    break;
            }
            FillManagedRelationsTable();
            FillInvitationsTable();
            FillManagedUsersTable();
        }

        //For Deletion ManAcc Relations
        protected void dlMyManagedRelations_ItemCommand(object source, DataListCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            string url = null;

            switch (e.CommandName)
            {
                case "AddTrack":
                    {

                        if (rowIndex > 0)
                        {
                            url = string.Format("~/Member/RegisterDocManaged.aspx?id={0}", rowIndex);
                        }
                    }
                    break;

                case "ViewTracks":
                    {

                        if (rowIndex > 0)
                        {
                            url = string.Format("~/Member/ViewDocManaged.aspx?id={0}", rowIndex);
                        }
                    }
                    break;
                case "DeleteUser":
                    if (rowIndex > 0)
                    {
                        long targetId = 0;
                        if (!long.TryParse(rowIndex.ToString(), out targetId))
                            targetId = 0;
                        if (targetId > 0)
                        {
                            long sourceId = Util.UserId;
                            using (Database db = new MySqlDatabase())
                            {
                                db.DeleteRelation(sourceId, targetId, 1);
                            }
                            FillManagedUsersTable();

                            string mainAccUserFirstName = string.Empty;
                            string mainAccUserLastName = string.Empty;
                            string userFirstaName = string.Empty;
                            string userEmail = string.Empty;

                            using (Database db = new MySqlDatabase())
                            {
                                ClientInfo ci = db.GetClientInfo(sourceId);

                                mainAccUserFirstName = ci.FirstName;
                                mainAccUserLastName = ci.Language;
                            }

                            using (Database db = new MySqlDatabase())
                            {
                                UserInfo ui = db.GetUser(targetId);
                                ClientInfo ci = db.GetClientInfo(targetId);

                                userFirstaName = ci.FirstName;

                                userEmail = ui.Email;
                            }

                            sendMail(mainAccUserFirstName, mainAccUserLastName, userFirstaName, userEmail);
                        }

                        FillManagedRelationsTable();
                        FillInvitationsTable();
                        FillManagedUsersTable();
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(url))
                Response.Redirect(url, false);
        }

        private void sendMail(string mainAccUserFirstName, string mainAccUserLastName, string userFirstaName, string userEmail)
        {
            try
            {
                StringBuilder body = new StringBuilder();

                using (TextReader rdr = new StreamReader(Server.MapPath(Resources.Resource.ManartEndManaccCnfrmBody)))
                {
                    string text = rdr.ReadToEnd();

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

                    text = text.Replace("{%mainAccFirstName%}", mainAccUserFirstName);
                    text = text.Replace("{%mainAccLastName%}", mainAccUserLastName);
                    text = text.Replace("{%userFirstName%}", userFirstaName);

                    body.Append(text);
                }

                Util.SendEmail(new string[] { userEmail }, "noreply@trackprotect.com", Resources.Resource.ManartEndManaccCnfrmSubject, body.ToString(), null, 0);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "ManageRelations sendMail()");
            }
        }

        protected void dlMyRelations_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            HiddenField hdnRelType = (HiddenField)e.Item.FindControl("hdnRelType");
            HtmlControl liRelation = (HtmlControl)e.Item.FindControl("liRelation");
            LinkButton lnkbtnRelate = (LinkButton)e.Item.FindControl("lnkbtnRelate");
            HiddenField hdnmanaged = (HiddenField)e.Item.FindControl("hdnmanaged");
            if (Convert.ToInt32(hdnRelType.Value) == 0)
            {
                liRelation.Style["Visibility"] = "hidden";
                lnkbtnRelate.CommandName = "RelateUser";
                lnkbtnRelate.Text = Resources.Resource.ManagementRequest;
                if (hdnmanaged.Value.Trim().ToLower() == "false")
                    lnkbtnRelate.Visible = false;
                else
                    lnkbtnRelate.Visible = true;

            }
            else
            {
                liRelation.Style["Visibility"] = "block";
                lnkbtnRelate.CommandName = "RevokeUser";
                lnkbtnRelate.Text = Resources.Resource.ManagementRevoke;
            }

        }
    }
}