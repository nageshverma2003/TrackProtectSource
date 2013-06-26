using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using TrackProtect.Logging;
using System.Configuration;

namespace TrackProtect
{
    public static class Confirmation
    {
        public enum ConfirmationRequestResult
        {
            Success,
            Exists,
            Failed,
            AlreadyRequested
        };

        public static ConfirmationRequestResult RequestConfirmation(
            string email,
            string firstname,
            string lastname,
            string guid,
            long requestingUserId,
            int relationType,
            string language = "")
        {
            long requestedUserId = 0;
            ClientInfo requestingClientInfo = null;
            ClientInfo requestedClientInfo = null;

            using (Database db = new MySqlDatabase())
            {
                requestedUserId = db.GetUserIdByEmail(email);
                requestingClientInfo = db.GetClientInfo(requestingUserId);
                requestedClientInfo = db.GetClientInfo(requestedUserId);

                if (db.RelationExists(requestingUserId, requestedUserId, relationType))
                    return ConfirmationRequestResult.Exists;

                if (db.ConfirmationExists(requestingUserId, email))
                    return ConfirmationRequestResult.AlreadyRequested;

                db.RequestConfirmation(guid, requestingUserId, requestedUserId, email, relationType);
            }

            string fullName = requestedClientInfo.GetFullName();
            if (string.IsNullOrEmpty(fullName) || fullName == " ")
                fullName = firstname;
            if (!string.IsNullOrEmpty(fullName))
                fullName += " ";
            fullName += lastname;

            string templatePath = string.Empty;

            string subject = string.Empty;


            if (relationType == 1)
            {
                if (!string.IsNullOrEmpty(language))
                {
                    if (language.ToLower().Contains("en"))
                    {
                        templatePath = "~/Templates/confirmmgmt.tpl";

                        subject = "invites you as a managed musician";
                    }
                    else if (language.ToLower().Contains("du"))
                    {
                        templatePath = "~/Templates/nl/confirmmgmt.tpl";

                        subject = "nodigt je uit als managed muzikant";
                    }
                }
                else
                {
                    templatePath = Resources.Resource.ConfirmMgmtTemplate;

                    subject = Resources.Resource.InviteManagedArtistSubject;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(language))
                {
                    if (language.ToLower().Contains("en"))
                    {
                        templatePath = "~/Templates/confirm.tpl";

                        subject = "invites you as a co-creator";
                    }
                    else if (language.ToLower().Contains("du"))
                    {
                        templatePath = "~/Templates/nl/confirm.tpl";

                        subject = "heeft je als co-creator uitgenodigd";
                    }
                }
                else
                {
                    templatePath = Resources.Resource.ConfirmTemplate;

                    subject = Resources.Resource.InviteRelationsubject;
                }
            }


            using (TextReader rdr = new StreamReader(HttpContext.Current.Server.MapPath(templatePath)))
            {
                string body = rdr.ReadToEnd();

                string link = string.Empty;

                if (requestedUserId != 0)
                    link =
                        string.Format(ConfigurationManager.AppSettings["SiteNavigationLink"] + "/Member/Confirm.aspx?id={0}&amp;tp={1}", guid, relationType);
                else
                    link =
                        string.Format(ConfigurationManager.AppSettings["SiteNavigationLink"] +
                        "/Member/Confirm.aspx?id={0}&amp;tp={1}&requestingUserinfo={2}", guid, relationType, EncryptionClass.Encrypt(fullName));

                body = body.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                body = body.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                body = body.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                body = body.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                body = body.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                body = body.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                body = body.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                body = body.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                body = body.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                body = body.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                body = body.Replace("{%receivingRelation%}", fullName);
                body = body.Replace("{%firstname%}", requestingClientInfo.FirstName);
                body = body.Replace("{%lastname%}", requestingClientInfo.LastName);

                body = body.Replace("{%confirmlink%}", link);

                if (!string.IsNullOrEmpty(requestedClientInfo.FirstName))
                    body = body.Replace("{%firstname_invitee%}", requestedClientInfo.FirstName);
                else
                    body = body.Replace("{%firstname_invitee%}", fullName);
                body = body.Replace("{%firstname_invitor%}", requestingClientInfo.FirstName);
                body = body.Replace("{%FAQ%}", ConfigurationManager.AppSettings["SiteNavigationLink"] + "/FAQ.aspx");

                body = body.Replace("{%lastname_invitor%}", requestingClientInfo.LastName);

                try
                {
                    Util.SendEmail(new string[] { email }, "noreply@trackprotect.com", requestingClientInfo.FirstName + " " + subject, body, null, 0);
                }
                catch { return ConfirmationRequestResult.Failed; }
            }

            return ConfirmationRequestResult.Success;
        }

        public static ConfirmationResult ProcessConfirmation(string guid, int relationType)
        {
            ConfirmationResult result = ConfirmationResult.ConfirmationFailed;

            string emailRequested = string.Empty;
            string emailRequesting = string.Empty;
            ClientInfo requestingClientInfo = null;
            ClientInfo requestedClientInfo = null;

            using (Database db = new MySqlDatabase())
            {
                result = db.ProcessConfirmation(guid, relationType, out emailRequested, out emailRequesting);

                long requestedUserId = db.GetUserIdByEmail(emailRequested);
                long requestingUserId = db.GetUserIdByEmail(emailRequesting);

                requestingClientInfo = db.GetClientInfo(requestingUserId);
                requestedClientInfo = db.GetClientInfo(requestedUserId);
            }

            if (result == ConfirmationResult.Success)
            {
                string _template = string.Empty;

                if (relationType == 1)
                    _template = Resources.Resource.ConfirmRequestorTemplate;
                else
                    _template = Resources.Resource.TemplateConfirmrelationcreate;

                using (TextReader rdr = new StreamReader(HttpContext.Current.Server.MapPath(_template)))
                {
                    string body = rdr.ReadToEnd();

                    body = body.Replace("{%EmailHeaderLogo%}", ConfigurationManager.AppSettings["EmailHeaderLogo"]);
                    body = body.Replace("{%EmailmailToLink%}", ConfigurationManager.AppSettings["EmailmailToLink"]);
                    body = body.Replace("{%SiteNavigationLink%}", ConfigurationManager.AppSettings["SiteNavigationLink"]);
                    body = body.Replace("{%EmailFooterLogo%}", ConfigurationManager.AppSettings["EmailFooterLogo"]);
                    body = body.Replace("{%EmailFBlink%}", ConfigurationManager.AppSettings["EmailFBlink"]);
                    body = body.Replace("{%EmailFBLogo%}", ConfigurationManager.AppSettings["EmailFBLogo"]);
                    body = body.Replace("{%EmailTwitterLink%}", ConfigurationManager.AppSettings["EmailTwitterLink"]);
                    body = body.Replace("{%EmailTwitterLogo%}", ConfigurationManager.AppSettings["EmailTwitterLogo"]);
                    body = body.Replace("{%EmailSoundCloudLink%}", ConfigurationManager.AppSettings["EmailSoundCloudLink"]);
                    body = body.Replace("{%EmailSoundCloudLogo%}", ConfigurationManager.AppSettings["EmailSoundCloudLogo"]);

                    body = body.Replace("{%receivingRelation%}", requestingClientInfo.GetFullName());
                    body = body.Replace("{%firstname%}", requestedClientInfo.FirstName);
                    body = body.Replace("{%lastname%}", requestedClientInfo.LastName);
                    body = body.Replace("{%firstname_invitee%}", requestedClientInfo.FirstName);
                    body = body.Replace("{%firstname_invitor%}", requestingClientInfo.FirstName);
                    body = body.Replace("{%lastname_invitor%}", requestingClientInfo.LastName);
                    body = body.Replace("{%FAQ%}", ConfigurationManager.AppSettings["SiteNavigationLink"] + "/FAQ.aspx");

                    try
                    {
                        Util.SendEmail(new string[] { emailRequesting }, "noreply@trackprotect.com",
                                       Resources.Resource.ConfirmationManagedMusician, body, null, 0);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[ProcessConfirmation]");
                    }
                }
            }
            return result;
        }
    }
}

