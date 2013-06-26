using System.IO;
using iTextSharp.text.pdf;

namespace TrackProtect
{
    internal class CertificateDocument
    {
        string _templateFilename = string.Empty;
        public string[] Name { get; set; }
        public string CertificateId { get; set; }
        public string RegisteredTrack { get; set; }
        public string RegistrationDate { get; set; }
        public string ExpirationDate { get; set; }
        public string Collaborators { get; set; }
        public string Agent { get; set; }

        public CertificateDocument(string templateFilename)
        {
            Name = new string[2];
            Name[0] = string.Empty;
            Name[1] = string.Empty;
            Collaborators = string.Empty;
            Agent = string.Empty;

            if (!File.Exists(templateFilename))
                throw new FileNotFoundException("File doesn't exist.", templateFilename);

            _templateFilename = templateFilename;
        }

        public void GenerateCertificateDocument(string targetFilename)
        {
            PdfReader reader = new PdfReader(_templateFilename);
            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(targetFilename, FileMode.Create)))
            {
                AcroFields form = stamper.AcroFields;
                var fieldKeys = form.Fields.Keys;
                foreach (string fieldKey in fieldKeys)
                {
                    if (fieldKey.Contains("issuedto1"))
                        form.SetField(fieldKey, Name[0]);
                    if (fieldKey.Contains("issuedto2"))
                        form.SetField(fieldKey, Name[1]);
                    if (fieldKey.Contains("certificatenumber"))
                        form.SetField(fieldKey, CertificateId);
                    if (fieldKey.Contains("registeredtrack"))
                        form.SetField(fieldKey, RegisteredTrack);
                    if (fieldKey.Contains("registrationdate"))
                        form.SetField(fieldKey, RegistrationDate);
                    if (fieldKey.Contains("expirationdate"))
                        form.SetField(fieldKey, ExpirationDate);
                    if (fieldKey.Contains("collaborators"))
                        form.SetField(fieldKey, Collaborators);
                    if (fieldKey.Contains("agent"))
                        form.SetField(fieldKey, Agent);
                }
                stamper.FormFlattening = true;
            }
        }
    }
}