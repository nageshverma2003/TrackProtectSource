using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using TrackProtect.Logging;

namespace TrackProtect
{
    public enum ConfirmationResult
    {
        Success,
        UserUnknown,
        ConfirmationFailed
    };

    public enum TransactionResult
    {
        Success,
        NotFound,
        AlreadyCompleted
    };

    public enum SocialConnector
    {
        Facebook,
        SoundCloud,
        Twitter
    };

    public class Mapping
    {
        public int pageId { get; set; }
        public int genreId { get; set; }
        public string genreType { get; set; }
    }

    public class GenrePageMapping
    {
        public string pageName { get; set; }
        public string genreName { get; set; }
        public string subGenreName { get; set; }
    }

    public abstract class Database : IDisposable
    {
        protected const int NEW_PASSWORD_LENGTH = 8;
        protected const string EVENT_SOURCE = "TrackProtectMembershipProvider";
        protected const string EVENT_LOG = "Application";
        protected const string EXCEPTION_MESSAGE = "An exception occurred. Please check the Event Log.";

        public bool WriteExceptionsToEventLog { get; set; }


        public Database()
        {
            WriteExceptionsToEventLog = false;
        }

        public abstract bool AdminLoginAuthentication(string email, string password);
        public abstract long RegisterUser(string username, string applicationname, string email, string comment,
                                          string password, string passwordquestion, string passwordanswer,
                                          int subscriptiontype);
        public abstract UserState VerifyUser(string username, string password);
        public abstract UserInfo GetUser(string username, string password);
        public abstract UserInfo GetUser(long userid, string password);
        public abstract UserInfo GetUser(long userid);
        public abstract long RegisterClientInfo(
            string lastname,
            string firstname,
            string addressline1,
            string addressline2,
            string zipcode,
            string state,
            string city,
            string country,
            string language,
            string telephone,
            string cellular,
            string companyname,
            long userid,
            string accountowner,
            string bumacode,
            string senacode,
            string isrccode,
            string twitterid,
            string facebookid,
            string soundcloudid,
            string soniallid,
            string ownerkind,
            string creditcardnr,
            string creditcardcvv,
            string emailforreceipt,
            string referer,
            char gender,
            DateTime birthdate,
            string stagename);
        public abstract IDictionary<object, object> getAdminFBCred();
        public abstract string getAdminFBName();
        public abstract bool saveAdminFBCred(string fbid, string fbname, string fbtoken, DateTime expires);
        public abstract void deleteAdminFBCred();
        public abstract void saveAdminFBPages(string pagename, string pageid, string pagetoken);
        public abstract IDictionary<string, string> getAlreadyStoredPageInfo();
        public abstract IDictionary<string, string> getAdminFBPages();
        public abstract int getAdminFBPageIdBYGenreIdandType(int genreid, string genretype);
        public abstract IDictionary<string, string> getAdminFBPageCredByPageID(int pageid);
        public abstract IDictionary<string, string> getGenreList();
        public abstract IDictionary<string, string> getSubGenreList(int genreid);
        public abstract IList<GenrePageMapping> getPageGenreMapping();
        public abstract IDictionary<string, string> getTrackInformationByID(int register_id);
        public abstract ClientInfo GetClientInfo(long userid);
        public abstract string GetMusicPathByRegID(int regID);
        public abstract long CreateRegistry
            (long userid, string certfilename, string basefilename, string trackname, string isrcCode,
            string genre1, string genre2, string genre3,
            string subgenre1, string subgenre2, string subgenre3,
            string sounds_like_tags1, string sounds_like_tags2, string sounds_like_tags3, string stageName);
        public abstract long CreateRegistry
            (long managerUserId, long managedUserId, string certfilename, string basefilename, string trackname, string isrcCode,
            string genre1, string genre2, string genre3,
            string subgenre1, string subgenre2, string subgenre3,
            string sounds_like_tags1, string sounds_like_tags2, string sounds_like_tags3, string stageName);
        public abstract void AddGenre(string genreName);
        public abstract bool AddGenreMapping(int pageid, int genreid, string genreType);
        public abstract void DeleteGenre(int id);
        public abstract void AddSubGenre(string subgenrename, int genreid);
        public abstract void DeleteSubGenre(int id);
        public abstract long RollbackRegistry(long registerid);
        public abstract long RegisterDocument(long registerid, string documentname, string documenthash);
        public abstract DataSet GetRegister(long userid);
        public abstract DataSet GetRegisterWithManager(long userIdManager);
        public abstract DataSet GetRegisterWithManager(long userIdManager, long userIdPerformer);
        public abstract void UpdateUserCredits(object userid, object productid, object credits);
        public abstract void DecrementCredits(long userid);
        public abstract int GetUserWhmcsClientId(long userid);
        public abstract void UpdateUserWhmcsClientId(long userid, int whmcsclientid);
        public abstract void ResetUserWhmcsClientd(long userid);
        public abstract string GetUserDocumentPath(long userid, string password);
        public abstract string GetUserDocumentPath(long userId);
        public abstract string GetUserUid(long userid, string password);
        public abstract string GetSetting(string settingKey);
        public abstract ProductInfoList GetProducts();
        public abstract ProductInfoList _GetProducts();
        public abstract ProductInfo GetProductById(long productid);
        public abstract ProductPriceInfoList GetProductPrices(long productid);
        public abstract ProductPriceInfoList GetProductPrices(long productid, string culture);
        public abstract ProductPriceInfoList GetProductPrices(long productid, string isoCurrency, string culture);
        public abstract ProductPriceInfoList GetProductPricesExact(long productid, string iso_currency, string culture);
        public abstract ProductPriceInfoList GetProductPricesWild(long productid, string isoCurrency, string culture);
        public abstract string GetProductTitle(long productid, string culture);
        public abstract string GetProduct_Desc_Price(long productid, string culture);
        public abstract string GetProductDescription(long productid, string country);
        public abstract string GetProductDescription(long productid, string language, string country);
        public abstract string GetProductDescription(int productid, CultureInfo culture);
        public abstract Transactions GetTransactions(long userId);
        public abstract List<CreditHistory> GetCreditHistory(long userId);
        public abstract CreditHistoryList GetCreditHistoryAdded(long userId);
        public abstract CreditHistoryList GetCreditHistoryUsed(long userId);
        public abstract long AddCreditHistory(long userId, long productId, int credits, long transaction_id);
        public abstract void UpdateCreditHistoryExpiry(long userId);
        public abstract string[] GetProductNames();
        public abstract bool isEmailAlreadyRegistered(string email);
        public abstract long CreateQuotation(long userId, int credits, long productId, string description);
        public abstract void UpdateQuotation(long transId, decimal amount);
        public abstract Transaction GetQuotation(long transId);
        public abstract long CreateTransaction(long userId, decimal amount, long productId, string description);
        public abstract long CreateTransaction(long userId, decimal total, long productId, string description, ProductInfo[] products);
        public abstract TransactionResult UpdateTransaction(string orderId, string result, string status, string statusCode,
                                                            string merchant, string paymentid, string reference, string transid,
                                                            string paymentmethod, decimal amount, ProductInfo productInfo,
                                                            string currencyIso, string countryIso2);
        public abstract void RegisterWithManager(long registerid, long userIdManager, long userIdPerformer);
        public abstract string GetSocialAccessCode(long clientId);
        public abstract void UpdateSocialCredential(long clientId, SocialConnector connector, string element, string credential);
        public abstract void RemoveSocialCredential(long clientId, SocialConnector connector);
        public abstract bool UpdateFacebookID(long clientID);
        public abstract bool UpdateTwitterID(long clientID);
        public abstract bool UpdateSoundCloudID(long clientID);
        public abstract void UpdateSoundCloudId(long clientId, string soundCloudId);
        public abstract string GetSocialCredential(long clientId, SocialConnector connector, string element);

        public abstract int ChangePassword(string username, string applicationname, string password);
        public abstract int ChangePasswordQuestionAndAnswer(string username, string applicationname, string question, string answer);
        public abstract bool DeleteUser(string username, string applicationname, bool deleteAllRelatedData);
        public abstract int GetUserCount(string applicationname);
        public abstract MembershipUserCollection GetAllUsers(string providerName, string applicationname, int pageIndex, int pageSize);
        public abstract int GetNumberOfUsersOnline(string applicationName);
        public abstract string GetPassword(string username, string answer, string applicationname, out string passwordAnswer);
        public abstract MembershipUser GetUser(string providerName, string username, string applicationname, bool userIsOnline);
        public abstract MembershipUser GetUser(string providerName, object providerUserKey, string applicationname, bool userIsOnline);
        public abstract bool UnlockUser(string username, string applicationname);
        public abstract string GetUserNameByEmail(string email, string applicationname);
        public abstract long GetUserIdByEmail(string email);
        public abstract string ResetPassword(string username, string applicationname, string answer, string newPassword,
                                             int passwordAttemptWindow, bool requiresQuestionAndAnswer,
                                             MembershipPasswordFormat passwordFormat, int maxInvalidPasswordAttempts,
                                             TrackProtectMembershipProvider provider);
        public abstract void UpdateUser(MembershipUser user, string applicationname);
        public abstract void UpdateUserLogon(string username, string applicationname);
        public abstract void UpdateFailureCount(string username, string applicationname, string failureType,
                                                int passwordAttemptWindow, int maxInvalidPasswordAttempts);
        protected abstract bool CheckPassword(string password, string dbpassword, MembershipPasswordFormat passwordFormat, TrackProtectMembershipProvider provider);
        public abstract MembershipUserCollection FindUsersByName(string providerName, string usernameToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords);
        public abstract MembershipUserCollection FindUsersByEmail(string providerName, string emailToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords);
        public abstract string GetUserEmail(long userid);
        public abstract string GetUserPassword(long userid);
        protected abstract MembershipUser GetUserFromReader(string providerName, IDataReader reader);
        protected abstract string GetConnectionString();
        public abstract bool RelationExists(long requestingUserId, long requestedUserId, int relationtype);
        public abstract bool ConfirmationExists(long requestingUserId, string requestedEmail);
        public abstract void DeleteRelation(long userId1, long userId2, int relationType);
        public abstract void UpdateConfirmation(Guid confirmationid, string email);
        public abstract void RequestConfirmation(string guid, long requestingUserId, long requestedUserId, string email, int relationType);
        public abstract void getEmailByUniqueId(string guid, out string emailRequested);
        public abstract ConfirmationResult ProcessConfirmation(string guid, int relationType, out string emailRequested, out string emailRequesting);



        protected bool _disposed = false;

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
        protected void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = EVENT_SOURCE;
            log.Log = EVENT_LOG;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }

        public abstract bool CheckActivationCode(string activationCode);
        public abstract void MarkActivationCode(string activationCode, long userid);
        public abstract void IncrementCredits(long userid, object credits);
        public abstract List<ClientInfo> GetAllClients();
        public abstract long RegisterCoArtist(long registerId, long clientId, string role);
        public abstract long GetUserIdByTpId(long tpId);
        public abstract long GetUserIdByUid(string uid);
        public abstract void ActivateUser(long userId);
        public abstract void RegisterManagedUser(long userId, long userIdToManage);
        public abstract UserInfo[] GetManagedUsers(long userId, int relationType);
        public abstract void RegisterUserRights(long userIdToManage, int vcl, int ecl);
        public abstract DataTable GetManagers();
        public abstract int GetUserCredits(long userid);
        public abstract void AddLogEntry(string logEntry);
        public abstract DataTable GetAllOpenQuotations();
        public abstract int DetermineRelationType(long userId1, long userId2);
        public abstract DataTable GetInvitations(long userId);
        public abstract string GetConfirmationUid(long confirmationId);
        public abstract void DeleteInvitation(long confirmationId);
        public abstract Transaction GetTransaction(long userId, string transid);
        //Added by Nagesh for update the status of user to active from inactive.
        public abstract bool UpdateUserStatus(long userId);
    }
}