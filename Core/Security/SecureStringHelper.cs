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
using System.Net;
using System.Security;

namespace VisionNet.Core.Security
{
    public static class SecureStringHelper
    {
        /// <summary>
        /// Converts the contents of a <see cref="SecureString"/> to a plain <see cref="string"/> by
        /// leveraging <see cref="NetworkCredential"/> to unwrap the protected value. The resulting
        /// string resides in managed memory and should be cleared or disposed promptly to avoid
        /// leaving sensitive data accessible.
        /// </summary>
        /// <param name="source">The secure text to convert. The value must not be <see langword="null"/> and must not have been disposed.</param>
        /// <returns>The plaintext representation of <paramref name="source"/>; returns <see cref="string.Empty"/> when the secure string is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when <paramref name="source"/> has already been disposed.</exception>
        public static string FromSecureString(this SecureString source)
        {
            if (source == null)
            {
                // GUARD: SecureString extensions must not accept null instances.
                throw new ArgumentNullException(nameof(source));
            }

            return new NetworkCredential("", source).Password;
        }

        /// <summary>
        /// Converts a plaintext <see cref="string"/> into a <see cref="SecureString"/> by assigning the
        /// value to a <see cref="NetworkCredential"/> and retrieving its protected representation. The
        /// caller is responsible for disposing the returned secure string to release the sensitive data
        /// from memory.
        /// </summary>
        /// <param name="source">The plaintext value to convert. A <see langword="null"/> value results in an empty <see cref="SecureString"/>.</param>
        /// <returns>A new <see cref="SecureString"/> containing the characters from <paramref name="source"/>; the caller must dispose the instance when finished.</returns>
        /// <remarks>No exceptions are thrown; a <see langword="null"/> input produces an empty secure string.</remarks>
        public static SecureString ToSecureString(this string source)
        {
            if (source == null)
            {
                // GUARD: SecureString conversion requires a valid source string.
                throw new ArgumentNullException(nameof(source));
            }

		  return new NetworkCredential("", source).SecurePassword;
       }

        
        /// <summary> The IsEqual function compares two SecureStrings for equality.
        /// It does this by using a HMACSHA256 hash to compare the strings, and then 
        /// overwrites the memory used by both strings with zeros before returning.</summary>
        /// <param name="ss1"> The first securestring to compare</param>
        /// <param name="ss2"> The second securestring to compare</param>
        /// <returns> A boolean value that indicates whether the two securestring objects are equal.</returns>
        public static bool IsEqual(this SecureString ss1, SecureString ss2)
        {
            if (ss1 == null)
            {
                // GUARD: Prevent null SecureString comparisons.
                throw new ArgumentNullException(nameof(ss1));
            }

            if (ss2 == null)
            {
                // GUARD: Prevent null SecureString comparisons.
                throw new ArgumentNullException(nameof(ss2));
            }

            return CompareSecureStringsWithHmac.IsEqual(ss1, ss2);
        }
    }
}