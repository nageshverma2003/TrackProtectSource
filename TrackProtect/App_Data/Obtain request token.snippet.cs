// Step 1: obtain a request token
OAuth["consumer_key"] = ConsumerKey;
OAuth["consumer_secret"] = ConsumerSecret;
OAuthResponse requestToken = OAuth.AcquireRequestToken(
	"http://api.soundcloud.com/oauth/request_token", 
	"POST");

// Step 2: Prompt the user for permission
var url = "http://soundcloud.com/oauth/authorize?oauth_token=" + OAuth["token"];
System.Diagnostics.Process.Start(url);	// <-- in case of Desktop application


//==============================================================
string pin = string.Empty;
OAuthResponse accessToken = OAuth.AcquireAccessToken(
	"http://api.soundcloud.com/oauth/access_token", 
	"POST",
	pin);

// Step 4:
var search = "http://api.soundcloud.com/me";
var authzHeader = OAuth.GenerateAuthzHeader(search, "GET");

public string Get(Uri requestUri, string authHeaders)
{
	if (requestUri == null)
		throw new ArgumentNullException("requestUri");

	HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;

	request.Method = "GET";
	request.ServicePoint.Expect100Continue = false;
	request.ContentType = "x-www-form-urlencoded";

	// Add OAuth headers
	request.Headers["Authorization"] = authHeaders;

	HttpWebResponse response = request.GetResponse() as HttpWebResponse;

	return ReadHttpResponseString(response);
}

==============================================================
OAuth["consumer_key"] = consumerKey;
OAuth["consumer_secret"] = consumerSecret;
OAuthResponse requestToken =
	OAuth.AcquireRequestToken("http://api.soundcloud.com/oauth/request_token", "POST");

var url = "http://soundcloud.com/oauth/authorize?oauth_token=" + OAuth["token"];
Response.Redirect(url);

public void RequestAndAuthorize()
{
	var credentials = new OAuthCredentials
	{
		CallbackUrl = "http://127.0.0.1/oauth/callback/",
		ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
		ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
		Verifier = "123456",
		Type = OAuthType.RequestToken
	};
	var client = new RestClient { Authority = "https://api.linkedin.com/uas/oath", Credentials = credentials };
	var request = new RestRequest { Path = "requestToken" };
	RestResponse response = client.Request(request);

	string token = response.Content.Split('&').Where(s => s.StartsWith("oauth_token=")).Single().Split('=')[1];
	string token_secret = response.Content.Split('&').Where s => s.StartsWith("oauth_token_secret=")).Single().Split('=')[1];

	Response.Redirect("https://api.linkedin.com/uas/oauth?oauth_token=" + token);
}

public void Callback()
{
	var credentials = new OAuthCredentials
	{
		ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
		ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
		Token = token,
		TokenSecret = token_secret,
		Verifier = verifier,
		Type = OAuthType.AccessToken,
		ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
		SignatureMethod = OAuthSignatureMethod.HmacSha1,
		Version = "1.0"
	};
	var client = new RestClient 
	{ 
		Authority = "https://api.linked.com/uas/oauth", Credentials = credentials, 
		Method = WebMethod.Post
	};
	var request = new RestRequest { Path = "accessToken" };
	RestResponse response = client.Request(request);
	string content = response.Content;\
}