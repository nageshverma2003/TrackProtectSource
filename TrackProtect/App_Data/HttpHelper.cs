using System;
using System.Web.UI;
using System.Collections.Specialized;
using System.Text;

namespace TrackProtect
{
	public class HttpHelper
	{
        /// <summary>
        /// POST data and Redirect to the specified url using the specified page.
        /// </summary>
        /// <param name="page">The page which will be the referrer page.</param>
        /// <param name="destinationUrl">The destination Url to which the post
        /// and redirection is occurring.</param>
        /// <param name="data">The data should be posted.</param>
        public static void RedirectAndPost(Page page, string destinationUrl, NameValueCollection data)
        {
            // Prepare the posting form
            string strForm = PreparePostForm(destinationUrl, data);
            
            // Add a little control to the specified page holding the post form,
            // this is to submit the posting form with the request.
            page.Controls.Add(new LiteralControl(strForm));
        }
        
        /// <summary>
        /// This method prepares a HTML form which holds all data in hidden
        /// field in addition to the form submitting script.
        /// </summary>
        /// <param name="url">The destination URL to which the post and the
        /// redirection will occur, the URL can be in the same App or outside
        /// the App.</param>
        /// <param name="data">A collection of data that will be posted to the
        /// destination URL.</param>
        /// <returns>Returns a string representation of the posting form.</returns>
        private static string PreparePostForm(string url, NameValueCollection data)
        {
            string formID = "PostForm";
            StringBuilder strForm = new StringBuilder();
            strForm.AppendFormat("<form id=\"{0}\" name=\"{0}\" action=\"{1}\" method=\"POST\">", formID, url);
                           
            foreach (string key in data)
                strForm.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", key, data[key]);
            
            strForm.Append("</form>");
            
            // Build the JavaScript which will do the posting operation
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language=\"javascript\">");
            strScript.AppendFormat("var v{0} = document.{0};", formID);
            strScript.AppendFormat("v{0}.submit();", formID);
            strScript.Append("</script>");
            
            return strForm.ToString() + strScript.ToString();
        }
	}
}

