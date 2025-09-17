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
        public static string FromSecureString(this SecureString source)
        {
            if (source == null)
            {
                // GUARD: SecureString extensions must not accept null instances.
                throw new ArgumentNullException(nameof(source));
            }

            return new NetworkCredential("", source).Password;
        }

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