using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// Base class for authentication in order to execute api actions.
    /// </summary>
    [DataContract(IsReference = false)]
    public class SoundCloudClient
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the credentials required for authentication.
        /// </summary>
        protected SoundCloudCredentials Credentials { get; set; }

        private bool isAuthenticated = false;
        /// <summary>
        /// Gets or sets whether the user is authenticated or not.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
            set { isAuthenticated = value; }
        }

        #endregion Public Properties

        protected static SoundCloudAccessToken SoundCloudAccessToken = null;

        #region Constructors

        /// <summary>
        /// Initializes a new isntance of the <see cref="SoundCloudClient"/> class.
        /// </summary>
        protected SoundCloudClient() { }

        /// <summary>
        /// Initializes a new isntance of the <see cref="SoundCloudClient"/> class.
        /// </summary>
        /// 
        /// <param name="credentials">Required credentials for authentication.</param>
        public SoundCloudClient(SoundCloudCredentials credentials)
        {
            Credentials = credentials;
        }

        #endregion Constructors

        public SoundCloudAccessToken Authenticate()
        {
            SoundCloudAccessToken token = SoundCloudApi.ApiAction<SoundCloudAccessToken>
                (ApiCommand.UserCredentialsFlow, HttpMethod.Post, Credentials.ClientID, Credentials.ClientSecret, Credentials.UserName, Credentials.Password);

            SoundCloudAccessToken = token;

            if (token != null)
            {
                IsAuthenticated = true;
            }

            return token;
        }

        #region Events

        public delegate void EventHandler(object sender, EventArgs e);

        public static event EventHandler ApiActionExecuting;

        protected static void OnApiActionExecuting(EventArgs e)
        {
            if (ApiActionExecuting != null)
                ApiActionExecuting(null, e);
        }

        public static event EventHandler ApiActionExecuted;

        protected static void OnApiActionExecuted(SoundCloudEventArgs e)
        {
            if (ApiActionExecuted != null)
                ApiActionExecuted(null, e);
        }

        public static event EventHandler ApiActionError;

        protected static void OnApiActionError(EventArgs e)
        {
            if (ApiActionError != null)
                ApiActionError(null, e);
        }

        #endregion Events
    }
}