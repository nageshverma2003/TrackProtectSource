using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using Npgsql;
using NpgsqlTypes;
using TrackProtect.Logging;

namespace TrackProtect
{
    public class PostgresDatabase : Database
    {
	    const int NEW_PASSWORD_LENGTH			= 8;
	    const string EVENT_SOURCE				= "TrackProtectMembershipProvider";
	    const string EVENT_LOG				= "Application";
	    const string EXCEPTION_MESSAGE		= "An exception occurred. Please check the Event Log.";
		
		public bool WriteExceptionsToEventLog { get; set; }
		
		private const string QRY_REGISTER_USER =
            "INSERT INTO vault.\"user\" (" +
            "\"username\", applicationname, email, \"comment\", \"password\", " +
            "passwordquestion, passwordanswer, isapproved, lastactivitydate, " +
            "lastlogindate, lastpasswordchangeddate, creationdate, isonline, " +
            "islockedout, lastlockedoutdate, failedpasswordattemptcount, " +
            "failedpasswordattemptwindowstart, failedpasswordanswerattemptcount, " +
            "failedpasswordanswerattemptwindowstart,subscriptiontype,credits,useruid" +
            ") values (" +
            "'{0}', '{1}', '{2}', '{3}', md5('{4}'), " +
            "'{5}', md5('{6}'), TRUE, current_timestamp, " +
            "current_timestamp, current_timestamp, current_timestamp, TRUE, " +
            "FALSE, current_timestamp, 0, current_timestamp, 0, " +
            "current_timestamp, {7},0,'{8}'" +
            ") RETURNING user_id";

        private const string QRY_VERIFY_USER =
            "SELECT user_id FROM vault.\"user\" WHERE username='{0}' AND \"password\"=md5('{1}')";

        private const string QRY_GET_USER_BY_NAME =
            "SELECT user_id, username, email, isapproved, islockedout, credits, useruid, password " + 
			"FROM vault.\"user\" WHERE username='{0}' AND \"password\"=md5('{1}')";

        private const string QRY_GET_USER_BY_ID =
            "SELECT user_id, username, email, isapproved, islockedout, credits, useruid, password " +
			"FROM vault.\"user\" WHERE user_id={0} AND \"password\"=md5('{1}')";

        private const string QRY_GET_USER_BY_ID_UNAUTHENTICATED =
            "SELECT user_id, username, email, isapproved, islockedout, credits, useruid, password " + 
			"FROM vault.\"user\" WHERE user_id={0}";

        private const string QRY_GET_CLIENT_INFO =
            "SELECT * FROM vault.client WHERE user_id={0}";

        private const string QRY_REGISTER_CLIENT_INFO =
            "INSERT INTO vault.client (" +
            "lastname, firstname, insertion, addressline1, addressline2, zipcode, " +
            "state, city, country, telephone, cellular, companyname, user_id" +
            ") values (" +
            "'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}',{12}" +
            ") RETURNING client_id";

        private const string QRY_UPDATE_CLIENT_INFO =
            "UPDATE vault.client SET " +
            "lastname='{0}',firstname='{1}',insertion='{2}'," +
            "addressline1='{3}',addressline2='{4}',zipcode='{5}'," + 
			"state='{6}', city='{7}',country='{8}',telephone='{9}'," +
            "cellular='{10}', companyname='{11}', user_id={12} " +
            "WHERE client_id={13}";

        private const string QRY_INSERT_REGISTRATION =
            "INSERT INTO vault.register (certificate, registrationdate, expirationdate, user_id) " +
            "VALUES ('{0}', current_timestamp, current_timestamp+interval '1 year', {1}) RETURNING register_id";

        private const string QRY_INSERT_DOCUMENT =
            "INSERT INTO vault.document (register_id, documentname, documenthash) values " +
			"({0}, '{1}', '{2}') RETURNING document_id";

        private const string QRY_DELETE_REGISTRATION = 
            "DELETE FROM vault.register WHERE register_id={0}";

        private const string QRY_DELETE_DOCUMENT = 
            "DELETE FROM vault.document WHERE register_id={0}";

        private const string QRY_GET_REGISTER =
            "SELECT * FROM vault.register WHERE user_id={0}";

        private const string QRY_DECREMENT_CREDITS = 
            "UPDATE vault.\"user\" SET credits=credits-1 WHERE user_id={0}";

		private const string QRY_UPDATE_USER_CREDITS =
			"UPDATE vault.\"user\" SET credits=credits+{1} WHERE user_id={0}";
		
        private const string QRY_GET_SETTING =
            "SELECT \"value\" FROM vault.setting WHERE key='{0}'";

	    private const string QRY_UPDATE_USER_PASSWORD =
	        "UPDATE vault.user SET password=md5('{0}'), lastpasswordchangeddate=current_timestamp " +
			"WHERE username='{1}' AND applicationname='{2}'";

	    private const string QRY_UPDATE_USER_PASSWORDQA =
	        "UPDATE vault.user SET passwordquestion='{0}', passwordanswer=md5('{1}') " +
			"WHERE username='{2}' AND applicationname='{3}'";
		
		private const string QRY_UPDATE_USER_WHMCSCLIENTID =
			"UPDATE vault.user SET whmcsclientid={1} WHERE user_id={0}";
		
	    private const string QRY_GET_USER_CREDENTIALS =
	        "SELECT passwordanswer, islockedout FROM vault.user " + 
			"WHERE username='{0}' AND applicationname='{1}'";

	    private const string QRY_UPDATE_USER_CREDENTIALS =
	        "UPDATE vault.user " +
	        "SET password=md5('{0}'), lastpasswordchangeddate=current_timestamp " +
	        "WHERE username='{1}' AND applicationname='{2}' AND islockedout=false";
		
	    private const string QRY_DELETE_USER_BY_USERNAME_AND_APPLICATION =
	        "DELETE FROM vault.\"user\" WHERE username='{0}' AND applicationname='{1}'";

	    private const string QRY_GET_USER_COUNT_BY_APPLICATION =
	        "SELECT count(*) FROM vault.user WHERE applicationname='{0}'";

        private const string QRY_GET_ALL_USER_BY_APPLICATION = 
            "SELECT user_id, username, email, passwordquestion," +
            " comment, isapproved, islockedout, creationdate, lastlogindate," +
            " lastactivitydate, lastpasswordchangeddate, lastlockedoutdate" +
            " subscriptiontype, credits, useruid" +
            " FROM vault.\"user\" WHERE applicationname='{0}' ORDER BY username ASC";

	    private const string QRY_GET_USERS_ONLINE =
	        "SELECT count(*) FROM vault.user WHERE lastactivitydate > '{0}' " +
			"AND applicationname='{1}'";

	    private const string QRY_GET_PASSWORD =
	        "SELECT password, passwordanswer, islockedout FROM vault.user " + 
			"WHERE username='{0}' AND applicationname='{1}'";

        private const string QRY_GET_USER_BY_NAME_AND_APPLICATION = 
            "SELECT user_id, username, email, passwordquestion," +
            " comment, isapproved, islockedout, creationdate, lastlogindate," +
            " lastactivitydate, lastpasswordchangeddate, lastlockedoutdate" +
            " subscriptiontype, credits, useruid" +
            " FROM vault.\"user\" WHERE username='{0}' AND applicationname='{1}'";

	    private const string QRY_GET_USER_BY_USER_ID =
	        "SELECT user_id, username, email, passwordquestion, " +
	        "comment, isapproved, islockedout, creationdate, lastlogindate, " +
	        "lastactivitydate, lastpasswordchangeddate, lastlockedoutdate " +
            " subscriptiontype, credits, useruid" +
            "FROM vault.\"user\" WHERE user_id={0}";

	    private const string QRY_UPDATE_USER_ACTIVITY =
	        "UPDATE vault.\"user\" SET lastactivitydate=current_timestamp " +
	        "WHERE username='{0}' AND applicationname='{1}'";

	    private const string QRY_UPDATE_USER_UNLOCK =
	        "UPDATE vault.\"user\" SET islockedout=false, lastlockedoutdate=current_timestamp " +
			"WHERE username=:username AND applicationname=:applicationname";

	    private const string QRY_GET_USERNAME_BY_EMAIL_AND_APPLICATIONNAME =
            "SELECT username FROM vault.\"user\" WHERE email='{0}' AND applicationname='{1}'";

	    private const string QRY_UPDATE_USER_APPROVAL =
	        "UPDATE vault.\"user\" SET email='{0}', comment='{1}', isapproved={2} " + 
			"WHERE username='{3}' AND applicationname='{4}'";

	    private const string QRY_UPDATE_USER_LOGON =
	        "UPDATE vault.\"user\" SET lastlogindate=current_timestamp " +
			"WHERE username='{0}' AND applicationname='{1}'";

	    private const string QRY_UPDATE_FAILURE_COUNT =
	        "SELECT failedpasswordattemptaount, " +
	        "  failedpasswordattemptwindowstart, " +
	        "  failedpasswordanswerattemptcount, " +
	        "  failedpasswordanswerattemptwindowstart " +
	        "  FROM vault.\"user\" " +
	        "  WHERE username='{0}' AND applicationname='{1}'";

		private const string QRY_UPDATE_USER_RESET_PASSWORD_FAILURE =
			"UPDATE vault.\"user\" " +
			"  SET failedpasswordattemptcount=1, " +
			"      failedpasswordattemptwindowstart=current_timestamp " +
			"  WHERE username='{0}' AND applicationname='{1}'";
		
		private const string QRY_UPDATE_USER_SET_PASSWORD_FAILURE =
			"UPDATE vault.\"user\" " +
			"  SET failedpasswordattemptcount={0} " +
			"  WHERE username='{1}' AND applicationname='{2}'";
		
		private const string QRY_UPDATE_USER_RESET_PASSWORDANSWER_FAILURE =
			"UPDATE vault.\"user\" " +
			"  SET failedpasswordattemptcount=1, " +
			"      failedpasswordattemptwindowstart=current_timestamp " +
			"  WHERE username='{0}' AND applicationname='{1}'";
		
		private const string QRY_UPDATE_USER_SET_PASSWORDANSWER_FAILURE =
			"UPDATE vault.\"user\" " +
			"  SET failedpasswordanswerattemptcount={0} " +
			"  WHERE username='{1}' AND applicationname='{2}'";
		
		private const string QRY_UPDATE_USER_LOCK_OUT =
			"UPDATE vault.\"user\" SET islockedout=true,lastlockedoutdate=current_timestamp " +
			"WHERE username='{0}' AND applicationname='{1}'";
		
	    private const string QRY_SELECT_USER_COUNT_BY_NAME =
	        "SELECT count(*) FROM vault.\"user\" " +
	        "WHERE username LIKE '{0}' AND applicationname = '{1}'";

	    private const string QRY_SELECT_USER_COUNT_BY_EMAIL =
	        "SELECT count(*) FROM vault.\"user\" " +
	        "WHERE email LIKE '{0}' AND applicationname='{1}'";

	    private const string QRY_GET_USER_BY_NAME_AND_APPLICATION_ORDERED = 
            "SELECT user_id, username, email, passwordquestion," +
            " comment, isapproved, islockedout, creationdate, lastlogindate," +
            " lastactivitydate, lastpasswordchangeddate, lastlockedoutdate" +
            " subscriptiontype, credits, useruid" +
            " FROM vault.\"user\" WHERE username LIKE '{0}' AND applicationname = '{1}' ORDER BY username Asc";

	    private const string QRY_GET_USER_BY_EMAIL_AND_APPLICATION_ORDERED =
	        "SELECT user_id, username, email, passwordquestion," +
	        " comment, isapproved, islockedout, creationdate, lastlogindate," +
	        " lastactivitydate, lastpasswordchangeddate, lastlockedoutdate " +
            " subscriptiontype, credits, useruid" +
            " FROM vault.\"user\" WHERE email LIKE '{0}' AND applicationname='{1}' ORDER BY username Asc";
		
		private const string QRY_GET_PRODUCTS =
			"SELECT * FROM vault.product";
		
		private const string QRY_GET_PRODUCT_BY_ID =
			"SELECT * FROM vault.product WHERE product_id={0}";
		
		private const string QRY_GET_PRODUCTPRICES_BY_PRODUCT_ID =
			"SELECT * FROM vault.productprice WHERE product_id={0}";
		
		private const string QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_EXACT =
			"SELECT * FROM vault.productprice " +
			"WHERE " +
			"product_id={0} AND iso_currency='{1}' AND iso2_country='{2}'";
		
		private const string QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_WILD =
			"SELECT * FROM vault.productprice " +
			"WHERE " +
			"product_id={0} AND " +
			"(iso_currency='{1}' AND (iso2_country='{2} OR iso2_country=''))";
		
		private const string QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE =
			"SELECT description FROM vault.productdesc " +
			"WHERE product_id={0} AND locale='{1}-{2}'";
		
		private const string QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_COUNTRY =
			"SELECT description FROM vault.productdesc " +
			"WHERE product_id={0} AND locale LIKE '__-{1}'";
		
		private bool _disposed = false;

        public PostgresDatabase()
        {
			WriteExceptionsToEventLog = false;
        }

        ~PostgresDatabase()
        {
            Dispose(false);
        }
        
        public int RegisterUser(
        	string username, 
        	string applicationname, 
        	string email, 
        	string comment, 
        	string password, 
        	string passwordquestion, 
        	string passwordanswer, 
        	int subscriptiontype)
        {
            int userId = -1;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_REGISTER_USER,
            		username,
            		applicationname,
            		email,
            		comment,
            		password,
            		passwordquestion,
            		passwordanswer,
            		subscriptiontype,
            		Guid.NewGuid().ToString("N"));
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        userId = (int)cmd.ExecuteScalar();

                    }
                    catch (NpgsqlException ex)
                    {
                        // Log the exception
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "RegisterUser<NpgsqlException>");
						userId = 0;
                    }
                    catch (Exception ex)
                    {
						Logger.Instance.Write(TrackProtect.Logging.LogLevel.Error, ex, "RegisterUser<Exception>");
                        throw;
                    }
                }
            }
            return userId;
        }

        public UserState VerifyUser(string username, string password)
        {
            UserState res = new UserState();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_VERIFY_USER,
            		username,
            		password);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt32(0);
                                res.State = 1;
                            }
                        }
                    }
                    catch (NpgsqlException ex)
                    {
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "VerifyUser<NpgsqlException>");
                    }
                    catch (Exception ex)
                    {
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "VerifyUser<Exception>");
                    }
                }
            }
            return res;
            
        }

        public UserInfo GetUser(string username, string password)
        {
            UserInfo res = new UserInfo();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_BY_NAME,
            		username,
            		password);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt32(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetBoolean(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return res;
        }

        public UserInfo GetUser(int userid, string password)
        {
            UserInfo res = new UserInfo();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_BY_ID,
            		userid,
            		password);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt32(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetBoolean(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return res;
        }

        public UserInfo GetUser(int userid)
        {
            UserInfo res = new UserInfo();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_BY_ID_UNAUTHENTICATED,
            		userid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {

                    try
                    {
                        conn.Open();
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt32(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetBoolean(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return res;
        }

        public int RegisterClientInfo(
            string lastname,
            string firstname,
            string insertion,
            string addressline1,
            string addressline2,
            string zipcode,
			string state,
            string city,
            string country,
            string telephone,
            string cellular,
            string companyname,
            int userid)
        {
            ClientInfo ci = GetClientInfo(userid);
            int clientId = 0;
            if (ci != null && ci.ClientId > 0)
                clientId = ci.ClientId;

            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
                if (clientId == 0)
                {
                	string query = string.Format(
                		QRY_REGISTER_CLIENT_INFO,
                		lastname,
                		firstname,
                		insertion,
                		addressline1,
                		addressline2,
                		zipcode,
						state,
                		city,
                		country,
                		telephone,
                		cellular,
                		companyname,
                		userid);
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        try
                        {
                            conn.Open();
                            clientId = (int) cmd.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
							Logger.Instance.Write(TrackProtect.Logging.LogLevel.Error, ex, "RegisterClientInfo<Exception>");
                            throw;
                        }
                    }
                }
                else
                {
                	string query = string.Format(
                		QRY_UPDATE_CLIENT_INFO,
                		lastname,
                		firstname,
                		insertion,
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
                		clientId);
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }
                    }
                }
            }
            return clientId;
        }

        public ClientInfo GetClientInfo(int userid)
        {
            ClientInfo res = new ClientInfo();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_CLIENT_INFO,
            		userid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.ClientId		= rdr.GetInt32(0);
                                res.LastName		= rdr.GetString(1);
                                res.FirstName		= rdr.GetString(2);
                                res.Insertion		= rdr.GetString(3);
                                res.AddressLine1	= rdr.GetString(4);
                                res.AddressLine2	= rdr.GetString(5);
                                res.ZipCode			= rdr.GetString(6);
                                res.City			= rdr.GetString(7);
								res.State			= rdr.GetString(8);
                                res.Country			= rdr.GetString(9);
                                res.Telephone		= rdr.GetString(10);
                                res.Cellular		= rdr.GetString(11);
                                res.CompanyName		= rdr.GetString(12);
                                res.UserId			= rdr.GetInt32(13);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return res;
        }

        public int CreateRegistry(int userid, string certfilename)
        {
            int registerid = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_INSERT_REGISTRATION,
            		Path.GetFileName(certfilename),
            		userid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        registerid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            return registerid;
        }

        public int RollbackRegistry(int registerid)
        {
            int rowsAffected = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_DELETE_DOCUMENT,
            		registerid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        // Remove all associated documents
                        cmd.ExecuteNonQuery();

                        // And then remove the actual registry
                        cmd.CommandText = string.Format(QRY_DELETE_REGISTRATION, registerid);
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        rowsAffected = 0;
                        throw;
                    }
                }
            }
            return rowsAffected;
        }

        public int RegisterDocument(int registerid, string documentname, string documenthash)
        {
            int documentid = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_INSERT_DOCUMENT,
            		registerid,
            		documentname,
            		documenthash);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        documentid = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return documentid;
        }

        public DataSet GetRegister(int userid)
        {
            DataSet dataSet = new DataSet("register");
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_REGISTER,
            		userid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                        {
                            da.Fill(dataSet);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dataSet;
        }
		
		public void UpdateUserCredits(object userid, object credits)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format (
					QRY_UPDATE_USER_CREDITS,
					userid,
					credits);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						conn.Open();
						cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "UpdateUserCredits<Exception>, userid: {0}, credits: {1}", userid, credits);
					}
					finally
					{
						conn.Close();
					}
				}
			}
		}

        public void DecrementCredits(int userid)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_DECREMENT_CREDITS,
            		userid);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "DecrementCredits<Exception>, userid: {0}", userid);
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

		public void UpdateUserWhmcsClientId(int userid, int whmcsclientid)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_UPDATE_USER_WHMCSCLIENTID,
					userid,
					whmcsclientid);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						conn.Open();
						cmd.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "UpdateUserWhmcsClientId<Exception>, userid: {0}, whmcsclientid: {1}", userid, whmcsclientid);
					}
					finally
					{
						conn.Close();
					}
				}
			}
		}

        public string GetUserDocumentPath(int userid, string password)
        {
            UserInfo ui = GetUser(userid, password);
            return Path.Combine(GetSetting("repository"), ui.UserUid);
        }

        public string GetSetting(string settingKey)
        {
            string ret = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_SETTING,
            		settingKey);
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        ret = (string)cmd.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                }
            }
            return ret;
        }
		
		public ProductInfoList GetProducts()
		{
			ProductInfoList res = new ProductInfoList();
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				using (NpgsqlCommand cmd = new NpgsqlCommand(QRY_GET_PRODUCTS, conn))
				{
					using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
					{
						DataTable tbl = new DataTable("product");
						try
						{
							da.Fill(tbl);
							foreach (DataRow row in tbl.Rows)
							{
								int productId		= Convert.ToInt32(row["product_id"]);
								string name			= Convert.ToString(row["name"]);
								string description	= Convert.ToString(row["description"]);
								int credits			= Convert.ToInt32(row["credits"]);
								res.Add(new ProductInfo(productId, name, description, credits));
							}
						}
						catch (NpgsqlException ex)
						{
							Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProducts<NpgsqlException>");
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			return res;
		}

		public ProductInfo GetProductById(int productid)
		{
			ProductInfo res = null;
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCT_BY_ID,
					productid);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					conn.Open();
					try
					{
						using (NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
						{
							if (reader.HasRows)
							{
								if (reader.Read())
								{
									int productId = reader.GetInt32(0);
									string name = reader.GetString(1);
									string desc = reader.GetString(2);
									int credits = reader.GetInt32 (3);
									res = new ProductInfo(productId, name, desc, credits);
								}
							}
						}
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProductById<Exception>");
					}
					finally
					{
						conn.Close();	
					}
				}
			}
			return res;
		}
		
		public ProductPriceInfoList GetProductPrices(int productid)
		{
			ProductPriceInfoList res = new ProductPriceInfoList();
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCTPRICES_BY_PRODUCT_ID,
					productid);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					DataTable tbl = new DataTable("productprice");
					using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
					{
						try
						{
							da.Fill(tbl);
							foreach (DataRow row in tbl.Rows)
							{
								int productPriceId	= Convert.ToInt32(row["productprice_id"]);
								decimal price		= Convert.ToDecimal(row["price"]);
								string isoCurreny	= Convert.ToString (row["iso_currency"]);
								string iso2Country	= Convert.ToString (row["iso2_country"]);
								int productId		= Convert.ToInt32(row["product_id"]);
								ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
								res.Add(ppi);
							}
						}
						catch (Exception ex)
						{
							Logger.Instance.Write(TrackProtect.Logging.LogLevel.Error, ex, "GetProductPrices<Exception>");
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			return res;
		}

		public ProductPriceInfoList GetProductPrices(int productid, string iso2_country)
		{
			return GetProductPrices(productid, Util.GetCurrencyIsoNameByCountryIso2(iso2_country), iso2_country);
		}
		
		public ProductPriceInfoList GetProductPrices(int productid, string iso_currency, string iso2_country)
		{
			ProductPriceInfoList res = GetProductPricesExact (productid, iso_currency, iso2_country);
			if (res.Count == 0)
				res = GetProductPricesWild (productid, iso_currency, iso2_country);
			return res;
		}

		private ProductPriceInfoList GetProductPricesExact(int productid, string iso_currency, string iso2_country)
		{
			ProductPriceInfoList res = new ProductPriceInfoList();
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_EXACT,
					productid,
					iso_currency,
					iso2_country);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					DataTable tbl = new DataTable("productprice");
					using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
					{
						try
						{
							da.Fill(tbl);
							foreach (DataRow row in tbl.Rows)
							{
								int productPriceId	= Convert.ToInt32(row["productprice_id"]);
								decimal price		= Convert.ToDecimal(row["price"]);
								string isoCurreny	= Convert.ToString (row["iso_currency"]);
								string iso2Country	= Convert.ToString (row["iso2_country"]);
								int productId		= Convert.ToInt32(row["product_id"]);
								ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
								res.Add(ppi);
							}
						}
						catch (Exception ex)
						{
							Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProductPricesExact<Exception>");
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			return res;
		}

		private ProductPriceInfoList GetProductPricesWild(int productid, string iso_currency, string iso2_country)
		{
			ProductPriceInfoList res = new ProductPriceInfoList();
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_WILD,
					productid,
					iso_currency,
					iso2_country);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					DataTable tbl = new DataTable("productprice");
					using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
					{
						try
						{
							da.Fill(tbl);
							foreach (DataRow row in tbl.Rows)
							{
								int productPriceId	= Convert.ToInt32(row["productprice_id"]);
								decimal price		= Convert.ToDecimal(row["price"]);
								string isoCurreny	= Convert.ToString (row["iso_currency"]);
								string iso2Country	= Convert.ToString (row["iso2_country"]);
								int productId		= Convert.ToInt32(row["product_id"]);
								ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
								res.Add(ppi);
							}
						}
						catch (Exception ex)
						{
							Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProductPricesWild<Exception>");
						}
						finally
						{
							conn.Close();
						}
					}
				}
			}
			return res;
		}
		
		public string GetProductDescription(int productid, string country)
		{
			string res = string.Empty;
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_COUNTRY,
					productid,
					country);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						conn.Open();
						res = cmd.ExecuteScalar() as string;
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProductDescription<NpgsqlException>");
					}
					finally
					{
						conn.Close();
					}
				}
			}
			return res;
		}
		
		public string GetProductDescription(int productid, string language, string country)
		{
			string res = string.Empty;
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE,
					productid,
					language,
					country);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						conn.Open();
						res = cmd.ExecuteScalar() as string;
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetProductDescription<NpgsqlException>");
					}
					finally
					{
						conn.Close();
					}
				}
			}
			return res;
		}

		public int ChangePassword(string username, string applicationname, string password)
		{
			int rowsAffected = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_UPDATE_USER_PASSWORD,
					password,
					username,
					applicationname);
					
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
				    try
				    {
		                conn.Open();
				        rowsAffected = cmd.ExecuteNonQuery();
				    }
				    catch (NpgsqlException ex)
				    {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "ChangePassword");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
				    }
		            finally
				    {
				        conn.Close();
				    }
				}
			}
			return rowsAffected;
		}
		
		public int ChangePasswordQuestionAndAnswer(string username, string applicationname, string question, string answer)
		{
			int rowsAffected = 0;
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_UPDATE_USER_PASSWORDQA,
					question,
					answer,
					username,
					applicationname
					);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
		            try
		            {
		                conn.Open();
		                rowsAffected = cmd.ExecuteNonQuery();
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "ChangePasswordQuestionAndAnswer");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
				}
			}
			return rowsAffected;
		}
		
		public bool DeleteUser(string username, string applicationname, bool deleteAllRelatedData)
		{
			int rowsAffected = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_DELETE_USER_BY_USERNAME_AND_APPLICATION,
					username,
					applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
		            try
		            {
			            conn.Open();
			            rowsAffected = cmd.ExecuteNonQuery();
			            if (deleteAllRelatedData)
			            {
				            // Process commands to delete all data
			            }
		            }
		            catch (NpgsqlException ex)
		            {
			            if (WriteExceptionsToEventLog)
			            {
				            WriteToEventLog(ex, "DeleteUser");
				            throw new ProviderException(EXCEPTION_MESSAGE);
			            }
			            else
			            {
				            throw ex;
			            }
		            }
		            finally
		            {
			            conn.Close();
		            }
				}
			}
			return (rowsAffected > 0);
		}
		
		public int GetUserCount(string applicationname)
		{
			int res = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_USER_COUNT_BY_APPLICATION,
					applicationname);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						res = (int)cmd.ExecuteScalar();
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetUserCount<NpgsqlException>");
					}
					finally
					{
						conn.Close();
					}
				}
			}
			return res;
		}
		
		//public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		public MembershipUserCollection GetAllUsers(string providerName, string applicationname, int pageIndex, int pageSize)
		{
			MembershipUserCollection users = new MembershipUserCollection();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_GET_ALL_USER_BY_APPLICATION,
					applicationname);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					NpgsqlDataReader reader = null;
					try
					{
						conn.Open();
		                reader = cmd.ExecuteReader();
		                int counter = 0;
		                int startIndex = pageSize * pageIndex;
		                int endIndex = startIndex + pageSize - 1;
		                while (reader.Read())
		                {
		                    if (counter >= startIndex)
		                    {
		                        MembershipUser u = GetUserFromReader(providerName, reader);
		                        users.Add(u);
		                    }
		
		                    if (counter >= endIndex)
		                        cmd.Cancel();
		                    ++counter;
		                }
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write (TrackProtect.Logging.LogLevel.Error, ex, "GetAllUsers<NpgsqlException>");
					}
					finally
					{
		                if (reader != null)
		                    reader.Close();
						conn.Close();
					}
				}
			}
			return users;
		}
		
		public int GetNumberOfUsersOnline(string applicationName)
		{
			int numOnline = 0;
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USERS_ONLINE,
            		compareTime.ToString("yyyy-MM-dd HH:mm:ss"),
            		applicationName);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            try
		            {
		                conn.Open();
		                numOnline = (int)cmd.ExecuteScalar();
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "GetNumberOfUsersOnline");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
	            }
            }
			return numOnline;
		}
		
		public string GetPassword(string username, string answer, string applicationname, out string passwordAnswer)
		{
			string password = string.Empty;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_PASSWORD,
            		username,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            passwordAnswer = string.Empty;
		            NpgsqlDataReader reader = null;
		            try
		            {
		                conn.Open();
		                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
		                if (reader.HasRows)
		                {
		                    reader.Read();
		                    if (reader.GetBoolean(2))
		                        throw new MembershipPasswordException("The supplied user is locked out.");
		                    password = reader.GetString(0);
		                    passwordAnswer = reader.GetString(1);
		                }
		                else
		                {
		                    throw new MembershipPasswordException("The supplied username is not found.");
		                }
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "GetPassword");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                if (reader != null)
		                    reader.Close();
		                conn.Close();
		            }
	            }
            }
            return password;
		}
		
		public MembershipUser GetUser(string providerName, string username, string applicationname, bool userIsOnline)
		{
            MembershipUser u = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_BY_NAME_AND_APPLICATION,
            		username,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            NpgsqlDataReader reader = null;
		
		            try
		            {
		                conn.Open();
		
		                reader = cmd.ExecuteReader();
		
		                if (reader.HasRows)
		                {
		                    reader.Read();
		                    u = GetUserFromReader(providerName, reader);
		                    if (userIsOnline)
		                    {
								query = string.Format(
									QRY_UPDATE_USER_ACTIVITY,
									username,
									applicationname
									);
		                        NpgsqlCommand updateCmd = new NpgsqlCommand(QRY_UPDATE_USER_ACTIVITY, conn);
		                        updateCmd.ExecuteNonQuery();
		                    }
		                    HttpContext.Current.Session["userid"] = u.ProviderUserKey;
		                }
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "GetUser(string, bool)");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                if (reader != null)
		                    reader.Close();
		
		                conn.Close();
		            }
	            }
            }

            return u;
		}
		
		public MembershipUser GetUser(string providerName, object providerUserKey, string applicationname, bool userIsOnline)
		{
            MembershipUser u = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_BY_USER_ID,
            		(int)providerUserKey);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            NpgsqlDataReader reader = null;
		
		            try
		            {
		                conn.Open();
		
		                reader = cmd.ExecuteReader();
		
		                if (reader.HasRows)
		                {
		                    reader.Read();
		                    u = GetUserFromReader(providerName, reader);
		                    if (userIsOnline)
		                    {
		                    	query = string.Format(
		                    		QRY_UPDATE_USER_ACTIVITY,
		                    		u.UserName,
		                    		applicationname);
		                        using (NpgsqlCommand updateCmd = new NpgsqlCommand(query, conn))
		                        {
		                        	updateCmd.ExecuteNonQuery();
		                        }
		                    }
		                    HttpContext.Current.Session["userid"] = (int)u.ProviderUserKey;
		                }
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "GetUser(object, bool)");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                if (reader != null)
		                    reader.Close();
		
		                conn.Close();
		            }
	            }
            }

            return u;
        }

		public bool UnlockUser(string username, string applicationname)
		{
			int rowsAffected = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_UPDATE_USER_UNLOCK,
            		username,
            		applicationname);
            	using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
            	{
		            try
		            {
		                conn.Open();
		                rowsAffected = cmd.ExecuteNonQuery();
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "UnlockUser");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
	            }
            }

            return (rowsAffected > 0);
		}
		
		public string GetUserNameByEmail(string email, string applicationname)
		{
            string username = string.Empty;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USERNAME_BY_EMAIL_AND_APPLICATIONNAME,
            		email,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            try
		            {
		                conn.Open();
		                username = (string)cmd.ExecuteScalar();
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "GetUserNameByEmail");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
	            }
            }

            if (username == null)
                username = string.Empty;
            return username;
        }
		
		public string ResetPassword(
			string username, 
			string applicationname, 
			string answer, 
			string newPassword, 
			int passwordAttemptWindow,
			bool requiresQuestionAndAnswer,
			MembershipPasswordFormat passwordFormat,
			int maxInvalidPasswordAttempts,
			TrackProtectMembershipProvider provider)
		{
            int rowsAffected = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_GET_USER_CREDENTIALS,
            		username,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            string passwordAnswer = string.Empty;
		            NpgsqlDataReader reader = null;
		
		            try
		            {
		                conn.Open();
		                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
		                if (reader.HasRows)
		                {
		                    reader.Read();
		                    if (reader.GetBoolean(1))
		                        throw new MembershipPasswordException("The supplied user is locked out.");
		                    passwordAnswer = reader.GetString(0);
		                }
		                else
		                {
		                    throw new MembershipPasswordException("The supplied user name is not found.");
		                }
		
		                if (requiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer, passwordFormat, provider))
		                {
		                    UpdateFailureCount(username, applicationname, "passwordAnswer", passwordAttemptWindow, maxInvalidPasswordAttempts);
		                    throw new MembershipPasswordException("Incorrect password answer");
		                }
	
						query = string.Format(
							QRY_UPDATE_USER_CREDENTIALS,
							newPassword,
							username,
							applicationname);
		                using (NpgsqlCommand updateCmd = new NpgsqlCommand(QRY_UPDATE_USER_CREDENTIALS, conn))
		                {
			                rowsAffected = updateCmd.ExecuteNonQuery();
		                }
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "ResetPassword");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                if (reader != null)
		                    reader.Close();
		                conn.Close();
		            }
	            }
            }

            if (rowsAffected > 0)
                return newPassword;

            throw new MembershipPasswordException("User not found, or user is locked out. Password not reset.");
        }
		
		public void UpdateUser(MembershipUser user, string applicationname)
		{
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_UPDATE_USER_APPROVAL,
            		user.Email,
            		user.Comment,
            		user.IsApproved,
            		user.UserName,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            try
		            {
		                conn.Open();
		                cmd.ExecuteNonQuery();
		            }
		            catch (NpgsqlException ex)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(ex, "UpdateUser");
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw ex;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
	            }
            }
        }
		
		public void UpdateUserLogon(string username, string applicationname)
		{
			using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
			{
				string query = string.Format(
					QRY_UPDATE_USER_LOGON,
					username,
					applicationname);
				using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
				{
					try
					{
						conn.Open();
						cmd.ExecuteNonQuery();
					}
					catch (NpgsqlException ex)
					{
						Logger.Instance.Write(TrackProtect.Logging.LogLevel.Error, ex, "UpdateUserLogon<NpgsqlException>");
					}
					finally
					{
						conn.Close();
					}
				}
			}
		}
		
		public void UpdateFailureCount(
			string username, 
			string applicationname, 
			string failureType, 
			int passwordAttemptWindow,
			int maxInvalidPasswordAttempts)
		{
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_UPDATE_FAILURE_COUNT,
            		username,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            DateTime windowStart = new DateTime();
		            int failureCount = 0;
		
		            try
		            {
		                conn.Open();
		
		                using (NpgsqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
		                {
			                if (reader.HasRows)
			                {
			                    reader.Read();
			
			                    if (failureType == "password")
			                    {
			                        failureCount = reader.GetInt32(0);
			                        windowStart = reader.GetDateTime(1);
			                    }
			
			                    if (failureType == "passwordAnswer")
			                    {
			                        failureCount = reader.GetInt32(2);
			                        windowStart = reader.GetDateTime(3);
			                    }
			                }
			
			                reader.Close();
		                }
		
		                DateTime windowEnd = windowStart.AddMinutes(passwordAttemptWindow);
		
		                if (failureCount == 0 || DateTime.Now > windowEnd)
		                {
		                    // First password failure or outside of PasswordAttemptWindow. 
		                    // Start a new password failure count from 1 and a new window starting now.
		
		                    if (failureType == "password")
		                    	query = string.Format(
		                    		QRY_UPDATE_USER_RESET_PASSWORD_FAILURE,
		                    		username,
		                    		applicationname);
		
		                    if (failureType == "passwordAnswer")
		                    	query = string.Format(
		                    		QRY_UPDATE_USER_RESET_PASSWORDANSWER_FAILURE,
		                    		username,
		                    		applicationname);
		                        cmd.CommandText = query;
		
		                    if (cmd.ExecuteNonQuery() < 0)
		                        throw new ProviderException("Unable to update failure count and window start.");
		                }
		                else
		                {
		                    if (failureCount++ >= maxInvalidPasswordAttempts)
		                    {
		                        // Password attempts have exceeded the failure threshold. Lock out
		                        // the user.
								query = string.Format(
									QRY_UPDATE_USER_LOCK_OUT,
									username,
									applicationname);
		                        cmd.CommandText = query;
		                        if (cmd.ExecuteNonQuery() < 0)
		                            throw new ProviderException("Unable to lock out user.");
		                    }
		                    else
		                    {
		                        // Password attempts have not exceeded the failure threshold. Update
		                        // the failure counts. Leave the window the same.
								
		                        if (failureType == "password")
		                        	query = string.Format(
		                        		QRY_UPDATE_USER_SET_PASSWORD_FAILURE,
		                        		failureCount,
		                        		username,
		                        		applicationname);
		
		                        if (failureType == "passwordAnswer")
		                        	query = string.Format(
		                        		QRY_UPDATE_USER_SET_PASSWORDANSWER_FAILURE,
		                        		failureCount,
		                        		username,
		                        		applicationname);

								cmd.CommandText = query;		
		
		                        if (cmd.ExecuteNonQuery() < 0)
		                            throw new ProviderException("Unable to update failure count.");
		                    }
		                }
		            }
		            catch (NpgsqlException e)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(e, "UpdateFailureCount");
		
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw e;
		                }
		            }
		            finally
		            {
		                conn.Close();
		            }
	            }
            }
        }
		
		private bool CheckPassword(string password, string dbpassword, MembershipPasswordFormat passwordFormat, TrackProtectMembershipProvider provider)
		{
			string pass1 = password;
			string pass2 = dbpassword;
			
			switch(passwordFormat)
			{
			case MembershipPasswordFormat.Encrypted:
				pass2 = provider.UnEncodePassword(dbpassword);
				break;
			case MembershipPasswordFormat.Hashed:
				pass2 = provider.EncodePassword(password);
				break;
			default:
				break;
			}
			
			return (pass1 == pass2);
		}
		
		public MembershipUserCollection FindUsersByName(string providerName, string usernameToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords)
		{
            MembershipUserCollection users = new MembershipUserCollection();
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_SELECT_USER_COUNT_BY_NAME,
            		usernameToMatch,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(QRY_SELECT_USER_COUNT_BY_NAME, conn))
	            {
		            NpgsqlDataReader reader = null;
		
		            try
		            {
		                conn.Open();
		                totalRecords = (int)cmd.ExecuteScalar();
		
		                if (totalRecords <= 0) { return users; }
		
						query = string.Format(
							QRY_GET_USER_BY_NAME_AND_APPLICATION_ORDERED,
							usernameToMatch,
							applicationname);
		                cmd.CommandText = query;
		
		                reader = cmd.ExecuteReader();
		
		                int counter = 0;
		                int startIndex = pageSize * pageIndex;
		                int endIndex = startIndex + pageSize - 1;
		
		                while (reader.Read())
		                {
		                    if (counter >= startIndex)
		                    {
		                        MembershipUser u = GetUserFromReader(providerName, reader);
		                        users.Add(u);
		                    }
		
		                    if (counter >= endIndex) { cmd.Cancel(); }
		
		                    counter++;
		                }
		            }
		            catch (NpgsqlException e)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(e, "FindUsersByName");
		
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw e;
		                }
		            }
		            finally
		            {
		                if (reader != null) { reader.Close(); }
		
		                conn.Close();
		            }
	            }
            }

            return users;
		}
		
		public MembershipUserCollection FindUsersByEmail(string providerName, string emailToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords)
		{
            MembershipUserCollection users = new MembershipUserCollection();
			totalRecords = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
            	string query = string.Format(
            		QRY_SELECT_USER_COUNT_BY_EMAIL,
            		emailToMatch,
            		applicationname);
	            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
	            {
		            NpgsqlDataReader reader = null;
		            totalRecords = 0;
		
		            try
		            {
		                conn.Open();
		                totalRecords = (int)cmd.ExecuteScalar();
		
		                if (totalRecords <= 0) { return users; }
		
						query = string.Format(
							QRY_GET_USER_BY_EMAIL_AND_APPLICATION_ORDERED,
							emailToMatch,
							applicationname);
							
		                cmd.CommandText = query;
		
		                reader = cmd.ExecuteReader();
		
		                int counter = 0;
		                int startIndex = pageSize * pageIndex;
		                int endIndex = startIndex + pageSize - 1;
		
		                while (reader.Read())
		                {
		                    if (counter >= startIndex)
		                    {
		                        MembershipUser u = GetUserFromReader(providerName, reader);
		                        users.Add(u);
		                    }
		
		                    if (counter >= endIndex) { cmd.Cancel(); }
		
		                    counter++;
		                }
		            }
		            catch (NpgsqlException e)
		            {
		                if (WriteExceptionsToEventLog)
		                {
		                    WriteToEventLog(e, "FindUsersByEmail");
		
		                    throw new ProviderException(EXCEPTION_MESSAGE);
		                }
		                else
		                {
		                    throw e;
		                }
		            }
		            finally
		            {
		                if (reader != null) { reader.Close(); }
		
		                conn.Close();
		            }
	            }
            }

            return users;
        }

		public string GetUserEmail(int userid)
		{
			UserInfo ui = GetUser(userid);
			if (ui == null)
				return string.Empty;
			return ui.Email;
		}

		public string GetUserPassword(int userid)
		{
			UserInfo ui = GetUser(userid);
			if (ui == null)
				return string.Empty;
			return ui.Password;
		}
		
        private MembershipUser GetUserFromReader(string providerName, NpgsqlDataReader reader)
        {
            object providerUserKey = reader.GetValue(0);
            string username = reader.GetString(1);
            string email = reader.GetString(2);
            string passwordQuestion = string.Empty;
            if (reader.GetValue(3) != DBNull.Value)
                passwordQuestion = reader.GetString(3);
            string comment = string.Empty;
            if (reader.GetValue(4) != DBNull.Value)
                comment = reader.GetString(4);
            bool isApproved = reader.GetBoolean(5);
            bool isLockedOut = reader.GetBoolean(6);
            DateTime creationDate = reader.GetDateTime(7);
            DateTime lastLoginDate = new DateTime();
            if (reader.GetValue(8) != DBNull.Value)
                lastLoginDate = reader.GetDateTime(8);
            DateTime lastActivityDate = reader.GetDateTime(9);
            DateTime lastPasswordChangedDate = reader.GetDateTime(10);

            DateTime lastLockedOutDate = new DateTime();
            if (reader.GetValue(11) != DBNull.Value)
                lastLockedOutDate = reader.GetDateTime(11);

            MembershipUser u = new MembershipUser(
                providerName,
                username,
                providerUserKey,
                email,
                passwordQuestion,
                comment,
                isApproved,
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockedOutDate);

            return u;
        }

        string GetConnectionString()
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["TrackProtect"];
            if (css == null || css.ConnectionString.Trim() == string.Empty)
                throw new Exception("Can't access database");
            return css.ConnectionString;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
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