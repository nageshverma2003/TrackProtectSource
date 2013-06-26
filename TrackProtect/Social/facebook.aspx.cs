using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using TrackProtect.Logging;
using Newtonsoft.Json.Linq;
using TrackProtect.Facebook;

namespace TrackProtect.Social
{
    public partial class facebook : System.Web.UI.Page
    {
        private string _fbAccessToken;

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthenticationService fbAuthentication = new AuthenticationService();
            Me fbMe;
            string fbAccessToken = "";

            if (!fbAuthentication.TryAuthenticate(out fbMe, out fbAccessToken))
            {
                fbAuthentication.Authenticate(Context, out fbMe, out fbAccessToken);
                if (fbMe == null)
                {
                    string message = "Cannot authenticate with facebook";
                    Logger.Instance.Write(LogLevel.Warning, message, new object[] { fbAccessToken });
                    liMessage.Text = message;
                    return;
                }
            }

            _fbAccessToken = fbAccessToken;

            //FriendService friendService = new FriendService(_accessToken);
            //List<Friend> friends = friendService.Get();

            if (!IsPostBack)
            {
                AccountService accountService = new AccountService(fbAccessToken);
                List<Facebook.Account> accounts = accountService.GetAccounts();
                ddlAccounts.DataSource = accounts;
                ddlAccounts.DataTextField = "Name";
                ddlAccounts.DataValueField = "Id";
                ddlAccounts.DataBind();
            }
        }

        protected void btPost_Click(object sender, EventArgs e)
         {
            string selectedId = ddlAccounts.SelectedValue;

            Facebook.AccountService accountService = new Facebook.AccountService(_fbAccessToken);
            List<Facebook.Account> accounts = accountService.GetAccounts();
            Facebook.Account account = accounts.Where(t => t.Id == selectedId).FirstOrDefault();
            if (account == null) throw new Exception("Account not found.");

            PostFeed feed = new PostFeed();
            feed.Message = tbMessage.Text;
            feed.Description = "dasda this is a desc";
            feed.Name = "Henkie";
            feed.Picture = "http://upload.wikimedia.org/wikipedia/commons/thumb/a/a6/Chrysippos_BM_1846.jpg/200px-Chrysippos_BM_1846.jpg";
            feed.Link = "http://www.yummo.nl";
            feed.Caption = "caption dsadad";

            Facebook.PostService postService = new PostService(account);
            postService.Post(feed);
        }
    }
}