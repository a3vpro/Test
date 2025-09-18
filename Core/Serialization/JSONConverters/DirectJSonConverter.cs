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
    /// Provides a Json.NET converter that performs direct serialization and deserialization for <typeparamref name="T"/> while bypassing any
    /// type converters that could introduce metadata or interfere with the raw JSON payload.
    /// </summary>
    /// <typeparam name="T">The concrete type that will be populated from and written to JSON when this converter is applied.</typeparam>
    /// <remarks>
    /// Apply this converter when the default Json.NET behavior would otherwise add type information or invoke registered type converters that are
    /// not desired for <typeparamref name="T"/> instances. The converter ensures that objects of compatible types are populated directly using the
    /// standard contract resolver configured in <see cref="JsonSerializer"/>.
    /// </remarks>
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

        
        /// <summary>
        /// Determines whether the converter supports the specified object type.
        /// </summary>
        /// <param name="objectType">The object type being queried for converter support.</param>
        /// <returns><see langword="true"/> when the type is <typeparamref name="T"/> or a derived type; otherwise, <see langword="false"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }


        /// <summary>
        /// Populates a new <typeparamref name="T"/> instance from the JSON provided by the <paramref name="reader"/> using the configured serializer.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> positioned at the start of the JSON object to deserialize.</param>
        /// <param name="objectType">The expected object type being created by the serializer.</param>
        /// <param name="existingValue">The current value of the object, which is ignored because the converter creates a new instance.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> that supplies settings and populates the instance.</param>
        /// <returns>A fully populated <typeparamref name="T"/> instance created from the JSON content.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            T item = new T();
            serializer.ContractResolver = resolver;
            serializer.Populate(jo.CreateReader(), item);

            return item;
        }

        /// <summary>
        /// Gets a value indicating whether the converter participates in writing JSON.
        /// </summary>
        /// <remarks>
        /// Always returns <see langword="true"/> so that <see cref="WriteJson(JsonWriter, object, JsonSerializer)"/> can serialize <typeparamref name="T"/>
        /// instances using the same contract resolver logic that is applied during deserialization.
        /// </remarks>
        public override bool CanWrite => true;


        /// <summary>
        /// Serializes an object to JSON using the supplied <paramref name="serializer"/> and the contract resolver configured for this converter.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> that receives the serialized JSON content.</param>
        /// <param name="value">The <typeparamref name="T"/> instance to serialize.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> responsible for writing JSON data.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ContractResolver = resolver;
            serializer.Serialize(writer, value);
        }
    }
}
