//----------------------------------------------------------------------------
// Author           : Álvaro Ibáñez del Pino
// NickName         : aibanez
// Created          : 28-08-2021
//
// Last Modified By : aibanez
// Last Modified On : 18-11-2023
// Description      : v1.7.1
//
// Copyright        : (C)  2023 by Sothis/Nunsys. All rights reserved.       
//----------------------------------------------------------------------------
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using VisionNet.Core.Encoding;

namespace VisionNet.Core.Security
{
    public class PasswordEncriptDecript
    {
        private SecureString _securityKey = new SecureString();

        
        /// <summary> The PasswordEncriptDecript function encrypts and decrypts a password.</summary>
        /// <returns> A string</returns>
        public PasswordEncriptDecript()
        {
            _securityKey.AppendChar('S');
            _securityKey.AppendChar('o');
            _securityKey.AppendChar('t');
            _securityKey.AppendChar('h');
            _securityKey.AppendChar('i');
            _securityKey.AppendChar('s');
            _securityKey.AppendChar('1');
            _securityKey.AppendChar('9');
            _securityKey.AppendChar('.');
        }

        // Encripta una cadena
        
        /// <summary> The Encript function takes a SecureString and returns an encrypted string.</summary>
        /// <param name="source"> The string to be decrypted</param>
        /// <param name="encriptMethod"> Encriptmethod encriptmethod</param>
        /// <returns> A string</returns>
        public string Encript(SecureString source, EncriptMethod encriptMethod = EncriptMethod.Base64)
        {
            // GUARD: Ensure the source SecureString contains data before encryption.
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Length == 0)
            {
                throw new ArgumentException("Source cannot be empty.", nameof(source));
            }

            switch (encriptMethod)
            {
                case EncriptMethod.Base64:
                default:
                    return new Base64EncoderDecoder().Encode(source.FromSecureString());

                case EncriptMethod.ECB:

                    // Getting the bytes of Input String.
                    byte[] toEncryptedArray = UTF8Encoding.UTF8.GetBytes(source.FromSecureString());

                    MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();
                    //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                    byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey.FromSecureString()));
                    //De-allocatinng the memory after doing the Job.
                    objMD5CryptoService.Clear();

                    var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
                    //Assigning the Security key to the TripleDES Service Provider.
                    objTripleDESCryptoService.Key = securityKeyArray;
                    //Mode of the Crypto service is Electronic Code Book.
                    objTripleDESCryptoService.Mode = CipherMode.ECB;
                    //Padding Mode is PKCS7 if there is any extra byte is added.
                    objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

                    var objCrytpoTransform = objTripleDESCryptoService.CreateEncryptor();
                    //Transform the bytes array to resultArray
                    byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                    objTripleDESCryptoService.Clear();
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        
        /// <summary> The Decript function takes a string and an EncriptMethod enum as parameters.
        /// The function then uses the switch statement to determine which encription method was used.
        /// If the Base64 encription method is used, it will return a SecureString object that contains 
        /// the decrypted string.</summary>
        /// <param name="source"> The string to be decrypted</param>
        /// <param name="encriptMethod"> The type of encription to use.  default is base64</param>
        /// <returns> A securestring</returns>
        public SecureString Decript(string source, EncriptMethod encriptMethod = EncriptMethod.Base64)
        {
            // GUARD: Ensure the source string contains data before decryption.
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Length == 0)
            {
                throw new ArgumentException("Source cannot be empty.", nameof(source));
            }

            switch (encriptMethod)
            {
                case EncriptMethod.Base64:
                default:
                    return new Base64EncoderDecoder().Decode(source).ToSecureString();

                case EncriptMethod.ECB:

                    byte[] toEncryptArray = Convert.FromBase64String(source);
                    MD5CryptoServiceProvider objMD5CryptoService = new MD5CryptoServiceProvider();

                    //Gettting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                    byte[] securityKeyArray = objMD5CryptoService.ComputeHash(UTF8Encoding.UTF8.GetBytes(_securityKey.FromSecureString()));
                    objMD5CryptoService.Clear();

                    var objTripleDESCryptoService = new TripleDESCryptoServiceProvider();
                    //Assigning the Security key to the TripleDES Service Provider.
                    objTripleDESCryptoService.Key = securityKeyArray;
                    //Mode of the Crypto service is Electronic Code Book.
                    objTripleDESCryptoService.Mode = CipherMode.ECB;
                    //Padding Mode is PKCS7 if there is any extra byte is added.
                    objTripleDESCryptoService.Padding = PaddingMode.PKCS7;

                    var objCrytpoTransform = objTripleDESCryptoService.CreateDecryptor();
                    //Transform the bytes array to resultArray
                    byte[] resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    objTripleDESCryptoService.Clear();

                    //Convert and return the decrypted data/byte into string format.
                    return UTF8Encoding.UTF8.GetString(resultArray).ToSecureString();
            }
        }
    }
}