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
    public class SecureStringConverter : JsonConverter<SecureString>
    {
        
        /// <summary> The WriteJson function is used to convert a SecureString object into a string.
        /// The function does this by first converting the SecureString object into an IntPtr, then 
        /// converts that IntPtr to a BSTR (Basic String), and finally writes the value of that BSTR as 
        /// a string.</summary>
        /// <param name="writer"> The writer to write the json data to.</param>
        /// <param name="value"> The securestring to be serialized.</param>
        /// <param name="serializer"> The serializer.</param>
        /// <returns> A string.</returns>
        public override void WriteJson(JsonWriter writer, SecureString value, JsonSerializer serializer)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(value);
            writer.WriteValue(Marshal.PtrToStringBSTR(ptr));
            Marshal.ZeroFreeBSTR(ptr);
        }

        /// <summary> The ReadJson function is used to convert a JSON string into a SecureString object.
        /// The function takes in the following parameters:
        ///     reader - A JsonReader that reads from the JSON string.
        ///     objectType - The type of object being deserialized.  In this case, it's always going to be SecureString.
        ///     existingValue - An existing value of the target type (SecureString).  This parameter will always be null because we're creating new objects and not updating them with this converter class.  
        ///                    If you wanted to update an existing SecureString, you would need to pass in</summary>
        /// <param name="reader">  JsonReader that reads from the JSON string.</param>
        /// <param name="objectType"> The type of object being deserialized.  In this case, it's always going to be SecureString</param>
        /// <param name="existingValue">An existing value of the target type (SecureString).  This parameter will always be null because we're creating new objects and not updating them with this converter class.</param>
        /// <param name="hasexistingvalue"></param>
        /// <param name="serializer">The serializer.</param>
        /// <returns> A securestring object.</returns>
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