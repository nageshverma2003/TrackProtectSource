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
    public partial class CouponFailure : BasePage
    {
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
            Session["bodyid"] = "coupon";
            // IncludePage(ProtectInc, Resources.Resource.incProtect);
            //IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

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
                Config cfg = new Config();
                cfg.Load(Server.MapPath("~/Config/trackprotect.config"));

                int couponCodeCount = 0;
                if (Session["coupon.entry"] != null)
                    couponCodeCount = (int)Session["coupon.entry"];
                if (couponCodeCount < 3)
                    ++couponCodeCount;
                Session["coupon.entry"] = couponCodeCount;

                if (couponCodeCount < 3)
                {
                    ErrorCode.Text = string.Format("<span style=\"color: orange;\">{0}</span>", Resources.Resource.CouponIncorrect);
                }
                else
                {
                    string couponCode = Request.Params["couponcode"];
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(Util.UserId);
                        ErrorCode.Text = string.Format("<span style=\"color: red;\">{0}</span>",
                                                       Resources.Resource.CouponError);
                        StringBuilder body = new StringBuilder();
                        body.AppendFormat("The user with id {0} ({1}) entered too many invalid coupon codes.",
                                          Util.UserId, ci.GetFullName());
                        body.Append(Environment.NewLine);
                        body.AppendFormat("The last coupon code entered was {0}.", couponCode);
                        body.Append(Environment.NewLine);
                        body.Append("Coupon code entry was disabled");

                        Util.SendEmail(new string[] { cfg["email.admin"] }, "noreply@trackprotect.com", "Invalid coupon entry", body.ToString(), null,0);

                        // Register entry violation in the database
                        // Alright. Some abuse here, we're going to use the whmcsclient_id here to store the violation.
                        // The id isn't used anymore since whmcs has been abandoned and thus it remains simply an integer.
                        db.UpdateUserWhmcsClientId(Util.UserId, 1);
                    }
                }
            }

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
    }
}