using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackProtect.Member
{
    /// <summary>
    /// Summary description for PlayMusic
    /// </summary>
    public class PlayMusic : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/mp3";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}