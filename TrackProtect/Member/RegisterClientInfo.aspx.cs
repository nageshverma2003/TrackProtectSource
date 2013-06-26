using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using TrackProtect;

namespace TrackProtect.Member
{
    public partial class RegisterClientInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("~/");

            long userid = Util.UserId;
			
            if (!IsPostBack)
            {
                Country.Items.Clear();
				string[] countries = Util.GetCountries();
                foreach (string country in countries)
                    Country.Items.Add(new ListItem(country));

                Country.SelectedIndex = Country.Items.IndexOf(Country.Items.FindByText("Netherlands"));

                if (userid > 0)
                {
                    using (Database db = new MySqlDatabase())
                    {
                        ClientInfo ci = db.GetClientInfo(userid);
                        if (ci != null && ci.ClientId > 0)
                        {
                            LastName.Text = ci.LastName;
                            FirstName.Text = ci.FirstName;
                            AddressLine1.Text = ci.AddressLine1;
                            AddressLine2.Text = ci.AddressLine2;
                            Zipcode.Text = ci.ZipCode;
							State.Text = ci.State;
                            City.Text = ci.City;
                            Country.SelectedIndex = Country.Items.IndexOf(Country.Items.FindByText(ci.Country));
                            Telephone.Text = ci.Telephone;
                            Cellular.Text = ci.Cellular;
							AccountOwner.Text = ci.AccountOwner;
							TwitterID.Text = ci.TwitterId;
							FacebookID.Text = ci.FacebookId;
							OwnerKind.SelectedIndex = OwnerKind.Items.IndexOf(OwnerKind.Items.FindByText(ci.OwnerKind));
							CreditCardNr.Text = ci.CreditCardNr;
							CVVNr.Text = ci.CreditCardCvv;
							EmailForReceipt.Text = ci.EmailReceipt;
							Referer.Text = ci.Referer;
                        }
                    }
                }
            }
        }

        
		void RegisterUser()
		{
			long userid = Util.UserId;
	        if (userid == 0)
	        {
	            // Oops, something went wrong, report it and bail out
	            return;
	        }
        	
	        using (Database db = new MySqlDatabase())
	        {
	            string lastname = LastName.Text;
	            string firstname = FirstName.Text;
	            string addressline1 = AddressLine1.Text;
	            string addressline2 = AddressLine2.Text;
	            string zipcode = Zipcode.Text;
	        	string state = State.Text;
	            string city = City.Text;
	            string country = Country.SelectedItem.Text;
	            string telephone = Telephone.Text;
	            string cellular = Cellular.Text;
	        	string companyname = "";
	        	string accountowner = AccountOwner.Text;
	        	string twitterid = TwitterID.Text;
	        	string facebookid = FacebookID.Text;
				string soniallid = SoniallID.Text;
	        	string ownerkind = OwnerKind.SelectedItem.Text;
	        	string creditcardnr = CreditCardNr.Text;
	        	string creditcardcvv = CVVNr.Text;
	        	string emailforreceipt = EmailForReceipt.Text;
	        	string referer = Referer.Text;
	            long clientid = db.RegisterClientInfo(
	                lastname,
	                firstname,
	                addressline1,
	                addressline2,
	                zipcode,
	        		state,
	                city,
	                country,
	                telephone,
	                cellular,
	                companyname,
	                userid,
	        		accountowner,
					senacode,
					isrccode,
	        		twitterid,
	        		facebookid,
					soniallid,
	        		ownerkind,
	        		creditcardnr,
	        		creditcardcvv,
	        		emailforreceipt,
	        		referer);
	            if (clientid == 0)
	            {
	                ErrorMessage.Text = "Couldn't register your client information.";
	            }
	        	else
	        	{
	        		// We were successful, now register the user with WHMCS
	        		string compositeLastname = lastname;
	        		AddClient addClient = new AddClient();
	        		addClient.Uri = new Uri("http://administratie.trackprotect.com/includes/api.php");
	        		addClient.AddElement("user", "username");
	        		addClient.AddElement("password", "password");
	        		addClient.Method = "POST";
	        		string countryIso2 = Util.GetCountryIso2(country);
	        		string currencyIso = Util.GetCurrencyIsoNameByCountryIso2(countryIso2);
	        		addClient.Client = new WhmcsClient() {
	        			Firstname		= firstname,
	        			Lastname		= compositeLastname,
	        			Companyname		= companyname,
	        			Email			= db.GetUserEmail(userid),
	        			Address1		= addressline1,
	        			Address2		= addressline2,
	        			City			= city,
	        			State			= state,
	        			Postcode		= zipcode,
	        			Country			= countryIso2,
	        			Phonenumber		= telephone,
	        			Password		= db.GetUserPassword(userid),
	        			Currency		= Util.GetCurrencyId(currencyIso),
	        			GroupId			= string.Empty,
	        			Notes			= string.Empty,
	        			CcType			= string.Empty,
	        			Cardnumber		= string.Empty,
	        			ExpiryDate		= DateTime.MaxValue,
	        			StartDate		= DateTime.MaxValue,
	        			Issuenumber		= string.Empty,
	        			CustomFields	= string.Empty,
	        			NoEmail			= false,
	        			SkipValidation	= true,
	        			Cvv				= string.Empty,
	        			Credit			= 0m,
	        			TaxExempt		= false,
	        			Status			= string.Empty
	        		};
	        		addClient.Uri = new Uri("http://administratie.trackprotect.com/includes/api.php");
	        		addClient.Username =  "whmcs_api-04Wyy1Ge4FGJ";
	        		addClient.Password = "febuxaWreC3Spe";
	        		addClient.SetElements();
	        		XmlDocument doc = addClient.Transceive();
	        		XmlNode element = doc.SelectSingleNode ("/whmcsapi/clientid");
	        		int whmcsclientid = 0;
	        		if (element != null)
	        			whmcsclientid = Convert.ToInt32(element.InnerText);
	        		db.UpdateUserWhmcsClientId(userid, whmcsclientid);

					Response.Redirect("~/Member/SelectProduct.aspx");
        		}
			}
		}
        
		
        protected void SubmitButton_Command(object sender, CommandEventArgs e)
        {
           RegisterUser ();
        }
    }
}