using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// SoundCloud credentials required for authentication.
    /// </summary>
    public class SoundCloudCredentials
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the authorization end point.
        /// </summary>
        public string EndUserAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the user name required for authentication.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password required for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get OAuth version.
        /// </summary>
        public double OAuth { get { return 2.0; } }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCloudCredentials"/> class.
        /// </summary>
        public SoundCloudCredentials() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCloudCredentials"/> class.
        /// </summary>
        /// 
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        public SoundCloudCredentials(string clientID, string clientSecret)
        {
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCloudCredentials"/> class.
        /// </summary>
        /// 
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public SoundCloudCredentials(string clientID, string clientSecret, string userName, string password)
        {
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
            this.UserName = userName;
            this.Password = password;
        }

        #endregion Constructors
    }
}