using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TrackProtect.Logging;

namespace TrackProtect.Facebook
{
    /// <summary>
    /// Provides a service for Facebook accounts
    /// </summary>
    public class AccountService
    {
        private const string FACEBOOK_ACCOUNTS = "https://graph.facebook.com/me/accounts?access_token={0}";

        private string _accessToken;

        /// <summary>
        /// Constructor for AccountService. Needs access token for authentication
        /// </summary>
        /// <param name="accessToken">Access token for authentication</param>
        public AccountService(string accessToken)
        {
            _accessToken = accessToken;
        }

        /// <summary>
        /// retrieve a list of Facebook accounts (application, pages, etc)
        /// </summary>
        /// <returns>List of accounts</returns>
        public List<Account> GetAccounts()
        {
            List<Account> accounts = new List<Account>();
            try
            {
                WebClient webClient = new WebClient();
                string rawResult = webClient.DownloadString(String.Format(FACEBOOK_ACCOUNTS, _accessToken));
                Response<List<Account>> result = JsonConvert.DeserializeObject<Response<List<Account>>>(rawResult);
                
                // Add personal page account into the list.
                Account meAccount = new Account();
                meAccount.Access_Token = _accessToken;
                meAccount.Category = "Me";
                meAccount.Id = "me";
                meAccount.Name = "Personal page";
                result.Data.Add(meAccount);

                return result.Data;
            }
            catch (Exception ex)
            {
                Log.Instance.Write(LogLevel.Error, ex);
                throw;
            }
        }
    }
}