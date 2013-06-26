using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Logging;
using TrackProtect.Facebook;
using Facebook;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace TrackProtect.Admin
{
    public partial class ManagePages : BasePage
    {
        string adminName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            new BasePage();

            Session["bodyid"] = "coupon";

            if (Session["AdminLogin"] == null)
            {
                divLogin.Visible = true;
            }
            else
            {
                divLogin.Visible = false;

                if (!IsPostBack)
                {
                    if (authenticateAndPopulateInformation())
                    {
                        HLfbconnect1.Enabled = false;
                        HLfbconnect2.Enabled = false;
                        
                        try
                        {
                            HLfbLocalize1.Text = Resources.Resource.AuthenticatedWithFacebook + adminName;
                            HLfbLocalize2.Text = Resources.Resource.AuthenticatedWithFacebook + adminName;                            
                        }
                        catch
                        {
                            HLfbLocalize1.Text = "Connected as " + adminName;
                            HLfbLocalize2.Text = "Connected as " + adminName;
                        }

                        divFBAuthenticate.Style.Add("width", "374px");

                        RemoveBtn.Visible = true;

                        ConnectionToFacebook.ImageUrl = "~/Images/socialmedia/clean.png";

                        divPageMapping.Visible = true;

                        using (Database db = new MySqlDatabase())
                        {
                            FBPagesList.DataSource = db.getAdminFBPages();
                            FBPagesList.DataTextField = "Value";
                            FBPagesList.DataValueField = "Key";
                            FBPagesList.DataBind();
                            FBPagesList.Items.Insert(0, new ListItem("--Select--", "Select"));


                            GenreList.DataSource = db.getGenreList();
                            GenreList.DataTextField = "Value";
                            GenreList.DataValueField = "Key";
                            GenreList.DataBind();
                            GenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                            GenreList.Items.Insert(1, new ListItem("--Add New Genre--", "Add"));

                            MappingGrid.DataSource = db.getPageGenreMapping();
                            MappingGrid.DataBind();
                        }
                    }
                    else
                    {
                        divFBAuthenticate.Style.Add("width", "230px");

                        HLfbconnect1.Enabled = true;
                        HLfbconnect2.Enabled = true;

                        HLfbLocalize1.Text = Resources.Resource.AuthenticateWithFacebook;
                        HLfbLocalize2.Text = Resources.Resource.AuthenticateWithFacebook;

                        RemoveBtn.Visible = false;

                        ConnectionToFacebook.ImageUrl = "~/Images/socialmedia/Redcross.png";

                        divPageMapping.Visible = false;

                        try
                        {
                            using (Database db = new MySqlDatabase())
                            {
                                db.deleteAdminFBCred();
                            }
                        }
                        catch { }
                    }
                }
            }

            if (Convert.ToString(Session["culture"]).Contains("nl"))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "HighLightLangBtn", "HighLightLangBtn('" + "ctl00_LanguageUS" + "');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "UnHighLightLangBtn", "UnHighLightLangBtn('" + "ctl00_LanguageNL" + "');", true);
            }
        }

        protected void FBPagesList_Selectionchanged(object sender, EventArgs e)
        {
            ViewState["PageId"] = ((DropDownList)sender).SelectedValue;
        }

        private bool authenticateAndPopulateInformation()
        {
            try
            {
                using (Database db = new MySqlDatabase())
                {
                    IDictionary<object, object> credDict = db.getAdminFBCred();

                    if (credDict != null)
                    {
                        if (credDict.Count > 0)
                        {
                            AuthenticationService auth = new AuthenticationService();

                            if (auth.TryAuthenticateAdminFBCred(Convert.ToString(credDict["expires"]), Convert.ToString(credDict["fbtoken"])))
                            {
                                adminName = db.getAdminFBName();
                                return true;
                            }
                            else
                                return false;
                        }

                        return false;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "Manage Pages");
                return false;
            }
        }

        protected void RemoveBtn_Click(object sender, EventArgs e)
        {
            if (Session["AdminLogin"] != null)
            {
                using (Database db = new MySqlDatabase())
                {
                    db.deleteAdminFBCred();

                    Response.Redirect("/Admin/ManagePages.aspx");
                }
            }
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                if (string.IsNullOrEmpty(Email.Text.Trim()))
                {
                    CustomValidator CustomValidatorCtrl = new CustomValidator();

                    CustomValidatorCtrl.IsValid = false;

                    CustomValidatorCtrl.ValidationGroup = "LoginUserValidationGroup";

                    CustomValidatorCtrl.ErrorMessage = "Enter required !";

                    this.Page.Form.Controls.Add(CustomValidatorCtrl);

                    Session.Remove("AdminLogin");
                }
                else if (string.IsNullOrEmpty(Password.Text.Trim()))
                {
                    CustomValidator CustomValidatorCtrl = new CustomValidator();

                    CustomValidatorCtrl.IsValid = false;

                    CustomValidatorCtrl.ValidationGroup = "LoginUserValidationGroup";

                    CustomValidatorCtrl.ErrorMessage = "Password required !";

                    this.Page.Form.Controls.Add(CustomValidatorCtrl);

                    Session.Remove("AdminLogin");
                }
                else
                {
                    if (db.AdminLoginAuthentication(Email.Text.Trim(), Password.Text.Trim()))
                    {
                        Session["AdminLogin"] = Email.Text.Trim();

                        Response.Redirect("ManagePages.aspx");
                    }
                    else
                    {
                        CustomValidator CustomValidatorCtrl = new CustomValidator();

                        CustomValidatorCtrl.IsValid = false;

                        CustomValidatorCtrl.ValidationGroup = "LoginUserValidationGroup";

                        CustomValidatorCtrl.ErrorMessage = "Login Failed !";

                        this.Page.Form.Controls.Add(CustomValidatorCtrl);

                        Session.Remove("AdminLogin");
                    }
                }
            }
        }

        #region Genre

        protected void AddGenre_Click(object sender, EventArgs e)
        {
            GenreList.DataSource = null;
            GenreList.DataBind();

            using (Database db = new MySqlDatabase())
            {
                db.AddGenre(addGenreText.Text.Trim());

                GenreList.DataSource = db.getGenreList();
                GenreList.DataTextField = "Value";
                GenreList.DataValueField = "Key";
                GenreList.DataBind();
                GenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                GenreList.Items.Insert(1, new ListItem("--Add New Genre--", "Add"));
            }

            EditGenre.Visible = false;
            divAddGenre.Visible = false;
            divDeleteGenre.Visible = false;
            divSubGenre.Visible = false;
        }

        protected void DeleteGenre_Click(object sender, EventArgs e)
        {
            GenreList.DataSource = null;
            GenreList.DataBind();

            MappingGrid.DataSource = null;
            MappingGrid.DataBind();

            using (Database db = new MySqlDatabase())
            {
                db.DeleteGenre(Convert.ToInt32(ViewState["GenreID"]));

                GenreList.DataSource = db.getGenreList();
                GenreList.DataTextField = "Value";
                GenreList.DataValueField = "Key";
                GenreList.DataBind();
                GenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                GenreList.Items.Insert(1, new ListItem("--Add New Genre--", "Add"));

                MappingGrid.DataSource = db.getPageGenreMapping();
                MappingGrid.DataBind();
            }

            EditGenre.Visible = false;
            divAddGenre.Visible = false;
            divDeleteGenre.Visible = false;
            divSubGenre.Visible = false;
        }

        protected void EditGenre_Click(object sender, EventArgs e)
        {
            divDeleteGenre.Visible = true;

            deleteGenreText.Text = GenreList.SelectedItem.Text;

            EditGenre.Visible = false;
        }

        protected void CanceldeletingGenre_Click(object sender, EventArgs e)
        {
            divDeleteGenre.Visible = false;

            EditGenre.Visible = true;
        }

        protected void GenreList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["GenreID"] = ((DropDownList)sender).SelectedItem.Value;

            if (Convert.ToString(ViewState["GenreID"]).Contains("Select"))
            {
                EditGenre.Visible = false;
                divAddGenre.Visible = false;
                divDeleteGenre.Visible = false;
                divSubGenre.Visible = false;
            }
            else if (Convert.ToString(ViewState["GenreID"]).Contains("Add"))
            {
                EditGenre.Visible = false;
                divAddGenre.Visible = true;
                divDeleteGenre.Visible = false;
                divSubGenre.Visible = false;
            }
            else if (Convert.ToInt32(ViewState["GenreID"]) > 0)
            {
                EditGenre.Visible = true;
                divAddGenre.Visible = false;
                divDeleteGenre.Visible = false;
                divSubGenre.Visible = true;

                EditSubGenre.Visible = false;
                divAddSubGenre.Visible = false;
                divDeleteSubGenre.Visible = false;

                SubGenreList.DataSource = null;
                SubGenreList.DataBind();

                using (Database db = new MySqlDatabase())
                {
                    SubGenreList.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["GenreID"]));
                    SubGenreList.DataTextField = "Value";
                    SubGenreList.DataValueField = "Key";
                    SubGenreList.DataBind();
                    SubGenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                    SubGenreList.Items.Insert(1, new ListItem("--Add New Sub Genre--", "Add"));
                }
            }
        }

        #endregion

        #region SubGenre

        protected void AddSubGenre_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                db.AddSubGenre(addSubGenreText.Text.Trim(), Convert.ToInt32(ViewState["GenreID"]));

                SubGenreList.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["GenreID"]));
                SubGenreList.DataTextField = "Value";
                SubGenreList.DataValueField = "Key";
                SubGenreList.DataBind();
                SubGenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                SubGenreList.Items.Insert(1, new ListItem("--Add New Sub Genre--", "Add"));

                GenreList.SelectedValue = Convert.ToString(ViewState["GenreID"]);
            }

            divAddSubGenre.Visible = false;
        }

        protected void EditSubGenre_Click(object sender, EventArgs e)
        {
            divDeleteSubGenre.Visible = true;

            DeleteSubGenreText.Text = SubGenreList.SelectedItem.Text;

            EditSubGenre.Visible = false;
        }

        protected void CanceldeletingSubGenre_click(object sender, EventArgs e)
        {
            divDeleteSubGenre.Visible = false;

            EditSubGenre.Visible = true;
        }

        protected void DeleteSubgenre_Click(object sender, EventArgs e)
        {
            using (Database db = new MySqlDatabase())
            {
                SubGenreList.DataSource = null;
                SubGenreList.DataBind();

                MappingGrid.DataSource = null;
                MappingGrid.DataBind();

                db.DeleteSubGenre(Convert.ToInt32(ViewState["SubGenreID"]));

                SubGenreList.DataSource = db.getSubGenreList(Convert.ToInt32(ViewState["GenreID"]));
                SubGenreList.DataTextField = "Value";
                SubGenreList.DataValueField = "Key";
                SubGenreList.DataBind();
                SubGenreList.Items.Insert(0, new ListItem("--Select--", "Select"));
                SubGenreList.Items.Insert(1, new ListItem("--Add New Sub Genre--", "Add"));

                MappingGrid.DataSource = db.getPageGenreMapping();
                MappingGrid.DataBind();

            }

            EditSubGenre.Visible = false;
            divAddSubGenre.Visible = false;
            divDeleteSubGenre.Visible = false;
        }

        protected void SubGenreList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["SubGenreID"] = ((DropDownList)sender).SelectedItem.Value;

            if (Convert.ToString(ViewState["SubGenreID"]).Contains("Select"))
            {
                EditSubGenre.Visible = false;
                divAddSubGenre.Visible = false;
                divDeleteSubGenre.Visible = false;
            }
            else if (Convert.ToString(ViewState["SubGenreID"]).Contains("Add"))
            {
                EditSubGenre.Visible = false;
                divAddSubGenre.Visible = true;
                divDeleteSubGenre.Visible = false;
            }
            else
            {
                EditSubGenre.Visible = true;
                divAddSubGenre.Visible = false;
                divDeleteSubGenre.Visible = false;
            }
        }

        #endregion

        protected void BtnMap_Click(object sender, EventArgs e)
        {
            if (Session["AdminLogin"] != null)
            {
                MappingGrid.DataSource = null;
                MappingGrid.DataBind();

                if (ViewState["PageId"] == null)
                {
                    ClientScript.RegisterStartupScript
                            (this.GetType(), "alert", "alert('" + "Please select a facebook page from list." + "');", true);
                }
                if (Convert.ToString(ViewState["PageId"]).Contains("Select"))
                {
                    ClientScript.RegisterStartupScript
                        (this.GetType(), "alert", "alert('" + "Please select a facebook page from list." + "');", true);
                }
                else if (ViewState["GenreID"] == null)
                {
                    ClientScript.RegisterStartupScript
                    (this.GetType(), "alert", "alert('" + "Please select a genre from list to map facebook page and genre. And select a sub genre from list to map facebook page and sub genre." + "');", true);
                }
                else if (Convert.ToString(ViewState["GenreID"]).Contains("Select") || Convert.ToString(ViewState["GenreID"]).Contains("Add"))
                {
                    ClientScript.RegisterStartupScript
                        (this.GetType(), "alert", "alert('" + "Please select a genre from list to map facebook page and genre. And select a sub genre from list to map facebook page and sub genre." + "');", true);
                }
                else
                {
                    using (Database db = new MySqlDatabase())
                    {
                        int genreid = 0;
                        string genretype = string.Empty;

                        if (ViewState["SubGenreID"] != null)
                        {
                            if (Convert.ToString(ViewState["SubGenreID"]).Contains("Select") || Convert.ToString(ViewState["SubGenreID"]).Contains("Add"))
                            {
                                genreid = Convert.ToInt32(ViewState["GenreID"]);
                                genretype = "genre";
                            }
                            else
                            {
                                genreid = Convert.ToInt32(ViewState["SubGenreID"]);
                                genretype = "subgenre";
                            }
                        }
                        else
                        {
                            genreid = Convert.ToInt32(ViewState["GenreID"]);
                            genretype = "genre";
                        }

                        if (db.AddGenreMapping(Convert.ToInt32(ViewState["PageId"]), genreid, genretype) == false)
                        {
                            switch (genretype)
                            {
                                case "genre":
                                    ClientScript.RegisterStartupScript
                                        (this.GetType(), "alert", "alert('" + "Mapping done between genre and facebook page." + "');", true);
                                    break;
                                case "subgenre":
                                    ClientScript.RegisterStartupScript
                                        (this.GetType(), "alert", "alert('" + "Mapping done between sub-genre and facebook page." + "');", true);
                                    break;
                            }
                        }
                        else
                        {
                            switch (genretype)
                            {
                                //case "genre":
                                //    ClientScript.RegisterStartupScript
                                //        (this.GetType(), "alert", "alert('" + "Mapping already exist between this genre and facebook page." + "');", true);
                                //    break;
                                //case "subgenre":
                                //    ClientScript.RegisterStartupScript
                                //        (this.GetType(), "alert", "alert('" + "Mapping already exist between this sub-genre and facebook page." + "');", true);
                                //    break;

                                case "genre":
                                    ClientScript.RegisterStartupScript
                                        (this.GetType(), "alert", "alert('" + "Mapping done between genre and facebook page." + "');", true);
                                    break;
                                case "subgenre":
                                    ClientScript.RegisterStartupScript
                                        (this.GetType(), "alert", "alert('" + "Mapping done between sub-genre and facebook page." + "');", true);
                                    break;
                            }
                        }

                        MappingGrid.DataSource = db.getPageGenreMapping();
                        MappingGrid.DataBind();
                    }
                }
            }
        }
    }
}