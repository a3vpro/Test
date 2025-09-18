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
    /// <summary>
    /// Provides helpers to encrypt and decrypt passwords backed by <see cref="SecureString"/> values
    /// using Base64 encoding or a TripleDES cipher in ECB mode.
    /// </summary>
    public class PasswordEncriptDecript
    {
        private SecureString _securityKey = new SecureString();

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordEncriptDecript"/> class with the
        /// internal security key used for TripleDES encryption and decryption.
        /// </summary>
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
        
        /// <summary>
        /// Encrypts the provided password using the selected algorithm, returning the result as a
        /// Base64-encoded string.
        /// </summary>
        /// <param name="source">The plain-text password stored in a <see cref="SecureString"/>. The value must not be <c>null</c>.</param>
        /// <param name="encriptMethod">Selects the encryption algorithm: Base64 encoding or TripleDES in ECB mode with PKCS7 padding.</param>
        /// <returns>A Base64 string that represents the encrypted password.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="CryptographicException">Thrown when the TripleDES provider cannot be created or encryption fails when using the ECB mode.</exception>
        public string Encript(SecureString source, EncriptMethod encriptMethod = EncriptMethod.Base64)
        {
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

        /// <summary>
        /// Decrypts an encrypted password using the specified algorithm and restores it into a
        /// <see cref="SecureString"/> instance.
        /// </summary>
        /// <param name="source">The Base64 string produced by <see cref="Encript(SecureString, EncriptMethod)"/>.</param>
        /// <param name="encriptMethod">Identifies the encryption algorithm that was used to produce <paramref name="source"/>.</param>
        /// <returns>A <see cref="SecureString"/> that contains the decrypted plain-text password.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="source"/> is not valid Base64.</exception>
        /// <exception cref="CryptographicException">Thrown when the TripleDES provider cannot be created or decryption fails when using the ECB mode.</exception>
        public SecureString Decript(string source, EncriptMethod encriptMethod = EncriptMethod.Base64)
        {
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