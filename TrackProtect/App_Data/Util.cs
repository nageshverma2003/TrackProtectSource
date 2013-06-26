using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml;
using System.Web;
using Resources;
using TrackProtect.Logging;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Configuration;


namespace TrackProtect
{
    public static class Util
    {
        static CountryInfo[] _countryInfo = null;
        static DateTime _countryInfoTimestamp = DateTime.MinValue;
        static long _countryInfoSize = 0;
        static LanguageInfo[] _languageInfo = null;
        static DateTime _languageInfoTimestamp = DateTime.MinValue;
        static long _languageInfoSize = 0;

        public static Config ReadConfig()
        {
            Config cfg = new Config();
            cfg.Load(HttpContext.Current.Server.MapPath("~/Config/trackprotect.config"));
            return cfg;
        }

        public static string Shorten(this string name, int chars)
        {
            if (name.ToCharArray().Length > chars)
                return name.Substring(0, chars) + "...";

            return name;
        }

        public static long UserId
        {
            get
            {
                long userid = 0L;
                if (HttpContext.Current.Session["userid"] != null)
                    userid = Convert.ToInt32(HttpContext.Current.Session["userid"]);
                return userid;
            }
            set { HttpContext.Current.Session["userid"] = value; }
        }

        public static string[] GetCountries()
        {
            LoadCountryInfo();
            string[] countries = new string[_countryInfo.Length];
            for (int i = 0; i < _countryInfo.Length; i++)
                countries[i] = _countryInfo[i].CountryName;
            return countries;
        }

        public static CountryInfo GetCountryInfo(string countryName)
        {
            LoadCountryInfo();
            int index = FindCountry(countryName);
            if (index == -1)
                return null;
            return _countryInfo[index];
        }

        public static string GetCountryIso2(string countryName)
        {
            LoadCountryInfo();
            if (countryName.Length == 2)
                return countryName;

            int index = -1;
            if (countryName.Length == 3)
                index = FindCountryByIso3(countryName);
            else
                index = FindCountry(countryName);

            if (index > -1)
                return _countryInfo[index].CountryIso2;

            return String.Empty;
        }

        public static string GetCountryIso3(string countryName)
        {
            LoadCountryInfo();
            if (countryName.Length == 3)
                return countryName;

            int index = -1;
            if (countryName.Length == 2)
                index = FindCountryByIso2(countryName);
            else
                index = FindCountry(countryName);

            if (index > -1)
                return _countryInfo[index].CountryIso3;

            return String.Empty;
        }

        public static int GetCountryCode(string countryName)
        {
            LoadCountryInfo();
            int index = FindCountry(countryName);
            if (index > -1)
                return _countryInfo[index].CountryCode;
            return -1;
        }

        public static string GetCurrencyIsoName(string countryName)
        {
            LoadCountryInfo();
            int index = FindCountry(countryName);
            if (index > -1)
                return _countryInfo[index].CurrencyCode;
            return String.Empty;
        }

        public static string GetCurrencyIsoNameByCountryIso2(string iso2)
        {
            LoadCountryInfo();
            int index = FindCountryByIso2(iso2);
            if (index > -1)
                return _countryInfo[index].CurrencyCode;
            return String.Empty;
        }

        public static string GetCurrencyIsoNameByCountryIso3(string iso3)
        {
            LoadCountryInfo();
            int index = FindCountryByIso3(iso3);
            if (index > -1)
                return _countryInfo[index].CurrencyCode;
            return String.Empty;
        }

        public static string GetCurrencySymbolByCountry(string countryName)
        {
            string res = "&curren;";
            LoadCountryInfo();
            int index = FindCountry(countryName);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencySymbol;
                if (String.IsNullOrEmpty(res))
                    res = _countryInfo[index].CurrencyCode;
            }
            return res;
        }

        public static string GetCurrencySymbolByCountryIso2(string iso2)
        {
            string res = "&curren;";
            LoadCountryInfo();
            int index = FindCountryByIso2(iso2);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencySymbol;
                if (String.IsNullOrEmpty(res))
                    res = _countryInfo[index].CurrencyCode;
            }
            return res;
        }

        public static string GetCurrencySymbolByCountryIso3(string iso3)
        {
            string res = "&curren;";
            LoadCountryInfo();
            int index = FindCountryByIso3(iso3);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencySymbol;
                if (String.IsNullOrEmpty(res))
                    res = _countryInfo[index].CurrencyCode;
            }
            return res;
        }

        public static string GetCurrencyIsoNameByCulture(string culture)
        {
            string workCulture = "nl-NL";
            if (!String.IsNullOrEmpty(culture))
                workCulture = culture;
            string res = "EUR";
            LoadCountryInfo();
            if (workCulture.Length > 2)
                workCulture = workCulture.Substring(3);
            int index = FindCountryByIso2(workCulture);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencyCode;
                if (String.IsNullOrEmpty(res))
                    res = "EUR";
            }
            return res;
        }

        public static string GetCurrencyFormatByCulture(string culture)
        {
            string workCulture = "nl-NL";
            if (!String.IsNullOrEmpty(culture))
                workCulture = culture;
            string res = "{0} {1:N2}";
            LoadCountryInfo();
            int index = FindCountryByIso2(workCulture.Substring(3));
            if (index > -1)
            {
                res = _countryInfo[index].CurrencyFormat;
                if (String.IsNullOrEmpty(res))
                    res = "{0} {1:N2}";
            }
            return res;
        }

        public static string GetCurrencyFormatByCountry(string countryName)
        {
            string res = "{0} {1:N2}";
            LoadCountryInfo();
            int index = FindCountry(countryName);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencyFormat;
                if (String.IsNullOrEmpty(res))
                    res = "{0} {1:N2}";
            }
            return res;
        }

        public static string GetCurrencyFormatByCountryIso2(string iso2)
        {
            string res = "{0} {1:N2}";
            LoadCountryInfo();
            int index = FindCountryByIso2(iso2);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencyFormat;
                if (String.IsNullOrEmpty(res))
                    res = "{0} {1:N2}";
            }
            return res;
        }

        public static string GetCurrencyFormatByCountryIso3(string iso3)
        {
            string res = "{0} {1:N2}";
            LoadCountryInfo();
            int index = FindCountryByIso3(iso3);
            if (index > -1)
            {
                res = _countryInfo[index].CurrencyFormat;
                if (String.IsNullOrEmpty(res))
                    res = "{0} {1:N2}";
            }
            return res;
        }

        public static LanguageInfo FindLanguageInfoByEnglishName(string languageName)
        {
            LoadLanguageInfo();
            foreach (LanguageInfo li in _languageInfo)
            {
                if (li.GlobalLanguageName == languageName)
                    return li;
            }
            return null;
        }

        public static string GetLanguageCodeByEnglishName(string languageName)
        {
            LanguageInfo li = FindLanguageInfoByEnglishName(languageName);
            if (li == null)
                return "en";

            return li.LanguageCode;
        }

        public static UserInfo GetUserInfo(long userId)
        {
            using (Database db = new MySqlDatabase())
            {
                return db.GetUser(userId);
            }
        }

        public static ClientInfo GetClientInfo(long userId)
        {
            using (Database db = new MySqlDatabase())
            {
                return db.GetClientInfo(userId);
            }
        }
        public static LanguageInfo[] LanguageInfo
        {
            get
            {
                LoadLanguageInfo();
                return _languageInfo;
            }
        }

        /*
        public static string GetNewOrderID()
        {
            return string.Empty;
        }
         */

        private static int FindCountry(string countryName)
        {
            for (int i = 0; i < _countryInfo.Length; i++)
            {
                if (String.Compare(_countryInfo[i].CountryName, countryName, true) == 0)
                    return i;
            }
            return -1;
        }

        private static int FindCountryByIso2(string iso2)
        {
            for (int i = 0; i < _countryInfo.Length; i++)
            {
                if (_countryInfo[i].CountryIso2 == iso2)
                    return i;
            }
            return -1;
        }

        private static int FindCountryByIso3(string iso3)
        {
            for (int i = 0; i < _countryInfo.Length; i++)
            {
                if (_countryInfo[i].CountryIso3 == iso3)
                    return i;
            }
            return -1;
        }

        private static CountryInfo[] ReadCountryInfo()
        {
            List<CountryInfo> countryInfoList = new List<CountryInfo>();
            string country = String.Empty;
            try
            {
                CsvReader reader = new CsvReader();
                CsvLines lines = reader.Load(HttpContext.Current.Server.MapPath("~/country.info"));
                foreach (CsvLine line in lines)
                {
                    country = line.Fields[0];
                    CountryInfo ci = new CountryInfo(
                        line.Fields[0],
                        line.Fields[1],
                        line.Fields[2],
                        Convert.ToInt32(line.Fields[3]),
                        line.Fields[4],
                        line.Fields[5],
                        line.Fields[6],
                        line.Fields[7]);
                    countryInfoList.Add(ci);
                }
            }
            catch (Exception ex)
            {
                string err = country + " " + ex.Message;
            }
            return countryInfoList.ToArray();
        }

        private static LanguageInfo[] ReadLanguageInfo()
        {
            List<LanguageInfo> languageInfoList = new List<LanguageInfo>();
            try
            {
                CsvReader reader = new CsvReader();
                CsvLines lines = reader.Load(HttpContext.Current.Server.MapPath("~/language.info"));
                foreach (CsvLine line in lines)
                {
                    LanguageInfo li = new LanguageInfo(
                        line.Fields[0],
                        line.Fields[1],
                        line.Fields[2]);
                    languageInfoList.Add(li);
                }
            }
            catch (Exception)
            {
            }
            return languageInfoList.ToArray();
        }

        private static void LoadCountryInfo()
        {
            FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath("~/country.info"));

            // If not yet loaded or file on disk differs from last read load it again
            if (_countryInfo == null ||
                _countryInfoSize != fi.Length ||
                _countryInfoTimestamp != fi.CreationTime)
            {
                _countryInfoSize = fi.Length;
                _countryInfoTimestamp = fi.CreationTime;
                _countryInfo = ReadCountryInfo();
            }
        }

        private static void LoadLanguageInfo()
        {
            FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath("~/language.info"));

            // If not yet loaded or file on disk differs from last read load it again
            if (_languageInfo == null ||
                _languageInfoSize != fi.Length ||
                _languageInfoTimestamp != fi.CreationTime)
            {
                _languageInfoSize = fi.Length;
                _languageInfoTimestamp = fi.CreationTime;
                _languageInfo = ReadLanguageInfo();
            }
        }

        internal static void TestInvoice()
        {
            ProductInfo pi = new ProductInfo(1, "Product 1", "Product 1 description", 10, string.Empty);
            ProductPriceInfo ppi = new ProductPriceInfo(1, 12.5m, "EUR", "NL", 1);
            CreateInvoice(1, "OK", DateTime.Now.ToString("HHmmss"), "IDEAL", pi, ppi);
        }

        internal static string CreateInvoice(long userId, string status, string transid, string paymentmethod, ProductInfo productInfo, ProductPriceInfo ppi)
        {
            string companyName = string.Empty;
            string userPath = String.Empty;
            string password = HttpContext.Current.Session["access"] as string;
            UserInfo userInfo = null;
            ClientInfo clientInfo = null;
            using (Database db = new MySqlDatabase())
            {
                userPath = db.GetUserDocumentPath(userId, password);

                userPath = userPath.Replace("\\", "/");

                if (!Directory.Exists(userPath))
                    Directory.CreateDirectory(userPath);

                userInfo = db.GetUser(userId, password);
                clientInfo = db.GetClientInfo(userId);

                companyName = clientInfo.CompanyName;
            }
            // complete userPath with document name
            string filename = String.Format("INV{0}.pdf", transid);
            userPath = Path.Combine(userPath, filename);

            // Get the invoice template from the proper location
            string templatePath = Resource.InvoiceTemplate;
            string invoiceTemplate = HttpContext.Current.Server.MapPath(templatePath);
            try
            {
                InvoiceForm form = new InvoiceForm(invoiceTemplate);
                string culture = "nl-NL";
                if (HttpContext.Current.Session["culture"] != null)
                    culture = HttpContext.Current.Session["culture"] as string;
                CultureInfo cultureInfo = new CultureInfo(culture);

                List<string> fields = new List<string>();
                fields.Add(clientInfo.GetFullName());
                fields.Add(clientInfo.AddressLine1);
                if (!string.IsNullOrEmpty(clientInfo.AddressLine2))
                    fields.Add(clientInfo.AddressLine2);
                string tmpResidence = clientInfo.ZipCode + " " + clientInfo.City.ToUpper();
                if (!string.IsNullOrEmpty(tmpResidence))
                    fields.Add(tmpResidence);
                if (!string.IsNullOrEmpty(clientInfo.Country))
                    fields.Add(clientInfo.Country);
                while (fields.Count < 5)
                    fields.Add(" ");

                form.ClientAddress = fields.ToArray();
                form.InvoiceDate = DateTime.Now.ToString("d", cultureInfo);
                form.InvoiceNumber = transid;
                using (Database db = new MySqlDatabase())
                {
                    Transaction transaction = db.GetTransaction(Util.UserId, transid);
                    foreach (TransactionLine tl in transaction.TransactionLines)
                    {
                        form.InvoiceLines.Add(new PdfInvoiceLine()
                        {
                            Description = tl.Description,
                            Quantity = tl.Quantity,
                            UnitPrice = tl.Price,
                            VatRate = tl.VatPercentage
                        });
                    }
                }
                form.GenerateInvoice(userPath, companyName);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[CreateInvoice]");
            }

            SendInvoice(userId, userPath);

            return userPath;
        }

        private static void SendInvoice(long userId, string userPath)
        {
            UserInfo ui = null;
            ClientInfo ci = null;
            using (Database db = new MySqlDatabase())
            {
                ui = db.GetUser(userId);
                ci = db.GetClientInfo(userId);
            }

            string email = ui.Email;
            if (!String.IsNullOrEmpty(ci.EmailReceipt))
                email = ci.EmailReceipt;

            List<string> attachments = new List<string>();

            attachments.Add(userPath);
            using (TextReader rdr = new StreamReader(HttpContext.Current.Server.MapPath(Resources.Resource.tplInvoice)))
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

                body = body.Replace("{%receivingRelation%}", ci.GetFullName());
                SendEmail(new string[] { email }, null, Resource.SubjectYourInvoice, body,
                          attachments.ToArray(), 0);
            }
        }

        public static void SendRegistration(long userId, string userPath, string trackname, params string[] attachments)
        {
            UserInfo ui = null;
            ClientInfo ci = null;
            using (Database db = new MySqlDatabase())
            {
                ui = db.GetUser(userId);
                ci = db.GetClientInfo(userId);
            }

            using (TextReader rdr = new StreamReader(HttpContext.Current.Server.MapPath(Resource.tplRegistration)))
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

                body = body.Replace("{%receivingRelation%}", ci.GetFullName());
                string subject = string.Format(Resources.Resource.SubjectYourRegistration, trackname);
                SendEmail(new string[] { ui.Email }, "noreply@trackprotect.com", subject, body, attachments, userId);
            }
        }

        public static void SendEmail(string[] to, string from, string subject, string body, string[] attachments, long userId)
        {
            string emailAddr = null;
            string emailHost = null;
            string emailUser = null;
            string emailPass = null;
            using (Database db = new MySqlDatabase())
            {
                emailAddr = db.GetSetting("email.address");
                emailHost = db.GetSetting("email.host");
                emailUser = db.GetSetting("email.username");
                emailPass = db.GetSetting("email.password");
            }
            MailMessage msg = new MailMessage();
            foreach (string toAddr in to)
                msg.To.Add(toAddr);
            string emailFrom = emailAddr;
            if (!string.IsNullOrEmpty(from) && Util.VerifyEmail(from))
                emailFrom = from;
            msg.From = new MailAddress(emailFrom);
            msg.Subject = subject;
            msg.IsBodyHtml = body.ToLower().Contains("<html>");
            msg.Body = body;
            if (attachments != null)
            {
                foreach (string att in attachments)
                {
                    if (!string.IsNullOrEmpty(att))
                    {
                        string workFilename = att;
                        if (!File.Exists(workFilename))
                        {
                            if (string.IsNullOrEmpty(Path.GetDirectoryName(workFilename)))
                                if (userId >= 0)
                                    workFilename = Path.Combine(Util.GetUserPath(userId), workFilename);
                                else
                                    workFilename = Path.Combine(Util.GetUserPath(), workFilename);
                        }
                        if (File.Exists(workFilename))
                            msg.Attachments.Add(new Attachment(workFilename));
                    }
                }
            }

            try
            {
                SmtpClient smtp = new SmtpClient(emailHost);
                smtp.Port = 25;
                if (!String.IsNullOrEmpty(emailUser))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailUser, emailPass);
                }
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[SendEmail]");
            }
        }

        public static Transactions GetTransactions(long userId)
        {
            using (Database db = new MySqlDatabase())
            {
                return db.GetTransactions(userId);
            }
        }

        public enum PaperSize
        {
            A4,
            A4_LANDSCAPE,
            LETTER,
            LETTER_LANDSCAPE
        };


        private static UserInfo GetUserInfo()
        {
            UserInfo res = new UserInfo();
            using (Database db = new MySqlDatabase())
            {
                res = db.GetUser(Util.UserId);
            }
            return res;
        }

        private static string GetUserPath()
        {
            string path = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StoragePath"]))
                {
                    string drivePath = ConfigurationManager.AppSettings["StoragePath"];

                    if (!Directory.Exists(drivePath))
                        return null;

                    UserInfo ui = GetUserInfo();
                    string fullDocPath = Path.Combine(drivePath, ui.UserUid);

                    if (!Directory.Exists(fullDocPath))
                        Directory.CreateDirectory(fullDocPath);

                    return fullDocPath.Replace("\\", "/");
                }

                return null;
            }
            catch { throw; }

            //UserInfo ui = GetUserInfo();
            //string repositoryPath = string.Empty;
            //using (Database db = new MySqlDatabase())
            //{
            //    repositoryPath = db.GetSetting("repository");
            //}
            //string path = Path.Combine(repositoryPath, ui.UserUid);
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);
            //return path;
        }

        private static string GetUserPath(long userId)
        {
            string path = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StoragePath"]))
                {
                    string drivePath = ConfigurationManager.AppSettings["StoragePath"];

                    if (!Directory.Exists(drivePath))
                        return null;

                    UserInfo ui = GetUserInfo(userId);
                    string fullDocPath = Path.Combine(drivePath, ui.UserUid);

                    if (!Directory.Exists(fullDocPath))
                        Directory.CreateDirectory(fullDocPath);

                    return fullDocPath.Replace("\\", "/");
                }

                return null;
            }
            catch { throw; }

            //UserInfo ui = GetUserInfo(userId);
            //string repositoryPath = string.Empty;
            //using (Database db = new MySqlDatabase())
            //{
            //    repositoryPath = db.GetSetting("repository");
            //}
            //string path = Path.Combine(repositoryPath, ui.UserUid);
            //if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);
            //return path;
        }



        public static int GetUserCredits(long userid)
        {
            int ret = 0;
            using (Database db = new MySqlDatabase())
            {
                ret = db.GetUserCredits(userid);
            }
            return ret;
        }

        public static void TestConvertHtmlToPdf(string filename)
        {
            string DocumentFilename = Path.GetFileNameWithoutExtension(filename) + ".pdf";
            string docfilename = Path.Combine(GetUserPath(), DocumentFilename);
            string tplfilename = HttpContext.Current.Server.MapPath(Resources.Resource.CertificateTemplate);
            string template = string.Empty;
            using (StreamReader rdr = new StreamReader(tplfilename))
                template = rdr.ReadToEnd();

            // Now create the document
            try
            {
                Util.ConvertHtmlToPdf(docfilename, template, Util.PaperSize.A4_LANDSCAPE);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[TestConvertHtmlToPdf]");
            }

        }

        public static void ConvertHtmlToPdf(string documentFilename, string htmlCode, PaperSize paperSize)
        {
            string pdfpath = HttpContext.Current.Server.MapPath("var/trackprotect");
            string imagepath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + "/Images";
            Document doc = new Document();
            try
            {
                PdfWriter.GetInstance(doc, new FileStream(pdfpath + "/Images.pdf", FileMode.Create));
                doc.Open();

                doc.Add(new Paragraph("PNG"));
                Image gif = Image.GetInstance(imagepath + "/invoicelogoTP.png");
                doc.Add(gif);
            }
            catch (DocumentException dex)
            {
                HttpContext.Current.Response.Write(dex.Message);
            }
            catch (IOException ioex)
            {
                HttpContext.Current.Response.Write(ioex.Message);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }
            finally
            {
                doc.Close();
            }
        }

        public static void ShowModalDialog(string pageUrl, int width, int height)
        {
            const string script = @"
			<script type=""text/javascript"">
				window.showModalDialog('{0}',null,'status:no;center:yes;dialogWidth:{1}px;dialogHeight:{2}px;help:no;scroll:no');
			</script>	
			";
            HttpContext.Current.Response.Write(string.Format(script, pageUrl, width, height));
        }

#if false
        public static void ConvertHtmlToPdf(string documentFilename, string HTMLCode, PaperSize paperSize)
        {
            Rectangle pageSize = PageSize.A4;
            switch (paperSize)
            {
            case PaperSize.A4:
                pageSize = PageSize.A4;
                break;
            case PaperSize.A4_LANDSCAPE:
                pageSize = PageSize.A4_LANDSCAPE;
                break;
            case PaperSize.LETTER:
                pageSize = PageSize.LETTER;
                break;
            case PaperSize.LETTER_LANDSCAPE:
                pageSize = PageSize.LETTER_LANDSCAPE;
                break;
            }

            HttpContext context = HttpContext.Current;
 
            //Render PlaceHolder to temporary stream
            StringWriter stringWrite = new StringWriter();
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

            string postContent = getImage(HTMLCode);
 
            StringReader reader = new StringReader(postContent);
 
            //Create PDF document
            Document doc = new Document(pageSize);
            HTMLWorker parser = new HTMLWorker(doc);
            PdfWriter.GetInstance(doc, new FileStream(documentFilename, FileMode.Create));
            doc.Open();
            try
            {
                parser.Parse(reader);
                parser.EndDocument();
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[Util.ConvertHtmlToPdf]");
                // Display parser errors in PDF.
                Paragraph paragraph = new Paragraph("Error!" + ex.Message);
                Chunk text = paragraph.Chunks[0] as Chunk;
                if (text != null)
                    text.Font.Color = BaseColor.RED;
                doc.Add(paragraph);
            }
            finally
            {
                parser.Close();
                doc.Close();
            }
        }
#endif

        //handle Image relative and absolute URL's
        public class ImageHander : IImageProvider
        {
            public string BaseUri;

            public iTextSharp.text.Image GetImage(string src,
                IDictionary<string, string> h,
                ChainedProperties cprops,
                IDocListener doc
            )
            {
                string imgPath = src;

                if (src.ToLower().Contains("http://") == false)
                    imgPath = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + src;

                return iTextSharp.text.Image.GetInstance(imgPath);
            }
        }

        public static string getImage(string input)
        {
            if (input == null)
                return string.Empty;
            string tempInput = input;
            string pattern = @"<img(.|\n)+?>";
            string src = string.Empty;
            HttpContext context = HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.RightToLeft))
            {
                if (m.Success)
                {
                    string tempM = m.Value;
                    string pattern1 = "src=[\'|\"](.+?)[\'|\"]";
                    Regex reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match mImg = reImg.Match(m.Value);

                    if (mImg.Success)
                    {
                        src = mImg.Value.Replace("src=", "").Replace("\"", "");

                        if (src.ToLower().Contains("http://") == false)
                        {
                            //Insert new URL in img tag
                            src = "src=\"" + context.Request.Url.Scheme + "://" +
                                context.Request.Url.Authority + src + "\"";
                            try
                            {
                                tempM = tempM.Remove(mImg.Index, mImg.Length);
                                tempM = tempM.Insert(mImg.Index, src);

                                //insert new url img tag in whole html code
                                tempInput = tempInput.Remove(m.Index, m.Length);
                                tempInput = tempInput.Insert(m.Index, tempM);
                                Logger.Instance.Write(LogLevel.Debug, "Image path: {0}", src);
                            }
                            catch (Exception e)
                            {
                                Logger.Instance.Write(LogLevel.Error, e, "[Util.getImage]");
                            }
                        }
                    }
                }
            }
            return tempInput;
        }

        static string getSrc(string input)
        {
            string pattern = "src=[\'|\"](.+?)[\'|\"]";
            System.Text.RegularExpressions.Regex reImg = new System.Text.RegularExpressions.Regex(pattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Match mImg = reImg.Match(input);
            if (mImg.Success)
            {
                return mImg.Value.Replace("src=", "").Replace("\"", ""); ;
            }

            return string.Empty;
        }

        public static void GetUserClearanceLevels(long userId, out int vcl, out int ecl)
        {
            vcl = 0;
            ecl = 0;
            using (Database db = new MySqlDatabase())
            {
                UserInfo ui = db.GetUser(userId);
                if (string.IsNullOrEmpty(ui.Comment))
                    return;

                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(ui.Comment);
                    XmlNodeList settings = doc.SelectNodes("/settings/setting");
                    foreach (XmlNode setting in settings)
                    {
                        if (setting.Attributes.Count > 0)
                        {
                            string attrName = string.Empty;
                            string attrValue = string.Empty;
                            foreach (XmlAttribute attr in setting.Attributes)
                            {
                                if (attr.Name.ToLower() == "name")
                                    attrName = attr.Value;
                                if (attr.Name.ToLower() == "value")
                                {
                                    switch (attrName.ToLower())
                                    {
                                        case "vcl": vcl = Convert.ToInt32(attr.Value); break;
                                        case "ecl": ecl = Convert.ToInt32(attr.Value); break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "[Util.GetUserClearanceLevels]");
                }
            }
        }

        public static bool VerifyEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        public static string GetEmailTemplate(string emailTemplate)
        {
            string ret = string.Empty;
            using (TextReader rdr = new StreamReader(emailTemplate))
            {
                ret = rdr.ReadToEnd();
            }
            return ret;
        }
    }

    public class CountryInfo
    {
        public string CountryName { get; set; }
        public string CountryIso2 { get; set; }
        public string CountryIso3 { get; set; }
        public int CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyFormat { get; set; }
        public string LanguageCode { get; set; }

        public string CultureName
        {
            get { return string.Format("{0}-{1}", LanguageCode, CountryIso2); }
        }

        public CountryInfo(string countryName,
                           string iso2,
                           string iso3,
                           int code,
                           string currencycode,
                           string currencySymbol,
                           string currencyFormat,
                           string languageCode)
        {
            CountryName = countryName;
            CountryIso2 = iso2;
            CountryIso3 = iso3;
            CountryCode = code;
            CurrencyCode = currencycode;
            CurrencySymbol = currencySymbol;
            CurrencyFormat = currencyFormat;
            LanguageCode = languageCode;
        }
    }

    public class LanguageInfo
    {
        public string GlobalLanguageName { get; set; }
        public string LocalLanguageName { get; set; }
        public string LanguageCode { get; set; }

        public LanguageInfo(string globalLanguageName, string localLanguageName, string languageCode)
        {
            GlobalLanguageName = globalLanguageName;
            LocalLanguageName = localLanguageName;
            LanguageCode = languageCode;
        }
    }
}


