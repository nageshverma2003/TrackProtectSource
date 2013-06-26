using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TrackProtect.Member
{
    public partial class ArtistRelation : BasePage
    {
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
            Session["bodyid"] = "relationships";
            //IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            Util.GetUserClearanceLevels(Util.UserId, out _vcl, out _ecl);

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
                TpIdButton.Height = TpIdText.Height;

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
                if (Invitations == null)
                {
                    DataTable dt = new DataTable("confirmations");
                    dt.Columns.Add("name", typeof(string));
                    dt.Columns.Add("email", typeof(string));
                    dt.Columns.Add("confirmation_id", typeof(long));
                    dt.Columns.Add("relation_type", typeof(int));
                    Invitations = dt;
                }
                FillInvitationsTable();
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
            Invitations.Clear();
            using (Database db = new MySqlDatabase())
            {
                DataTable dt = db.GetInvitations(Util.UserId);
                pendingInvitationsHeader.Visible = (dt.Rows.Count > 0);
                foreach (DataRow row in dt.Rows)
                {
                    long confId = Convert.ToInt64(row["confirmation_id"]);
                    long userId = Convert.ToInt64(row["requesting_user_id"]);
                    int relType = Convert.ToInt32(row["relationtype"]);

                    UserInfo ui = db.GetUser(userId);
                    ClientInfo ci = db.GetClientInfo(userId);

                    AddInvitationsRow(Invitations, ci.GetFullName(), ui.Email, confId, relType);
                }
                Invitations.DefaultView.Sort = "name ASC";
                pendingInvitations.DataSource = Invitations;
                pendingInvitations.DataBind();
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

        protected void TpIdSearch(object sender, CommandEventArgs e)
        {
            RequestConfirmation(TpIdText.Text, 0);
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
                                RequestConfirmation(targetUi.Email, 1);
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
        }
    }
}