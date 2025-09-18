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
using System.Runtime.InteropServices;
using System.Security;
using Newtonsoft.Json;

namespace VisionNet.Core.Serialization
{
    /// <summary>
    /// Provides JSON serialization support for <see cref="SecureString"/> instances by converting
    /// them to and from string values while preserving their secure storage semantics.
    /// </summary>
    public class SecureStringConverter : JsonConverter<SecureString>
    {

        /// <summary>
        /// Overrides <see cref="JsonConverter{T}.WriteJson(JsonWriter, T, JsonSerializer)"/> to write a
        /// <see cref="SecureString"/> to JSON as an in-memory string value.
        /// </summary>
        /// <param name="writer">The JSON writer that receives the serialized characters.</param>
        /// <param name="value">The secure string to serialize; must not be <c>null</c>.</param>
        /// <param name="serializer">The serializer that orchestrates the operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> or <paramref name="value"/> is <c>null</c>.</exception>
        public override void WriteJson(JsonWriter writer, SecureString value, JsonSerializer serializer)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(value);
            writer.WriteValue(Marshal.PtrToStringBSTR(ptr));
            Marshal.ZeroFreeBSTR(ptr);
        }

        /// <summary>
        /// Overrides <see cref="JsonConverter{T}.ReadJson(JsonReader, Type, T, bool, JsonSerializer)"/> to
        /// create a new <see cref="SecureString"/> instance from a JSON string value.
        /// </summary>
        /// <param name="reader">The JSON reader that supplies the string characters to convert.</param>
        /// <param name="objectType">The target type for deserialization; expected to be <see cref="SecureString"/>.</param>
        /// <param name="existingValue">An existing secure string instance; ignored by this converter.</param>
        /// <param name="hasExistingValue">Indicates whether <paramref name="existingValue"/> contains a value.</param>
        /// <param name="serializer">The serializer coordinating the operation.</param>
        /// <returns>A read-only <see cref="SecureString"/> containing the characters from the JSON value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="reader"/> is <c>null</c>.</exception>
        public override SecureString ReadJson(JsonReader reader, Type objectType, SecureString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string json_val = (string)reader.Value;
            SecureString s = new SecureString();

            if (json_val != null)
            {
                foreach (char c in json_val)
                {
                    s.AppendChar(c);
                }
            }
            s.MakeReadOnly();
            return s;
        }
    }
}