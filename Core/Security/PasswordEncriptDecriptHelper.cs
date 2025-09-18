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
        /// Encrypts the specified secure password using the provided algorithm.
        /// </summary>
        /// <param name="source">Secure text that must not be <see langword="null"/> and contains the password to protect.</param>
        /// <param name="encriptMethod">Encryption algorithm to apply; defaults to <see cref="EncriptMethod.Base64"/>.</param>
        /// <returns>A Base64-encoded cipher text representation of the original secure password.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="CryptographicException">Thrown when the selected encryption algorithm cannot encrypt the value.</exception>
        // Encripta una cadena
        public static string Encript(this SecureString source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Encript(source, encriptMethod);

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public static SecureString Decript(this string source, EncriptMethod encriptMethod = EncriptMethod.Base64)
            => new PasswordEncriptDecript().Decript(source, encriptMethod);
    }
}