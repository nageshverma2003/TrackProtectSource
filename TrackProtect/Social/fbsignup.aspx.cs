using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Facebook;
using TrackProtect.Logging;
using System.Net;
using Newtonsoft.Json;
//using Facebook;
using Facebook;
using System.Configuration;

namespace TrackProtect.Social
{
    public partial class fbsignup : System.Web.UI.Page
    {
        #region Variables ------- !

        bool isEmailExist = false;
        string firstName = string.Empty;
        string lastName = string.Empty;
        string email = string.Empty;

        MeData result = null;
        AuthenticationService fbAuthentication = new AuthenticationService();

        private const string FACEBOOK_ME = "https://graph.facebook.com/me?access_token={0}";
        private const string FACEBOOK_OAUTH_DIALOG = "https://www.facebook.com/dialog/oauth";
        private const string FACEBOOK_OAUTH_ACCESS = "https://graph.facebook.com/oauth/access_token";

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Context.Session.Clear();
            }

            Me fbMe = null;
            string fbAccessToken = "";

            bool flag = true;

            try
            {
                flag = AuthenticateForSignUp(Context, out fbMe, out fbAccessToken);

                if (isEmailExist == true)
                {
                    Response.Redirect("~/Account/Register.aspx?ErrorId=alreadyexist");
                }
                else if (fbMe == null || flag == false)
                {
                    Response.Redirect("~/Account/Register.aspx?ErrorId=error");
                }
                else
                {
                    Response.Redirect("~/Account/Register.aspx?ErrorId=success&email=" + email + "&first_name=" + firstName + "&last_name=" + lastName);
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("error_reasonuser_denied")
                    || ex.ToString().Contains("erroraccess_denied")
                    || ex.ToString().Contains("error_descriptionPermissions error"))
                {
                    Response.Redirect("~/Account/Register.aspx?ErrorId=cancel");
                }
            }
        }

        public bool AuthenticateForSignUp(System.Web.HttpContext context, out Me me, out string accessToken)
        {
            me = null;
            accessToken = "";
            string code = context.Request.QueryString["code"];
            string url;
            WebClient webClient = new WebClient();

            // If this is a return of a Facebook result then check is an error has orrured
            string log = "";
            if (context.Request.QueryString["error_reason"] != null) log += "error_reason" + context.Request.QueryString["error_reason"] + ";";
            if (context.Request.QueryString["error"] != null) log += "error" + context.Request.QueryString["error"] + ";";
            if (context.Request.QueryString["error_description"] != null) log += "error_description" + context.Request.QueryString["error_description"] + ";";
            if (!String.IsNullOrEmpty(log))
            {
                Exception exception = new Exception(log);
                Logger.Instance.Write(LogLevel.Error, exception, log);
                throw exception;
            }

            // No code given means initialize Facebook authentication. Redirect this page to Facebook with neccesery parameters
            if (String.IsNullOrEmpty(code))
            {
                url = FACEBOOK_OAUTH_DIALOG;
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"];  //GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri);
                url += "&scope=email,publish_stream,manage_pages";
                url += "&state=" + context.Session.SessionID;
                context.Response.Redirect(url, true);
                //return;
            }

            // When code is given then Facebook authentication went without errors.
            if (context.Session.SessionID == context.Request.QueryString["state"])
            {
                url = FACEBOOK_OAUTH_ACCESS;
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"];  //GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri.Replace(context.Request.Url.Query, ""));
                url += "&client_secret=" + ConfigurationManager.AppSettings["Facebook.App.Secret"];  //GetConfiguration("facebook.app_secret");
                url += "&code=" + code;
                string oAuthAccessResult = webClient.DownloadString(url);
                Logger.Instance.Write(LogLevel.Debug, "Facebook authentication result: " + oAuthAccessResult, oAuthAccessResult);

                DateTime expires = DateTime.Now.AddMinutes(1);
                int expiresIndex = oAuthAccessResult.IndexOf("&expires=");
                if (expiresIndex == -1)
                {
                    expiresIndex = oAuthAccessResult.Length;
                }
                else
                {
                    string expiresStr = oAuthAccessResult.Substring(expiresIndex + 9, oAuthAccessResult.Length - expiresIndex - 9);
                    int expiresInSec = int.Parse(expiresStr);
                    expires = DateTime.Now.AddSeconds(expiresInSec);
                }
                accessToken = oAuthAccessResult.Substring(oAuthAccessResult.IndexOf("access_token=") + 13, expiresIndex - 13);

                me = GetMe(accessToken);
                if (me == null)
                {
                    Logger.Instance.Write(LogLevel.Error, "Cannot verify the Facebook account", new object[] { accessToken });
                    throw new Exception("Cannot verify the Facebook account");
                }

                var _client = new FacebookClient(accessToken);
                dynamic _me = _client.Get("me");
                firstName = _me.first_name;
                lastName = _me.last_name;
                email = _me.email;

                using (Database db = new MySqlDatabase())
                {
                    isEmailExist = db.isEmailAlreadyRegistered(email);
                }

                if (string.IsNullOrEmpty(email) || isEmailExist == true)
                    return false;

                //fbAuthentication.Persist(me, accessToken, expires);
            }

            return true;
        }

        private string GetConfiguration(string key)
        {
            string res = null;
            using (Database db = new MySqlDatabase())
            {
                res = db.GetSetting(key);
            }
            return res;
        }

        public Me GetMe(string accessToken)
        {
            Me me = null;
            try
            {
                WebClient webClient = new WebClient();
                string rawResult = webClient.DownloadString(String.Format(FACEBOOK_ME, accessToken));
                Me result = JsonConvert.DeserializeObject<Me>(rawResult); // http://james.newtonking.com/projects/json-net.aspx
                me = result;
            }
            catch (Exception ex)
            {
                Log.Instance.Write(LogLevel.Error, ex);
            }
            return me;
        }
    }
}