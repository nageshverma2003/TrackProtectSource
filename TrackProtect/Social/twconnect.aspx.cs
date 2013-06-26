using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Twitterizer;
using TrackProtect.Twitter;
using TrackProtect.Logging;

namespace TrackProtect.Social
{
    public partial class twconnect : System.Web.UI.Page
    {
        #region Variables ------- !

        string AccessToken = string.Empty;
        string AccessTokenSecret = string.Empty;

        #endregion


        #region Events ------- !

        protected void Page_Load(object sender, EventArgs e)
        {
            //OAuthTokenResponse tokens = null;

            //AuthenticationService authentication = new AuthenticationService();

            //if (!authentication.TryAuthenticate(out AccessToken, out AccessTokenSecret))
            //{
            //    tokens = CheckAuthorization();

            //    authentication.Persist(tokens, tokens.Token, tokens.TokenSecret);
            //}
            //else
            //{
            //    Response.Redirect("~/Member/Profile.aspx");
            //}
            //if (!string.IsNullOrEmpty(tokens.Token))
            //{
            //    Response.Redirect("~/Member/Profile.aspx");
            //}

            OAuthTokenResponse tokens = null;

            AuthenticationService authentication = new AuthenticationService();

            tokens = CheckAuthorization();

            authentication.Persist(tokens, tokens.Token, tokens.TokenSecret);

            //Response.Redirect("~/Member/Profile.aspx");

        }

        #endregion


        #region Methods ------- !

        private OAuthTokenResponse CheckAuthorization()
        {
            var oauth_consumer_key = ConfigurationManager.AppSettings["ConsumerKey"];

            var oauth_consumer_secret = ConfigurationManager.AppSettings["ConsumerSecret"];


            if (String.IsNullOrEmpty(Request["oauth_token"]))
            {
                OAuthTokenResponse reqToken = OAuthUtility.GetRequestToken(oauth_consumer_key, oauth_consumer_secret, Request.Url.AbsoluteUri);

                Response.Redirect(string.Format("http://twitter.com/oauth/authorize?oauth_token={0}", reqToken.Token));
            }
            else
            {
                string requestToken = Request["oauth_token"].ToString();

                string pin = Request["oauth_verifier"].ToString();

                var tokens = OAuthUtility.GetAccessToken(

                    oauth_consumer_key,

                    oauth_consumer_secret,

                    requestToken,

                    pin);

                OAuthTokens accesstoken = new OAuthTokens()
                {
                    AccessToken = tokens.Token,

                    AccessTokenSecret = tokens.TokenSecret,

                    ConsumerKey = oauth_consumer_key,

                    ConsumerSecret = oauth_consumer_secret,
                };

                return tokens;

                //TwitterStatus.Update(accesstoken, "India clinched the first Test against Australia with a comfortable eight-wicket victory new.");
            }

            return null;
        }

        #endregion
    }
}