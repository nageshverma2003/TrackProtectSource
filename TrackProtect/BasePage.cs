using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrackProtect.Logging;

namespace TrackProtect
{
    public class BasePage : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            string culture = "nl-NL";
            if (Session["culture"] != null)
                culture = Session["culture"] as string;
            else
                Session["culture"] = culture;

            try
            {
                Culture = culture;
                UICulture = culture;

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            }
            catch (CultureNotFoundException ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "[BasePage.InitializeCulture]");
            }

            base.InitializeCulture();
        }

        public void IncludePage(Literal control, string filename)
        {
            string filepath = HttpContext.Current.Server.MapPath(filename);
            using (TextReader rdr = new StreamReader(filepath))
            {
                control.Text = rdr.ReadToEnd();
            }
        }

        public void IncludePage(Literal control, string filename, ParamsDictionary parms)
        {
            string filepath = HttpContext.Current.Server.MapPath(filename);
            using (TextReader rdr = new StreamReader(filepath))
            {
                string body = rdr.ReadToEnd();
                foreach (KeyValuePair<string, string> kvp in parms)
                {
                    body = body.Replace(kvp.Key, kvp.Value);
                }
                control.Text = body;
            }
        }

        protected decimal DetermineCompletion(string userDocPath, UserInfo ui, ClientInfo ci)
        {
            decimal percentComplete = 0;

            if (!string.IsNullOrEmpty(ci.FirstName))
                percentComplete += 5;

            if (!string.IsNullOrEmpty(ci.LastName))
                percentComplete += 5;

            if (!string.IsNullOrEmpty(ci.stagename))
                percentComplete += 10;

            if (ci.Gender != 'U')
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.Birthdate.ToString()))
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.AddressLine1))
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.ZipCode))
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.City))
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.State))
                percentComplete += 10;

            if (!string.IsNullOrEmpty(ci.OwnerKind))
                percentComplete += 10;


            //if (!string.IsNullOrEmpty(ci.TwitterId))
            //    percentComplete += 10;

            //if (!string.IsNullOrEmpty(ci.FacebookId))
            //    percentComplete += 10;

            //if (!string.IsNullOrEmpty(ci.SenaCode))
            //    percentComplete += 10;

            //if (!string.IsNullOrEmpty(ci.IsrcCode))
            //    percentComplete += 10;



            /*
            if (!ci.Birthdate.Equals(new DateTime(1, 1, 1, 0, 0, 0)))
                percentComplete += 9;
             */

            string identDocPath = Path.Combine(userDocPath, string.Format("ID{0:D10}.cer", ui.UserId));
            if (File.Exists(identDocPath))
                percentComplete += 10;

            return percentComplete;
        }

        public void PurchaseProduct(long prodid, decimal amount, string desc)
        {
            int merchantId = 10000;
            string secretCode = "ABcdEFgHIJklmNOPQrSTUvwXyZ";
            Config cfg = new Config();
            cfg.Load(Server.MapPath("~/Config/icepay.config"));
            if (cfg["merchantid"] != null)
                merchantId = Convert.ToInt32(cfg["merchantid"]);
            if (cfg["secretcode"] != null)
                secretCode = cfg["secretcode"];
            long userId = Util.UserId;
            using (Database db = new MySqlDatabase())
            {
                int amountInCents = Convert.ToInt32(amount * 100);
                UserInfo ui = db.GetUser(userId);
                ClientInfo ci = db.GetClientInfo(userId);
                string langCode = Util.GetLanguageCodeByEnglishName(ci.Language);
                string countryCode = Util.GetCountryIso2(ci.Country);
                //if (string.IsNullOrEmpty(countryCode))
                countryCode = "NL";
                string currency = Util.GetCurrencyIsoName(countryCode);
                if (string.IsNullOrEmpty(currency))
                    currency = "EUR";
                // EUR, GBP, USD, KRO

                long orderId = db.CreateTransaction(userId, amount, prodid, desc);

                ICEPAY.ICEPAY ice = new ICEPAY.ICEPAY(merchantId, secretCode);
                string orderReference = string.Format("{0:D10}", orderId);
                ice.SetOrderID(orderReference);
                ice.SetReference(orderReference);
                Response.Redirect(ice.Pay(countryCode, langCode, currency, amountInCents, desc), false);
            }
        }

        public void PurchaseProduct(long prodid, decimal amount, string desc, ProductInfo[] products)
        {
            int merchantId = 10000;
            string secretCode = "ABcdEFgHIJklmNOPQrSTUvwXyZ";
            Config cfg = new Config();
            cfg.Load(Server.MapPath("~/Config/icepay.config"));
            if (cfg["merchantid"] != null)
                merchantId = Convert.ToInt32(cfg["merchantid"]);
            if (cfg["secretcode"] != null)
                secretCode = cfg["secretcode"];
            long userId = Util.UserId;
            using (Database db = new MySqlDatabase())
            {
                int amountInCents = Convert.ToInt32(amount * 100);
                UserInfo ui = db.GetUser(userId);
                ClientInfo ci = db.GetClientInfo(userId);
                //string langCode = Util.GetLanguageCodeByEnglishName(ci.Language);
                string langCode = "nl";
                //string countryCode = Util.GetCountryIso2(ci.Country);
                string countryCode = "NL";
                if (string.IsNullOrEmpty(countryCode))
                    countryCode = "NL";
                string currency = Util.GetCurrencyIsoName(countryCode);
                if (string.IsNullOrEmpty(currency))
                    currency = "EUR";
                // EUR, GBP, USD, KRO

                long orderId = db.CreateTransaction(userId, amount, prodid, desc, products);

                ICEPAY.ICEPAY ice = new ICEPAY.ICEPAY(merchantId, secretCode);
                string orderReference = string.Format("{0:D10}", orderId);
                ice.SetOrderID(orderReference);
                ice.SetReference(orderReference);
                Response.Redirect(ice.Pay(countryCode, langCode, currency, amountInCents, desc), false);
            }
        }
    }

    public class ParamsDictionary : Dictionary<string, string>
    {

    }
}