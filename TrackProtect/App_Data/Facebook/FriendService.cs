using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TrackProtect.Logging;

namespace TrackProtect.Facebook
{
    public class FriendService
    {
        private const string FACEBOOK_FRIENDS = "https://graph.facebook.com/{0}/friends?limit={1}&offset={2}&fields={3}&access_token={4}";

        private string _accessToken;

        /// <summary>
        /// Constructor for FriendService. Post to personal page.
        /// </summary>
        /// <param name="accessToken">Access token for authentication</param>
        public FriendService(string accessToken)
        {
            _accessToken = accessToken;
        }

        public List<Friend> Get()
        {
            WebClient webClient = new WebClient();
            string rawResult = webClient.DownloadString(String.Format(FACEBOOK_FRIENDS, "me", "5000", "0", "id,name,location,picture,birthday", _accessToken));

            // string result = "{\"data\": [{\"id\": \"510453820\",\"name\": \"Hakan Vurel\",\"installed\": true},{\"id\": \"100003817945807\",\"name\": \"Michel van Lieshout\"}], \"paging\": {\"next\": \"https://graph.facebook.com/100001465318209/friends?fields=id\u00252Cname\u00252Cinstalled&limit=5000&offset=5000\"}}";            

            JObject result = JObject.Parse(rawResult);
            List<Friend> friends = new List<Friend>();
            Friend friend;
            foreach (JToken jFriend in result["data"])
            {
                friend = new Friend();
                friend.FacebookId = jFriend["id"].Value<string>();
                friend.Name = jFriend["name"].Value<string>();                
                friends.Add(friend);
            }
            return friends;
        }
    }
}