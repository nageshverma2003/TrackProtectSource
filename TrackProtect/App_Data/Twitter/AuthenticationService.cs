using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using TrackProtect.Logging;
using Twitterizer;
using System.Configuration;

namespace TrackProtect.Twitter
{
    public class AuthenticationService
    {
        private UserInfo _userInfo;
        private ClientInfo _clientInfo;

        public void Persist(
            OAuthTokenResponse me,
            string oauth_token,
            string oauth_verifier)
        {
            if (_clientInfo == null)
                GetUser();

            using (Database db = new MySqlDatabase())
            {
                ClientInfo ci = db.GetClientInfo(Util.UserId);

                db.UpdateSocialCredential(ci.ClientId, SocialConnector.Twitter, "twitterid", Convert.ToString(me.UserId));
                db.UpdateSocialCredential(ci.ClientId, SocialConnector.Twitter, "oauthtoken", oauth_token);
                db.UpdateSocialCredential(ci.ClientId, SocialConnector.Twitter, "oauthverifier", oauth_verifier);

                _clientInfo.TwitterId = me.ScreenName;

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


        public bool TryAuthenticate(out string oauthtoken, out string oauthverifier)
        {
            if (_clientInfo == null) GetUser();

            oauthtoken = null;
            oauthverifier = null;

            using (Database db = new MySqlDatabase())
            {
                oauthtoken = db.GetSocialCredential(_clientInfo.ClientId, SocialConnector.Twitter, "oauthtoken");
                if (String.IsNullOrEmpty(oauthtoken))
                    return false;

                oauthverifier = db.GetSocialCredential(_clientInfo.ClientId, SocialConnector.Twitter, "oauthverifier");
                if (String.IsNullOrEmpty(oauthverifier))
                    return false;
            }

            if (IsTwitterAccessTokenValid(oauthtoken, oauthverifier) == false)
                return false;

            return true;
        }

        public bool IsTwitterAccessTokenValid(String _AccessToken, String _AccessTokenSecret)
        {
            OAuthTokens token = new OAuthTokens()
            {
                AccessToken = _AccessToken,
                AccessTokenSecret = _AccessTokenSecret,
                ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"]
            };

            TwitterResponse<TwitterUser> twitterResponse = TwitterAccount.VerifyCredentials(token);

            if (twitterResponse.Result == RequestResult.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetUser()
        {
            using (Database db = new MySqlDatabase())
            {
                _userInfo = db.GetUser(Util.UserId);
                _clientInfo = db.GetClientInfo(_userInfo.UserId);
            }
        }

        private string GetConfiguration(string key)
        {
            string res;
            using (Database db = new MySqlDatabase())
            {
                res = db.GetSetting(key);
            }
            return res;
        }
    }
}