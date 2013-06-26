using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

/* Provider data table layout
	Username								string
	Applicationname							string
	Email									string
	Comment									string
	Password								string
	PasswordQuestion						string
	PasswordAnswer							string
	IsApproved								short
	LastActivityDate						datetime
	LastLoginDate							datetime
	LastPasswordChangedDate					datetime
	CreationDate							datetime
	IsOnline								bool
	IsLockedOut								bool
	LastLockedOutDate						datetime
	FailedPasswordAttemptCount				int
	FailedPasswordAttemptWindowStart		datetime
	FailedPasswordAnswerAttemptCount		int
	FailedPasswordAnswerAttemptWindowStart	datetime
*/

namespace TrackProtect
{
	public sealed class TrackProtectMembershipProvider : MembershipProvider
	{
	    const int NEW_PASSWORD_LENGTH			= 8;
	    const string EVENT_SOURCE				= "TrackProtectMembershipProvider";
	    const string EVENT_LOG				= "Application";
	    const string EXCEPTION_MESSAGE		= "An exception occurred. Please check the Event Log.";

        MachineKeySection			_machineKey;
	    private string				_connectionString;
        string						_applicationName;
        bool						_enablePasswordReset;
        bool						_enablePasswordRetrieval;
        bool						_requiresQuestionAndAnswer;
        bool						_requiresUniqueEmail;
        int							_maxInvalidPasswordAttempts;
        int							_passwordAttemptWindow;
        MembershipPasswordFormat	_passwordFormat;
        int							_minRequiredNonAlphanumericCharacters;
        int							_minRequiredPasswordLength;
        string						_passwordStrengthRegularExpression;

		
		public bool WriteExceptionsToEventLog { get; set; }
		
		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null)
				throw new ArgumentNullException("config");
				
			if (string.IsNullOrEmpty(name))
				name = "TrackProtectMembershipProvider";
				
			if (string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "TrackProtect Membership provider");
			}
			
			base.Initialize(name, config);
			
			_applicationName						= GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			_maxInvalidPasswordAttempts				= Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
			_passwordAttemptWindow					= Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
			_minRequiredNonAlphanumericCharacters	= Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"));
			_minRequiredPasswordLength				= Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
			_passwordStrengthRegularExpression		= Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
			_enablePasswordReset					= Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
			_enablePasswordRetrieval				= Convert.ToBoolean(GetConfigValue(config["enabledPassworRetrieval"], "true"));
			_requiresQuestionAndAnswer				= Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
			_requiresUniqueEmail					= Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
			WriteExceptionsToEventLog				= Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));
			
			string tempFormat = config["passwordFormat"];
			if (string.IsNullOrEmpty(tempFormat))
				tempFormat = "Hashed";
				
			switch (tempFormat)
			{
			case "Hashed":
				_passwordFormat = MembershipPasswordFormat.Hashed;
				break;
			case "Encrypted":
				_passwordFormat = MembershipPasswordFormat.Encrypted;
				break;
			case "Clear":
				_passwordFormat = MembershipPasswordFormat.Clear;
				break;
			default:
				throw new ProviderException("Password format not supported.");
			}
			
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
                throw new ProviderException("Connection string cannot be blank.");

            _connectionString = connectionStringSettings.ConnectionString;

            Configuration cfg = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

			_machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");
			
			if (_machineKey.ValidationKey.Contains("AutoGenerate"))
				if (_passwordFormat != MembershipPasswordFormat.Clear)
					throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
		}
		
		private string GetConfigValue(string configValue, string defaultValue)
		{
			if (string.IsNullOrEmpty(configValue))
				return defaultValue;
				
			return configValue;
		}
		
        public override string Name
        {
            get { return "TrackProtectMembershipProvider"; }
        }

		public override string ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}
		
		public override bool EnablePasswordReset
		{
			get { return _enablePasswordReset; }
		}
		
		public override bool EnablePasswordRetrieval
		{
			get { return _enablePasswordRetrieval; }
		}
		
		public override bool RequiresQuestionAndAnswer
		{
			get { return _requiresQuestionAndAnswer; }
		}
		
		public override bool RequiresUniqueEmail
		{
			get { return _requiresUniqueEmail; }
		}
		
		public override int MaxInvalidPasswordAttempts
		{
			get { return _maxInvalidPasswordAttempts; }
		}
		
		public override int PasswordAttemptWindow
		{
			get { return _passwordAttemptWindow; }
		}
		
		public override MembershipPasswordFormat PasswordFormat
		{
			get { return _passwordFormat; }
		}
		
		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return _minRequiredNonAlphanumericCharacters; }
		}
		
		public override int MinRequiredPasswordLength
		{
			get { return _minRequiredPasswordLength; }
		}

	    public override string PasswordStrengthRegularExpression
		{
			get { return _passwordStrengthRegularExpression; }
		}
		
		public override bool ChangePassword(string username, string oldPwd, string newPwd)
		{
			if (!ValidateUser(username, oldPwd))
				return false;
				
			ValidatePasswordEventArgs args =
				new ValidatePasswordEventArgs(username, newPwd, true);
				
			OnValidatingPassword(args);
			if (args.Cancel)
			{
				if (args.FailureInformation != null)
					throw args.FailureInformation;

				throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
			}

			int rowsAffected = 0;
			using (Database db = new MySqlDatabase())
			{
				rowsAffected = db.ChangePassword(username, _applicationName, newPwd);
			}
            if (rowsAffected > 0)
                return true;

			return false;	// update failed
		}

        public override bool ChangePasswordQuestionAndAnswer(string username,
			string password, string newQuestion, string newAnswer)
		{
			if (!ValidateUser(username, password))
				return false;
				
			int rowsAffected = 0;
			using (Database db = new MySqlDatabase())
			{
				rowsAffected = db.ChangePasswordQuestionAndAnswer(username, _applicationName, newQuestion, newAnswer);
			}

            return (rowsAffected > 0);
		}
		
		public override MembershipUser CreateUser(
			string username,
			string password,
			string email,
			string question,
			string answer,
			bool isApproved,
			object providerUserKey,
			out MembershipCreateStatus status)
		{
            if (string.IsNullOrEmpty(username))
                username = email;

			status = MembershipCreateStatus.ProviderError;
			ValidatePasswordEventArgs args =
				new ValidatePasswordEventArgs(username, password, true);
				
			OnValidatingPassword(args);
			
			if (args.Cancel)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}
			
			if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}
			
			MembershipUser user = GetUser(username, false);
			if (user == null)
			{
				if (providerUserKey == null)
				{
					providerUserKey = Guid.NewGuid();
				}
				else
				{
					if (!(providerUserKey is Guid))
					{
						status = MembershipCreateStatus.InvalidProviderUserKey;
						return null;
					}
				}

			    int subscriptionType = 0;
                if (HttpContext.Current.Session["subscriptiontype"] != null)
                    subscriptionType = (int)HttpContext.Current.Session["subscriptiontype"];
				
				using (Database db = new MySqlDatabase())
				{
					if (db.RegisterUser(
                            username, 
                            _applicationName, 
                            email, 
                            "", 
                            password, 
                            question, 
                            answer, 
                            subscriptionType) > 0)
						status = MembershipCreateStatus.Success;
				}

                return GetUser(username, false);
			}

            status = MembershipCreateStatus.DuplicateUserName;
			
			return null;
		}
		
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
            bool res = false;
			using (Database db = new MySqlDatabase())
			{
				res = db.DeleteUser(username, _applicationName, deleteAllRelatedData);
			}
            return res;
		}
		
		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
            totalRecords = 0;
            MembershipUserCollection users = new MembershipUserCollection();
			using (Database db = new MySqlDatabase())
			{
				totalRecords = db.GetUserCount(_applicationName);
                if (totalRecords <= 0)
                    return users;
				
	            users = db.GetAllUsers(Name, _applicationName, pageIndex, pageSize);
			}
			
            return users;
        }
		
		public override int GetNumberOfUsersOnline()
		{
            int numOnline = 0;
			using (Database db = new MySqlDatabase())
			{
				numOnline = db.GetNumberOfUsersOnline(_applicationName);
			}
			return numOnline;
        }
		
		public override string GetPassword(string username, string answer)
		{
            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password retrieval not enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve hashed passwords.");

			string password = null;
			string passwordAnswer = null;
			using (Database db = new MySqlDatabase())
			{
				password = db.GetPassword(username, answer, _applicationName, out passwordAnswer);
			}

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                password = UnEncodePassword(password);

            return password;
        }
		
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
            MembershipUser user = null;
			using (Database db = new MySqlDatabase())
			{
				user = db.GetUser(Name, username, _applicationName, userIsOnline);
			}
			return user;
        }
		
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
            MembershipUser u = null;
			using (Database db = new MySqlDatabase())
			{
				u = db.GetUser(Name, providerUserKey, _applicationName, userIsOnline);
			}
            return u;
        }

		public override bool UnlockUser(string username)
		{
            bool res = false;
            using (Database db = new MySqlDatabase())
            {
            	res = db.UnlockUser(username, _applicationName);
            }
            return res;
        }
		
		public override string GetUserNameByEmail(string email)
		{
			string username = string.Empty;
			using (Database db = new MySqlDatabase())
			{
				username = db.GetUserNameByEmail(email, _applicationName);
			}

            if (username == null)
                username = string.Empty;
            return username;
        }
		
		public override string ResetPassword(string username, string answer)
		{
			if (!EnablePasswordReset)
				throw new NotSupportedException("Password reset is not enabled.");
				
			if (string.IsNullOrEmpty(answer) && RequiresQuestionAndAnswer)
			{
				UpdateFailureCount(username, "passwordAnswer");
				throw new ProviderException("Password answer required for password reset.");
			}
			
			string newPassword = System.Web.Security.Membership.GeneratePassword(NEW_PASSWORD_LENGTH, MinRequiredNonAlphanumericCharacters);
			ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
			
			OnValidatingPassword(args);
			
			if (args.Cancel)
			{
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
			}

			string res = string.Empty;
			using (Database db = new MySqlDatabase())
			{
				res = db.ResetPassword(
					username, 
					_applicationName, 
					answer, newPassword, 
					PasswordAttemptWindow, 
					RequiresQuestionAndAnswer,
					PasswordFormat,
					MaxInvalidPasswordAttempts,
					this);
			}

            if (!string.IsNullOrEmpty(res))
                return newPassword;

            throw new MembershipPasswordException("User not found, or user is locked out. Password not reset.");
        }
		
		public override void UpdateUser(MembershipUser user)
		{
			using (Database db = new MySqlDatabase())
			{
				db.UpdateUser(user, _applicationName);
			}
        }
		
		public override bool ValidateUser(string username, string password)
		{
	        bool isValid = false;
	        
	        using (Database db = new MySqlDatabase())
	        {
	        	UserState us = db.VerifyUser(username, password);
	        	if (us.State >= 0)
	        	{
	        		UserInfo ui = db.GetUser(username, password);
					if (ui != null && CheckPassword(md5(password), ui.Password))
					{
						if (ui.IsApproved > 0)
						{
							isValid = true;
				            HttpContext.Current.Session["access"] = password;
				            HttpContext.Current.Session["useruid"] = ui.UserUid;
				            HttpContext.Current.Session["userid"] = ui.UserId;
				            
				            db.UpdateUserLogon(username, _applicationName);
						    string culture = "en-US";
                            ClientInfo ci = db.GetClientInfo(ui.UserId);
                            if (ci != null)
                            {
                                if (!string.IsNullOrEmpty(ci.Country) && !string.IsNullOrEmpty(ci.Language))
                                {
                                    string cultLang = Util.GetLanguageCodeByEnglishName(ci.Language);
                                    string cultCtry = Util.GetCountryIso2(ci.Country);
                                    culture = string.Format("{0}-{1}", cultLang, cultCtry);
                                }
                            }
                            if (string.IsNullOrEmpty(culture) || culture == "-")
                                culture = "en-US";

						    //HttpContext.Current.Session["culture"] = culture;
						}
					}	        		
	        	}
	        }

	        return isValid;
        }
		
		private string md5(string text)
		{
			MD5 hash = new MD5CryptoServiceProvider();
			byte[] val = hash.ComputeHash(Encoding.UTF8.GetBytes (text));
			string res = string.Empty;
			foreach (byte b in val)
				res += string.Format ("{0:x2}", b);
			return res;
		}
		
		public string EncodePassword(string password)
		{
			string encodedPassword = password;
			switch (PasswordFormat)
			{
			case MembershipPasswordFormat.Clear:
				break;
			case MembershipPasswordFormat.Encrypted:
				encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
				break;
			case MembershipPasswordFormat.Hashed:
				HMACSHA1 hash = new HMACSHA1();
				hash.Key = HexToByte(_machineKey.ValidationKey);
				encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
				break;
			default:
				throw new ProviderException("Unsupported password format.");
			}
			
			return encodedPassword;
		}
		
		public string UnEncodePassword(string encodedPassword)
		{
			string password = encodedPassword;
			
			switch (PasswordFormat)
			{
			case MembershipPasswordFormat.Clear:
				break;
			case MembershipPasswordFormat.Encrypted:
				password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
				break;
			case MembershipPasswordFormat.Hashed:
				throw new ProviderException("Cannot unencode a hashed password.");
			default:
				throw new ProviderException("Unsupported password format.");
			}
			return password;
		}
		
		private byte[] HexToByte(string hexString)
		{
			byte[] returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i*2, 2), 16);
			return returnBytes;
		}
		
		private void UpdateFailureCount(string username, string failureType)
		{
			using (Database db = new MySqlDatabase())
			{
				db.UpdateFailureCount(username, _applicationName, failureType, PasswordAttemptWindow, MaxInvalidPasswordAttempts);
			}
        }
		
		private bool CheckPassword(string password, string dbpassword)
		{
			string pass1 = password;
			string pass2 = dbpassword;
			
			switch(PasswordFormat)
			{
			case MembershipPasswordFormat.Encrypted:
				pass2 = UnEncodePassword(dbpassword);
				break;
			case MembershipPasswordFormat.Hashed:
				pass2 = EncodePassword(password);
				break;
			default:
				break;
			}
			
			return (pass1 == pass2);
		}
		
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
            MembershipUserCollection users = new MembershipUserCollection();
            using (Database db = new MySqlDatabase())
            {
            	users = db.FindUsersByName(Name, usernameToMatch, _applicationName, pageIndex, pageSize, out totalRecords);
            }
            return users;
		}
		
		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
            MembershipUserCollection users = new MembershipUserCollection();
            using (Database db = new MySqlDatabase())
            {
            	users = db.FindUsersByEmail(Name, emailToMatch, _applicationName, pageIndex, pageSize, out totalRecords);
            }
            return users;
        }
		
		private void WriteToEventLog(Exception e, string action)
		{
			EventLog log = new EventLog();
			log.Source = EVENT_SOURCE;
			log.Log = EVENT_LOG;
			
			string message = "An exception occurred communicating with the data source.\n\n";
			message += "Action: " + action + "\n\n";
			message += "Exception: " + e;
			
			log.WriteEntry(message);
		}
	}
}