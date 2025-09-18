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
using System.Net;
using System.Security;

namespace VisionNet.Core.Security
{
    public static class SecureStringHelper
    {
        /// <summary>
        /// Converts the provided <see cref="SecureString"/> into a managed <see cref="string"/> instance by leveraging <see cref="NetworkCredential"/>.
        /// </summary>
        /// <param name="source">Secure text to convert. May be empty, but must not be disposed when invoked.</param>
        /// <returns>A managed string containing the decrypted value of <paramref name="source"/>. Returns <see cref="string.Empty"/> when the secure string is empty.</returns>
        /// <exception cref="ObjectDisposedException">Thrown when <paramref name="source"/> has already been disposed.</exception>
        /// <remarks>The resulting string resides in managed memory and should be handled carefully to avoid lingering sensitive data.</remarks>
        public static string FromSecureString(this SecureString source) => new NetworkCredential("", source).Password;

        /// <summary>
        /// Converts the specified <see cref="string"/> into a new <see cref="SecureString"/> by assigning it through <see cref="NetworkCredential"/>.
        /// </summary>
        /// <param name="source">Plain text value to protect. May be <see langword="null"/>, in which case an empty secure string is returned.</param>
        /// <returns>A new <see cref="SecureString"/> instance whose contents mirror <paramref name="source"/>.</returns>
        /// <exception cref="OutOfMemoryException">Thrown when the runtime cannot allocate the secure string.</exception>
        /// <remarks>The caller is responsible for disposing the returned secure string as soon as it is no longer required.</remarks>
        public static SecureString ToSecureString(this string source) => new NetworkCredential("", source).SecurePassword;

        
        /// <summary> The IsEqual function compares two SecureStrings for equality.
        /// It does this by using a HMACSHA256 hash to compare the strings, and then 
        /// overwrites the memory used by both strings with zeros before returning.</summary>
        /// <param name="ss1"> The first securestring to compare</param>
        /// <param name="ss2"> The second securestring to compare</param>
        /// <returns> A boolean value that indicates whether the two securestring objects are equal.</returns>
        public static bool IsEqual(this SecureString ss1, SecureString ss2)
        {
            return CompareSecureStringsWithHmac.IsEqual(ss1, ss2);
        }
    }
}