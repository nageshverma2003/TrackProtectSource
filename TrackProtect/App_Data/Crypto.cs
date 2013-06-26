using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Crypto
{
    private static readonly byte[] IVa = Encoding.ASCII.GetBytes("TrackProtect!");
    public const string DEFAULT_SALT = "t8JVZbsYoTWSGDq6EkqqulFN/FZ9dFhaTH/kmatrG6E=";

    public static string Encrypt(this string text, string salt)
    {
        if (string.IsNullOrEmpty(salt))
            salt = DEFAULT_SALT;

        try
        {
            using (Aes aes = new AesManaged())
            {
                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(IVa, 0, IVa.Length), Encoding.UTF8.GetBytes(salt));
                aes.Key = deriveBytes.GetBytes(128 / 8);
                aes.IV = aes.Key;
                using (MemoryStream encryptionStream = new MemoryStream())
                {
                    using (CryptoStream encrypt = new CryptoStream(encryptionStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] cleanText = Encoding.UTF8.GetBytes(text);
                        encrypt.Write(cleanText, 0, cleanText.Length);
                        encrypt.FlushFinalBlock();
                    }

                    byte[] encryptedData = encryptionStream.ToArray();
                    string encryptedText = Convert.ToBase64String(encryptedData);


                    return encryptedText;
                }
            }
        }
        catch
        {
            return String.Empty;
        }
    }

    public static string Decrypt(this string text, string salt)
    {
        if (string.IsNullOrEmpty(salt))
            salt = DEFAULT_SALT;
        
        try
        {
            using (Aes aes = new AesManaged())
            {
                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(IVa, 0, IVa.Length), Encoding.UTF8.GetBytes(salt));
                aes.Key = deriveBytes.GetBytes(128 / 8);
                aes.IV = aes.Key;

                using (MemoryStream decryptionStream = new MemoryStream())
                {
                    using (CryptoStream decrypt = new CryptoStream(decryptionStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byte[] encryptedData = Convert.FromBase64String(text);


                        decrypt.Write(encryptedData, 0, encryptedData.Length);
                        decrypt.Flush();
                    }

                    byte[] decryptedData = decryptionStream.ToArray();
                    string decryptedText = Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);


                    return decryptedText;
                }
            }
        }
        catch
        {
            return String.Empty;
        }
    }

    public static string ComputeHash(string input, HashAlgorithm algorithm)
    {
        byte[] salt = Convert.FromBase64String(DEFAULT_SALT);
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Combine salt and input bytes
        byte[] saltedInput = new Byte[salt.Length + inputBytes.Length];
        salt.CopyTo(saltedInput, 0);
        inputBytes.CopyTo(saltedInput, salt.Length);

        byte[] hashedBytes = algorithm.ComputeHash(saltedInput);
        return Convert.ToBase64String(hashedBytes);
    }
}