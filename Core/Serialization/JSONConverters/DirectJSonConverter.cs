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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace VisionNet.Core.Serialization
{
    /// <summary>
    /// Provides a Json.NET converter that directly populates instances of <typeparamref name="T"/> without
    /// invoking type-level converters.
    /// </summary>
    /// <typeparam name="T">The target type created and populated during serialization and deserialization.</typeparam>
    public class DirectJSonConverter<T> : JsonConverter
        where T: new()
    {
        // nuevo
        static readonly IContractResolver resolver = new NoTypeConverterContractResolver();
        class NoTypeConverterContractResolver : DefaultContractResolver
        {
            
            /// <summary> The CreateContract function is called by the JsonSerializer to create a contract for each type it encounters.
            /// This override allows us to return our own custom contract that will be used when serializing and deserializing objects of type T.</summary>
            /// <param name="objectType"> What is this?</param>
            /// <returns> A jsoncontract object.</returns>
            protected override JsonContract CreateContract(Type objectType)
            {
                if (typeof(T).IsAssignableFrom(objectType))
                {
                    var contract = this.CreateObjectContract(objectType);
                    contract.Converter = null; // Also null out the converter to prevent infinite recursion.
                    return contract;
                }
                return base.CreateContract(objectType);
            }
        }

        
        /// <summary>Determines whether this converter can handle the specified object type.</summary>
        /// <param name="objectType">The type to evaluate for compatibility with <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> when <paramref name="objectType"/> is assignable to <typeparamref name="T"/>; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }


        /// <summary>Deserializes the JSON representation into a new <typeparamref name="T"/> instance.</summary>
        /// <param name="reader">The reader providing the JSON content.</param>
        /// <param name="objectType">The expected object type for the resulting value.</param>
        /// <param name="existingValue">The existing value of the object being read, ignored by this converter.</param>
        /// <param name="serializer">The serializer responsible for populating the created instance.</param>
        /// <returns>A populated <typeparamref name="T"/> instance created from the JSON content.</returns>
        /// <exception cref="JsonReaderException">Thrown when the JSON content is invalid.</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            T item = new T();
            serializer.ContractResolver = resolver;
            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        /// <summary>Gets a value indicating that this converter supports writing JSON using the direct population contract resolver.</summary>
        /// <value><see langword="true"/>, enabling the converter to participate when serializing instances of <typeparamref name="T"/>.</value>
        public override bool CanWrite => true;


        /// <summary>Serializes the provided value using the converter's contract resolver.</summary>
        /// <param name="writer">The writer receiving the JSON output.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="serializer">The serializer performing the serialization process.</param>
        /// <exception cref="JsonSerializationException">Thrown when an error occurs while writing the JSON content.</exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver = resolver;
            serializer.Serialize(writer, value);
        }
    }
}
