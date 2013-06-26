using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace TrackProtect.Member
{
    public partial class Download : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string filePath = string.Empty;

            #region Code1 ------- !

            //try
            //{
            //    bool blnFileDownload = false;
            //    if (!string.IsNullOrEmpty(Session["strDwnPath"].ToString()))
            //    {
            //        filePath = Session["strDwnPath"].ToString();
            //        Session["strDwnPath"] = "";
            //    }
            //    if (filePath == "") return;
            //    Response.Clear();
            //    // Clear the content of the response
            //    Response.ClearContent();
            //    Response.ClearHeaders();
            //    // Buffer response so that page is sent
            //    // after processing is complete.
            //    Response.BufferOutput = true;
            //    // Add the file name and attachment,
            //    // which will force the open/cance/save dialog to show, to the header               

            //    if (blnFileDownload == true && (File.Exists(Request.PhysicalApplicationPath + filePath) || File.Exists(Server.MapPath(filePath))))
            //    {
            //        Response.ContentType = "application/octet-stream";
            //        Response.AddHeader("Content-Disposition", "attachment; filename=" + "fileName.pdf");
            //        Response.AddHeader("Connection", "Keep-Alive");
            //        Response.ContentEncoding = Encoding.UTF8;
            //        Response.WriteFile(filePath);
            //        Response.End();
            //        HttpContext.Current.ApplicationInstance.CompleteRequest();
            //    }
            //    else
            //    {
            //        if (Request.UrlReferrer != null)
            //        {
            //            Type csType = GetType();
            //            string jsScript = "alert('File Not Found');";
            //            ScriptManager.RegisterClientScriptBlock(Page, csType, "popup", jsScript, true);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string errorMsg = ex.Message;
            //}
            #endregion

            #region Code2 ------- !

            try
            {
                string[] _splitedfilePath = Session["strDwnPath"].ToString().Split('/');
                string _fileName = _splitedfilePath[_splitedfilePath.Count() - 1];

                //System.IO.FileInfo FileName = new System.IO.FileInfo(Request.PhysicalApplicationPath + Session["strDwnPath"].ToString());
                System.IO.FileInfo FileName = new System.IO.FileInfo(Session["strDwnPath"].ToString());

                //FileStream myFile = new FileStream(Request.PhysicalApplicationPath + Session["strDwnPath"].ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                FileStream myFile = new FileStream(Session["strDwnPath"].ToString(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                BinaryReader _BinaryReader = new BinaryReader(myFile);

                Session["strDwnPath"] = null;

                try
                {
                    long startBytes = 0;

                    Response.Clear();
                    Response.Buffer = false;
                    Response.AddHeader("Accept-Ranges", "bytes");
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + _fileName);
                    Response.AddHeader("Content-Length", (FileName.Length - startBytes).ToString());
                    Response.AddHeader("Connection", "Keep-Alive");
                    Response.ContentEncoding = Encoding.UTF8;

                    //Send data
                    _BinaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    //Dividing the data in 1024 bytes package
                    int maxCount = (int)Math.Ceiling((FileName.Length - startBytes + 0.0) / 1024);

                    //Download in block of 1024 bytes
                    int i;
                    for (i = 0; i < maxCount && Response.IsClientConnected; i++)
                    {
                        Response.BinaryWrite(_BinaryReader.ReadBytes(1024));
                        Response.Flush();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Response.End();
                    _BinaryReader.Close();
                }
            }
            catch (FileNotFoundException ex)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileaccessWarning", "alert('File not found.');", true);
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileaccessWarning", "alert('Please provide access permissions to the file path.');", true);
            }
            catch (Exception ex)
            {
                string str = ex.Message.Replace("'", "\"").Replace("\r\n", string.Empty);

                System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(),
                "FileerrorWarning", "alert('" + str + "');", true);
            }
            #endregion
        }
    }
}