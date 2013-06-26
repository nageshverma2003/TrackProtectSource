using System.Text;
using System.Web;
using System;

namespace TrackProtect.Facebook
{
    /// <summary>
    /// Post on a wall. Not all properties in FB API available are implemented, see source reference.
    /// http://developers.facebook.com/docs/reference/api/user/#posts
    /// </summary>
    public class PostFeed : IFacebookFeed
    {
        public string Message       { get; set; }
        public string Link          { get; set; }
        public string Picture       { get; set; }
        public string Name          { get; set; }
        public string Caption       { get; set; }
        public string Description   { get; set; }

        public string GetQueryString()
        {
            if (string.IsNullOrEmpty(Message) && string.IsNullOrEmpty(Link)) throw new Exception("One of the fields 'message' or 'link' must be used.");
            StringBuilder query = new StringBuilder("");
            if (!string.IsNullOrEmpty(Message))     query.Append("&message="        + HttpUtility.UrlEncodeUnicode(Message));
            if (!string.IsNullOrEmpty(Link))        query.Append("&link="           + HttpUtility.UrlEncodeUnicode(Link));
            if (!string.IsNullOrEmpty(Picture))     query.Append("&picture="        + HttpUtility.UrlEncodeUnicode(Picture));
            if (!string.IsNullOrEmpty(Name))        query.Append("&name="           + HttpUtility.UrlEncodeUnicode(Name));
            if (!string.IsNullOrEmpty(Caption))     query.Append("&caption="        + HttpUtility.UrlEncodeUnicode(Caption));
            if (!string.IsNullOrEmpty(Description)) query.Append("&description="    + HttpUtility.UrlEncodeUnicode(Description));
            return query.ToString();
        }
    }

    /// <summary>
    /// Status post on a wall.
    /// http://developers.facebook.com/docs/reference/api/user/#statuses
    /// </summary>
    public class StatusFeed : IFacebookFeed
    {
        public string Message       { get; set; }

        public string GetQueryString()
        {
            if (string.IsNullOrEmpty(Message)) throw new Exception("Status feed message is empty.");
            return "&message=" + HttpUtility.UrlEncodeUnicode(Message);
        }
    }

    /// <summary>
    /// Link post on a wall.
    /// http://developers.facebook.com/docs/reference/api/user/#links
    /// </summary>
    public class LinkFeed : IFacebookFeed
    {
        public string Link          { get; set; }
        public string Message       { get; set; }

        public string GetQueryString()
        {
            if (string.IsNullOrEmpty(Link)) throw new Exception("Link feed link is empty.");
            string query = "&link=" + HttpUtility.UrlEncodeUnicode(Link);
            if (!string.IsNullOrEmpty(Message)) query += "&message=" + HttpUtility.UrlEncodeUnicode(Message);
            return query;
        }
    }

    /// <summary>
    /// Interface for a Facebook feed.
    /// </summary>
    public interface IFacebookFeed
    {
        /// <summary>
        /// Returns query parameter(s). Result must always begin with a '&'.
        /// </summary>
        /// <returns>Query parameters of the feed.</returns>
        string GetQueryString();
    }
}