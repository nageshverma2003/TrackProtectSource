using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TrackProtect.SoundCloud.Net
{
    /// <summary>
    /// Common helpers.
    /// </summary>
    public class Utils : SoundCloudClient
    {
        #region Licenses

        protected const string NoRightsReserved = "no-rights-reserved";
        protected const string AllRightsReserved = "all-rights-reserved";
        protected const string CcBy = "cc-by";
        protected const string CcByNc = "cc-by-nc";
        protected const string CcByNd = "cc-by-nd";
        protected const string CcBySa = "cc-by-sa";
        protected const string CcByNcNd = "cc-by-nc-nd";
        protected const string CcByNcSa = "cc-by-nc-sa";

        #endregion

        #region Uri

        /// <summary>
        /// Returns an Uri after replacement of the format item with the corresponding string representation.
        /// </summary>
        /// 
        /// <param name="uri">Input Uri.</param>
        /// <param name="keys">Format items.</param>
        public static Uri FormatToUri(Uri uri, params object[] keys)
        {
            return new Uri(string.Format(uri.ToString(), keys));
        }

        /// <summary>
        /// Returns a Uri with authorization segment.
        /// </summary>
        /// 
        /// <param name="baseUri">Input Uri.</param>
        /// <param name="token">Token.</param>
        public static Uri AuthorizedUri(Uri baseUri, string token)
        {
            string json = ".json";

            string baseToken = "?oauth_token=" + token;

            string newUri = string.Empty;

            if (baseUri.ToString().IndexOf(json) != -1)
            {
                newUri = baseUri.ToString().Insert(baseUri.ToString().IndexOf(json) + json.Length, baseToken);
            }
            else
            {
                newUri = baseUri.ToString().Insert(baseUri.ToString().IndexOf("&"), baseToken);
            }

            return new Uri(newUri);
        }

        /// <summary>
        /// Adds query strings to a given uri.
        /// </summary>
        /// 
        /// <param name="baseUri">Input uri.</param>
        /// <param name="parameters">Dictionnary of^parameters to add.</param>
        public static Uri AddParametersToUri(Uri baseUri, Dictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder(baseUri.ToString());

            foreach (KeyValuePair<string, object> pair in parameters)
            {
                sb.AppendFormat("&{0}={1}", pair.Key, pair.Value);
            }

            return new Uri(sb.ToString());
        }

        #endregion Uri
    }}