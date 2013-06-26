<%@ WebHandler Language="C#" Class="TrackProtect.DownloadHandler" %>

using System;
using System.Web;

namespace TrackProtect
{
	public class DownloadHandler : System.Web.IHttpHandler
	{
		
		public virtual bool IsReusable
		{
			get
			{
				return false;
			}
		}
		
		public virtual void ProcessRequest(HttpContext context)
		{
	        string file = "";
	
	        // get the file name from the querystring
	        if (context.Request.QueryString["file"] != null)
	        {
	            file = context.Request.QueryString["file"].ToString();
	        }
	
			file = file.Trim('\'');
	        //string filename = context.Server.MapPath(file);
	        string filename = file;
	        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
	
	        try
	        {
	            if (fileInfo.Exists)
	            {
	                context.Response.Clear();
	                //context.Response.AddHeader("Content-Disposition", "inline;attachment; filename=\"" + fileInfo.Name + "\"");
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + HttpUtility.UrlEncode(fileInfo.Name) + "\"");
                    context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
	                context.Response.ContentType = "application/octet-stream";
	                context.Response.TransmitFile(fileInfo.FullName);
	                context.Response.Flush();
	            }
	            else
	            {
	                throw new Exception("File not found: " + fileInfo.FullName);
	            }
	        }
	        catch (Exception ex)
	        {
	            context.Response.ContentType = "text/plain";
	            context.Response.Write(ex.Message);
	        }
	        finally
	        {
	            context.Response.End();
	        }
		}
	}
}
