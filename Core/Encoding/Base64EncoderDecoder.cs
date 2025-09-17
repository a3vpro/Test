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

namespace VisionNet.Core.Encoding
{
    /// <summary>
    /// Provides helpers for converting plain text to and from Base64-encoded ASCII representations.
    /// </summary>
    public class Base64EncoderDecoder
    {
        /// <summary>
        /// Encodes the supplied text as a Base64 string using ASCII byte representation.
        /// </summary>
        /// <param name="source">The plain-text value to encode; must not be <c>null</c>.</param>
        /// <returns>The Base64-encoded ASCII string representation of <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <c>null</c>, because ASCII encoding requires a valid input string.</exception>
        public string Encode(string source)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.ASCII.GetBytes(source);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        /// <summary>
        /// Decodes the supplied Base64 string into its ASCII plain-text representation.
        /// </summary>
        /// <param name="source">The Base64-encoded string to decode; must contain valid Base64 data.</param>
        /// <returns>The ASCII string produced by decoding <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <c>null</c>, because Base64 conversion requires an input value.</exception>
        /// <exception cref="FormatException">Thrown when <paramref name="source"/> is not a valid Base64 string.</exception>
        public string Decode(string source)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(source);
            result = System.Text.Encoding.ASCII.GetString(decryted);
            return result;
        }
    }
}
