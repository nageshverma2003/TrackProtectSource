using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Xml;
using MySql.Data.MySqlClient;
using TrackProtect.Logging;
using System.Collections.Generic;

namespace TrackProtect
{
    public class MySqlDatabase : Database, IDisposable
    {
        #region Queries ------- !

        private const string QRY_REGISTER_USER = @"
            INSERT INTO `user` (
            `username`, `applicationname`, `email`, `comment`, `password`,
            `passwordquestion`, `passwordanswer`, `isapproved`, `lastactivitydate`,
            `lastlogindate`, `lastpasswordchangeddate`, `creationdate`, `isonline`,
            `islockedout`, `lastlockedoutdate`, `failedpasswordattemptcount`,
            `failedpasswordattemptwindowstart`, `failedpasswordanswerattemptcount`,
            `failedpasswordanswerattemptwindowstart`, `subscriptiontype`, `credits`, `useruid`, `whmcsclientid`,`isActive`
            ) values (
            @username, @applicationname, @email, @comment, md5(@password),
            @passwordquestion, md5(@passwordanswer), 1, now(),
            now(), now(), now(), 1, 0, now(), 0, now(), 0,
            now(), @subscriptiontype, 0, @useruid, @whmcsclientid,0
            );";

        private const string QRY_VERIFY_USER =
            "SELECT `user_id` FROM `user` WHERE `username`=@username AND `password`=md5(@password)";

        private const string QRY_GET_USER_BY_NAME = @"
            SELECT `user_id`, `username`, `email`, `isapproved`, `islockedout`, `credits`, `useruid`, `password`,
			`creationdate`, `subscriptiontype`, `comment` , `isActive`
			FROM `user` WHERE `username`=@username AND `password`=md5(@password)";

        private const string QRY_GET_USER_BY_ID = @"
            SELECT `user_id`, `username`, `email`, `isapproved`, `islockedout`, `credits`, `useruid`, `password`,
			`creationdate`, `subscriptiontype`, `comment`, `isActive`
			FROM `user` WHERE `user_id`=@user_id AND `password`=md5(@password)";

        private const string QRY_GET_USER_BY_ID_UNAUTHENTICATED = @"
            SELECT `user_id`, `username`, `email`, `isapproved`, `islockedout`, `credits`, `useruid`, `password`,
			`creationdate`, `subscriptiontype`, `comment`, `isActive`
			FROM `user` WHERE `user_id`=@user_id";

        private const string QRY_GET_CLIENT_INFO =
            "SELECT * FROM `client` WHERE `user_id`=@user_id";

        private const string QRY_REGISTER_CLIENT_INFO = @"
            INSERT INTO `client` (
            `lastname`, `firstname`, `addressline1`, `addressline2`, `zipcode`, `state`, `city`, `country`, `telephone`, 
			`cellular`, `companyname`, `user_id`, `accountowner`, `senacode`, `isrccode`, `twitterid`, `facebookid`, 
			`soniallid`, `ownerkind`, `creditcardnr`, `creditcardcvv`, `emailreceipt`, `referer`, `language`,
			`gender`, `birthdate`, `bumacode`, `soundcloudid`, `socialaccess`, `socialcred`, `stagename`
            ) values (
            @lastname, @firstname, @addressline1, @addressline2, @zipcode, @state, @city, @country, @telephone, 
			@cellular, @companyname, @user_id, @accountowner, @senacode, @isrccode, @twitterid, @facebookid, 
			@soniallid, @ownerkind, @creditcardnr, @creditcardcvv, @emailreceipt, @referer, @language,
			@gender, @birthdate, @bumacode, @soundcloudid, @socialaccess, @socialcred, @stagename
            );";

        private const string QRY_UPDATE_CLIENT_INFO = @"
            UPDATE `client` SET 
            `lastname`=@lastname, `firstname`=@firstname, `addressline1`=@addressline1, `addressline2`=@addressline2, 
			`zipcode`=@zipcode, `state`=@state, `city`=@city, `country`=@country, `telephone`=@telephone, 
			`cellular`=@cellular, `companyname`=@companyname, `user_id`=@user_id, `accountowner`=@accountowner, 
			`senacode`=@senacode, `isrccode`=@isrccode, `twitterid`=@twitterid, `facebookid`=@facebookid,
			`soniallid`=@soniallid, `ownerkind`=@ownerkind, `creditcardnr`=@creditcardnr, 
			`creditcardcvv`=@creditcardcvv, `emailreceipt`=@emailreceipt, `referer`=@referer, 
			`language`=@language, `gender`=@gender, `birthdate`=@birthdate, `bumacode`=@bumacode, 
            `soundcloudid`=@soundcloudid, `stagename`=@stagename
            WHERE `client_id`=@client_id;";

        private const string QRY_GET_SOCIAL_ACCESS_CODE = @"
            SELECT `socialaccess` FROM `client` WHERE `client_id`=@client_id
        ";

        private const string QRY_GET_SOCIAL_ACCESS_CRED = @"
            SELECT `socialcred` FROM `client` WHERE `client_id`=@client_id
        ";

        private const string QRY_UPDATE_SOCIAL_ACCESS = @"
            UPDATE `client` SET `socialcred`=@socialcred WHERE `client_id`=@client_id
        ";

        private const string QRY_UPDATE_SOUND_CLOUD_USER_NAME = @"UPDATE `client` SET `soundcloudid`=@soundcloudid WHERE `client_id`=@client_id";

        private const string QRY_TO_EMAIL_EXIST = @"SELECT * FROM user WHERE `email`=@email";


        private const string QRY_INSERT_REGISTRATION = @"
            INSERT INTO `register` (`certificate`, `registrationdate`, `expirationdate`, `user_id`, `name`, `genre1`, `genre2`, `genre3`, `subgenre1`, `subgenre2`, `subgenre3`, `sounds_like_tags1`, `sounds_like_tags2`, `sounds_like_tags3`)
            VALUES (@certificate, now(), adddate(now(), interval 1 year), @user_id, @name, @genre1, @genre2, @genre3, @subgenre1, @subgenre2, @subgenre3, @sounds_like_tags1, @sounds_like_tags2, @sounds_like_tags3);";

        private const string QRY_INSERT_REGISTRATION_WITH_GENRES = @"
            INSERT INTO `register` (`certificate`, `registrationdate`, `expirationdate`, `user_id`, `name`,`isrcCode`, `genre1`, `genre2`, `genre3`, `subgenre1`, `subgenre2`, `subgenre3`, `sounds_like_tags1`, `sounds_like_tags2`, `sounds_like_tags3`, `stagename`)
            VALUES (@certificate, now(), adddate(now(), interval 1 year), @user_id, @name, @isrcCode, @genre1, @genre2, @genre3, @subgenre1, @subgenre2, @subgenre3, @sounds_like_tags1, @sounds_like_tags2, @sounds_like_tags3, @stagename);";


        private const string QRY_INSERT_DOCUMENT = @"
            INSERT INTO `document` (`register_id`, `documentname`, `documenthash`) values
			(@register_id, @documentname, @documenthash);";

        private const string QRY_DELETE_REGISTRATION =
            "DELETE FROM `register` WHERE `register_id`=@register_id";

        private const string QRY_DELETE_DOCUMENT =
            "DELETE FROM `document` WHERE `register_id`=@register_id";

        private const string QRY_GET_REGISTER =
            "SELECT * FROM `register` WHERE `user_id`=@user_id ORDER BY `registrationdate` DESC";

        private const string QRY_DECREMENT_CREDITS =
            "UPDATE `user` SET `credits`=`credits`-1 WHERE `user_id`=@user_id";

        private const string QRY_UPDATE_USER_CREDITS =
            "UPDATE `user` SET `credits`=`credits`+@credits WHERE `user_id`=@user_id";

        private const string QRY_GET_SETTING =
            "SELECT `value` FROM `setting` WHERE `name`=@name";

        private const string QRY_UPDATE_USER_PASSWORD = @"
	        UPDATE `user` SET `password`=md5(@password), `lastpasswordchangeddate`=now()
			WHERE `username`=@username AND `applicationname`=@applicationname;";

        private const string QRY_UPDATE_USER_PASSWORDQA = @"
	        UPDATE `user` SET `passwordquestion`=@passwordquestion, `passwordanswer`=md5(@passwordanswer)
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_WHMCSCLIENTID = @"
			UPDATE `user` SET `whmcsclientid`=@whmcsclientid, `lastactivitydate`=now() WHERE `user_id`=@user_id
			";

        private const string QRY_RESET_USER_WHMCSCLIENTID = @"
			UPDATE `user` SET `whmcsclientid`=0 WHERE adddate(`lastactivitydate`, interval 1 day) < now() AND `user_id`=@user_id
			";
        private const string QRY_GET_WHMCSCLIENT_ID = @"
                SELECT `whmcsclientid` FROM `user` WHERE `user_id`=@userid
            ";

        private const string QRY_GET_USER_CREDENTIALS = @"
	        SELECT `passwordanswer`, `islockedout` FROM `user`
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_CREDENTIALS = @"
	        UPDATE `user` SET `password`=md5(@password), `lastpasswordchangeddate`=now()
	        WHERE `username`=@username AND `applicationname`=@applicationname AND `islockedout`=false";

        private const string QRY_DELETE_USER_BY_USERNAME_AND_APPLICATION =
            "DELETE FROM `user` WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_GET_USER_COUNT_BY_APPLICATION =
            "SELECT count(*) FROM `user` WHERE `applicationname`=@applicationname";

        private const string QRY_GET_ALL_USER_BY_APPLICATION = @"
            SELECT `user_id`, `username`, `email`, `passwordquestion`, `comment`, `isapproved`, `islockedout`, 
			`creationdate`, `lastlogindate`, `lastactivitydate`, `lastpasswordchangeddate`, `lastlockedoutdate`
            `subscriptiontype`, `credits`, `useruid`
            FROM `user` WHERE `applicationname`=@applicationname ORDER BY `username` ASC";

        private const string QRY_GET_USERS_ONLINE = @"
	        SELECT count(*) FROM `user` WHERE `lastactivitydate` > @lastactivitydate
			AND `applicationname`=@applicationname";

        private const string QRY_GET_PASSWORD = @"
	        SELECT `password`, `passwordanswer`, `islockedout` FROM `user`
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_GET_USER_BY_NAME_AND_APPLICATION = @"
            SELECT `user_id`, `username`, `email`, `passwordquestion`,
            `comment`, `isapproved`, `islockedout`, `creationdate`, `lastlogindate`,
            `lastactivitydate`, `lastpasswordchangeddate`, `lastlockedoutdate`
            `subscriptiontype`, `credits`, `useruid`
            FROM `user` WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_GET_USER_BY_USER_ID = @"
	        SELECT `user_id`, `username`, `email`, `passwordquestion`, `comment`, `isapproved`, `islockedout`, 
			`creationdate`, `lastlogindate`, `lastactivitydate`, `lastpasswordchangeddate`, `lastlockedoutdate`
            `subscriptiontype`, `credits`, `useruid`, `isActive`
            FROM `user` WHERE `user_id`=@user_id";

        private const string QRY_UPDATE_USER_ACTIVITY = @"
	        UPDATE `user` SET `lastactivitydate`=now()
	        WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_UNLOCK = @"
	        UPDATE `user` SET `islockedout`=0, `lastlockedoutdate`=now()
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_GET_USERNAME_BY_EMAIL_AND_APPLICATIONNAME =
            "SELECT `username` FROM `user` WHERE `email`=@email AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_APPROVAL = @"
	        UPDATE `user` SET `email`=@email, `comment`=@comment, `isapproved`=@isapproved 
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_LOGON = @"
	        UPDATE `user` SET `lastlogindate`=now()
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_FAILURE_COUNT = @"
	        SELECT `failedpasswordattemptaount`, `failedpasswordattemptwindowstart`, `failedpasswordanswerattemptcount`,
	        `failedpasswordanswerattemptwindowstart` 
			FROM `user` WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_RESET_PASSWORD_FAILURE = @"
			UPDATE `user` SET `failedpasswordattemptcount`=1, `failedpasswordattemptwindowstart`=now()
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_SET_PASSWORD_FAILURE = @"
			UPDATE `user` SET `failedpasswordattemptcount`=@failurecount
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_RESET_PASSWORDANSWER_FAILURE = @"
			UPDATE `user`
			  SET `failedpasswordanswerattemptcount`=1,
			      `failedpasswordanswerattemptwindowstart`=now()
			  WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_SET_PASSWORDANSWER_FAILURE = @"
			UPDATE `user`
			  SET `failedpasswordanswerattemptcount`=@failurecount
			  WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_UPDATE_USER_LOCK_OUT = @"
			UPDATE `user` SET `islockedout`=1, `lastlockedoutdate`=now()
			WHERE `username`=@username AND `applicationname`=@applicationname";

        private const string QRY_SELECT_USER_COUNT_BY_NAME = @"
	        SELECT count(*) FROM `user`
	        WHERE `username` LIKE @username AND `applicationname` = @applicationname";

        private const string QRY_SELECT_USER_COUNT_BY_EMAIL = @"
	        SELECT count(*) FROM `user`
	        WHERE `email` LIKE @email AND `applicationname`=@applicationname";

        private const string QRY_GET_USER_BY_NAME_AND_APPLICATION_ORDERED = @"
            SELECT `user_id`, `username`, `email`, `passwordquestion`,
            `comment`, `isapproved`, `islockedout`, `creationdate`, `lastlogindate`,
            `lastactivitydate`, `lastpasswordchangeddate`, `lastlockedoutdate`
            `subscriptiontype`, `credits`, `useruid`
            FROM `user` WHERE `username` LIKE @username AND `applicationname`=@applicationname ORDER BY `username` Asc";

        private const string QRY_GET_USER_BY_EMAIL_AND_APPLICATION_ORDERED = @"
	        SELECT `user_id`, `username`, `email`, `passwordquestion`,
	        `comment`, `isapproved`, `islockedout`, `creationdate`, `lastlogindate`,
	        `lastactivitydate`, `lastpasswordchangeddate`, `lastlockedoutdate`
            `subscriptiontype`, `credits`, `useruid`
            FROM `user` WHERE `email` LIKE @email AND `applicationname`=@applicationname ORDER BY `username` Asc";

        private const string QRY_GET_PRODUCTS =
            "SELECT * FROM `product`";

        private const string QRY_GET_PRODUCT_BY_ID =
            "SELECT * FROM `product` WHERE `product_id`=@product_id";

        private const string QRY_GET_PRODUCTPRICES_BY_PRODUCT_ID =
            "SELECT * FROM `productprice` WHERE `product_id`=@product_id";

        private const string QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_EXACT = @"
			SELECT * FROM `productprice`
			WHERE
			`product_id`=@product_id AND `iso_currency`=@iso_currency AND `iso2_country`=@iso2_country";

        private const string QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_WILD = @"
			SELECT * FROM `productprice`
			WHERE
			`product_id`=@product_id AND
			(`iso_currency`=@iso_currency AND (`iso2_country`=@iso2_country OR iso2_country=''))";

        private const string QRY_GET_PRODUCTTITLE_BY_PRODUCT_ID_AND_LOCALE = @"
			SELECT `title` FROM `productdesc`
			WHERE `product_id`=@product_id AND `locale`=@locale";

        private const string QRY_GET_PRODUC_DESC_PRICE_BY_PRODUCT_ID_AND_LOCALE = @"
            SELECT `product_desc`,`product_price` FROM `productdesc`
			WHERE `product_id`=@product_id AND `locale`=@locale";

        /*
        private const string QRY_GET_PRODUCTTITLE_BY_PRODUCT_ID_AND_COUNTRY = @"
            SELECT `title` FROM `productdesc`
            WHERE `product_id`=@product_id AND `locale` LIKE concat('__-', @locale)";
        */

        private const string QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE = @"
			SELECT `description` FROM `productdesc`
			WHERE `product_id`=@product_id AND `locale`=@locale";

        /*
        private const string QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_COUNTRY = @"
            SELECT `description` FROM `productdesc`
            WHERE `product_id`=@product_id AND `locale` LIKE '__-' + @locale";
        */

        private const string QRY_GET_PRODUCT_NAMES =
            "SELECT `name` FROM `product`";

        private const string QRY_ADD_CREDITHISTORY = @"
			INSERT INTO `credithistory` 
			(`user_id`, `product_id`, `credits`, `purchasedate`, `expired`, `transaction_id`) VALUES
			(@user_id, @product_id, @credits, @purchasedate, 0, @transaction_id)
		";

        /*
        private const string QRY_CREDITHISTORY_EXPIRED = @"
            UPDATE `credithistory` SET `expired`=1 WHERE `credithistory_id`=@credithistory_id
        ";
         */

        private const string QRY_CREDITHISTORY_EXPIRED_BY_USER_ID = @"
			UPDATE `credithistory` SET `expired`=1 WHERE `user_id`=@user_id
		";

        /*
        private const string QRY_GET_CREDITHISTORY = @"
            SELECT * FROM `credithistory` WHERE `user_id`=@user_id
        ";
         */

        private const string QRY_GET_CREDITHISTORY_ADDED = @"
			SELECT * FROM `credithistory` WHERE `user_id`=@userid AND `product_id`>0
		";

        private const string QRY_GET_CREDITHISTORY_USED = @"
			SELECT * FROM `credithistory` WHERE `user_id`=@userid AND `product_id`=0
		";

        /*
		private const string QRY_EXPIRE_CREDITHISTORY = @"
			UPDATE `credithistory` SET `expired`=1 WHERE `purchasedate` < @expirydate
		";
         */

        private const string QRY_CREATE_TRANSACTION = @"
			INSERT INTO `transaction` (
			`user_id`,`amount`,`date`,`description`,`product_id`,`status`,`statuscode`,`merchant`,`paymentid`,
			`transid`,`paymentmethod`
			) VALUES (
			@user_id, @amount, now(), @description, @product_id, 'OPEN', @statuscode, '', '',
			'', ''
			)
		";

        private const string QRY_CREATE_QUOTATION = @"
			INSERT INTO `transaction` (
			`user_id`, `amount`, `date`, `description`, `product_id`, `status`, `statuscode`, `merchant`,
			`paymentid`, `transid`, `paymentmethod`
			) VALUES (
			@user_id, 0.0, now(), @description, @productid, @status, @statuscode, '',
			'', '', ''
			)
		";

        private const string QRY_UPDATE_QUOTATION = @"
			UPDATE `transaction` SET `amount`=@amount, `status`=@status WHERE `transaction_id`=@transaction_id
		";

        private const string QRY_UPDATE_TRANSACTION = @"
			UPDATE `transaction` SET `status`=@status, `statuscode`=@statuscode, `merchant`=@merchant,
			`paymentid`=@paymentid, `transid`=@transid, `paymentmethod`=@paymentmethod
			WHERE `transaction_id`=@transaction_id
		";

        private const string QRY_GET_QUOTATIONS = @"
			SELECT `transaction`.*, `user`.`email` 
			FROM `transaction` JOIN `user` 
			ON `transaction`.`user_id`=`user`.`user_id`
			WHERE `transaction`.`status`='RFQ'
		";

        private const string QRY_GET_TRANSACTION = @"
			SELECT * FROM `transaction` WHERE `transaction_id`=@transaction_id
		";

        const string QRY_INSERT_TRANSACTION_LINE = @"
            INSERT INTO `transaction_line` (
            `transaction_id`, `item_line`, `description`, `quantity`, `price`, `vat_percentage`, `vat_amount`
            ) VALUES (
            @transaction_id, @item_line, @description, @quantity, @price, @vat_percentage, @vat_amount
            )
        ";

        private const string QRY_GET_TRANSACTIONS_BY_USERID = @"
            SELECT * FROM `transaction` WHERE `user_id`=@user_id AND `status`='OK' ORDER BY `transaction_id` DESC
        ";

        private const string QRY_CHECK_ACTIVATIONCODE = @"
            SELECT COUNT(*) FROM `activationcode` WHERE `code`=@code AND `active`=1 AND `issued`=1 AND `entered`=0
        ";

        private const string QRY_MARK_ACTIVATIONCODE = @"
            UPDATE `activationcode` SET `entered`=1, `enterdate`=now(), `user_id`=@user_id WHERE `code`=@code
        ";

        private const string QRY_GET_ALL_CLIENT_INFO = @"
            SELECT * FROM `client`
        ";

        private const string QRY_REGISTER_COARTIST = @"
            INSERT INTO `coartist` (`client_id`,`register_id`,`role`) VALUES (@client_id, @register_id, @role)
        ";

        private const string QRY_GET_USERID_BY_EMAIL = @"
            SELECT `user_id` FROM `user` WHERE `email`=@email;
        ";

        private const string QRY_INSERT_CONFIRM = @"
			INSERT INTO `confirmation` (`unique_id`, `requesting_user_id`, `requested_email`, `relationtype`)
			VALUES (@unique_id, @requestinguserid, @requestedemail, @relationtype);
		";

        private const string QRY_SELECT_CONFIRM = @"
			SELECT * FROM `confirmation` WHERE `unique_id`=@unique_id
		";

        private const string QRY_DELETE_CONFIRM = @"
			DELETE FROM `confirmation` WHERE `unique_id`=@unique_id
		";

        private const string QRY_INSERT_USER_RELATION = @"
            INSERT INTO `relation` (`owner_user_id`, `owned_user_id`, `relationtype`)
                VALUES (@owneruserid, @owneduserid, @relationtype);
        ";

        private const string QRY_GET_USER_CREDITS = @"
			SELECT
				(sum(`credits`) -
				(SELECT sum(`credits`) as sumused FROM `credithistory` 
					WHERE `product_id`=0 AND `user_id`=@userid AND `purchasedate` >
					(
						SELECT ADDDATE(`purchasedate`, INTERVAL 1 YEAR) as `expirydate` FROM `credithistory` 
						WHERE `expired`=1 AND `user_id`=@userid AND `product_id`!=0
						UNION 
						(SELECT DATE('2000-01-01'))
						ORDER BY `expirydate` DESC LIMIT 1
					)
				   UNION (SELECT 0) ORDER BY sumused DESC LIMIT 1
				)
				) as `totalcredits`
			FROM `credithistory` WHERE `product_id`!=0 AND `user_id`=@userid AND `expired`=0
			UNION
			(SELECT 0) ORDER BY totalcredits DESC LIMIT 1;
		";

        private const string QRY_CHECK_RELATION = @"
			SELECT count(*) FROM `relation` 
			WHERE `owner_user_id`=@owneruserid 
			AND `owned_user_id`=@owneduserid 
			AND `relationtype`=@relationtype
		";

        private const string QRY_CHECK_Confirmation = @"
            SELECT count(*) FROM `confirmation` 
			WHERE `requesting_user_id`=@requestinguserid 
			AND `requested_email`=@requestedemail
        ";

        private const string QRY_INSERT_INTO_MANAGED_REGISTER = @"
            insert into `managed` (`register_id`, `user_id_manager`, `user_id_performer`)
            values (@register_id, @user_id_manager, @user_id_performer)
        ";

        private const string QRY_GET_TRANSACTION_STATUS = @"
            SELECT `status` FROM `transaction` WHERE `transaction_id`=@transaction_id;
        ";

        private const string QRY_GET_TRANSACTION_STATUSCODE = @"
            SELECT `statuscode` FROM `transaction` WHERE `transaction_id`=@transaction_id;
        ";

        private const string QRY_GET_TRANSACTION_USERID = @"
            SELECT `user_id` FROM `transaction` WHERE `transaction_id`=@transaction_id;
        ";

        private const string QRY_GET_RELATIONTYPE = @"
            select max(`relationtype`) from `relation` where `owner_user_id`=@owner_user_id and `owned_user_id`=@owned_user_id        
        ";

        private const string QRY_GET_CONFIRMATION_UID = @"
            SELECT `unique_id` FROM `confirmation` WHERE `confirmation_id`=@confirmation_id
        ";

        private const string QRY_SELECT_MANAGED_REGISTER_BY_MANAGER = @"
                select * 
	                from `register` 
	                join `regtrack` 
		                on `register`.`register_id` = `regtrack`.`register_id` 
		                and `register`.`user_id` = `regtrack`.`user_id_performer` 
	                where `regtrack`.`user_id_manage`r=@user_id_manager
            ";

        private const string QRY_SELECT_MANAGED_REGISTER_BY_MANAGER_AND_USER = @"
                select * 
	                from `register` 
	                join `regtrack` 
		                on `register`.`register_id` = `regtrack`.`register_id` 
		                and `register`.`user_id` = `regtrack`.`user_id_performer` 
	                where `regtrack`.`user_id_manager`=@user_id_manager
                    and `regtrack`.`user_id_performer`=@user_id_performer ORDER BY `registrationdate` DESC
            ";
        private const string QRY_UPDATE_USER_STATUS = @"
                update `user` set `isActive`= 1 where `user_id`= @userid
            ";

        private const string QRY_UPDATE_FACEBOOK_ID = @"UPDATE `client` SET `facebookid`=NULL WHERE `client_id`=@client_id";

        private const string QRY_UPDATE_TWITTER_ID = @"UPDATE `client` SET `twitterid`=NULL WHERE `client_id`=@client_id";

        private const string QRY_UPDATE_SOUNDCLOUD_ID = @"UPDATE `client` SET `soundcloudid`=NULL WHERE `client_id`=@client_id";

        #endregion


        #region Constructor/ Destructor ------- !

        public MySqlDatabase()
        {
            WriteExceptionsToEventLog = false;
        }

        ~MySqlDatabase()
        {
            Dispose(false);
        }

        #endregion

        private const string QRY_FOR_MUSIC = @"SELECT documentname FROM document WHERE `register_id`= @regID";

        public override string GetMusicPathByRegID(int regID)
        {
            string musicPath = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_FOR_MUSIC, conn))
                {
                    cmd.Parameters.Add("@regID", MySqlDbType.VarChar).Value = regID;
                    try
                    {
                        conn.Open();
                        object ret = cmd.ExecuteScalar();
                        if (ret != null)
                            musicPath = Convert.ToString(ret);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetMusicPathByRegID]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return musicPath;
        }

        private long GetLastInsertId(MySqlConnection connection)
        {
            long id;
            using (MySqlCommand cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
            {
                //id = (long)cmd.ExecuteScalar();
                id = Convert.ToInt64(cmd.ExecuteScalar());
            }
            return id;
        }


        public override bool AdminLoginAuthentication(string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("Select COUNT(*) FROM `adminlogin` WHERE `email`=@email AND `password`=@password", conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;

                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();

                        if (res != null)
                        {
                            int count = Convert.ToInt32(res);

                            if (count > 0)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string SELECT_ADMIN_FB_Cred_QRY = @"SELECT * FROM `adminfbsocialcred`";

        private const string INSERT_ADMIN_FB_Cred_QRY = @"INSERT INTO `adminfbsocialcred` (`id`, `fbid`, `fbname`, `fbtoken`, `expires`) VALUES(1, @fbid, @fbname, @fbtoken, @expires)";

        private const string UPDATE_ADMIN_FB_Cred_QRY = @"UPDATE `adminfbsocialcred` SET `fbid`=@fbid, `fbname`=@fbname, `fbtoken`=@fbtoken, `expires`=@expires WHERE `id`=1";

        private const string DELETE_ADMIN_FB_CRED_QRY = @"TRUNCATE `adminfbsocialcred`";

        private const string INSERT_ADMIN_FB_PAGES_QRY = @"INSERT INTO `adminfbpages` (`pagename`,`pageid`,`pagetoken`) VALUES (@pagename,@pageid,@pagetoken)";

        private const string SELECT_ADMIN_FB_PAGES_QRY = @"SELECT * FROM `adminfbpages`";

        private const string DELETE_ADMIN_FB_PAGES_QRY = @"TRUNCATE `adminfbpages`";

        private const string SELECT_ADMIN_FB_PAGES_CRED_BY_ID_QRY = @"SELECT * FROM `adminfbpages` WHERE `id`=@id";

        private const string SELECT_ADMIN_FB_PAGE_ID_GENRE_ID_AND_TYPE_QRY =
                                         @"SELECT `pageid` FROM `pagegenremapping` WHERE `genreid`=@genreid AND `genretype`=@genretype";

        public override IDictionary<object, object> getAdminFBCred()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_Cred_QRY, conn))
                {
                    try
                    {
                        IDictionary<object, object> credDict = new Dictionary<object, object>();

                        conn.Open();
                        MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);

                        while (reader.Read())
                        {
                            string fbid = reader.GetString(1);
                            string fbname = reader.GetString(2);
                            string fbtoken = reader.GetString(3);

                            DateTime expires = new DateTime();

                            try
                            {
                                expires = reader.GetDateTime(4);
                            }
                            catch { }

                            credDict.Add("fbid", fbid);
                            credDict.Add("fbname", fbname);
                            credDict.Add("fbtoken", fbtoken);
                            credDict.Add("expires", expires);

                            break;
                        }

                        return credDict;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "Update Admin FB credentials");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override string getAdminFBName()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `fbname` FROM `adminfbsocialcred` WHERE `id`=1", conn))
                {
                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();

                        if (res != null)
                            return Convert.ToString(res);

                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override bool saveAdminFBCred(string fbid, string fbname, string fbtoken, DateTime expires)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                bool flag = false;

                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_Cred_QRY, conn))
                {
                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();

                        if (res != null)
                        {
                            if (Convert.ToInt32(res) > 0)
                                flag = true;
                            else
                                flag = false;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    catch
                    {
                        flag = false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                if (flag == true)
                {
                    using (MySqlCommand cmd = new MySqlCommand(UPDATE_ADMIN_FB_Cred_QRY, conn))
                    {
                        cmd.Parameters.Add("@fbid", MySqlDbType.VarChar).Value = fbid;
                        cmd.Parameters.Add("@fbname", MySqlDbType.VarChar).Value = fbname;
                        cmd.Parameters.Add("@fbtoken", MySqlDbType.VarChar).Value = fbtoken;
                        cmd.Parameters.Add("@expires", MySqlDbType.DateTime).Value = expires;

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "Update Admin FB credentials");
                            return false;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand(INSERT_ADMIN_FB_Cred_QRY, conn))
                    {
                        cmd.Parameters.Add("@fbid", MySqlDbType.VarChar).Value = fbid;
                        cmd.Parameters.Add("@fbname", MySqlDbType.VarChar).Value = fbname;
                        cmd.Parameters.Add("@fbtoken", MySqlDbType.VarChar).Value = fbtoken;
                        cmd.Parameters.Add("@expires", MySqlDbType.DateTime).Value = expires;

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "INSERT Admin FB credentials");
                            return false;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
        }

        public override void deleteAdminFBCred()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(DELETE_ADMIN_FB_CRED_QRY, conn))
                {
                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "Delete Admin FB credentials");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }

                //using (MySqlCommand cmd = new MySqlCommand(DELETE_ADMIN_FB_PAGES_QRY, conn))
                //{
                //    try
                //    {
                //        conn.Open();
                //        object res = cmd.ExecuteNonQuery();
                //    }
                //    catch (MySqlException ex)
                //    {
                //        Logger.Instance.Write(LogLevel.Error, ex, "Delete Admin FB Page credentials");
                //    }
                //    finally
                //    {
                //        conn.Close();
                //    }
                //}

                //using (MySqlCommand cmd = new MySqlCommand("TRUNCATE `pagegenremapping`", conn))
                //{
                //    try
                //    {
                //        conn.Open();
                //        cmd.ExecuteNonQuery();
                //    }
                //    catch
                //    {

                //    }
                //    finally
                //    {
                //        conn.Close();
                //    }
                //}
            }
        }

        public override void saveAdminFBPages(string pagename, string pageid, string pagetoken)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(INSERT_ADMIN_FB_PAGES_QRY, conn))
                {
                    cmd.Parameters.Add("@pagename", MySqlDbType.VarChar).Value = pagename;
                    cmd.Parameters.Add("@pageid", MySqlDbType.VarChar).Value = pageid;
                    cmd.Parameters.Add("@pagetoken", MySqlDbType.VarChar).Value = pagetoken;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "INSERT Admin FB Page credentials");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override IDictionary<string, string> getAdminFBPages()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_PAGES_QRY, conn))
                {
                    try
                    {
                        IDictionary<string, string> pageList = new Dictionary<string, string>();

                        conn.Open();
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                pageList.Add(Convert.ToString(rdr[0]), Convert.ToString(rdr[1]));
                            }
                        }

                        return pageList;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "INSERT Admin FB Page credentials");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override IDictionary<string, string> getAlreadyStoredPageInfo()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_PAGES_QRY, conn))
                {
                    try
                    {
                        IDictionary<string, string> pageList = new Dictionary<string, string>();

                        conn.Open();
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                try
                                {
                                    pageList.Add(Convert.ToString(rdr[1]), Convert.ToString(rdr[2]));
                                }
                                catch { }
                            }
                        }

                        return pageList;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "INSERT Admin FB Page credentials");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        public override int getAdminFBPageIdBYGenreIdandType(int genreid, string genretype)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_PAGE_ID_GENRE_ID_AND_TYPE_QRY, conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                    cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = genretype;

                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();

                        if (res != null)
                            return Convert.ToInt32(res);
                        else
                            return 0;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "getAdminFBPageIdBYGenreIdandType");
                        return 0;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override IDictionary<string, string> getAdminFBPageCredByPageID(int pageid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ADMIN_FB_PAGES_CRED_BY_ID_QRY, conn))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = pageid;

                    try
                    {
                        IDictionary<string, string> adminFBCredList = new Dictionary<string, string>();

                        conn.Open();
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                adminFBCredList.Add("PageName", Convert.ToString(rdr[1]));
                                adminFBCredList.Add("PageID", Convert.ToString(rdr[2]));
                                adminFBCredList.Add("PageToken", Convert.ToString(rdr[3]));
                            }
                        }

                        return adminFBCredList;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "getAdminFBPageCredByPageID");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string SELECT_GENRE_QRY = @"SELECT * FROM `genre`";

        public override IDictionary<string, string> getGenreList()
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_GENRE_QRY, conn))
                {
                    try
                    {
                        IDictionary<string, string> genreList = new Dictionary<string, string>();

                        conn.Open();

                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                genreList.Add(Convert.ToString(rdr[0]), Convert.ToString(rdr[1]));
                            }
                        }

                        return genreList;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "SELECT GENRE");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string SELECT_SUB_GENRE_QRY = @"SELECT * FROM `subgenre` WHERE `genreid`=@genreid";

        public override IDictionary<string, string> getSubGenreList(int genreid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_SUB_GENRE_QRY, conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                    try
                    {
                        IDictionary<string, string> subGenreList = new Dictionary<string, string>();

                        conn.Open();

                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                subGenreList.Add(Convert.ToString(rdr[0]), Convert.ToString(rdr[1]));
                            }
                        }

                        return subGenreList;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "SELECT SUBGENRE");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }



        public override IList<GenrePageMapping> getPageGenreMapping()
        {
            IList<Mapping> mappinglist = new List<Mapping>();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `pagegenremapping`", conn))
                {
                    try
                    {
                        conn.Open();
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr != null)
                            if (rdr.HasRows)
                                while (rdr.Read())
                                {
                                    Mapping obj = new Mapping();
                                    obj.pageId = rdr.GetInt32(1);
                                    obj.genreId = rdr.GetInt32(2);
                                    obj.genreType = rdr.GetString(3);

                                    mappinglist.Add(obj);
                                }

                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "getPageGenreMapping SELECT Page Genre Mapping");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            IList<GenrePageMapping> GenrePageMappingList = new List<GenrePageMapping>();

            if (mappinglist != null)
                if (mappinglist.Count() > 0)
                    foreach (Mapping obj in mappinglist)
                    {
                        GenrePageMapping genPageObj = new GenrePageMapping();

                        if (obj.genreType == "genre")
                            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                            {
                                using (MySqlCommand cmd = new MySqlCommand("SELECT `genrename` FROM `genre` WHERE `id`=@id", conn))
                                {
                                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = obj.genreId;

                                    try
                                    {
                                        conn.Open();
                                        object res = cmd.ExecuteScalar();
                                        genPageObj.genreName = Convert.ToString(res);
                                        genPageObj.subGenreName = string.Empty;
                                    }
                                    catch
                                    {

                                    }
                                    finally
                                    {
                                        conn.Close();
                                    }
                                }
                            }
                        else if (obj.genreType == "subgenre")
                            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                            {
                                using (MySqlCommand cmd = new MySqlCommand("SELECT `subgenrename` FROM `subgenre` WHERE `id`=@id", conn))
                                {
                                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = obj.genreId;

                                    try
                                    {
                                        conn.Open();
                                        object res = cmd.ExecuteScalar();
                                        genPageObj.genreName = string.Empty;
                                        genPageObj.subGenreName = Convert.ToString(res);
                                    }
                                    catch
                                    {

                                    }
                                    finally
                                    {
                                        conn.Close();
                                    }
                                }
                            }

                        using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                        {
                            using (MySqlCommand cmd = new MySqlCommand("SELECT `pagename` FROM `adminfbpages` WHERE `id`=@id", conn))
                            {
                                cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = obj.pageId;

                                try
                                {
                                    conn.Open();
                                    object res = cmd.ExecuteScalar();
                                    genPageObj.pageName = Convert.ToString(res);
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                        }

                        GenrePageMappingList.Add(genPageObj);
                    }

            return GenrePageMappingList;
        }


        private const string SELECT_REGISTER_QRY = @"SELECT * FROM `register` WHERE `register_id`=@register_id";

        public override IDictionary<string, string> getTrackInformationByID(int register_id)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_REGISTER_QRY, conn))
                {
                    cmd.Parameters.Add("@register_id", MySqlDbType.Int32).Value = register_id;

                    try
                    {
                        IDictionary<string, string> trackInformation = new Dictionary<string, string>();

                        conn.Open();

                        MySqlDataReader rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                if (!rd.IsDBNull(15))
                                    trackInformation.Add("StageName", rd.GetString(15));
                                else
                                    trackInformation.Add("StageName", "");
                                if (!rd.IsDBNull(16))
                                    trackInformation.Add("isrcCode", rd.GetString(16));
                                else
                                    trackInformation.Add("isrcCode", "");
                                if (!rd.IsDBNull(5))
                                    trackInformation.Add("TrackName", rd.GetString(5));
                                else
                                    trackInformation.Add("TrackName", "");
                                if (!rd.IsDBNull(6))
                                    trackInformation.Add("genre1", rd.GetString(6));
                                else
                                    trackInformation.Add("genre1", "");
                                if (!rd.IsDBNull(7))
                                    trackInformation.Add("genre2", rd.GetString(7));
                                else
                                    trackInformation.Add("genre2", "");
                                if (!rd.IsDBNull(8))
                                    trackInformation.Add("genre3", rd.GetString(8));
                                else
                                    trackInformation.Add("genre3", "");
                                if (!rd.IsDBNull(9))
                                    trackInformation.Add("subgenre1", rd.GetString(9));
                                else
                                    trackInformation.Add("subgenre1", "");
                                if (!rd.IsDBNull(10))
                                    trackInformation.Add("subgenre2", rd.GetString(10));
                                else
                                    trackInformation.Add("subgenre2", "");
                                if (!rd.IsDBNull(11))
                                    trackInformation.Add("subgenre3", rd.GetString(11));
                                else
                                    trackInformation.Add("subgenre3", "");
                                if (!rd.IsDBNull(12))
                                    trackInformation.Add("tag1", rd.GetString(12));
                                else
                                    trackInformation.Add("tag1", "");
                                if (!rd.IsDBNull(13))
                                    trackInformation.Add("tag2", rd.GetString(13));
                                else
                                    trackInformation.Add("tag2", "");
                                if (!rd.IsDBNull(14))
                                    trackInformation.Add("tag3", rd.GetString(14));
                                else
                                    trackInformation.Add("tag3", "");
                            }

                            return trackInformation;
                        }

                        return null;

                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "SELECT Track Information");
                        return null;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override long RegisterUser(
            string username,
            string applicationname,
            string email,
            string comment,
            string password,
            string passwordquestion,
            string passwordanswer,
            int subscriptiontype)
        {
            long userId = -1L;
            string userUid = Guid.NewGuid().ToString("N");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_REGISTER_USER, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = email;
                    cmd.Parameters.Add("@comment", MySqlDbType.Text).Value = comment;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = password;
                    cmd.Parameters.Add("@passwordquestion", MySqlDbType.VarChar, 200).Value = passwordquestion ?? string.Empty;
                    cmd.Parameters.Add("@passwordanswer", MySqlDbType.VarChar, 200).Value = passwordanswer ?? string.Empty;
                    cmd.Parameters.Add("@subscriptiontype", MySqlDbType.Int32).Value = subscriptiontype;
                    cmd.Parameters.Add("@useruid", MySqlDbType.VarChar, 100).Value = userUid;
                    cmd.Parameters.Add("@whmcsclientid", MySqlDbType.Int32).Value = 0;

                    try
                    {
                        conn.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            userId = GetLastInsertId(conn);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        // Log the exception
                        Logger.Instance.Write(LogLevel.Error, ex, "RegisterUser<MySqlException>");
                        userId = 0;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "RegisterUser<Exception>");
                        throw;
                    }
                }
            }

            return userId;
        }

        public override UserState VerifyUser(string username, string password)
        {
            UserState res = new UserState();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_VERIFY_USER, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = password;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt64(0);
                                res.State = 1;
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "VerifyUser<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "VerifyUser<Exception>");
                    }
                }
            }
            return res;

        }

        public override UserInfo GetUser(string username, string password)
        {
            UserInfo res = new UserInfo();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_BY_NAME, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = password;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt64(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetInt16(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                                res.MemberSince = rdr.GetDateTime(8);
                                res.SubscriptionType = rdr.GetInt32(9);
                                res.Comment = rdr.GetString(10);
                                //Added by Nagesh for active inactive flag
                                res.IsActive = rdr.GetInt32(11);
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

        public override UserInfo GetUser(long userid, string password)
        {
            UserInfo res = new UserInfo();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_BY_ID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = password;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt64(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetInt16(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                                res.MemberSince = rdr.GetDateTime(8);
                                res.SubscriptionType = rdr.GetInt32(9);
                                res.Comment = rdr.GetString(10);
                                //Added by Nagesh for active inactive flag
                                res.IsActive = rdr.GetInt32(11);
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

        public override UserInfo GetUser(long userid)
        {
            UserInfo res = new UserInfo();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_BY_ID_UNAUTHENTICATED, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.UserId = rdr.GetInt64(0);
                                res.UserName = rdr.GetString(1);
                                res.Email = rdr.GetString(2);
                                res.IsApproved = rdr.GetInt16(3);
                                res.IsLockedOut = rdr.GetBoolean(4);
                                res.Credits = rdr.GetInt32(5);
                                res.UserUid = rdr.GetString(6);
                                res.Password = rdr.GetString(7);
                                res.MemberSince = rdr.GetDateTime(8);
                                res.SubscriptionType = rdr.GetInt32(9);
                                res.Comment = rdr.GetString(10);
                                //Added by Nagesh for active inactive flag
                                res.IsActive = rdr.GetInt32(11);
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

        public override long RegisterClientInfo(
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
            string stagename)
        {
            ClientInfo ci = GetClientInfo(userid);
            long clientId = 0;
            if (ci != null && ci.ClientId > 0)
                clientId = ci.ClientId;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                if (clientId == 0)
                {
                    using (MySqlCommand cmd = new MySqlCommand(QRY_REGISTER_CLIENT_INFO, conn))
                    {
                        cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 100).Value = lastname;
                        cmd.Parameters.Add("@firstname", MySqlDbType.VarChar, 50).Value = firstname;
                        cmd.Parameters.Add("@addressline1", MySqlDbType.VarChar, 100).Value = addressline1;
                        cmd.Parameters.Add("@addressline2", MySqlDbType.VarChar, 100).Value = addressline2;
                        cmd.Parameters.Add("@zipcode", MySqlDbType.VarChar, 10).Value = zipcode;
                        cmd.Parameters.Add("@city", MySqlDbType.VarChar, 100).Value = city;
                        cmd.Parameters.Add("@state", MySqlDbType.VarChar, 100).Value = state;
                        cmd.Parameters.Add("@country", MySqlDbType.VarChar, 100).Value = country;
                        cmd.Parameters.Add("@language", MySqlDbType.VarChar, 100).Value = language;
                        cmd.Parameters.Add("@telephone", MySqlDbType.VarChar, 20).Value = telephone;
                        cmd.Parameters.Add("@cellular", MySqlDbType.VarChar, 20).Value = cellular;
                        cmd.Parameters.Add("@companyname", MySqlDbType.VarChar, 100).Value = companyname;
                        cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                        cmd.Parameters.Add("@accountowner", MySqlDbType.VarChar, 500).Value = accountowner;
                        cmd.Parameters.Add("@bumacode", MySqlDbType.VarChar, 100).Value = bumacode;
                        cmd.Parameters.Add("@senacode", MySqlDbType.VarChar, 100).Value = senacode;
                        cmd.Parameters.Add("@isrccode", MySqlDbType.VarChar, 100).Value = isrccode;
                        cmd.Parameters.Add("@twitterid", MySqlDbType.VarChar, 100).Value = twitterid;
                        cmd.Parameters.Add("@facebookid", MySqlDbType.VarChar, 100).Value = facebookid;
                        cmd.Parameters.Add("@soundcloudid", MySqlDbType.VarChar, 100).Value = soundcloudid;
                        cmd.Parameters.Add("@soniallid", MySqlDbType.VarChar, 100).Value = soniallid;
                        cmd.Parameters.Add("@ownerkind", MySqlDbType.VarChar, 500).Value = ownerkind;
                        cmd.Parameters.Add("@creditcardnr", MySqlDbType.VarChar, 20).Value = creditcardnr;
                        cmd.Parameters.Add("@creditcardcvv", MySqlDbType.VarChar, 10).Value = creditcardcvv;
                        cmd.Parameters.Add("@emailreceipt", MySqlDbType.VarChar, 100).Value = emailforreceipt;
                        cmd.Parameters.Add("@referer", MySqlDbType.Text).Value = referer;
                        cmd.Parameters.Add("@gender", MySqlDbType.VarChar, 1).Value = gender;
                        cmd.Parameters.Add("@birthdate", MySqlDbType.DateTime).Value = birthdate;
                        cmd.Parameters.Add("@socialaccess", MySqlDbType.VarChar, 100).Value = Crypto.ComputeHash(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"), new SHA256CryptoServiceProvider());
                        cmd.Parameters.Add("@socialcred", MySqlDbType.Text).Value = string.Empty;
                        cmd.Parameters.Add("@stagename", MySqlDbType.VarChar, 50).Value = stagename;
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            clientId = GetLastInsertId(conn);
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "RegisterClientInfo<MySqlException>");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "RegisterClientInfo<Exception>");
                            throw;
                        }
                    }
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_CLIENT_INFO, conn))
                    {
                        cmd.Parameters.Add("@lastname", MySqlDbType.VarChar, 100).Value = lastname;
                        cmd.Parameters.Add("@firstname", MySqlDbType.VarChar, 50).Value = firstname;
                        cmd.Parameters.Add("@addressline1", MySqlDbType.VarChar, 100).Value = addressline1;
                        cmd.Parameters.Add("@addressline2", MySqlDbType.VarChar, 100).Value = addressline2;
                        cmd.Parameters.Add("@zipcode", MySqlDbType.VarChar, 10).Value = zipcode;
                        cmd.Parameters.Add("@city", MySqlDbType.VarChar, 100).Value = city;
                        cmd.Parameters.Add("@state", MySqlDbType.VarChar, 100).Value = state;
                        cmd.Parameters.Add("@country", MySqlDbType.VarChar, 100).Value = country;
                        cmd.Parameters.Add("@language", MySqlDbType.VarChar, 100).Value = language;
                        cmd.Parameters.Add("@telephone", MySqlDbType.VarChar, 20).Value = telephone;
                        cmd.Parameters.Add("@cellular", MySqlDbType.VarChar, 20).Value = cellular;
                        cmd.Parameters.Add("@companyname", MySqlDbType.VarChar, 100).Value = companyname;
                        cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                        cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                        cmd.Parameters.Add("@accountowner", MySqlDbType.VarChar, 500).Value = accountowner;
                        cmd.Parameters.Add("@bumacode", MySqlDbType.VarChar, 100).Value = bumacode;
                        cmd.Parameters.Add("@senacode", MySqlDbType.VarChar, 100).Value = senacode;
                        cmd.Parameters.Add("@isrccode", MySqlDbType.VarChar, 100).Value = isrccode;
                        cmd.Parameters.Add("@twitterid", MySqlDbType.VarChar, 100).Value = twitterid;
                        cmd.Parameters.Add("@facebookid", MySqlDbType.VarChar, 100).Value = facebookid;
                        cmd.Parameters.Add("@soundcloudid", MySqlDbType.VarChar, 100).Value = soundcloudid;
                        cmd.Parameters.Add("@soniallid", MySqlDbType.VarChar, 100).Value = soniallid;
                        cmd.Parameters.Add("@ownerkind", MySqlDbType.VarChar, 500).Value = ownerkind;
                        cmd.Parameters.Add("@creditcardnr", MySqlDbType.VarChar, 20).Value = creditcardnr;
                        cmd.Parameters.Add("@creditcardcvv", MySqlDbType.VarChar, 10).Value = creditcardcvv;
                        cmd.Parameters.Add("@emailreceipt", MySqlDbType.VarChar, 100).Value = emailforreceipt;
                        cmd.Parameters.Add("@referer", MySqlDbType.Text).Value = referer;
                        cmd.Parameters.Add("@gender", MySqlDbType.VarChar, 1).Value = gender;
                        cmd.Parameters.Add("@birthdate", MySqlDbType.DateTime).Value = birthdate;
                        cmd.Parameters.Add("@stagename", MySqlDbType.VarChar, 50).Value = stagename;
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "RegisterClientInfo<Exception>");
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "RegisterClientInfo<Exception>");
                            throw;
                        }
                    }
                }
            }
            return clientId;
        }

        public override bool isEmailAlreadyRegistered(string email)
        {
            try
            {
                bool ret = false;
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    using (MySqlCommand cmd = new MySqlCommand(QRY_TO_EMAIL_EXIST, conn))
                    {
                        cmd.Parameters.Add("@email", MySqlDbType.VarChar, 20).Value = email;

                        try
                        {
                            conn.Open();
                            object res = cmd.ExecuteScalar();
                            if (res != null)
                            {
                                int val = Convert.ToInt32(res);
                                ret = (val > 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "CheckActivationCode<Exception>");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(LogLevel.Error, ex, "CheckEmailExistence<Exception>");
                throw;
            }

            return false;
        }


        public override ClientInfo GetClientInfo(long userid)
        {
            ClientInfo res = new ClientInfo();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_CLIENT_INFO, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                res.ClientId = rdr.GetInt64(0);
                                res.LastName = rdr.GetString(1);
                                res.FirstName = rdr.GetString(2);
                                res.AddressLine1 = rdr.GetString(3);
                                res.AddressLine2 = rdr.GetString(4);
                                res.ZipCode = rdr.GetString(5);
                                res.City = rdr.GetString(6);
                                res.State = rdr.GetString(7);
                                res.Country = rdr.GetString(8);
                                res.Telephone = rdr.GetString(9);
                                res.Cellular = rdr.GetString(10);
                                res.CompanyName = rdr.GetString(11);
                                res.UserId = rdr.GetInt64(12);
                                res.AccountOwner = rdr.GetString(13);
                                res.SenaCode = rdr.GetString(14);
                                res.IsrcCode = rdr.GetString(15);
                                res.SoniallId = rdr.GetString(16);
                                if (!rdr.IsDBNull(17))
                                    res.TwitterId = rdr.GetString(17);
                                else
                                    res.TwitterId = "";
                                if (!rdr.IsDBNull(18))
                                    res.FacebookId = rdr.GetString(18);
                                else
                                    res.FacebookId = "";
                                res.OwnerKind = rdr.GetString(19);
                                res.CreditCardNr = rdr.GetString(20);
                                res.CreditCardCvv = rdr.GetString(21);
                                res.EmailReceipt = rdr.GetString(22);
                                res.Referer = rdr.GetString(23);
                                res.Language = rdr.GetString(24);
                                res.Gender = rdr.GetChar(25);
                                res.Birthdate = rdr.GetDateTime(26);
                                res.BumaCode = rdr.GetString(27);
                                if (!rdr.IsDBNull(28))
                                    res.SoundCloudId = rdr.GetString(28);
                                else
                                    res.SoundCloudId = "";
                                res.stagename = rdr.GetString(31);
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetClientInfo<MySqlException>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetClientInfo<Exception>");
                        throw;
                    }
                }
            }
            return res;
        }

        public override long CreateRegistry
            (long userid, string certfilename, string basefilename, string trackname, string isrcCode,
            string genre1, string genre2, string genre3,
            string subgenre1, string subgenre2, string subgenre3,
            string sounds_like_tags1, string sounds_like_tags2, string sounds_like_tags3, string stageName)
        {
            long registerid;
            string workingTrackName = trackname;
            if (string.IsNullOrEmpty(workingTrackName))
                workingTrackName = Path.GetFileNameWithoutExtension(basefilename);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_INSERT_REGISTRATION_WITH_GENRES, conn))
                {
                    cmd.Parameters.Add("@certificate", MySqlDbType.VarChar, 500).Value = Path.GetFileName(certfilename);
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = workingTrackName;
                    cmd.Parameters.Add("@isrcCode", MySqlDbType.VarChar).Value = isrcCode;
                    cmd.Parameters.Add("@genre1", MySqlDbType.VarChar, 50).Value = genre1;
                    cmd.Parameters.Add("@genre2", MySqlDbType.VarChar, 50).Value = genre2;
                    cmd.Parameters.Add("@genre3", MySqlDbType.VarChar, 50).Value = genre3;
                    cmd.Parameters.Add("@subgenre1", MySqlDbType.VarChar, 50).Value = subgenre1;
                    cmd.Parameters.Add("@subgenre2", MySqlDbType.VarChar, 50).Value = subgenre2;
                    cmd.Parameters.Add("@subgenre3", MySqlDbType.VarChar, 50).Value = subgenre3;
                    cmd.Parameters.Add("@sounds_like_tags1", MySqlDbType.VarChar, 50).Value = sounds_like_tags1;
                    cmd.Parameters.Add("@sounds_like_tags2", MySqlDbType.VarChar, 50).Value = sounds_like_tags2;
                    cmd.Parameters.Add("@sounds_like_tags3", MySqlDbType.VarChar, 50).Value = sounds_like_tags3;
                    cmd.Parameters.Add("@stagename", MySqlDbType.VarChar, 200).Value = stageName;


                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        registerid = GetLastInsertId(conn);
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateRegistry<MySqlException>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateRegistry<Exception>");
                        throw;
                    }
                }
            }
            return registerid;
        }


        public override long CreateRegistry
            (long managerUserId, long managedUserId, string certfilename, string basefilename, string trackname, string isrcCode,
            string genre1, string genre2, string genre3,
            string subgenre1, string subgenre2, string subgenre3,
            string sounds_like_tags1, string sounds_like_tags2, string sounds_like_tags3, string stageName)
        {
            long registerid;
            string workingTrackName = trackname;
            if (string.IsNullOrEmpty(workingTrackName))
                workingTrackName = Path.GetFileNameWithoutExtension(basefilename);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_INSERT_REGISTRATION_WITH_GENRES, conn))
                {
                    cmd.Parameters.Add("@certificate", MySqlDbType.VarChar, 500).Value = Path.GetFileName(certfilename);
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = managedUserId;
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = workingTrackName;
                    cmd.Parameters.Add("@isrcCode", MySqlDbType.VarChar).Value = isrcCode;
                    cmd.Parameters.Add("@genre1", MySqlDbType.VarChar, 50).Value = genre1;
                    cmd.Parameters.Add("@genre2", MySqlDbType.VarChar, 50).Value = genre2;
                    cmd.Parameters.Add("@genre3", MySqlDbType.VarChar, 50).Value = genre3;
                    cmd.Parameters.Add("@subgenre1", MySqlDbType.VarChar, 50).Value = subgenre1;
                    cmd.Parameters.Add("@subgenre2", MySqlDbType.VarChar, 50).Value = subgenre2;
                    cmd.Parameters.Add("@subgenre3", MySqlDbType.VarChar, 50).Value = subgenre3;
                    cmd.Parameters.Add("@sounds_like_tags1", MySqlDbType.VarChar, 50).Value = sounds_like_tags1;
                    cmd.Parameters.Add("@sounds_like_tags2", MySqlDbType.VarChar, 50).Value = sounds_like_tags2;
                    cmd.Parameters.Add("@sounds_like_tags3", MySqlDbType.VarChar, 50).Value = sounds_like_tags3;
                    cmd.Parameters.Add("@stagename", MySqlDbType.VarChar, 200).Value = stageName;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        registerid = GetLastInsertId(conn);

                        cmd.CommandText =
                            "INSERT INTO `regtrack` (`register_id`,`user_id_manager`,`user_id_performer`) VALUES (@register_id,@user_id_manager,@user_id_performer)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@register_id", MySqlDbType.Int64).Value = registerid;
                        cmd.Parameters.Add("@user_id_manager", MySqlDbType.Int64).Value = managerUserId;
                        cmd.Parameters.Add("@user_id_performer", MySqlDbType.Int64).Value = managedUserId;
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateRegistry<MySqlException>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateRegistry<Exception>");
                        throw;
                    }
                }
            }
            return registerid;
        }

        private const string INSERT_GENRE_QRY = @"INSERT INTO `genre` (`genrename`) VALUES (@genrename)";

        public override void AddGenre(string genreName)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(INSERT_GENRE_QRY, conn))
                {
                    cmd.Parameters.Add("@genrename", MySqlDbType.VarChar).Value = genreName;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "AddGenre");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string SELECT_GENRE_MAPPING =
                              @"SELECT COUNT(*) FROM `pagegenremapping` WHERE `genreid`=@genreid AND `genretype`=@genretype";

        private const string INSERT_GENRE_MAPPING =
                              @"INSERT INTO `pagegenremapping` (`pageid`,`genreid`,`genretype`) VALUES (@pageid,@genreid,@genretype)";

        private const string UPDATE_GENRE_MAPPING =
                              @"UPDATE `pagegenremapping` SET `pageid`=@pageid WHERE `genreid`=@genreid AND `genretype`=@genretype";

        public override bool AddGenreMapping(int pageid, int genreid, string genreType)
        {
            bool isExist = false;

            int res = 0;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(SELECT_GENRE_MAPPING, conn))
                {
                    cmd.Parameters.Add("@pageid", MySqlDbType.Int32).Value = pageid;
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                    cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = genreType;

                    try
                    {
                        conn.Open();
                        res = Convert.ToInt32(cmd.ExecuteScalar());

                        if (res > 0)
                            isExist = true;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "AddGenreMapping");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            if (isExist == false)
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    using (MySqlCommand cmd = new MySqlCommand(INSERT_GENRE_MAPPING, conn))
                    {
                        cmd.Parameters.Add("@pageid", MySqlDbType.Int32).Value = pageid;
                        cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                        cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = genreType;

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "AddGenreMapping");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                {
                    using (MySqlCommand cmd = new MySqlCommand(UPDATE_GENRE_MAPPING, conn))
                    {
                        cmd.Parameters.Add("@pageid", MySqlDbType.Int32).Value = pageid;
                        cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                        cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = genreType;

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "Update AddGenreMapping");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return isExist;
        }


        private const string DELETE_GENRE_QRY = @"DELETE FROM `genre` WHERE `id`=@id";

        private const string DELETE_SUBGENRE_BY_GENREID_QRY = @"DELETE FROM `subgenre` WHERE `genreid`=@genreid";

        public override void DeleteGenre(int genreid)
        {
            IList<Int32> subGenreId = new List<Int32>();

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `id` FROM `subgenre` WHERE `genreid`=@genreid", conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;

                    try
                    {
                        conn.Open();

                        MySqlDataReader rd = cmd.ExecuteReader();

                        if (rd != null)
                            while (rd.Read())
                            {
                                if (!rd.IsDBNull(0))
                                    subGenreId.Add(rd.GetInt32(0));
                            }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteGenre Select sungenre !");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            if (subGenreId != null)
            {
                if (subGenreId.Count() > 0)
                {
                    foreach (int id in subGenreId)
                    {
                        using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                        {
                            using (MySqlCommand cmd = new MySqlCommand
                                ("DELETE FROM `pagegenremapping` WHERE `genreid`=@genreid AND `genretype`=@genretype", conn))
                            {
                                cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = subGenreId[id];
                                cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = "subgenre";

                                try
                                {
                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (MySqlException ex)
                                {
                                    Logger.Instance.Write(LogLevel.Error, ex, "DeleteGenre delete subgenre !");
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                        }
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand
                    ("DELETE FROM `pagegenremapping` WHERE `genreid`=@genreid AND `genretype`=@genretype", conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;
                    cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = "genre";

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteGenre delete genre !");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(DELETE_SUBGENRE_BY_GENREID_QRY, conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteSubGenre");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(DELETE_GENRE_QRY, conn))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = genreid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteGenre");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string INSERT_SUB_GENRE_QRY = @"INSERT INTO `subgenre` (`subgenrename`, `genreid`) VALUES (@subgenrename,@genreid)";

        public override void AddSubGenre(string subgenrename, int genreid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(INSERT_SUB_GENRE_QRY, conn))
                {
                    cmd.Parameters.Add("@subgenrename", MySqlDbType.VarChar).Value = subgenrename;
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = genreid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "AddSubGenre");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        private const string DELETE_SUBGENRE_QRY = @"DELETE FROM `subgenre` WHERE `id`=@id";

        public override void DeleteSubGenre(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand
                    ("DELETE FROM `pagegenremapping` WHERE `genreid`=@genreid AND `genretype`=@genretype", conn))
                {
                    cmd.Parameters.Add("@genreid", MySqlDbType.Int32).Value = id;
                    cmd.Parameters.Add("@genretype", MySqlDbType.VarChar).Value = "subgenre";

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteSubGenre delete subgenre !");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(DELETE_SUBGENRE_QRY, conn))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = id;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteSubGenre");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        public override long RollbackRegistry(long registerid)
        {
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_DELETE_DOCUMENT, conn))
                {
                    cmd.Parameters.Add("@register_id", MySqlDbType.Int64).Value = registerid;

                    try
                    {
                        // Remove all associated documents
                        cmd.ExecuteNonQuery();

                        // And then remove the actual registry
                        cmd.CommandText = QRY_DELETE_REGISTRATION;
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "RollbackRegistry<MySqlException>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "RollbackRegistry<Exception>");
                        throw;
                    }
                }
            }
            return rowsAffected;
        }

        public override long RegisterDocument(long registerid, string documentname, string documenthash)
        {
            long documentid;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_INSERT_DOCUMENT, conn))
                {
                    cmd.Parameters.Add("@register_id", MySqlDbType.Int64).Value = registerid;
                    cmd.Parameters.Add("@documentname", MySqlDbType.VarChar, 500).Value = documentname;
                    cmd.Parameters.Add("@documenthash", MySqlDbType.VarChar, 100).Value = documenthash;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        documentid = GetLastInsertId(conn);
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "RegisterDocument<MySqlException>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "RegisterDocument<Exception>");
                        throw;
                    }
                }
            }
            return documentid;
        }

        public override DataSet GetRegister(long userid)
        {
            DataSet dataSet = new DataSet("register");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_REGISTER, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dataSet);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetRegister<Exception>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetRegister<Exception>");
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

        public override void UpdateUserCredits(object userid, object productid, object credits)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_CREDITS, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = (long)userid;
                    cmd.Parameters.Add("@credits", MySqlDbType.Int32).Value = (int)credits;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        /*
                        cmd.CommandText = @"
                            INSERT INTO `credithistory` 
                            (`user_id`,`product_id`,`credits`,`purchasedate`,`expired`)
                            VALUES
                            (@user_id, @product_id, @credits, now(), 0)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value		= (long)userid;
                        cmd.Parameters.Add("product_id", MySqlDbType.Int64).Value	= (long)productid;
                        cmd.Parameters.Add("@credits", MySqlDbType.Int32).Value		= (int)credits;
                        cmd.ExecuteNonQuery();
                         */
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateUserCredits<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateUserCredits<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void DecrementCredits(long userid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_DECREMENT_CREDITS, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DecrementCredits<Exception>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DecrementCredits<Exception>, userid: {0}", userid);
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override int GetUserWhmcsClientId(long userid)
        {
            int ret = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_WHMCSCLIENT_ID, conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                            ret = Convert.ToInt32(obj);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetUserWhmcsClientId<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override void IncrementCredits(long userid, object credits)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_CREDITS, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                    cmd.Parameters.Add("@credits", MySqlDbType.Int32).Value = credits;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "IncrementCredits<Exception>");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "IncrementCredits<Exception>, userid: {0}", userid);
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override List<ClientInfo> GetAllClients()
        {
            List<ClientInfo> clients = new List<ClientInfo>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_ALL_CLIENT_INFO, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ClientInfo ci = new ClientInfo
                                {
                                    ClientId = rdr.GetInt64(0),
                                    LastName = rdr.GetString(1),
                                    FirstName = rdr.GetString(2),
                                    AddressLine1 = rdr.GetString(3),
                                    AddressLine2 = rdr.GetString(4),
                                    ZipCode = rdr.GetString(5),
                                    City = rdr.GetString(6),
                                    State = rdr.GetString(7),
                                    Country = rdr.GetString(8),
                                    Telephone = rdr.GetString(9),
                                    Cellular = rdr.GetString(10),
                                    CompanyName = rdr.GetString(11),
                                    UserId = rdr.GetInt64(12),
                                    AccountOwner = rdr.GetString(13),
                                    SenaCode = rdr.GetString(14),
                                    IsrcCode = rdr.GetString(15),
                                    SoniallId = rdr.GetString(16),
                                    TwitterId = rdr.GetString(17),
                                    FacebookId = rdr.GetString(18),
                                    OwnerKind = rdr.GetString(19),
                                    CreditCardNr = rdr.GetString(20),
                                    CreditCardCvv = rdr.GetString(21),
                                    EmailReceipt = rdr.GetString(22),
                                    Referer = rdr.GetString(23),
                                    Language = rdr.GetString(24),
                                    Gender = rdr.GetChar(25),
                                    Birthdate = rdr.GetDateTime(26),
                                    BumaCode = rdr.GetString(27),
                                    SoundCloudId = rdr.GetString(28)
                                };
                                clients.Add(ci);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetAllClients]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return clients;
        }

        public override long RegisterCoArtist(long registerId, long clientId, string role)
        {
            long coartistId = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_REGISTER_COARTIST, conn))
                {
                    cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                    cmd.Parameters.Add("@register_id", MySqlDbType.Int64).Value = registerId;
                    cmd.Parameters.Add("@role", MySqlDbType.VarChar, 100).Value = role;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        coartistId = GetLastInsertId(conn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[RegisterCoArtist]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return coartistId;
        }

        public override long GetUserIdByTpId(long tpId)
        {
            long userId = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `user_id` FROM `tpid` WHERE `tpid`=@tpid", conn))
                {
                    cmd.Parameters.Add("@tpid", MySqlDbType.Int64).Value = tpId;
                    try
                    {
                        conn.Open();
                        object ret = cmd.ExecuteScalar();
                        if (ret != null)
                            userId = Convert.ToInt64(ret);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetUserIdByTpId]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return userId;
        }

        public override long GetUserIdByUid(string uid)
        {
            long userId = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT `user_id` FROM `user` WHERE `useruid`=@guid", conn))
                {
                    cmd.Parameters.Add("@guid", MySqlDbType.String, 100).Value = uid;

                    try
                    {
                        conn.Open();
                        object ret = cmd.ExecuteScalar();
                        if (ret != null)
                            userId = Convert.ToInt64(ret);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetUserIdByUid]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return userId;
        }

        public override void ActivateUser(long userId)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("UPDATE `user` SET `isapproved`=2 WHERE `user_id`=@userid", conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.ActivateUser]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        //Added by Nagesh
        public override bool UpdateUserStatus(long userId)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_STATUS, conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.User]");
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void RegisterManagedUser(long userId, long userIdToManage)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `management` (`manager_user_id`,`managed_user_id`) VALUES (@manager_user_id, @managed_user_id)", conn))
                {
                    cmd.Parameters.Add("@manager_user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@managed_user_id", MySqlDbType.Int64).Value = userIdToManage;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.RegisterManagedUser]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private const string QRY_SELECT_USERS_BY_MANAGER = @"
            SELECT  `user`.`user_id`, `user`.`username`, `user`.`email`,
                    `user`.`isapproved`, `user`.`islockedout`,
                    `user`.`credits`, `user`.`useruid`, `user`.`password`,
                    `user`.`creationdate`, `user`.`subscriptiontype`,
                    `user`.`comment`
            FROM `user` INNER JOIN `relation` 
                ON `user`.`user_id` = `relation`.`owned_user_id` 
                AND `relation`.`owner_user_id` = @user_id
                AND `relation`.`relationtype` = @relationtype
            ";


        public override UserInfo[] GetManagedUsers(long userId, int relationType)
        {
            List<UserInfo> users = new List<UserInfo>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_USERS_BY_MANAGER, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;

                    try
                    {
                        conn.Open();
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            UserInfo res = new UserInfo
                            {
                                UserId = rdr.GetInt64(0),
                                UserName = rdr.GetString(1),
                                Email = rdr.GetString(2),
                                IsApproved = rdr.GetInt16(3),
                                IsLockedOut = rdr.GetBoolean(4),
                                Credits = rdr.GetInt32(5),
                                UserUid = rdr.GetString(6),
                                Password = rdr.GetString(7),
                                MemberSince = rdr.GetDateTime(8),
                                SubscriptionType = rdr.GetInt32(9),
                                Comment = rdr.GetString(10)
                            };
                            users.Add(res);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetManagedUsers]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return users.ToArray();
        }

        private const string XML_CLEARANCE_LEVELS = "<?xml version=\"1.0\"?><settings><setting name=\"vcl\" value=\"{0}\"/><setting name=\"ecl\" value=\"{1}\"/></settings>";
        public override void RegisterUserRights(long userIdToManage, int vcl, int ecl)
        {
            string comment = string.Format(XML_CLEARANCE_LEVELS, vcl, ecl);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand("UPDATE `user` SET `comment`=@comment WHERE `user_id`=@user_id AND `comment`='';", conn))
            {
                cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userIdToManage;
                cmd.Parameters.Add("@comment", MySqlDbType.Text).Value = comment;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.RegisterUserRights]");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override DataTable GetManagers()
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `user` WHERE `comment`!=''", conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                            da.Fill(dt);

                        // Still need to filter rows that don't contain security intel
                        List<DataRow> rowsToDelete = new List<DataRow>();
                        foreach (DataRow row in dt.Rows)
                        {
                            string comment = row["comment"] as string;
                            if (comment != null && !comment.StartsWith("<?xml"))
                            {
                                rowsToDelete.Add(row);
                            }
                            else
                            {
                                if (comment != null && ((!comment.Contains("vcl")) && (!comment.Contains("ecl"))))
                                    rowsToDelete.Add(row);
                            }
                        }
                        foreach (DataRow row in rowsToDelete)
                            dt.Rows.Remove(row);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetManagers]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dt;
        }

        public override int GetUserCredits(long userid)
        {
            int ret = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_CREDITS, conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                            ret = Convert.ToInt32(obj);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetUserCredits<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        private const string QRY_ADD_LOG = @"INSERT INTO `log` (`entry`) VALUES (@entry)";

        public override void AddLogEntry(string logEntry)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_ADD_LOG, conn))
                {
                    cmd.Parameters.Add("@entry", MySqlDbType.Text).Value = logEntry;
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void UpdateUserWhmcsClientId(long userid, int whmcsclientid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_WHMCSCLIENTID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;
                    cmd.Parameters.Add("@whmcsclientid", MySqlDbType.Int32).Value = whmcsclientid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateUserWhmcsClientId<Exception>, userid: {0}, whmcsclientid: {1}", userid, whmcsclientid);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void ResetUserWhmcsClientd(long userid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_RESET_USER_WHMCSCLIENTID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "ResetUserWhmcsClientId<Exception>, userid: {0}", userid);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override string GetUserDocumentPath(long userid, string password)
        {
            string path = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StoragePath"]))
                {
                    string drivePath = ConfigurationManager.AppSettings["StoragePath"];

                    if (!Directory.Exists(drivePath))
                        return null;

                    UserInfo ui = GetUser(userid, password);
                    string fullDocPath = Path.Combine(drivePath, ui.UserUid);

                    if (!Directory.Exists(fullDocPath))
                        Directory.CreateDirectory(fullDocPath);

                    return fullDocPath.Replace("\\", "/");
                }

                return null;
            }
            catch { throw; }

            //if (string.IsNullOrEmpty(path))
            //{
            //    UserInfo ui = GetUser(userid, password);
            //    string userDocPath = Path.Combine(GetSetting("repository"), ui.UserUid);
            //    string fullDocPath = HttpContext.Current.Server.MapPath(userDocPath);
            //    if (!Directory.Exists(fullDocPath))
            //        Directory.CreateDirectory(fullDocPath);
            //    return fullDocPath.Replace("\\", "/");
            //}
            //else
            //{
            //    UserInfo ui = GetUser(userid, password);
            //    string userDocPath = Path.Combine(GetSetting("repository"), ui.UserUid);
            //    string fullDocPath = ConfigurationManager.AppSettings["StoragePath"] + userDocPath;
            //    if (!Directory.Exists(fullDocPath))
            //        Directory.CreateDirectory(fullDocPath);
            //    return fullDocPath.Replace("\\", "/");
            //}
        }

        public override string GetUserDocumentPath(long userId)
        {
            string path = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["StoragePath"]))
                {
                    string drivePath = ConfigurationManager.AppSettings["StoragePath"];

                    if (!Directory.Exists(drivePath))
                        return null;

                    UserInfo ui = GetUser(userId);
                    string fullDocPath = Path.Combine(drivePath, ui.UserUid);

                    if (!Directory.Exists(fullDocPath))
                        Directory.CreateDirectory(fullDocPath);

                    return fullDocPath.Replace("\\", "/");
                }

                return null;
            }
            catch { throw; }
        }

        public override string GetUserUid(long userid, string password)
        {
            UserInfo ui = GetUser(userid, password);
            return ui.UserUid;
        }

        public override string GetSetting(string settingKey)
        {
            string ret;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_SETTING, conn))
                {
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar, 100).Value = settingKey;

                    try
                    {
                        conn.Open();
                        ret = (string)cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetSetting<Exception>");
                        throw;
                    }
                }
            }
            return ret;
        }

        public override ProductInfoList GetProducts()
        {
            ProductInfoList res = new ProductInfoList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTS, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable tbl = new DataTable("product");
                        try
                        {
                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                long productId = Convert.ToInt64(row["product_id"]);
                                string name = Convert.ToString(row["name"]);
                                string description = Convert.ToString(row["description"]);
                                int credits = Convert.ToInt32(row["credits"]);
                                string extra = Convert.ToString(row["extra"]);
                                res.Add(new ProductInfo(productId, name, description, credits, extra));
                            }
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProducts<MySqlException>");
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

        public override ProductInfoList _GetProducts()
        {
            ProductInfoList res = new ProductInfoList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTS, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable tbl = new DataTable("product");
                        try
                        {
                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                long productId = Convert.ToInt64(row["product_id"]);
                                string name = Convert.ToString(row["name"]);
                                string description = Convert.ToString(row["description"]);
                                int credits = Convert.ToInt32(row["credits"]);
                                string extra = Convert.ToString(row["extra"]);
                                string productPlan = Convert.ToString(row["product_plan"]);
                                res.Add(new ProductInfo(productId, name, description, credits, extra, productPlan));
                            }
                        }
                        catch (MySqlException ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProducts<MySqlException>");
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

        public override ProductInfo GetProductById(long productid)
        {
            ProductInfo res = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCT_BY_ID, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                {
                                    long productId = reader.GetInt64(0);
                                    string name = reader.GetString(1);
                                    string desc = reader.GetString(2);
                                    int credits = reader.GetInt32(3);
                                    string extra = reader.GetString(4);
                                    res = new ProductInfo(productId, name, desc, credits, extra);
                                }
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductById<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override ProductPriceInfoList GetProductPrices(long productid)
        {
            ProductPriceInfoList res = new ProductPriceInfoList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTPRICES_BY_PRODUCT_ID, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;

                    DataTable tbl = new DataTable("productprice");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        try
                        {
                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                long productPriceId = Convert.ToInt64(row["productprice_id"]);
                                decimal price = Convert.ToDecimal(row["price"]);
                                string isoCurreny = Convert.ToString(row["iso_currency"]);
                                string iso2Country = Convert.ToString(row["iso2_country"]);
                                long productId = Convert.ToInt64(row["product_id"]);
                                ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
                                res.Add(ppi);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProductPrices<Exception>");
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

        public override ProductPriceInfoList GetProductPrices(long productid, string culture)
        {
            return GetProductPrices(productid, Util.GetCurrencyIsoNameByCulture(culture), culture);
        }

        public override ProductPriceInfoList GetProductPrices(long productid, string isoCurrency, string culture)
        {
            ProductPriceInfoList res = GetProductPricesExact(productid, isoCurrency, culture);
            if (res.Count == 0)
                res = GetProductPricesWild(productid, isoCurrency, culture);
            return res;
        }

        public override ProductPriceInfoList GetProductPricesExact(long productid, string iso_currency, string culture)
        {
            if (iso_currency == null)
                throw new ArgumentNullException("iso_currency");
            string workCulture = "nl-NL";
            if (!string.IsNullOrEmpty(culture))
                workCulture = culture;
            string isoCurrency = "EUR";
            string isoCountry = "NL";
            if (!string.IsNullOrEmpty(iso_currency))
                isoCurrency = iso_currency;
            if (!string.IsNullOrEmpty(culture))
            {
                isoCountry = culture.Length == 2 ? culture : culture.Substring(3);
            }

            ProductPriceInfoList res = new ProductPriceInfoList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_EXACT, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@iso_currency", MySqlDbType.VarChar, 10).Value = isoCurrency;
                    cmd.Parameters.Add("@iso2_country", MySqlDbType.VarChar, 2).Value = isoCountry;

                    DataTable tbl = new DataTable("productprice");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        conn.Open();
                        try
                        {
                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                long productPriceId = Convert.ToInt64(row["productprice_id"]);
                                decimal price = Convert.ToDecimal(row["price"]);
                                string isoCurreny = Convert.ToString(row["iso_currency"]);
                                string iso2Country = Convert.ToString(row["iso2_country"]);
                                long productId = Convert.ToInt64(row["product_id"]);
                                ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
                                res.Add(ppi);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProductPricesExact<Exception>");
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

        public override ProductPriceInfoList GetProductPricesWild(long productid, string isoCurrency, string culture)
        {
            ProductPriceInfoList res = new ProductPriceInfoList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_WILD, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@iso_currency", MySqlDbType.VarChar, 10).Value = isoCurrency;
                    //cmd.Parameters.Add("@iso2_country", MySqlDbType.VarChar, 2).Value = culture.Substring(3);
                    cmd.Parameters.Add("@iso2_country", MySqlDbType.VarChar, 2).Value = culture.Substring(3);

                    DataTable tbl = new DataTable("productprice");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        try
                        {
                            conn.Open();

                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                long productPriceId = Convert.ToInt64(row["productprice_id"]);
                                decimal price = Convert.ToDecimal(row["price"]);
                                string isoCurreny = Convert.ToString(row["iso_currency"]);
                                string iso2Country = Convert.ToString(row["iso2_country"]);
                                long productId = Convert.ToInt64(row["product_id"]);
                                ProductPriceInfo ppi = new ProductPriceInfo(productPriceId, price, isoCurreny, iso2Country, productId);
                                res.Add(ppi);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProductPricesWild<Exception>");
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

        public override string GetProductTitle(long productid, string culture)
        {
            string workCulture = "nl-NL";
            if (!string.IsNullOrEmpty(culture))
                workCulture = culture;
            string res = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTTITLE_BY_PRODUCT_ID_AND_LOCALE, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@locale", MySqlDbType.VarChar, 2).Value = workCulture;

                    try
                    {
                        conn.Open();
                        res = cmd.ExecuteScalar() as string;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductTitle<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override string GetProduct_Desc_Price(long productid, string culture)
        {
            string workCulture = "nl-NL";
            if (!string.IsNullOrEmpty(culture))
                workCulture = culture;
            string res = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUC_DESC_PRICE_BY_PRODUCT_ID_AND_LOCALE, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@locale", MySqlDbType.VarChar, 2).Value = workCulture;

                    DataTable tbl = new DataTable("productprice");
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        try
                        {
                            conn.Open();

                            da.Fill(tbl);
                            foreach (DataRow row in tbl.Rows)
                            {
                                res = res + Convert.ToString(row["product_desc"]) + "#";
                                res = res + Convert.ToString(row["product_price"]);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Write(LogLevel.Error, ex, "GetProductDesc<Exception>");
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

        public override string GetProductDescription(long productid, string culture)
        {
            string res = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@locale", MySqlDbType.VarChar, 2).Value = culture;

                    try
                    {
                        conn.Open();
                        res = cmd.ExecuteScalar() as string;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductDescription<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override string GetProductDescription(long productid, string language, string country)
        {
            string res = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@locale", MySqlDbType.VarChar, 10).Value = language + "-" + country;

                    try
                    {
                        conn.Open();
                        res = cmd.ExecuteScalar() as string;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductDescription<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override string GetProductDescription(int productid, CultureInfo culture)
        {
            string res = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTDESC_BY_PRODUCT_ID_AND_LOCALE, conn))
                {
                    string cultureName = culture.Name;
                    int pos = cultureName.IndexOf("/", StringComparison.Ordinal);
                    if (pos > -1)
                    {
                        cultureName = cultureName.Substring(0, pos);
                    }
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productid;
                    cmd.Parameters.Add("@locale", MySqlDbType.VarChar, 10).Value = cultureName;

                    try
                    {
                        conn.Open();
                        res = cmd.ExecuteScalar() as string;
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductDescription<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override Transactions GetTransactions(long userId)
        {
            Transactions transactions = new Transactions();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTIONS_BY_USERID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                transactions.Add(new Transaction
                                {
                                    TransactionId = rdr.GetInt64(0),
                                    UserId = userId,
                                    Amount = rdr.GetDecimal(2),
                                    Date = rdr.GetDateTime(3),
                                    Description = rdr.GetString(4),
                                    ProductId = rdr.GetInt64(5),
                                    Status = rdr.GetString(6),
                                    StatusCode = rdr.GetString(7),
                                    Merchant = rdr.GetString(8),
                                    PaymentId = rdr.GetString(9),
                                    TransactionReference = rdr.GetString(10),
                                    PaymentMethod = rdr.GetString(11)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetTransactions<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return transactions;
        }

        public override Transaction GetTransaction(long userId, string transid)
        {
            Transaction trans = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `transaction` WHERE `transid`=@transid", conn))
                {
                    cmd.Parameters.Add("@transid", MySqlDbType.VarChar, 100).Value = transid;
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                trans = new Transaction
                                {
                                    TransactionId = rdr.GetInt64(0),
                                    UserId = userId,
                                    Amount = rdr.GetDecimal(2),
                                    Date = rdr.GetDateTime(3),
                                    Description = rdr.GetString(4),
                                    ProductId = rdr.GetInt64(5),
                                    Status = rdr.GetString(6),
                                    StatusCode = rdr.GetString(7),
                                    Merchant = rdr.GetString(8),
                                    PaymentId = rdr.GetString(9),
                                    TransactionReference = rdr.GetString(10),
                                    PaymentMethod = rdr.GetString(11)
                                };
                            }
                        }

                        if (trans != null)
                        {
                            cmd.CommandText = "SELECT * FROM `transaction_line` WHERE `transaction_id`=@trans_id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@trans_id", MySqlDbType.Int64).Value = trans.TransactionId;

                            using (MySqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    trans.TransactionLines.Add(new TransactionLine
                                    {
                                        ItemLine = rdr.GetInt32(2),
                                        Description = rdr.GetString(3),
                                        Quantity = rdr.GetInt32(4),
                                        Price = rdr.GetDecimal(5),
                                        VatPercentage = rdr.GetDecimal(6),
                                        VatAmount = rdr.GetDecimal(7)
                                    }
                                        );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetTransaction<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return trans;
        }

        public override List<CreditHistory> GetCreditHistory(long userId)
        {
            CreditHistoryList res = new CreditHistoryList();

            CreditHistoryList ret = new CreditHistoryList();
            CreditHistoryList added = GetCreditHistoryAdded(userId);
            CreditHistoryList used = GetCreditHistoryUsed(userId);

            // Now we have all the credits added and used for the specified user
            foreach (CreditHistory ch in added)
            {
                bool found = true;
                int credits = ch.Credits;
                while (found)
                {
                    if (used.Count > 0 && used[0].PurchaseDate < ch.PurchaseDate.AddYears(1))
                    {
                        credits -= used[0].Credits;
                        used.RemoveAt(0);
                    }
                    else
                    {
                        CreditHistory creditHistory = new CreditHistory(ch.CreditHistoryId,
                                                                        ch.UserId,
                                                                        ch.ProductId,
                                                                        credits,
                                                                        ch.PurchaseDate,
                                                                        ch.Expired,
                                                                        ch.TransactionId);
                        Transaction transaction = null;
                        using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
                        {
                            string QRY_GET_TRANSACTION_BY_USERID_AND_DATE = string.Empty;

                            if (creditHistory.TransactionId == 0)
                                QRY_GET_TRANSACTION_BY_USERID_AND_DATE = @"
                			                                SELECT * FROM `transaction` 
                			                                WHERE `user_id`=@userid 
                			                                AND `status`='OK' 
                			                                AND (`date` BETWEEN SUBDATE(@transdate, interval 5 minute) AND ADDDATE(@transdate, interval 5 minute)) 
                			                                LIMIT 1
                		                                ";

                            else
                                QRY_GET_TRANSACTION_BY_USERID_AND_DATE = @"         
                                			                SELECT * FROM `transaction` 
                                			                WHERE `user_id`=@userid 
                                			                AND `status`='OK' 
                                			                AND `transaction_id`=@transactionId
                                		                ";


                            using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTION_BY_USERID_AND_DATE, conn))
                            {

                                cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userId;
                                cmd.Parameters.Add("@transdate", MySqlDbType.DateTime).Value = creditHistory.PurchaseDate;
                                cmd.Parameters.Add("@transactionId", MySqlDbType.Int64).Value = creditHistory.TransactionId;

                                try
                                {
                                    conn.Open();
                                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                                    {
                                        while (rdr.Read())
                                        {
                                            transaction = new Transaction();

                                            transaction.TransactionId = rdr.GetInt64(0);
                                            transaction.UserId = userId;
                                            transaction.Amount = rdr.GetDecimal(2);
                                            transaction.Date = rdr.GetDateTime(3);
                                            transaction.Description = rdr.GetString(4);
                                            transaction.ProductId = rdr.GetInt64(5);
                                            transaction.Status = rdr.GetString(6);
                                            transaction.StatusCode = rdr.GetString(7);
                                            transaction.Merchant = rdr.GetString(8);
                                            transaction.PaymentId = rdr.GetString(9);
                                            transaction.TransactionReference = rdr.GetString(10);
                                            transaction.PaymentMethod = rdr.GetString(11);

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Instance.Write(LogLevel.Error, ex, "GetCreditHistory<exception>");
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }
                        }
                        if (transaction != null)
                        {
                            creditHistory.Description = transaction.Description.Replace(".", ",") ?? "";
                            creditHistory.InvoiceFile = string.Format("INV{0}.pdf", transaction.PaymentId ?? "");
                        }
                        found = false;
                        ret.Add(creditHistory);
                    }
                }
            }

            List<CreditHistory> TempRes = (from obj in ret
                                           orderby obj.CreditHistoryId descending
                                           select obj).ToList<CreditHistory>();

            return TempRes;
        }

        public override CreditHistoryList GetCreditHistoryAdded(long userId)
        {
            CreditHistoryList ret = new CreditHistoryList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_CREDITHISTORY_ADDED, conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userId;
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                long creditHistoryId = rdr.GetInt64(0);
                                long newUserId = rdr.GetInt64(1);
                                long productId = rdr.GetInt64(2);
                                int credits = rdr.GetInt32(3);
                                DateTime purchaseDate = rdr.GetDateTime(4);
                                int expired = rdr.GetInt32(5);
                                long transactionId = rdr.GetInt64(6);

                                ret.Add(new CreditHistory(creditHistoryId, newUserId, productId, credits, purchaseDate, expired, transactionId));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetCreditHistoryAdded<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override CreditHistoryList GetCreditHistoryUsed(long userId)
        {
            CreditHistoryList ret = new CreditHistoryList();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_CREDITHISTORY_USED, conn))
                {
                    cmd.Parameters.Add("@userid", MySqlDbType.Int64).Value = userId;
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                long creditHistoryId = rdr.GetInt64(0);
                                long newUserId = rdr.GetInt64(1);
                                long productId = rdr.GetInt64(2);
                                int credits = rdr.GetInt32(3);
                                DateTime purchaseDate = rdr.GetDateTime(4);
                                int expired = rdr.GetInt32(5);
                                long transactionId = rdr.GetInt64(6);

                                ret.Add(new CreditHistory(creditHistoryId, newUserId, productId, credits, purchaseDate, expired, transactionId));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetCreditHistoryUsed<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override long AddCreditHistory(long userId, long productId, int credits, long transaction_id)
        {
            long creditHistoryId = -1L;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_ADD_CREDITHISTORY, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productId;
                    cmd.Parameters.Add("@credits", MySqlDbType.Int32).Value = credits;
                    cmd.Parameters.Add("@purchasedate", MySqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transaction_id;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        creditHistoryId = GetLastInsertId(conn);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "AddCreditHistory<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return creditHistoryId;
        }

        public override void UpdateCreditHistoryExpiry(long userId)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CREDITHISTORY_EXPIRED_BY_USER_ID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateCreditHistoryExpiry<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override string[] GetProductNames()
        {
            List<string> names = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCT_NAMES, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                    names.Add(rdr.GetString(0));
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductNames<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetProductNames<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return names.ToArray();
        }

        public override long CreateQuotation(long userId, int credits, long productId, string description)
        {
            long res = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CREATE_QUOTATION, conn))
                {
                    string statuscode = string.Format("RFQ(credits:{0})", credits);

                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, 100).Value = description;
                    cmd.Parameters.Add("@status", MySqlDbType.VarChar, 100).Value = "RFQ";
                    cmd.Parameters.Add("@statuscode", MySqlDbType.VarChar, 100).Value = statuscode;
                    cmd.Parameters.Add("@productid", MySqlDbType.Int64).Value = productId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        res = GetLastInsertId(conn);
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override void UpdateQuotation(long transId, decimal amount)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_QUOTATION, conn))
                {
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transId;
                    cmd.Parameters.Add("@status", MySqlDbType.VarChar, 100).Value = "QUOTE";
                    cmd.Parameters.Add("@amount", MySqlDbType.Decimal).Value = amount;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateQuotation<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override long CreateTransaction(long userId, decimal total, long productId, string description, ProductInfo[] products)
        {
            int totalcredits = 0;
            long res = 0;
            bool subscription = false;
            CultureInfo culture = HttpContext.Current.Session["culture"] as CultureInfo ?? new CultureInfo("nl-NL");

            List<decimal> prices = new List<decimal>();
            foreach (ProductInfo product in products)
            {
                totalcredits += product.Credits;
                if (!string.IsNullOrEmpty(product.Extra) && product.Extra.ToLower().StartsWith("subscription"))
                    subscription = true;

                ProductPriceInfoList ppil = GetProductPrices(product.ProductId, culture.ToString());
                if (ppil != null && ppil.Count > 0)
                {
                    prices.Add(ppil[0].Price);
                }
                else
                {
                    // Check whether an alternative price is given in the "Extra" field
                    decimal price = 0m;
                    string[] parts = product.Extra.Split('\x01');
                    if (parts.Length > 0)
                    {
                        decimal tmp;
                        if (decimal.TryParse(parts[parts.Length - 1], out tmp))
                            price = tmp;

                        if (product.Extra == "subscription" && tmp == 0)
                            price = 1;
                    }
                    prices.Add(price);
                }
            }

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CREATE_TRANSACTION, conn))
                {
                    string statCode = string.Empty;
                    if (subscription)
                        statCode = string.Format("SUB(credits:{0})", totalcredits);
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@amount", MySqlDbType.Decimal).Value = total;
                    cmd.Parameters.Add("@statuscode", MySqlDbType.VarChar, 100).Value = statCode;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, 100).Value = description;
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        res = GetLastInsertId(conn);

                        int i = 0;
                        foreach (ProductInfo product in products)
                        {
                            cmd.CommandText = QRY_INSERT_TRANSACTION_LINE;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = res;
                            cmd.Parameters.Add("@item_line", MySqlDbType.Int32).Value = 1;
                            cmd.Parameters.Add("@description", MySqlDbType.VarChar, 100).Value = product.Name;
                            cmd.Parameters.Add("@quantity", MySqlDbType.Int32).Value = product.ProductId == 0 ? product.Credits : 1;
                            cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = prices[i];
                            cmd.Parameters.Add("@vat_percentage", MySqlDbType.Decimal).Value = 21m;
                            cmd.Parameters.Add("@vat_amount", MySqlDbType.Decimal).Value = 0;

                            cmd.ExecuteNonQuery();
                            ++i;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override long CreateTransaction(long userId, decimal amount, long productId, string description)
        {
            long res = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CREATE_TRANSACTION, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;
                    cmd.Parameters.Add("@amount", MySqlDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar, 100).Value = description;
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productId;
                    cmd.Parameters.Add("@statuscode", MySqlDbType.VarChar, 100).Value = string.Empty;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        res = GetLastInsertId(conn);

                        cmd.CommandText = QRY_INSERT_TRANSACTION_LINE;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = res;
                        cmd.Parameters.Add("@item_line", MySqlDbType.Int32).Value = 1;
                        cmd.Parameters.Add("@description", MySqlDbType.VarChar, 100).Value = description;
                        cmd.Parameters.Add("@quantity", MySqlDbType.Int32).Value = 1;
                        cmd.Parameters.Add("@price", MySqlDbType.Decimal).Value = amount;
                        cmd.Parameters.Add("@vat_percentage", MySqlDbType.Decimal).Value = 21m;
                        cmd.Parameters.Add("@vat_amount", MySqlDbType.Decimal).Value = 0;

                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<MySqlException>");
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CreateTransaction<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return res;
        }

        public override Transaction GetQuotation(long transId)
        {
            Transaction transaction = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTION, conn))
                {
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transId;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    transaction = new Transaction
                                    {
                                        TransactionId = rdr.GetInt64(0),
                                        UserId = rdr.GetInt64(1),
                                        Amount = rdr.GetDecimal(2),
                                        Date = rdr.GetDateTime(3),
                                        Description = rdr.GetString(4),
                                        ProductId = rdr.GetInt64(5),
                                        Status = rdr.GetString(6),
                                        StatusCode = rdr.GetString(7),
                                        Merchant = rdr.GetString(8),
                                        PaymentId = rdr.GetString(9),
                                        TransactionReference = rdr.GetString(10),
                                        PaymentMethod = rdr.GetString(11)
                                    };
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetTransaction<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return transaction;
        }

        public override TransactionResult UpdateTransaction(string orderId, string result, string status, string statusCode,
                                               string merchant, string paymentid, string reference, string transid,
                                               string paymentmethod, decimal amount, ProductInfo productInfo,
                                               string currencyIso, string countryIso2)
        {
            long userId = -1;
            ProductPriceInfo ppi = null;
            string oldStatusCode = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PRODUCTPRICE_BY_PRODUCT_ID_CURRENCY_COUNTRY_WILD, conn))
                {
                    cmd.Parameters.Add("@product_id", MySqlDbType.Int64).Value = productInfo.ProductId;
                    cmd.Parameters.Add("@iso_currency", MySqlDbType.VarChar, 10).Value = currencyIso;
                    cmd.Parameters.Add("@iso2_country", MySqlDbType.VarChar, 2).Value = countryIso2;

                    try
                    {
                        conn.Open();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    long productPriceId = rdr.GetInt64(0);
                                    decimal price = rdr.GetDecimal(1);
                                    string isoCurrency = rdr.GetString(2);
                                    string iso2Country = rdr.GetString(3);
                                    ppi = new ProductPriceInfo(productPriceId, price, isoCurrency, iso2Country,
                                                               productInfo.ProductId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(
                            LogLevel.Error, ex,
                            "UpdateTransaction<Exception:Get ProductPriceInfo>");
                        conn.Close();
                        return TransactionResult.NotFound;
                    }
                }

                long transactionId;
                if (!long.TryParse(orderId, out transactionId))
                    transactionId = 0;
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTION_STATUS, conn))
                {
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transactionId;
                    try
                    {
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            string currStatus = Convert.ToString(obj);
                            if (currStatus.ToLower() == "ok" || currStatus.ToLower() == "err")
                                return TransactionResult.AlreadyCompleted;
                        }
                        else
                        {
                            return TransactionResult.NotFound;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateTransaction<exception>[check status]");
                        return TransactionResult.NotFound;
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTION_STATUSCODE, conn))
                {
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transactionId;
                    try
                    {
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            oldStatusCode = Convert.ToString(obj);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateTransaction<exception>[check status]");
                        return TransactionResult.NotFound;
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_TRANSACTION, conn))
                {
                    //cmd.Parameters.Add("@result", MySqlDbType.VarChar, 100).Value			= result;
                    cmd.Parameters.Add("@status", MySqlDbType.VarChar, 100).Value = status;
                    cmd.Parameters.Add("@statuscode", MySqlDbType.VarChar, 100).Value = statusCode;
                    cmd.Parameters.Add("@merchant", MySqlDbType.VarChar, 50).Value = merchant;
                    cmd.Parameters.Add("@paymentid", MySqlDbType.VarChar, 100).Value = paymentid;
                    cmd.Parameters.Add("@transid", MySqlDbType.VarChar, 100).Value = transid;
                    cmd.Parameters.Add("@paymentmethod", MySqlDbType.VarChar, 100).Value = paymentmethod;
                    cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transactionId;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateTransaction<MySqlException>");
                        return TransactionResult.NotFound;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateTransaction<Exception>");
                        return TransactionResult.NotFound;
                    }
                }

                if (status == "OK" && paymentmethod.ToUpper() != "COUPON")	// Valid, completed transaction
                {
                    Util.CreateInvoice(Util.UserId, status, transid, paymentmethod, productInfo, ppi);
                    if (string.Compare(productInfo.Extra, "subscription", true) == 0)
                    {
                        // Customer bought a subscription, check whether we have to update
                        // his management rights.
                        using (MySqlCommand cmd = new MySqlCommand(QRY_GET_TRANSACTION_USERID, conn))
                        {
                            cmd.Parameters.Add("@transaction_id", MySqlDbType.Int64).Value = transactionId;
                            try
                            {
                                object obj = cmd.ExecuteScalar();
                                if (obj != null)
                                    userId = (long)obj;
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Write(LogLevel.Error, ex, "UpdateTransaction<exception>[check user id]");
                            }
                        }
                        if (!string.IsNullOrEmpty(oldStatusCode))
                        {
                            if (oldStatusCode.StartsWith("SUB"))
                            {
                                int credits;
                                oldStatusCode = oldStatusCode.Substring(3);
                                oldStatusCode = oldStatusCode.Trim('(', ')');
                                string[] parts = oldStatusCode.Split(':');
                                if (parts.Length > 1)
                                {
                                    if (int.TryParse(parts[1], out credits))
                                        productInfo.Credits = credits;
                                }
                            }
                        }
                    }
                }
            }
            if (userId > -1)
                RegisterUserRights(userId, 100, 100);

            return TransactionResult.Success;
        }

        public override void RegisterWithManager(long registerid, long userIdManager, long userIdPerformer)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_INSERT_INTO_MANAGED_REGISTER, conn))
                {
                    cmd.Parameters.Add("@register_id", MySqlDbType.Int64).Value = registerid;
                    cmd.Parameters.Add("@user_id_manager", MySqlDbType.Int64).Value = userIdManager;
                    cmd.Parameters.Add("@user_id_performer", MySqlDbType.Int64).Value = userIdPerformer;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.RegisterWithManager]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override string GetSocialAccessCode(long clientId)
        {
            string ret = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_GET_SOCIAL_ACCESS_CODE, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                try
                {
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                        ret = obj as string;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "GetSocialAccessCode");
                }
                finally
                {
                    conn.Close();
                }
            }
            return ret;
        }

        /* Social connector credentials format example
            <socialconnector>
              <credentials id='twitter'>
                <key name='client_id' value='...'/>
                <key name='client_secret' value='...'/>
                <key name='access_token' value='...'/>
              </credentials>
              <credentials id='facebook'>
                <key name='client_id' value='...'/>
                <key name='client_secret' value='...'/>
                <key name='access_token' value='...'/>
              </credentials>
              <credentials id='soundcloud'>
                <key name='client_id' value='...'/>
                <key name='client_secret' value='...'/>
                <key name='access_token' value='...'/>
              </credentials>
            </socialconnector>
         */
        private string getCredentialFromXml(string credentials, SocialConnector connector, string element)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(credentials);

            XmlElement elemSc = doc.FirstChild as XmlElement;
            if (elemSc != null)
            {
                XmlNodeList nl = elemSc.GetElementsByTagName(connector.ToString());
                if (nl.Count == 0)
                    return string.Empty;

                XmlElement elem = nl[0] as XmlElement;
                // If the element wasn't found we bail out with an empty string
                if (elem == null)
                    return string.Empty;

                XmlNodeList elemList = elem.GetElementsByTagName("key");
                // if there are no child elements, bail out with an empty string

                // search through the list for the specified child
                foreach (XmlNode node in elemList)
                {
                    XmlElement el = node as XmlElement;
                    if (el != null && (el.HasAttributes &&
                                       el.HasAttribute("name") &&
                                       el.GetAttribute("name") == element))
                        return el.GetAttribute("value");
                }
            }

            return string.Empty;
        }

        private string updateCredentialInXml(string credentials, SocialConnector connector, string element, string value)
        {
            if (string.IsNullOrEmpty(credentials))
                credentials = "<socialconnector></socialconnector>";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(credentials);
            XmlElement elemSc = xmlDoc.FirstChild as XmlElement;
            //connector.ToString()
            XmlNodeList nl = elemSc.GetElementsByTagName(connector.ToString());
            if (nl.Count == 0)
            {
                XmlElement elemCon = xmlDoc.CreateElement(connector.ToString());
                XmlElement el = xmlDoc.CreateElement("key");
                XmlAttribute attr = xmlDoc.CreateAttribute("name");
                attr.Value = element;
                el.Attributes.Append(attr);
                attr = xmlDoc.CreateAttribute("value");
                attr.Value = value;
                el.Attributes.Append(attr);
                elemCon.AppendChild(el);
                elemSc.AppendChild(elemCon);
            }
            else
            {
                XmlElement elem = nl[0] as XmlElement;
                if (elem != null)
                {
                    XmlNodeList elemList = elem.GetElementsByTagName("key");
                    bool found = false;
                    foreach (XmlNode node in elemList)
                    {
                        XmlElement el = node as XmlElement;
                        if (el.HasAttributes &&
                            el.HasAttribute("name") &&
                            el.GetAttribute("name") == element)
                        {
                            found = true;
                            el.SetAttribute("value", value);
                            break;
                        }
                    }
                    if (!found)
                    {
                        XmlElement el1 = xmlDoc.CreateElement("key");
                        XmlAttribute attr = xmlDoc.CreateAttribute("name");
                        attr.Value = element;
                        el1.Attributes.Append(attr);
                        attr = xmlDoc.CreateAttribute("value");
                        attr.Value = value;
                        el1.Attributes.Append(attr);
                        elem.AppendChild(el1);
                    }
                }
            }

            return xmlDoc.OuterXml;
        }

        private string deleteCredentialFromXml(string credentials, SocialConnector connector)
        {
            string xmlString = string.Empty;

            if (string.IsNullOrEmpty(credentials))
                credentials = "<socialconnector></socialconnector>";

            xmlString = DeleteSocialAttributes(credentials, connector);

            return xmlString;
        }

        private string DeleteSocialAttributes(string credentials, SocialConnector connector)
        {
            string xmlString = string.Empty;

            int startIndex = 0;
            int endIndex = 0;

            switch (connector)
            {
                case SocialConnector.Facebook:
                    startIndex = credentials.IndexOf("<Facebook>");
                    endIndex = credentials.IndexOf("</Facebook>");
                    credentials = credentials.Replace("</Facebook>", "");
                    break;
                case SocialConnector.Twitter:
                    startIndex = credentials.IndexOf("<Twitter>");
                    endIndex = credentials.IndexOf("</Twitter>");
                    credentials = credentials.Replace("</Twitter>", "");
                    break;
                case SocialConnector.SoundCloud:
                    startIndex = credentials.IndexOf("<SoundCloud>");
                    endIndex = credentials.IndexOf("</SoundCloud>");
                    credentials = credentials.Replace("</SoundCloud>", "");
                    break;
            }

            if (startIndex > -1 && endIndex > startIndex)
                xmlString = credentials.Remove(startIndex, endIndex - startIndex);
            else
                xmlString = credentials;

            return xmlString;
        }

        public override string GetSocialCredential(long clientId, SocialConnector connector, string element)
        {
            string ret = string.Empty;
            string accessCode = GetSocialAccessCode(clientId);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_GET_SOCIAL_ACCESS_CRED, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                try
                {
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        string credentials = obj as string;
                        string xmlText = Crypto.Decrypt(credentials, accessCode);

                        // At this point ret contains the xml with the connector access
                        // now we have to look for the proper connector and the specified element
                        ret = getCredentialFromXml(xmlText, connector, element);
                    }
                    else
                    {
                        ret = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "GetSocialCredentials");
                    ret = null;
                }
                finally
                {
                    conn.Close();
                }
            }

            return ret;
        }

        /*
            private XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(xmlText);

            XmlElement elem = xmlDoc.GetElementById("twitter");

            XmlNodeList elemList = elem.GetElementsByTagName("key");
            foreach (XmlNode node in elemList)
            {
              XmlElement el = node as XmlElement;
              string val = el.GetAttribute(element);
            }
         */

        public override void UpdateSocialCredential(long clientId, SocialConnector connector, string element, string credential)
        {
            // Here we know an element and a credential (value)
            // Now we have to add them to the existing credentials
            string accessCode = GetSocialAccessCode(clientId);
            string credentials = string.Empty;
            // First get the current credentials from the database
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_GET_SOCIAL_ACCESS_CRED, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                try
                {
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        string credEncrypted = obj as string;
                        if (!string.IsNullOrEmpty(credEncrypted))
                            credentials = Crypto.Decrypt(credEncrypted, accessCode);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "GetSocialCredentials");
                }
                finally
                {
                    conn.Close();
                }
            }

            credentials = updateCredentialInXml(credentials, connector, element, credential);

            string credentialsEncrypted = Crypto.Encrypt(credentials, accessCode);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_SOCIAL_ACCESS, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                cmd.Parameters.Add("@socialcred", MySqlDbType.Text).Value = credentialsEncrypted;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "UpdateSocialCredentials");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override void RemoveSocialCredential(long clientId, SocialConnector connector)
        {
            // Here we know an element and a credential (value)
            // Now we have to add them to the existing credentials
            string accessCode = GetSocialAccessCode(clientId);
            string credentials = string.Empty;
            // First get the current credentials from the database
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_GET_SOCIAL_ACCESS_CRED, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                try
                {
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    if (obj != null)
                    {
                        string credEncrypted = obj as string;
                        if (!string.IsNullOrEmpty(credEncrypted))
                            credentials = Crypto.Decrypt(credEncrypted, accessCode);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "GetSocialCredentials");
                }
                finally
                {
                    conn.Close();
                }
            }

            credentials = deleteCredentialFromXml(credentials, connector);

            string credentialsEncrypted = Crypto.Encrypt(credentials, accessCode);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_SOCIAL_ACCESS, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                cmd.Parameters.Add("@socialcred", MySqlDbType.Text).Value = credentialsEncrypted;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "UpdateSocialCredentials");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override bool UpdateFacebookID(long clientID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_FACEBOOK_ID, conn))
                {
                    cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientID;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateFacebookID");
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override bool UpdateTwitterID(long clientID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_TWITTER_ID, conn))
                {
                    cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientID;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateTwitterID");
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override bool UpdateSoundCloudID(long clientID)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_SOUNDCLOUD_ID, conn))
                {
                    cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientID;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateSoundCloudID");
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void UpdateSoundCloudId(long clientId, string soundCloudId)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_SOUND_CLOUD_USER_NAME, conn))
            {
                cmd.Parameters.Add("@client_id", MySqlDbType.Int64).Value = clientId;
                cmd.Parameters.Add("@soundcloudid", MySqlDbType.VarChar).Value = soundCloudId;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Write(LogLevel.Error, ex, "UpdateSoundCloudId");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public override DataSet GetRegisterWithManager(long userIdManager, long userIdPerformer)
        {
            DataSet dataSet = new DataSet("regtrack");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_MANAGED_REGISTER_BY_MANAGER_AND_USER, conn))
                {
                    cmd.Parameters.Add("@user_id_manager", MySqlDbType.Int64).Value = userIdManager;
                    cmd.Parameters.Add("@user_id_performer", MySqlDbType.Int64).Value = userIdPerformer;
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dataSet);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetRegisterWithManager]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dataSet;
        }

        public override DataSet GetRegisterWithManager(long userIdManager)
        {
            DataSet dataSet = new DataSet("regtrack");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_MANAGED_REGISTER_BY_MANAGER, conn))
                {
                    cmd.Parameters.Add("@user_id_manager", MySqlDbType.Int64).Value = userIdManager;
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(dataSet);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[MySqlDatabase.GetRegisterWithManager]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return dataSet;
        }

        public override bool CheckActivationCode(string activationCode)
        {
            bool ret = false;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CHECK_ACTIVATIONCODE, conn))
                {
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar, 20).Value = activationCode;

                    try
                    {
                        conn.Open();
                        object res = cmd.ExecuteScalar();
                        if (res != null)
                        {
                            int val = Convert.ToInt32(res);
                            ret = (val > 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "CheckActivationCode<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override void MarkActivationCode(string activationCode, long userid)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_MARK_ACTIVATIONCODE, conn))
                {
                    cmd.Parameters.Add("@code", MySqlDbType.VarChar, 20).Value = activationCode;
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "MarkActivationCode<Exception>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override int ChangePassword(string username, string applicationname, string password)
        {
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_PASSWORD, conn))
                {
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = password;
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "ChangePassword");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override int ChangePasswordQuestionAndAnswer(string username, string applicationname, string question, string answer)
        {
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_PASSWORDQA, conn))
                {
                    cmd.Parameters.Add("@passwordquestion", MySqlDbType.VarChar, 200).Value = question;
                    cmd.Parameters.Add("@passwordanswer", MySqlDbType.VarChar, 200).Value = answer;
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "ChangePasswordQuestionAndAnswer");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override bool DeleteUser(string username, string applicationname, bool deleteAllRelatedData)
        {
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_DELETE_USER_BY_USERNAME_AND_APPLICATION, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                        if (deleteAllRelatedData)
                        {
                            // Process commands to delete all data
                        }
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "DeleteUser");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override int GetUserCount(string applicationname)
        {
            int res = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_COUNT_BY_APPLICATION, conn))
                {
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        res = (int)cmd.ExecuteScalar();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetUserCount<MySqlException>");
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
        public override MembershipUserCollection GetAllUsers(string providerName, string applicationname, int pageIndex, int pageSize)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_ALL_USER_BY_APPLICATION, conn))
                {
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    MySqlDataReader reader = null;
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
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "GetAllUsers<MySqlException>");
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

        public override int GetNumberOfUsersOnline(string applicationName)
        {
            int numOnline;
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USERS_ONLINE, conn))
                {
                    cmd.Parameters.Add("@lastactivitydate", MySqlDbType.DateTime).Value = compareTime;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationName;

                    try
                    {
                        conn.Open();
                        numOnline = (int)cmd.ExecuteScalar();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "GetNumberOfUsersOnline");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override string GetPassword(string username, string answer, string applicationname, out string passwordAnswer)
        {
            string password;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_PASSWORD, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    passwordAnswer = string.Empty;
                    MySqlDataReader reader = null;
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
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "GetPassword");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override MembershipUser GetUser(string providerName, string username, string applicationname, bool userIsOnline)
        {
            MembershipUser u = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_BY_NAME_AND_APPLICATION, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    MySqlDataReader reader = null;

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
                                MySqlCommand updateCmd = new MySqlCommand(QRY_UPDATE_USER_ACTIVITY, conn);
                                updateCmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                                updateCmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                                updateCmd.ExecuteNonQuery();
                            }
                            HttpContext.Current.Session["userid"] = u.ProviderUserKey;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "GetUser(string, bool)");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override MembershipUser GetUser(string providerName, object providerUserKey, string applicationname, bool userIsOnline)
        {
            MembershipUser u = null;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_BY_USER_ID, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = (long)providerUserKey;
                    MySqlDataReader reader = null;

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
                                using (MySqlCommand updateCmd = new MySqlCommand(QRY_UPDATE_USER_ACTIVITY, conn))
                                {
                                    updateCmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = u.UserName;
                                    updateCmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            HttpContext.Current.Session["userid"] = (int)u.ProviderUserKey;
                        }
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "GetUser(object, bool)");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override bool UnlockUser(string username, string applicationname)
        {
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_UNLOCK, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "UnlockUser");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override string GetUserNameByEmail(string email, string applicationname)
        {
            string username;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USERNAME_BY_EMAIL_AND_APPLICATIONNAME, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = email;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        username = (string)cmd.ExecuteScalar();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "GetUserNameByEmail");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            return username ?? string.Empty;
        }

        public override long GetUserIdByEmail(string email)
        {
            long userId = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USERID_BY_EMAIL, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = email;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                            userId = Convert.ToInt64(obj);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetUserIdByEmail]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return userId;
        }

        public override string ResetPassword(
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
            int rowsAffected;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_USER_CREDENTIALS, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    MySqlDataReader reader = null;

                    try
                    {
                        conn.Open();
                        reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        string passwordAnswer;
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
                            UpdateFailureCount(username, applicationname, "passwordAnswer", passwordAttemptWindow,
                                               maxInvalidPasswordAttempts);
                            throw new MembershipPasswordException("Incorrect password answer");
                        }
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "ResetPassword");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        if (reader != null)
                            reader.Close();
                        conn.Close();
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_CREDENTIALS, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                    cmd.Parameters.Add("@password", MySqlDbType.VarChar, 200).Value = newPassword;

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "ResetPassword");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            if (rowsAffected > 0)
                return newPassword;

            throw new MembershipPasswordException("User not found, or user is locked out. Password not reset.");
        }

        public override void UpdateUser(MembershipUser user, string applicationname)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_APPROVAL, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = user.Email;
                    cmd.Parameters.Add("@comment", MySqlDbType.Text).Value = user.Comment;
                    cmd.Parameters.Add("@isapproved", MySqlDbType.Int16).Value = user.IsApproved ? 1 : 0;
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = user.UserName;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(ex, "UpdateUser");
                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void UpdateUserLogon(string username, string applicationname)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_USER_LOGON, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "UpdateUserLogon<MySqlException>");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public override void UpdateFailureCount(
            string username,
            string applicationname,
            string failureType,
            int passwordAttemptWindow,
            int maxInvalidPasswordAttempts)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_FAILURE_COUNT, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    DateTime windowStart = new DateTime();
                    int failureCount = 0;

                    try
                    {
                        conn.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
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

                            string query = string.Empty;
                            if (failureType == "password")
                                query = QRY_UPDATE_USER_RESET_PASSWORD_FAILURE;

                            if (failureType == "passwordAnswer")
                                query = QRY_UPDATE_USER_RESET_PASSWORDANSWER_FAILURE;

                            cmd.CommandText = query;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                            cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                            if (cmd.ExecuteNonQuery() < 0)
                                throw new ProviderException("Unable to update failure count and window start.");
                        }
                        else
                        {
                            if (failureCount++ >= maxInvalidPasswordAttempts)
                            {
                                // Password attempts have exceeded the failure threshold. Lock out
                                // the user.
                                cmd.CommandText = QRY_UPDATE_USER_LOCK_OUT;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                                cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                                if (cmd.ExecuteNonQuery() < 0)
                                    throw new ProviderException("Unable to lock out user.");
                            }
                            else
                            {
                                // Password attempts have not exceeded the failure threshold. Update
                                // the failure counts. Leave the window the same.
                                string query = string.Empty;
                                if (failureType == "password")
                                    query = QRY_UPDATE_USER_SET_PASSWORD_FAILURE;

                                if (failureType == "passwordAnswer")
                                    query = QRY_UPDATE_USER_SET_PASSWORDANSWER_FAILURE;

                                cmd.CommandText = query;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("");
                                cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;
                                cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                                cmd.Parameters.Add("@failurecount", MySqlDbType.Int32).Value = failureCount;

                                if (cmd.ExecuteNonQuery() < 0)
                                    throw new ProviderException("Unable to update failure count.");
                            }
                        }
                    }
                    catch (MySqlException e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "UpdateFailureCount");

                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        protected override bool CheckPassword(string password, string dbpassword, MembershipPasswordFormat passwordFormat, TrackProtectMembershipProvider provider)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (passwordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = provider.UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass2 = provider.EncodePassword(password);
                    break;
            }

            return (pass1 == pass2);
        }

        public override MembershipUserCollection FindUsersByName(string providerName, string usernameToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_USER_COUNT_BY_NAME, conn))
                {
                    cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = usernameToMatch;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;

                    MySqlDataReader reader = null;

                    try
                    {
                        conn.Open();
                        totalRecords = (int)cmd.ExecuteScalar();

                        if (totalRecords <= 0) { return users; }

                        cmd.CommandText = QRY_GET_USER_BY_NAME_AND_APPLICATION_ORDERED;

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
                    catch (MySqlException e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "FindUsersByName");

                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override MembershipUserCollection FindUsersByEmail(string providerName, string emailToMatch, string applicationname, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_USER_COUNT_BY_EMAIL, conn))
                {
                    cmd.Parameters.Add("@email", MySqlDbType.VarChar, 200).Value = emailToMatch;
                    cmd.Parameters.Add("@applicationname", MySqlDbType.VarChar, 100).Value = applicationname;
                    MySqlDataReader reader = null;
                    totalRecords = 0;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                            totalRecords = Convert.ToInt32(obj);

                        if (totalRecords <= 0)
                            return users;

                        cmd.CommandText = QRY_GET_USER_BY_EMAIL_AND_APPLICATION_ORDERED;

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
                    catch (MySqlException e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "FindUsersByEmail");

                            throw new ProviderException(EXCEPTION_MESSAGE);
                        }
                        else
                        {
                            throw;
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

        public override string GetUserEmail(long userid)
        {
            UserInfo ui = GetUser(userid);
            if (ui == null)
                return string.Empty;
            return ui.Email;
        }

        public override string GetUserPassword(long userid)
        {
            UserInfo ui = GetUser(userid);
            if (ui == null)
                return string.Empty;
            return ui.Password;
        }

        protected override MembershipUser GetUserFromReader(string providerName, IDataReader reader)
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
            short isApproved = reader.GetInt16(5);
            bool isLockedOut = reader.GetInt16(6) > 0;
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
                (isApproved > 0),
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockedOutDate);

            return u;
        }

        protected override string GetConnectionString()
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["ApplicationServices"];
            if (css == null || css.ConnectionString.Trim() == string.Empty)
                throw new Exception("Can't access database");
            return css.ConnectionString;
        }

        public override bool RelationExists(long requestingUserId, long requestedUserId, int relationType)
        {
            bool ret = false;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CHECK_RELATION, conn))
                {
                    cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = requestingUserId;
                    cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = requestedUserId;
                    cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            int tmp = Convert.ToInt32(obj);
                            ret = (tmp > 0);

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[RelationExists]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override bool ConfirmationExists(long requestingUserId, string requestedEmail)
        {
            bool ret = false;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_CHECK_Confirmation, conn))
                {
                    cmd.Parameters.Add("@requestinguserid", MySqlDbType.Int64).Value = requestingUserId;
                    cmd.Parameters.Add("@requestedemail", MySqlDbType.VarChar).Value = requestedEmail;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            int tmp = Convert.ToInt32(obj);
                            ret = (tmp > 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[ConfirmationExists]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ret;
        }

        public override void RequestConfirmation(string guid, long requestingUserId, long requestedUserId, string email, int relationType)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_INSERT_CONFIRM, conn))
                {
                    cmd.Parameters.Add("@unique_id", MySqlDbType.VarChar, 100).Value = guid;
                    cmd.Parameters.Add("@requestinguserid", MySqlDbType.Int64).Value = requestingUserId;
                    cmd.Parameters.Add("@requestedemail", MySqlDbType.VarChar, 100).Value = email;
                    cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[RequestConfirmation]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        internal class RequestInfo
        {
            public long RequestingUserId { get; set; }
            public string RequestedEmail { get; set; }
            public long RequestedUserId { get; set; }
        };

        const string QRY_DELETE_RELATION = @"
            DELETE FROM `relation` WHERE `owner_user_id`=@owneruserid AND `owned_user_id`=@owneduserid AND `relationtype`=@relationtype;
        ";
        public override void DeleteRelation(long userId1, long userId2, int relationType)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_DELETE_RELATION, conn))
                {
                    cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = userId1;
                    cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = userId2;
                    cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;


                    try
                    {
                        conn.Open();
                        if (relationType == 2)
                        {
                            cmd.Parameters["@relationtype"].Value = 1;
                            cmd.ExecuteNonQuery();

                            cmd.Parameters["@owneruserid"].Value = userId2;
                            cmd.Parameters["@owneduserid"].Value = userId1;

                            cmd.ExecuteNonQuery();
                        }
                        else
                        {

                            cmd.ExecuteNonQuery();

                            if (relationType == 1)
                            {
                                cmd.Parameters["@relationtype"].Value = 0;
                                cmd.ExecuteNonQuery();
                                relationType = 0;
                            }



                            cmd.Parameters["@owneruserid"].Value = userId2;
                            cmd.Parameters["@owneduserid"].Value = userId1;

                            cmd.ExecuteNonQuery();

                            if (relationType == 0)
                            {
                                cmd.Parameters["@relationtype"].Value = 1;
                                cmd.ExecuteNonQuery();
                            }



                            cmd.Parameters["@owneruserid"].Value = userId2;
                            cmd.Parameters["@owneduserid"].Value = userId1;

                            cmd.ExecuteNonQuery();

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "DeleteRelation");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }


        public override void getEmailByUniqueId(string guid, out string emailRequested)
        {
            emailRequested = string.Empty;

            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_CONFIRM, conn))
                {
                    cmd.Parameters.Add("@unique_id", MySqlDbType.VarChar, 100).Value = guid;

                    conn.Open();

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            emailRequested = rdr.GetString(3);
                        }
                    }
                }
            }
        }

        public override ConfirmationResult ProcessConfirmation(string guid, int relationType, out string emailRequested, out string emailRequesting)
        {
            emailRequested = string.Empty;
            emailRequesting = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_SELECT_CONFIRM, conn))
                {
                    cmd.Parameters.Add("@unique_id", MySqlDbType.VarChar, 100).Value = guid;

                    try
                    {
                        List<RequestInfo> requests = new List<RequestInfo>();

                        conn.Open();
                        long requestingUserId = -1;
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                requestingUserId = rdr.GetInt64(2);
                                emailRequested = rdr.GetString(3);

                                requests.Add(
                                    new RequestInfo
                                    {
                                        RequestingUserId = requestingUserId,
                                        RequestedEmail = emailRequested,
                                        RequestedUserId = 0
                                    });
                            }
                        }

                        if (requests.Count == 0)
                            return ConfirmationResult.ConfirmationFailed;

                        cmd.CommandText = "SELECT `email` FROM `user` WHERE `user_id`=@user_id";
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = requestingUserId;
                        object objEmail = cmd.ExecuteScalar();
                        if (objEmail != null)
                            emailRequesting = objEmail as string;

                        bool found = false;
                        foreach (RequestInfo request in requests)
                        {
                            cmd.CommandText = "SELECT `user_id` FROM `user` WHERE `email`=@email";
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@email", MySqlDbType.VarChar, 100).Value =
                                request.RequestedEmail;
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                request.RequestedUserId = Convert.ToInt64(obj);
                                found = true;
                            }
                        }

                        //TODO: Bij een nieuw aangemaakt user zal deze nooit bestaan. Maar dan returned hij deze en dan stop het vervolgproces.
                        // Terwijl het de bedoeling is om juist een gebruiker die nog niet bestaat aan te maken. -Lambert 2012-12-1
                        if (!found)
                            return ConfirmationResult.UserUnknown;

                        foreach (RequestInfo request in requests)
                        {
                            if (relationType == 1)
                            {
                                cmd.CommandText = QRY_INSERT_USER_RELATION;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = request.RequestingUserId;
                                cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = request.RequestedUserId;
                                cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;
                                cmd.ExecuteNonQuery();


                            }

                            relationType = 0;

                            // Check whether a relation of this nature already exists
                            bool relationExists = false;
                            cmd.CommandText = QRY_CHECK_RELATION;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = request.RequestingUserId;
                            cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = request.RequestedUserId;
                            cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;
                            object obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                int count = Convert.ToInt32(obj);
                                relationExists = (count > 0);
                            }

                            if (!relationExists)
                            {
                                cmd.CommandText = QRY_INSERT_USER_RELATION;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = request.RequestingUserId;
                                cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = request.RequestedUserId;
                                cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;
                                cmd.ExecuteNonQuery();


                            }

                            cmd.CommandText = QRY_CHECK_RELATION;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = request.RequestedUserId;
                            cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = request.RequestingUserId;
                            cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;
                            obj = cmd.ExecuteScalar();
                            if (obj != null)
                            {
                                int count = Convert.ToInt32(obj);
                                relationExists = (count > 0);
                            }

                            if (!relationExists)
                            {
                                cmd.CommandText = QRY_INSERT_USER_RELATION;
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@owneruserid", MySqlDbType.Int64).Value = request.RequestedUserId;
                                cmd.Parameters.Add("@owneduserid", MySqlDbType.Int64).Value = request.RequestingUserId;
                                cmd.Parameters.Add("@relationtype", MySqlDbType.Int32).Value = relationType;
                                cmd.ExecuteNonQuery();
                            }

                        }

                        cmd.CommandText = QRY_DELETE_CONFIRM;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@unique_id", MySqlDbType.VarChar, 100).Value = guid;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[ProcessConfirmation]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return ConfirmationResult.Success;
        }

        public override DataTable GetAllOpenQuotations()
        {
            DataTable table = new DataTable("quotations");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_QUOTATIONS, conn))
                {
                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetAllQuotations]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return table;
        }

        public override int DetermineRelationType(long userId1, long userId2)
        {
            int relType = 0;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_RELATIONTYPE, conn))
                {
                    cmd.Parameters.Add("owner_user_id", MySqlDbType.Int64).Value = userId1;
                    cmd.Parameters.Add("owned_user_id", MySqlDbType.Int64).Value = userId2;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                        {
                            relType = Convert.ToInt32(obj);
                            if (relType == 0)
                            {
                                cmd.Parameters["owner_user_id"].Value = userId2;
                                cmd.Parameters["owned_user_id"].Value = userId1;
                                obj = cmd.ExecuteScalar();
                                relType = Convert.ToInt32(obj);
                                if (relType > 0)
                                {
                                    relType = 2;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[DetermineRelationType]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return relType;
        }

        private const string QRY_UPDATE_CONFIRMATION_EMAIL =
            "UPDATE `confirmation` SET `requested_email`=@requested_email WHERE `unique_id`=@unique_id";

        public override void UpdateConfirmation(Guid confirmationid, string email)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_UPDATE_CONFIRMATION_EMAIL, conn))
                {
                    cmd.Parameters.Add("@requested_email", MySqlDbType.VarChar, 100).Value = email;
                    cmd.Parameters.Add("@unique_id", MySqlDbType.VarChar, 100).Value = confirmationid;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[UpdateConfirmation]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private const string QRY_GET_INVITATIONS = @"
            SELECT * FROM `confirmation` WHERE `requested_email`=(SELECT `email` FROM `user` WHERE `user_id`=@user_id)
        ";

        public override DataTable GetInvitations(long userId)
        {
            DataTable table = new DataTable("confirmation");
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_INVITATIONS, conn))
                {
                    cmd.Parameters.Add("@user_id", MySqlDbType.Int64).Value = userId;

                    try
                    {
                        conn.Open();
                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            da.Fill(table);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetInvitations]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return table;
        }

        public override string GetConfirmationUid(long confirmationId)
        {
            string uid = string.Empty;
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(QRY_GET_CONFIRMATION_UID, conn))
                {
                    cmd.Parameters.Add("@confirmation_id", MySqlDbType.Int64).Value = confirmationId;

                    try
                    {
                        conn.Open();
                        object obj = cmd.ExecuteScalar();
                        if (obj != null)
                            uid = Convert.ToString(obj);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetConfirmationUid]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return uid;
        }


        public override void DeleteInvitation(long confirmationId)
        {
            using (MySqlConnection conn = new MySqlConnection(GetConnectionString()))
            {
                using (MySqlCommand cmd = new MySqlCommand("Delete from `confirmation` Where `confirmation_id`= @confirmation_id", conn))
                {
                    cmd.Parameters.Add("confirmation_id", MySqlDbType.Int64).Value = confirmationId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Write(LogLevel.Error, ex, "[GetInvitations]");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}