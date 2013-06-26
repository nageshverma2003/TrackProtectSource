using System;
using System.Collections.Generic;
using System.IO;
//using System.Security.Cryptography;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using TrackProtect.Logging;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;
//using iTextSharp.tool.xml.pipeline.html;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;
using System.Text;
using System.Configuration;
//using iTextSharp.tool.xml.html;

namespace TrackProtect
{
    public class CertificateManager : IDisposable
    {
        private const string OID_SID = "1.3.6.1.4.1.37516";
        private const string OID_SID_TRACKPROTECT = OID_SID + ".1";
        private const string OID_SID_DOCUMENT_DATE = OID_SID_TRACKPROTECT + ".1";
        private const string OID_SID_DOCUMENT_NAME = OID_SID_TRACKPROTECT + ".2";
        private const string OID_SID_DOCUMENT_HASH = OID_SID_TRACKPROTECT + ".3";
        private const string OID_SID_DOCUMENT_ISRC = OID_SID_TRACKPROTECT + ".4";
        private const string OID_SID_DOCUMENT_TRCK = OID_SID_TRACKPROTECT + ".5";
        private const string OID_SID_DOCUMENT_COOP = OID_SID_TRACKPROTECT + ".6";
        private const string OID_SID_DOCUMENT_AGNT = OID_SID_TRACKPROTECT + ".7";

        // The serial number offset is a value that will be deducted from
        // the serial number timestamp value in order to provider less
        // trackable serial numbers for the certificates
        private readonly BigInteger SERIAL_NUMBER_OFFSET = new BigInteger("63461536368", 10);

        //private X509Store       	_store				= null;
        private X509Certificate2 _certRoot = null;
        private X509Certificate2 _certSign = null;
        private List<string> _documents = new List<string>();
        private List<string> _artists = new List<string>();
        private List<string> _hashes = new List<string>();
        private long _userId = 0L;
        private string _password = string.Empty;
        private UserInfo _userInfo = null;
        private ClientInfo _clientInfo = null;
        private string _pathCertificates = string.Empty;
        private string _fileCertRoot = string.Empty;
        private string _fileCertSign = string.Empty;
        private bool _disposed = false;
        private AsymmetricAlgorithm _pkSign = null;
        private string _isrcCode = string.Empty;
        private string _trackName = string.Empty;

        public string CertificateFilename { get; set; }
        public byte[] Certificate { get; set; }
        public string Agent { get; set; }

        public CertificateManager(long userId, string password)
        {

            _pathCertificates = GetConfiguration("certificatepath");
            _fileCertRoot = GetConfiguration("certificateroot");
            _fileCertSign = GetConfiguration("certificatesign");
            Certificate = null;
            _userId = userId;
            _password = password;

            //_store = new X509Store("local");
            //_store = new X509Store("trackprotect", StoreLocation.LocalMachine);
            //_store.Open(OpenFlags.ReadWrite);
            /*
            X509Certificate2 certRoot = new X509Certificate2(Path.Combine(_pathCertificates, _fileCertRoot),
                                                             "UDfPct0w3zdHEb6", X509KeyStorageFlags.Exportable);
             */
            string rootpath = ConfigurationManager.AppSettings["CetificatePath"]; //new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;

            string rootCertificate = Path.Combine(rootpath, ConfigurationManager.AppSettings["RootCertificate_Name"]);
            string rootCertificatePassword = ConfigurationManager.AppSettings["RootCertificate_Password"];
            _certRoot = new X509Certificate2(rootCertificate, rootCertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
            //_store.Add(certRoot);
            //X509Certificate2 certSign = null;
            int retries = 20;
            while (_certSign == null)
            {
                string signingCertificate = Path.Combine(rootpath, ConfigurationManager.AppSettings["SigningCertificate_Name"]);
                string signingCertificatePassword = ConfigurationManager.AppSettings["SigningCertificate_Password"];
                _certSign = new X509Certificate2(signingCertificate, signingCertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);
                try
                {
                    _pkSign = _certSign.PrivateKey;
                }
                catch (CryptographicException)
                {
                    // Key exception occured, try to reread the certificate
                    // keep doing this until the key is correct or we've tried 'retries' times
                    _certSign = null;
                    if (--retries == 0)
                        throw;
                }
                catch (Exception)
                {

                    throw;
                }
            }

            //_store.Add(certSign);
            _userInfo = GetUnauthenticatedUser();
            _clientInfo = GetClientInfo();
        }

        ~CertificateManager()
        {
            Dispose(false);
        }

        UserInfo GetUserInfo()
        {
            UserInfo res = new UserInfo();
            using (Database db = new MySqlDatabase())
            {
                res = db.GetUser(_userId);
            }
            return res;
        }

        UserInfo GetUnauthenticatedUser()
        {
            UserInfo res = new UserInfo();
            using (Database db = new MySqlDatabase())
            {
                res = db.GetUser(_userId);
            }
            return res;
        }

        ClientInfo GetClientInfo()
        {
            ClientInfo res = new ClientInfo();
            using (Database db = new MySqlDatabase())
            {
                res = db.GetClientInfo(_userId);
            }
            return res;
        }

        public void AddDocument(string document)
        {
            _documents.Add(document);
        }

        public void AddCoopArtist(string name, string role)
        {
            string coop = string.Format("{0}\x02{1}", name, role);
            _artists.Add(coop);
        }

        public void CreateCertificate(string filename)
        {
            if (_pkSign == null)
                throw new KeyNotFoundException("Private signing key not available.");

            if (string.IsNullOrEmpty(_trackName))
                _trackName = Path.GetFileNameWithoutExtension(_documents[0]);

            // Generate a new key
            var kpgen = new RsaKeyPairGenerator();
            kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));
            AsymmetricCipherKeyPair pair = kpgen.GenerateKeyPair();

            // Get the signing certificate from the store
            string subjectDN =
                "CN=TrackProtect Signing Certificate, OU=SonoGence TrackProtect, O=SonoGence, L=Amsterdam, S=NH, C=NL";
            X509Certificate certSign = null;
            X509Certificate2 crt2Sign = null;
            if (crt2Sign == null && _certRoot.Subject == subjectDN)
                crt2Sign = _certRoot;
            if (crt2Sign == null && _certSign.Subject == subjectDN)
                crt2Sign = _certSign;
            /*
            foreach (X509Certificate2 c in _store.Certificates)
            {
                if (c.Subject == subjectDN)
                {
                    crt2Sign = c;
                    break;
                }
            }
             */
            if (crt2Sign == null)
                throw new CertificateException("Couldn't find signing certificate");
            certSign = DotNetUtilities.FromX509Certificate(crt2Sign);

            DateTime now = DateTime.UtcNow;

            // Create the certificate
            X509V3CertificateGenerator gen = new X509V3CertificateGenerator();

            BigInteger serialNumberBase = BigInteger.ValueOf(DateTime.Now.Ticks / 10000000);
            gen.SetSerialNumber(serialNumberBase.Subtract(SERIAL_NUMBER_OFFSET));
            gen.SetIssuerDN(certSign.SubjectDN);
            gen.SetNotBefore(now);
            gen.SetNotAfter(now.AddYears(1));
            gen.SetSubjectDN(new X509Name(string.Format("CN={0}", _clientInfo.GetFullName())));
            gen.SetPublicKey(pair.Public);
            gen.SetSignatureAlgorithm("SHA256WithRSAEncryption");

            gen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(certSign));
            gen.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(pair.Public));
            gen.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(false));
            gen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(KeyUsage.NonRepudiation | KeyUsage.DigitalSignature));

            gen.AddExtension(OID_SID_DOCUMENT_DATE, true, new DerPrintableString(now.ToString("yyyy-MM-dd HH:mm")));
            gen.AddExtension(OID_SID_DOCUMENT_TRCK, true, new DerPrintableString(_trackName));
            if (!string.IsNullOrEmpty(_isrcCode))
                gen.AddExtension(OID_SID_DOCUMENT_ISRC, true, new DerPrintableString(_isrcCode));

            List<byte[]> hashes = new List<byte[]>();
            int i = 1;
            foreach (string document in _documents)
            {
                byte[] hash = CalculateHash(document);
                hashes.Add(hash);
                if (hash == null)
                    hash = new byte[] { 0 };
                string docNameOid = string.Format("{0}.{1}", OID_SID_DOCUMENT_NAME, i);
                string docHashOid = string.Format("{0}.{1}", OID_SID_DOCUMENT_HASH, i);
                gen.AddExtension(docNameOid, true, new DerPrintableString(Path.GetFileName(document)));
                gen.AddExtension(docHashOid, true, hash);
                ++i;
            }

            i = 1;
            foreach (string artist in _artists)
            {
                string[] parts = artist.Split('\x02');
                string coopArtsOid = string.Format("{0}.1.{1}", OID_SID_DOCUMENT_COOP, i);
                string coopRoleOid = string.Format("{0}.2.{1}", OID_SID_DOCUMENT_COOP, i);
                gen.AddExtension(coopArtsOid, true, new DerPrintableString(parts[0]));
                gen.AddExtension(coopRoleOid, true, new DerPrintableString(parts[1]));
                ++i;
            }

            if (!string.IsNullOrEmpty(Agent))
                gen.AddExtension(OID_SID_DOCUMENT_AGNT, true, new DerPrintableString(Agent));

            AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(_pkSign);
            X509Certificate cert = gen.Generate(keyPair.Private);
            CertificateFilename = filename;
            string certfilename = Path.Combine(ConfigurationManager.AppSettings["StoragePath"], Path.Combine(GetUserPath(), CertificateFilename)); //ConfigurationManager.AppSettings["StoragePath"] + "\\" + Path.Combine(GetUserPath(), CertificateFilename); //HttpContext.Current.Server.MapPath(Path.Combine(GetUserPath(), CertificateFilename));
            using (StreamWriter sw = new StreamWriter(certfilename))
            {
                PemWriter wtr = new PemWriter(sw);
                wtr.WriteObject(cert);
                sw.Close();
            }
            Certificate = cert.GetEncoded();

            string DocumentFilename = Path.GetFileNameWithoutExtension(filename) + ".pdf";
            string docfilename = Path.Combine(ConfigurationManager.AppSettings["StoragePath"], Path.Combine(GetUserPath(), DocumentFilename)); //ConfigurationManager.AppSettings["StoragePath"] + "\\" + Path.Combine(GetUserPath(), DocumentFilename); //HttpContext.Current.Server.MapPath(Path.Combine(GetUserPath(), DocumentFilename));
            string tplfilename = HttpContext.Current.Server.MapPath(Resources.Resource.CertificateTemplate);

            // Now create the document
            try
            {
                CertificateDocument doc = new CertificateDocument(tplfilename);
                string commonName = cert.SubjectDN.ToString();
                if (commonName.StartsWith("CN="))
                    commonName = commonName.Substring(3).Trim();
                StringBuilder sb = new StringBuilder();
                foreach (string coop in _artists)
                {
                    string[] parts = coop.Split('\x02');
                    if (sb.Length > 0)
                        sb.Append("\r\n");
                    sb.AppendFormat("{0} ({1})", parts[0], parts[1]);
                }

                doc.Name[0] = commonName;
                doc.RegisteredTrack = _trackName;

                string artist = string.Empty;

                foreach (string coop in _artists)
                {
                    artist = artist + coop + System.Environment.NewLine;
                }

                doc.Collaborators = artist;
                doc.CertificateId = cert.SerialNumber.ToString();
                doc.RegistrationDate = cert.NotBefore.ToString("D");
                doc.ExpirationDate = cert.NotAfter.ToString("D");
                doc.Agent = artist;
                doc.GenerateCertificateDocument(docfilename);
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[CreateCertificate]");
            }
        }

        private string ToHex(byte[] data)
        {
            string res = string.Empty;
            foreach (byte b in data)
                res += string.Format("{0:X2}", b);
            return res;
        }

        private string GetUserPath()
        {
            UserInfo ui = GetUserInfo();
            //string repositoryPath = string.Empty;
            //using (Database db = new MySqlDatabase())
            //{
            //    repositoryPath = db.GetSetting("repository");
            //}
            //string path = Path.Combine(repositoryPath, ui.UserUid);
            //string fullPath = HttpContext.Current.Server.MapPath(path);
            //if (!Directory.Exists(fullPath))
            //    Directory.CreateDirectory(fullPath);
            //return path;  

            return Convert.ToString(ui.UserUid);
        }

        private string GetConfiguration(string key)
        {
            string res = null;
            using (Database db = new MySqlDatabase())
            {
                res = db.GetSetting(key);
            }
            return res;
        }

        public static byte[] CalculateHash(string filename)
        {
            byte[] res = null;
            if (File.Exists(filename))
            {
                SHA512Managed hash = new SHA512Managed();
                using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    res = hash.ComputeHash(stream);
                }
            }
            return res;
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    _documents.Clear();
                    _hashes.Clear();
                    _userInfo = null;
                    _clientInfo = null;
                    /*
                    if (_store != null)
                    {
                        _store.Close();
                        _store = null;
                    }
                     */
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                _disposed = true;

            }
        }

        public void AddIsrcCode(string isrcCode)
        {
            _isrcCode = isrcCode;
        }

        internal void AddTrackName(string trackName)
        {
            _trackName = trackName;
        }
    }

    public class CoopArtist
    {
        public string Artist { get; set; }
        public string Role { get; set; }

        public CoopArtist(string artist, string role)
        {
            Artist = artist;
            Role = role;
        }
    }

    public class CoopArtistList : List<CoopArtist>
    {

    }
}