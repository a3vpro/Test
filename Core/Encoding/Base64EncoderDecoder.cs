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
    public class Base64EncoderDecoder
    {
        // Encripta una cadena
        /// <summary> The Encode function takes a string and returns the base64 encoded version of that string.</summary>
        /// <param name="source"> </param>
        /// <returns> A string.</returns>
        public string Encode(string source)
        {
            // GUARD: Ensure the source string is present before encoding.
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            string trimmedSource = source.Trim();

            if (trimmedSource.Length == 0)
            {
                throw new ArgumentException("Source string must not be empty or whitespace.", nameof(source));
            }

            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.ASCII.GetBytes(trimmedSource);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        /// <summary> The Decode function takes a string and converts it to ASCII.</summary>
        /// <param name="source"> The string to be decoded</param>
        /// <returns> A string</returns>
        public string Decode(string source)
        {
            // GUARD: Ensure the source string is present before decoding.
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            string trimmedSource = source.Trim();

            if (trimmedSource.Length == 0)
            {
                throw new ArgumentException("Source string must not be empty or whitespace.", nameof(source));
            }

            try
            {
                string result = string.Empty;
                byte[] decryted = Convert.FromBase64String(trimmedSource);
                result = System.Text.Encoding.ASCII.GetString(decryted);
                return result;
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Source string is not a valid Base64 value.", nameof(source), ex);
            }
        }
    }
}
