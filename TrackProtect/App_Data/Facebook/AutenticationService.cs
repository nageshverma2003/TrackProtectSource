using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TrackProtect.Logging;
using Facebook;
using System.IO;
using System.Text;
using System.Dynamic;
using System.Configuration;

namespace TrackProtect.Facebook
{
    /// <summary>
    /// Provides authentication service for Facebook
    /// </summary>
    public class AuthenticationService
    {
        private const string FACEBOOK_ME = "https://graph.facebook.com/me?access_token={0}";
        private const string FACEBOOK_OAUTH_DIALOG = "https://www.facebook.com/dialog/oauth";
        private const string FACEBOOK_OAUTH_ACCESS = "https://graph.facebook.com/oauth/access_token";

        private UserInfo _userInfo;
        private ClientInfo _clientInfo;

        /// <summary>
        /// Save Facebook data to the database
        /// </summary>
        /// <param name="me">Me info</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="expires">Experiation of the access token</param>
        public void Persist(Me me, string accessToken, DateTime expires)
        {
            if (_clientInfo == null) GetUser();

            using (Database db = new MySqlDatabase())
            {
                db.UpdateSocialCredential(_clientInfo.ClientId, SocialConnector.Facebook, "facebookid", me.Id);
                db.UpdateSocialCredential(_clientInfo.ClientId, SocialConnector.Facebook, "accesstoken", accessToken);
                db.UpdateSocialCredential(_clientInfo.ClientId, SocialConnector.Facebook, "accesstokenexpires", expires.ToString("o"));

                string facebookId = me.Name;
                _clientInfo.FacebookId = facebookId;
                db.RegisterClientInfo(
                    _clientInfo.LastName,
                    _clientInfo.FirstName,
                    _clientInfo.AddressLine1,
                    _clientInfo.AddressLine2,
                    _clientInfo.ZipCode,
                    _clientInfo.State,
                    _clientInfo.City,
                    _clientInfo.Country,
                    _clientInfo.Language,
                    _clientInfo.Telephone,
                    _clientInfo.Cellular,
                    _clientInfo.CompanyName,
                    _clientInfo.UserId,
                    _clientInfo.AccountOwner,
                    _clientInfo.BumaCode,
                    _clientInfo.SenaCode,
                    _clientInfo.IsrcCode,
                    _clientInfo.TwitterId,
                    _clientInfo.FacebookId,
                    _clientInfo.SoundCloudId,
                    _clientInfo.SoniallId,
                    _clientInfo.OwnerKind,
                    _clientInfo.CreditCardNr,
                    _clientInfo.CreditCardCvv,
                    _clientInfo.EmailReceipt,
                    _clientInfo.Referer,
                    _clientInfo.Gender,
                    _clientInfo.Birthdate,
                    _clientInfo.stagename);
            }
        }


        public void PersistForAdmin(Me me, string accessToken, DateTime expires)
        {
            using (Database db = new MySqlDatabase())
            {
                string fbid = me.Id;
                string fbname = me.Name;
                db.saveAdminFBCred(fbid, fbname, accessToken, expires);

                IDictionary<string, string> pagesAlreadyInDB = db.getAlreadyStoredPageInfo();
                IList<TrackProtect.Facebook.Page> pageList = new List<TrackProtect.Facebook.Page>();

                if (pagesAlreadyInDB != null)
                    if (pagesAlreadyInDB.Count > 0)
                    {
                        pageList = getFacebookPageList(accessToken);

                        foreach (TrackProtect.Facebook.Page pg in pageList)
                        {
                            bool exist = false;

                            foreach (var dict in pagesAlreadyInDB)
                            {
                                if (pg.PageID == dict.Value)
                                {
                                    exist = true;
                                    break;
                                }
                            }

                            if (exist == false)
                                db.saveAdminFBPages(pg.PageName, pg.PageID, pg.AccessToken);
                        }

                        return;
                    }


                pageList = getFacebookPageList(accessToken);

                foreach (TrackProtect.Facebook.Page pg in pageList)
                {
                    db.saveAdminFBPages(pg.PageName, pg.PageID, pg.AccessToken);
                }
            }
        }

        private IList<TrackProtect.Facebook.Page> getFacebookPageList(string accessToken)
        {
            try
            {
                IList<TrackProtect.Facebook.Page> faceBookPageList = new List<TrackProtect.Facebook.Page>();

                FacebookClient client = new FacebookClient(accessToken);

                var PageListDataString = client.Get("/me/accounts");

                MemoryStream ms =
                    new MemoryStream(Encoding.Default.GetBytes(Convert.ToString(PageListDataString).ToCharArray()));

                StreamReader sr = new StreamReader(ms);

                Dictionary<string, object> facebookJson =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(sr.ReadToEnd());

                foreach (KeyValuePair<string, object> json in facebookJson)
                {
                    switch (json.Key.ToLower())
                    {
                        case "data":

                            List<object> dataList =
                                JsonConvert.DeserializeObject<List<object>>(json.Value.ToString());

                            int index = 0;

                            while (index != dataList.Count)
                            {
                                Dictionary<string, object> pageDictionary =
                                    JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(dataList[index]));

                                TrackProtect.Facebook.Page fbPageObj = new TrackProtect.Facebook.Page();

                                foreach (KeyValuePair<string, object> page in pageDictionary)
                                {
                                    switch (page.Key)
                                    {
                                        case "name":
                                            fbPageObj.PageName = Convert.ToString(page.Value);
                                            break;
                                        case "id":
                                            fbPageObj.PageID = Convert.ToString(page.Value);
                                            break;
                                        case "access_token":
                                            fbPageObj.AccessToken = Convert.ToString(page.Value);
                                            break;
                                    }
                                }

                                try
                                {
                                    faceBookPageList.Add(fbPageObj);
                                }
                                catch
                                {
                                    return null;
                                }

                                index++;
                            }

                            break;
                    }
                }

                return faceBookPageList;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Authenticate with social credentials from database. Also validates at Facebook.
        /// </summary>
        /// <param name="me">Me info</param>
        /// <param name="accessToken">Access token</param>
        /// <returns>True if authenticated, false if not.</returns>
        public bool TryAuthenticate(out Me me, out string accessToken)
        {
            if (_clientInfo == null) GetUser();

            me = null;
            accessToken = "";
            using (Database db = new MySqlDatabase())
            {
                accessToken = db.GetSocialCredential(_clientInfo.ClientId, SocialConnector.Facebook, "accesstoken");
                if (String.IsNullOrEmpty(accessToken))
                    return false;

                string expiresRaw = db.GetSocialCredential(_clientInfo.ClientId, SocialConnector.Facebook, "accesstokenexpires");
                if (String.IsNullOrEmpty(expiresRaw))
                    return false;

                DateTime expires = DateTime.MinValue;
                if (!DateTime.TryParse(expiresRaw, out expires))
                    return false;
                if (expires < DateTime.Now)
                    return false;

                me = GetMe(accessToken);
                if (me == null)
                    return false;
            }

            return true;
        }


        public bool TryAuthenticateAdminFBCred(string expiresRaw, string accessToken)
        {
            Me me;

            if (String.IsNullOrEmpty(accessToken))
                return false;

            if (String.IsNullOrEmpty(expiresRaw))
                return false;

            DateTime expires = DateTime.MinValue;
            if (!DateTime.TryParse(expiresRaw, out expires))
                return false;

            if (expires < DateTime.Now)
                return false;

            me = GetMe(accessToken);
            if (me == null)
                return false;

            return true;
        }

        /// <summary>
        /// Authenticate on Facebook through a aspx page.
        /// </summary>
        /// <param name="context">Http context to the login page</param>
        /// <param name="me">Me info</param>
        /// <param name="accessToken">Access token</param>
        public void Authenticate(System.Web.HttpContext context, out Me me, out string accessToken)
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
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"]; //GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri);
                url += "&scope=publish_stream,manage_pages,publish_actions,read_friendlists,manage_friendlists";
                url += "&state=" + context.Session.SessionID;
                context.Response.Redirect(url, true);
                return;
            }

            // When code is given then Facebook authentication went without errors.
            if (context.Session.SessionID == context.Request.QueryString["state"])
            {
                url = FACEBOOK_OAUTH_ACCESS;
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"];  //GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri.Replace(context.Request.Url.Query, ""));
                url += "&client_secret=" + ConfigurationManager.AppSettings["Facebook.App.Secret"]; //GetConfiguration("facebook.app_secret");
                url += "&code=" + code;
                string oAuthAccessResult = webClient.DownloadString(url);
                Logger.Instance.Write(LogLevel.Debug, "Facebook authentication result: " + oAuthAccessResult, oAuthAccessResult);

                DateTime expires = DateTime.Now.AddHours(1);
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

                accessToken = getLongLivedAccessToken(accessToken);
                expires = DateTime.Now.AddDays(59);

                me = GetMe(accessToken);
                if (me == null)
                {
                    Logger.Instance.Write(LogLevel.Error, "Cannot verify the Facebook account", new object[] { accessToken });
                    throw new Exception("Cannot verify the Facebook account");
                }

                Persist(me, accessToken, expires);
            }
        }

        public void AuthenticateForAdmin(System.Web.HttpContext context, out Me me, out string accessToken)
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
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"]; // GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri);
                url += "&scope=publish_stream,manage_pages,publish_actions,read_friendlists,manage_friendlists";
                url += "&state=" + context.Session.SessionID;
                context.Response.Redirect(url, true);
                return;
            }

            // When code is given then Facebook authentication went without errors.
            if (context.Session.SessionID == context.Request.QueryString["state"])
            {
                url = FACEBOOK_OAUTH_ACCESS;
                url += "?client_id=" + ConfigurationManager.AppSettings["Facebook.App.Id"]; //GetConfiguration("facebook.app_id");
                url += "&redirect_uri=" + HttpUtility.UrlEncode(context.Request.Url.AbsoluteUri.Replace(context.Request.Url.Query, ""));
                url += "&client_secret=" + ConfigurationManager.AppSettings["Facebook.App.Secret"];  //GetConfiguration("facebook.app_secret");
                url += "&code=" + code;
                string oAuthAccessResult = webClient.DownloadString(url);
                Logger.Instance.Write(LogLevel.Debug, "Facebook authentication result: " + oAuthAccessResult, oAuthAccessResult);

                DateTime expires = DateTime.Now.AddHours(1);
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


                accessToken = getLongLivedAccessToken(accessToken);
                expires = DateTime.Now.AddDays(59);

                me = GetMe(accessToken);
                if (me == null)
                {
                    Logger.Instance.Write(LogLevel.Error, "Cannot verify the Facebook account", new object[] { accessToken });
                    throw new Exception("Cannot verify the Facebook account");
                }

                PersistForAdmin(me, accessToken, expires);
            }
        }

        private string getLongLivedAccessToken(string accessToken)
        {
            try
            {
                FacebookClient client = new FacebookClient();

                dynamic parameters = new ExpandoObject();
                parameters.client_id = ConfigurationManager.AppSettings["Facebook.App.Id"];  //GetConfiguration("facebook.app_id");
                parameters.client_secret = ConfigurationManager.AppSettings["Facebook.App.Secret"];  //GetConfiguration("facebook.app_secret");
                parameters.grant_type = "fb_exchange_token";
                parameters.fb_exchange_token = accessToken;

                dynamic result = null;

                result = client.Post("/oauth/access_token", parameters);

                Dictionary<string, string> jsonStr = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(result));

                return Convert.ToString(jsonStr["access_token"]);
            }
            catch (Exception ex)
            {
                Log.Instance.Write(LogLevel.Error, ex);
            }

            return null;
        }

        private bool isEmailAccessible(Me me)
        {
            MeData medata = null;
            try
            {
                WebClient webClient = new WebClient();
                string rawResult = webClient.DownloadString(String.Format("https://graph.facebook.com/{0}?fields=first_name,last_name,email", me.Id));
                MeData result = JsonConvert.DeserializeObject<MeData>(rawResult); // http://james.newtonking.com/projects/json-net.aspx
                medata = result;

                if (string.IsNullOrEmpty(medata.email))
                    return false;
            }
            catch (Exception ex)
            {
                Log.Instance.Write(LogLevel.Error, ex);
            }

            return true;
        }

        /// <summary>
        /// Get Me data from Facebook
        /// </summary>
        /// <param name="accesstoken">Access token to retrieve data</param>
        /// <returns>Me result, null if not valid of found</returns>
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

        private void GetUser()
        {
            using (Database db = new MySqlDatabase())
            {
                _userInfo = db.GetUser(Util.UserId);
                _clientInfo = db.GetClientInfo(_userInfo.UserId);
            }
        }

        public string GetConfiguration(string key)
        {
            string res = null;
            using (Database db = new MySqlDatabase())
            {
                res = db.GetSetting(key);
            }
            return res;
        }
    }
}