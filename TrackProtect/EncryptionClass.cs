using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace TrackProtect
{
    public class EncryptionClass
    {
        #region Variable/Properties ------- !

        private const string cryptoKey = "cryptoKey";

        //------- The Initialization Vector for the DES encryption routine ------- !
        private static readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 255, 76, 173, 57 };

        #endregion


        #region Method ------- !

        #region Encryption Method ------- !

        /// <summary>
        /// Encrypts to provided string parameter.
        /// </summary>
        public static string Encrypt(string s)
        {
            if (s == null || s.Length == 0) return string.Empty;

            string result = string.Empty;

            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);

                TripleDESCryptoServiceProvider des =
                    new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 =
                    new MD5CryptoServiceProvider();

                des.Key =
                    MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

                des.IV = IV;
                result = Convert.ToBase64String(
                    des.CreateEncryptor().TransformFinalBlock(
                        buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }

            return result;
        }

        #endregion


        #region Decryption Method ------- !

        /// <summary>
        /// Decrypts to provided string parameter.
        /// </summary>
        public static string Decrypt(string s)
        {
            if (s == null || s.Length == 0) return string.Empty;

            string result = string.Empty;

            try
            {
                byte[] buffer = Convert.FromBase64String(s);

                TripleDESCryptoServiceProvider des =
                    new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 =
                    new MD5CryptoServiceProvider();

                des.Key =
                    MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

                des.IV = IV;

                result = Encoding.ASCII.GetString(
                    des.CreateDecryptor().TransformFinalBlock(
                    buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }

            return result;
        }

        #endregion

        #endregion
    }
}