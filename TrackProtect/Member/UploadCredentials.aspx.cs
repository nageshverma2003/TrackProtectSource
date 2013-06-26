using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Logging;

namespace TrackProtect.Member
{
    public partial class UploadCredentials : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IncludePage(ProtectInc, Resources.Resource.incProtect);
            IncludePage(RhosMovementInc, Resources.Resource.incRhosMovement2);

            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(Util.UserId);
                ClientInfo ci = db.GetClientInfo(Util.UserId);
                DataSet ds = db.GetRegister(Util.UserId);
                int protectedTracks = ds.Tables[0].Rows.Count;

                LoggedOnTitle.Text = Resources.Resource.LoggedOnTitle;
                LoggedOnUserName.Text = string.Format("<span><b>{0}</b></span>", ci.FirstName); // ci.GetFullName());
                CreditsLiteral.Text = string.Format(Resources.Resource.spnCredits, Util.GetUserCredits(Util.UserId));
                ProtectedLiteral.Text = string.Format(Resources.Resource.spnProtected, protectedTracks);
                decimal percentComplete = 0m;
                if (Session["percentComplete"] != null)
                    percentComplete = Convert.ToDecimal(Session["percentComplete"]);
                CompletedLiteral.Text = string.Empty;
                if (percentComplete < 100)
                    CompletedLiteral.Text = string.Format(Resources.Resource.PercentComplete, percentComplete / 100m);
                ClickToLinkLiteral.Visible = (CompletedLiteral.Text != string.Empty);
            }
        }

        protected void UploadCredentialsButton_Command(object sender, CommandEventArgs e)
        {
            long userid = Util.UserId;
            if (userid <= 0)
            {
                Logger.Instance.Write(LogLevel.Warning, "No user-id known, out of context page access");
                return;
            }

            string statusInfo1 = string.Empty;
            string statusInfo2 = string.Empty;
            using (Database db = new MySqlDatabase())
            {
                string password = Session["access"] as string;
                string uploadPath = db.GetUserDocumentPath(userid, password);
                string doc1 = string.Empty, doc2 = string.Empty;
                bool failed = false;
                if (!failed && UploadFile1.HasFile)
                {
                    doc1 = UploadFile1.FileName;
                    try
                    {
                        UploadFile1.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc1)));
                        statusInfo1 = string.Format("O{0}", Path.GetFileName(doc1));
                        //statusInfo1 = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc1));
                    }
                    catch (Exception)
                    {
                        statusInfo1 = string.Format("E{0}", Path.GetFileName(doc1));
                        //statusInfo1 = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc1));
                        failed = true;
                    }
                }
                if (!failed && UploadFile2.HasFile)
                {
                    doc2 = UploadFile2.FileName;
                    try
                    {
                        UploadFile2.SaveAs(Path.Combine(uploadPath, Path.GetFileName(doc2)));
                        statusInfo2 = string.Format("O{0}", Path.GetFileName(doc2));
                        //statusInfo2 = string.Format(Resources.Resource.FileStoreSuccess, Path.GetFileName(doc2));
                    }
                    catch (Exception)
                    {
                        statusInfo2 = string.Format("E{0}", Path.GetFileName(doc2));
                        //statusInfo2 = string.Format(Resources.Resource.FileStoreFailed, Path.GetFileName(doc2));
                        failed = true;
                    }
                }
                string er1 = Uri.EscapeDataString(statusInfo1);
                string er2 = Uri.EscapeDataString(statusInfo2);
                string res = "OK";
                if (!failed)
                {
                    string f1 = string.Empty, f2 = string.Empty;
                    if (!string.IsNullOrEmpty(doc1))
                        f1 = Path.Combine(uploadPath, Path.GetFileName(doc1));
                    if (!string.IsNullOrEmpty(doc2))
                        f2 = Path.Combine(uploadPath, Path.GetFileName(doc2));
                    string cerfilename = CreateCertificate(userid, password, f1, f2, null);
                }
                else
                {
                    res = "ERR";
                }
                Response.Redirect(string.Format("~/Member/UploadCredentialResult.aspx?res={0}&t1={1}&t2={2}", res, er1, er2), false);
            }
        }

        private string CreateCertificate(long userid, string password, string doc1, string doc2, string doc3)
        {
            string ret = string.Empty;
            using (CertificateManager mgr = new CertificateManager(userid, password))
            {
                if (!string.IsNullOrEmpty(doc1))
                    mgr.AddDocument(doc1);
                if (!string.IsNullOrEmpty(doc2))
                    mgr.AddDocument(doc2);
                if (!string.IsNullOrEmpty(doc3))
                    mgr.AddDocument(doc3);
                mgr.CreateCertificate(string.Format("ID{0:D10}.cer", userid));
                ret = mgr.CertificateFilename;
            }
            return ret;
        }
    }
}