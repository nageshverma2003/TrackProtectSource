using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TrackProtect
{
    public static class BitlyApi
    {
        private const string apiKey = "R_db49754f018d9647b6eb871ccb510e1b";
        private const string login = "ashu7557";

        public static string GetShortUrl(string longUrl)
        {
            string url = string.Format("http://api.bit.ly/shorten?format=xml&version=2.0.1&longUrl={0}&login={1}&apiKey={2}",
                              HttpUtility.UrlEncode(longUrl), login, apiKey);
            try
            {
                System.Net.WebClient we = new System.Net.WebClient();
                string ret = we.DownloadString(url);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(ret);

                System.Xml.XmlNode xnode = doc.DocumentElement.SelectSingleNode("results/nodeKeyVal/shortUrl");
                if (!System.String.IsNullOrEmpty(xnode.InnerText))
                {
                    return xnode.InnerText;
                }
                else
                {
                    throw new Exception("Unable to shorten URL");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}