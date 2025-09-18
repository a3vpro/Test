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
using System.Security;

namespace VisionNet.Core.Security
{
    public static class PasswordEncriptDecriptHelper
    {
        /// <summary>
        /// Encrypts the contents of the provided <see cref="SecureString"/> using the specified encryption algorithm.
        /// </summary>
        /// <param name="source">Secure string containing the plaintext to encrypt. Must not be <see langword="null"/>; may be empty to produce an empty result.</param>
        /// <param name="encriptMethod">Encryption strategy to apply. Defaults to <see cref="EncriptMethod.Base64"/>; use <see cref="EncriptMethod.ECB"/> for TripleDES ECB mode.</param>
        /// <returns>A non-null string containing the encrypted representation of <paramref name="source"/>. The value is empty when the input is empty.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException">Thrown when the encryption provider cannot be initialized or fails while processing <paramref name="source"/> when using <see cref="EncriptMethod.ECB"/>.</exception>
        public static string Encript(this SecureString source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Encript(source, encriptMethod);

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public static SecureString Decript(this string source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Decript(source, encriptMethod);
    }
}