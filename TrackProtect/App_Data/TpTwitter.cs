using System;
using TweetSharp;

namespace TrackProtect
{
	public class TpTwitter
	{
		string _key = string.Empty;
		string _secret = string.Empty;
		TwitterService _service = null;

		public TpTwitter(string key, string secret)
		{
			_key = key;
			_secret = secret;
		}

		public ActionResult Authorize()
		{
			// Step 1: Retrieve an OAuth Request Token
			_service = new TwitterService(key, secret);
			OAuthRequestToken requestToken = _service.GetRequestToken("http://localhost:9090/AuthorizeCallback");

			// Step 2: Redirect to the OAuth Authorization URL
			Uri uri = _service.GetAuthorizationUri(requestToken);
			return new RedirectResult(uri.ToString(), false /*permanent*/);
		}

		public ActionResult AuthorizeCallback(string oauthToken, string oauthVerifier)
		{
			var requestToken = new OAuthRequestToken { Token = oauthToken };

			// Step 3: Exchange the request token for an access token
			TwitterService svc = new TwitterService(_key, _secret);
			OAuthAccessToken accessToken = svc.GetAccessToken(requestToken, oauthVerifier);

			// Step 4: User authentication using the Access Token
			svc.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);
			TwitterUser user = svc.VerifyCredentials();
			ViewModel.Message = string.Format("Your username is {0}", user.ScreenName);
			return View();
		}
	}
}